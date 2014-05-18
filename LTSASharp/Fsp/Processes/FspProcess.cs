using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Relabelling;

namespace LTSASharp.Fsp.Processes
{
    public class FspProcess : FspBaseProcess
    {
        public FspProcess()
        {
            Body = new MultiMap<string, FspLocalProcess>();

            AlphabetExtension = new List<IFspActionLabel>();
            Relabel = new FspRelabel();
        }

        public MultiMap<string, FspLocalProcess> Body { get; private set; }
        public List<IFspActionLabel> AlphabetExtension { get; private set; }

        public FspRelabel Relabel { get; set; }

        public override string ToString()
        {
            //TODO parameters

            return string.Join(",\n", Body.GetPairs().Select(x => x.Item1 + " = " + x.Item2));
        }
    }
}
