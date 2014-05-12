using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Composites
{
    class FspComposite
    {
        public FspComposite()
        {
            Body = new List<FspCompositeBody>();
        }

        public string Name { get; set; }
        public List<FspCompositeBody> Body { get; private set; }
        // Relabel
        // Prio
        // Hiding

        public override string ToString()
        {
            return "||" + Name + " = " + string.Join(" || ", Body.Select(x => "( " + x + " )"));
        }
    }
}