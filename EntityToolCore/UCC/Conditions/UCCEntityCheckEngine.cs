using Astral.Classes;
using Astral.Logic.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using EntityTools.UCC.Conditions;
using EntityTools.UCC.Extensions;
using MyNW.Classes;
using System;
using System.Text;
using EntityTools.Tools.Entities;

namespace EntityCore.UCC.Conditions
{
    public class UccEntityCheckEngine : IUccConditionEngine
    {
        #region Данные
        private UCCEntityCheck @this;

#if false
        private Predicate<Entity> checkEntity { get; set; } = null; 
#endif
        private Entity entity;
        private readonly Timeout timeout = new Timeout(0);
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


            if (entityKey.Validate(entity))
            {
                switch (@this.PropertyType)
                {
                    case EntityPropertyType.Distance:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Location.Distance3DFromPlayer - @this.PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Location.Distance3DFromPlayer - @this.PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Location.Distance3DFromPlayer < @this.PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Location.Distance3DFromPlayer > @this.PropertyValue;
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this.PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this.PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Character.AttribsBasic.HealthPercent < @this.PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Character.AttribsBasic.HealthPercent > @this.PropertyValue;
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        switch (@this.Sign)
                        {
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Equal:
                                return Math.Abs(entity.Location.Z - @this.PropertyValue) <= 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.NotEqual:
                                return Math.Abs(entity.Location.Z - @this.PropertyValue) > 1;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Inferior:
                                return entity.Location.Z < @this.PropertyValue;
                            case Astral.Logic.UCC.Ressources.Enums.Sign.Superior:
                                return entity.Location.Z > @this.PropertyValue;
                        }
                        break;
                }
                return false;
            }
            // Если Entity не найдено, условие будет истино в единственном случае:
            return @this.PropertyType == EntityPropertyType.Distance
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
                StringBuilder sb = new StringBuilder("Found closest Entity");
                var propType = @this.PropertyType;
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
                sb.Append(" [").Append(@this.EntityID).Append(']').AppendLine();
                if (@this.PropertyType == EntityPropertyType.Distance)
                    sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                return sb.ToString();
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                _label = $"{@this.GetType().Name} [{@this.EntityID}]";
            }
            else _label = @this.GetType().Name;

            return _label;
        }
        #endregion

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ??
            (_key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType, @this.EntityNameType));

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
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this.HealthCheck,
                                                            @this.ReactionRange,
                                                            @this.ReactionZRange > 0 
                                                                ? @this.ReactionZRange 
                                                                : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                            @this.RegionCheck,
                                                            @this.Aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion
    }
}
