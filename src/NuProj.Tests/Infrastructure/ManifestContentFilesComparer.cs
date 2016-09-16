using System;
using System.Collections.Generic;

using NuGet.Packaging;

namespace NuProj.Tests.Infrastructure
{
    public class ManifestContentFilesComparer : IEqualityComparer<ManifestContentFiles>
    {
        private static ManifestContentFilesComparer _instance = new ManifestContentFilesComparer(StringComparer.OrdinalIgnoreCase);

        private StringComparer _stringComparer;

        public ManifestContentFilesComparer(StringComparer stringComparer)
        {
            if (stringComparer == null)
            {
                throw new ArgumentNullException("stringComparer");
            }

            _stringComparer = stringComparer;
        }

        public static ManifestContentFilesComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool Equals(ManifestContentFiles x, ManifestContentFiles y)
        {
            if (x == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return _stringComparer.Equals(x.BuildAction, y.BuildAction)
                && _stringComparer.Equals(x.CopyToOutput, y.CopyToOutput)
                && _stringComparer.Equals(x.Exclude, x.Exclude)
                && _stringComparer.Equals(x.Flatten, y.Flatten)
                && _stringComparer.Equals(x.Include, y.Include);
        }

        public int GetHashCode(ManifestContentFiles obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            return _stringComparer.GetHashCode(obj.BuildAction ?? "")
                + _stringComparer.GetHashCode(obj.CopyToOutput ?? "")
                + _stringComparer.GetHashCode(obj.Exclude ?? "")
                +_stringComparer.GetHashCode(obj.Flatten ?? "")
                +_stringComparer.GetHashCode(obj.Include ?? "");
        }
    }
}