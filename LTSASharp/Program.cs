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
            const string prog =
                "RESOURCE = (acquire->release->RESOURCE)." +
                "USER = (acquire->use->release->USER)." +
                "||RESOURCE_SHARE = (a:USER || b:USER || {a,b}::RESOURCE).";

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
