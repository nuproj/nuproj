using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Packaging;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestFrameworkAssemblyComparer : IEqualityComparer<FrameworkAssemblyReference>
    {
        private static ManifestFrameworkAssemblyComparer _instance = new ManifestFrameworkAssemblyComparer(StringComparer.OrdinalIgnoreCase);

        private StringComparer _stringComparer;

        public ManifestFrameworkAssemblyComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;
        }

        public static ManifestFrameworkAssemblyComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(FrameworkAssemblyReference x, FrameworkAssemblyReference y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xSupportedFrameworks = new HashSet<NuGetFramework>(x.SupportedFrameworks);
            var ySupportedFrameworks = new HashSet<NuGetFramework>(y.SupportedFrameworks);

            return _stringComparer.Equals(x.AssemblyName, y.AssemblyName)
                && xSupportedFrameworks.SetEquals(ySupportedFrameworks);
        }

        public int GetHashCode(FrameworkAssemblyReference obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.AssemblyName ?? "")
                + obj.SupportedFrameworks.Select(x => x.GetHashCode()).Sum();
        }
    }
}