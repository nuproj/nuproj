using System;
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
    }
}
