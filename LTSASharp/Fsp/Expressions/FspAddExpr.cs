namespace LTSASharp.Fsp.Expressions
{
    internal class FspAddExpr : FspBinaryExpression
    {
        public FspAddExpr(FspExpression left, FspExpression right) : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l + r;
        }
    }
}