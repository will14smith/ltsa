using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Processes
{
    class FspProcess
    {
        public FspProcess()
        {
            Body = new Dictionary<string, FspLocalProcess>();
        }

        public string Name { get; set; }
        public Dictionary<string, FspLocalProcess> Body { get; private set; }

        // Alphabet ext
        // Relabl 
        // Hiding 

        public override string ToString()
        {
            return string.Join(",\n", Body.Select(x => x.Key + " = " + x.Value));
        }
    }

    class FspComposite
    {
        public string Name { get; set; }
        public List<FspProcess> Body { get; set; }
        // Relabel
        // Prio
        // Hiding
    }
}
