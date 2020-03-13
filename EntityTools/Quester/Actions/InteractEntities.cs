#define DEBUG_INTERACTENTITIES
//#define PROFILING

using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Facades;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InteractEntities : Astral.Quester.Classes.Action,
                                    INotifyPropertyChanged
    {
#if DEBUG && PROFILING
        public static int RunCount = 0;
        public static void ResetWatch()
        {
            RunCount = 0;
            Logger.WriteLine(Logger.LogType.Debug, $"InteractEntities::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (RunCount > 0)
                Logger.WriteLine(Logger.LogType.Debug, $"InteractEntities: RunCount: {RunCount}");
        }
#endif

        #region Опции команды
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => _entityId;
            set
            {
                if (_entityId != value)
                {
                    _entityId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityID)));
                }
            }
        }
        [NonSerialized]
        internal string _entityId = string.Empty;

        [Description("Type of the EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => _entityIdType;
            set
            {
                if (_entityIdType != value)
                {
                    _entityIdType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityIdType)));
                }
            }
        }
        [NonSerialized]
        internal ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => _entityNameType;
            set
            {
                if (_entityNameType != value)
                {
                    _entityNameType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        [NonSerialized]
        internal EntityNameType _entityNameType = EntityNameType.InternalName;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Optional")]
        public bool RegionCheck
        {
            get => _regionCheck; set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegionCheck)));
                }
            }
        }
        internal bool _regionCheck = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck
        {
            get => _healthCheck; set
            {
                if (_healthCheck != value)
                {
                    _healthCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck)));
                }
            }
        }
        internal bool _healthCheck = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within Distance of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Optional")]
        public bool HoldTargetEntity
        {
            get => _holdTargetEntity; set
            {
                if (_holdTargetEntity != value)
                {
                    _holdTargetEntity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoldTargetEntity)));
                }
            }
        }
        internal bool _holdTargetEntity = true;

        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Optional")]
        public bool SkipMoving
        {
            get => _skipMoving; set
            {
                if (_skipMoving != value)
                {
                    _skipMoving = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SkipMoving)));
                }
            }
        }
        internal bool _skipMoving = false;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The value equals 0(zero) disables distance checking")]
        [Category("Optional")]
        public float ReactionRange
        {
            get => _reactionRange; set
            {
                if (_reactionRange != value)
                {
                    _reactionRange = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionRange)));
                }
            }
        }
        internal float _reactionRange = 150;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange
        {
            get => _reactionZRange; set
            {
                if (_reactionZRange != value)
                {
                    _reactionZRange = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionZRange)));
                }
            }
        }
        internal float _reactionZRange = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public List<string> CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        [NonSerialized]
        internal List<string> _customRegionNames = new List<string>();

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType
        {
            get => _entitySetType; set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntitySetType)));
                }
            }
        }
        internal EntitySetType _entitySetType = EntitySetType.Contacts;

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        public int SearchTimeInterval
        {
            get => _searchTimeInterval; set
            {
                if (_searchTimeInterval != value)
                {
                    _searchTimeInterval = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchTimeInterval)));
                }
            }
        }
        internal int _searchTimeInterval = 100;

        [Description("Distance to the Entity by which it is necessary to approach to disable 'IgnoreCombat' mode\n" +
            "Ignored if 'IgnoreCombat' does not True")]
        [Category("Interruptions")]
        public float CombatDistance
        {
            get => _combatDistance; set
            {
                if (_combatDistance != value)
                {
                    _combatDistance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CombatDistance)));
                }
            }
        }
        internal float _combatDistance = 30;

        [Description("Enable IgnoreCombat mode while distance to the closest Entity greater then 'CombatDistance'")]
        [Category("Interruptions")]
        public bool IgnoreCombat
        {
            get => _ignoreCombat; set
            {
                if (_ignoreCombat != value)
                {
                    _ignoreCombat = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IgnoreCombat)));
                }
            }
        }
        internal bool _ignoreCombat = true;

        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractitTimeout' period")]
        public bool InteractOnce
        {
            get => _interactOnce; set
            {
                if (_interactOnce != value)
                {
                    _interactOnce = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractOnce)));
                }
            }
        }
        internal bool _interactOnce = false;

        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set")]
        public int InteractingTimeout
        {
            get => _interactingTimeout; set
            {
                if (_interactingTimeout != value)
                {
                    _interactingTimeout = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractingTimeout)));
                }
            }
        }
        internal int _interactingTimeout = 60;

        [Description("Time to interact (ms)")]
        [Category("Interaction")]
        public int InteractTime
        {
            get => _interactTime;
            set
            {
                if (_interactTime != value)
                {
                    _interactTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractTime)));
                }
            }
        }
        internal int _interactTime = 2000;

        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
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

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        internal IQuesterActionEngine ActionEngine;

        #region Реализация через делегаты
        //[NonSerialized]
        //internal Func<ActionResult> coreRun = null;
        //[NonSerialized]
        //internal Func<bool> coreNeedToRun = null;
        //[NonSerialized]
        //internal Func<bool> coreInternalConditions = null;
        //[NonSerialized]
        //internal Func<ActionValidity> coreActionValidity = null;
        //[NonSerialized]
        //internal System.Action coreReset = null;
        //[NonSerialized]
        //internal System.Action coreGatherInfos = null;
        //[NonSerialized]
        //internal Func<string> coreLabel = null;
        //[NonSerialized]
        //internal Func<Entity> coreTarget = null;
        //[NonSerialized]
        //internal Func<bool> coreTargetValidate = null; 
        #endregion

        public InteractEntities()
        {
            ActionEngine = new QuesterActionInitializer(this);

            //coreRun = () => Core.Initializer.Initialize(ref coreRun);
            //coreNeedToRun = () => Core.Initializer.Initialize(ref coreNeedToRun);
            //coreInternalConditions = () => Core.Initializer.Initialize(ref coreInternalConditions);
            //coreActionValidity = () => Core.Initializer.Initialize(ref coreActionValidity);
            //coreReset = () => Core.Initializer.Initialize(ref coreReset);
            //coreGatherInfos = () => Core.Initializer.Initialize(ref coreGatherInfos);
            //coreLabel = () => Core.Initializer.Initialize(ref coreLabel);
            //coreTarget = () => Core.Initializer.Initialize(ref coreTarget);
            //coreTargetValidate = () => Core.Initializer.Initialize(ref coreTargetValidate);
        }
        #endregion

        #region Интерфейс Quester.Action через делегаты
        //public override bool NeedToRun => coreNeedToRun();
        //public override ActionResult Run() => coreRun();

        //public override string ActionLabel => coreLabel();
        //public override string InternalDisplayName => string.Empty;
        //public override bool UseHotSpots => true;
        //protected override Vector3 InternalDestination
        //{
        //    get
        //    {
        //        if (coreTargetValidate())
        //        {
        //            Entity target = coreTarget();
        //            if (_ignoreCombat && (target.Location.Distance3DFromPlayer > _combatDistance))
        //                return target.Location.Clone();
        //        }
        //        return new Vector3();
        //    }
        //}

        //protected override bool IntenalConditions => coreInternalConditions();
        //protected override ActionValidity InternalValidity => coreActionValidity();

        //public override void InternalReset() => coreReset();
        //public override void GatherInfos() => coreGatherInfos();
        //public override void OnMapDraw(GraphicsNW graph)
        //{
        //    if (coreTargetValidate())
        //    {
        //        graph.drawFillEllipse(coreTarget().Location, new Size(10, 10), Brushes.Beige);
        //    }
        //}
        #endregion

        #region Интерфейс Quester.Action через IQuesterActionEngine
        public override bool NeedToRun => ActionEngine.NeedToRun;
        public override ActionResult Run() => ActionEngine.Run();

        public override string ActionLabel => ActionEngine.ActionLabel;
        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => ActionEngine.UseHotSpots;
        protected override Vector3 InternalDestination => ActionEngine.InternalDestination;

        protected override bool IntenalConditions => ActionEngine.InternalConditions;
        protected override ActionValidity InternalValidity => ActionEngine.InternalValidity;

        public override void InternalReset() => ActionEngine.InternalReset();

        public override void GatherInfos() => ActionEngine.GatherInfos();

        public override void OnMapDraw(GraphicsNW graph) => ActionEngine.OnMapDraw(graph);
        #endregion
    }
}
