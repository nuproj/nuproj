using System;
using System.Linq;
using System.Threading.Tasks;

using NuProj.Tests.Infrastructure;

using Xunit;

namespace NuProj.Tests
{
    public class DependencyTests
    {
        [Fact]
        public async Task Dependency_Content_IsNotFiltered()
        {
            var package = await Scenario.RestoreAndBuildSinglePackage("Dependency_Content_IsNotFiltered");
            var expectedFileNames = new[]
            {
                @"content\jquery-2.1.1.js",
                @"lib\net45\ClassLibrary1.dll"
            };
            var files = package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }

        [Fact]
        public async Task Dependency_Tools_IsNotFiltered()
        {
            var package = await Scenario.RestoreAndBuildSinglePackage("Dependency_Tools_IsNotFiltered");
            var expectedFileNames = new[]
            {
                @"lib\net45\ClassLibrary1.dll",
                @"tools\Microsoft.CodeAnalysis.CSharp.Desktop.dll",
                @"tools\Microsoft.CodeAnalysis.CSharp.dll",
                @"tools\Microsoft.CodeAnalysis.Desktop.dll",
                @"tools\Microsoft.CodeAnalysis.dll",
            };
            var files = package.GetFiles().Select(f => f.Path);
            Assert.Equal(expectedFileNames, files);
        }
    }
}
