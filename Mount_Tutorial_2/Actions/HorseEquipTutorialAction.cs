using Astral.Logic.Classes.Map;
using EntityTools.Core.Proxy;
using EntityTools.Core.Interfaces;
using MyNW.Classes;
using System;

namespace Mount_Tutorial
{
    public class HorseEquipTutorialAction : Astral.Quester.Classes.Action
    {
        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;

        public HorseEquipTutorialAction()
        {
            ActionEngine = new QuesterActionProxy(this);
            MountTutorialLoaderPlugin.Core.Initialize(this);
        }

        public override string ActionLabel => ActionEngine.ActionLabel;
        public override bool NeedToRun => ActionEngine.NeedToRun;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => ActionEngine.UseHotSpots;
        protected override bool IntenalConditions => ActionEngine.InternalConditions;
        protected override Vector3 InternalDestination => ActionEngine.InternalDestination;
        protected override ActionValidity InternalValidity => ActionEngine.InternalValidity;
        public override void GatherInfos() => ActionEngine.GatherInfos();
        public override void InternalReset() => ActionEngine.InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => ActionEngine.OnMapDraw(graph);
        public override ActionResult Run() => ActionEngine.Run();
    }
}
