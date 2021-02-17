#define MissionGiverInfo

using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using EntityTools.Editors;
using EntityTools.Tools;
using EntityTools.Tools.Missions;
using Action = Astral.Quester.Classes.Action;
using NPCInfos = Astral.Quester.Classes.NPCInfos;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PickUpMissionExt : Action, INotifyPropertyChanged
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
        [Editor(typeof(MissionGiverInfoEditor), typeof(UITypeEditor))]
        [Category("Required")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
#if MissionGiverInfo
        public MissionGiverInfo Giver
#else
        public NPCInfos Giver 
#endif
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
#if MissionGiverInfo
        internal MissionGiverInfo _giver = new MissionGiverInfo();
#else
        internal NPCInfos _giver = new NPCInfos(); 
#endif

        [Browsable(false)]
        public string GiverId
        {
#if MissionGiverInfo
            get => string.Empty;//_giver.Id;
            set
            {
                if(!string.IsNullOrEmpty(value))
                    _giver.Id = value;
            } 
#else
            get => _giver.CostumeName;
            set
            {
                _giver.CostumeName = value;
                _giver.DisplayName = value;
            }
#endif
        }
        [Browsable(false)]
        public Vector3 GiverPosition
        {
            get => null;//_giver.Position;
            set
            {
                if (value != null && value.IsValid)
                    _giver.Position = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileUnSuccess
        {
            get => false;
        }
        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileConditionsAreOk
        {
            get => false;
        }
        [Browsable(false)]
        [XmlIgnore]
        public new bool Loop
        {
            get => false;
        }
        [Browsable(false)]
        [XmlIgnore]
        public new string AssociateMissionSuccess
        {
            get => string.Empty;
        }

#if !DEVELOPER
        [Browsable(false)]
#else
        [Description("Check if contact have mission")]
#endif
        public ContactHaveMissionCheckType ContactHaveMission
        {
            get => _contactHaveMission;
            set
            {
                if (_contactHaveMission == value) return;
                _contactHaveMission = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContactHaveMission)));
            }
        }
        internal ContactHaveMissionCheckType _contactHaveMission = ContactHaveMissionCheckType.Any;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public bool CloseContactDialog
        {
            get => _closeContactDialog;
            set
            {
                if (_closeContactDialog == value) return;
                _closeContactDialog = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CloseContactDialog)));
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
                if (_interactDistance == value) return;
                _interactDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance)));
            }
        }
        internal float _interactDistance;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public float InteractZDifference
        {
            get => _interactZDifference;
            set
            {
                if (_interactZDifference == value) return;
                _interactDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractZDifference)));
            }
        }
        internal float _interactZDifference = 5;

#if !DEVELOPER
        [Browsable(false)]
#else
        [Browsable(false)]
#endif
        public bool AutoAcceptOfferedMission
        {
            get => _autoAcceptOfferedMission; set
            {
                if (_autoAcceptOfferedMission == value) return;
                _autoAcceptOfferedMission = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoAcceptOfferedMission)));

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
                if (_requiredRewardItem == value) return;
                _requiredRewardItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RequiredRewardItem)));
            }
        }
        internal string _requiredRewardItem = string.Empty;
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;

        public PickUpMissionExt()
        {
            ActionEngine = new QuesterActionProxy(this);
            base.PlayWhileConditionsAreOk = false;
            base.PlayWhileUnSuccess = false;
            base.Loop = false;
        }
        #endregion

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
    }
}
