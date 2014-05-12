namespace LTSASharp.Fsp.Expressions
{
    internal class FspMultiplyExpr : FspBinaryExpression
    {
        public FspMultiplyExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l * r;
        }
    }
}