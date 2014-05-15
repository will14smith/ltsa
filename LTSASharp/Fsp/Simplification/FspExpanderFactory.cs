using System;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp.Simplification
{
    class FspExpanderFactory
    {
        public static IFspExpander<TProcess> GetExpander<TProcess>(TProcess p, string n, FspExpressionEnvironment e, FspDescription oldDesc, FspDescription newDesc)
            where TProcess : FspBaseProcess
        {
            var process = p as FspProcess;
            if (process != null)
                return (IFspExpander<TProcess>)new FspProcessExpander(process, n, e);

            var composite = p as FspComposite;
            if (composite != null)
                return (IFspExpander<TProcess>)new FspCompositeExpander(composite, n, e, oldDesc, newDesc);

            throw new ArgumentException("Unexpected process type", "p");
        }

        public static IFspExpander<TProcess> GetExpander<TProcess>(TProcess p, FspDescription oldDesc, FspDescription newDesc)
            where TProcess : FspBaseProcess
        {
            var process = p as FspProcess;
            if (process != null)
                return (IFspExpander<TProcess>)new FspProcessExpander(process);

            var composite = p as FspComposite;
            if (composite != null)
                return (IFspExpander<TProcess>)new FspCompositeExpander(composite, oldDesc, newDesc);

            throw new ArgumentException("Unexpected process type", "p");
        }
    }
}