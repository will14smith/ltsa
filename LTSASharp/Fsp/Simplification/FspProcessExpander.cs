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
                foreach (var e in entry.Value)
                    if (e is FspIndexedProcess)
                    {
                        var results = ExpandIndexed(entry.Key, (FspIndexedProcess)e);
                        foreach (var result in results)
                            newProcess.Body.Map(result.Key, result.Value);
                    }
                    else
                    {
                        newProcess.Body.Map(entry.Key, Expand(e));
                    }
            }

            return newProcess;
        }

        private Dictionary<string, FspLocalProcess> ExpandIndexed(string name, FspIndexedProcess value)
        {
            var results = new Dictionary<string, FspLocalProcess>();

            if (value.Index is FspActionRange)
            {
                var range = (FspActionRange)value.Index;
                var bounds = range.Range.GetBounds(env);

                for (var i = bounds.Lower; i <= bounds.Upper; i++)
                {
                    env.PushVariable(range.Target, i);

                    var newName = name + "." + i;
                    if (value.Process is FspIndexedProcess)
                    {
                        var innerResults = ExpandIndexed(newName, (FspIndexedProcess)value.Process);
                        foreach (var result in innerResults)
                            results.Add(result.Key, result.Value);
                    }
                    else
                    {
                        results.Add(newName, Expand(value.Process));
                    }

                    env.PopVariable(range.Target);
                }

                return results;
            }

            if (value.Index is FspExpressionRange)
            {
                var range = (FspExpressionRange)value.Index;
                var val = range.Expr.GetValue(env);

                var newName = name + "." + val;

                if (value.Process is FspIndexedProcess)
                {
                    var innerResults = ExpandIndexed(newName, (FspIndexedProcess)value.Process);
                    foreach (var result in innerResults)
                        results.Add(result.Key, result.Value);
                }
                else
                {
                    results.Add(newName, Expand(value.Process));
                }

                return results;
            }

            throw new InvalidOperationException();
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
            {
                var refProc = (FspRefProcess)value;
                if (!refProc.Indices.Any())
                    return value;

                var newIndices = refProc.Indices.Select(index => index.Evaluate(env)).ToList();

                if (newIndices.All(x => x is FspIntegerExpr))
                {
                    // flattern
                    var name = newIndices.Cast<FspIntegerExpr>().Aggregate(refProc.Name, (n, e) => n + "." + e.Value);

                    return new FspRefProcess(name);
                }

                return new FspRefProcess(refProc.Name, newIndices);
            }
            if (value is FspEndProcess)
                return value;
            if (value is FspStopProcess)
                return value;

            throw new NotImplementedException();
        }

        private readonly FspExpressionEnvironment env = new FspExpressionEnvironment();
        private IEnumerable<FspChoice> Expand(FspChoice value)
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

                        result.AddRange(from x in expanded2
                                        let label = new FspFollowAction(x.Label, c.Label)
                                        select new FspChoice { Label = label, Process = x.Process });
                    }
                }
                else
                {
                    result.AddRange(from c in expanded
                                    let label = c.Label == null ? head : new FspFollowAction(head, c.Label)
                                    select new FspChoice { Label = label, Process = c.Process });
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
