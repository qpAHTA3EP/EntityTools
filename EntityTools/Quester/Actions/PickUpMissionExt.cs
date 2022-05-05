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
using System.Threading;
using System.Xml.Serialization;
using Astral.Quester.Classes.Actions;
using EntityTools.Annotations;
using EntityTools.Enums;
using Action = Astral.Quester.Classes.Action;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class PickUpMissionExt : Action, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Description("Identifier of the Mission.\n" +
            "Allows simple mask (*) at the begin and at the end.")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private string _missionId = string.Empty;

#if DEVELOPER
        [Editor(typeof(MissionGiverInfoEditor), typeof(UITypeEditor))]
        [Category("Required")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        [NotifyParentProperty(true)]
        public MissionGiverInfo Giver
        {
            get => _giver;
            set
            {
                if (_giver != value)
                {
                    _giver = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private MissionGiverInfo _giver = new MissionGiverInfo();

#if DEVELOPER
        [Description("The minimum value is 5.")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public float InteractDistance
        {
            get => _interactDistance;
            set
            {
                value = Math.Max(value, 5);
                if (Math.Abs(_interactDistance - value) < 0.1f) return;
                _interactDistance = value;
                NotifyPropertyChanged();
            }
        }
        private float _interactDistance = 10;

#if DEVELOPER
        [Description("The minimum value is 1.")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public float InteractZDifference
        {
            get => _interactZDifference;
            set
            {
                value = Math.Max(value, 1);
                if (Math.Abs(_interactZDifference - value) < 0.1f) return;
                _interactZDifference = value;
                NotifyPropertyChanged();
            }
        }
        private float _interactZDifference = 10;

#if DEVELOPER
        [Description("Answers in dialog which have to be performed before mission picking up.")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
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
                    NotifyPropertyChanged();
                }
            }
        }
        private List<string> _dialogs = new List<string>();

#if DEVELOPER
        [Description("Skip directly if mission is not available.")]
        [Category("Interaction")]
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
                    NotifyPropertyChanged();
                }

            }
        }
        private bool _skipOnFail;

#if DEVELOPER
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
        public bool CloseContactDialog
        {
            get => _closeContactDialog;
            set
            {
                if (_closeContactDialog == value) return;
                _closeContactDialog = value;
                NotifyPropertyChanged();
            }
        }
        private bool _closeContactDialog;

#if !DEVELOPER
        [Browsable(false)]
#else
        [Description("Check if contact have mission.")]
        [Category("Optional")]
#endif
        public ContactHaveMissionCheckType ContactHaveMission
        {
            get => _contactHaveMission;
            set
            {
                if (_contactHaveMission == value) return;
                _contactHaveMission = value;
                NotifyPropertyChanged();
            }
        }
        private ContactHaveMissionCheckType _contactHaveMission = ContactHaveMissionCheckType.Disabled;

        #region Manage Combat Options
#if DEVELOPER
        [Description("Ignore the enemies while approaching the " + nameof(Giver) + ".")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public bool IgnoreCombat
        {
            get => _ignoreCombat;
            set
            {
                if (_ignoreCombat == value) return;
                _ignoreCombat = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ignoreCombat;

#if DEVELOPER
        [Description("Sets the ucc option '" + nameof(IgnoreCombatMinHP) + "' when disabling combat mode.\n" +
                     "Options ignored if the value is -1.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public int IgnoreCombatMinHP
        {
            get => _ignoreCombatMinHp; set
            {
                if (value < -1)
                    value = 0;
                if (value > 100)
                    value = 100;
                if (_ignoreCombatMinHp != value)
                {
                    _ignoreCombatMinHp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _ignoreCombatMinHp = -1;

#if DEVELOPER
        [Description("Special check before disabling combat while playing action.\n" +
                     "The condition is checking when option '" + nameof(IgnoreCombat) + "' is active.")]
        [Category("Manage Combat Options")]
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public Astral.Quester.Classes.Condition IgnoreCombatCondition
        {
            get => _ignoreCombatCondition;
            set
            {
                if (value != _ignoreCombatCondition)
                {
                    _ignoreCombatCondition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Astral.Quester.Classes.Condition _ignoreCombatCondition; 
        #endregion

#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to " + nameof(PickUpMission) + ".\n" +
                     "Simple wildcard (*) is allowed.\n" +
                     "Mission Offer dialog have to be opened for choosen the " + nameof(RequiredRewardItem) + ".")]
        [Category("Optional")]
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
                NotifyPropertyChanged();
            }
        }
        private string _requiredRewardItem = string.Empty;

#if DEVELOPER
        [Description("The Id of the Action executing if the '" + nameof(RequiredRewardItem) + "' would be missing.")]
        [Category("Optional")]
        [DisplayName("TargetActionOnRequiredRewardMissing")]
#else
        [Browsable(false)]
#endif
        public Guid TargetAction
        {
            get => _targetActionId;
            set
            {
                if (_targetActionId == value) return;
                _targetActionId = value;
                NotifyPropertyChanged();
            }
        }
        private Guid _targetActionId = Guid.Empty;

        [Browsable(false)]
        public string GiverId
        {
            get => string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _giver.Id = value;
            }
        }
        [Browsable(false)]
        public Vector3 GiverPosition
        {
            get => null;
            set
            {
                if (value != null && value.IsValid)
                    _giver.Position = value;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileUnSuccess => false;

        [Browsable(false)]
        [XmlIgnore]
        public new bool PlayWhileConditionsAreOk => false;

        [Browsable(false)]
        [XmlIgnore]
        public new bool Loop => false;

        [Browsable(false)]
        [XmlIgnore]
        public new string AssociateMissionSuccess => string.Empty;

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            _engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        private IQuesterActionEngine _engine;

        public PickUpMissionExt()
        {
            _engine = MakeProxy();
            base.PlayWhileConditionsAreOk = false;
            base.PlayWhileUnSuccess = false;
            base.Loop = false;
        }

        public void Bind(IQuesterActionEngine engine)
        {
            _engine = engine;
        }
        public void Unbind()
        {
            _engine = MakeProxy();
            PropertyChanged = null;
        }

        private IQuesterActionEngine MakeProxy()
        {
            return new QuesterActionProxy(this);
        }
        #endregion

        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).NeedToRun;
        public override ActionResult Run() => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).Run();
        public override string ActionLabel => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).UseHotSpots;
        protected override bool IntenalConditions => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).InternalConditions;
        protected override Vector3 InternalDestination => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).InternalDestination;
        protected override ActionValidity InternalValidity => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).InternalValidity;
        public override void GatherInfos() => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).GatherInfos();
        public override void InternalReset() => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => LazyInitializer.EnsureInitialized(ref _engine, MakeProxy).OnMapDraw(graph);
    }
}
