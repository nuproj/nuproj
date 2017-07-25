using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;

using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace NuProj.Tests.Infrastructure
{
    public static class NuPkg
    {
        public static async Task<Package> GetPackageAsync(this Project nuProj)
        {
            string nuPkgPath = await nuProj.GetNuPkgPathAsync();
            return GetPackage(nuPkgPath);
        }

        public static Package GetPackage(string nupkgPath)
        {
            var result = new Package();
            using (var packageFile = File.Open(nupkgPath, FileMode.Open, FileAccess.Read))
            {
                using (var packageReader = new PackageArchiveReader(packageFile))
                {
                    var manifestReader = new NuspecReader(packageReader.GetNuspec());
                    result.Authors = manifestReader.GetAuthors();
                    result.DependencySets = manifestReader.GetDependencyGroups();
                    result.Description = manifestReader.GetDescription();
                    result.Id = manifestReader.GetIdentity().Id;
                    result.LicenseUrl = manifestReader.GetLicenseUrl();
                    result.ProjectUrl = manifestReader.GetProjectUrl();
                    result.Summary = manifestReader.GetSummary();
                    result.Version = manifestReader.GetIdentity().Version;
                    result.RequireLicenseAcceptance = manifestReader.GetRequireLicenseAcceptance();
                    result.Files = packageReader.GetFiles()
                        .Where(x => x != "[Content_Types].xml" &&
                                    x != "_rels/.rels" &&
                                    !x.EndsWith(".nuspec") &&
                                    !x.EndsWith(".psmdcp"))
                        .Select(x => x.Replace("/", "\\"))
                        .OrderBy(x => x).ToList();
                }
            }

            return result;
        }

        public static string GetFile(this Package package, string effectivePath)
        {
            return package.Files.SingleOrDefault(f => string.Equals(f, effectivePath, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyCollection<Package> GetPackages(string projectDirectory)
        {
            return Directory.GetFiles(projectDirectory, "*.nupkg", SearchOption.AllDirectories)
                            .Where(f => !IsSymbolPackage(f) && !IsExternalPackage(f))
                            .Where(f=> !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
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
