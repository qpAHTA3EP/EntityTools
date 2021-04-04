using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityCore.Extensions;
using EntityTools;
using EntityTools.Enums;
using EntityTools.Extensions;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Conditions
{
    internal class EntityCountEngine : IQuesterConditionEngine
#if IEntityDescriptor
        , IEntityInfos  
#endif
    {
        private EntityCount @this = null;
        private LinkedList<Entity> entities = null;
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
                bool debugInfoEnabled = EntityTools.EntityTools.Config.Logger.QuesterConditions.DebugConditionEntityCount;
                string currentMethodName = debugInfoEnabled ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

                if (debugInfoEnabled)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Begins");
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (!string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    uint entCount = (entities is null) ? 0u: (uint)entities.Count;

                    if (debugInfoEnabled)
                    {
                        string debugMsg;
                        if (entities?.Count > 0)
                            debugMsg = string.Concat(currentMethodName, ": Search Entities (irrespectively CustomRegion). Total found: ", entCount);
                        else debugMsg = string.Concat(currentMethodName, ": Search Entities (irrespectively CustomRegion). Nothing found");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }

#if false
                    switch (@this._sign)
                    {
                        case Relation.Equal:
                            result = entCount == @this._value;
                            break;
                        case Relation.NotEqual:
                            result = entCount != @this._value;
                            break;
                        case Relation.Inferior:
                            result = entCount < @this._value;
                            break;
                        case Relation.Superior:
                            result = entCount > @this._value;
                            break;
                    } 
#else
                    result = countChecker(entities.Count);
#endif

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
                if (!string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty)
                {
                    entities = SearchCached.FindAllEntity(EntityKey, SpecialCheck);

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null && entities.Count > 0)
                    {
                        if (@this._customRegionNames.Count > 0)
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

                                switch (@this._entityNameType)
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
                                strBldr.Insert(0, $"Total {entCount} Entities [{@this._entityId}] are detected in CustomRegions({@this._customRegionNames}):");
                            if (@this.CustomRegionCheck == Presence.NotEquel)
                                strBldr.Insert(0, $"Total {entCount} Entities [{@this._entityId}] are detected out of CustomRegions({@this._customRegionNames}):");
                        }
                        else strBldr.AppendLine($"Total {entities.Count} Entities [{@this._entityId}] are detected");
                    }
                    else strBldr.AppendLine($"No Entity [{@this._entityId}] was found.");

                    if (EntityTools.EntityTools.Config.Logger.QuesterConditions.DebugConditionEntityCount)
                    {
                        string debugMsg = string.Concat(_idStr, '.', nameof(TestInfos), ':', strBldr.ToString());

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    return strBldr.ToString();
                }
                return $"Property '{nameof(@this._entityId)}' does not set !";
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
                if(string.IsNullOrEmpty(@this._entityId))
                    _label = $"{@this.GetType().Name} {@this._sign} {@this._value}";
                else _label = $"{@this.GetType().Name}({@this._entityId}) {@this._sign} {@this._value}";
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
                if(_specialCheck is null)
                {
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                            @this._reactionRange, @this._reactionZRange,
                                                            @this._regionCheck,
                                                            @this._customRegionNames,
                                                            @this._customRegionCheck == Presence.NotEquel);

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
            switch (@this._sign)
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
            return count < @this._value;
        }
        private bool Superior_Than_Value(int count)
        {
            return count > @this._value;
        }
        private bool Equal_To_Value(int count)
        {
            return count == @this._value;
        }
        private bool NotEqual_To_Value(int count)
        {
            return count != @this._value;
        }
        #endregion

#if IEntityDescriptor
        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append("EntityID: ").AppendLine(@this._entityId);
                sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
                sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
                sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
                sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
                sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
                sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
                if (@this._customRegionNames?.Count > 0)
                {
                    sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
                    for (int i = 1; i < @this._customRegionNames.Count; i++)
                        sb.Append(", ").Append(@this._customRegionNames[i]);
                    sb.AppendLine("}");
                }

                sb.AppendLine();

                var entityKey = EntityKey;
                var entityCheck = SpecialCheck;

                var entities = SearchCached.FindAllEntity(entityKey, entityCheck);

                // Количество Entity, удовлетворяющих условиям
                if (entities != null)
                    sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
                else sb.Append("Founded Entities: 0");
                sb.AppendLine();

                // Ближайшее Entity
                Entity closestEntity = SearchCached.FindClosestEntity(entityKey, entityCheck);

                if (closestEntity != null)
                {
                    bool distOk = @this._reactionRange <= 0 || closestEntity.Location.Distance3DFromPlayer < @this._reactionRange;
                    bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(closestEntity.Location) < @this._reactionZRange;
                    bool alive = !@this._healthCheck || !closestEntity.IsDead;
                    bool crOK = SpecialCheck(closestEntity);
                    sb.Append("ClosestEntity: ").Append(closestEntity.ToString());
                    if (distOk && zOk && alive && crOK)
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

                    var crList = @this._customRegionNames;
                    if (crList?.Count > 0)
                    {
                        int crNum = 0;
                        sb.Append("\tRegionCheck: {");
                        foreach (var cr in crList)
                            if (cr.Cover(closestEntity))
                            {
                                if (crNum > 0)
                                    sb.Append(", ");
                                sb.Append(cr.Name);
                            }
                        sb.Append("}");
                        if (crOK)
                            sb.AppendLine(" [OK]");
                        else sb.AppendLine(" [FAIL]");
                    }

                }
                else sb.AppendLine("Closest Entity not found!");
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.Message);
                ETLogger.WriteLine(LogType.Error, sb.ToString());
                throw;
            }

            infoString = sb.ToString();
            return true;
        } 
#endif
    }
}
