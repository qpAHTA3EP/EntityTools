#define DEBUG_INSERTINSIGNIA
//#define CORE_DELEGATES

using System;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Proxy;
using EntityTools.Core.Interfaces;
using MyNW.Classes;
using Mount_Tutorial.Proxies;

namespace Mount_Tutorial
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
        internal Func<string> coreActionLabel = null;
        
        [NonSerialized]
        internal Func<bool> coreInternalConditions = null;
        [NonSerialized]
        internal Func<ActionValidity> coreInternalValidity = null;

        [NonSerialized]
        internal Func<bool> coreUseHotSpots = null;
        [NonSerialized]
        internal Func<Vector3> coreInternalDestination = null;

        [NonSerialized]
        internal Action coreReset = null;
        [NonSerialized]
        internal Action coreGatherInfos = null;

        [NonSerialized]
        internal Action<GraphicsNW> coreMapDraw = null;
#else
        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;
#endif
        public InsertInsignia()
        {
#if CORE_DELEGATES
            coreNeedToRun = () => EntityCoreInitiazlizer.Initialize(ref coreNeedToRun);
            coreRun = () => EntityCoreInitiazlizer.Initialize(ref coreRun);

            coreActionLabel = () => EntityCoreInitiazlizer.Initialize(ref coreActionLabel);

            coreInternalConditions = () => EntityCoreInitiazlizer.Initialize(ref coreInternalConditions);
            coreInternalValidity = () => EntityCoreInitiazlizer.Initialize(ref coreInternalValidity);

            coreUseHotSpots = () => EntityCoreInitiazlizer.Initialize(ref coreUseHotSpots);
            coreInternalDestination = () => EntityCoreInitiazlizer.Initialize(ref coreInternalDestination);

            coreReset = () => EntityCoreInitiazlizer.Initialize(ref coreReset);
            coreGatherInfos = () => EntityCoreInitiazlizer.Initialize(ref coreGatherInfos);

            coreMapDraw = (GraphicsNW g) => EntityCoreInitiazlizer.Initialize(ref coreMapDraw, g);
#else
            ActionEngine = new QuesterActionProxy(this);
            MountTutorialLoaderPlugin.Core.Initialize(this);
#endif
        }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }
#if CORE_DELEGATES
        #region Интерфес Quester.Action, реализованный через делегаты
        public override bool NeedToRun => coreNeedToRun();
        public override ActionResult Run() => coreRun();
        public override string ActionLabel => coreActionLabel();
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => coreUseHotSpots();
        protected override bool IntenalConditions => coreInternalConditions();
        protected override Vector3 InternalDestination => coreInternalDestination();
        protected override ActionValidity InternalValidity => coreInternalValidity();
        public override void GatherInfos() => coreGatherInfos();
        public override void InternalReset() => coreReset();
        public override void OnMapDraw(GraphicsNW graph) => coreMapDraw(graph);
        #endregion
#else
        #region Интерфес Quester.Action, реализованный через ActionEngine
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
        #endregion
#endif
    }
}
