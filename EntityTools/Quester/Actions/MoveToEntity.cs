using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using MyNW.Classes;
using Action = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToEntity : Action, INotifyPropertyChanged, IEntityDescriptor
    {
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
                    Engine.OnPropertyChanged(this, nameof(EntityID));
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityIdType))); 
#else
                    Engine.OnPropertyChanged(this, nameof(EntityIdType));
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
                    Engine.OnPropertyChanged(this, nameof(EntityNameType));
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
                    Engine.OnPropertyChanged(this, nameof(RegionCheck));
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
                if (_healthCheck = value)
                {
                    _healthCheck = value;
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck))); 
#else
                    Engine.OnPropertyChanged(this, nameof(HealthCheck));
#endif
                }
            }
        }
        internal bool _healthCheck = true;

#if DEVELOPER
        [Description("True: Do not change the target Entity while it is alive or until the Bot within '"+nameof(Distance)+"' of it\n" +
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
                    Engine.OnPropertyChanged(this, nameof(HoldTargetEntity));
#endif
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionRange))); 
#else
                    Engine.OnPropertyChanged(this, nameof(ReactionRange));
#endif
                }
            }
        }
        internal float _reactionRange;

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
                    Engine.OnPropertyChanged(this, nameof(ReactionZRange));
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
        [XmlElement("CustomRegionNames")]
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
                    Engine.OnPropertyChanged(this, nameof(CustomRegionNames));
#endif
                }
            }
        }
        internal CustomRegionCollection _customRegionNames = new CustomRegionCollection();


#if DEVELOPER
        [Description("Distance to the Entity by which it is necessary to approach.\n" +
                     "Keep in mind that the distance below 5 is too small to display on the Mapper")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Distance))); 
#else
                    Engine.OnPropertyChanged(this, nameof(Distance));
#endif
                }
            }
        }
        internal float _distance = 30;

#if DEVELOPER
        [Description("Enable '"+nameof(IgnoreCombat)+"' profile value while playing action")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IgnoreCombat))); 
#else
                    Engine.OnPropertyChanged(this, nameof(IgnoreCombat));
#endif
                }
            }
        }
        internal bool _ignoreCombat = true;

#if DEVELOPER
        [Description("The battle is aborted outside '"+nameof(AbortCombatDistance) +"' radius from the target entity.\n" +
                     "The combat is restored within the '"+nameof(Distance)+"' radius.\n" +
                     "However, this is not performed if the value less than '"+ nameof(Distance) +"' or '"+nameof(IgnoreCombat)+"' is False.")]
        [Category("Interruptions")]
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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AbortCombatDistance))); 
#else
                    Engine.OnPropertyChanged(this, nameof(AbortCombatDistance));
#endif
                }
            }
        }
        internal uint _abortCombatDistance;

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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StopOnApproached))); 
#else
                    Engine.OnPropertyChanged(this, nameof(StopOnApproached));
#endif
                }
            }
        }
        internal bool _stopOnApproached;

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
#if false
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AttackTargetEntity))); 
#else
                    Engine.OnPropertyChanged(this, nameof(AttackTargetEntity));
#endif
                }
            }
        }
        internal bool _attackTargetEntity = true;

#if DEVELOPER
        [Description("Reset current HotSpot after approaching the target Entity")]
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
                    Engine.OnPropertyChanged(this, nameof(ResetCurrentHotSpot));
#endif
                }
            }
        }
        internal bool _resetCurrentHotSpot = false;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";

#if false
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть информацию о текущей цели")]
        public string TargetInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>"; 
#endif
#endif

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;

        [XmlIgnore]
        [NonSerialized]
        private IQuesterActionEngine Engine;

        public MoveToEntity()
        {
            Engine = new QuesterActionProxy(this);
        }

        public void Bind(IQuesterActionEngine engine)
        {
            Engine = engine;
        }
        public void Unbind()
        {
            Engine = new QuesterActionProxy(this);
            PropertyChanged = null;
        }
        #endregion

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
    }
}
