using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Simplification;

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

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            // forall[i:0..2] S[i] == (S[0] || S[1] || S[2])

            var results = new List<FspCompositeBody> { Body };

            foreach (var range in Range)
            {
                var newResults = new List<FspCompositeBody>();
                range.Iterate(env.ExprEnv, i =>
                {
                    foreach (var result in results)
                    {
                        newResults.AddRange(result.ExpandProcess(env));
                    }
                });
                results = newResults;
            }

            return results;
        }
    }
}
