using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Xunit;

namespace NuProj.Tests
{
    public class Transitivity
    {
        [Theory]
        [InlineData(@"Transitivity", @"Transitivity.sln")]
        [InlineData(@"Transitivity", @"A.nuget\A.nuget.nuproj")]
        public async Task MSBuildDependencyTransitivityTest(string scenarioName, string projectToBuild)
        {
            // Arange

            // by convention, all scenarios should be in directory
            string solutionDir = NuGetHelper.GetScenarioDirectory(scenarioName);

            await NuGetHelper.RestorePackagesAsync(solutionDir);

            var projectPath = Path.Combine(solutionDir, projectToBuild);

            // Act
            var result = await MSBuild.ExecuteAsync(projectPath);

            // Assert
            Assert.Equal(BuildResultCode.Success, result.Result.OverallResult);
            Assert.Equal(0, result.LogEvents.OfType<BuildErrorEventArgs>().Count());

            var packagePath = Path.Combine(solutionDir, @"A.nuget\bin\Debug\A.1.0.0.nupkg");
            Assert.True(File.Exists(packagePath));
            var package = new NuGet.OptimizedZipPackage(packagePath);
            var files = package.GetFiles();

            Assert.None(files, x => x.Path.Contains("Newtonsoft.Json.dll"));
            Assert.None(files, x => x.Path.Contains("ServiceModel.Composition.dll"));
        }
    }
}