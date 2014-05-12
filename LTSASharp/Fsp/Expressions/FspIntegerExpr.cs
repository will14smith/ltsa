using System;

namespace LTSASharp.Fsp.Expressions
{
    internal class FspIntegerExpr : FspExpression
    {
        public int Value { get; private set; }

        public FspIntegerExpr(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override FspExpression Evaluate(FspExpressionEnvironment env)
        {
            return this;
        }
    }
}