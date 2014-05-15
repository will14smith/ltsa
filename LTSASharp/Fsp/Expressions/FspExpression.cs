using System;

namespace LTSASharp.Fsp.Expressions
{
    public abstract class FspExpression
    {
        public abstract FspExpression Evaluate(FspExpressionEnvironment env);

        public int GetValue(FspExpressionEnvironment env)
        {
            var result = Evaluate(env);
            if (!(result is FspIntegerExpr))
                throw new InvalidOperationException();

            return ((FspIntegerExpr)result).Value;
        }
    }
}
