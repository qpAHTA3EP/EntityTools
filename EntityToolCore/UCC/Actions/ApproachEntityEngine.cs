using Astral.Classes;
using Astral.Logic.NW;
using EntityCore.Entities;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Logic.UCC.Classes;

namespace EntityCore.UCC.Actions
{
    internal class ApproachEntityEngine : IUccActionEngine
#if IEntityDescriptor
        , IEntityInfos  
#endif
    {
        #region Данные
        private ApproachEntity @this;

        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string _label = string.Empty;
        private string _idStr;
        #endregion

        internal ApproachEntityEngine(ApproachEntity ettApproach)
        {

            InternalRebase(ettApproach);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~ApproachEntityEngine()
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
                    case nameof(ApproachEntity.EntityID):
                        checkEntity = initialize_CheckEntity;
                        _label = string.Empty;
                        break;
                    case nameof(ApproachEntity.EntityIdType):
                        checkEntity = initialize_CheckEntity;
                        break;
                    case nameof(ApproachEntity.EntityNameType):
                        checkEntity = initialize_CheckEntity;
                        break;
                } 
#else
                _key = null;
                _specialCheck = null;
                _label = string.Empty;
#endif
                entity = null;
                timeout.ChangeTime(0);
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is ApproachEntity ettApproach)
            {
                if (InternalRebase(ettApproach))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(ApproachEntity) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(ApproachEntity ettApproach)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = ettApproach;
            @this.PropertyChanged += PropertyChanged;

            _key = null;
            _specialCheck = null;
            _label = string.Empty;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    var entityKey = EntityKey;
                    if (timeout.IsTimedOut)
                    {
#if false
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                                @this._healthCheck, @this._reactionRange, @this._reactionZRange,
                                                                                @this._regionCheck, null, @this._aura.Checker); 
#else
                        entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
#endif

                        timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
                    }

                    return entityKey.Validate(entity) && !(@this._healthCheck && entity.IsDead) && entity.CombatDistance > @this._entityRadius;
                }
                return false;
            }
        }

        public bool Run()
        {
            if (entity.Location.Distance3DFromPlayer >= @this._entityRadius)
                return Approach.EntityByDistance(entity, @this._entityRadius);
            return true;
        }

        public Entity UnitRef
        {
            get
            {
                var entityKey = EntityKey;
                if (entityKey.Validate(entity))
                    return entity;
                else
                {
                    if (!string.IsNullOrEmpty(@this._entityId))
                    {
#if false
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                                @this._healthCheck, @this._reactionRange,
                                                                                @this._reactionZRange,
                                                                                @this._regionCheck, null, @this._aura.Checker); 
#else
                        entity = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
#endif
                        return entity;
                    }
                }
                return null;
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (string.IsNullOrEmpty(@this._entityId))
                    _label = GetType().Name;
                else _label = $"{GetType().Name} [{@this._entityId}]"; 
            }
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
                                                            @this._reactionZRange,
                                                            @this._regionCheck, 
                                                            @this._aura.IsMatch);
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;
        #endregion

#else
        private bool ValidateTarget(Entity e)
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
#if false
            Predicate<Entity> predicate = EntityTools.Tools.Entities.EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
#else
            Predicate<Entity> predicate = EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
#endif
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{_idStr}: Fail to initialize the Comparer.");
#endif
            return false;
        } 
#endif

#if IEntityDescriptor
        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            //sb.Append("EntitySetType: ").AppendLine(@this._entitySetType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            sb.Append("Aura: ").AppendLine(@this._aura.ToString());
            sb.AppendLine();

            var entityKey = EntityKey;
            var entityCheck = SpecialCheck;

            // список всех Entity, удовлетворяющих условиям
            var entities = SearchCached.FindAllEntity(entityKey, entityCheck);


            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            entity = SearchCached.FindClosestEntity(entityKey, entityCheck);
            if (entity != null)
            {
                bool distOk = @this._reactionRange <= 0 || entity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !entity.IsDead;
                sb.Append("ClosestEntity: ").Append(entity.ToString());
                if (distOk && zOk && alive)
                    sb.AppendLine(" [MATCH]");
                else sb.AppendLine(" [MISMATCH]");
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").Append(entity.IsDead.ToString());
                if (alive)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").Append(entity.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
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
