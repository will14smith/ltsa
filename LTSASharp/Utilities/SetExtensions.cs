using System;
using System.Collections.Generic;

namespace LTSASharp.Utilities
{
    public static class SetExtensions
    {
        /// <summary>
        /// Perform a shallow clone of a set
        /// </summary>
        public static TSet Clone<TSet, TItem>(this TSet origSet)
            where TSet : ISet<TItem>, new()
        {
            var newSet = new TSet();

            newSet.UnionWith(origSet);

            return newSet;
        }

        /// <summary>
        /// Perform a shallow clone of a set
        /// </summary>
        public static ISet<TItem> Clone<TItem>(this ISet<TItem> origSet)
        {
            var newSet = (ISet<TItem>)Activator.CreateInstance(origSet.GetType());

            newSet.UnionWith(origSet);

            return newSet;
        }

        public static ISet<TItem> ToSet<TItem>(this IEnumerable<TItem> items)
        {
            if (items is ISet<TItem>)
                return ((ISet<TItem>)items).Clone();

            var set = new HashSet<TItem>();

            set.UnionWith(items);

            return set;
        }
    }
}
