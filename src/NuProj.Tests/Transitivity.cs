using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

//using NuGet;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System.Threading.Tasks;

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

            NuGetHelper.RestorePackages(solutionDir);

            var projectPath = Path.Combine(solutionDir, projectToBuild);

            // Act
            BuildResult result = await MSBuild.ExecuteAsync(projectPath, err => Assert.False(true, "Error logged."));

            // Assert
            Assert.Equal(result.OverallResult, BuildResultCode.Success);

            var packagePath = Path.Combine(solutionDir, @"A.nuget\bin\Debug\A.1.0.0.nupkg");
            Assert.True(File.Exists(packagePath));
            var package = new NuGet.OptimizedZipPackage(packagePath);
            var files = package.GetFiles();

            Assert.None(files, x => x.Path.Contains("Newtonsoft.Json.dll"));
            Assert.None(files, x => x.Path.Contains("ServiceModel.Composition.dll"));
        }


        //[Theory]
        //[InlineData(@"Transitivity", @"Transitivity.sln", "Debug", "Any CPU")]
        public void DevEnvDependencyTransitivityTest(string scenarioName, string projectToBuild, string configuration, string platform)
        {
            string solutionDir = NuGetHelper.GetScenarioDirectory(scenarioName);
            var projectPath = Path.Combine(solutionDir, projectToBuild);

            NuGetHelper.RestorePackages(solutionDir);

            using (var devEnv = new DevEnv())
            {
                devEnv.OpenSolution(projectPath);
                devEnv.ActivateConfiguration(configuration, platform);
                devEnv.Clean();
                devEnv.Build();
            }
        }
    }
}