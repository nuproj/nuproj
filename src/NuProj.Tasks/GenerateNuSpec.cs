using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet;

namespace NuProj.Tasks
{
    public class GenerateNuSpec : Task
    {
        private const string NuSpecXmlNamespace = @"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

        [Required]
        public string OutputFileName { get; set; }

        public string MinClientVersion { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public string Version { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Authors { get; set; }

        [Required]
        public string Owners { get; set; }

        [Required]
        public string Description { get; set; }

        public string ReleaseNotes { get; set; }

        public string Summary { get; set; }

        public string Language { get; set; }

        public string ProjectUrl { get; set; }

        public string IconUrl { get; set; }

        public string LicenseUrl { get; set; }

        public string Copyright { get; set; }

        public bool RequireLicenseAcceptance { get; set; }

        public bool DevelopmentDependency { get; set; }

        public string Tags { get; set; }

        public ITaskItem[] Dependencies { get; set; }

        public ITaskItem[] References { get; set; }

        public ITaskItem[] FrameworkReferences { get; set; }

        public ITaskItem[] Files { get; set; }

        public override bool Execute()
        {
            try
            {
                WriteNuSpecFile();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                Log.LogErrorFromException(ex);
            }

            return !Log.HasLoggedErrors;
        }

        private void WriteNuSpecFile()
        {
            var manifest = CreateManifest();

            if (!IsDifferent(manifest))
            {
                Log.LogMessage("Skipping generation of .nuspec because contents are identical.");
                return;
            }

            var directory = Path.GetDirectoryName(OutputFileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var file = File.OpenWrite(OutputFileName))
            {
                manifest.Save(file, false);
            }
        }

        private bool IsDifferent(Manifest newManifest)
        {
            if (!File.Exists(OutputFileName))
                return true;

            var oldSource = File.ReadAllText(OutputFileName);
            var newSource = "";
            using (var stream = new MemoryStream())
            {
                newManifest.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                newSource = Encoding.UTF8.GetString(stream.ToArray());
            }

            return oldSource != newSource;
        }

        private Manifest CreateManifest()
        {
            var manifestMetadata = new ManifestMetadata();
            manifestMetadata.Authors = Authors;
            manifestMetadata.Copyright = Copyright;
            manifestMetadata.DependencySets = GetDependencySets();
            manifestMetadata.Description = Description;
            manifestMetadata.DevelopmentDependency = DevelopmentDependency;
            manifestMetadata.FrameworkAssemblies = GetFrameworkAssemblies();
            manifestMetadata.IconUrl = IconUrl;
            manifestMetadata.Id = Id;
            manifestMetadata.Language = Language;
            manifestMetadata.LicenseUrl = LicenseUrl;
            manifestMetadata.MinClientVersionString = MinClientVersion;
            manifestMetadata.Owners = Owners;
            manifestMetadata.ProjectUrl = ProjectUrl;
            manifestMetadata.ReferenceSets = GetReferenceSets();
            manifestMetadata.ReleaseNotes = ReleaseNotes;
            manifestMetadata.RequireLicenseAcceptance = RequireLicenseAcceptance;
            manifestMetadata.Summary = Summary;
            manifestMetadata.Tags = Tags;
            manifestMetadata.Title = Title;
            manifestMetadata.Version = Version;

            var manifest = new Manifest()
            {
                Metadata = manifestMetadata,
                Files = GetMainfiestFiles()
            };

            return manifest;
        }

        private List<ManifestFile> GetMainfiestFiles()
        {
            return (from f in Files.NullAsEmpty()
                    select new ManifestFile
                    {
                        Source = f.GetMetadata("FullPath"),
                        Target = f.GetMetadata("TargetPath"),
                        Exclude = f.GetMetadata("Exclude"),
                    }).ToList();
        }

        private List<ManifestFrameworkAssembly> GetFrameworkAssemblies()
        {
            return (from fr in FrameworkReferences.NullAsEmpty()
                    select new ManifestFrameworkAssembly
                    {
                        AssemblyName = fr.ItemSpec,
                        TargetFramework = fr.GetFrameworkName("TargetFramework").GetShortFrameworkName(),
                    }).ToList();
        }

        private List<ManifestDependencySet> GetDependencySets()
        {
            var dependencies = from d in Dependencies.NullAsEmpty()
                               select new PackageReference(
                                   d.ItemSpec,
                                   d.GetVersion("Version"),
                                   d.GetVersionSpec("AllowedVersions"),
                                   d.GetFrameworkName("TargetFramework"),
                                   d.GetBoolean("DevelopmentDependency"),
                                   d.GetBoolean("RequireReinstallation"));

            return (from dependency in dependencies
                    where !dependency.IsDevelopmentDependency
                    group dependency by dependency.TargetFramework into dependenciesByFramework
                    select new ManifestDependencySet
                    {
                        TargetFramework = dependenciesByFramework.Key.GetShortFrameworkName(),
                        Dependencies = (from dependency in dependenciesByFramework
                                        group dependency by dependency.Id into dependenciesById
                                        select new ManifestDependency
                                        {
                                            Id = dependenciesById.Key,
                                            Version = ToVersion(dependenciesById.Aggregate(AggregateVersions))
                                        }).ToList()
                    }).ToList();
        }

        private string ToVersion(PackageReference packageReference)
        {
            if (packageReference.VersionConstraint != null)
            {
                return packageReference.VersionConstraint.ToString();
            }
            else if (packageReference.Version != null)
            {
                return packageReference.Version.ToString();
            }
            else
            {
                return null;
            }
        }

        private List<ManifestReferenceSet> GetReferenceSets()
        {
            var references = from r in References.NullAsEmpty()
                             select new
                             {
                                 File = r.ItemSpec,
                                 TargetFramework = r.GetFrameworkName("TargetFramework"),
                             };

            return (from reference in references
                    group reference by reference.TargetFramework into referencesByFramework
                    select new ManifestReferenceSet
                    {
                        TargetFramework = referencesByFramework.Key.GetShortFrameworkName(),
                        References = (from reference in referencesByFramework
                                      select new ManifestReference
                                      {
                                          File = reference.File
                                      }).ToList()
                    }).ToList();
        }

        private static PackageReference AggregateVersions(PackageReference aggregate, PackageReference next)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            SemanticVersion version = aggregate.Version;
            if (version != null && next.Version != null)
            {
                version = version > next.Version ? version : next.Version;
            }
            else if (next.Version != null)
            {
                version = next.Version;
            }

            var versionSpec = new VersionSpec();
            SetMinVersion(versionSpec, aggregate.VersionConstraint);
            SetMinVersion(versionSpec, next.VersionConstraint);
            SetMaxVersion(versionSpec, aggregate.VersionConstraint);
            SetMaxVersion(versionSpec, next.VersionConstraint);

            if (versionSpec.MinVersion == null && versionSpec.MaxVersion == null)
            {
                versionSpec = null;
            }

            return new PackageReference(next.Id, version, versionSpec, next.TargetFramework, false);
        }

        private static void SetMinVersion(VersionSpec target, IVersionSpec source)
        {
            if (source == null || source.MinVersion == null)
            {
                return;
            }

            if (target.MinVersion == null)
            {
                target.MinVersion = source.MinVersion;
                target.IsMinInclusive = source.IsMinInclusive;
            }

            if (target.MinVersion < source.MinVersion)
            {
                target.MinVersion = source.MinVersion;
                target.IsMinInclusive = source.IsMinInclusive;
            }

            if (target.MinVersion == source.MinVersion)
            {
                target.IsMinInclusive = target.IsMinInclusive && source.IsMinInclusive;
            }
        }

        private static void SetMaxVersion(VersionSpec target, IVersionSpec source)
        {
            if (source == null || source.MaxVersion == null)
            {
                return;
            }

            if (target.MaxVersion == null)
            {
                target.MaxVersion = source.MaxVersion;
                target.IsMaxInclusive = source.IsMaxInclusive;
            }

            if (target.MaxVersion > source.MaxVersion)
            {
                target.MaxVersion = source.MaxVersion;
                target.IsMaxInclusive = source.IsMaxInclusive;
            }

            if (target.MaxVersion == source.MaxVersion)
            {
                target.IsMaxInclusive = target.IsMaxInclusive && source.IsMaxInclusive;
            }
        }
    }
}