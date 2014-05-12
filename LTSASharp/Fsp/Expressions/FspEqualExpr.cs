namespace LTSASharp.Fsp.Expressions
{
    internal class FspEqualExpr : FspBinaryExpression
    {
        public FspEqualExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l == r ? 1 : 0;
        }
    }
}