using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuProj.Tests.Infrastructure
{
    public static class Extensions
    {
        public static int GetHashCodeNullSafe<T>(this T obj)
            where T : class
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            return source;
        }

    }
}
