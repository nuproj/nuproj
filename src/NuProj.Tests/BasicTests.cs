namespace NuProj.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Xunit;

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

            string expectedNuPkgFileName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.nupkg", nuproj.GetPropertyValue("Id"), nuproj.GetPropertyValue("Version"));
            string actualNuPkgPath = nuproj.GetNuPkgPath();
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
                XElement package = NuPkg.ExtractNuSpecFromPackage(nuproj);
                var metadata = package.Element(XName.Get("metadata", NuPkg.NuSpecXmlNamespace));

                var properties = new string[] { "id", "version", "authors", "requireLicenseAcceptance", "description", "projectUrl", "licenseUrl" };
                foreach (string propertyName in properties)
                {
                    XElement nuspecPropertyElement = metadata.Element(XName.Get(propertyName, NuPkg.NuSpecXmlNamespace));
                    string projectPropertyValue = nuproj.GetPropertyValue(propertyName);
                    if (nuspecPropertyElement == null)
                    {
                        Assert.True(string.IsNullOrEmpty(projectPropertyValue));
                    }
                    else
                    {
                        Assert.Equal(projectPropertyValue, nuspecPropertyElement.Value, StringComparer.OrdinalIgnoreCase);
                    }
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
