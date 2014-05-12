namespace LTSASharp.Fsp.Expressions
{
    internal class FspModuloExpr : FspBinaryExpression
    {
        public FspModuloExpr(FspExpression left, FspExpression right)
            : base(left, right)
        {

        }

        protected override int Apply(int l, int r)
        {
            return l % r;
        }
    }
}