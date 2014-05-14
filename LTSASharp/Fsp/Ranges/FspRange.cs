using System;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Fsp.Ranges
{
    abstract class FspRange : IFspActionLabel
    {
        public abstract FspRangeBounds GetBounds(FspExpressionEnvironment env);

        public virtual void Iterate(FspExpressionEnvironment env, Action<int> action)
        {
            var bounds = GetBounds(env);

            for (var i = bounds.Lower; i <= bounds.Upper; i++)
            {
                action(i);
            }
        }

        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            Iterate(env, i => action(new FspActionName(i.ToString())));
        }
    }

    internal class FspRangeBounds
    {
        public FspRangeBounds(int lower, int upper)
        {
            Upper = upper;
            Lower = lower;
        }

        public int Lower { get; private set; }
        public int Upper { get; private set; }
    }
}
