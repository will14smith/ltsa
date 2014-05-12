using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Tree;
using LTSASharp.Parsing;
using Sharpen;

namespace LTSASharp
{
    internal class DebugListener : IParserErrorListener, IParseTreeListener, IAntlrErrorListener<int>
    {
        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new NotImplementedException();
        }

        public void ReportAmbiguity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact, BitSet ambigAlts, ATNConfigSet configs)
        {
            throw new NotImplementedException();
        }

        public void ReportAttemptingFullContext(Parser recognizer, DFA dfa, int startIndex, int stopIndex, BitSet conflictingAlts, SimulatorState conflictState)
        {
            throw new NotImplementedException();
        }

        public void ReportContextSensitivity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, int prediction, SimulatorState acceptState)
        {
            //throw new NotImplementedException();
        }

        public void VisitTerminal(ITerminalNode node)
        {
            Console.WriteLine(pad + "terminal {0} '{1}'", FSPActualLexer.ruleNames[node.Symbol.Type + 3], node.GetText());
        }

        public void VisitErrorNode(IErrorNode node)
        {
            Console.WriteLine(pad + "error {0}",node.GetText());
        }

        private string pad = "";
        public void EnterEveryRule(ParserRuleContext ctx)
        {
            Console.WriteLine(pad + "Enter Rule {0}", ctx.GetType().Name);

            pad += "  ";
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            pad = pad.Substring(2);

            Console.WriteLine(pad + "Exit Rule {0}", ctx.GetType().Name);

        }

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new NotImplementedException();
        }
    }
}