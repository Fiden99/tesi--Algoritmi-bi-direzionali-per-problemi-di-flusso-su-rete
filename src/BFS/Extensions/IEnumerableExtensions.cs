using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS
{
    public static class IEnumerableExtensions
    {
        public static void Print<T>(this IEnumerable<T> values, Func<T, string> stringify)
        {
            Console.WriteLine($"[" + string.Join(",", values.Select(stringify)) + "]");
        }

        public static void Print<T>(this IEnumerable<T> values)
        {
            Print<T>(values, x => x.ToString());
        }
    }

}