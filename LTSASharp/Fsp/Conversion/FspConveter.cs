using System;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Simplification;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    public class FspConveter : FspBaseConverter
    {
        private readonly FspConverterEnvironment env;
        public FspDescription Description { get; private set; }

        public FspConveter()
        {
            env = new FspConverterEnvironment();
        }

        public override bool VisitFsp_definition(FSPActualParser.Fsp_definitionContext context)
        {
            // always has 1 child
            return context.children[0].Accept(this);
        }

        public override bool VisitFsp_description(FSPActualParser.Fsp_descriptionContext context)
        {
            Description = new FspDescription();

            foreach (var x in context.fsp_definition())
            {
                if (!x.Accept(this))
                    return false;
            }

            // Have to have a full model for correct expansion
            var actualDescription = new FspDescription();

            foreach (var p in Description.Processes)
            {
                var process = FspExpanderFactory.GetExpander(p.Value, Description, actualDescription).Expand();

                actualDescription.Processes.Add(process.Name, process);
            }

            Description = actualDescription;

            return true;
        }

        public override bool VisitProcessDef(FSPActualParser.ProcessDefContext context)
        {
            var process = new FspProcess();

            var name = context.UpperCaseIdentifier();
            var body = context.processBody();

            process.Name = name.GetText();

            if (context.param() != null)
            {
                foreach (var paramCtx in context.param().parameterList().parameter())
                {
                    var paramName = paramCtx.UpperCaseIdentifier().GetText();
                    var paramDefault = paramCtx.expression().Accept(new FspExpressionConverter(env));

                    process.Parameters.Add(new FspParameter(paramName, paramDefault));
                }
            }

            Check(body.Accept(new FspProcessConverter(env, process)));
            
            if (context.alphabetExtension() != null)
            {
                var alphaExt = context.alphabetExtension().set().Accept(new FspLabelConverter(env));

                alphaExt.Expand(new FspExpressionEnvironment(Description), x => process.AlphabetExtension.Add(x));
            }

            Unimpl(context.relabel());

            HandleHiding(context.hiding(), process);



            Description.Processes.Add(process.Name, process);

            return true;
        }

        public override bool VisitCompositeDef(FSPActualParser.CompositeDefContext context)
        {
            // OrOr UpperCaseIdentifier param? Equal compositeBody priority? hiding? Dot
            var composite = new FspComposite();

            var name = context.UpperCaseIdentifier();

            if (context.param() != null)
            {
                foreach (var paramCtx in context.param().parameterList().parameter())
                {
                    var paramName = paramCtx.UpperCaseIdentifier().GetText();
                    var paramDefault = paramCtx.expression().Accept(new FspExpressionConverter(env));

                    composite.Parameters.Add(new FspParameter(paramName, paramDefault));
                }
            }

            var body = context.compositeBody();

            composite.Name = name.GetText();

            Check(body.Accept(new FspCompositeConverter(env, composite)));

            Unimpl(context.priority());

            HandleHiding(context.hiding(), composite);

            Description.Processes.Add(composite.Name, composite);

            return true;
        }

        private void HandleHiding(FSPActualParser.HidingContext context, FspBaseProcess process)
        {
            if (context != null)
            {
                var hiding = context.set().Accept(new FspLabelConverter(env));

                hiding.Expand(new FspExpressionEnvironment(Description), x => process.Hiding.Add(x));

                if (context.BackSlash() != null)
                    process.HidingMode = FspHidingMode.Black;
                else if (context.At() != null)
                    process.HidingMode = FspHidingMode.White;
                else
                    throw new InvalidOperationException();
            }
        }

        public override bool VisitMenuDef(FSPActualParser.MenuDefContext context)
        {
            // Ignore menus for now, it is a GUI thing

            return true;
        }

        public override bool VisitConstantDef(FSPActualParser.ConstantDefContext context)
        {
            // constantDef: Const UpperCaseIdentifier Equal simpleExpression;

            var name = context.UpperCaseIdentifier().GetText();
            var value = context.simpleExpression().Accept(new FspExpressionConverter(env)).GetValue(new FspExpressionEnvironment(Description));

            Description.Constants.Add(name, value);

            return true;
        }

        public override bool VisitRangeDef(FSPActualParser.RangeDefContext context)
        {
            // rangeDef: Range UpperCaseIdentifier Equal simpleExpression DotDot simpleExpression;
            var name = context.UpperCaseIdentifier().GetText();

            var lowerValue = context.simpleExpression(0).Accept(new FspExpressionConverter(env)).GetValue(new FspExpressionEnvironment(Description));
            var upperValue = context.simpleExpression(1).Accept(new FspExpressionConverter(env)).GetValue(new FspExpressionEnvironment(Description));

            Description.Ranges.Add(name, new FspBoundedRange(lowerValue, upperValue));

            return true;
        }

        public override bool VisitSetDef(FSPActualParser.SetDefContext context)
        {
            // setDef: Set UpperCaseIdentifier Equal LCurly setElements RCurly;

            return base.VisitSetDef(context);
        }
    }
}
