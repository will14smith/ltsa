using System;
using LTSASharp.Fsp.Composites;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspCompositeConverter : FspBaseConverter<FspCompositeBody>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspComposite composite;

        private int block;

        public FspCompositeConverter(FspConverterEnvironment env, FspComposite composite)
        {
            this.env = env;
            this.composite = composite;
        }

        public override FspCompositeBody VisitCompositeBody(FSPActualParser.CompositeBodyContext context)
        {
            // prefixLabel? (processRef | LRound parallelComposition RRound) relabel?
            // prefixLabel?  relabel?
            // ForAll ranges compositeBody  //replication
            // If expression Then compositeBody (Else compositeBody)?

            Unimpl(context.prefixLabel());
            Unimpl(context.ForAll());
            Unimpl(context.If());

            if (context.processRef() != null)
            {
                var result = context.processRef().Accept(this);
                //TODO handle prefix

                if (block == 0)
                {
                    composite.Body.Add(result);
                }

                return result;
            }

            if (context.parallelComposition() != null)
            {
                var result = (FspMultiComposite)context.parallelComposition().Accept(this);
                //TODO handle prefix

                if (block == 0)
                {
                    foreach (var c in result.Composites)
                        composite.Body.Add(c);
                }

                return result;
            }


            throw new InvalidOperationException();
        }

        public override FspCompositeBody VisitParallelComposition(FSPActualParser.ParallelCompositionContext context)
        {
            var result = new FspMultiComposite();
            block++;

            foreach (var c in context.compositeBody())
                result.Composites.Add(c.Accept(this));

            block--;
            return result;
        }

        public override FspCompositeBody VisitProcessRef(FSPActualParser.ProcessRefContext context)
        {
            // UpperCaseIdentifier argument?

            var name = context.UpperCaseIdentifier();

            Unimpl(context.argument());

            return new FspRefComposite(name.GetText());
        }
    }
}