using System;

namespace LTSASharp.Fsp.Expressions
{
    abstract class FspBinaryExpression : FspExpression
    {
        public FspExpression Left { get; set; }
        public FspExpression Right { get; set; }

        protected FspBinaryExpression(FspExpression left, FspExpression right)
        {
            Left = left;
            Right = right;
        }

        public override FspExpression Evaluate(FspExpressionEnvironment env)
        {
            var left = Left.Evaluate(env);
            var right = Right.Evaluate(env);

            if (left is FspIntegerExpr && right is FspIntegerExpr)
                return new FspIntegerExpr(Apply(((FspIntegerExpr)left).Value, ((FspIntegerExpr)right).Value));

            return (FspExpression) Activator.CreateInstance(GetType(), left, right);
        }

        protected abstract int Apply(int l, int r);
    }
}