using System;
using System.Collections.Generic;
using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestReferenceSetComparer : IEqualityComparer<ManifestReferenceSet>
    {
        private static ManifestReferenceSetComparer _instance = new ManifestReferenceSetComparer(StringComparer.OrdinalIgnoreCase);

        private ManifestReferenceComparer _manifestReferenceComparer;

        private StringComparer _stringComparer;

        public ManifestReferenceSetComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;
            _manifestReferenceComparer = new ManifestReferenceComparer(stringComparer);
        }

        public static ManifestReferenceSetComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(ManifestReferenceSet x, ManifestReferenceSet y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xReferences = new HashSet<ManifestReference>(x.References.NullAsEmpty(), _manifestReferenceComparer);
            var yReferences = new HashSet<ManifestReference>(y.References.NullAsEmpty(), _manifestReferenceComparer);

            return _stringComparer.Equals(x.TargetFramework, y.TargetFramework)
                && xReferences.SetEquals(yReferences);
        }

        public int GetHashCode(ManifestReferenceSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.TargetFramework ?? "");
        }
    }
}