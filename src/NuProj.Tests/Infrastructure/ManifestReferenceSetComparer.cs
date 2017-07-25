using System;
using System.Collections.Generic;
using NuGet.Packaging;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestReferenceSetComparer : IEqualityComparer<PackageReferenceSet>
    {
        private static ManifestReferenceSetComparer _instance = new ManifestReferenceSetComparer(StringComparer.OrdinalIgnoreCase);

        //private ManifestReferenceComparer _manifestReferenceComparer;

        private StringComparer _stringComparer;

        public ManifestReferenceSetComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;
            //_manifestReferenceComparer = new ManifestReferenceComparer(stringComparer);
        }

        public static ManifestReferenceSetComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(PackageReferenceSet x, PackageReferenceSet y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xReferences = new HashSet<string>(x.References.NullAsEmpty());
            var yReferences = new HashSet<string>(y.References.NullAsEmpty());

            return _stringComparer.Equals(x.TargetFramework, y.TargetFramework)
                && xReferences.SetEquals(yReferences);
        }

        public int GetHashCode(PackageReferenceSet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return obj.TargetFramework.GetHashCode();
        }
    }
}