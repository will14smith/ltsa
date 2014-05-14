using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class GuardTests : BaseTest
    {
        [TestMethod]
        public void SimpleGuard1()
        {
            const string prog = "P = (a[i:0..3] -> " +
                                "( when i==0 x -> STOP" +
                                "| when i!=0 y -> P" +
                                ")).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        //TODO some more guard tests
    }
}
