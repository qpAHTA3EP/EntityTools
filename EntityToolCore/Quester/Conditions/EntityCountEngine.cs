using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityCore.Extensions;
using EntityTools;
using EntityTools.Enums;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using static Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Conditions
{
    internal class EntityCountEngine : IEntityInfos
#if CORE_INTERFACES
        , IQuesterConditionEngine
#endif
    {
        private EntityCount @this = null;

        private Predicate<Entity> checkEntity = null;
        private Func<List<CustomRegion>> getCustomRegions = null;
        private Predicate<Entity> customRegionCheck = null;

        private List<CustomRegion> customRegions = null;
        private LinkedList<Entity> entities = null;
#if timeout
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0); 
#endif
        private string label = string.Empty;

        /// <summary>
        /// Префикс, идентифицирующий принадлежность отладочной информации, выводимой в Лог
        /// </summary>
        private string conditionIDstr = string.Empty;

        internal EntityCountEngine(EntityCount ettc)
        {
            @this = ettc;

#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;
            customRegionCheck = internal_CustomRegionCheck_Initializer;

            conditionIDstr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr} initialized: {Label()}");
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.Sign) || e.PropertyName == nameof(@this.Value))
                    label = string.Empty;
                else if (e.PropertyName == nameof(@this.EntityID)
                        || e.PropertyName == nameof(@this.EntityIdType)
                        || e.PropertyName == nameof(@this.EntityNameType))
                    checkEntity = internal_CheckEntity_Initializer;
                else if (e.PropertyName == nameof(@this.CustomRegionNames))
                {
                    getCustomRegions = internal_GetCustomRegion_Initializer;
                    customRegionCheck = internal_CustomRegionCheck_Initializer;
                }
                else if (e.PropertyName == nameof(@this.Tested))
                {
                    customRegionCheck = internal_CustomRegionCheck_Initializer;
                }

                entities?.Clear();
            }
        }

#if CORE_INTERFACES
        public bool IsValid
        {
            get
            {
                bool result = false;
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
#if timeout
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Begins. Timeout left:", timeout.Left);
#else
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Begins");
#endif
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (!string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty)
                {
#if timeout
                    if (timeout.IsTimedOut)
                    { 
#endif
                    entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                       @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, null/*getCustomRegions()*/, customRegionCheck);

                    uint entCount = (entities is null) ? 0u: (uint)entities.Count;

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg;
                        if (entities?.Count > 0)
                            debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Search Entities (irrespectively CustomRegion). Total found: ", entCount);
                        else debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Search Entities (irrespectively CustomRegion). Nothing found");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
#if timeout
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }
                else if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Total entities cached (irrespectively CustomRegion): ", entities.Count);

                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                } 
#endif

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

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Result=", result, " (", entCount, " entities mutched)");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                }
                return result;
            }
#if disabled_20200709_1325
            get
            {
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
#if timeout
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Begins. Timeout left:", timeout.Left);
#else
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Begins");
#endif
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (!string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty)
                {
#if timeout
                    if (timeout.IsTimedOut)
                    { 
#endif
                    entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                       @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck/*, getCustomRegions()*/);
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg;
                        if (entities is null || entities.Count == 0)
                            debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Search Entities (irrespectively CustomRegion). Total found: ", entities.Count);
                        else debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Search Entities (irrespectively CustomRegion). Nothing found");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
#if timeout
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }
                else if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Total entities cached (irrespectively CustomRegion): ", entities.Count);

                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                } 
#endif

                    uint entCount = 0;

                    if (entities != null && entities.Count > 0)
                    {
                        List<CustomRegion> crList = getCustomRegions();
                        if (crList != null && crList.Count > 0)
                            foreach (Entity entity in entities)
                            {
                                bool match = false;
                                foreach (CustomRegion cr in crList)
                                {
                                    if (entity.Within(cr))
                                    {
                                        match = true;
                                        break;
                                    }
                                }

                                if (@this.Tested == Presence.Equal && match)
                                    entCount++;
                                if (@this.Tested == Presence.NotEquel && !match)
                                    entCount++;
                            }
                        else entCount = (uint)entities.Count;
                    }

                    bool result = false;
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

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(conditionIDstr, '.', nameof(IsValid), ": Result=", result, " (", entCount, " entities mutched)");

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                }
                return false;
            } 
#endif
        }

        public void Reset() => entities?.Clear();

        public string TestInfos
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty)
                {
#if false
                    if (entities is null || entities.Count == 0)
#elif timeout
                    if (timeout.IsTimedOut)
#endif
                    {
                        entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                           @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, null /*getCustomRegions()*/, customRegionCheck);
                    }

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null && entities.Count > 0)
                    {
                        List<CustomRegion> crList = getCustomRegions();
                        if (crList != null && crList.Count > 0)
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

                                if (@this.Tested == Presence.Equal && match)
                                    entCount++;
                                if (@this.Tested == Presence.NotEquel && !match)
                                    entCount++;

                                switch (@this._entityNameType)
                                {
                                    case EntityNameType.InternalName:
                                        strBldr.Append($"[{entity.InternalName}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.NameUntranslated:
                                        strBldr.Append($"[{entity.NameUntranslated}] is in CustomRegions: ");
                                        break;
                                    case EntityNameType.Empty:
                                        strBldr.Append($"[{entity.Name}] is in CustomRegions: ");
                                        break;
                                }
                                strBldr.Append(strBldr2).AppendLine();
                            }

                            if (@this.Tested == Presence.Equal)
                                strBldr.Insert(0, $"Total {entCount} Entities [{@this._entityId}] are detected in {crList.Count} CustomRegion:");
                            if (@this.Tested == Presence.NotEquel)
                                strBldr.Insert(0, $"Total {entCount} Entities [{@this._entityId}] are detected out of {crList.Count} CustomRegion:");
                        }
                        else strBldr.AppendLine($"Total {entities.Count} Entities [{@this._entityId}] are detected");
                    }
                    else strBldr.AppendLine($"No Entity [{@this._entityId}] was found.");
#if timeout
                    strBldr.Append($"Timeout left: {timeout.Left}"); 
#endif

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(conditionIDstr, '.', nameof(TestInfos), ':', strBldr.ToString());

                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    return strBldr.ToString();
                }
                return $"Property '{nameof(@this._entityId)}' does not set !";
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} {@this._sign} {@this._value}";
            return label;
        }
#endif

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
                for (int i = 1; i < @this._customRegionNames.Count; i++)
                    sb.Append(", ").Append(@this._customRegionNames[i]);
                sb.AppendLine("}");
            }

            sb.AppendLine();

            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                         false, 0, 0, @this._regionCheck, null/*getCustomRegions()*/, customRegionCheck);

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity
            Entity closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType,
                            @this._entityNameType, @this._entitySetType, false, 0, 0, @this._regionCheck);//, null/*getCustomRegions()*/, customRegionCheck);

            if (closestEntity != null)
            {
                bool distOk = @this._reactionRange <= 0 || closestEntity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(closestEntity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !closestEntity.IsDead;
                bool crOK = customRegionCheck(closestEntity);
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
                //TODO: Добавить отображение CustomRegion'ов в другие классы, связанные с проверкой CustomRegion 
                var crList = getCustomRegions();
                if(crList?.Count > 0)
                {
                    int crNum = 0;
                    sb.Append("\tRegionCheck: {");
                    foreach (var cr in crList)
                        if(cr.Within(closestEntity))
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

            infoString = sb.ToString();
            return true;
        }

        #region Вспомогательные методы
        internal bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity(e);
        }

        private bool internal_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{conditionIDstr}: Fail to initialize the Comparer.");
#endif
            return false;
        }

        private List<CustomRegion> internal_GetCustomRegion_Initializer()
        {
            //TODO: Исправить internal_GetCustomRegion_Initializer в других класах, связанных с проверкой CustomRegion (см. MoveToEntityEngine)
            if (@this._customRegionNames?.Count > 0)
            {
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
#if DEBUG
                if (customRegions is null || customRegions.Count == 0)
                    ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(internal_GetCustomRegion_Initializer)}: List of {nameof(@this.CustomRegionNames)} is empty");
                else ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(internal_GetCustomRegion_Initializer)}: Select List of {customRegions.Count} CustomRegions");
#endif
            }
            else
            {
                customRegions = null;
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(internal_GetCustomRegion_Initializer)}: List of {nameof(@this.CustomRegionNames)} is empty");
#endif
            }

            getCustomRegions = internal_GetCustomRegion_Getter;
            return customRegions;
        }

        private List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        }

        private bool internal_CustomRegionCheck_Initializer(Entity e)
        {
            if (getCustomRegions()?.Count > 0)
            {
                if (@this.Tested == Presence.Equal)
                {
                    customRegionCheck = internal_CustomRegionCheckWithing;
#if DEBUG
                    ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(internal_CustomRegionCheck_Initializer)}: {nameof(customRegionCheck)} := {nameof(internal_CustomRegionCheckWithing)}");
#endif
                }
                else
                {
                    customRegionCheck = internal_CustomRegionCheckOutsize;
#if DEBUG
                    ETLogger.WriteLine(LogType.Debug, $"{conditionIDstr}.{nameof(internal_CustomRegionCheck_Initializer)}: {nameof(customRegionCheck)} := {nameof(internal_CustomRegionCheckOutsize)}");
#endif
                }
            }
            else customRegionCheck = (Entity ett) => true;

            return customRegionCheck?.Invoke(e) == true;
        }
        private bool internal_CustomRegionCheckWithing(Entity e)
        {
            return getCustomRegions()?.FindIndex((cr) => e.Within(cr)) >= 0;
        }
        private bool internal_CustomRegionCheckOutsize(Entity e)
        {
            return getCustomRegions()?.FindIndex((cr) => e.Within(cr)) < 0;
        }
        #endregion
    }
}
