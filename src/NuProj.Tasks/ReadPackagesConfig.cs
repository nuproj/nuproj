using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NuProj.Tasks
{
    public class ReadPackagesConfig : Task
    {
        [Required]
        public string PackagesConfigPath { get; set; }

        [Output]
        public ITaskItem[] Packages { get; set; }

        public override bool Execute()
        {
            var document = XDocument.Load(PackagesConfigPath);
            var packageElements = document.XPathSelectElements("/packages/package");
            Packages = packageElements.Select(ConvertPackageElement).ToArray();
            return true;
        }

        protected ITaskItem ConvertPackageElement(XElement element)
        {
            var idAttribute = element.Attribute("id");
            var versionAttribute = element.Attribute("version");
            var targetFrameworkAttribute = element.Attribute("targetFramework");

            var id = idAttribute.Value;
            var version = versionAttribute.Value;
            var targetFramework = targetFrameworkAttribute == null ? null : targetFrameworkAttribute.Value;
            var packageDirectoryPath = GetPackageDirectoryPath(PackagesConfigPath, id, version);

            var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            metadata.Add("Version", version);
            if (targetFramework != null)
                metadata.Add("TargetFramework", targetFramework);
            metadata.Add("PackageDirectoryPath", packageDirectoryPath);

            var item = new TaskItem(id, metadata);
            return item;
        }

        private static string GetPackageDirectoryPath(string packagesConfigPath, string packageId, string packageVersion)
        {
            var packageDirectoryName = packageId + "." + packageVersion;
            var candidateFolder = Path.GetDirectoryName(packagesConfigPath);
            while (candidateFolder != null)
            {
                var packageDirectoryPath = Path.Combine(candidateFolder, "packages", packageDirectoryName);
                if (Directory.Exists(packageDirectoryPath))
                    return packageDirectoryPath;

                candidateFolder = Path.GetDirectoryName(candidateFolder);
            }

            return string.Empty;
        }
    }
}