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

#if CORE_DELEGATES
        [NonSerialized]
        internal Func<ActionResult> coreRun = null;
        [NonSerialized]
        internal Func<bool> coreNeedToRun = null;
        [NonSerialized]
        internal Func<bool> coreValidate = null;
        [NonSerialized]
        internal Action coreReset = null;
        [NonSerialized]
        internal Action coreGatherInfos = null;
        [NonSerialized]
        internal Func<string> coreLabel = null;
#endif
#if CORE_INTERFACES
        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;
#endif
        public InsertInsignia()
        {
#if CORE_DELEGATES
            coreRun = () => Core.EntityCoreProxy.Initialize(ref coreRun);
            coreNeedToRun = () => Core.EntityCoreProxy.Initialize(ref coreNeedToRun);
            coreValidate = () => Core.EntityCoreProxy.Initialize(ref coreValidate);
            coreReset = () => Core.EntityCoreProxy.Initialize(ref coreReset);
            coreGatherInfos = () => Core.EntityCoreProxy.Initialize(ref coreGatherInfos);
            coreLabel = () => Core.EntityCoreProxy.Initialize(ref coreLabel);
#endif
#if CORE_INTERFACES
            ActionEngine = new QuesterActionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }

#if CORE_DELEGATES
        // Интерфес Quester.Action
        public override bool NeedToRun => coreNeedToRun();
        public override ActionResult Run() => coreRun();
        public override string ActionLabel => coreLabel();
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity => new ActionValidity();
        public override void GatherInfos() => coreGatherInfos();
        public override void InternalReset() => coreReset();
        public override void OnMapDraw(GraphicsNW graph) { }
#endif
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
