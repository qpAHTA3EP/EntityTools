using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;

namespace EntityTools.UCC.Conditions
{
    public class UCCEntityCheck : UCCCondition, ICustomUCCCondition
    {
        #region Взаимодействие с EntityToolsCore
#if CORE_INTERFACES
        [NonSerialized]
        internal IUCCConditionEngine Engine;
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        public UCCEntityCheck()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
#if CORE_INTERFACES
            Engine = new UCCConditionProxy(this);
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
        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
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
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
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
        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public AuraOption Aura
        {
            get => _aura; set
            {
                if (_aura != value)
                {
                    _aura = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Aura)));
                }
            }
        }
        internal AuraOption _aura = new AuraOption();

#if !DEVELOPER
        [Browsable(false)]
#endif
        public EntityPropertyType PropertyType
        {
            get => _propertyType; set
            {
                if (_propertyType != value)
                {
                    _propertyType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyType)));
                }
            }
        }
        internal EntityPropertyType _propertyType = EntityPropertyType.Distance;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public float PropertyValue
        {
            get => _propertyValue; set
            {
                if (_propertyValue != value)
                {
                    _propertyValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyValue)));
                }
            }
        }
        internal float _propertyValue;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        //[Category("Entity")]
#else
        [Browsable(false)]
#endif
        public string TestInfo { get; } = "Нажми '...' =>";

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
        #endregion
        #endregion


        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/) => Engine.IsOK(refAction);

        bool ICustomUCCCondition.Loked { get => Locked; set => Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction) => Engine.TestInfos(refAction);
        #endregion


        public override string ToString() => Engine.Label();
    }
}
