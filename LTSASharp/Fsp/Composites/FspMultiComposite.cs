using System.Collections.Generic;

namespace LTSASharp.Fsp.Composites
{
    class FspMultiComposite : FspCompositeBody
    {
        public FspMultiComposite()
        {
            Composites = new List<FspCompositeBody>();
        }

        public List<FspCompositeBody> Composites { get; private set; } 
    }
}