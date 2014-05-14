using System;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Labels
{
    public interface IFspActionLabel
    {
        void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action);
    }
}
