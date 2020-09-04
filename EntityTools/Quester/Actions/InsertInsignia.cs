#define DEBUG_INSERTINSIGNIA

using System;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using MyNW.Classes;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InsertInsignia : Astral.Quester.Classes.Action, INotifyPropertyChanged
    {
        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

#if CORE_INTERFACES
        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;
#endif
        public InsertInsignia()
        {
#if CORE_INTERFACES
            ActionEngine = new QuesterActionProxy(this);
#endif
        }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }

#if CORE_INTERFACES
        // Интерфес Quester.Action, реализованный через ActionEngine
        public override bool NeedToRun => ActionEngine.NeedToRun;
        public override ActionResult Run() => ActionEngine.Run();
        public override string ActionLabel => ActionEngine.ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => ActionEngine.UseHotSpots;
        protected override bool IntenalConditions => ActionEngine.InternalConditions;
        protected override Vector3 InternalDestination => ActionEngine.InternalDestination;
        protected override ActionValidity InternalValidity => ActionEngine.InternalValidity;
        public override void GatherInfos() => ActionEngine.GatherInfos();
        public override void InternalReset() => ActionEngine.InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => ActionEngine.OnMapDraw(graph);
#endif
    }
}
