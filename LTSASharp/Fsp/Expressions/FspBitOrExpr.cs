namespace LTSASharp.Fsp.Expressions
{
    internal class FspBitOrExpr : FspBinaryExpression
    {
        public FspBitOrExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l | r;
        }
    }
}