using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using LTSASharp.Utilities;

namespace LTSASharp.Lts.Conversion
{
    internal class LtsRelabeler
    {
        private readonly MultiMap<LtsLabel, LtsLabel> map;

        public LtsRelabeler(MultiMap<LtsLabel, LtsLabel> map)
        {
            this.map = map;
        }

        public LtsSystem Relabel(LtsSystem lts)
        {
            var newLts = new LtsSystem { InitialState = lts.InitialState };

            newLts.States.AddRange(lts.States);

            var b1 = GetB1(lts.Alphabet).ToSet();
            var b2 = GetB2(lts.Alphabet).ToSet();

            newLts.Alphabet.AddRange(lts.Alphabet);
            newLts.Alphabet.ExceptWith(b1);
            newLts.Alphabet.UnionWith(b2);

            var d1 = GetD1(lts.Transitions, b1).ToSet();
            var d2 = GetD2(d1);

            newLts.Transitions.AddRange(lts.Transitions);
            newLts.Transitions.ExceptWith(d1);
            newLts.Transitions.UnionWith(d2);

            return newLts;
        }

        /// <summary>
        /// All the labels to remove from the alphabet
        /// </summary>
        private IEnumerable<LtsLabel> GetB1(IEnumerable<LtsLabel> alphabet)
        {
            return alphabet.Where(a => map.ContainsKey(a) && map[a].Any());
        }

        /// <summary>
        /// All the labels to add to the alphabet
        /// </summary>
        private IEnumerable<LtsLabel> GetB2(IEnumerable<LtsLabel> alphabet)
        {
            return alphabet.Where(a => map.ContainsKey(a)).SelectMany(a => map[a]);
        }

        /// <summary>
        /// All the transitions to remove
        /// </summary>
        private IEnumerable<LtsAction> GetD1(IEnumerable<LtsAction> actions, IEnumerable<LtsLabel> b1)
        {
            return actions.Where(a => b1.Contains(a.Action));
        }
        /// <summary>
        /// All the transitions to add
        /// </summary>
        private IEnumerable<LtsAction> GetD2(IEnumerable<LtsAction> d1)
        {
            return d1.Where(a => map.ContainsKey(a.Action) && map[a.Action].Any()).SelectMany(a => map[a.Action], (a, t) => new LtsAction(a.Source, t, a.Destination));
        }
    }
}