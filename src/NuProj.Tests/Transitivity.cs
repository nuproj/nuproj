using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace NuProj.Tests
{
    public class Transitivity
    {
        [Theory]
        [InlineData(@"..\..\Transitivity\Transitivity.sln", "Debug", "Any CPU")]
        //[InlineData(@"c"..\..\Transitivity\B.nuget\A.nuget.nuproj", "Debug", "AnyCPU")]
        public void DependencyTransitivityTest(string msbuildFileName, string configuration, string platform)
        {
            // TODO: package restore for test
            
            var errorLogger = new ErrorLogger();

            List<ILogger> loggers = new List<ILogger>()
            {
                errorLogger
            };

            var msbuildFilePath = Path.GetFullPath(msbuildFileName);

            // get OutDir location of this project, that should contain all required files: .targets, .props, nuget.exe
            var nuProjPath = Directory.GetCurrentDirectory();

            string[] targetsToBuild = new[] { "Rebuild" };

            var props = new Dictionary<string, string>()
            {
                {"NuProjPath", nuProjPath},
                {"Configuration", configuration},
                {"Platform", platform},
                //{"OutDir", nuProjPath + "\\"}
            };

            using (var buildManager = new BuildManager())
            {
                var parameters = new BuildParameters();
                parameters.Loggers = loggers;
                BuildRequestData requestData = new BuildRequestData(msbuildFilePath, props, "12.0", targetsToBuild, null);

                BuildResult result = buildManager.Build(parameters, requestData);
                Assert.Equal(result.OverallResult, BuildResultCode.Success);
                Assert.Empty(errorLogger.Errors);
            }
        }
    }
}