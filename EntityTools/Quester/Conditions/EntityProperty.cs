using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class EntityProperty : Condition
    {
        public EntityProperty() { }

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

        [Description("Check Entity's Region:\n" +
            "True: Search an Entity only if it located in the same Region as Player\n" +
            "False: Does not consider the region when searching Entities")]
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


        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType
        {
            get => _entitySetType; set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntitySetType)));
                }
            }
        }
        private EntitySetType _entitySetType = EntitySetType.Complete;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
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
        private List<string> _customRegionNames = new List<string>();

        [Category("Tested")]
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
        private EntityPropertyType _propertyType = EntityPropertyType.Distance;

        [Category("Tested")]
        public float Value
        {
            get => _value; set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }
        private float _value = 0;

        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
        public Relation Sign
        {
            get => _sign; set
            {
                if (_sign != value)
                {
                    _sign = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sign)));
                }
            }
        }
        private Relation _sign = Relation.Superior;

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
        #endregion

        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;
        public Func<bool> doValidate = null;
        public Func<string> getString = null;
        public Func<string> getTestInfos = null;
        public System.Action doReset = null;
        #endregion

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
    }
}