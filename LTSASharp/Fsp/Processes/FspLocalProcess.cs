using System;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Simplification;

namespace LTSASharp.Fsp.Processes
{
    public abstract class FspLocalProcess : IFspActionLabel, IFspProcessExpandable
    {
        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            //TODO is this correct?
            action(this);
        }

        public abstract FspLocalProcess ExpandProcess(FspExpanderEnvironment<FspProcess> env);
    }
}