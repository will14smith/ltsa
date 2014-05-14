using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using LTSASharp.Utilities;

namespace LTSASharp.Lts
{
    public class LtsSystem
    {
        public LtsSystem()
        {
            Transitions = new HashSet<LtsAction>();
            Alphabet = new HashSet<LtsLabel>();
            States = new HashSet<LtsState>();
        }

        public ISet<LtsState> States { get; private set; }
        public ISet<LtsLabel> Alphabet { get; private set; }
        public ISet<LtsAction> Transitions { get; private set; }
        public LtsState InitialState { get; set; }

        /// <summary>
        /// Remove any unreachable states and transitions from this lts
        /// </summary>
        public LtsSystem Prune()
        {
            // possibly need to add state renumbering?

            var lts = new LtsSystem { InitialState = InitialState };

            lts.States.Add(InitialState);

            var count = 0;
            while (count != lts.States.Count)
            {
                count = lts.States.Count;

                var newTransitions = Transitions.Where(x => lts.States.Contains(x.Source)).ToSet();
                var newStates = newTransitions.Select(x => x.Destination);

                lts.Transitions.AddRange(newTransitions);
                lts.States.AddRange(newStates);
            }

            lts.Alphabet.AddRange(lts.Transitions.Select(x => x.Action));

            return lts;
        }
    }
}
