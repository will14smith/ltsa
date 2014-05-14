using System;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Fsp.Processes
{
    public abstract class FspLocalProcess : IFspActionLabel
    {
        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            //TODO is this correct?
            action(this);
        }
    }
}