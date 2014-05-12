using System.Collections.Generic;

namespace LTSASharp.Lts
{
    class LtsSystem
    {
        public LtsSystem()
        {
            Actions = new HashSet<LtsAction>();
            Alphabet = new HashSet<LtsLabel>();
            States = new HashSet<LtsState>();
        }

        public ISet<LtsState> States { get; private set; }
        public ISet<LtsLabel> Alphabet { get; private set; }
        public ISet<LtsAction> Actions { get; private set; }
        public LtsState InitialState { get; set; }
    }
}
