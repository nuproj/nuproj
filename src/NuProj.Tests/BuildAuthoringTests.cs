using System;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;
using System.Linq;
using NuGet.Versioning;

namespace NuProj.Tests
{
    public class BuildAuthoringTests
    {
        [Fact]
        public async Task BuildAuthoring_Version_IsDeterminedAtBuildTime()
        {
            var package = await Scenario.RestoreAndBuildSinglePackageAsync();
            Assert.Equal(new NuGetVersion(2, 0, 3), package.Version);
        }

        [Fact]
        public async Task BuildAuthoring_P2P_ResolvedPathHasComputedVersion()
        {
            var packages = await Scenario.RestoreAndBuildPackagesAsync();
            var package = packages.Single(p => p.Id == "NuProjReferencingOtherNuProj");
            Assert.Equal(new NuGetVersion(2, 0, 3), package.DependencySets.Single().Packages.Single().VersionRange.MinVersion);
        }
    }
}
