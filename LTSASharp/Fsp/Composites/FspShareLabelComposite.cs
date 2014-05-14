using LTSASharp.Fsp.Labels;

namespace LTSASharp.Fsp.Composites
{
    public class FspShareLabelComposite : FspCompositeBody
    {
        public FspCompositeBody Body { get; set; }
        public IFspActionLabel Share { get; set; }
        public IFspActionLabel Label { get; set; }

        public FspShareLabelComposite(FspCompositeBody body, IFspActionLabel share, IFspActionLabel label)
        {
            Body = body;
            Share = share;
            Label = label;
        }
    }
}