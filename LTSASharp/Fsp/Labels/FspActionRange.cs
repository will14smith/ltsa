using System;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Labels
{
    class FspActionRange : FspRange
    {
        public string Target { get; set; }
        public FspRange Range { get; set; }

        public FspActionRange(string target, FspRange range)
        {
            Target = target;
            Range = range;
        }

        public FspActionRange(FspRange range)
        {
            Range = range;
        }

        public override string ToString()
        {
            return Target == null? Range.ToString() : Target + ":" + Range;
        }

        public override FspRangeBounds GetBounds(FspExpressionEnvironment env)
        {
            return Range.GetBounds(env);
        }
    }
}
