using System;
using System.Linq;

using NuGet;

using Microsoft.Build.Evaluation;

namespace NuProj.Tests
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
