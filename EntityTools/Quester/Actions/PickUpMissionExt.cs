using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;
using NPCInfos = Astral.Quester.Classes.NPCInfos;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PickUpMissionExt : Action,
                                    INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Editor(typeof(MainMissionEditor), typeof(UITypeEditor))]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public string MissionId
        {
            get => _missionId;
            set
            {
                if (_missionId != value)
                {
                    _missionId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MissionId)));
                }
            }
        }
        internal string _missionId = string.Empty;

#if DEVELOPER
        [Description("Skip directly if mission is not available.")]
#else
        [Browsable(false)]
#endif
        public bool SkipOnFail
        {
            get => _skipOnFail; set
            {
                if (_skipOnFail != value)
                {
                    _skipOnFail = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SkipOnFail)));
                }

            }
        }
        internal bool _skipOnFail;

#if DEVELOPER
        [Editor(typeof(Astral.Quester.UIEditors.NPCInfos), typeof(UITypeEditor))]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public NPCInfos Giver
        {
            get => _giver;
            set
            {
                if (_giver != value)
                {
                    _giver = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Giver)));
                }
            }
        }
        internal NPCInfos _giver = new NPCInfos();

#if !DEVELOPER
        [Browsable(false)]
#endif
        public bool CloseContactDialog
        {
            get => _closeContactDialog;
            set
            {
                if (_closeContactDialog != value)
                {
                    _closeContactDialog = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseContactDialog)));
                }
            }
        }
        internal bool _closeContactDialog;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public float InteractDistance
        {
            get => _interactDistance;
            set
            {
                if (_interactDistance != value)
                {
                    _interactDistance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance)));
                }
            }
        }
        internal float _interactDistance;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public bool AutoAcceptOfferedMission
        {
            get => _autoAcceptOfferedMission; set
            {
                if(_autoAcceptOfferedMission != value)
                {
                    _autoAcceptOfferedMission = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoAcceptOfferedMission)));
                }
                
            }
        }
        internal bool _autoAcceptOfferedMission = true;

#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to PickUpMission\n" +
                     "Simple wildcard (*) is allowed\n" +
                     "Mission Offer dialog have to be opened for choosen the ReqieredRewardItem")]
#else
        [Browsable(false)]
#endif
        public string RequiredRewardItem
        {
            get => _requiredRewardItem;
            set
            {
                if (_requiredRewardItem != value)
                {
                    _requiredRewardItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RequiredRewardItem)));
                }
            }
        }
        internal string _requiredRewardItem = string.Empty;
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

#if CORE_DELEGATES
        internal Func<ActionResult> coreRun = null;
        internal Func<bool> coreNeedToRun = null;
        internal Func<bool> coreValidate = null;
        internal Action coreReset = null;
        internal Action coreGatherInfos = null;
        internal Func<string> coreString = null;
#endif
#if CORE_INTERFACES
        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;
#endif

        public PickUpMissionExt()
        {
#if CORE_DELEGATES
            coreRun = () => Core.EntityCoreProxy.Initialize(ref coreRun);
            coreNeedToRun = () => Core.EntityCoreProxy.Initialize(ref coreNeedToRun);
            coreValidate = () => Core.EntityCoreProxy.Initialize(ref coreValidate);
            coreReset = () => Core.EntityCoreProxy.Initialize(ref coreReset);
            coreGatherInfos = () => Core.EntityCoreProxy.Initialize(ref coreGatherInfos);
            coreString = () => Core.EntityCoreProxy.Initialize(ref coreString);
#endif
#if CORE_INTERFACES
            ActionEngine = new QuesterActionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion

#if CORE_DELEGATES
        #region Интерфейс Quester.Action
        public override bool NeedToRun => coreNeedToRun();

        public override ActionResult Run() => coreRun();

        public override void GatherInfos() => coreGatherInfos();

        [Browsable(false)]
        [XmlIgnore]
        public new string AssociateMission => string.Empty;

        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileConditionsAreOk => false;//true;

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (Giver == null || !Giver.Position.IsValid)
                {
                    return new ActionValidity("Giver position invalid.");
                }
                if (this.MissionId.Length == 0)
                {
                    return new ActionValidity("Invalid mission id.");
                }
                return new ActionValidity();
            }
        }

        public override string ActionLabel => coreString();

        protected override bool IntenalConditions => coreValidate();

        protected override Vector3 InternalDestination
        {
            get
            {
                if (Giver != null && Giver.Position.IsValid)
                    return Giver.Position.Clone();
                else return new Vector3();
            }
        }

        public override bool UseHotSpots => false;

        public override void OnMapDraw(GraphicsNW graph)
        {
            if (Giver != null && Giver.Position.IsValid)
            {
                graph.drawFillEllipse(Giver.Position, new Size(10, 10), Brushes.Beige);
            }
        }

        public override void InternalReset() => coreReset();

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";

        public override string InternalDisplayName => string.Empty;
        #endregion
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
