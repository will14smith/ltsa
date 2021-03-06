﻿using System.Linq;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspProcessConverter : FspBaseConverter<FspLocalProcess>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspProcess process;

        public FspProcessConverter(FspConverterEnvironment env, FspProcess process)
        {
            this.env = env;
            this.process = process;
        }

        public override FspLocalProcess VisitProcessBody(FSPActualParser.ProcessBodyContext context)
        {
            // localProcess (Comma localProcessDefs)?;
            var parent = Check(context.localProcess().Accept(this));
            process.Body.Map(process.Name, parent);

            if (context.localProcessDefs() != null)
                Check(context.localProcessDefs().Accept(this));

            return new FspEndProcess();
        }

        public override FspLocalProcess VisitLocalProcessDefs(FSPActualParser.LocalProcessDefsContext context)
        {
            // localProcessDef (Comma localProcessDef)*;
            FspLocalProcess first = null;
            var firstSet = false;

            foreach (var x in context.localProcessDef())
            {
                if (!firstSet)
                {
                    first = x.Accept(this);
                    firstSet = true;
                }
                else
                {
                    x.Accept(this);
                }
            }

            return first;
        }

        public override FspLocalProcess VisitLocalProcessDef(FSPActualParser.LocalProcessDefContext context)
        {
            // UpperCaseIdentifier indexRanges? Equal localProcess
            var name = context.UpperCaseIdentifier().GetText();
            var local = context.localProcess().Accept(this);

            if (context.indexRanges() != null)
            {
                // (LSquare (expression|actionRange) RSquare)+
                //TODO is order important?

                foreach (var t in context.indexRanges().expression())
                {
                    var expr = t.Accept(new FspExpressionConverter(env));

                    local = new FspIndexedProcess(local, new FspExpressionRange(expr));
                }
                foreach (var t in context.indexRanges().actionRange())
                {
                    var index = t.Accept(new FspRangeConverter(env));

                    local = new FspIndexedProcess(local, index);
                }
            }

            process.Body.Map(name, local);

            return local;
        }

        public override FspLocalProcess VisitLocalProcess(FSPActualParser.LocalProcessContext context)
        {
            //   baseLocalProcess
            // | sequentialComposition
            // | If expression Then localProcess
            // | If expression Then localProcess Else localProcess
            // | LRound choice RRound

            if (context.baseLocalProcess() != null)
            {
                return context.baseLocalProcess().Accept(this);
            }

            if (context.sequentialComposition() != null)
            {
                return context.sequentialComposition().Accept(this);
            }

            Unimpl(context.If());

            if (context.choice() != null)
            {
                return context.choice().Accept(this);
            }

            return null;
        }

        public override FspLocalProcess VisitBaseLocalProcess(FSPActualParser.BaseLocalProcessContext context)
        {
            // UpperCaseIdentifier indices?
            var name = context.UpperCaseIdentifier().GetText();

            switch (name)
            {
                case "STOP":
                    Unimpl(context.indices());
                    return new FspStopProcess();
                case "END":
                    Unimpl(context.indices());
                    return new FspEndProcess();
                case "ERROR":
                    Unimpl(context.indices());
                    return new FspErrorProcess();
                default:
                    if (context.indices() != null)
                        return new FspLocalRefProcess(name, context.indices().expression().Select(x => x.Accept(new FspExpressionConverter(env))));
                    return new FspLocalRefProcess(name);
            }
        }

        public override FspLocalProcess VisitChoice(FSPActualParser.ChoiceContext context)
        {
            var choices = new FspChoices();

            foreach (var choice in context.actionPrefix())
            {
                var c = choice.Accept(new FspChoiceConverter(env, process, choices));

                choices.Children.Add(c);
            }

            return choices;
        }

        public override FspLocalProcess VisitSequentialComposition(FSPActualParser.SequentialCompositionContext context)
        {
            // sequentialComposition: seqProcessList Semicolon baseLocalProcess;
            // seqProcessList: processRef (Semicolon processRef)*;

            var sequence = new FspSequenceProcess();

            foreach (var seq in context.seqProcessList().processRef())
            {
                sequence.Processes.Add(seq.Accept(this));
            }

            sequence.Processes.Add(context.baseLocalProcess().Accept(this));

            return sequence;
        }

        public override FspLocalProcess VisitProcessRef(FSPActualParser.ProcessRefContext context)
        {
            // UpperCaseIdentifier argument?

            var name = context.UpperCaseIdentifier();

            var reference = new FspRefProcess(name.GetText());

            if (context.argument() != null)
                foreach (var arg in context.argument().argumentList().expression())
                {
                    reference.Arguments.Add(arg.Accept(new FspExpressionConverter(env)));
                }

            return reference;
        }
    }
}