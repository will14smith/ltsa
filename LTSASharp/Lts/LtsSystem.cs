using System.Collections.Generic;
using System.Linq;

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
        public void Prune()
        {
            //TODO make functional and do state renumber?

            var count = 0;
            while (Transitions.Count != count)
            {
                foreach (var transition in Transitions.ToList())
                {
                    if (InitialState == transition.Source)
                        continue;

                    if (!Transitions.Any(x => x.Destination == transition.Source))
                        Transitions.Remove(transition);
                }

                count = Transitions.Count;
            }

            foreach (var state in States.ToList())
            {
                if (InitialState == state)
                    continue;

                if (!Transitions.Any(x => x.Destination == state))
                    States.Remove(state);
            }
        }
    }
}
