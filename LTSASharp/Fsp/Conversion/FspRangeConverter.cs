using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    class FspRangeConverter : FspBaseConverter<FspRange>
    {
        private readonly FspConverterEnvironment env;

        public FspRangeConverter(FspConverterEnvironment env)
        {
            this.env = env;
        }

        public override FspRange VisitActionRange(FSPActualParser.ActionRangeContext context)
        {
            var range = context.range() != null
                ? context.range().Accept(this)
                : context.set().Accept(this);

            var name = context.LowerCaseIdentifier();

            return name != null
                ? new FspActionRange(name.GetText(), range)
                : new FspActionRange(range);
        }

        public override FspRange VisitRange(FSPActualParser.RangeContext context)
        {
            if (context.UpperCaseIdentifier() != null)
            {
                return new FspRefRange(context.UpperCaseIdentifier().GetText());
            }

            var lower = context.expression(0).Accept(new FspExpressionConverter(env));
            var upper = context.expression(1).Accept(new FspExpressionConverter(env));

            return new FspBoundedRange(lower, upper);
        }
    }
}
