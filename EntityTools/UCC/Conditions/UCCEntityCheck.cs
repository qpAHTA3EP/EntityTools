using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;

using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using EntityTools.UCC.Extensions;
using MyNW.Classes;
using Timeout = Astral.Classes.Timeout;

namespace EntityTools.UCC.Conditions
{
    public class UCCEntityCheck : UCCCondition, ICustomUCCCondition, IEntityDescriptor, INotifyPropertyChanged
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

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
        private bool _regionCheck;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
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
        private bool _healthCheck = true;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private float _reactionZRange;

        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Optional")]
        public AuraOption Aura
        {
            get => _aura; set
            {
                if (_aura != value)
                {
                    _aura = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private AuraOption _aura = new AuraOption();
        public bool ShouldSerializeAura() => !string.IsNullOrEmpty(_aura.AuraName);

        public EntityPropertyType PropertyType
        {
            get => _propertyType; set
            {
                if (_propertyType != value)
                {
                    _propertyType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EntityPropertyType _propertyType = EntityPropertyType.Distance;

        public float PropertyValue
        {
            get => _propertyValue; set
            {
                if (Math.Abs(_propertyValue - value) > 0.1)
                {
                    _propertyValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private float _propertyValue;


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


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string memberName = default)
        {
            _key = null;
            _specialCheck = null;
            _label = string.Empty;
            entity = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(memberName)));
        }
        #endregion




        public new ICustomUCCCondition Clone()
        {
            var copy = new UCCEntityCheck
            {
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType,
                _regionCheck = _regionCheck,
                _healthCheck = _healthCheck,
                _reactionRange = _reactionRange,
                _reactionZRange = _reactionZRange,
                _aura = new AuraOption
                {
                    AuraName = _aura.AuraName,
                    AuraNameType = _aura.AuraNameType,
                    Sign = _aura.Sign,
                    Stacks = _aura.Stacks
                },
                _propertyType = _propertyType,
                _propertyValue = _propertyValue,

                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Tested = Tested,
                Value = Value
            };
            return copy;
        }

        


        #region Данные
        private Entity entity;
        private readonly Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        #endregion

        

        

        public new bool IsOK(UCCAction refAction)
        {
            Entity targetEntity = refAction?.GetTarget();

            var entityKey = EntityKey;

            if (entityKey.Validate(targetEntity))
                entity = targetEntity;
            else if (timeout.IsTimedOut)
            {
                entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
                timeout.ChangeTime(EntityTools.Config.EntityCache.CombatCacheTime / 2);
            }


            if (entityKey.Validate(entity))
            {
                switch (PropertyType)
                {
                    case EntityPropertyType.Distance:
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Location.Distance3DFromPlayer - PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Location.Distance3DFromPlayer - PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Location.Distance3DFromPlayer < PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Location.Distance3DFromPlayer > PropertyValue;
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Character.AttribsBasic.HealthPercent - PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Character.AttribsBasic.HealthPercent - PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Character.AttribsBasic.HealthPercent < PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Character.AttribsBasic.HealthPercent > PropertyValue;
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        switch (Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Location.Z - PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Location.Z - PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Location.Z < PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Location.Z > PropertyValue;
                        }
                        break;
                }
                return false;
            }
            // Если Entity не найдено, условие будет истино в единственном случае:
            return PropertyType == EntityPropertyType.Distance
                   && Sign == Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity closestEntity = refAction?.GetTarget();

            var entityKey = EntityKey;

            if (!entityKey.Validate(closestEntity))
                closestEntity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

            if (entityKey.Validate(closestEntity))
            {
                StringBuilder sb = new StringBuilder("Found closest Entity");
                var propType = PropertyType;
                sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(propType).Append(" = ");
                switch (propType)
                {
                    case EntityPropertyType.Distance:
                        sb.Append(closestEntity.Location.Distance3DFromPlayer);
                        break;
                    case EntityPropertyType.ZAxis:
                        sb.Append(closestEntity.Location.Z);
                        break;
                    case EntityPropertyType.HealthPercent:
                        sb.Append(closestEntity.Character?.AttribsBasic?.HealthPercent);
                        break;
                }
                return sb.ToString();
            }
            else
            {
                StringBuilder sb = new StringBuilder("No one Entity matched to");
                sb.Append(" [").Append(EntityID).Append(']').AppendLine();
                if (PropertyType == EntityPropertyType.Distance)
                    sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                return sb.ToString();
            }
        }

        public string Label()
        {
            _label = string.IsNullOrEmpty(_label) 
                   ? $"{GetType().Name} [{EntityID}]" 
                   : GetType().Name;

            return _label;
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// Использовать самомодифицирующийся предиката, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                            ReactionRange,
                                                            ReactionZRange > 0
                                                                ? ReactionZRange
                                                                : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            RegionCheck,
                                                            Aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion
    }
}
