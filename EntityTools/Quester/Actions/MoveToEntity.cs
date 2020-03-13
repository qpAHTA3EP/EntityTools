using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using EntityTools.Editors;
using MyNW.Classes;
using MyNW.Internals;
using EntityTools.Extentions;
using EntityTools.Enums;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EntityCore")]

namespace EntityTools.Quester.Actions
{
    [Serializable]
    public class MoveToEntity : Astral.Quester.Classes.Action, INotifyPropertyChanged
    {
        public MoveToEntity() { }

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
        private string _entityId = string.Empty;

        [Description("Type of and EntityID:\n" +
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
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => entityNameType;
            set
            {
                if (entityNameType != value)
                {
                    entityNameType = value;
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        private EntityNameType entityNameType = EntityNameType.NameUntranslated;

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
        private bool _regionCheck = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
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
        private bool _healthCheck = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within 'Distance' of it\n" +
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
        private bool _holdTargetEntity = true;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
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
        private float _reactionRange = 0;

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
        private float _reactionZRange = 0;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionNames)));
                }
            }
        }
        [NonSerialized]
        private List<string> _customRegionNames = new List<string>();

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
        private int _searchTimeInterval = 100;

        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Interruptions")]
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
        private float _distance = 30;

        [Description("Enable 'IgnoreCombat' profile value while playing action")]
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
        private bool _ignoreCombat = true;

        [Description("True: Complete an action when the Entity is closer than 'Distance'\n" +
                     "False: Follow an Entity regardless of its distance")]
        [Category("Interruptions")]
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
        private bool _stopOnApproached = false;

        [Description("True: Clear the list of attackers and attack the target Entity when it is approached\n" +
            "This option is ignored if 'IgnoreCombat' does not set")]
        [Category("Interruptions")]
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
        private bool _attackTargetEntity = true;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";
        #endregion

        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;

        internal Func<ActionResult> coreRun = null;
        internal Func<bool> coreNeedToRun = null;
        internal Func<bool> coreValidate = null;
        internal System.Action coreReset = null;
        internal System.Action coreGatherInfos = null;
        internal Func<string> coreString = null;
        #endregion

        #region Интерфейс Quester.Action
        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";
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
        #endregion
    }
}
