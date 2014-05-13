using System.Linq;
using Antlr4.Runtime.Misc;

namespace LTSASharp.Fsp.Processes
{
    public class FspProcess
    {
        public FspProcess()
        {
            Body = new MultiMap<string, FspLocalProcess>();
        }

        public string Name { get; set; }
        public MultiMap<string, FspLocalProcess> Body { get; private set; }

        // Alphabet ext
        // Relabl 
        // Hiding 

        public override string ToString()
        {
            return string.Join(",\n", Body.GetPairs().Select(x => x.Item1 + " = " + x.Item2));
        }
    }
}
