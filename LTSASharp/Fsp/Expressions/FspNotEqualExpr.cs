namespace LTSASharp.Fsp.Expressions
{
    internal class FspNotEqualExpr : FspBinaryExpression
    {
        public FspNotEqualExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l != r ? 1 : 0;
        }
    }
}