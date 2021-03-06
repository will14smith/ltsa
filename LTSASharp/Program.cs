﻿using System;
using System.IO;
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
            var a = CompileFsp(new AntlrFileStream(@"E:\Projects\C#\LTSASharp\Chapter_examples\chapter3_lts\thread_demo.lts"));
            var b = CompileLts(a);

            foreach (var file in Directory.GetFiles(@"E:\Projects\C#\LTSASharp\Chapter_examples\chapter3_lts", "*.lts", SearchOption.AllDirectories))
            {
                var sr = GetMemory();
                var fsp = CompileFsp(new AntlrFileStream(file));
                var lts = CompileLts(fsp);
                var er = GetMemory();

                Console.WriteLine("Used {0:n0} kb memory", (er - sr) / 1024);
            }

            Console.ReadLine();
        }

        private static long GetMemory()
        {
            return GC.GetTotalMemory(false);
            //return Process.GetCurrentProcess().WorkingSet64;
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
            Console.WriteLine();

            return fspConverter.Description;
        }
    }
}
