using System.Collections.Generic;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
{
    class FspMultiComposite : FspCompositeBody
    {
        public FspMultiComposite()
        {
            Composites = new List<FspCompositeBody>();
        }

        public List<FspCompositeBody> Composites { get; private set; }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            throw new System.NotImplementedException();
        }
    }
}