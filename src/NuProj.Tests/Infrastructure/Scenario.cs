using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using NuGet;
using Xunit.Abstractions;

namespace NuProj.Tests.Infrastructure
{
    public static class Scenario
    {
        /// <summary>
        /// Executes package restore and builds the test solution.
        /// </summary>
        /// <param name="scenarioName">The name of the scenario to build. If omitted, it will be the name of the calling method.</param>
        /// <param name="packageId">The leaf name of the project to be built or rebuilt, and the package ID to return after the build.</param>
        /// <param name="properties">Build properties to pass to MSBuild.</param>
        /// <returns>The single built package, or the package whose ID matches <paramref name="packageId"/>.</returns>
        public static async Task<IPackage> RestoreAndBuildSinglePackageAsync([CallerMemberName] string scenarioName = null, string packageId = null, IDictionary<string, string> properties = null, ITestOutputHelper testLogger = null)
        {
            var packages = await RestoreAndBuildPackagesAsync(scenarioName, packageId, properties, testLogger);
            return packageId == null
                    ? packages.Single()
                    : packages.Single(p => string.Equals(p.Id, packageId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Executes package restore and builds the test solution.
        /// </summary>
        /// <param name="scenarioName">The name of the scenario to build. If omitted, it will be the name of the calling method.</param>
        /// <param name="projectName">The leaf name of the project to be built or rebuilt.</param>
        /// <param name="properties">Build properties to pass to MSBuild.</param>
        /// <returns>A collection of the built packages.</returns>
        public static async Task<IReadOnlyCollection<IPackage>> RestoreAndBuildPackagesAsync([CallerMemberName] string scenarioName = null, string projectName = null, IDictionary<string, string> properties = null, ITestOutputHelper testLogger = null)
        {
            var projectFullPath = Assets.GetScenarioSolutionPath(scenarioName);

            var projectDirectory = Path.GetDirectoryName(projectFullPath);
            await NuGetHelper.RestorePackagesAsync(projectDirectory);

            var result = await MSBuild.RebuildAsync(projectFullPath, projectName, properties, testLogger);
            result.AssertSuccessfulBuild();

            return NuPkg.GetPackages(projectDirectory);
        }

        /// <summary>
        /// Executes package restore and builds the test solution, asserting build failure.
        /// </summary>
        /// <param name="scenarioName">The name of the scenario to build. If omitted, it will be the name of the calling method.</param>
        /// <param name="projectName">The leaf name of the project to be built or rebuilt.</param>
        /// <param name="properties">Build properties to pass to MSBuild.</param>
        public static async Task RestoreAndFailBuildAsync([CallerMemberName] string scenarioName = null, string projectName = null, IDictionary<string, string> properties = null, ITestOutputHelper testLogger = null)
        {
            var projectFullPath = Assets.GetScenarioSolutionPath(scenarioName);

            var projectDirectory = Path.GetDirectoryName(projectFullPath);
            await NuGetHelper.RestorePackagesAsync(projectDirectory);

            var result = await MSBuild.RebuildAsync(projectFullPath, projectName, properties, testLogger);

            result.AssertUnsuccessfulBuild();
        }
    }
}