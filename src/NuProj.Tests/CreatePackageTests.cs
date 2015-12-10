using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class CreatePackageTests
    {
        [Fact]
        public async Task CreatePackage_LibraryWithXmlDoc_ShouldNotFail()
        {
            var solutionPath = Assets.GetScenarioSolutionPath("CreatePackage_LibraryWithXmlDoc_ShouldNotFail");
            var result = await MSBuild.RebuildAsync(solutionPath);

            result.AssertSuccessfulBuild();
        }

        [Fact]
        public async Task CreatePackage_SubstituteMacros()
        {
            var properties = MSBuild.Properties.Default
                .Add("NuSpecProperties", "SummaryMacro=TestSummary");
            var package = await Scenario.RestoreAndBuildSinglePackageAsync(properties: properties);
            Assert.Equal("TestSummary", package.Summary);
        }
    }
}
