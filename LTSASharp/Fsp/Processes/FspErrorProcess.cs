using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    internal class FspErrorProcess : FspBaseLocalProcess
    {
        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            return this;
        }
    }
}