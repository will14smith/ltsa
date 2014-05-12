using System.Collections.Generic;

namespace LTSASharp.Lts
{
    class LtsSystem
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
    }
}
