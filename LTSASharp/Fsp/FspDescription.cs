using System.Collections.Generic;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp
{
    class FspDescription
    {
        public FspDescription()
        {
            Processes = new List<FspProcess>();
            Composites = new List<FspComposite>();
        }

        public List<FspProcess> Processes { get; private set; }
        public List<FspComposite> Composites { get; private set; } 
    }
}
