namespace LTSASharp.Fsp.Expressions
{
    internal class FspBitExclOrExpr : FspBinaryExpression
    {
        public FspBitExclOrExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l ^ r;
        }
    }
}