using System.Linq;
using Antlr4.Runtime.Misc;

namespace LTSASharp.Fsp.Processes
{
    public class FspProcess : FspBaseProcess
    {
        public FspProcess()
        {
            Body = new MultiMap<string, FspLocalProcess>();
        }

        public MultiMap<string, FspLocalProcess> Body { get; private set; }

        // Alphabet ext
        // Relabl 
        // Hiding 

        public override string ToString()
        {
            //TODO parameters

            return string.Join(",\n", Body.GetPairs().Select(x => x.Item1 + " = " + x.Item2));
        }
    }
}
