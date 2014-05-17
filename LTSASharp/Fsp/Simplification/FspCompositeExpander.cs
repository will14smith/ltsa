using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Simplification
{
    class FspCompositeExpander : IFspExpander<FspComposite>
    {
        private readonly FspComposite composite;
        private readonly FspExpanderEnvironment<FspComposite> env;

        public FspCompositeExpander(FspComposite process, FspDescription oldDesc, FspDescription newDesc)
        {
            composite = process;

            env = new FspExpanderEnvironment<FspComposite>(process, oldDesc, newDesc);
        }
        public FspCompositeExpander(FspComposite process, string name, FspExpressionEnvironment initialEnv, FspDescription oldDesc, FspDescription newDesc)
        {
            composite = process;

            env = new FspExpanderEnvironment<FspComposite>(process, oldDesc, newDesc, initialEnv, name);
        }

        public FspComposite Expand()
        {
            var newComposite = new FspComposite { Name = env.Name };
            
            foreach (var result in composite.Body.Select(x=>x.ExpandProcess(env)).SelectMany(r => r))
            {
                newComposite.Body.Add(result);
            }

            return newComposite;
        }
    }
}
