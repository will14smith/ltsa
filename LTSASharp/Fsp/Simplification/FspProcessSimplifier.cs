﻿using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Simplification
{
    class FspProcessSimplifier
    {
        private readonly FspProcess process;
        private FspLocalProcess current;

        private readonly FspLocalRefProcess selfLocalRef;

        public FspProcessSimplifier(FspProcess process)
        {
            this.process = process;

            selfLocalRef = new FspLocalRefProcess(process.Name);
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

            foreach (var b in process.Body)
            {
                if (b.Key == process.Name)
                    newProcess.Body.Map(process.Name, current);
                else
                    newProcess.Body.Add(b.Key, b.Value);
            }

            return newProcess;
        }

        private FspLocalProcess Simplify(FspLocalProcess local, bool top)
        {
            if (local is FspLocalRefProcess)
            {
                var name = Rewrite(((FspLocalRefProcess)local).Name);

                if (name == process.Name)
                    return selfLocalRef;

                var inline = process.Body[name].Single();

                if (top)
                    AddRewrite(name, process.Name);
                else if (!CanInline(name, inline))
                    return local;

                return inline;
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
                return Simplify((FspChoice)local);

            return local;
        }

        private bool CanInline(string name, FspLocalProcess local, ISet<FspLocalProcess> visited = null)
        {
            visited = visited ?? new HashSet<FspLocalProcess>();
            
            // true iff local isn't (self or mutually) recursive
            //TODO uncomment in FspConverter when done.
            throw new System.NotImplementedException();
        }

        private FspChoice Simplify(FspChoice local)
        {
            // should p2 be simplified?
            var p2 = Simplify(local.Process, false);

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

                if (name == newName)
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
