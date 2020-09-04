using EntityTools.Core.Interfaces;
using Mount_Tutorial;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Logic.Classes.Map;
using static Astral.Quester.Classes.Action;

namespace Mount_Tutorial_Core
{
    public class HorseEquipTutorialEngine : IQuesterActionEngine
    {
        HorseEquipTutorialAction @this;

        internal HorseEquipTutorialEngine(HorseEquipTutorialAction a)
        {
            @this = a;
            @this.ActionEngine = this;
        }

        public bool NeedToRun => true;

        public string ActionLabel => throw new NotImplementedException();

        public bool InternalConditions => true;

        public ActionValidity InternalValidity => new ActionValidity();

        public bool UseHotSpots => false;

        public Vector3 InternalDestination => new Vector3();

        public void GatherInfos() { }

        public void InternalReset() { }

        public void OnMapDraw(GraphicsNW graph) { }

        public ActionResult Run() => Instruments.MainMethod();
    }
}
