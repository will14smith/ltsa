using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Relabelling;

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
    }
}