using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Choices
{
    internal class FspChoice : FspLocalProcess
    {
        public IFspActionLabel Label { get; set; }
        public FspLocalProcess Process { get; set; }

        public FspExpression Guard { get; set; }
 
        public override string ToString()
        {
            return Label + " -> " + Process;
        }
    }
}