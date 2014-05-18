using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Relabelling;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
{
    public class FspShareComposite : FspCompositeBody
    {
        public FspCompositeBody Body { get; set; }
        public IFspActionLabel Label { get; set; }

        public FspShareComposite(FspCompositeBody body, IFspActionLabel label)
        {
            Body = body;
            Label = label;
        }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            var compShare = this;

            // Convert to relabel
            return compShare.Body.ExpandProcess(env).Select(x => new FspPrefixRelabel(x, compShare.Label)).ToList<FspCompositeBody>();

        }
    }
}