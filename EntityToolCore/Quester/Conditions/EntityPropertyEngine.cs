using System;
using System.Collections.Generic;
using System.Text;
using Astral.Classes;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Enums;
using EntityCore.Extensions;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using static Astral.Quester.Classes.Condition;
using EntityTools;
using EntityTools.Core.Interfaces;

namespace EntityCore.Quester.Conditions
{
    internal class EntityPropertyEngine : IQuesterConditionEngine
#if IEntityDescriptor
        , IEntityInfos  
#endif
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
                    //if (!string.IsNullOrEmpty(@this._entityId))
                        entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);
                    isValid = entity != null;
                    timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                if (isValid)
                {
#if true
                   return propertyValueChecker(entity);
#else
                    switch (@this.PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Location.Distance3DFromPlayer < @this._value;
                                case Relation.Superior:
                                    return entity.Location.Distance3DFromPlayer > @this._value;
                            }
                            break;
                        case EntityPropertyType.HealthPercent:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Character.AttribsBasic.HealthPercent < @this._value;
                                case Relation.Superior:
                                    return entity.Character.AttribsBasic.HealthPercent > @this._value;
                            }
                            break;
                        case EntityPropertyType.ZAxis:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Location.Z - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Location.Z - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Location.Z < @this._value;
                                case Relation.Superior:
                                    return entity.Location.Z > @this._value;
                            }
                            break;
                    } 
#endif
                }
                else return @this._propertyType == EntityPropertyType.Distance && @this._sign == Relation.Superior;
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
                        @this._entityNameType == EntityNameType.NameUntranslated ? entity.NameUntranslated : entity.InternalName,
                        "] which ", @this.PropertyType, " = ",
                        @this.PropertyType == EntityPropertyType.Distance ? entity.Location.Distance3DFromPlayer.ToString("N2") :
                        @this.PropertyType == EntityPropertyType.ZAxis ? entity.Location.Z .ToString("N2") : entity.Character.AttribsBasic.HealthPercent.ToString("N2"));
                }
                else
                {
                    return string.Concat("No one Entity matched to [", @this._entityId, ']', Environment.NewLine,
                        @this.PropertyType == EntityPropertyType.Distance ? "The distance to the missing Entity is considered equal to infinity." : string.Empty);
                }
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
                _label = $"Entity [{@this._entityId}] {@this._propertyType} {@this._sign} to {@this._value}";
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
                    _key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete);
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
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                            @this._reactionRange, @this._reactionZRange,
                                                            @this._regionCheck,
                                                            @this._customRegionNames);
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
            return  entity.Location.Distance3DFromPlayer < @this._value;
        }
        private bool Distance_Superior(Entity e)
        {
            return entity.Location.Distance3DFromPlayer > @this._value;
        }
        private bool Distance_Equal(Entity e)
        {
            return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) < 0.1;
        }
        private bool Distance_NotEqual(Entity e)
        {
            return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) >= 0.1;
        }

        private bool HealthPercent_Inferior(Entity e)
        {
            return entity.Character.AttribsBasic.HealthPercent < @this._value; ;
        }
        private bool HealthPercent_Superior(Entity e)
        {
            return entity.Character.AttribsBasic.HealthPercent > @this._value; ;
        }
        private bool HealthPercent_Equal(Entity e)
        {
            return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) < 0.1;
        }
        private bool HealthPercent_NotEqual(Entity e)
        {
            return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) >= 0.1;
        }

        private bool ZAxis_Inferior(Entity e)
        {
            return entity.Z < @this._value; ;
        }
        private bool ZAxis_Superior(Entity e)
        {
            return entity.Z > @this._value; ;
        }
        private bool ZAxis_Equal(Entity e)
        {
            return Math.Abs(entity.Z - @this._value) < 0.1;
        }
        private bool ZAxis_NotEqual(Entity e)
        {
            return Math.Abs(entity.Z - @this._value) >= 0.1;
        }
        #endregion

#if IEntityDescriptor
        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this.EntityID);
            sb.Append("EntityIdType: ").AppendLine(@this.EntityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this.EntityNameType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this.HealthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this.ReactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this.ReactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this.RegionCheck.ToString());
            if (@this.CustomRegionNames != null && @this.CustomRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this.CustomRegionNames[0]);
                for (int i = 1; i < @this.CustomRegionNames.Count; i++)
                    sb.Append(", ").Append(@this.CustomRegionNames[i]);
                sb.AppendLine("}");
            }

            sb.AppendLine();

            var entityKey = EntityKey;
            var entityCheck = SpecialCheck;
            LinkedList<Entity> entities = SearchCached.FindAllEntity(entityKey, entityCheck);

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity
            Entity closestEntity = SearchCached.FindClosestEntity(entityKey, entityCheck);

            if (closestEntity != null && closestEntity.IsValid)
            {
                bool distOk = @this._reactionRange <= 0 || closestEntity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(closestEntity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !closestEntity.IsDead;
                sb.Append("ClosestEntity: ").Append(closestEntity.ToString());
                if (distOk && zOk && alive)
                    sb.AppendLine(" [MATCH]");
                else sb.AppendLine(" [MISMATCH]");
                sb.Append("\tName: ").AppendLine(closestEntity.Name);
                sb.Append("\tInternalName: ").AppendLine(closestEntity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(closestEntity.NameUntranslated);
                sb.Append("\tIsDead: ").Append(closestEntity.IsDead.ToString());
                if (alive)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(closestEntity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(closestEntity.Location.ToString());
                sb.Append("\tDistance: ").Append(closestEntity.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(closestEntity.Location).ToString());
                if (zOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        } 
#endif
    }
}
