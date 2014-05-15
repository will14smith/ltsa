using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Composites
{
    public class FspComposite
    {
        public FspComposite()
        {
            Body = new List<FspCompositeBody>();
            Parameters = new List<FspParameter>();
        }

        public string Name { get; set; }
        public List<FspCompositeBody> Body { get; private set; }
        public List<FspParameter> Parameters { get; private set; }

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