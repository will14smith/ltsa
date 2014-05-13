using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class ChoiceTests : BaseTest
    {
        [TestMethod]
        public void SimpleChoice()
        {
            const string prog = "DRINKS = (red -> coffee -> DRINKS | blue -> tea -> DRINKS).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void SetChoice()
        {
            const string prog = "FAULTY = ({red,blue,green} -> FAULTY | yellow -> candy -> FAULTY).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
        
        [TestMethod]
        public void NonDeterministicChoice()
        {
            const string prog = "COIN = (toss -> heads -> COIN | toss -> tails -> COIN).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void RecursiveChoice()
        {
            const string prog = "INPUTSPEED = (engineOn -> CHECKSPEED), CHECKSPEED = (speed -> CHECKSPEED |engineOff -> INPUTSPEED).";
            
            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }
    }
}
