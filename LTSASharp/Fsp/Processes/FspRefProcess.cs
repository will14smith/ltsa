using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Simplification;

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

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            if (!Arguments.Any())
                return this;

            // return ref to expanded process
            return new FspRefProcess(ExpandProcessRef(Name, Arguments, env));
        }

        private static string ExpandProcessRef<TProcess>(string name, List<FspExpression> arguments, FspExpanderEnvironment<TProcess> env) where TProcess : FspBaseProcess
        {
            return FspRefComposite.ExpandProcessRef(name, arguments, env);
        }
    }
}
