using System;
using System.Collections.Generic;
using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestDependencySetComparer : IEqualityComparer<ManifestDependencySet>
    {
        private static ManifestDependencySetComparer _ordinal = new ManifestDependencySetComparer(StringComparer.Ordinal);

        private static ManifestDependencySetComparer _ordinalIgnoreCase = new ManifestDependencySetComparer(StringComparer.OrdinalIgnoreCase);

        private ManifestDependencyComparer _manifestDependencyComparer;

        private StringComparer _stringComparer;

        public ManifestDependencySetComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;
            _manifestDependencyComparer = new ManifestDependencyComparer(stringComparer);
        }

        public static ManifestDependencySetComparer Ordinal
        {
            get
            {
                return _ordinal;
            }
        }

        public static ManifestDependencySetComparer OrdinalIgnoreCase
        {
            get
            {
                return _ordinalIgnoreCase;
            }
        }

        public bool Equals(ManifestDependencySet x, ManifestDependencySet y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xDependencies = new HashSet<ManifestDependency>(x.Dependencies.NullAsEmpty(), _manifestDependencyComparer);
            var yDependencies = new HashSet<ManifestDependency>(y.Dependencies.NullAsEmpty(), _manifestDependencyComparer);

            return _stringComparer.Equals(x.TargetFramework, y.TargetFramework)
                && xDependencies.SetEquals(yDependencies);
        }

        public int GetHashCode(ManifestDependencySet obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.TargetFramework ?? "");
        }
    }
}