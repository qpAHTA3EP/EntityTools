using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
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
                    NotifyPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _regionCheck;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _holdTargetEntity = true;

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
                if (Math.Abs(_reactionRange - value) > 0.1f)
                {
                    _reactionRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionRange;

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
                if (Math.Abs(_reactionZRange - value) > 0.1f)
                {
                    _reactionZRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

#if DEVELOPER
        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        [XmlElement("CustomRegionNames")]
        [DisplayName("CustomRegions")]
        [NotifyParentProperty(true)]
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();


#if DEVELOPER
        [Description("Distance to the Entity by which it is necessary to approach.\n" +
                     "Keep in mind that the distance below 5 is too small to display on the Mapper")]
        [Category("Interruptions")]
        [DisplayName("CombatDistance")]
#else
        [Browsable(false)]
#endif

        public float Distance
        {
            get => _distance; set
            {
                if (Math.Abs(_distance - value) > 0.1f)
                {
                    _distance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _distance = 30;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _ignoreCombat = true;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _abortCombatDistance;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _stopOnApproached;

        #region Опции команды
#if DEVELOPER
        [Description("The Id of the skill applied on the target Entity")]
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("First strike")]
#else
        [Browsable(false)]
#endif
        public string PowerId
        {
            get => _powerId;
            set
            {
                if (_powerId != value)
                {
                    _powerId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _powerId = string.Empty;

#if DEVELOPER
        [Description("The time needed to activate the skill '" + nameof(PowerId) + "'")]
        [Category("First strike")]
        [DisplayName("CastingTime (ms)")]
#else
        [Browsable(false)]
#endif
        public int CastingTime
        {
            get => _castingTime;
            set
            {
                if (_castingTime != value)
                {
                    _castingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _castingTime;



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
                if (_attackTargetEntity != value)
                {
                    _attackTargetEntity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _attackTargetEntity = true;

        //TODO Добавить опции IgnoreCombatMinHP, EntitySearchTime

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
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _resetCurrentHotSpot;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo => @"Нажми на кнопку '...' чтобы увидеть больше =>";

#if false
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть информацию о текущей цели")]
        public string TargetInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>"; 
#endif
#endif
#endregion

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
        #endregion

        #region Взаимодействие с ядром EntityToolsCore
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
