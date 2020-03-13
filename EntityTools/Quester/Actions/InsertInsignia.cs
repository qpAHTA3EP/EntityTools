#define DEBUG_INSERTINSIGNIA

using System;
using System.ComponentModel;
using Astral.Logic.Classes.Map;
using MyNW.Classes;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InsertInsignia : Astral.Quester.Classes.Action, INotifyPropertyChanged
    {
        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        internal Func<ActionResult> coreRun = null;
        internal Func<bool> coreNeedToRun = null;
        internal Func<bool> coreValidate = null;
        internal Action coreReset = null;
        internal Action coreGatherInfos = null;
        internal Func<string> coreLabel = null;

        public InsertInsignia()
        {
            coreRun = () => Core.Initializer.Initialize(ref coreRun);
            coreNeedToRun = () => Core.Initializer.Initialize(ref coreNeedToRun);
            coreValidate = () => Core.Initializer.Initialize(ref coreValidate);
            coreReset = () => Core.Initializer.Initialize(ref coreReset);
            coreGatherInfos = () => Core.Initializer.Initialize(ref coreGatherInfos);
            coreLabel = () => Core.Initializer.Initialize(ref coreLabel);
        }
        #endregion

        //[Description("Not used yet")]
        //[Editor(typeof(ItemIdFilterEditor), typeof(UITypeEditor))]
        //public ItemFilterCore Mounts { get; set; }

        #region Интерфес Quester.Action
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
        #endregion
    }
}
