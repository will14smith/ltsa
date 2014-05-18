using System.Collections.Generic;
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
        }

        internal class DirectEntry : Entry
        {
            public IFspActionLabel Old { get; set; }
            public IFspActionLabel New { get; set; }
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
        }
    }
}
