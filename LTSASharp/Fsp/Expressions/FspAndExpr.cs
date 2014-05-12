namespace LTSASharp.Fsp.Expressions
{
    internal class FspAndExpr : FspBinaryExpression
    {
        public FspAndExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {
        }

        protected override int Apply(int l, int r)
        {
            return l == 1 && r == 1 ? 1 : 0;
        }
    }
}