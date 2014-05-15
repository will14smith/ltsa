using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    [TestClass]
    public class BasicComposition2  :BaseTest
    {
        [TestMethod]
        public void ProcessLabel()
        {
            const string prog = 
                "SWITCH = (on->off->SWITCH).\n" +
                "||TWO_SWITCH = (a:SWITCH || b:SWITCH).";
            
            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void ForAll()
        {
            const string prog = 
                "SWITCH = (on->off->SWITCH)." +
                "||SWITCHES =(forall[i:1..3] s[i]:SWITCH).";
        
            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void ResourceSharing()
        {
            const string prog = 
                "RESOURCE = (acquire->release->RESOURCE)." +
                "USER = (acquire->use->release->USER)." +
                "||RESOURCE_SHARE = (a:USER || b:USER || {a,b}::RESOURCE).";

            var fsp = CompileFsp(prog);
            var lts = CompileLts(fsp);

            //TODO make some assertions
        }

        [TestMethod]
        public void ExpansionEquality()
        {
            const string progBase = "P = (on -> off -> P).";

            const string prog1 = progBase + "||S = a[1..3]:P.";
            const string prog2 = progBase + "||S = {a[1],a[2],a[3]}:P.";
            const string prog3 = progBase + "||S = forall[i:1..3] a[i]:P.";
            //TODO relabelling... const string prog4 = progBase + "||S = forall[i:1..3] a:P/{a[i]/a}.";
            const string prog5 = progBase + "||S = (a[1]:P || a[2]:P || a[3]:P).";

            var lts1 = CompileLts(CompileFsp(prog1));
            var lts2 = CompileLts(CompileFsp(prog2));
            var lts3 = CompileLts(CompileFsp(prog3));
            // var lts4 = CompileLts(CompileFsp(prog4));
            var lts5 = CompileLts(CompileFsp(prog5));

            AssertLtsEqual(lts1, lts2);
            AssertLtsEqual(lts1, lts3);
            // AssertLtsEqual(lts1, lts4);
            AssertLtsEqual(lts1, lts5);
        }
    }
}
