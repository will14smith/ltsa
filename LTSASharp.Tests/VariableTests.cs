using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class VariableTests : BaseTest
    {
        [TestMethod]
        public void SingleVariable()
        {
            const string prog = "BUFF = (in[i:0..3] -> out[i] -> BUFF).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void MultipleVariable()
        {
            const string prog = "SUM = (in[a:0..2][b:0..2] -> out[a+b] -> END).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void ProcessVariable()
        {
            const string prog = "SUM = (in[a:0..2][b:0..2] -> TOTAL[a+b]), TOTAL[s:0..4] = (out[s] -> SUM).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
    }
}
