using System.Collections.Generic;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
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

        public override FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env)
        {
            var choices = new List<FspChoice>();

            Label.Expand(env.ExprEnv, label =>
            {
                if (Guard != null && Guard.GetValue(env.ExprEnv) == 0)
                    return;

                var choice = new FspChoice
                {
                    Label = label,
                    Process = Process.ExpandProcess(env)
                };

                choices.Add(choice);
            });


            if (choices.Count == 1)
                return choices[0];

            return new FspChoices(choices);
        }
    }
}