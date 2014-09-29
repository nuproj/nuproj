using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Execution;

using NuProj.Tests.Infrastructure;

using Xunit;

namespace NuProj.Tests
{
    public class BasicTests
    {
        [Fact]
        public async Task ProjectTemplateCanBuild()
        {
            var nuproj = Assets.FromTemplate()
                               .AssignNuProjDirectory()
                               .ToProject()
                               .CreateMockContentFiles();
            try
            {
                var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());
                result.AssertSuccessfulBuild();
            }
            finally
            {
                ProjectBuilder.Cleanup(nuproj);
            }
        }

        [Fact]
        public void NuPkgFileNameBasedOnProjectName()
        {
            var nuproj = Assets.FromTemplate()
                               .AssignNuProjDirectory()
                               .ToProject();

            var expectedNuPkgFileName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.nupkg", nuproj.GetPropertyValue("Id"), nuproj.GetPropertyValue("Version"));
            var actualNuPkgPath = nuproj.GetNuPkgPath();
            Assert.Equal(expectedNuPkgFileName, Path.GetFileName(actualNuPkgPath));
        }

        [Fact]
        public async Task PackageIncludesContentFiles()
        {
            var nuproj = Assets.FromTemplate()
                               .AssignNuProjDirectory()
                               .ToProject()
                               .CreateMockContentFiles();
            try
            {
                var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());
                result.AssertSuccessfulBuild();
                AssertNu.PackageContainsContentItems(nuproj);
            }
            finally
            {
                ProjectBuilder.Cleanup(nuproj);
            }
        }

        [Fact]
        public async Task NuSpecPropertiesMatchProjectProperties()
        {
            var nuproj = Assets.FromTemplate()
                               .AssignNuProjDirectory()
                               .ToProject()
                               .CreateMockContentFiles();
            try
            {
                var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());
                result.AssertSuccessfulBuild();

                var package = nuproj.GetPackage();

                var properties = new Dictionary<string, object>
                {
                    {"Id", package.Id},
                    {"Version", package.Version},
                    {"Authors", package.Authors},
                    {"RequireLicenseAcceptance", package.RequireLicenseAcceptance},
                    {"Description", package.Description},
                    {"ProjectUrl", package.ProjectUrl},
                    {"LicenseUrl", package.LicenseUrl},
                };

                foreach (var property in properties)
                {
                    var propertyName = property.Key;
                    var expectedValueText = nuproj.GetPropertyValue(propertyName);
                    var actualValue = property.Value;
                    var actualValueText = actualValue == null
                                            ? string.Empty
                                            : actualValue is IEnumerable<string>
                                                ? ((IEnumerable<string>) actualValue).First()
                                                : actualValue.ToString();

                    Assert.Equal(expectedValueText, actualValueText);
                }
            }
            finally
            {
                ProjectBuilder.Cleanup(nuproj);
            }
        }

        [Fact]
        public async Task EmptyProjectCannotBuild()
        {
            var nuproj = Assets.FromTemplate()
                               .AssignNuProjDirectory()
                               .ToProject();

            try
            {
                // This test focuses on a completely empty project.
                nuproj.RemoveItems(nuproj.GetItems("Content"));

                var result = await MSBuild.ExecuteAsync(nuproj.CreateProjectInstance());

                // Verify that the build fails and tells the user why.
                Assert.Equal(BuildResultCode.Failure, result.Result.OverallResult);
                Assert.NotEqual(0, result.ErrorEvents.Count());
            }
            finally
            {
                ProjectBuilder.Cleanup(nuproj);
            }
        }
    }
}
