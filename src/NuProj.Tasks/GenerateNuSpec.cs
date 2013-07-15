using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

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
            var document = CreateNuSpecDocument();
            if (!IsDifferent(document))
            {
                Log.LogMessage("Skipping generation of .nuspec because contents are identical.");
                return;
            }

            var directory = Path.GetDirectoryName(OutputFileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            document.Save(OutputFileName);
        }

        private bool IsDifferent(XDocument newDocument)
        {
            if (!File.Exists(OutputFileName))
                return true;

            var oldSource = XDocument.Load(OutputFileName).ToString();
            var newSource = newDocument.ToString();
            return oldSource != newSource;
        }

        private XDocument CreateNuSpecDocument()
        {
            var element = CreatePackageElement();
            return new XDocument(element);
        }

        private XElement CreatePackageElement()
        {
            var metadata = CreateMetadataElement();
            var files = CreateFilesElement();
            return new XElement(XName.Get("package", NuSpecXmlNamespace), metadata, files);
        }

        private XElement CreateMetadataElement()
        {
            var minClientVersionAttribute = GetMinClientVersionAttribute();
            var simpleElements = GetSimpleMetadataElements();
            var dependency = GetDependenciesElement();
            var references = GetReferencesElement();
            var frameworkReference = GetFrameworkReferencesElement();
            return new XElement(XName.Get("metadata", NuSpecXmlNamespace), minClientVersionAttribute, simpleElements, dependency, references, frameworkReference);
        }

        private XAttribute GetMinClientVersionAttribute()
        {
            return string.IsNullOrEmpty(MinClientVersion)
                     ? null
                     : new XAttribute("minClientVersion", MinClientVersion);
        }

        private IEnumerable<XElement> GetSimpleMetadataElements()
        {
            yield return GetMetadataElement("id", Id);
            yield return GetMetadataElement("version", Version);
            yield return GetMetadataElement("title", Title);
            yield return GetMetadataElement("authors", Authors);
            yield return GetMetadataElement("owners", Owners);
            yield return GetMetadataElement("description", Description);
            yield return GetMetadataElement("releaseNotes", ReleaseNotes);
            yield return GetMetadataElement("summary", Summary);
            yield return GetMetadataElement("language", Language);
            yield return GetMetadataElement("projectUrl", ProjectUrl);
            yield return GetMetadataElement("iconUrl", IconUrl);
            yield return GetMetadataElement("licenseUrl", LicenseUrl);
            yield return GetMetadataElement("copyright", Copyright);
            yield return GetMetadataElement("requireLicenseAcceptance", RequireLicenseAcceptance);
            yield return GetMetadataElement("tags", Tags);
        }

        private static XElement GetMetadataElement(string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var qualifiedName = XName.Get(name, NuSpecXmlNamespace);
            return new XElement(qualifiedName, value);
        }

        private static XElement GetMetadataElement(string name, bool value)
        {
            return GetMetadataElement(name, XmlConvert.ToString(value));
        }

        private XElement GetDependenciesElement()
        {
            if (Dependencies == null || Dependencies.Length == 0)
                return null;

            var dependencies = from d in Dependencies
                               select new
                                          {
                                              Id = d.ItemSpec,
                                              Version = d.GetMetadata("Version"),
                                              TargetFramework = d.GetMetadata("TargetFramework"),
                                          };

            var dependenciesGroupedByTargetFramework = dependencies.GroupBy(d => d.TargetFramework);

            var elements = from g in dependenciesGroupedByTargetFramework
                           let d = from d in g
                                   let idAttribute = new XAttribute("id", d.Id)
                                   let versionAttribute = string.IsNullOrEmpty(d.Id)
                                                              ? null
                                                              : new XAttribute("version", d.Version)
                                   select new XElement(XName.Get("dependency", NuSpecXmlNamespace), idAttribute, versionAttribute)
                           let targetFramework = g.Key
                           let targetFrameworkAttribute = string.IsNullOrEmpty(targetFramework)
                                                               ? null
                                                               : new XAttribute("targetFramework", targetFramework)
                           select new XElement(XName.Get("group", NuSpecXmlNamespace), targetFrameworkAttribute, d);

            return new XElement(XName.Get("dependencies", NuSpecXmlNamespace), elements);
        }

        private XElement GetReferencesElement()
        {
            if (References == null || References.Length == 0)
                return null;

            var references = from r in References
                             select new
                                        {
                                            File = r.ItemSpec,
                                        };

            var elements = from r in references
                           let fileAttribute = new XAttribute("file", r.File)
                           select new XElement(XName.Get("reference", NuSpecXmlNamespace), fileAttribute);

            return new XElement(XName.Get("references", NuSpecXmlNamespace), elements);
        }

        private XElement GetFrameworkReferencesElement()
        {
            if (FrameworkReferences == null || FrameworkReferences.Length == 0)
                return null;

            var references = from r in FrameworkReferences
                             select new
                             {
                                 AssemblyName = r.ItemSpec,
                                 TargetFramework = r.GetMetadata("TargetFramework")
                             };

            var elements = from r in references
                           let assemblyNameAttribute = new XAttribute("assemblyName", r.AssemblyName)
                           let targetFrameworkAttribute = string.IsNullOrEmpty(r.TargetFramework)
                                                              ? null
                                                              : new XAttribute("targetFramework", r.TargetFramework)
                           select new XElement(XName.Get("frameworkAssembly", NuSpecXmlNamespace), assemblyNameAttribute, targetFrameworkAttribute);

            return new XElement(XName.Get("frameworkAssemblies", NuSpecXmlNamespace), elements);
        }

        private XElement CreateFilesElement()
        {
            if (Files == null || Files.Length == 0)
                return null;

            var files = from f in Files
                        select new
                                   {
                                       Include = f.GetMetadata("FullPath"),
                                       TargetPath = f.GetMetadata("TargetPath")
                                   };

            var elements = from f in files
                           let srcAttribute = new XAttribute("src", f.Include)
                           let targetPathAttribute = string.IsNullOrEmpty(f.TargetPath)
                                                         ? null
                                                         : new XAttribute("target", f.TargetPath)
                           select new XElement(XName.Get("file", NuSpecXmlNamespace), srcAttribute, targetPathAttribute);

            return new XElement(XName.Get("files", NuSpecXmlNamespace), elements);
        }
    }
}