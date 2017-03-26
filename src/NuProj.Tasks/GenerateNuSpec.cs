using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuProj.Tasks
{
    public class GenerateNuSpec : Task
    {
        private const string NuSpecXmlNamespace = @"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

        public string InputFileName { get; set; }

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

        [Output]
        public ITaskItem[] FilesWritten { get; set; }

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
                FilesWritten = new[] { new TaskItem(OutputFileName) };
                return;
            }

            var directory = Path.GetDirectoryName(OutputFileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var file = File.Create(OutputFileName))
            {
                manifest.Save(file, false);
                FilesWritten = new[] { new TaskItem(OutputFileName) };
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
                var reader = new StreamReader(stream, Encoding.UTF8, true);
                newSource = reader.ReadToEnd();
                //newSource = new UTF8Encoding(true).GetString(stream.ToArray());
                //newSource = Encoding.Default.GetString(stream.ToArray());
            }

            return oldSource != newSource;
        }

        private Manifest CreateManifest()
        {
            Manifest manifest;
            ManifestMetadata manifestMetadata;
            if (!string.IsNullOrEmpty(InputFileName))
            {
                using (var stream = File.OpenRead(InputFileName))
                {
                    manifest = Manifest.ReadFrom(stream, false);
                }

                manifestMetadata = manifest.Metadata;
            }
            else
            {
                manifest = new Manifest(new ManifestMetadata());
            }


            manifestMetadata = manifest.Metadata;
            var files = GetManifestFiles();

            if (manifest.Files == null && files.Any())
            {
                manifest = new Manifest(manifest.Metadata, new List<ManifestFile>());
            }

            manifestMetadata.UpdateMember(x => x.Authors, Split(Authors));
            manifestMetadata.AddRangeToMember(x => x.ContentFiles, GetManifestContentFiles());
            manifestMetadata.UpdateMember(x => x.Copyright, Copyright);
            manifestMetadata.AddRangeToMember(x => x.DependencyGroups, GetDependencySets());
            manifestMetadata.UpdateMember(x => x.Description, Description);
            manifestMetadata.DevelopmentDependency |= DevelopmentDependency;
            manifestMetadata.AddRangeToMember(x => x.FrameworkReferences, GetFrameworkAssemblies());
            manifestMetadata.UpdateMember(x => x.IconUrl, IconUrl);
            manifestMetadata.UpdateMember(x => x.Id, Id);
            manifestMetadata.UpdateMember(x => x.Language, Language);
            manifestMetadata.UpdateMember(x => x.LicenseUrl, LicenseUrl);
            manifestMetadata.UpdateMember(x => x.MinClientVersionString, MinClientVersion);
            manifestMetadata.UpdateMember(x => x.Owners, Split(Owners));
            manifestMetadata.UpdateMember(x => x.ProjectUrl, ProjectUrl);
            manifestMetadata.AddRangeToMember(x => x.PackageAssemblyReferences, GetReferenceSets());
            manifestMetadata.UpdateMember(x => x.ReleaseNotes, ReleaseNotes);
            manifestMetadata.RequireLicenseAcceptance |= RequireLicenseAcceptance;
            manifestMetadata.UpdateMember(x => x.Summary, Summary);
            manifestMetadata.UpdateMember(x => x.Tags, Tags);
            manifestMetadata.UpdateMember(x => x.Title, Title);
            manifestMetadata.UpdateMember(x => x.Version, string.IsNullOrEmpty(Version) ? null : new NuGetVersion(Version));


            manifest.AddRangeToMember(x => x.Files, GetManifestFiles());

            return manifest;
        }

        private List<ManifestContentFiles> GetManifestContentFiles()
        {
            return (from f in Files.NullAsEmpty()
                    where f.GetPackageDirectory() == PackageDirectory.ContentFiles
                    let source = f.GetMetadata(Metadata.FileSource)
                    select new ManifestContentFiles
                    {
                        Include = GetContentFilesIncludePath(f),
                        Exclude = f.GetMetadata(Metadata.FileExclude, null),
                        CopyToOutput = f.GetBoolean(Metadata.ContentFile.CopyToOutput).ToString().ToLower(),
                        Flatten = f.GetBoolean(Metadata.ContentFile.Flatten).ToString().ToLower(),
                        BuildAction = f.GetMetadata(Metadata.ContentFile.BuildAction, null),
                    }).ToList();
        }

        private List<ManifestFile> GetManifestFiles()
        {
            return (from f in Files.NullAsEmpty()
                    select new ManifestFile
                    {
                        Source = f.GetMetadata(Metadata.FileSource),
                        Target = f.GetMetadata(Metadata.FileTarget),
                        Exclude = f.GetMetadata(Metadata.FileExclude),
                    }).ToList();
        }

        private List<FrameworkAssemblyReference> GetFrameworkAssemblies()
        {
            return (from fr in FrameworkReferences.NullAsEmpty()
                    select new FrameworkAssemblyReference(fr.ItemSpec, new[] { fr.GetTargetFramework() }))
                    .ToList();
        }

        private List<PackageDependencyGroup> GetDependencySets()
        {
            var dependencies = from d in Dependencies.NullAsEmpty()
                               select new
                               {
                                   Id = d.ItemSpec,
                                   Version = d.GetVersion(),
                                   TargetFramework = d.GetTargetFramework()
                               };

            return (from dependency in dependencies
                    group dependency by dependency.TargetFramework into dependenciesByFramework
                    let targetFramework = dependenciesByFramework.Key
                    let packages = (from dependency in dependenciesByFramework
                                    where dependency.Id != "_._"
                                    group dependency by dependency.Id into dependenciesById
                                    select new PackageDependency(dependenciesById.Key, VersionRange.CommonSubSet(dependenciesById.Select(x => x.Version))))
                    select new PackageDependencyGroup(targetFramework, packages)
                    ).ToList();
        }

        private List<PackageReferenceSet> GetReferenceSets()
        {
            var references = from r in References.NullAsEmpty()
                             select new
                             {
                                 File = r.ItemSpec,
                                 TargetFramework = r.GetTargetFramework(),
                             };

            return (from reference in references
                    group reference by reference.TargetFramework into referencesByFramework
                    let targetFramework = referencesByFramework.Key
                    select new PackageReferenceSet(referencesByFramework.Key, referencesByFramework.Select(x => x.File)))
                    .ToList();
        }

        private IEnumerable<string> Split(string field)
        {
            var parts = field?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            return parts?.Any() == true ? parts : null;
        }

        private Uri ToUri(string field)
        {
            return string.IsNullOrEmpty(field) ? null : new Uri(field);
        }

        private string GetContentFilesIncludePath(ITaskItem taskItem)
        {
            PackageDirectory packageDirectory;
            string includePath;
            taskItem.GetTargetPackageDirectory(out packageDirectory, out includePath);
            if (packageDirectory != PackageDirectory.ContentFiles)
            {
                Log.LogError($"File '{taskItem.ItemSpec}' has unexpected PackageDirectory metadata. Expected '{PackageDirectory.ContentFiles}', actual '{packageDirectory}'.");
            }

            return includePath;
        }
    }
}