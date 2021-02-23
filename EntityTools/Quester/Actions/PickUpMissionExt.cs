using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Tools.Missions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Action = Astral.Quester.Classes.Action;

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
            get => _skipOnFail;
            set
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
        public MissionGiverInfo Giver
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
        internal MissionGiverInfo _giver = new MissionGiverInfo();

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
        public bool IgnoreCombat
        {
            get => _ignoreCombat;
            set
            {
                if (_ignoreCombat == value) return;
                _ignoreCombat = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IgnoreCombat)));
            }
        }
        internal bool _ignoreCombat = false;

#if DEVELOPER
        [Description("The minimum value is 5")]
#else
        [Browsable(false)]
#endif
        public float InteractDistance
        {
            get => _interactDistance;
            set
            {
                value = Math.Max(value, 5);
                if (_interactDistance == value) return;
                _interactDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance)));
            }
        }
        internal float _interactDistance = 5;

#if DEVELOPER
        [Description("The minimum value is 1")]
#else
        [Browsable(false)]
#endif
        public float InteractZDifference
        {
            get => _interactZDifference;
            set
            {
                value = Math.Max(value, 1);
                if (_interactZDifference == value) return;
                    _interactDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractZDifference)));
            }
        }
        internal float _interactZDifference = 5;

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

#if DEVELOPER
        [Description("Answers in dialog which have to be performed before mission picking up")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public List<string> Dialogs
        {
            get => _dialogs; set
            {
                if (_dialogs != value)
                {
                    _dialogs = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dialogs)));
                }
            }
        }
        internal List<string> _dialogs = new List<string>();

        [Browsable(false)]
        public string GiverId
        {
            get => string.Empty;//_giver.Id;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _giver.Id = value;
            }
        }
        [Browsable(false)]
        public Vector3 GiverPosition
        {
            get => null;//_giver.Position;
            set
            {
                if (value != null && value.IsValid)
                {
                    _giver.Position = value;
                }
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
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        internal IQuesterActionEngine Engine;

        public PickUpMissionExt()
        {
            Engine = new QuesterActionProxy(this);
            base.PlayWhileConditionsAreOk = false;
            base.PlayWhileUnSuccess = false;
            base.Loop = false;
        }
        #endregion

        public override bool NeedToRun => Engine.NeedToRun;
        public override ActionResult Run() => Engine.Run();
        public override string ActionLabel => Engine.ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => Engine.UseHotSpots;
        protected override bool IntenalConditions => Engine.InternalConditions;
        protected override Vector3 InternalDestination => Engine.InternalDestination;
        protected override ActionValidity InternalValidity => Engine.InternalValidity;
        public override void GatherInfos() => Engine.GatherInfos();
        public override void InternalReset() => Engine.InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => Engine.OnMapDraw(graph);
    }
}
