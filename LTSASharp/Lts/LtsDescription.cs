﻿using System.Collections.Generic;

namespace LTSASharp.Lts
{
    public class LtsDescription
    {
        public LtsDescription()
        {
            Systems = new Dictionary<string, LtsSystem>();
        }

        public Dictionary<string, LtsSystem> Systems { get; private set; }
    }
}