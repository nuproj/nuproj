using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NuProj.Tests
{
    public class GetPackageIdentity
    {
        [Theory]
        [InlineData("Transitivity", @"A.nuget\A.nuget.nuproj", "A")]
        public async Task ExecuteGetPackageIdentityTarget(string scenarioName, string projectToBuild, string identity)
        {
            const string target = "GetPackageIdentity";

            // Arange
            // by convention, all scenarios should be in directory
            string solutionDir = NuGetHelper.GetScenarioDirectory(scenarioName);

            var projectPath = Path.Combine(solutionDir, projectToBuild);

            // Act
            BuildResult result = await MSBuild.ExecuteAsync(projectPath, target, err => Assert.False(true, "Error logged."));

            // Assert
            Assert.Equal(result.OverallResult, BuildResultCode.Success);
            Assert.Single(result.ResultsByTarget[target].Items);
            Assert.Equal(result.ResultsByTarget[target].Items[0].ItemSpec, identity);
        }
    }
}
