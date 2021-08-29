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
        [Description("Identifier of the Mission.\n\r" +
            "Allows simple mask (*) at the begin and at the end")]
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
        internal string _missionId = string.Empty;

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
        internal MissionGiverInfo _giver = new MissionGiverInfo();

#if DEVELOPER
        [Description("The minimum value is 5")]
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
                if (_interactDistance == value) return;
                _interactDistance = value;
                NotifyPropertyChanged();
            }
        }
        internal float _interactDistance = 10;

#if DEVELOPER
        [Description("The minimum value is 1")]
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
                if (_interactZDifference == value) return;
                _interactZDifference = value;
                NotifyPropertyChanged();
            }
        }
        internal float _interactZDifference = 10;

#if DEVELOPER
        [Description("Answers in dialog which have to be performed before mission picking up")]
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
        internal List<string> _dialogs = new List<string>();

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
        internal bool _skipOnFail = false;

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
        internal bool _closeContactDialog;

#if !DEVELOPER
        [Browsable(false)]
#else
        [Description("Check if contact have mission")]
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
        internal ContactHaveMissionCheckType _contactHaveMission = ContactHaveMissionCheckType.Any;

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
                NotifyPropertyChanged();
            }
        }
        internal bool _ignoreCombat = false;

#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to PickUpMission\n" +
                     "Simple wildcard (*) is allowed\n" +
                     "Mission Offer dialog have to be opened for choosen the ReqieredRewardItem")]
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
        internal string _requiredRewardItem = string.Empty;

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
            get => null;
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

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        private IQuesterActionEngine Engine;

        public PickUpMissionExt()
        {
            Engine = MakeProxy();
            base.PlayWhileConditionsAreOk = false;
            base.PlayWhileUnSuccess = false;
            base.Loop = false;
        }

        public void Bind(IQuesterActionEngine engine)
        {
            Engine = engine;
        }
        public void Unbind()
        {
            Engine = MakeProxy();
            PropertyChanged = null;
        }

        private IQuesterActionEngine MakeProxy()
        {
            return new QuesterActionProxy(this);
        }
        #endregion

        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).NeedToRun;
        public override ActionResult Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Run();
        public override string ActionLabel => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).UseHotSpots;
        protected override bool IntenalConditions => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalConditions;
        protected override Vector3 InternalDestination => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalDestination;
        protected override ActionValidity InternalValidity => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalValidity;
        public override void GatherInfos() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).GatherInfos();
        public override void InternalReset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).OnMapDraw(graph);
    }
}
