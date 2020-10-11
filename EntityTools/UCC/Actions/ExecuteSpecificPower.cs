#if DEBUG
#define DEBUG_ExecuteSpecificPower
#endif
#define REFLECTION_ACCESS

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ExecuteSpecificPower : UCCAction
    {
        #region Взаимодействие с EntityToolsCore
#if CORE_INTERFACES
        [NonSerialized]
        internal IUCCActionEngine Engine;
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        public ExecuteSpecificPower()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Target;
#if CORE_INTERFACES
            Engine = new UCCActionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion

        #region Опции команды
#if DEVELOPER
        [Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
        [Category("Required")]
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PowerId)));
                }
            }
        }
        internal string _powerId = string.Empty;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckPowerCooldown
        {
            get => _checkPowerCooldown;
            set
            {
                if (_checkPowerCooldown != value)
                {
                    _checkPowerCooldown = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckPowerCooldown)));
                }
            }
        }
        internal bool _checkPowerCooldown;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckInTray
        {
            get => _checkInTray;
            set
            {
                if (_checkInTray != value)
                {
                    _checkInTray = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckInTray)));
                }
            }
        }
        internal bool _checkInTray;

#if DEVELOPER
        [Category("Optional")]
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CastingTime)));
                }
            }
        }
        internal int _castingTime;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool ForceMaintain
        {
            get => _forceMaintain;
            set
            {
                if (_forceMaintain != value)
                {
                    _forceMaintain = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceMaintain)));
                }
            }
        }
        internal bool _forceMaintain;

#if DEVELOPER
        [Description("ID of the Entity that is preferred to attack\n" +
            "If Entity does not exist or EntityID is empty then the Target option is used")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("TargetEntity")]
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
        [Category("TargetEntity")]
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
        [Category("TargetEntity")]
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
        internal EntityNameType _entityNameType = EntityNameType.NameUntranslated;

#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("TargetEntity (Optional)")]
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
        [Category("TargetEntity (Optional)")]
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
        [Category("TargetEntity (Optional)")]
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
            "The default value is 0, which disables distance checking")]
        [Category("TargetEntity (Optional)")]
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
        internal float _reactionRange = 60;

#if DEVELOPER
        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("TargetEntity (Optional)")]
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
        internal float _reactionZRange;

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
        public new string ActionName { get; set; }
        #endregion
        #endregion

        [XmlIgnore]
        [Browsable(false)]
        public override bool NeedToRun => Engine.NeedToRun;

        public override bool Run() => Engine.Run();

        public override UCCAction Clone()
        {
            return BaseClone(new ExecuteSpecificPower
            {
                _powerId = _powerId,
                _checkPowerCooldown = _checkPowerCooldown,
                _checkInTray = _checkInTray,
                _castingTime = _castingTime,
                _forceMaintain = _forceMaintain,
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType,
                _regionCheck = _regionCheck,
                _healthCheck = _healthCheck,
                _aura = new AuraOption
                {
                    AuraName = _aura.AuraName,
                    AuraNameType = _aura.AuraNameType,
                    Sign = _aura.Sign,
                    Stacks = _aura.Stacks
                }
            });
        }

        public override string ToString() => Engine.Label();

        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => Engine.UnitRef;
    }
}
