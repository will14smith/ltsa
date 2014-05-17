using System;
using System.Collections.Generic;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Expressions
{
    public class FspExpressionEnvironment
    {
        private readonly FspDescription description;

        private readonly Dictionary<string, int> variables;
        private readonly Dictionary<string, Stack<int>> variableStack;
        
        public FspExpressionEnvironment(FspDescription description)
        {
            this.description = description;
            variables = new Dictionary<string, int>();
            variableStack = new Dictionary<string, Stack<int>>();
        }

        internal FspRange GetRange(string name)
        {
            return description.Ranges[name];
        }

        public int GetValue(string name)
        {
            int val;

            if (GetValue(name, out val))
                return val;


            throw new InvalidOperationException();
        }
        public bool GetValue(string name, out int val)
        {
            if (variables.TryGetValue(name, out val))
                return true;

            if (!char.IsUpper(name[0]))
                return false;

            if (description.Constants.TryGetValue(name, out val))
                return true;

            return false;
        }

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

        public virtual void PopVariable(string name)
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
