using System.Collections.Generic;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Processes
{
    class FspRefProcess : FspLocalProcess
    {
        public string Name { get; private set; }
        public List<FspExpression> Arguments { get; private set; }

        public FspRefProcess(string name)
        {
            Name = name;
            Arguments = new List<FspExpression>();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
