namespace LTSASharp.Fsp.Expressions
{
    internal class FspNegateExpr : FspExpression
    {
        public FspExpression Expr { get; set; }

        public FspNegateExpr(FspExpression expr)
        {
            Expr = expr;
        }

        public override FspExpression Evaluate(FspExpressionEnvironment env)
        {
            var expr = Expr.Evaluate(env);

            if (!(expr is FspIntegerExpr))
                return new FspNegateExpr(expr);

            var val = ((FspIntegerExpr)expr).Value;

            return new FspIntegerExpr(-val);

        }
    }
}