namespace LTSASharp.Fsp.Expressions
{
    internal class FspDivideExpr : FspBinaryExpression
    {
        public FspDivideExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l / r;
        }
    }
}