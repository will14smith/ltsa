using System.Collections.Generic;

namespace LTSASharp.Fsp.Processes
{
    internal class FspSequenceProcess : FspBaseLocalProcess
    {
        public FspSequenceProcess()
        {
            Processes = new List<FspLocalProcess>();
        }

        public List<FspLocalProcess> Processes { get; private set; } 
    }
}