using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

//using NuGet;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace NuProj.Tests
{
    public class Transitivity
    {
        [Theory]
        [InlineData(@"Transitivity", @"Transitivity.sln", "Debug", "Any CPU")]
        [InlineData(@"Transitivity", @"A.nuget\A.nuget.nuproj", "Debug", "AnyCPU")]
        public void MSBuildDependencyTransitivityTest(string scenarioName, string projectToBuild, string configuration, string platform)
        {
            // Arange

            // by convention, all scenarios should be in directory
            string solutionDir = NuGetHelper.GetScenarioDirectory(scenarioName);

            var errorLogger = new ErrorLogger();

            List<ILogger> loggers = new List<ILogger>()
            {
                errorLogger
            };

            NuGetHelper.RestorePackages(solutionDir);

            var projectPath = Path.Combine(solutionDir, projectToBuild);

            string[] targetsToBuild = new[] { "Rebuild" };

            var props = new Dictionary<string, string>()
            {
                {"Configuration", configuration},
                {"Platform", platform},
            };

            var parameters = new BuildParameters();
            parameters.Loggers = loggers;

            // Act
            BuildResult result;
            using (var buildManager = new BuildManager())
            {
                BuildRequestData requestData = new BuildRequestData(projectPath, props, "4.0", targetsToBuild, null);

                result = buildManager.Build(parameters, requestData);
            }

            // Assert
            Assert.Equal(result.OverallResult, BuildResultCode.Success);
            Assert.Empty(errorLogger.Errors);

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