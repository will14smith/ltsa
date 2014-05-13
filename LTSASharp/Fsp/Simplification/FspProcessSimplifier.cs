using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Simplification
{
    class FspProcessSimplifier
    {
        private readonly FspProcess process;
        private FspLocalProcess current;

        private readonly FspRefProcess selfRef;

        public FspProcessSimplifier(FspProcess process)
        {
            this.process = process;

            selfRef = new FspRefProcess(process.Name);
        }

        public FspProcess Simplify()
        {
            var newProcess = new FspProcess { Name = process.Name };

            FspLocalProcess last = null;
            current = process.Body[process.Name].Single();

            while (last != current)
            {
                last = current;
                current = Simplify(current, true);
            }

            var references = GetReferences();

            foreach (var b in process.Body)
            {
                if (b.Key == process.Name)
                    newProcess.Body.Map(process.Name, current);
                else //TODO if(references.Contains(b.Key))
                    newProcess.Body.Add(b.Key, b.Value);
            }

            return newProcess;
        }

        private List<string> GetReferences()
        {
            //TODO Get all the reachable local processes
            return new List<string>();
        }

        private FspLocalProcess Simplify(FspLocalProcess local, bool top)
        {
            if (local is FspRefProcess)
            {
                var name = Rewrite(((FspRefProcess)local).Name);

                if (name == process.Name)
                    return selfRef;

                if (top)
                    AddRewrite(name, process.Name);

                return process.Body[name].Single();
            }

            if (local is FspChoices)
            {
                var newChoices = new FspChoices();
                var changedChoices = false;

                foreach (var c in ((FspChoices)local).Children)
                {
                    var c2 = Simplify(c);

                    changedChoices |= c2 != c;

                    newChoices.Children.Add(c2);
                }

                return changedChoices ? newChoices : local;
            }

            if (local is FspChoice)
                return Simplify((FspChoice) local);

            return local;
        }

        private FspChoice Simplify(FspChoice local)
        {
            //TODO? var p2 = Simplify(local.Process, false);
            var p2 = local.Process;

            IFspActionLabel label = local.Label;
            if (local.Label is FspFollowAction)
            {
                var fa = (FspFollowAction)local.Label;
                label = fa.MergeDown();
            }

            if (p2 != local.Process || label != local.Label)
                return new FspChoice { Label = label, Process = p2 };

            return local;
        }


        private readonly Dictionary<string, string> rewrite = new Dictionary<string, string>();

        private string Rewrite(string name)
        {
            while (true)
            {
                string newName;
                if (!rewrite.TryGetValue(name, out newName))
                    return name;

                name = newName;
            }
        }

        private void AddRewrite(string oldName, string newName)
        {
            rewrite.Add(oldName, newName);
        }
    }
}
