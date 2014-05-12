using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Ranges
{
    class FspExpressionRange : FspRange
    {
        public FspExpression Expr { get; private set; }

        public FspExpressionRange(FspExpression expr)
        {
            Expr = expr;
        }

        public override FspRangeBounds GetBounds(FspExpressionEnvironment env)
        {
            var val = Expr.GetValue(env);

            return new FspRangeBounds(val, val);
        }
    }
}
