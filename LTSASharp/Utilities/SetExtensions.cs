using System.Collections.Generic;

namespace LTSASharp.Utilities
{
    public static class SetExtensions
    {
        public static void AddRange<TItem>(this ISet<TItem> set, IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}
