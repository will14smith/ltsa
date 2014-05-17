using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    internal class FspLocalRefProcess : FspBaseLocalProcess
    {
        public string Name { get; private set; }
        public List<FspExpression> Indices { get; private set; }

        public FspLocalRefProcess(string name, IEnumerable<FspExpression> indices)
        {
            Name = name;
            Indices = indices.ToList();
        }

        public FspLocalRefProcess(string name)
        {
            Name = name;
            Indices = new List<FspExpression>();
        }

        public override string ToString()
        {
            return Name;
        }

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            var refProc = this;

            if (refProc.Name == env.Process.Name)
                refProc = new FspLocalRefProcess(env.Name, refProc.Indices);

            if (!refProc.Indices.Any())
                return refProc;

            var newIndices = refProc.Indices.Select(index => index.Evaluate(env.ExprEnv)).ToList();

            if (newIndices.All(x => x is FspIntegerExpr))
            {
                // flatten
                var newName = newIndices.Cast<FspIntegerExpr>().Aggregate(refProc.Name, (n, e) => n + "." + e.Value);

                return new FspLocalRefProcess(newName);
            }

            return new FspLocalRefProcess(refProc.Name, newIndices);
        }
    }
}