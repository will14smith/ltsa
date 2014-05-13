using System;
using Antlr4.Runtime;
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
            BasicComposition();
            BasicComposition2();

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
            const string prog0 = "DRINKS = (red -> coffee -> DRINKS | blue -> tea -> DRINKS).";
            const string prog1 = "FAULTY = ({red,blue,green} -> FAULTY | yellow -> candy -> FAULTY).";
            const string prog2 = "COIN = (toss -> heads -> COIN | toss -> tails -> COIN).";
            const string prog3 = "INPUTSPEED = (engineOn -> CHECKSPEED), CHECKSPEED = (speed -> CHECKSPEED |engineOff -> INPUTSPEED).";

            var lts0 = CompileLTS(CompileFSP(new AntlrInputStream(prog0)));
            var lts1 = CompileLTS(CompileFSP(new AntlrInputStream(prog1)));
            var lts2 = CompileLTS(CompileFSP(new AntlrInputStream(prog2)));
            //TODO INFINITE var lts3 = CompileLTS(CompileFSP(new AntlrInputStream(prog3)));
        }

        private static void Variables()
        {
            const string prog0 = "BUFF = (in[i:0..3] -> out[i] -> BUFF).";
            const string prog1 = "SUM = (in[a:0..2][b:0..2] -> out[a+b] -> END).";
            const string prog2 = "SUM = (in[a:0..2][b:0..2] -> TOTAL[a+b]), TOTAL[s:0..4] = (out[s] -> SUM).";

            var lts0 = CompileLTS(CompileFSP(new AntlrInputStream(prog0)));
            var lts1 = CompileLTS(CompileFSP(new AntlrInputStream(prog1)));
            //TODO FSP var lts2 = CompileLTS(CompileFSP(new AntlrInputStream(prog2)));
        }

        public static void BasicComposition()
        {
            const string prog0 = "ITCH = (scratch->STOP).\n" +
                                 "CONVERSE = (think->talk->STOP).\n" +
                                 "||CONVERSE_ITCH = (ITCH || CONVERSE).";
            const string prog1 = "CLOCK = (tick->CLOCK).\n" +
                                 "RADIO = (on->off->RADIO).\n" +
                                 "||CLOCK_RADIO = (CLOCK || RADIO).";
            const string prog2 = "BILL = (play -> meet -> STOP).\n" +
                                 "BEN  = (work -> meet -> STOP).\n" +
                                 "||BILL_BEN = (BILL || BEN).";
            const string prog3 = "MAKE_A   = (makeA->ready->used->MAKE_A).\n" +
                                  "MAKE_B   = (makeB->ready->used->MAKE_B).\n" +
                                  "ASSEMBLE = (ready->assemble->used->ASSEMBLE).\n" +
                                  "||FACTORY = (MAKE_A || MAKE_B || ASSEMBLE).";

            var lts0 = CompileLTS(CompileFSP(new AntlrInputStream(prog0)));
            var lts1 = CompileLTS(CompileFSP(new AntlrInputStream(prog1)));
            var lts2 = CompileLTS(CompileFSP(new AntlrInputStream(prog2)));
            var lts3 = CompileLTS(CompileFSP(new AntlrInputStream(prog3)));
        }

        public static void BasicComposition2()
        {
            const string prog0 = "SWITCH = (on->off->SWITCH).\n" +
                                 "||TWO_SWITCH = (a:SWITCH || b:SWITCH).";
            const string prog1 = "SWITCH = (on->off->SWITCH).\n" +
                                 "||SWITCHES =(forall[i:1..3] s[i]:SWITCH).";
            const string prog2 = "SWITCH = (on->off->SWITCH).\n" +
                                 "||SWITCHES(N=3) =(forall[i:1..N] s[i]:SWITCH).";
            const string prog3 = "RESOURCE = (acquire->release->RESOURCE).\n" +
                                 "USER = (acquire->use->release->USER).\n" +
                                 "||RESOURCE_SHARE = (a:USER || b:USER || {a,b}::RESOURCE).";

            //TODO FSP var lts0 = CompileLTS(CompileFSP(new AntlrInputStream(prog0)));
            //TODO FSP var lts1 = CompileLTS(CompileFSP(new AntlrInputStream(prog1)));
            //TODO FSP var lts2 = CompileLTS(CompileFSP(new AntlrInputStream(prog2)));
            //TODO FSP var lts3 = CompileLTS(CompileFSP(new AntlrInputStream(prog3)));
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

            foreach (var p in fspConverter.Description.Processes)
            {
                Console.WriteLine(p);
            }
            foreach (var c in fspConverter.Description.Composites)
            {
                Console.WriteLine(c);
            }
            Console.WriteLine();

            return fspConverter.Description;
        }
    }
}
