using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using System.Collections.Generic;

namespace Mount_Tutorial
{
    public class HorseEquipTutorialAction : Astral.Quester.Classes.Action
    {
        public override string ActionLabel => GetType().Name;
        public override bool NeedToRun => true;
        public override string InternalDisplayName => GetType().Name;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity => new ActionValidity();
        public override void GatherInfos() { }
        public override void InternalReset() { }
        public override void OnMapDraw(GraphicsNW graph) { }

        public override ActionResult Run()
        {
            return Instruments.MainMethod();
        }
    }
}
