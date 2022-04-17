using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ExecuteSpecificPower : UCCAction
    {
        #region Опции команды
#if DEVELOPER
        //[Editor(typeof(PowerAllIdEditor), typeof(UITypeEditor))]
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        internal bool _forceMaintain;

#if EntityTarget
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        internal EntityNameType _entityNameType = EntityNameType.InternalName;

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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        internal float _reactionZRange;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Press '...', to test 'TargetEntity' options.")]
        [Category("TargetEntity")]
        public string TestEntity => "Нажми '...' =>";
#endif
#endif

#if DEVELOPER
        [Category("Optional")]
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
        [Description("Набор нестандартных UCC-условий, которые проверяют после основных")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public UCCConditionPack CustomConditions
        {
            get => _customConditions;
            set
            {
                if (ReferenceEquals(_customConditions, value))
                    return;

                _customConditions = value;
                NotifyPropertyChanged();
            }
        }
        internal UCCConditionPack _customConditions = new UCCConditionPack();

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        #endregion
        #endregion

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccActionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            //Engine.OnPropertyChanged(this, propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExecuteSpecificPower()
        {
            Target = Astral.Logic.UCC.Ressources.Enums.Unit.Target;
            Engine = new UccActionProxy(this);
        }
        private IUccActionEngine MakeProxy()
        {
            return new UccActionProxy(this);
        }
        #endregion

        #region Интерфейс команды
        public override bool NeedToRun => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).NeedToRun;
        public override bool Run() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Run();
        [XmlIgnore]
        [Browsable(false)]
        public Entity UnitRef => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).UnitRef;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();
        #endregion

        public override UCCAction Clone()
        {
            return BaseClone(new ExecuteSpecificPower
            {
                _powerId = _powerId,
                _checkPowerCooldown = _checkPowerCooldown,
                _checkInTray = _checkInTray,
                _castingTime = _castingTime,
                _forceMaintain = _forceMaintain,
#if EntityTarget
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
#endif
            });
        }
    }
}
