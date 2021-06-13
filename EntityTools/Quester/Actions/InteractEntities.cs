#define DEBUG_INTERACTENTITIES
//#define PROFILING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class InteractEntities : Action, INotifyPropertyChanged, IEntityDescriptor
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityID))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityIdType))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegionCheck))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoldTargetEntity))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SkipMoving))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionRange))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionZRange))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal float _reactionZRange;

#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionNames))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal CustomRegionCollection _customRegionNames = new CustomRegionCollection();

#if DEVELOPER
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntitySetType))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
            
        }
        internal EntitySetType _entitySetType = EntitySetType.Contacts;

#if DEVELOPER
        [Category("Interruptions")]
        [Description("Distance to the Entity by which it is necessary to approach to disable 'IgnoreCombat' mode\n" +
            "Ignored if 'IgnoreCombat' does not True")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CombatDistance))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal float _combatDistance = 30;

#if DEVELOPER
        [Category("Interruptions")]
        [Description("Enable IgnoreCombat mode while distance to the closest Entity greater then 'CombatDistance'")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IgnoreCombat))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractOnce))); 
#else
                    OnPropertyChanged();
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractingTimeout))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal int _interactingTimeout = 60;

#if DEVELOPER
        [Category("Interaction")]
        [Description("Time to interact (ms)")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractTime))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal int _interactTime = 2000;

#if DEVELOPER
        [Category("Interaction")]
        [Description("The distance to the Entity within which the interaction is possible")]
#else
        [Browsable(false)]
#endif
        public int InteractDistance
        {
            get => _interactDistance;
            set
            {
                if (_interactDistance != value)
                {
                    _interactDistance = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal int _interactDistance = 5;

#if DEVELOPER
        [Category("Interaction")]
        [Description("Answers in dialog while interact with Entity")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Dialogs))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal List<string> _dialogs = new List<string>();

#if DEVELOPER
        [Description("Reset current HotSpot after interaction and move to the closest one")]
#else
        [Browsable(false)]
#endif
        public bool ResetCurrentHotSpot
        {
            get => _resetCurrentHotSpot; set
            {
                if (_resetCurrentHotSpot != value)
                {
                    _resetCurrentHotSpot = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResetCurrentHotSpot))); 
#else
                    OnPropertyChanged();
#endif
                }
            }
        }
        internal bool _resetCurrentHotSpot;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        private IQuesterActionEngine Engine;

        public InteractEntities()
        {
            Engine = MakeProxy();
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

        // Интерфейс Quester.Action через IQuesterActionEngine
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
