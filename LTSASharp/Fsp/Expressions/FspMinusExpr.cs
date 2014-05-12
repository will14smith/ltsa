namespace LTSASharp.Fsp.Expressions
{
    internal class FspMinusExpr : FspBinaryExpression
    {
        public FspMinusExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l - r;
        }
    }
}