using System.Collections.Generic;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Relabelling
{
    class FspPrefixRelabel : FspCompositeBody, IFspRelabel
    {
        public FspCompositeBody Body { get; set; }
        public IFspActionLabel Label { get; set; }

        public FspPrefixRelabel(FspCompositeBody body, IFspActionLabel label)
        {
            Body = body;
            Label = label;
        }

        public override string ToString()
        {
            return Label + ":" + Body;
        }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            throw new System.NotImplementedException();
        }
    }
}