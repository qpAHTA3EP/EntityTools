using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;

using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.Quester.Conditions
{
    [Serializable]
    public class EntityProperty : Condition, INotifyPropertyChanged, IEntityDescriptor
    {
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
                    _key = null;
                    _label = string.Empty;
                    entity = null;
                    NotifyPropertyChanged();
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
                    _key = null;
                    _label = string.Empty;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

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
                    _key = null;
                    _label = string.Empty;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";

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
                    _specialCheck = null;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _regionCheck;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck
        {
            get => _healthCheck; set
            {
                if (_healthCheck == value)
                {
                    _healthCheck = value;
                    _specialCheck = null;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;


        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Category("Optional")]
        public EntitySetType EntitySetType
        {
            get => _entitySetType;
            set
            {
                if (_entitySetType != value)
                {
                    _entitySetType = value;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntitySetType _entitySetType = EntitySetType.Complete;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
        public float ReactionRange
        {
            get => _reactionRange;
            set
            {
                if (Math.Abs(_reactionRange - value) > 0.1)
                {
                    _reactionRange = value;
                    _specialCheck = null;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionRange;

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange
        {
            get => _reactionZRange;
            set
            {
                if (Math.Abs(_reactionZRange - value) > 0.1)
                {
                    _reactionZRange = value;
                    _specialCheck = null;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public CustomRegionCollection CustomRegionNames
        {
            get => _customRegionNames;
            set
            {
                if (_customRegionNames != value)
                {
                    _customRegionNames = value;
                    _specialCheck = null;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private CustomRegionCollection _customRegionNames = new CustomRegionCollection();

        [Category("Tested")]
        public EntityPropertyType PropertyType
        {
            get => _propertyType; set
            {
                if (_propertyType != value)
                {
                    _propertyType = value;
                    propertyValueChecker = Initialize_PropertyValueChecker;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityPropertyType _propertyType = EntityPropertyType.Distance;

        [Category("Tested")]
        public float Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) > 0.1)
                {
                    _value = value;
                    propertyValueChecker = Initialize_PropertyValueChecker;
                    entity = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _value;

        [Description("Value comparison type to the closest Entity")]
        [Category("Tested")]
        public Relation Sign
        {
            get => _sign;
            set
            {
                if (_sign != value)
                {
                    _sign = value;
                    propertyValueChecker = Initialize_PropertyValueChecker;
                    NotifyPropertyChanged();
                }
            }
        }
        private Relation _sign = Relation.Superior;
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Данные
        private Entity entity;
        private Timeout timeout = new Timeout(0);

        private string _label = string.Empty;
        #endregion
        



        public override bool IsValid
        {
            get
            {
                var entityKey = EntityKey;
                bool isValid = entityKey.Validate(entity);
                if (timeout.IsTimedOut || !isValid)
                {
                    entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);
                    isValid = entity != null;
                    timeout.ChangeTime(EntityTools.Config.EntityCache.LocalCacheTime);
                }

                if (isValid)
                {
                    return propertyValueChecker(entity);
                }

                return PropertyType == EntityPropertyType.Distance && Sign == Relation.Superior;
            }
        }

        public override void Reset() { }

        public override string TestInfos
        {
            get
            {
                entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);

                if (EntityKey.Validate(entity))
                {
                    return string.Concat("Found closest Entity [",
                        EntityNameType == EntityNameType.NameUntranslated ? entity.NameUntranslated : entity.InternalName,
                        "] which ", PropertyType, " = ",
                        PropertyType == EntityPropertyType.Distance ? entity.Location.Distance3DFromPlayer.ToString("N2") :
                        PropertyType == EntityPropertyType.ZAxis ? entity.Location.Z.ToString("N2") : entity.Character.AttribsBasic.HealthPercent.ToString("N2"));
                }
                
                return string.Concat("No one Entity matched to [", EntityID, ']', Environment.NewLine,
                    PropertyType == EntityPropertyType.Distance ? "The distance to the missing Entity is considered equal to infinity." : string.Empty);
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
                _label = $"Entity [{EntityID}] {PropertyType} {Sign} to {Value}";
            return _label;
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType, EntitySetType.Complete));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                            ReactionRange, ReactionZRange,
                                                            RegionCheck,
                                                            CustomRegionNames);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion

        #region EntityPropertyChecker
        /// <summary>
        /// Предикат, проверяющий истинность соотношения <seealso cref="Sign"/> 
        /// между величиной атрибута, заданного <seealso cref="PropertyType"/>, 
        /// и референтным значением <seealso cref="Value"/>
        /// </summary>
        Predicate<Entity> propertyValueChecker;
        private bool Initialize_PropertyValueChecker(Entity e)
        {
            if (e is null)
                return false;

            switch (PropertyType)
            {
                case EntityPropertyType.Distance:
                    switch (Sign)
                    {
                        case Relation.Equal:
                            propertyValueChecker = Distance_Equal;
                            break;
                        case Relation.NotEqual:
                            propertyValueChecker = Distance_NotEqual;
                            break;
                        case Relation.Inferior:
                            propertyValueChecker = Distance_Inferior;
                            break;
                        case Relation.Superior:
                            propertyValueChecker = Distance_Superior;
                            break;
                    }
                    break;
                case EntityPropertyType.HealthPercent:
                    switch (Sign)
                    {
                        case Relation.Equal:
                            propertyValueChecker = HealthPercent_Equal;
                            break;
                        case Relation.NotEqual:
                            propertyValueChecker = HealthPercent_NotEqual;
                            break;
                        case Relation.Inferior:
                            propertyValueChecker = HealthPercent_Inferior;
                            break;
                        case Relation.Superior:
                            propertyValueChecker = HealthPercent_Superior;
                            break;
                    }
                    break;
                case EntityPropertyType.ZAxis:
                    switch (Sign)
                    {
                        case Relation.Equal:
                            propertyValueChecker = ZAxis_Equal;
                            break;
                        case Relation.NotEqual:
                            propertyValueChecker = ZAxis_NotEqual;
                            break;
                        case Relation.Inferior:
                            propertyValueChecker = ZAxis_Inferior;
                            break;
                        case Relation.Superior:
                            propertyValueChecker = ZAxis_Superior;
                            break;
                    }
                    break;
            }

            return propertyValueChecker(e);
        }

        private bool Distance_Inferior(Entity e) => entity.Location.Distance3DFromPlayer < Value;
        private bool Distance_Superior(Entity e) => entity.Location.Distance3DFromPlayer > Value;
        private bool Distance_Equal(Entity e) => Math.Abs(entity.Location.Distance3DFromPlayer - Value) <= 1;
        private bool Distance_NotEqual(Entity e) => Math.Abs(entity.Location.Distance3DFromPlayer - Value) > 1;

        private bool HealthPercent_Inferior(Entity e) => entity.Character.AttribsBasic.HealthPercent < Value;
        private bool HealthPercent_Superior(Entity e) => entity.Character.AttribsBasic.HealthPercent > Value;
        private bool HealthPercent_Equal(Entity e) => Math.Abs(entity.Character.AttribsBasic.HealthPercent - Value) <= 1;
        private bool HealthPercent_NotEqual(Entity e) => Math.Abs(entity.Character.AttribsBasic.HealthPercent - Value) > 1;

        private bool ZAxis_Inferior(Entity e) => entity.Z < Value;
        private bool ZAxis_Superior(Entity e) => entity.Z > Value;
        private bool ZAxis_Equal(Entity e) => Math.Abs(entity.Z - Value) <= 1;
        private bool ZAxis_NotEqual(Entity e) => Math.Abs(entity.Z - Value) > 1;

        #endregion
    }
}