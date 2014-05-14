using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Composites
{
    class FspRangeComposition : FspCompositeBody
    {
        public List<FspRange> Range { get; private set; }
        public FspCompositeBody Body { get; private set; }

        public FspRangeComposition(IEnumerable<FspRange> range, FspCompositeBody body)
        {
            Range = range.ToList();
            Body = body;
        }
    }
}
