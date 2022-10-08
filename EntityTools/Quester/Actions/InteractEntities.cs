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
using EntityTools.PropertyEditors;
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

        #region Entity
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private string _entityId = string.Empty;

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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

#if DEVELOPER
        [Description("A subset of entities that are searched for a target:\n" +
                     "Contacts: Only interactable Entities\n" +
                     "Complete: All possible Entities")]
        [Category("Entity")]
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
                    NotifyPropertyChanged();
                }
            }

        }
        private EntitySetType _entitySetType = EntitySetType.Contacts;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the entity searching.")]
        [Category("Entity")]
        public string TestSearch => @"Push button '...' =>";
#endif
        #endregion


        #region EntitySearchOptions
#if DEVELOPER
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity Search Options")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _healthCheck = true;

#if DEVELOPER
        [Description("True: Do not change the target Entity while it is alive or until the Bot within " + nameof(InteractDistance) + " of it.\n" +
                    "False: Constantly scan an area and target the nearest Entity.")]
        [Category("Entity Search Options")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _holdTargetEntity = true;

#if DEVELOPER
        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Entity Search Options")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _skipMoving;
        #endregion


        #region SearchArea
#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected;\n" +
            "False: Entity's Region does not checked during search.")]
        [Category("Search Area")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _regionCheck;

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched.\n" +
                     "The value equals 0(zero) disables distance checking.")]
        [Category("Search Area")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private float _reactionRange = 150;
#if DEVELOPER
        [Description("The maximum Z-coordiante difference with Player within which the Entity is searched.\n" +
                     "The default value is 0, which disables Z-coordiante checking.")]
        [Category("Search Area")]
#else
        [Browsable(false)]
#endif
        public float ReactionZRange
        {
            get => _reactionZRange; set
            {
                if (Math.Abs(_reactionZRange - value) > 0.1f)
                {
                    _reactionZRange = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionZRange))); 
#else
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private float _reactionZRange;

#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Search Area")]
        [DisplayName("CustomRegions")]
#else
        [Browsable(false)]
#endif
        [NotifyParentProperty(true)]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

#if DEVELOPER
        [Description("Checking the Player is in the area, defined by 'CustomRegions'")]
        [Category("Search Area")]
#else
        [Browsable(false)]
#endif
        public bool CustomRegionsPlayerCheck
        {
            get => customRegionsPlayerCheck; set
            {
                if (customRegionsPlayerCheck != value)
                {
                    customRegionsPlayerCheck = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool customRegionsPlayerCheck;

#if DEVELOPER
        [Description("The name of the Map where current action can be run.\n" +
                     "Ignored if empty.")]
        [Editor(typeof(CurrentMapEdit), typeof(UITypeEditor))]
        [Category("Search Area")]
#else
        [Browsable(false)]
#endif        
        public string CurrentMap
        {
            get => currentMap;
            set
            {
                if (value != currentMap)
                {
                    currentMap = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string currentMap;
        #endregion


        #region Manage Combat Options
#if DEVELOPER
        [Description("Ignore the enemies while approaching the target Entity.\n" +
                     "The minimum value is 5.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public float CombatDistance
        {
            get => _combatDistance; 
            set
            {
                if (value < 5)
                    value = 5;

                if (Math.Abs(_combatDistance - value) > 0.1)
                {
                    _combatDistance = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CombatDistance))); 
#else
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private float _combatDistance = 30;

#if DEVELOPER
        [Description("Ignore the enemies while approaching the " + nameof(Entity) + ".")]
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
                    value = -1;
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

#if DEVELOPER
        [Description("The battle is aborted outside '" + nameof(AbortCombatDistance) + "' radius from the target entity.\n" +
                     "The combat is restored within the '" + nameof(InteractDistance) + "' radius.\n" +
                     "However, this is not performed if the value less than '" + nameof(InteractDistance) + "' or '" + nameof(IgnoreCombat) + "' is False.")]
        [Category("Manage Combat Options")]
#else
        [Browsable(false)]
#endif
        public uint AbortCombatDistance
        {
            get => _abortCombatDistance; set
            {
                if (_abortCombatDistance != value)
                {
                    _abortCombatDistance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _abortCombatDistance;
        #endregion


        #region Interaction
#if DEVELOPER
        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractingTimeout' period")]
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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _interactOnce;

#if DEVELOPER
        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set.")]
        [DisplayName("InteractionTimeout")]
#else
        [Browsable(false)]
#endif
        public int InteractingTimeout
        {
            get => _interactingTimeout; 
            set
            {
                if (value < 0)
                    value = 0;

                if (_interactingTimeout != value)
                {
                    _interactingTimeout = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractingTimeout))); 
#else
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private int _interactingTimeout = 60;

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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private int _interactTime = 2000;

#if DEVELOPER
        [Category("Interaction")]
        [Description("The distance to the Entity within which the interaction is possible.\n" +
                     "Minimum value is 5.")]
#else
        [Browsable(false)]
#endif
        public int InteractDistance
        {
            get => _interactDistance;
            set
            {
                if (value < 5)
                    value = 5;

                if (_interactDistance != value)
                {
                    _interactDistance = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InteractDistance))); 
#else
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private int _interactDistance = 5;

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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private List<string> _dialogs = new List<string>(); 
        #endregion


        #region Interruptions

#if DEVELOPER
        [Description("The command is interrupted upon entity search timer reaching zero (ms).\n" +
                     "Set zero value to infinite search.")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public int EntitySearchTime
        {
            get => _entitySearchTime;
            set
            {
                if (value < 0)
                    value = 0;
                if (_entitySearchTime != value)
                {
                    _entitySearchTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _entitySearchTime;
        #endregion

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
                    NotifyPropertyChanged();
#endif
                }
            }
        }
        private bool _resetCurrentHotSpot;


        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";

        #region Взаимодействие с EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NonSerialized]
        private IQuesterActionEngine _engine;

        public InteractEntities()
        {
            _engine = MakeProxy();
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

        // Интерфейс Quester.Action через IQuesterActionEngine
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
