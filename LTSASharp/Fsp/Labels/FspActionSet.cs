using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Labels
{
    class FspActionSet : IFspActionLabel
    {
        public FspActionSet()
        {
            Items = new List<IFspActionLabel>();
        }

        public List<IFspActionLabel> Items { get; private set; }

        public override string ToString()
        {
            return "{" + string.Join(", ", Items.Select(x => x.ToString())) + "}";
        }
    }
}
