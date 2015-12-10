namespace NuProj.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NuProj.Tests.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class ExtensibilityTests
    {
        private ITestOutputHelper logger;

        public ExtensibilityTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public async Task Extensibility_LoadsDotUserFile()
        {
            var projectPath = Assets.GetScenarioFilePath(
                "Extensibility_LoadsDotUserFile",
                @"NuGetPackage\NuGetPackage.nuproj");
            var properties = MSBuild.Properties.Default;
            var result = await MSBuild.ExecuteAsync(projectPath, "Rebuild", properties, this.logger);
            result.AssertSuccessfulBuild();

            var warnings = result.WarningEvents.First();
            Assert.Equal("NuGetPackage.nuproj.user target imported", warnings.Message);
        }
    }
}