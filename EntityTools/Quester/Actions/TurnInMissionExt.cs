﻿#define MissionGiverInfo
//#define MissionGiverBase

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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class TurnInMissionExt : Action, INotifyPropertyChanged
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
                _interactZDifference = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractZDifference)));
            }
        }
        internal float _interactZDifference = 5;

#if DEVELOPER
        [Editor(typeof(RewardsEditor), typeof(UITypeEditor))]
        [Description("Item that is requered in rewards to TurnInMission\n" +
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
        private IQuesterActionEngine Engine;

        public TurnInMissionExt()
        {
            Engine = MakeProxie();
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
            Engine = MakeProxie();
            PropertyChanged = null;
        }

        private IQuesterActionEngine MakeProxie()
        {
            return new QuesterActionProxy(this);
        }
        #endregion

        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).NeedToRun;
        public override ActionResult Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).Run();
        public override string ActionLabel => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).UseHotSpots;
        protected override bool IntenalConditions => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalConditions;
        protected override Vector3 InternalDestination => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalDestination;
        protected override ActionValidity InternalValidity => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalValidity;
        public override void GatherInfos() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).GatherInfos();
        public override void InternalReset() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).InternalReset();
        public override void OnMapDraw(GraphicsNW graph) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxie).OnMapDraw(graph);
    }
}
