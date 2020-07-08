using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using static Astral.Logic.UCC.Ressources.Enums;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class DodgeFromEntity : UCCAction
    {
        #region Взаимодействие с EntityToolsCore
        private Dodge dodge = new Dodge();
#if CORE_INTERFACES
        [NonSerialized]
        internal IUCCActionEngine Engine;
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        public DodgeFromEntity()
        {
            Target = Unit.Player;
            dodge.Direction = DodgeDirection.DodgeSmart;
#if CORE_INTERFACES
            Engine = new UCCActionProxy(this);
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityNameType)));
                }
            }
        }
        internal EntityNameType _entityNameType = EntityNameType.InternalName;

#if DEVELOPER
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public float EntityRadius
        {
            get => _entityRadius;
            set
            {
                if (_entityRadius != value)
                {
                    _entityRadius = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityRadius)));
                }
            }
        }
        internal float _entityRadius = 12;

#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool RegionCheck
        {
            get => _regionCheck;
            set
            {
                if (_regionCheck != value)
                {
                    _regionCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegionCheck)));
                }
            }
        }
        internal bool _regionCheck = true;

#if DEVELOPER
        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool HealthCheck
        {
            get => _healthCheck;
            set
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
        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public AuraOption Aura
        {
            get => _aura;
            set
            {
                if (_aura != value)
                {
                    _aura = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Aura)));
                }
            }
        }
        internal AuraOption _aura = new AuraOption();

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The 0 (zero) value disables distance checking")]
        //[Category("Entity")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionRange
        {
            get => _reactionRange;
            set
            {
                if (_reactionRange != value)
                {
                    _reactionRange = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionRange)));
                }
            }
        }
        internal float _reactionRange = 30;

#if DEVELOPER
        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionZRange
        {
            get => _reactionZRange;
            set
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
        [DisplayName("Moving time")]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public int MovingTime
        {
            get => _movingTime;
            set
            {
                if (_movingTime != value)
                {
                    _movingTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MovingTime)));
                }
            }
        }
        internal int _movingTime = 700;

#if DEVELOPER
        [DisplayName("Dodge Direction")]
        [Category("Required")]
#else
        [Browsable(false)]
#endif
        public DodgeDirection Direction
        {
            get => _dodgeDirection;
            set
            {
                if (_dodgeDirection != value)
                {
                    _dodgeDirection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Direction)));
                }
            }
        }
        internal DodgeDirection _dodgeDirection = DodgeDirection.DodgeSmart;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
        public string TestInfo { get; } = "Нажми '...' =>";
#endif

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new int Timer { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
        #endregion

        public override bool NeedToRun => Engine.NeedToRun;

        public override bool Run() => Engine.Run();

        /// <summary>
        /// Ссылка на ближайший к персонажу Entity
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => Engine.UnitRef;

        public override string ToString() => Engine.Label();

        public override UCCAction Clone()
        {
            return base.BaseClone(new DodgeFromEntity
            {
                _entityId = this._entityId,
                _entityIdType = this._entityIdType,
                _entityNameType = this._entityNameType,
                _regionCheck = this._regionCheck,
                _healthCheck = this._healthCheck,
                _entityRadius = this._entityRadius,
                _reactionRange = this._reactionRange,
                _reactionZRange = this._reactionZRange,
                _aura = new AuraOption
                {
                    AuraName = this._aura.AuraName,
                    AuraNameType = this._aura.AuraNameType,
                    Sign = this._aura.Sign,
                    Stacks = this._aura.Stacks
                }
            });
        }
    }
}
