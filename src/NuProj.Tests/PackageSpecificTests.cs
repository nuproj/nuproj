using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class PackageSpecificTests
    {
        [Fact]
        public async Task PackageSpecific_MicrosoftBclBuild()
        {
            var package = await Scenario.RestoreAndBuildPackagesAsync(properties: MSBuild.Properties.BuildingInsideVisualStudio);
        }
    }
}
