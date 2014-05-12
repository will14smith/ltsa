namespace LTSASharp.Fsp.Expressions
{
    class FspOrExpr : FspBinaryExpression
    {
        public FspOrExpr(FspExpression left, FspExpression right) : base(left, right)
        {
        }

        protected override int Apply(int l, int r)
        {
            return l == 1 || r == 1 ? 1 : 0;
        }
    }
}
