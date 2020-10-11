using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class EntityProperty : Condition, INotifyPropertyChanged
    {
        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;
#if CORE_DELEGATES
        public Func<bool> doValidate = null;
        public Func<string> getString = null;
        public Func<string> getTestInfos = null;
        public System.Action doReset = null;
#endif
#if CORE_INTERFACES
        internal IQuesterConditionEngine Engine;
#endif
        #endregion

        public EntityProperty()
        {
#if CORE_INTERFACES
            Engine = new QuesterConditionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }

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
        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck)));
                }
            }
        }
        internal bool _healthCheck = true;


#if DEVELOPER
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public EntitySetType EntitySetType
        {
            get => _entitySetType;
            set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntitySetType)));
                }
            }
        }
        internal EntitySetType _entitySetType = EntitySetType.Complete;

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

#if DEVELOPER
        [Category("Tested")]
#else
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

#if DEVELOPER
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public float Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }
        internal float _value;

#if DEVELOPER
        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public Relation Sign
        {
            get => _sign;
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sign)));
                }
            }
        }
        internal Relation _sign = Relation.Superior;

#if false
#if DEVELOPER
        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public int SearchTimeInterval
        {
            get => _searchTimeInterval;
            set
            {
                if (_searchTimeInterval != value)
                {
                    _searchTimeInterval = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchTimeInterval)));
                }
            }
        }
        internal int _searchTimeInterval = 200; 
#endif

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";
#endif
        #endregion

#if CORE_DELEGATES
       public override string ToString()
        {
            if (getString == null)
                EntityTools.Core.Initialize(this);
            return getString();
        }

        public override bool IsValid
        {
            get
            {
                if (doValidate == null)
                    EntityTools.Core.Initialize(this);
                return doValidate();
            }
        }

        public override string TestInfos
        {
            get
            {
                if (getTestInfos == null)
                    EntityTools.Core.Initialize(this);
                return getTestInfos();
            }
        }

        public override void Reset()
        {
            if (doReset == null)
                EntityTools.Core.Initialize(this);
            doReset();
        }
#endif
#if CORE_INTERFACES
        public override bool IsValid => Engine.IsValid;
        public override void Reset() => Engine.Reset();
        public override string TestInfos => Engine.TestInfos;
        public override string ToString() => Engine.Label();
#endif
    }
}