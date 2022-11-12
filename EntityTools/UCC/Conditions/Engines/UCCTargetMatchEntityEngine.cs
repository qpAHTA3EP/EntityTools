using System;
using System.Text;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools.Entities;
using EntityTools.UCC.Extensions;
using MyNW.Classes;

namespace EntityTools.UCC.Conditions.Engines
{
    public class UccTargetMatchEntityEngine : IUccConditionEngine
    {
        #region Данные
        private UCCTargetMatchEntity @this;

#if false
        private Predicate<Entity> checkEntity = null; 
#endif

        private string label = string.Empty;
        private string _idStr;
        #endregion

        internal UccTargetMatchEntityEngine(UCCTargetMatchEntity tarMatch)
        {
            InternalRebase(tarMatch);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~UccTargetMatchEntityEngine()
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
                        label = string.Empty;
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
#endif
            }
        }

        public bool Rebase(UCCCondition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is UCCTargetMatchEntity tarMatch)
            {
                if (InternalRebase(tarMatch))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(UCCTargetMatchEntity) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(UCCTargetMatchEntity tarMatch)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;//new EntityTools.Core.Proxies.UccConditionProxy(@this);
            }

            @this = tarMatch;
            @this.PropertyChanged += PropertyChanged;

            _key = null;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region Вспомогательные методы
        public bool IsOK(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

#if false
            switch (@this._match)
            {
                case MatchType.Match:
                    return ValidateEntity(target);
                case MatchType.Mismatch:
                    return !ValidateEntity(target);
            } 
            return false;
#else
            bool match = EntityKey.IsMatch(target);
            if (@this.Match == MatchType.Match)
                return match;
            return !match;
#endif
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            StringBuilder sb = new StringBuilder("Target ");
            if (target != null)
            {
                if (@this.EntityNameType == EntityNameType.InternalName)
                    sb.Append('[').Append(target.InternalName).Append(']');
                else sb.Append('[').Append(target.NameUntranslated).Append(']');
                if (EntityKey.IsMatch(target))
                    sb.Append(" match");
                else sb.Append(" does not match");
                sb.Append(" EntityID [").Append(@this.EntityID).Append(']');
            }
            else sb.Append("is NULL");

            return sb.ToString();
        }

        public string Label()
        {
            label = string.IsNullOrEmpty(label) 
                        ? $"{@this.GetType().Name} [{@this.EntityID}]" 
                        : @this.GetType().Name;

            return label;
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
                    _key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;
        #endregion
    }
}
