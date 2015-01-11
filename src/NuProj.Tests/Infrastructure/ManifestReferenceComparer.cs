using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestReferenceComparer : IEqualityComparer<ManifestReference>
    {
        private StringComparer _stringComparer;

        public ManifestReferenceComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;

        }

        public bool Equals(ManifestReference x, ManifestReference y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return _stringComparer.Equals(x.File, y.File);
        }

        public int GetHashCode(ManifestReference obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.File ?? "");
        }
    }
}
