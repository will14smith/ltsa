using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Simplification
{
    class FspProcessExpander : IFspExpander<FspProcess>
    {
        private readonly FspProcess process;
        private readonly string name;
        private readonly FspExpressionEnvironment env;

        public FspProcessExpander(FspProcess process)
        {
            this.process = process;

            name = process.Name;
            env = new FspExpressionEnvironment();

            foreach (var param in process.Parameters)
            {
                var value = param.DefaultValue.GetValue(env);

                name += "." + value;
                env.PushVariable(param.Name, value);
            }
        }
        public FspProcessExpander(FspProcess process, string name, FspExpressionEnvironment initialEnv)
        {
            this.process = process;
            this.name = name;

            env = initialEnv;
        }

        public FspProcess Expand()
        {
            var newProcess = new FspProcess { Name = name };

            foreach (var entry in process.Body)
            {
                foreach (var e in entry.Value)
                {
                    var newName = entry.Key;
                    if (newName == process.Name)
                        newName = name;

                    if (e is FspIndexedProcess)
                    {
                        var results = ExpandIndexed(newName, (FspIndexedProcess)e);
                        foreach (var result in results)
                            newProcess.Body.Map(result.Key, result.Value);
                    }
                    else
                    {
                        newProcess.Body.Map(newName, Expand(e));
                    }}
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

            if (value is FspLocalRefProcess)
            {
                var refProc = (FspLocalRefProcess)value;

                if(refProc.Name == process.Name)
                    refProc = new FspLocalRefProcess(name, refProc.Indices);

                if (!refProc.Indices.Any())
                    return value;

                var newIndices = refProc.Indices.Select(index => index.Evaluate(env)).ToList();

                if (newIndices.All(x => x is FspIntegerExpr))
                {
                    // flatten
                    var newName = newIndices.Cast<FspIntegerExpr>().Aggregate(refProc.Name, (n, e) => n + "." + e.Value);

                    return new FspLocalRefProcess(newName);
                }

                return new FspLocalRefProcess(refProc.Name, newIndices);
            }
            if (value is FspEndProcess)
                return value;
            if (value is FspStopProcess)
                return value;

            throw new ArgumentException("Unexpected local process type", "value");
        }

        private IEnumerable<FspChoice> Expand(FspChoice value)
        {
            var result = new List<FspChoice>();

            value.Label.Expand(env, label =>
            {
                if (value.Guard != null && value.Guard.GetValue(env) == 0)
                    return;

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
