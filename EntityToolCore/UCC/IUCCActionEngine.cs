using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.UCC
{
    public interface IUCCActionEngine
    {
        bool NeedToRun();
        bool Validate();
        void Reset();
        void GatherInfos();
    }
}
