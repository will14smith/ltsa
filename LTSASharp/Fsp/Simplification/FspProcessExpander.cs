using System.Collections.Generic;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Simplification
{
    class FspProcessExpander : IFspExpander<FspProcess>
    {
        private readonly FspProcess process;
        private readonly FspExpanderEnvironment<FspProcess> env;

        public FspProcessExpander(FspProcess process, FspDescription oldDesc, FspDescription newDesc)
        {
            this.process = process;

            env = new FspExpanderEnvironment<FspProcess>(process, oldDesc, newDesc);
        }
        public FspProcessExpander(FspProcess process, string name, FspExpressionEnvironment initialEnv, FspDescription oldDesc, FspDescription newDesc)
        {
            this.process = process;

            env = new FspExpanderEnvironment<FspProcess>(process, oldDesc, newDesc, initialEnv, name);
        }

        public FspProcess Expand()
        {
            var newProcess = new FspProcess { Name = env.Name };

            newProcess.AlphabetExtension.AddRange(process.AlphabetExtension);
            newProcess.Hiding.AddRange(process.Hiding);
            newProcess.HidingMode = process.HidingMode;

            foreach (var entry in process.Body)
            {
                foreach (var e in entry.Value)
                {
                    var newName = entry.Key;
                    if (newName == process.Name)
                        newName = env.Name;

                    if (e is FspIndexedProcess)
                    {
                        var results = ExpandIndexed(newName, (FspIndexedProcess)e);
                        foreach (var result in results)
                            newProcess.Body.Map(result.Key, result.Value);
                    }
                    else
                    {
                        newProcess.Body.Map(newName, e.ExpandProcess(env));
                    }
                }
            }

            return newProcess;
        }

        private Dictionary<string, FspLocalProcess> ExpandIndexed(string namePrefix, FspIndexedProcess value)
        {
            var results = new Dictionary<string, FspLocalProcess>();

            value.Index.Iterate(env.ExprEnv, val =>
            {
                var newName = namePrefix + "." + val;

                if (value.Process is FspIndexedProcess)
                {
                    var innerResults = ExpandIndexed(newName, (FspIndexedProcess)value.Process);
                    foreach (var result in innerResults)
                        results.Add(result.Key, result.Value);
                }
                else
                {
                    results.Add(newName, value.Process.ExpandProcess(env));
                }
            });

            return results;
        }
    }
}
