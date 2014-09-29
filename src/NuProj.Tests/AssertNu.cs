namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Xunit;

    /// <summary>
    /// NuProj specific assertions.
    /// </summary>
    public static class AssertNu
    {
        /// <summary>
        /// Asserts that a build completed successfully and without logging any errors.
        /// </summary>
        public static void SuccessfulBuild(MSBuild.BuildResultAndLogs result)
        {
            result.AssertSuccessfulBuild();
        }

        /// <summary>
        /// Asserts that a NuPkg archive contains a file with the specified relative path.
        /// </summary>
        /// <param name="nupkgPath">The path to the zip archive.</param>
        /// <param name="packedFile">The path to the file that should exist in the archive. Case sensitive.</param>
        public static void PackageContains(string nupkgPath, string packedFile)
        {
            using (var archive = NuPkg.GetArchive(nupkgPath))
            {
                Assert.NotNull(archive.GetEntry(packedFile));
            }
        }

        public static void PackageContainsContentItems(Project nuProj)
        {
            using (var archive = NuPkg.GetArchive(nuProj))
            {
                foreach (var contentItem in nuProj.GetItems("Content"))
                {
                    string expectedPath = GetExpectedPackagePathForContent(contentItem);
                    Assert.NotNull(archive.GetEntry(expectedPath));
                }
            }
        }

        private static string GetExpectedPackagePathForContent(ProjectItem item)
        {
            // TODO: account for items with Link metadata.
            return item.EvaluatedInclude;
        }
    }
}
