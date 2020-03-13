#define DEBUG_INSERTINSIGNIA

using System;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Facades;
using EntityTools.Core.Interfaces;
using MyNW.Classes;

namespace Mount_Tutorial
{
    [Serializable]
    public class InsertInsignia : Astral.Quester.Classes.Action, INotifyPropertyChanged
    {
        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;
        //internal Func<ActionResult> coreRun = null;
        //internal Func<bool> coreNeedToRun = null;
        //internal Func<bool> coreValidate = null;
        //internal Action coreReset = null;
        //internal Action coreGatherInfos = null;
        //internal Func<string> coreLabel = null;

        public InsertInsignia()
        {
            ActionEngine = new QuesterActionInitializer(this);
            //coreRun = () => Core.Initializer.Initialize(ref coreRun);
            //coreNeedToRun = () => Core.Initializer.Initialize(ref coreNeedToRun);
            //coreValidate = () => Core.Initializer.Initialize(ref coreValidate);
            //coreReset = () => Core.Initializer.Initialize(ref coreReset);
            //coreGatherInfos = () => Core.Initializer.Initialize(ref coreGatherInfos);
            //coreLabel = () => Core.Initializer.Initialize(ref coreLabel);
        }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }

        #region Интерфес Quester.Action
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
    }
}
