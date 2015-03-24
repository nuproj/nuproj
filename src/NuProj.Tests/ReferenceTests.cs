using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class ReferenceTests
    {
        [Fact]
        public async Task References_MultipleFrameworks_ReferenceAll()
        {
            var package = await Scenario.RestoreAndBuildSinglePackage("References_MultipleFrameworks", "ReferenceAll");
            var expectedFileNames = new[]
            {
                @"lib\net40\net40.dll",
                @"lib\net45\net40.dll",
                @"lib\net45\net45.dll",
                @"lib\net451\net40.dll",
                @"lib\net451\net45.dll",
                @"lib\net451\net451.dll",
                @"Readme.txt",
            };
            var files = package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }
        
        [Fact]
        public async Task References_MultipleFrameworks_ReferenceNet451()
        {
            var package = await Scenario.RestoreAndBuildSinglePackage("References_MultipleFrameworks", "ReferenceNet451");
            var expectedFileNames = new[]
            {
                @"lib\net451\net40.dll",
                @"lib\net451\net45.dll",
                @"lib\net451\net451.dll",
                @"Readme.txt",
            };
            var files = package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }

        [Theory]
        [InlineData("PackageToBuild", new[] { @"build\Tool.dll" })]
        [InlineData("PackageToLib", new[] { @"lib\net45\Tool.dll" })]
        [InlineData("PackageToRoot", new[] { @"Tool.dll", @"Tool.pdb" })]
        [InlineData("PackageToTools", new[] { @"tools\Tool.dll" })]
        [InlineData("PackageDependencyToTools", new[] { @"tools\Tool.dll" })]
        [InlineData("PackageClosureToTools", new[] { @"tools\Tool.dll", @"tools\ToolWithClosure.dll" })]
        public async Task References_PackageDirectory_ToolIsPackaged(string packageId, string[] expectedFiles)
        {
            var package = await Scenario.RestoreAndBuildSinglePackage("References_PackageDirectory", packageId);
            var files = package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFiles.OrderBy(x => x), files.OrderBy(x => x));
        }

        [Theory]
        [InlineData("PackageToContent")]
        public async Task References_PackageDirectory_Fails(string packageId)
        {
            await Scenario.RestoreAndFailBuild("References_PackageDirectory", packageId);
        }

    }
}
    
