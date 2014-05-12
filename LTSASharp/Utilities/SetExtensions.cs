using System;
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

        /// <summary>
        /// Perform a shallow clone of a set
        /// </summary>
        public static TSet Clone<TSet, TItem>(this TSet origSet)
            where TSet : ISet<TItem>, new()
        {
            var newSet = new TSet();

            newSet.AddRange(origSet);

            return newSet;
        }

        /// <summary>
        /// Perform a shallow clone of a set
        /// </summary>
        public static ISet<TItem> Clone<TItem>(this ISet<TItem> origSet)
        {
            var newSet = (ISet<TItem>)Activator.CreateInstance(origSet.GetType());

            newSet.AddRange(origSet);

            return newSet;
        }
    }
}
