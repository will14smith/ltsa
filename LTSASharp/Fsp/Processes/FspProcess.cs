using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace LTSASharp.Fsp.Processes
{
    public class FspProcess
    {
        public FspProcess()
        {
            Body = new MultiMap<string, FspLocalProcess>();
            Parameters = new List<FspParameter>();
        }

        public string Name { get; set; }
        public MultiMap<string, FspLocalProcess> Body { get; private set; }
        public List<FspParameter> Parameters { get; private set; }

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
