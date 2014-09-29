using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Build.Framework;

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
            var result = await MSBuild.ExecuteAsync(projectPath, target);

            // Assert
            result.AssertSuccessfulBuild();
            Assert.Single(result.Result.ResultsByTarget[target].Items);
            Assert.Equal(result.Result.ResultsByTarget[target].Items[0].ItemSpec, identity);
        }
    }
}
