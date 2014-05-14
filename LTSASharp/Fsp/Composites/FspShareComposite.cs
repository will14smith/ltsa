using LTSASharp.Fsp.Labels;

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
    }
}