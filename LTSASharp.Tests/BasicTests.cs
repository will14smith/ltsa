using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class BasicTests : BaseTest
    {
        [TestMethod]
        public void OneShot()
        {
            const string prog = "BUTTON = (press -> press -> press -> END).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void Recursion()
        {
            const string prog = "SWITCH = OFF, OFF = (on -> ON), ON = (off -> OFF).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
    }
}
