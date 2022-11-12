using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityTools.Annotations;
using EntityTools.Core.Interfaces;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using EntityTools.Tools;
using EntityTools.Tools.Entities;

using MyNW.Classes;

using Timeout = Astral.Classes.Timeout;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ApproachEntity : UCCAction, INotifyPropertyChanged, IEntityDescriptor
    {
        #region Опции команды
        #region Entity
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
                    OnPropertyChanged();
                }
            }
        }
        private string _entityId = string.Empty;

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
                    OnPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _entityIdType = ItemFilterStringType.Simple;

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
                    OnPropertyChanged();
                }
            }
        }
        private EntityNameType _entityNameType = EntityNameType.InternalName;

#if DEVELOPER
        [Category("Entity")]
#else
        [Browsable(false)]
#endif
        public float EntityRadius
        {
            get => _entityRadius; set
            {
                if (Math.Abs(_entityRadius - value) > 0.1)
                {
                    _entityRadius = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _entityRadius = 12;

#if DEVELOPER
        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Test the Entity searching.")]
        [Category("Entity")]
        public string EntityTestInfo => "Push button '...' =>";
#endif
        #endregion

        #region Entity Options
#if DEVELOPER
        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
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
                    OnPropertyChanged();
                }
            }
        }
        private bool _regionCheck = true;

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
                if (_healthCheck != value)
                {
                    _healthCheck = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _healthCheck = true;

#if DEVELOPER
        [Description("Aura which checked on the Entity")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
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
                    OnPropertyChanged();
                }
            }
        }
        private AuraOption _aura = new AuraOption();
        public bool ShouldSerializeAura() => !string.IsNullOrEmpty(_aura?.AuraName);

#if DEVELOPER
        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public float ReactionRange
        {
            get => _reactionRange; set
            {
                if (Math.Abs(_reactionRange - value) > 0.1)
                {
                    _reactionRange = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _reactionRange = 30;

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
                if (Math.Abs(_reactionZRange - value) > 0.1)
                {
                    _reactionZRange = value;
                    OnPropertyChanged();
                }
            }
        }
        private float _reactionZRange; 
        #endregion

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new int Range { get; set; } = 0;

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; } = string.Empty;
        #endregion
        #endregion
        



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            _key = null;
            _specialCheck = null;
            _label = string.Empty;
            
            entityCache = null;
            timeout.ChangeTime(0);
        }
        #endregion

        public override UCCAction Clone()
        {
            return BaseClone(new ApproachEntity
            {
                _entityId = _entityId,
                _entityIdType = _entityIdType,
                _entityNameType = _entityNameType,
                _regionCheck = _regionCheck,
                _healthCheck = _healthCheck,
                _reactionRange = _reactionRange,
                _reactionZRange = _reactionZRange,
                _entityRadius = _entityRadius,
                _aura = new AuraOption
                {
                    AuraName = _aura.AuraName,
                    AuraNameType = _aura.AuraNameType,
                    Sign = _aura.Sign,
                    Stacks = _aura.Stacks
                }
            });
        }

        #region Данные
        private Entity entityCache;
        private Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        #endregion
        

        
        #region IUCCActionEngine
        public override bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    var entityKey = EntityKey;
                    if (timeout.IsTimedOut)
                    {
                        entityCache = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

                        timeout.ChangeTime(EntityTools.Config.EntityCache.CombatCacheTime);

                        return entityCache != null && entityCache.CombatDistance > EntityRadius;
                    }

                    return entityKey.Validate(entityCache) && !(HealthCheck && entityCache.IsDead) && entityCache.CombatDistance > EntityRadius;
                }
                return false;
            }
        }

        public override bool Run()
        {
            if (entityCache.Location.Distance3DFromPlayer >= EntityRadius)
                return Approach.EntityByDistance(entityCache, EntityRadius);
            return true;
        }

        public Entity UnitRef
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityID))
                {
                    var entityKey = EntityKey;
                    if (entityKey.Validate(entityCache))
                        return entityCache;
                    if (timeout.IsTimedOut)
                    {
                        entityCache = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
                        timeout.ChangeTime(EntityTools.Config.EntityCache.CombatCacheTime);
                        return entityCache;
                    }
                }
                return null;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var entId = EntityID;
                _label = string.IsNullOrEmpty(entId) 
                       ? GetType().Name 
                       : $"{GetType().Name} [{entId}]";
            }
            return _label;
        }
        #endregion


        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey => _key ?? (_key = new EntityCacheRecordKey(EntityID, EntityIdType, EntityNameType));
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// Использовать самомодифицирующийся предикат нельзя, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(HealthCheck,
                                                            ReactionZRange,
                                                            ReactionZRange > 0 ? ReactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            RegionCheck,
                                                            Aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion
    }
}
