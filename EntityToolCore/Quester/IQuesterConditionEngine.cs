using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.Quester
{
#if CORE_DELEGATES
    public interface IQuesterConditionEngine
    {
        bool NeedToRun();
        bool Validate();
        void GatherInfos();
        void Reset();
    }
#endif
}
