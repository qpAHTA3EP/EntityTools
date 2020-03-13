using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester
{
    public interface IQuesterActionEngine
    {
        bool NeedToRun();
        ActionResult Run();
        bool InternalConditions();
        ActionValidity ActionValidity();
        void Reset();
        void GatherInfos();
        string Label();
    }
}
