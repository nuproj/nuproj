using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using NuGet;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NuProj.Tasks;
using NuProj.Tests.Infrastructure;
using Xunit;

namespace NuProj.Tests
{
    public class GenerateNuSpecTests : IDisposable
    {
        private string _projectDirectory;

        public GenerateNuSpecTests()
        {
            var tempPath = Path.GetTempPath();
            var randomFileName = Path.GetRandomFileName();
            _projectDirectory = Path.Combine(tempPath, randomFileName);
            Directory.CreateDirectory(_projectDirectory);
        }

        public void Dispose()
        {
            Directory.Delete(_projectDirectory, true);
        }

        [Fact]
        public void Task_GenerateNuSpec_OverrideInputFileName()
        {
            var input = Assets.GetScenarioFilePath("Task_GenerateNuSpec", "NuGetPackage.nuspec");
            var output = Path.Combine(_projectDirectory, "OverrideInputFileName.nuspec");

            var target = new GenerateNuSpec();
            target.InputFileName = input;
            target.OutputFileName = output;
            target.Id = "NuGetPackage2";
            target.Version = "2.0.0";
            target.Title = "NuGetPackage2";
            target.Authors = "Pavol";
            target.RequireLicenseAcceptance = true;
            target.Description = "NuGetPackage2";
            target.ReleaseNotes = "Released! (Again)";
            target.Summary = "NuGetPackage2";
            target.Language = "sk-sk";
            target.ProjectUrl = "http://nuproj.net/changes";
            target.IconUrl = "http://placekitten.com/g/128/128";
            target.LicenseUrl = "http://nuproj.net/LICENSE/changes";
            target.Copyright = "Copyright © Pavol";
            target.Tags = "NuGetPackage2";
            target.DevelopmentDependency = false;


            var fileToAdd = Path.Combine(_projectDirectory, "SomeProject.dll");
            target.Files = new[] {
                new TaskItem(fileToAdd, new Dictionary<string, string>
                    {
                        {Metadata.FileSource, fileToAdd},
                        {Metadata.FileTarget, @"lib\net45"},
                    })
            };
            var result = target.Execute();

            target.OutputFileName = output;
            Assert.True(result);

            using (var stream = File.OpenRead(output))
            {
                var manifest = Manifest.ReadFrom(stream, false);
                Assert.Equal(target.Id, manifest.Metadata.Id);
                Assert.Equal(new NuGetVersion(target.Version), manifest.Metadata.Version);
                Assert.Equal(target.Title, manifest.Metadata.Title);
                Assert.Equal(new[] { target.Authors }, manifest.Metadata.Authors);
                Assert.Equal(target.RequireLicenseAcceptance, manifest.Metadata.RequireLicenseAcceptance);
                Assert.Equal(target.Description, manifest.Metadata.Description);
                Assert.Equal(target.ReleaseNotes, manifest.Metadata.ReleaseNotes);
                Assert.Equal(target.Summary, manifest.Metadata.Summary);
                Assert.Equal(target.Language, manifest.Metadata.Language);
                Assert.Equal(new Uri(target.ProjectUrl), manifest.Metadata.ProjectUrl);
                Assert.Equal(new Uri(target.IconUrl), manifest.Metadata.IconUrl);
                Assert.Equal(new Uri(target.LicenseUrl), manifest.Metadata.LicenseUrl);
                Assert.Equal(target.Copyright, manifest.Metadata.Copyright);
                Assert.Equal(target.Tags, manifest.Metadata.Tags);
                Assert.Equal(true, manifest.Metadata.DevelopmentDependency);

                var expectedFiles = new[] {
                    new ManifestFile
                    {
                        Source = "Readme.txt",
                        Target = "",
                        Exclude = "",
                    },
                    new ManifestFile
                    {
                        Source = fileToAdd,
                        Target = @"lib\net45",
                        Exclude = "",
                    },
                };

                Assert.Equal(expectedFiles,
                    manifest.Files,
                    ManifestFileComparer.Instance);
            }
        }

        [Fact]
        public void Task_GenerateNuSpec_UseInputFileName()
        {
            var input = Assets.GetScenarioFilePath("Task_GenerateNuSpec", "NuGetPackage.nuspec");
            var output = Path.Combine(_projectDirectory, "UseInputFileName.nuspec");

            var target = new GenerateNuSpec();
            target.InputFileName = input;
            target.OutputFileName = output;
            var result = target.Execute();

            target.OutputFileName = output;
            Assert.True(result);

            using (var stream = File.OpenRead(output))
            {
                var manifest = Manifest.ReadFrom(stream, false);
                Assert.Equal("NuGetPackage", manifest.Metadata.Id);
                Assert.Equal(new NuGetVersion(1, 0, 0), manifest.Metadata.Version);
                Assert.Equal("NuGetPackage", manifest.Metadata.Title);
                Assert.Equal(new[] { "Immo" }, manifest.Metadata.Authors);
                Assert.Equal(false, manifest.Metadata.RequireLicenseAcceptance);
                Assert.Equal("NuGetPackage", manifest.Metadata.Description);
                Assert.Equal("Released!", manifest.Metadata.ReleaseNotes);
                Assert.Equal("NuGetPackage", manifest.Metadata.Summary);
                Assert.Equal("en-us", manifest.Metadata.Language);
                Assert.Equal(new Uri("http://nuproj.net/"), manifest.Metadata.ProjectUrl);
                Assert.Equal(new Uri("http://placekitten.com/g/64/64"), manifest.Metadata.IconUrl);
                Assert.Equal(new Uri("http://nuproj.net/LICENSE/"), manifest.Metadata.LicenseUrl);
                Assert.Equal("Copyright © Immo", manifest.Metadata.Copyright);
                Assert.Equal("NuGetPackage", manifest.Metadata.Tags);
                Assert.Equal(true, manifest.Metadata.DevelopmentDependency);

                var expectedFrameworkAssemblies = new[] {
                    new FrameworkAssemblyReference("Microsoft.Build.Framework", new[] { NuGetFramework.AnyFramework })
                };

                var expectedDependencySets = new[] {
                    new PackageDependencyGroup(
                        NuGetFramework.AnyFramework, 
                        new[] { new PackageDependency("NuGet.Core", new VersionRange(new NuGetVersion(2, 8, 5))) })
                };

                var expectedReferenceSets = new[] {
                    new PackageReferenceSet(new[] {"NuGet.Core.dll"})
                };

                var expectedFiles = new[] {
                    new ManifestFile
                    {
                        Source = "Readme.txt",
                        Target = "",
                        Exclude = "",
                    }
                };

                Assert.Equal(expectedFrameworkAssemblies,
                    manifest.Metadata.FrameworkReferences,
                    ManifestFrameworkAssemblyComparer.Instance);

                Assert.Equal(expectedDependencySets,
                    manifest.Metadata.DependencyGroups);

                Assert.Equal(expectedReferenceSets,
                    manifest.Metadata.PackageAssemblyReferences,
                    ManifestReferenceSetComparer.Instance);

                Assert.Equal(expectedFiles,
                    manifest.Files,
                    ManifestFileComparer.Instance);
            }
        }

        [Fact]
        public void Task_GenerateNuSpec_PlaceholderDependencies()
        {
            var output = Path.Combine(_projectDirectory, "PlaceholderDependencies.nuspec");

            var target = new GenerateNuSpec();
            target.OutputFileName = output;
            target.Id = "PlaceHolderDependenciesTest";
            target.Version = "1.0.0";
            target.Title = "PlaceHolderDependenciesTest";
            target.Authors = "Nuproj";
            target.RequireLicenseAcceptance = true;
            target.Description = "PlaceHolderDependenciesTest";
            target.ReleaseNotes = "Testing";
            target.Summary = "PlaceHolderDependenciesTest";
            target.ProjectUrl = "http://nuproj.net/changes";
            target.IconUrl = "http://placekitten.com/g/128/128";
            target.LicenseUrl = "http://nuproj.net/LICENSE/changes";
            target.Copyright = "Copyright © Testing";
            target.Tags = "PlaceHolderDependenciesTest";
            target.DevelopmentDependency = false;

            target.Dependencies = new[]
            {
                new TaskItem("APackage", new Dictionary<string, string>
                {
                    { Metadata.TargetFramework, "portable-net45+win8" }
                }),

                new TaskItem("_._", new Dictionary<string, string>
                {
                    { Metadata.TargetFramework, "net45" }
                })
            };

            var result = target.Execute();
            Assert.True(result);

            using (var stream = File.OpenRead(output))
            {
                var manifest = Manifest.ReadFrom(stream, false);
                Assert.Equal(target.Id, manifest.Metadata.Id);
                Assert.Equal(new NuGetVersion(target.Version), manifest.Metadata.Version);
                Assert.Equal(target.Title, manifest.Metadata.Title);
                Assert.Equal(new[] { target.Authors }, manifest.Metadata.Authors);
                Assert.Equal(target.RequireLicenseAcceptance, manifest.Metadata.RequireLicenseAcceptance);
                Assert.Equal(target.Description, manifest.Metadata.Description);
                Assert.Equal(target.ReleaseNotes, manifest.Metadata.ReleaseNotes);
                Assert.Equal(target.Summary, manifest.Metadata.Summary);
                Assert.Equal(target.Language, manifest.Metadata.Language);
                Assert.Equal(new Uri(target.ProjectUrl), manifest.Metadata.ProjectUrl);
                Assert.Equal(new Uri(target.IconUrl), manifest.Metadata.IconUrl);
                Assert.Equal(new Uri(target.LicenseUrl), manifest.Metadata.LicenseUrl);
                Assert.Equal(target.Copyright, manifest.Metadata.Copyright);
                Assert.Equal(target.Tags, manifest.Metadata.Tags);
                Assert.Equal(false, manifest.Metadata.DevelopmentDependency);

                //compare dependencies
                Assert.Equal(2, manifest.Metadata.DependencyGroups.Count());

                foreach (var dependencySet in manifest.Metadata.DependencyGroups)
                {
                    if (dependencySet.TargetFramework.GetShortFolderName() == "net45")
                    {
                        Assert.Equal(0, dependencySet.Packages.Count());
                    }
                    else if (dependencySet.TargetFramework.GetShortFolderName() == "portable-net45+win8")
                    {
                        Assert.Equal(1, dependencySet.Packages.Count());
                        Assert.Equal("APackage", dependencySet.Packages.First().Id);
                    }
                    else
                    {
                        // unexpected dependency set
                        Assert.True(false);
                    }

                }
            }
        }
    }
}