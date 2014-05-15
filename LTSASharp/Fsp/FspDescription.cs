using System.Collections.Generic;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp
{
    public class FspDescription
    {
        public FspDescription()
        {
            Processes = new Dictionary<string, FspBaseProcess>();
        }

        public Dictionary<string, FspBaseProcess> Processes { get; private set; }
    }
}
