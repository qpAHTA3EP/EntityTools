﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityTools.Quester.Conditions
{
#if CORE_INTERFACES
    public interface IQuesterConditionEngine
    {
        bool IsValid { get; }
        void Reset();
        string TestInfos { get; }
        string Label();
    }
#endif
}