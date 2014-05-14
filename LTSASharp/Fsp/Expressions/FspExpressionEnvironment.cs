using System;
using System.Collections.Generic;

namespace LTSASharp.Fsp.Expressions
{
    public class FspExpressionEnvironment
    {
        private readonly Dictionary<string, int> variables;
        private readonly Dictionary<string, Stack<int>> variableStack;
        
        public FspExpressionEnvironment()
        {
            variables = new Dictionary<string, int>();
            variableStack = new Dictionary<string, Stack<int>>();
        }

        public IReadOnlyDictionary<string, int> Variables { get { return variables; } }

        public void PushVariable(string name, int value)
        {
            if(!variableStack.ContainsKey(name))
                variableStack.Add(name, new Stack<int>());

            if (variables.ContainsKey(name))
            {
                variableStack[name].Push(variables[name]);
                variables[name] = value;
            }
            else
            {
                variables.Add(name, value);   
            }
        }

        public void PopVariable(string name)
        {
            if (variableStack.ContainsKey(name))
            {
                if (variableStack[name].Count > 0)
                {
                    variables[name] = variableStack[name].Pop();
                }
                else
                {
                    variables.Remove(name);
                }
            }
            else
            {
                variables.Remove(name);
            }
        }
    }
}
