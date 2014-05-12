using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Simplification
{
    class FspProcessExpander
    {
        private readonly FspProcess process;

        public FspProcessExpander(FspProcess process)
        {
            this.process = process;
        }

        public FspProcess Expand()
        {
            var newProcess = new FspProcess { Name = process.Name };

            foreach (var entry in process.Body)
            {
                newProcess.Body.Add(entry.Key, Expand(entry.Value));
            }

            return newProcess;
        }

        private FspLocalProcess Expand(FspLocalProcess value)
        {
            if (value is FspChoices)
            {
                var newChoices = new FspChoices();

                foreach (var c in ((FspChoices)value).Children)
                {
                    newChoices.Children.AddRange(Expand(c));
                }

                return newChoices;
            }

            if (value is FspChoice)
            {
                var choices = Expand((FspChoice)value).ToList();

                if (choices.Count == 1)
                    return choices[0];

                var result = new FspChoices();
                result.Children.AddRange(choices);
                return result;
            }

            if (value is FspRefProcess)
                return value;
            if (value is FspEndProcess)
                return value;
            if (value is FspStopProcess)
                return value;

            throw new NotImplementedException();
        }

        private FspExpressionEnvironment env = new FspExpressionEnvironment();
        private List<FspChoice> Expand(FspChoice value)
        {
            var result = new List<FspChoice>();

            // set expansion
            // range expansion (including guard evaluation)

            if (value.Label is FspActionName)
            {
                var choice = new FspChoice
                {
                    Label = value.Label,
                    Process = Expand(value.Process)
                };

                result.Add(choice);

                return result;
            }

            if (value.Label is FspActionSet)
            {
                var items = ((FspActionSet)value.Label).Items;

                foreach (var item in items)
                {
                    var choice = new FspChoice
                    {
                        Label = item,
                        Process = Expand(value.Process)
                    };

                    result.AddRange(Expand(choice));
                }

                return result;
            }

            if (value.Label is FspFollowAction)
            {
                var follow = (FspFollowAction)value.Label;
                follow = follow.MakeTailHeavy();

                var head = follow.Head;
                var tail = follow.Tail;

                // expand tail + proc
                var tmp = new FspChoice { Label = tail, Process = value.Process };

                var expanded = Expand(tmp);

                // expand head
                if (head is FspRange)
                {
                    foreach (var c in expanded)
                    {
                        tmp = new FspChoice { Label = head, Process = c.Process };
                        var expanded2 = Expand(tmp);

                        foreach (var x in expanded2)
                        {
                            var label = new FspFollowAction(x.Label, c.Label);
                            var r = new FspChoice { Label = label, Process = x.Process };

                            result.Add(r);
                        }
                    }
                }
                else
                {
                    foreach (var c in expanded)
                    {
                        var label = c.Label == null ? head : new FspFollowAction(head, c.Label);
                        var r = new FspChoice { Label = label, Process = c.Process };

                        result.Add(r);
                    }
                }

                return result;
            }

            if (value.Label is FspActionRange)
            {
                var range = (FspActionRange)value.Label;
                var bounds = range.Range.GetBounds(env);

                for (var i = bounds.Lower; i <= bounds.Upper; i++)
                {
                    env.PushVariable(range.Target, i);

                    result.Add(new FspChoice { Label = new FspActionName(i.ToString()), Process = Expand(value.Process) });

                    env.PopVariable(range.Target);
                }

                return result;
            }

            if (value.Label is FspExpressionRange)
            {
                var range = (FspExpressionRange)value.Label;
                var expr = range.Expr.Evaluate(env);

                IFspActionLabel label;
                if (expr is FspIntegerExpr)
                    label = new FspActionName(((FspIntegerExpr)expr).Value.ToString());
                else
                    label = new FspExpressionRange(expr);

                return new List<FspChoice> { new FspChoice { Label = label, Process = Expand(value.Process) } };
            }

            throw new NotImplementedException();
        }
    }
}
