using Astral.Classes;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Quester.Actions;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;

namespace EntityCore.UCC.Actions
{
    internal class ApproachEntityEngine : IUccActionEngine
#if IEntityDescriptor
        , IEntityInfos  
#endif
    {
        #region Данные
        private ApproachEntity @this;

        private Entity entityCache;
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
                _key = null;
                _specialCheck = null;
                _label = string.Empty;
                entityCache = null;
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
                        entityCache = SearchCached.FindClosestEntity(entityKey, SpecialCheck);

                        timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);

                        return entityCache != null && entityCache.CombatDistance > @this._entityRadius;
                    }

                    return entityKey.Validate(entityCache) && !(@this._healthCheck && entityCache.IsDead) && entityCache.CombatDistance > @this._entityRadius;
                }
                return false;
            }
        }

        public bool Run()
        {
            if (entityCache.Location.Distance3DFromPlayer >= @this._entityRadius)
                return Approach.EntityByDistance(entityCache, @this._entityRadius);
            return true;
        }

        public Entity UnitRef
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    var entityKey = EntityKey;
                    if (entityKey.Validate(entityCache))
                        return entityCache;
                    if (timeout.IsTimedOut)
                    {
                        entityCache = SearchCached.FindClosestEntity(entityKey, SpecialCheck);
                        timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.CombatCacheTime);
                        return entityCache;
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
        /// Использовать самомодифицирующийся предикат нельзя, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
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
    }
}
