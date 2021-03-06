﻿using System;
using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Relabelling;
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
            // ForAll ranges compositeBody  //replication
            // If expression Then compositeBody (Else compositeBody)?

            if (context.ForAll() != null)
            {
                var ranges = context.ranges().actionRange().Select(x => x.Accept(new FspRangeConverter(env)));

                block++;
                var body = context.compositeBody(0).Accept(this);
                block--;

                var result = new FspRangeComposition(ranges, body);

                if (block == 0)
                    composite.Body.Add(result);

                return result;
            }

            Unimpl(context.If());

            Func<FspCompositeBody, FspCompositeBody> prefixFunc = c => c;

            var prefix = context.prefixLabel();
            if (prefix != null)
            {
                var label1 = prefix.actionLabels(0).Accept(new FspLabelConverter(env));
                if (prefix.ColonColon() != null)
                {
                    var al2 = prefix.actionLabels(1);
                    if (al2 != null)
                    {
                        // ActionLabels :: ActionLabel :
                        var label2 = al2.Accept(new FspLabelConverter(env));
                        prefixFunc = c => new FspShareComposite(new FspLabelComposite(c, label2), label1);
                    }
                    else
                    {
                        // ActionLabels ::
                        prefixFunc = c => new FspShareComposite(c, label1);
                    }
                }
                else
                {
                    // ActionLabels :
                    prefixFunc = c => new FspLabelComposite(c, label1);
                }
            }

            if (context.relabel() != null)
            {
                var relabel = context.relabel().Accept(new FspRelabelConverter(env));

                var oldPrefixFunc = prefixFunc;

                prefixFunc = c => new FspRelabelComposite(oldPrefixFunc(c), relabel);
            }

            if (context.processRef() != null)
            {
                var result = context.processRef().Accept(this);

                if (block == 0)
                {
                    composite.Body.Add(prefixFunc(result));
                }

                return prefixFunc(result);
            }

            if (context.parallelComposition() != null)
            {
                var result = (FspMultiComposite)context.parallelComposition().Accept(this);

                if (block == 0)
                {
                    foreach (var c in result.Composites)
                        composite.Body.Add(prefixFunc(c));
                }

                return prefixFunc(result);
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

            var reference = new FspRefComposite(name.GetText());

            if (context.argument() != null)
                foreach (var arg in context.argument().argumentList().expression())
                {
                    reference.Arguments.Add(arg.Accept(new FspExpressionConverter(env)));
                }

            return reference;
        }
    }
}