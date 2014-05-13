using LTSASharp.Fsp.Ranges;

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
    }
}
