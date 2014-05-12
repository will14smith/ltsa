namespace LTSASharp.Fsp.Expressions
{
    internal class FspRightShiftExpr : FspBinaryExpression
    {
        public FspRightShiftExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l >> r;
        }
    }
}