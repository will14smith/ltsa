using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    internal class FspChoices : FspLocalProcess
    {
        public FspChoices(List<FspChoice> choices)
        {
            Children = choices;
        }
        public FspChoices()
        {
            Children = new List<FspChoice>();
        }

        public List<FspChoice> Children { get; private set; }

        public override string ToString()
        {
            return string.Join(" | ", Children.Select(x => "( " + x + " )"));
        }

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            var newChoices = new FspChoices();

            foreach (var c in Children)
            {
                var expanded = c.ExpandProcess(env);

                if (expanded is FspChoices)
                    newChoices.Children.AddRange(((FspChoices)expanded).Children);
                else
                    newChoices.Children.Add((FspChoice)expanded);
            }

            return newChoices;
        }
    }
}