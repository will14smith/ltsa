using System.Collections.Generic;
using System.Linq;

namespace LTSASharp.Fsp.Processes
{
    internal class FspSequenceProcess : FspBaseLocalProcess
    {
        public FspSequenceProcess()
        {
            Processes = new List<FspLocalProcess>();
        }

        public List<FspLocalProcess> Processes { get; private set; }

        public override string ToString()
        {
            return "(" + string.Join("; ", Processes.Select(x => x.ToString())) + ")";
        }
    }
}