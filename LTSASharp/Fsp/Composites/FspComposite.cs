using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Composites
{
    public class FspComposite : FspBaseProcess
    {
        public FspComposite()
        {
            Body = new List<FspCompositeBody>();
        }

        public List<FspCompositeBody> Body { get; private set; }

        // Relabel
        // Prio
        // Hiding

        public override string ToString()
        {
            //TODO parameters
            return "||" + Name + " = " + string.Join(" || ", Body.Select(x => "( " + x + " )"));
        }
    }
}