using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
{
    class FspRefComposite : FspCompositeBody
    {
        public string Name { get; private set; }
        public List<FspExpression> Arguments { get; private set; }


        public FspRefComposite(string name)
        {
            Name = name;
            Arguments = new List<FspExpression>();
        }

        public override string ToString()
        {
            return Name;
        }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            if (!Arguments.Any())
                return new FspCompositeBody[] { this };

            // return ref to expanded process
            return new FspCompositeBody[] { new FspRefComposite(ExpandProcessRef(Name, Arguments, env)) };
        }

        // NOTE: shared with FspRefProcess
        internal static string ExpandProcessRef<TProcess>(string name, List<FspExpression> arguments, FspExpanderEnvironment<TProcess> env)
            where TProcess : FspBaseProcess
        {
            var newRef = arguments.Aggregate(name, (n, arg) => n + ("." + arg.GetValue(env.ExprEnv)));

            if (!env.NewDesc.Processes.ContainsKey(newRef))
            {
                var paramProc = env.OldDesc.Processes[name];

                // populate arguments
                var paramEnv = new FspExpressionEnvironment(env.OldDesc);
                for (int i = 0; i < paramProc.Parameters.Count; i++)
                    paramEnv.PushVariable(paramProc.Parameters[i].Name, arguments[i].GetValue(env.ExprEnv));

                var expander = FspExpanderFactory.GetExpander(paramProc, newRef, paramEnv, env.OldDesc, env.NewDesc);

                // add expanded ref'd process to description
                env.NewDesc.Processes.Add(newRef, expander.Expand());
            }

            return newRef;
        }
    }
}
