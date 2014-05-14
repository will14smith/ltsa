using System.Collections.Generic;
using LTSASharp.Lts;
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

            AssertSystemsEquals(lts, "BUTTON");

            var system = lts.Systems["BUTTON"];
            AssertAlphabetEquals(system, "press");
            AssertStateCountEquals(system, 4);
        }

        [TestMethod]
        public void Recursion()
        {
            const string prog = "SWITCH = OFF, OFF = (on -> ON), ON = (off -> OFF).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            AssertSystemsEquals(lts, "SWITCH");

            var system = lts.Systems["SWITCH"];
            AssertAlphabetEquals(system, "on", "off");
            AssertStateCountEquals(system, 2);
        }
    }
}
