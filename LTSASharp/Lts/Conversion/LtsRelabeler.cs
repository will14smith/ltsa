using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using LTSASharp.Utilities;

namespace LTSASharp.Lts.Conversion
{
    internal class LtsRelabeler
    {
        // old => [new]
        private readonly MultiMap<LtsLabel, LtsLabel> masterMap;

        public LtsRelabeler(MultiMap<LtsLabel, LtsLabel> map)
        {
            masterMap = map;
        }

        public LtsSystem Relabel(LtsSystem lts)
        {
            // handle prefixes
            var map = new MultiMap<LtsLabel, LtsLabel>();
            foreach (var e in masterMap)
                map.Add(e.Key, e.Value);

            AddPrefixMappings(lts, map);

            // relabel lts
            var newLts = new LtsSystem { InitialState = lts.InitialState };

            newLts.States.UnionWith(lts.States);

            var b1 = GetB1(map, lts.Alphabet).ToSet();
            var b2 = GetB2(map, lts.Alphabet).ToSet();

            newLts.Alphabet.UnionWith(lts.Alphabet);
            newLts.Alphabet.ExceptWith(b1);
            newLts.Alphabet.UnionWith(b2);

            var d1 = GetD1(map, lts.Transitions, b1).ToSet();
            var d2 = GetD2(map, d1);

            newLts.Transitions.UnionWith(lts.Transitions);
            newLts.Transitions.ExceptWith(d1);
            newLts.Transitions.UnionWith(d2);

            return newLts;
        }

        private static void AddPrefixMappings(LtsSystem lts, MultiMap<LtsLabel, LtsLabel> map)
        {
            foreach (var a in lts.Alphabet)
            {
                // loop the prefix pairs from largest prefix first
                foreach (var prefix in a.PrefixPairs)
                {
                    // if map contains the prefix
                    if (!map.ContainsKey(prefix.Item1))
                        continue;

                    // add suffix to all mappings
                    var newMapping = map[prefix.Item1].Select(x =>
                    {
                        var newLabel = x.Name + "." + prefix.Item2.Name;
                        return new LtsLabel(newLabel);
                    }).ToList();

                    // map a to suffix mappings
                    map.Add(new LtsLabel(a.Name), newMapping);

                    break;
                }
            }
        }

        /// <summary>
        /// All the labels to remove from the alphabet
        /// </summary>
        private IEnumerable<LtsLabel> GetB1(MultiMap<LtsLabel, LtsLabel> map, IEnumerable<LtsLabel> alphabet)
        {
            return alphabet.Where(a => map.ContainsKey(a) && map[a].Any());
        }

        /// <summary>
        /// All the labels to add to the alphabet
        /// </summary>
        private IEnumerable<LtsLabel> GetB2(MultiMap<LtsLabel, LtsLabel> map, IEnumerable<LtsLabel> alphabet)
        {
            return alphabet.Where(map.ContainsKey).SelectMany(a => map[a]);
        }

        /// <summary>
        /// All the transitions to remove
        /// </summary>
        private IEnumerable<LtsAction> GetD1(MultiMap<LtsLabel, LtsLabel> map, IEnumerable<LtsAction> actions, IEnumerable<LtsLabel> b1)
        {
            return actions.Where(a => b1.Contains(a.Action));
        }
        /// <summary>
        /// All the transitions to add
        /// </summary>
        private IEnumerable<LtsAction> GetD2(MultiMap<LtsLabel, LtsLabel> map, IEnumerable<LtsAction> d1)
        {
            return d1.Where(a => map.ContainsKey(a.Action) && map[a.Action].Any()).SelectMany(a => map[a.Action], (a, t) => new LtsAction(a.Source, t, a.Destination));
        }
    }
}