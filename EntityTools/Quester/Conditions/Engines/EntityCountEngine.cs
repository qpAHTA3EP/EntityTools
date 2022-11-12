using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Conditions.Engines
{
    internal class EntityCountEngine : IQuesterConditionEngine
    {
        private EntityCount @this;
        private LinkedList<Entity> entities;
        private string _label = string.Empty;

        /// <summary>
        /// Префикс, идентифицирующий принадлежность отладочной информации, выводимой в Лог
        /// </summary>
        private string _idStr = string.Empty;

        internal EntityCountEngine(EntityCount ettc)
        {
            InternalRebase(ettc);

            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~EntityCountEngine()
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

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                _key = null;
                _label = string.Empty;
                _specialCheck = null;
                countChecker = Initicalize_CountChecker;

                entities?.Clear();
            }
        }

        public bool Rebase(Condition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is EntityCount ettCount)
            {
                if (InternalRebase(ettCount))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(EntityCount) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(EntityCount ettCount)
        {
            entities?.Clear();

            // Убираем привязку к старому условию
            @this?.Unbind();

            @this = ettCount;
            @this.PropertyChanged += PropertyChanged;

            _key = null;
            _label = string.Empty;
            _specialCheck = null;

            countChecker = Initicalize_CountChecker;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Bind(this);

            return true;
        }

        public bool IsValid
        {
            get
            {
                bool result = false;
                bool debugInfoEnabled = ExtendedDebugInfo;
                string currentMethodName = debugInfoEnabled ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(IsValid)) : string.Empty;

                if (debugInfoEnabled)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Begins");
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (!string.IsNullOrEmpty(@this.EntityID) || @this.EntityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    int entCount = entities is null 
                        ? 0 
                        : entities.Count;

                    if (debugInfoEnabled)
                    {
                        string debugMsg;
                        if (entCount > 0)
                            debugMsg = string.Concat(currentMethodName, ": Search Entities (irrespectively CustomRegion). Total found: ", entCount);
                        else debugMsg = string.Concat(currentMethodName, ": Search Entities (irrespectively CustomRegion). Nothing found");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }

                    result = countChecker(entCount);

                    if (debugInfoEnabled)
                    {
                        string debugMsg = string.Concat(currentMethodName, ": Result=", result, " (", entCount, " entities mutched)");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                }
                return result;
            }
        }

        public void Reset() => entities?.Clear();

        public string TestInfos
        {
            get
            {
                var enttId = @this.EntityID;
                if (!string.IsNullOrEmpty(enttId) || @this.EntityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null && entities.Count > 0)
                    {
                        var crList = @this.CustomRegionNames;
                        if (crList.Count > 0)
                        {
                            strBldr.AppendLine();
                            foreach (Entity entity in entities)
                            {
                                StringBuilder strBldr2 = new StringBuilder();
                                bool match = false;

                                foreach (CustomRegion customRegion in Astral.Quester.API.CurrentProfile.CustomRegions)
                                {
                                    if (entity.Within(customRegion))
                                    {
                                        match = true;
                                        if (strBldr2.Length > 0)
                                            strBldr2.Append(", ");
                                        strBldr2.Append($"[{customRegion.Name}]");
                                    }
                                }

                                if (@this.CustomRegionCheck == Presence.Equal && match)
                                    entCount++;
                                if (@this.CustomRegionCheck == Presence.NotEquel && !match)
                                    entCount++;

                                switch (@this.EntityNameType)
                                {
                                    case EntityNameType.InternalName:
                                        strBldr.Append($"\t[{entity.InternalName}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.NameUntranslated:
                                        strBldr.Append($"\t[{entity.NameUntranslated}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.Empty:
                                        strBldr.Append($"\t[{entity.Name}] is in CustomRegions: ");
                                        break;
                                }
                                strBldr.Append(strBldr2).AppendLine();
                            }

                            if (@this.CustomRegionCheck == Presence.Equal)
                                strBldr.Insert(0, $"Total {entCount} Entities [{enttId}] are detected in CustomRegions({crList}):");
                            if (@this.CustomRegionCheck == Presence.NotEquel)
                                strBldr.Insert(0, $"Total {entCount} Entities [{enttId}] are detected out of CustomRegions({crList}):");
                        }
                        else strBldr.AppendLine($"Total {entities.Count} Entities [{enttId}] are detected");
                    }
                    else strBldr.AppendLine($"No Entity [{enttId}] was found.");

                    if (ExtendedDebugInfo)
                    {
                        string debugMsg = string.Concat(_idStr, '.', nameof(TestInfos), ':', strBldr.ToString());

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    return strBldr.ToString();
                }
                return $"Property '{nameof(enttId)}' does not set !";
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var entId = @this.EntityID;
                _label = string.IsNullOrEmpty(entId)
                    ? $"{@this.GetType().Name} {@this.Sign} {@this.Value}"
                    : $"{@this.GetType().Name}({entId}) {@this.Sign} {@this.Value}";
            }
            return _label;
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType,
                                                     @this.EntityNameType, EntitySetType.Complete));

        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="EntityCount.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if(_specialCheck is null)
                {
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this.HealthCheck,
                                                            @this.ReactionZRange, @this.ReactionZRange,
                                                            @this.RegionCheck,
                                                            @this.CustomRegionNames,
                                                            @this.CustomRegionCheck == Presence.NotEquel);

                }
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;

        /// <summary>
        /// Предикат, сравнивающий количество <seealso cref="Entity"/>, удовлетворяющих уcловиям <seealso cref="EntityCount"/>
        /// c референтным значеним <seealso cref="EntityCount.Value"/>
        /// </summary>
        Predicate<int> countChecker;

        private bool Initicalize_CountChecker(int count)
        {
            switch (@this.Sign)
            {
                case Relation.Inferior:
                    countChecker = Inferior_Than_Value;
                    break;
                case Relation.Superior:
                    countChecker = Superior_Than_Value;
                    break;
                case Relation.Equal:
                    countChecker = Equal_To_Value;
                    break;
                case Relation.NotEqual:
                    countChecker = NotEqual_To_Value;
                    break;
            }
            return countChecker(count);
        }
        private bool Inferior_Than_Value(int count)
        {
            return count < @this.Value;
        }
        private bool Superior_Than_Value(int count)
        {
            return count > @this.Value;
        }
        private bool Equal_To_Value(int count)
        {
            return count == @this.Value;
        }
        private bool NotEqual_To_Value(int count)
        {
            return count != @this.Value;
        }
        #endregion

        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = global::EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterConditions.DebugEntityCount && logConf.Active;
            }
        }
    }
}
