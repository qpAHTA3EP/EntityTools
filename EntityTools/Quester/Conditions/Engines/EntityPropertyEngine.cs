using System;
using Astral.Classes;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Conditions.Engines
{
    internal class EntityPropertyEngine : IQuesterConditionEngine
    {
        private EntityProperty @this = null;

        #region Данные ядра
        private Entity entity = null;
        private Timeout timeout = new Timeout(0);

        private string _label = string.Empty; 
        private string _idStr;
        #endregion

        internal EntityPropertyEngine(EntityProperty ettProp)
        {
            InternalRebase(ettProp);

            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~EntityPropertyEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.Unbind();
                @this = null;
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                _key = null;
                _label = string.Empty;
                _specialCheck = null;
                propertyValueChecker = Intialize_PropertyValueChecker;

                entity = null;
                timeout.ChangeTime(0);
            }
        }

        public bool Rebase(Condition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is EntityProperty ettProp)
            {
                InternalRebase(ettProp);
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                return true;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(EntityProperty) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(EntityProperty ettProp)
        {
            // Убираем привязку к старому условию
            @this?.Unbind();

            @this = ettProp;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            _key = null;
            _label = string.Empty;
            _specialCheck = null;
            propertyValueChecker = Intialize_PropertyValueChecker;

            @this.Bind(this);

            return true;
        }

        public bool IsValid
        {
            get
            {
                var entityKey = EntityKey;
                bool isValid = entityKey.Validate(entity);
                if (timeout.IsTimedOut || !isValid)
                {
                    entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);
                    isValid = entity != null;
                    timeout.ChangeTime(global::EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                if (isValid)
                {
                   return propertyValueChecker(entity);
                }

                return @this.PropertyType == EntityPropertyType.Distance && @this.Sign == Relation.Superior;
            }
        }

        public void Reset() { }

        public string TestInfos
        {
            get
            {
                entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);

                if (EntityKey.Validate(entity))
                {
                    return string.Concat("Found closest Entity [",
                        @this.EntityNameType == EntityNameType.NameUntranslated ? entity.NameUntranslated : entity.InternalName,
                        "] which ", @this.PropertyType, " = ",
                        @this.PropertyType == EntityPropertyType.Distance ? entity.Location.Distance3DFromPlayer.ToString("N2") :
                        @this.PropertyType == EntityPropertyType.ZAxis ? entity.Location.Z .ToString("N2") : entity.Character.AttribsBasic.HealthPercent.ToString("N2"));
                }
                else
                {
                    return string.Concat("No one Entity matched to [", @this.EntityID, ']', Environment.NewLine,
                        @this.PropertyType == EntityPropertyType.Distance ? "The distance to the missing Entity is considered equal to infinity." : string.Empty);
                }
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
                _label = $"Entity [{@this.EntityID}] {@this.PropertyType} {@this.Sign} to {@this.Value}";
            return _label;
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType, @this.EntityNameType, EntitySetType.Complete);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this.HealthCheck,
                                                            @this.ReactionRange, @this.ReactionZRange,
                                                            @this.RegionCheck,
                                                            @this.CustomRegionNames);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion

        #region EntityPropertyChecker
        /// <summary>
        /// Предикат, проверяющий истинность соотношения <seealso cref="EntityProperty.Sign"/> 
        /// между величиной атрибута, заданного <seealso cref="EntityProperty.PropertyType"/>, 
        /// и референтным значением <seealso cref="EntityProperty.Value"/>
        /// </summary>
        Predicate<Entity> propertyValueChecker;
        private bool Intialize_PropertyValueChecker(Entity e)
        {
            if (e is null)
                return false;

            switch (@this.PropertyType)
            {
                case EntityPropertyType.Distance:
                    switch (@this.Sign)
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
                    switch (@this.Sign)
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
                    switch (@this.Sign)
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

        private bool Distance_Inferior(Entity e)
        {
            return  entity.Location.Distance3DFromPlayer < @this.Value;
        }
        private bool Distance_Superior(Entity e)
        {
            return entity.Location.Distance3DFromPlayer > @this.Value;
        }
        private bool Distance_Equal(Entity e)
        {
            return Math.Abs(entity.Location.Distance3DFromPlayer - @this.Value) <= 1;
        }
        private bool Distance_NotEqual(Entity e)
        {
            return Math.Abs(entity.Location.Distance3DFromPlayer - @this.Value) > 1;
        }

        private bool HealthPercent_Inferior(Entity e)
        {
            return entity.Character.AttribsBasic.HealthPercent < @this.Value;
        }
        private bool HealthPercent_Superior(Entity e)
        {
            return entity.Character.AttribsBasic.HealthPercent > @this.Value;
        }
        private bool HealthPercent_Equal(Entity e)
        {
            return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this.Value) <= 1;
        }
        private bool HealthPercent_NotEqual(Entity e)
        {
            return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this.Value) > 1;
        }

        private bool ZAxis_Inferior(Entity e)
        {
            return entity.Z < @this.Value; 
        }
        private bool ZAxis_Superior(Entity e)
        {
            return entity.Z > @this.Value;
        }
        private bool ZAxis_Equal(Entity e)
        {
            return Math.Abs(entity.Z - @this.Value) <= 1;
        }
        private bool ZAxis_NotEqual(Entity e)
        {
            return Math.Abs(entity.Z - @this.Value) > 1;
        }
        #endregion
    }
}
