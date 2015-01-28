using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NuProj.Tests.Infrastructure;

using Xunit;

namespace NuProj.Tests
{
    public class IncrementalBuildTests
    {
        [Fact]
        public async Task IncrementalBuild_NuSpecIsNotUpdated_WhenNothingChanged()
        {
            var solutionPath = Assets.GetScenarioSolutionPath("IncrementalBuild_NuSpecIsNotUpdated");
            var projectPath = Path.GetDirectoryName(solutionPath);
            var nuspecFile = Path.Combine(projectPath, @"NuGetPackage\obj\Debug\NuGetPackage.nuspec");

            // Perform first build

            var result1 = await MSBuild.ExecuteAsync(solutionPath, "Build");
            result1.AssertSuccessfulBuild();

            // Get file stamp of the first nuspec file
            //
            // NOTE: We're asserting that the file exists because otherwise if the file doesn't
            //       exist FileInfo will simply return a placeholder value.

            var fileInfo1 = new FileInfo(nuspecFile);
            Assert.True(fileInfo1.Exists);
            
            var lastWriteTime1 = fileInfo1.LastWriteTimeUtc;

            // Wait for short period

            await Task.Delay(TimeSpan.FromMilliseconds(300));

            // Perform second build

            var result2 = await MSBuild.ExecuteAsync(solutionPath, "Build");
            result2.AssertSuccessfulBuild();

            // Get file stamp of the nuspec file for the second build

            var fileInfo2 = new FileInfo(nuspecFile);
            Assert.True(fileInfo2.Exists);

            var lastWriteTime2 = fileInfo2.LastWriteTimeUtc;

            // The file stamps should match

            Assert.Equal(lastWriteTime1, lastWriteTime2);
        }

        [Fact]
        public async Task IncrementalBuild_NuSpecIsUpdated_WhenGlobalPropertiesChange()
        {
            var solutionPath = Assets.GetScenarioSolutionPath("IncrementalBuild_NuSpecIsUpdated");
            var projectPath = Path.GetDirectoryName(solutionPath);

            const string expectedDescription1 = "First";
            const string expectedDescription2 = "Second";

            // Perform first build

            var properties = MSBuild.Properties.Default.Add("Description", expectedDescription1);
            var result1 = await MSBuild.ExecuteAsync(solutionPath, "Build", properties);
            result1.AssertSuccessfulBuild();

            var package1 = NuPkg.GetPackages(projectPath).Single();
            var actualDescription1 = package1.Description;

            // Perform second build

            var modifiedProperties = properties.SetItem("Description", expectedDescription2);
            var result2 = await MSBuild.ExecuteAsync(solutionPath, "Build", modifiedProperties);
            result2.AssertSuccessfulBuild();

            var package2 = NuPkg.GetPackages(projectPath).Single();
            var actualDescription2 = package2.Description;

            Assert.Equal(expectedDescription1, actualDescription1);
            Assert.Equal(expectedDescription2, actualDescription2);
        }
    }
}