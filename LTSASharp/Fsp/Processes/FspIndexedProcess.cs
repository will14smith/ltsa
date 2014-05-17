using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    class FspIndexedProcess : FspLocalProcess
    {
        public FspLocalProcess Process { get; private set; }
        public FspRange Index { get; private set; }

        public FspIndexedProcess(FspLocalProcess process, FspRange index)
        {
            Process = process;
            Index = index;
        }

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            throw new System.NotImplementedException();
        }
    }
}
