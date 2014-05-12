using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Choices
{
    internal class FspChoices : FspLocalProcess
    {
        public FspChoices()
        {
            Children = new List<FspChoice>();
        }

        public List<FspChoice> Children { get; private set; }

        public override string ToString()
        {
            return string.Join(" | ", Children.Select(x => "( " + x + " )"));
        }
    }
}