using System;
using LTSASharp.Fsp.Labels;

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
    }
}
