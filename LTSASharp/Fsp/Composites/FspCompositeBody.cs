using System.Collections.Generic;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Composites
{
    public abstract class FspCompositeBody : IFspCompositeExpandable
    {
        public abstract IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env);
    }
}