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
using EntityTools;

namespace EntityCore.UCC.Conditions
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
            if (@this._match == MatchType.Match)
                return match;
            else return !match;
#endif
        }

        public string TestInfos(UCCAction refAction)
        {
            Entity target = refAction?.GetTarget();

            StringBuilder sb = new StringBuilder("Target ");
            if (target != null)
            {
                if (@this._entityNameType == EntityNameType.InternalName)
                    sb.Append('[').Append(target.InternalName).Append(']');
                else sb.Append('[').Append(target.NameUntranslated).Append(']');
                if (EntityKey.IsMatch(target))
                    sb.Append(" match");
                else sb.Append(" does not match");
                sb.Append(" EntityID [").Append(@this._entityId).Append(']');
            }
            else sb.Append("is NULL");

            return sb.ToString();
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} [{@this._entityId}]";
            else label = @this.GetType().Name;

            return label;
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
        #endregion
#else
        private bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity(e);
        }

        private bool initialize_CheckEntity(Entity e)
        {
            Predicate<Entity> predicate = EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Fail to initialize the Comparer.");
#endif
            return false;
        } 
#endif
    }
}
