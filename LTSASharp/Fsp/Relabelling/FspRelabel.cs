using System.Collections.Generic;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Relabelling
{
    interface IFspRelabel
    {
    }

    public class FspRelabel : IFspRelabel
    {
        public FspRelabel()
        {
            Entries = new List<Entry>();
        }

        public List<Entry> Entries { get; private set; }

        public abstract class Entry
        {
            public abstract IEnumerable<Entry> Expand<TProcess>(FspExpanderEnvironment<TProcess> env)
                where TProcess : FspBaseProcess;
        }

        internal class DirectEntry : Entry
        {
            public DirectEntry(IFspActionLabel oldLabel, IFspActionLabel newLabel)
            {
                Old = oldLabel;
                New = newLabel;
            }

            public IFspActionLabel Old { get; set; }
            public IFspActionLabel New { get; set; }

            public override IEnumerable<Entry> Expand<TProcess>(FspExpanderEnvironment<TProcess> env)
            {
                var result = new List<Entry>();

                Old.Expand(env.ExprEnv, o => New.Expand(env.ExprEnv, n => result.Add(new DirectEntry(o, n))));

                return result;
            }
        }

        internal class IndexedEntry : Entry
        {
            public IndexedEntry()
            {
                Entries = new List<Entry>();
                Ranges = new List<FspRange>();
            }

            public List<FspRange> Ranges { get; private set; }
            public List<Entry> Entries { get; private set; }

            public override IEnumerable<Entry> Expand<TProcess>(FspExpanderEnvironment<TProcess> env)
            {
                throw new System.NotImplementedException();
            }
        }

        public FspRelabel Expand<TProcess>(FspExpanderEnvironment<TProcess> env)
            where TProcess : FspBaseProcess
        {
            var relabel = new FspRelabel();

            foreach (var entry in Entries)
                relabel.Entries.AddRange(entry.Expand(env));

            return relabel;
        }
    }
}
