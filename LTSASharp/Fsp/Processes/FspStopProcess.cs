using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    internal class FspStopProcess : FspBaseLocalProcess
    {
        public override string ToString()
        {
            return "STOP";
        }

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            return this;
        }
    }
}