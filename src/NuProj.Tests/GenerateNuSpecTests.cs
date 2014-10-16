using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class GenerateNuSpecTests
    {
        [Fact]
        public void Task_GenerateNuSpec_UseInputFileName()
        {
            var nuspec = Assets.GetScenarioFilePath("Task_GenerateNuSpec", "NuGetPackage.nuspec");

            var target = new GenerateNuSpec();
        }
        
        [Fact]
        public void Task_GenerateNuSpec_OverrideInputFileName()
        {

        }
    }
}
