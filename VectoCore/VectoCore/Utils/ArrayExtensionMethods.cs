using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCore.Utils
{
    public static class ArrayExtensionMethods
    {
        public static bool IsEqual<T>(this T[] a, T[] b, IEqualityComparer<T> comparer = null)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((a == null) || (b == null) || (a.Length != b.Length))
            {
                return false;
            }

            comparer = (comparer != null) ? comparer : EqualityComparer<T>.Default;

            return a.SequenceEqual(b, comparer);
        }

        public static bool IsEqual<T>(this T[][] a, T[][] b, IEqualityComparer<T> comparer = null)
        {
            if (ReferenceEquals(a, b))
            { 
                return true;
            }

            if ((a == null) || (b == null) || (a.Length != b.Length))
            {
                return false;
            }

            comparer = (comparer != null) ? comparer : EqualityComparer<T>.Default;

            for (int i = 0; i < a.Length; i++)
            {
                var x = a[i];
                var y = b[i];

                if (ReferenceEquals(x, y))
                {
                    continue;
                }

                if ((x == null) || (y == null) || (x.Length != y.Length))
                {
                    return false;
                }

                for (int j = 0; j < x.Length; j++)
                {
                    if (!comparer.Equals(x[j], y[j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
