using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Ranges
{
    class FspBoundedRange : FspRange
    {
        public FspBoundedRange(FspExpression lower, FspExpression upper)
        {
            Lower = lower;
            Upper = upper;
        }

        public FspBoundedRange(int lowerValue, int upperValue)
        {
            Lower = new FspIntegerExpr(lowerValue);
            Upper = new FspIntegerExpr(upperValue);
        }

        public FspExpression Lower { get; set; }
        public FspExpression Upper { get; set; }

        public override string ToString()
        {
            return Lower + ".." + Upper;
        }

        public override FspRangeBounds GetBounds(FspExpressionEnvironment env)
        {
            return new FspRangeBounds(Lower.GetValue(env), Upper.GetValue(env));
        }
    }
}