using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Versioning;

namespace NuProj.Tests.Infrastructure
{
    public class Package
    {
        public string Authors { get; internal set; }
        public IEnumerable<PackageDependencyGroup> DependencySets { get; internal set; }
        public string Description { get; internal set; }
        public IEnumerable<string> Files { get; internal set; }
        public string Id { get; internal set; }
        public string LicenseUrl { get; internal set; }
        public string ProjectUrl { get; internal set; }
        public bool RequireLicenseAcceptance { get; internal set; }

        public IEnumerable<string> GetLibFiles()
        {
            return Files.Where(x => x.StartsWith("lib\\"));
        }

        public string Summary { get; set; }
        public NuGetVersion Version { get; internal set; }
        
    }
}
