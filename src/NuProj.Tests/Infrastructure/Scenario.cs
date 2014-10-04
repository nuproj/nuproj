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
        public static async Task<IPackage> RestoreAndBuildSinglePackage(string scenarioName)
        {
            var packages = await RestoreAndBuildPackages(scenarioName);
            return packages.Single();
        }

        public static async Task<IReadOnlyCollection<IPackage>> RestoreAndBuildPackages(string scenarioName)
        {
            var projectFullPath = Assets.GetScenarioSolutionPath(scenarioName);

            var projectDirectory = Path.GetDirectoryName(projectFullPath);
            await NuGetHelper.RestorePackagesAsync(projectDirectory);
            
            var result = await MSBuild.RebuildAsync(projectFullPath);
            result.AssertSuccessfulBuild();

            return NuPkg.GetPackages(projectDirectory);
        }
    }
}