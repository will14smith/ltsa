using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class SequencialTests : BaseTest
    {
        [TestMethod]
        public void Seq1()
        {
            const string prog = "P(I=1) = (a[I]-> b[I] -> END)." +
                                "SC = P(1);P(2);SC.";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }    
        
        [TestMethod]
        public void Seq2()
        {
            const string prog = "SP(I=0) = (a[I] -> END)." +
                                "P123 = (start -> SP(1);SP(2);SP(3);END)." +
                                "LOOP = P123;LOOP.";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
    }
}