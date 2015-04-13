using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public static class Scenario
    {
        public static async Task<IPackage> RestoreAndBuildSinglePackage(string scenarioName, string packageId = null, IDictionary<string, string> properties = null)
        {
            var packages = await RestoreAndBuildPackages(scenarioName, packageId, properties);
            return packageId == null
                    ? packages.Single()
                    : packages.Single(p => string.Equals(p.Id, packageId, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<IReadOnlyCollection<IPackage>> RestoreAndBuildPackages(string scenarioName, string packageId, IDictionary<string, string> properties = null)
        {
            var projectFullPath = Assets.GetScenarioSolutionPath(scenarioName);

            var projectDirectory = Path.GetDirectoryName(projectFullPath);
            await NuGetHelper.RestorePackagesAsync(projectDirectory);

            var result = await MSBuild.RebuildAsync(projectFullPath, packageId, properties);
            result.AssertSuccessfulBuild();

            return NuPkg.GetPackages(projectDirectory);
        }

        public static async Task RestoreAndFailBuild(string scenarioName, string packageId, IDictionary<string, string> properties = null)
        {
            var projectFullPath = Assets.GetScenarioSolutionPath(scenarioName);

            var projectDirectory = Path.GetDirectoryName(projectFullPath);
            await NuGetHelper.RestorePackagesAsync(projectDirectory);

            var result = await MSBuild.RebuildAsync(projectFullPath, packageId, properties);

            result.AssertUnsuccessfulBuild();

        }
    }
}