using System;
using System.Collections.Generic;

using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestFrameworkAssemblyComparer : IEqualityComparer<ManifestFrameworkAssembly>
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

        public bool Equals(ManifestFrameworkAssembly x, ManifestFrameworkAssembly y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return _stringComparer.Equals(x.AssemblyName, y.AssemblyName)
                && _stringComparer.Equals(x.TargetFramework, y.TargetFramework);
        }

        public int GetHashCode(ManifestFrameworkAssembly obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.AssemblyName ?? "")
                + _stringComparer.GetHashCode(obj.TargetFramework ?? "");
        }
    }
}