using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void ForAllParameter()
        {
            const string prog = 
                "SWITCH = (on->off->SWITCH)." +
                "||SWITCHES(N=3) =(forall[i:1..N] s[i]:SWITCH).";
            
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
    }
}
