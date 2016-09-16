using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using NuGet;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;

namespace NuProj.Tasks
{
    public static class Extensions
    {
        public static bool GetBoolean(this ITaskItem taskItem, string metadataName, bool defaultValue = false)
        {
            bool result = false;
            var metadataValue = taskItem.GetMetadata(metadataName);
            bool.TryParse(metadataValue, out result);
            return result;
        }

        public static string GetMetadata(this ITaskItem taskItem, string metadataName, string defaultValue)
        {
            var metadataValue = taskItem.GetMetadata(metadataName);
            return string.IsNullOrEmpty(metadataValue) ? defaultValue : metadataValue;
        }

        public static NuGetFramework GetTargetFramework(this ITaskItem taskItem)
        {
            NuGetFramework result = null;
            var metadataValue = taskItem.GetMetadata(Metadata.TargetFramework);
            
            if (!string.IsNullOrEmpty(metadataValue))
            {
                result = NuGetFramework.Parse(metadataValue);
            }
            else
            {
                result = NuGetFramework.AgnosticFramework;
            }

            return result;
        }

        public static NuGetFramework GetTargetFrameworkMoniker(this ITaskItem taskItem)
        {
            NuGetFramework result = null;
            var metadataValue = taskItem.GetMetadata(Metadata.TargetFrameworkMoniker);
            if (!string.IsNullOrEmpty(metadataValue))
            {
                result = NuGetFramework.ParseFrameworkName(metadataValue, DefaultFrameworkNameProvider.Instance);
            }
            else
            {
                result = NuGetFramework.AnyFramework;
            }

            return result;
        }

        public static PackageDirectory GetPackageDirectory(this ITaskItem taskItem, PackageDirectory defaultValue = PackageDirectory.Root)
        {
            var packageDirectoryName = taskItem.GetMetadata(Metadata.PackageDirectory);
            if (string.IsNullOrEmpty(packageDirectoryName))
            {
                return defaultValue;
            }

            PackageDirectory result;
            Enum.TryParse(packageDirectoryName, true, out result);
            return result;
        }

        public static void GetTargetPackageDirectory(this ITaskItem taskItem, out PackageDirectory packageDirectory, out string directoryPath)
        {
            var fileTarget = taskItem.GetMetadata(Metadata.FileTarget) ?? string.Empty;
            var fileTargetParts = fileTarget.Split(new[] { '\\', '/' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (fileTargetParts.Length <= 1)
            {
                packageDirectory = PackageDirectory.Root;
                directoryPath = fileTarget;
                return;
            }

            Enum.TryParse(fileTargetParts[0], true, out packageDirectory);
            if (packageDirectory == PackageDirectory.Root)
            {
                directoryPath = fileTarget;
            }
            else
            {
                directoryPath = fileTargetParts[1];
            }
        }

        public static string GetTargetSubdirectory(this ITaskItem taskItem)
        {
            return  taskItem.GetMetadata(Metadata.TargetSubdirectory) ?? string.Empty;
        }

        public static VersionRange GetVersion(this ITaskItem taskItem)
        {
            var result = VersionRange.All;
            var metadataValue = taskItem.GetMetadata(Metadata.Version);
            if (!string.IsNullOrEmpty(metadataValue))
            {
                VersionRange.TryParse(metadataValue, out result);
            }

            return result;
        }

        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            return source;
        }

        public static string GetShortFrameworkName(this NuGetFramework frameworkName)
        {
            if (frameworkName == null || frameworkName == NuGetFramework.AnyFramework)
            {
                return null;
            }

            if (frameworkName.Equals(new NuGetFramework(FrameworkConstants.FrameworkIdentifiers.Portable, new Version(5, 0))))
            {
                // Avoid calling GetShortFrameworkName because NuGet throws ArgumentException
                // in this case.
                return "dotnet";
            }

            return frameworkName.GetShortFolderName();
        }

        public static string GetAnalyzersFrameworkName(this NuGetFramework frameworkName)
        {
            // At this time there is no host other than Roslyn compiler that can run analyzers. 
            // Therefore, Framework Name and Version should always be specified as 'dotnet' until another host is 
            // implemented that has runtime restrictions.
            return "dotnet";

        }

        public static string ToStringSafe(this object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public static void UpdateMember<T>(this T target, Expression<Func<T, Uri>> memberLamda, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }

            var setter = typeof(T).GetMethod($"Set{property.Name}");
            setter.Invoke(target, new[] { value });

        }

        public static void UpdateMember<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda, TValue value)
        {
            if (value == null)
            {
                return;
            }

            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }
            
            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }
            
            property.SetValue(target, value, null);
        }

        public static void AddRangeToMember<T, TItem>(this T target, Expression<Func<T, IEnumerable<TItem>>> memberLamda, IEnumerable<TItem> value)
        {
            if (value == null || value.Count() == 0)
            {
                return;
            }
            
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }

            var property = memberSelectorExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidOperationException("Invalid member expression.");
            }

            if (property.CanWrite)
            {
                var list = (ICollection<TItem>)property.GetValue(target) ?? new List<TItem>();
                list.AddRange(value);

                property.SetValue(target, list, null);
            }
            else
            {
                var list = (ICollection<TItem>)property.GetValue(target);
                list.AddRange(value);
            }

        }

        public static string Combine(this PackageDirectory packageDirectory, string targetFramework, string targetSubdirectory, string fileName)
        { 
            switch (packageDirectory)
            {
                case PackageDirectory.Root:
                    return Path.Combine(targetSubdirectory, fileName);
                case PackageDirectory.Content:
                    return Path.Combine(PackagingConstants.Folders.Content, targetSubdirectory, fileName);
                case PackageDirectory.ContentFiles:
                    return Path.Combine(PackagingConstants.Folders.ContentFiles, targetSubdirectory, fileName);
                case PackageDirectory.Build:
                    return Path.Combine(PackagingConstants.Folders.Build, targetSubdirectory, fileName);
                case PackageDirectory.Lib:
                    return Path.Combine(PackagingConstants.Folders.Lib, targetFramework, targetSubdirectory, fileName);
                case PackageDirectory.Tools:
                    return Path.Combine(PackagingConstants.Folders.Tools, targetSubdirectory, fileName);
                case PackageDirectory.Analyzers:
                    return Path.Combine(PackagingConstants.Folders.Analyzers, targetFramework, targetSubdirectory, fileName);
                default:
                    return fileName;
            }
        }
    }
}
