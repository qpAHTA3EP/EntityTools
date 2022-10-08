﻿using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using EntityTools.PropertyEditors;

namespace EntityTools.Quester.Conditions
{
    /// <summary>
    /// Проверка наличия хотя бы одного объекта Entity, подпадающих под шаблон EntityID,
    /// в регионе CustomRegion, заданным в CustomRegionNames
    /// </summary>
    [Serializable]
    public class EntityCount : Condition, INotifyPropertyChanged, IEntityDescriptor
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EntityID)));
                }
            }
        }
        private string _entityId = string.Empty;

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
        private EntityNameType _entityNameType = EntityNameType.InternalName;

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
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Category("Entity")]
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
        private EntitySetType _entitySetType = EntitySetType.Complete;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";
#endif

#if DEVELOPER
        [Description("Check Entity's Region:\n" +
            "True: Count Entity if it located in the same Region as Player\n" +
            "False: Does not consider the region when counting Entities")]
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
                if (_healthCheck = value)
                {
                    _healthCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HealthCheck)));
                }
            }
        }
        private bool _healthCheck = true;

#if DEVELOPER
        [Description("Threshold value to compare by 'Sign' with the number of the Entities")]
        [Category("Tested")]
#else
        [Browsable(false)]
#endif
        public uint Value
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
        private uint _value;

#if DEVELOPER
        [Description("The comparison type for the number of the Entities with 'Value'")]
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
        private Relation _sign = Relation.Superior;

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
                if (_reactionZRange != value)
                {
                    _reactionZRange = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionZRange)));
                }
            }
        }
        private float _reactionZRange;

#if DEVELOPER
        [Description("The list of the CustomRegions where Entities is counted")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Location")]
        [DisplayName("CustomRegions")]
#else
        [Browsable(false)]
#endif
        public CustomRegionCollection CustomRegionNames
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
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();


#if DEVELOPER
        [Category("Location")]
        [Description("The position of the counted entities relative to CustomRegions")]
#else
        [Browsable(false)]
#endif
        [XmlElement("Tested")]
        public Presence CustomRegionCheck
        {
            get => _customRegionCheck;
            set
            {
                if (_customRegionCheck != value)
                {
                    _customRegionCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomRegionCheck)));
                }
            }
        }
        private Presence _customRegionCheck = Presence.Equal;
        #endregion


        #region Взаимодействие с ядром EntityTools
        [NonSerialized]
        private IQuesterConditionEngine engine;
        public event PropertyChangedEventHandler PropertyChanged;

        public EntityCount()
        {
            engine = new QuesterConditionProxy(this);
        }

        public void Bind(IQuesterConditionEngine engine)
        {
            this.engine = engine;
        }
        public void Unbind()
        {
            engine = MakeProxy();
            PropertyChanged = null;
        }
        private IQuesterConditionEngine MakeProxy()
        {
            return new QuesterConditionProxy(this);
        }
        #endregion

        public override bool IsValid => LazyInitializer.EnsureInitialized(ref engine, MakeProxy).IsValid;
        public override void Reset() => LazyInitializer.EnsureInitialized(ref engine, MakeProxy).Reset();
        public override string TestInfos => LazyInitializer.EnsureInitialized(ref engine, MakeProxy).TestInfos;
        public override string ToString() => LazyInitializer.EnsureInitialized(ref engine, MakeProxy).Label();
    }
}
