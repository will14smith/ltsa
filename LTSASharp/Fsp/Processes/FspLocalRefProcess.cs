using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Processes
{
    internal class FspLocalRefProcess : FspBaseLocalProcess
    {
        public string Name { get; private set; }
        public List<FspExpression> Indices { get; private set; }

        public FspLocalRefProcess(string name, IEnumerable<FspExpression> indices)
        {
            Name = name;
            Indices = indices.ToList();
        }

        public FspLocalRefProcess(string name)
        {
            Name = name;
            Indices = new List<FspExpression>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}