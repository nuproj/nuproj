using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
//using NuGet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace NuProj.Tests
{
    public class Transitivity
    {
        [Theory]
        [InlineData(@"Transitivity", @"Transitivity.sln", "Debug", "Any CPU")]
        [InlineData(@"Transitivity", @"A.nuget\A.nuget.nuproj", "Debug", "AnyCPU")]
        public void DependencyTransitivityTest(string scenarioName, string projectToBuild, string configuration, string platform)
        {
            // Arange
            
            // get testOutDir location of this project, that should contain all required files: .targets, .props, nuget.exe
            var testOutDir = Directory.GetCurrentDirectory();
            
            // by convention, all scenarios should be in directory 
            string solutionDir = TestsHelper.GetScenarioDirectory(scenarioName);

            var errorLogger = new ErrorLogger();

            List<ILogger> loggers = new List<ILogger>()
            {
                errorLogger
            };

            TestsHelper.RestorePackages(solutionDir);

            var projectPath = Path.Combine(solutionDir, projectToBuild);

            string[] targetsToBuild = new[] { "Rebuild" };

            var props = new Dictionary<string, string>()
            {
                {"NuProjPath", testOutDir},
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
    }
}