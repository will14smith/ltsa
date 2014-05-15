using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class ParameterTests : BaseTest
    {
        [TestMethod]
        public void ProcessParameter()
        {
            const string prog = "P(X=1) = (a[X] -> STOP)." +
                                "||S    = (P(3) || P(4)).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void SequencialParameter()
        {
            const string prog = "P(I=1) = (a[I]-> b[I] -> END)." +
                                "SC = P(1);P(2);SC.";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void BothParameter()
        {
            const string prog = "P(X=1)   = (a[X] -> STOP)." +
                                "||S(Y=2) = (P(Y+1) || P(Y+2)).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void CompositionParameter()
        {
            const string prog =
                "SWITCH = (on->off->SWITCH)." +
                "||SWITCHES(N=3) = (forall[i:1..N] s[i]:SWITCH).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void CompositionParameter2()
        {
            const string prog = "P(X=1)   = (a[X] -> STOP)." +
                                "||S(Y=2) = (P(Y+1) || P(Y+2))." +
                                "||Q = S(3).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
    }
}