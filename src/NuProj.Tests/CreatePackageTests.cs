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
            var solutionPath = Assets.GetScenarioSolutionPath("Dependency_NoDependencies_Fails");
            var result = await MSBuild.RebuildAsync(solutionPath);

            result.AssertSuccessfulBuild();
        }
    }
}
