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

            value.Index.Iterate(env, val =>
            {
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
            });

            return results;
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

                return new FspChoices(choices);
            }

            if (value is FspRefProcess)
            {
                var refProc = (FspRefProcess)value;
                if (!refProc.Indices.Any())
                    return value;

                var newIndices = refProc.Indices.Select(index => index.Evaluate(env)).ToList();

                if (newIndices.All(x => x is FspIntegerExpr))
                {
                    // flatten
                    var name = newIndices.Cast<FspIntegerExpr>().Aggregate(refProc.Name, (n, e) => n + "." + e.Value);

                    return new FspRefProcess(name);
                }

                return new FspRefProcess(refProc.Name, newIndices);
            }
            if (value is FspEndProcess)
                return value;
            if (value is FspStopProcess)
                return value;

            throw new ArgumentException("Unexpected local process type", "value");
        }

        private readonly FspExpressionEnvironment env = new FspExpressionEnvironment();
        private IEnumerable<FspChoice> Expand(FspChoice value)
        {
            var result = new List<FspChoice>();

            //TODO guard evaluation

            value.Label.Expand(env, label =>
            {
                var choice = new FspChoice
                {
                    Label = label,
                    Process = Expand(value.Process)
                };

                result.Add(choice);
            });

            return result;
        }
    }
}
