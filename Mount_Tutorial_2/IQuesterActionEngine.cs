using Astral.Logic.Classes.Map;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Core.Interfaces
{
    public interface IQuesterActionEngine
    {
        bool NeedToRun { get; }
        ActionResult Run();

        string ActionLabel { get; }

        bool InternalConditions { get; }
        ActionValidity InternalValidity { get; }

        bool UseHotSpots { get; }
        Vector3 InternalDestination { get; }

        void InternalReset();
        void GatherInfos();
        void OnMapDraw(GraphicsNW graph);
    }
}
