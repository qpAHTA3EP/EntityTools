#define DEBUG_INTERACTENTITIES
//#define PROFILING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InteractEntities : Action,
                                    INotifyPropertyChanged
    {
#if DEBUG && PROFILING
        public static int RunCount = 0;
        public static void ResetWatch()
        {
            RunCount = 0;
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntities::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (RunCount > 0)
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntities: RunCount: {RunCount}");
        }
#endif

        #region Опции команды
#if DEVELOPER
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
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
        internal string _entityId = string.Empty;

#if DEVELOPER
        [Description("Type of the EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
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
        internal ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
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
        internal EntityNameType _entityNameType = EntityNameType.InternalName;

#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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
        internal bool _regionCheck;

#if DEVELOPER
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("True: Do not change the target Entity while it is alive or until the Bot within Distance of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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
        internal bool _skipMoving;

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The value equals 0(zero) disables distance checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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
        internal float _reactionZRange;

#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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
        internal List<string> _customRegionNames = new List<string>();

#if DEVELOPER
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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

#if disabled_at_20200506_1318
        // Более не используется, т.к. реализована единая настройка:
        // EntityTools.Config.EntityCache.LocalCacheTime
#if DEVELOPER
        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
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
#endif

#if DEVELOPER
        [Description("Distance to the Entity by which it is necessary to approach to disable 'IgnoreCombat' mode\n" +
            "Ignored if 'IgnoreCombat' does not True")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("Enable IgnoreCombat mode while distance to the closest Entity greater then 'CombatDistance'")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
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


        //TODO: Добавить опцию, которая автоматически прерывать бой при удалении от заданного Entity.
        // - автоматическая вставлка/удаление (при завершении команды) AbortCombat
        // - фоновый процесс

#if DEVELOPER
        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractitTimeout' period")]
#else
        [Browsable(false)]
#endif
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
        internal bool _interactOnce;

#if DEVELOPER
        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("Time to interact (ms)")]
        [Category("Interaction")]
#else
        [Browsable(false)]
#endif
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

#if DEVELOPER
        [Description("Answers in dialog while interact with Entity")]
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dialogs)));
                }
            }
        }
        internal List<string> _dialogs = new List<string>();

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";
#endif


        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

#if CORE_INTERFACES
        [NonSerialized]
        internal IQuesterActionEngine Engine;
#endif

        public InteractEntities()
        {
#if CORE_INTERFACES
            Engine = new QuesterActionProxy(this);
#endif
        }
        #endregion

#if CORE_INTERFACES
        // Интерфейс Quester.Action через IQuesterActionEngine
        public override bool NeedToRun => Engine.NeedToRun;
        public override ActionResult Run() => Engine.Run();
        public override string ActionLabel => Engine.ActionLabel;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => Engine.UseHotSpots;
        protected override Vector3 InternalDestination => Engine.InternalDestination;
        protected override bool IntenalConditions => Engine.InternalConditions;
        protected override ActionValidity InternalValidity => Engine.InternalValidity;
        public override void InternalReset() => Engine.InternalReset();
        public override void GatherInfos() => Engine.GatherInfos();
        public override void OnMapDraw(GraphicsNW graph) => Engine.OnMapDraw(graph);
#endif
    }
}
