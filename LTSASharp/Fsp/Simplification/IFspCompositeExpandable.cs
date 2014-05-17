using System.Collections.Generic;
using LTSASharp.Fsp.Composites;

namespace LTSASharp.Fsp.Simplification
{
    internal interface IFspCompositeExpandable : IFspExpandable<FspComposite, IList<FspCompositeBody>>
    {
    }
}