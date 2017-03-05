using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;

namespace NuProj.Tests.Infrastructure
{
    public static class Extensions
    {
        public static IEnumerable<string> Flatten(this IEnumerable<PackageDependencyGroup> dependencySets)
        {
            return from formattedDependency in
                       (from dependencySet in dependencySets
                        from dependency in dependencySet.Packages
                        select dependencySet.TargetFramework == null
                        ? dependency.ToString()
                        : string.Format("{0} ({1})", dependency, dependencySet.TargetFramework.GetShortFolderName()))
                   select formattedDependency;
        }

        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            return source;
        }
    }
}