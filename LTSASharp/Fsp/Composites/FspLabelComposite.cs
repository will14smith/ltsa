using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
{
    public class FspLabelComposite : FspCompositeBody
    {
        public FspCompositeBody Body { get; private set; }
        public IFspActionLabel Label { get; private set; }

        public FspLabelComposite(FspCompositeBody body, IFspActionLabel label)
        {
            Body = body;
            Label = label;
        }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            var results = new List<FspCompositeBody>();

            // Expand label
            if (Label is FspRange)
            {
                var range = (FspRange)Label;

                range.Expand(env.ExprEnv, label => results.AddRange(new FspLabelComposite(Body, label).ExpandProcess(env)));

                return results;
            }
            // Convert to relabel
            Label.Expand(env.ExprEnv, label => results.AddRange(Body.ExpandProcess(env).Select(x => new FspPrefixRelabel(x, label))));

            return results;
        }
    }
}
