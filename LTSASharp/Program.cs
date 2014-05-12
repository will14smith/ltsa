using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Conversion;
using LTSASharp.Lts;
using LTSASharp.Lts.Conversion;
using LTSASharp.Parsing;

namespace LTSASharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Basics();
            Choices();
            Variables();
            Composition();

            Console.ReadLine();
        }

        private static void Basics()
        {
            const string prog0 = "BUTTON = (press -> press -> press -> END).";
            const string prog1 = "SWITCH = OFF, OFF = (on -> ON), ON = (off -> OFF).";

            var lts0 = CompileLTS(CompileFSP(new AntlrInputStream(prog0)));
            var lts1 = CompileLTS(CompileFSP(new AntlrInputStream(prog1)));
        }

        private static void Choices()
        {
            const string prog2 = "DRINKS = (red -> coffee -> DRINKS | blue -> tea -> DRINKS).";
            const string prog3 = "FAULTY = ({red,blue,green} -> FAULTY | yellow -> candy -> FAULTY).";
            const string prog4 = "COIN = (toss -> heads -> COIN | toss -> tails -> COIN).";

            var lts2 = CompileLTS(CompileFSP(new AntlrInputStream(prog2)));
            var lts3 = CompileLTS(CompileFSP(new AntlrInputStream(prog3)));
            var lts4 = CompileLTS(CompileFSP(new AntlrInputStream(prog4)));
        }

        private static void Variables()
        {
            const string prog5 = "BUFF = (in[i:0..3] -> out[i] -> BUFF).";
            const string prog6 = "SUM = (in[a:0..2][b:0..2] -> out[a+b] -> END).";
            const string prog7 = "SUM = (in[a:0..2][b:0..2] -> TOTAL[a+b]), TOTAL[s:0..4] = (out[s] -> SUM).";

            var lts5 = CompileLTS(CompileFSP(new AntlrInputStream(prog5)));
            var lts6 = CompileLTS(CompileFSP(new AntlrInputStream(prog6)));
            //TODO var lts7 = CompileLTS(CompileFSP(new AntlrInputStream(prog7)));
        }

        public static void Composition()
        {
            const string prog8 = "ITCH = (scratch->STOP).\n" +
                                 "CONVERSE = (think->talk->STOP).\n" +
                                 "||CONVERSE_ITCH = (ITCH || CONVERSE).";

            var lts8 = CompileLTS(CompileFSP(new AntlrInputStream(prog8)));
        }

        private static LtsDescription CompileLTS(FspDescription fsp)
        {
            var ltsConverter = new LtsConverter(fsp);

            return ltsConverter.Convert();
        }
        private static FspDescription CompileFSP(AntlrInputStream input)
        {
            var listener = new DebugListener();

            var lexer = new FSPActualLexer(input);
            lexer.AddErrorListener(listener);
            var parser = new FSPActualParser(new BufferedTokenStream(lexer));
            //parser.AddParseListener(listener);
            //parser.AddErrorListener(listener);

            var fspConverter = new FspConveter();
            parser.fsp_description().Accept(fspConverter);

            Console.WriteLine(fspConverter.Description);

            return fspConverter.Description;
        }
    }
}
