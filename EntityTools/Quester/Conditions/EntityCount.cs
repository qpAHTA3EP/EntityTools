using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using MyNW.Classes;

namespace EntityTools.Quester.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>

    [Serializable]
    public class EntityCount : Condition
    {
        public EntityCount() { }

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
        private EntityNameType _entityNameType = EntityNameType.NameUntranslated;

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

        [Description("Check Entity's Region:\n" +
            "True: Count Entity if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting Entities")]
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

        [Description("Threshold value to compare by 'Sign' with the number of the Entities")]
        [Category("Tested")]
        public uint Value
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
        private uint _value = 0;

        [Description("The comparison type for the number of the Entities with 'Value'")]
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

        [Category("Location")]
        public Condition.Presence Tested
        {
            get => _tested; set
            {
                if (_tested != value)
                {
                    _tested = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tested)));
                }
            }
        }
        private Presence _tested = Condition.Presence.Equal;

        [Description("The list of the CustomRegions where Entities is counted")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Location")]
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

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
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

        //[Description("Time between searches of the Entity (ms)")]
        //[Category("Optional")]
        //public int SearchTimeInterval { get; set; } = 100; 
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
