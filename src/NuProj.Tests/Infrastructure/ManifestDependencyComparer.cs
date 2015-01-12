using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestDependencyComparer : IEqualityComparer<ManifestDependency>
    {
        private StringComparer _stringComparer;
        
        public ManifestDependencyComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;

        }
        
        public bool Equals(ManifestDependency x, ManifestDependency y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return _stringComparer.Equals(x.Id, y.Id) && _stringComparer.Equals(x.Version, y.Version);
        }

        public int GetHashCode(ManifestDependency obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.Id ?? "") + _stringComparer.GetHashCode(obj.Version ?? "");
        }
    }
}
