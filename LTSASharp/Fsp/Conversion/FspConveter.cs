using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Processes;
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

            return true;
        }

        public override bool VisitProcessDef(FSPActualParser.ProcessDefContext context)
        {
            var process = new FspProcess();

            var name = context.UpperCaseIdentifier();
            var body = context.processBody();

            process.Name = name.GetText();
            Unimpl(context.param());

            Check(body.Accept(new FspProcessConverter(env, process)));

            Unimpl(context.alphabetExtension());
            Unimpl(context.relabel());
            Unimpl(context.hiding());

            process = new FspProcessExpander(process).Expand();
            process = new FspProcessSimplifier(process).Simplify();

            Description.Processes.Add(process);

            return true;
        }

        public override bool VisitCompositeDef(FSPActualParser.CompositeDefContext context)
        {
            // OrOr UpperCaseIdentifier param? Equal compositeBody priority? hiding? Dot
            var composite = new FspComposite();

            var name = context.UpperCaseIdentifier();
            var body = context.compositeBody();

            composite.Name = name.GetText();

            Check(body.Accept(new FspCompositeConverter(env, composite)));

            Unimpl(context.priority());
            Unimpl(context.hiding());

            Description.Composites.Add(composite);

            return true;
        }
    }
}
