using System.Text;

namespace LTSASharp.Fsp.Expressions
{
    internal class FspNotExpr : FspExpression
    {
        public FspExpression Expr { get; private set; }

        public FspNotExpr(FspExpression expr)
        {
            Expr = expr;
        }

        public override FspExpression Evaluate(FspExpressionEnvironment env)
        {
            var expr = Expr.Evaluate(env);

            if (!(expr is FspIntegerExpr))
                return new FspNotExpr(expr);

            var val = ((FspIntegerExpr)expr).Value;

            return new FspIntegerExpr(val == 0 ? 1 : 0);
        }
    }
}