using System.Collections.Generic;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Relabelling;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
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