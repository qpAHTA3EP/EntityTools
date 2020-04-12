using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using EntityTools.Editors;
using MyNW.Classes;
using EntityTools.Enums;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToEntity : Astral.Quester.Classes.Action, INotifyPropertyChanged
    {
        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

#if CORE_DELEGATES
        internal Func<ActionResult> coreRun = null;
        internal Func<bool> coreNeedToRun = null;
        internal Func<bool> coreValidate = null;
        internal System.Action coreReset = null;
        internal System.Action coreGatherInfos = null;
        internal Func<string> coreString = null;
#endif
#if CORE_INTERFACES
        [NonSerialized]
        internal IQuesterActionEngine Engine;
#endif

        public MoveToEntity()
        {
#if CORE_INTERFACES
            Engine = new QuesterActionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion

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
        [Description("Type of and EntityID:\n" +
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
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        internal EntityNameType _entityNameType = EntityNameType.NameUntranslated;

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
        internal bool _regionCheck = false;

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
                if (_healthCheck = value)
                {
                    _healthCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck)));
                }
            }
        }
        internal bool _healthCheck = true;

#if DEVELOPER
        [Description("True: Do not change the target Entity while it is alive or until the Bot within 'Distance' of it\n" +
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
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
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
        internal float _reactionRange = 0;

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
        internal float _reactionZRange = 0;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionNames)));
                }
            }
        }
        internal List<string> _customRegionNames = new List<string>();

/*#if DEVELOPER
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
        internal int _searchTimeInterval = 100;*/

#if DEVELOPER
        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public float Distance
        {
            get => _distance; set
            {
                if (_distance != value)
                {
                    _distance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Distance)));
                }
            }
        }
        internal float _distance = 30;

#if DEVELOPER
        [Description("Enable 'IgnoreCombat' profile value while playing action")]
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

#if DEVELOPER
        [Description("True: Complete an action when the Entity is closer than 'Distance'\n" +
                     "False: Follow an Entity regardless of its distance")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public bool StopOnApproached
        {
            get => _stopOnApproached; set
            {
                if (_stopOnApproached != value)
                {
                    _stopOnApproached = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StopOnApproached)));
                }
            }
        }
        internal bool _stopOnApproached = false;

#if DEVELOPER
        [Description("True: Clear the list of attackers and attack the target Entity when it is approached\n" +
            "This option is ignored if 'IgnoreCombat' does not set")]
        [Category("Interruptions")]
#else
        [Browsable(false)]
#endif
        public bool AttackTargetEntity
        {
            get => _attackTargetEntity; set
            {
                if (_attackTargetEntity = value)
                {
                    _attackTargetEntity = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AttackTargetEntity)));
                }
            }
        }
        internal bool _attackTargetEntity = true;

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


#if CORE_DELEGATES
        // Интерфейс Quester.Action через делегаты
        public override string ActionLabel => $"{GetType().Name} [{_entityId}]";
        protected override bool IntenalConditions => !string.IsNullOrEmpty(_entityId);//Comparer != null;
        public override string InternalDisplayName => string.Empty;
        public override bool UseHotSpots => true;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (coreValidate())
                {
                    Entity target = coreTarget();
                    if (target.Location.Distance3DFromPlayer > Distance)
                        return target.Location.Clone();
                    else return EntityManager.LocalPlayer.Location.Clone();
                }
                return new Vector3();
            }
        }
        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(_entityId))
                {
                    return new ActionValidity($"EntityID property not set.");
                }
                return new ActionValidity();
            }
        }
        public override bool NeedToRun
        {
            get
            {
                if (coreNeedToRun == null)
                    EntityTools.Core.Initialize(this);
                return coreNeedToRun();
            }
        }
        public override ActionResult Run()
        {
            if (coreRun == null)
                EntityTools.Core.Initialize(this);
            return coreRun();
        }
        public override void OnMapDraw(GraphicsNW graph)
        {
            if (coreValidate())
            {
                graph.drawFillEllipse(getTarget().Location, new Size(10, 10), Brushes.Beige);
            }
        }
        public override void InternalReset() { }
        public override void GatherInfos() { }        
#endif
#if CORE_INTERFACES
        // Интерфес Quester.Action, реализованный через ActionEngine
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
#endif
    }
}
