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
            const string progC = "||S = a[1..3]:P.";
            //const string progC = "||S = {a[1],a[2],a[3]}:P.";
            //const string progC = "||S = forall[i:1..3] a[i]:P.";
            //const string progC = "||S = forall[i:1..3] a:P/{a[i]/a}.";
            //const string progC = "||S = (a[1]:P || a[2]:P || a[3]:P).";

            var prog = "P = (on -> off -> P)." + progC;

            var fsp = CompileFsp(new AntlrInputStream(prog));
            var lts = CompileLts(fsp);
            
            Console.ReadLine();
        }

        private static LtsDescription CompileLts(FspDescription fsp)
        {
            var ltsConverter = new LtsConverter(fsp);

            return ltsConverter.Convert();
        }
        private static FspDescription CompileFsp(AntlrInputStream input)
        {
            var lexer = new FSPActualLexer(input);
            var parser = new FSPActualParser(new BufferedTokenStream(lexer));

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
