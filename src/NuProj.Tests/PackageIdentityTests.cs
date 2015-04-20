using System;
using System.IO;
using System.Threading.Tasks;

using NuProj.Tests.Infrastructure;

using Xunit;

namespace NuProj.Tests
{
    public class PackageIdentityTests
    {
        [Theory]
        [InlineData("Dependency_IndirectDependencies_AreNotPackaged", @"A.nuget\A.nuget.nuproj", "A.nuget")]
        public async Task PackageIdentity_GetPackageIdentity_ResturnsCorrectValue(string scenarioName, string projectToBuild, string identity)
        {
            const string target = "GetPackageIdentity";

            // Arange
            // by convention, all scenarios should be in directory
            var solutionDir = Assets.GetScenarioDirectory(scenarioName);

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
