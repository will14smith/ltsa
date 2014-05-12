namespace LTSASharp.Fsp.Expressions
{
    internal class FspLeftShiftExpr : FspBinaryExpression
    {
        public FspLeftShiftExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l << r;
        }
    }
}