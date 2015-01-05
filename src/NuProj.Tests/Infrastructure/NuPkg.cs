using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Evaluation;

using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public static class NuPkg
    {
        public static IPackage GetPackage(this Project nuProj)
        {
            var nuPkgPath = nuProj.GetNuPkgPath();
            return GetPackage(nuPkgPath);
        }

        public static IPackage GetPackage(string nupkgPath)
        {
            return new OptimizedZipPackage(nupkgPath);
        }

        public static IPackageFile GetFile(this IPackage package, string effectivePath)
        {
            return package.GetFiles().SingleOrDefault(f => string.Equals(f.EffectivePath, effectivePath, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyCollection<IPackage> GetPackages(string projectDirectory)
        {
            return Directory.GetFiles(projectDirectory, "*.nupkg", SearchOption.AllDirectories)
                            .Where(f => !IsSymbolPackage(f) && !IsExternalPackage(f))
                            .Select(GetPackage)
                            .ToArray();
        }

        private static bool IsSymbolPackage(string f)
        {
            return f.EndsWith("symbols.nupkg", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsExternalPackage(string f)
        {
            return f.IndexOf(@"\packages\", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
