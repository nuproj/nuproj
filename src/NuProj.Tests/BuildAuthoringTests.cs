using System;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;
using System.Linq;

namespace NuProj.Tests
{
    public class BuildAuthoringTests
    {
        [Fact]
        public async Task BuildAuthoring_Version_IsDeterminedAtBuildTime()
        {
            var package = await Scenario.RestoreAndBuildSinglePackageAsync();
            Assert.Equal(new Version(2, 0, 3, 0), package.Version.Version);
        }

        [Fact]
        public async Task BuildAuthoring_P2P_ResolvedPathHasComputedVersion()
        {
            var packages = await Scenario.RestoreAndBuildPackagesAsync();
            var package = packages.Single(p => p.Id == "NuProjReferencingOtherNuProj");
            Assert.Equal(new Version("2.0.3.0"), package.DependencySets.Single().Dependencies.Single().VersionSpec.MinVersion.Version);
        }
    }
}
