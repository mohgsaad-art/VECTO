
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCore.Utils
{
    public static class EnumerableExtensionMethods
    {
        public static bool IsEqual<T>(
            this IEnumerable<Tuple<T, T>> a,
            IEnumerable<Tuple<T, T>> b,
            IEqualityComparer<T> comparer = null)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((a == null) || (b == null) || (a.Count() != b.Count()))
            {
                return false;
            }
                
            comparer = (comparer != null) ? comparer : EqualityComparer<T>.Default;

            return a
                .Zip(b, (x, y) => comparer.Equals(x.Item1, y.Item1) && comparer.Equals(x.Item2, y.Item2))
                .All(equal => equal);
        }
    }
}
