namespace LTSASharp.Fsp.Expressions
{
    internal class FspGreaterExpr : FspBinaryExpression
    {
        public FspGreaterExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l > r ? 1 : 0;
        }
    }
}