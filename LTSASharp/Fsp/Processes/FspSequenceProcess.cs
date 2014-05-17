using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Simplification;

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

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            var newSeq = new FspSequenceProcess
            {
                Processes = Processes.Select(subProc => subProc.ExpandProcess(env)).ToList()
            };

            return newSeq;
        }
    }
}