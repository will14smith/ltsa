namespace LTSASharp.Fsp.Expressions
{
    internal class FspBitAndExpr : FspBinaryExpression
    {
        public FspBitAndExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l & r;
        }
    }
}