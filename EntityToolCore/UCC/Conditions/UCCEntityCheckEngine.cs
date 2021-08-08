using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EntityTools.UCC.Extensions;
using Astral.Classes;
using EntityTools;

namespace EntityCore.UCC.Conditions
{
    public class UccEntityCheckEngine : IUccConditionEngine
    {
        #region Данные
        private UCCEntityCheck @this;

#if false
        private Predicate<Entity> checkEntity { get; set; } = null; 
#endif
        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        private string _idStr;
        #endregion

        internal UccEntityCheckEngine(UCCEntityCheck ettCheck)
        {
            InternalRebase(ettCheck);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~UccEntityCheckEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
#if false
                switch (e.PropertyName)
                {
                    case nameof(@this.EntityID):
                        checkEntity = initialize_CheckEntity;
                        _label = string.Empty;
                        break;
                    case nameof(@this.EntityIdType):
                        checkEntity = initialize_CheckEntity;
                        break;
                    case nameof(@this.EntityNameType):
                        checkEntity = initialize_CheckEntity;
                        break;
                } 
#else
                _key = null;
                _specialCheck = null;
                _label = string.Empty;
#endif
                entity = null;
            }
        }

        public bool Rebase(UCCCondition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is UCCEntityCheck execPower)
            {
                if (InternalRebase(execPower))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(UCCEntityCheck) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(UCCEntityCheck execPower)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

            _key = null;
            _specialCheck = null;
            _label = string.Empty;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region MyRegion
        public bool IsOK(UCCAction refAction)
        {
            Entity targetEntity = refAction?.GetTarget();

            var entityKey = EntityKey;

            if (entityKey.Validate(targetEntity))
                entity = targetEntity;
            else if (timeout.IsTimedOut)
            {
                entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
                timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime/2);
            }


            bool result = false;
            if (entityKey.Validate(entity))
            {
                switch (@this._propertyType)
                {
                    case EntityPropertyType.Distance:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Location.Distance3DFromPlayer == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Location.Distance3DFromPlayer != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Location.Distance3DFromPlayer < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Location.Distance3DFromPlayer > @this._propertyValue;
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Character?.AttribsBasic?.HealthPercent == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Character?.AttribsBasic?.HealthPercent != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Character?.AttribsBasic?.HealthPercent < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Character?.AttribsBasic?.HealthPercent > @this._propertyValue;
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return result = entity.Location.Z == @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return result = entity.Location.Z != @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return result = entity.Location.Z < @this._propertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return result = entity.Location.Z > @this._propertyValue;
                        }
                        break;
                }
                return result;
            }
            // Если Entity не найдено, условие будет истино в единственном случае:
            else return @this._propertyType == EntityPropertyType.Distance
                        && @this.Sign == Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity closestEntity = refAction?.GetTarget();

            var entityKey = EntityKey;

            if (!entityKey.Validate(closestEntity))
                closestEntity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

            if (entityKey.Validate(closestEntity))
            {
                StringBuilder sb = new StringBuilder("Found closect Entity");
                sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(@this._propertyType).Append(" = ");
                switch (@this._propertyType)
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
                sb.Append(" [").Append(@this._entityId).Append(']').AppendLine();
                if (@this._propertyType == EntityPropertyType.Distance)
                    sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                return sb.ToString();
                //return string.Concat("No one Entity matched to [", @this._entityId, ']',Environment.NewLine, (@this._propertyType == EntityPropertyType.Distance)? "The distance to the missing Entity is considered equal to infinity." : string.Empty, "The distance to the missing Entity is considered equal to infinity.");
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = $"{@this.GetType().Name} [{@this._entityId}]";
            }
            else _label = @this.GetType().Name;

            return _label;
        }
        #endregion

#if true
        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType, @this._entityNameType);
                return _key;
            }
        }
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
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                            @this._reactionRange,
                                                            //@this._reactionZRange,
                                                            @this._reactionZRange > 0 ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            @this._regionCheck,
                                                            @this._aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion
#else
        private bool ValidateEntity(Entity e)
        {
#if false
            return e != null && e.IsValid && checkEntity(e); 
#else
            return e != null && e.IsValid
                    && (e.Character.IsValid || e.Critter.IsValid || e.Player.IsValid)
                    && checkEntity(e);
#endif
        }

        private bool initialize_CheckEntity(Entity e)
        {
            Predicate<Entity> predicate = EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{_idStr}: Fail to initialize the Comparer.");
#endif
            return false;
        } 
#endif
    }
}
