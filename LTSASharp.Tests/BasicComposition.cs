using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class BasicComposition : BaseTest
    {
        [TestMethod]
        public void OneShot()
        {
            const string prog = 
                "ITCH = (scratch->STOP)." +
                "CONVERSE = (think->talk->STOP)." +
                "||CONVERSE_ITCH = (ITCH || CONVERSE).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void Recursion()
        {
            const string prog = 
                "CLOCK = (tick->CLOCK)." +
                "RADIO = (on->off->RADIO)." +
                "||CLOCK_RADIO = (CLOCK || RADIO).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void SharedAction()
        {
            const string prog = 
                "BILL = (play -> meet -> STOP)." +
                "BEN  = (work -> meet -> STOP)." +
                "||BILL_BEN = (BILL || BEN).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void SharedActionRecursion()
        {
            const string prog = 
                "MAKE_A   = (makeA->ready->used->MAKE_A)." +
                "MAKE_B   = (makeB->ready->used->MAKE_B)." +
                "ASSEMBLE = (ready->assemble->used->ASSEMBLE)." +
                "||FACTORY = (MAKE_A || MAKE_B || ASSEMBLE).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }   
    }
}
