using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Relabelling
{
    class FspRelabelComposite : FspCompositeBody
    {
        public FspRelabelComposite(FspCompositeBody body, FspRelabel relabel)
        {
            Relabel = relabel;
            Body = body;
        }

        public FspCompositeBody Body { get; private set; }
        public FspRelabel Relabel { get; private set; }

        public override IList<FspCompositeBody> ExpandProcess(FspExpanderEnvironment<FspComposite> env)
        {
            var processes = Body.ExpandProcess(env);

            var relabel = Relabel.Expand(env);

            return processes.Select(x => new FspRelabelComposite(x, relabel)).ToList<FspCompositeBody>();
        }
    }
}
