namespace LTSASharp.Fsp.Expressions
{
    internal class FspVariableExpr : FspExpression
    {
        public string Name { get; private set; }

        public FspVariableExpr(string name)
        {
            Name = name;
        }

        public override FspExpression Evaluate(FspExpressionEnvironment env)
        {
            int val;
            if (env.Variables.TryGetValue(Name, out val))
                return new FspIntegerExpr(val);

            return this;
        }
    }
}