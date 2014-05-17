using System.Collections.Generic;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp
{
    public class FspDescription
    {
        public FspDescription()
        {
            Processes = new Dictionary<string, FspBaseProcess>();

            Constants = new Dictionary<string, int>();
            Ranges = new Dictionary<string, FspRange>();
        }

        public Dictionary<string, FspBaseProcess> Processes { get; private set; }

        internal Dictionary<string, int> Constants { get; private set; }
        internal Dictionary<string, FspRange> Ranges { get; private set; }
    }
}
