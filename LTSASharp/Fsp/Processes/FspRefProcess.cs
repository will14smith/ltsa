using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Processes
{
    internal class FspRefProcess : FspBaseLocalProcess
    {
        public string Name { get; private set; }
        public List<FspExpression> Indices { get; private set; }

        public FspRefProcess(string name, IEnumerable<FspExpression> indices)
        {
            Name = name;
            Indices = indices.ToList();
        }

        public FspRefProcess(string name)
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