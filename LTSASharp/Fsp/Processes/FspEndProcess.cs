using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    internal class FspEndProcess : FspBaseLocalProcess
    {
        public override string ToString()
        {
            return "END";
        }

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            return this;
        }
    }
}