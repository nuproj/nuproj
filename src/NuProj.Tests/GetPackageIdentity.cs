using System;
using System.IO;
using System.Threading.Tasks;

using NuProj.Tests.Infrastructure;

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
            var solutionDir = NuGetHelper.GetScenarioDirectory(scenarioName);

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
