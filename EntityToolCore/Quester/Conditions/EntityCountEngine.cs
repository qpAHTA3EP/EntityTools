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

        private List<CustomRegion> customRegions = null;
        private LinkedList<Entity> entities = null;
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        private string label = string.Empty;

        internal EntityCountEngine(EntityCount ettc)
        {
            @this = ettc;

#if CORE_DELEGATES
            @this.doValidate = Validate;
            @this.doReset = Reset;
            @this.getString = GetString;
            @this.getTestInfos = TestInfos;
#endif
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.Sign) || e.PropertyName == nameof(@this.Value))
                    label = string.Empty;//$"{nameof(EntityCount)} {@this.Sign} {@this.Value}";
                else if(e.PropertyName == nameof(@this.EntityID)
                        || e.PropertyName == nameof(@this.EntityIdType)
                        || e.PropertyName == nameof(@this.EntityNameType))
                    checkEntity = internal_CheckEntity_Initializer;
                else if(e.PropertyName == nameof(@this.CustomRegionNames))
                    getCustomRegions = internal_GetCustomRegion_Initializer;

                entities?.Clear();
            }
        }

#if CORE_DELEGATES
        internal bool Validate()
        {
            if (!string.IsNullOrEmpty(@this.EntityID))
            {
                if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                    customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

                if (Comparer == null)
                    Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);


                //if (timeout.IsTimedOut)
                {
                    entities = SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                       @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);
                    //timeout.ChangeTime(SearchTimeInterval);
                }

                uint entCount = 0;

                if (entities != null)
                {
                    if (customRegions != null && customRegions.Count > 0)
                        foreach (Entity entity in entities)
                        {
                            bool match = false;
                            foreach (CustomRegion cr in customRegions)
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

                switch (@this.Sign)
                {
                    case Relation.Equal:
                        return entCount == @this.Value;
                    case Relation.NotEqual:
                        return entCount != @this.Value;
                    case Relation.Inferior:
                        return entCount < @this.Value;
                    case Relation.Superior:
                        return entCount > @this.Value;
                }
            }
            return false;
        }

        internal string GetString()
        {
            if (string.IsNullOrEmpty(cachedString))
                lable = $"{nameof(EntityCount)} {@this.Sign} {@this.Value}";
            return lable;
        }

        internal string TestInfos()
        {
            if (!string.IsNullOrEmpty(@this.EntityID))
            {
                if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                    customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

                entities = SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                       @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);

                StringBuilder strBldr = new StringBuilder();
                uint entCount = 0;

                if (entities != null)
                {
                    if (customRegions != null && customRegions.Count > 0)
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

                            strBldr.Append($"[{entity.InternalName}] is in CustomRegions: ").Append(strBldr2).AppendLine();
                        }

                        if (@this.Tested == Presence.Equal)
                            strBldr.Insert(0, $"Total {entCount} Entities [{@this.EntityID}] are detected in {customRegions.Count} CustomRegion:");
                        if (@this.Tested == Presence.NotEquel)
                            strBldr.Insert(0, $"Total {entCount} Entities [{@this.EntityID}] are detected out of {customRegions.Count} CustomRegion:");
                    }
                    else strBldr.Append($"Total {entities.Count} Entities [{@this.EntityID}] are detected");
                }
                else strBldr.Append($"No Entity [{@this.EntityID}] was found.");

                return strBldr.ToString();
            }
            return $"Property '{nameof(@this.EntityID)}' does not set !";
        }

        internal void Reset() { }
#endif

#if CORE_INTERFACES
        public bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    if (timeout.IsTimedOut)
                    {
                        entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                           @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());
                        timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                    }

                    uint entCount = 0;

                    if (entities != null)
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

                    switch (@this._sign)
                    {
                        case Relation.Equal:
                            return entCount == @this._value;
                        case Relation.NotEqual:
                            return entCount != @this._value;
                        case Relation.Inferior:
                            return entCount < @this._value;
                        case Relation.Superior:
                            return entCount > @this._value;
                    }
                }
                return false;
            }
        }

        public void Reset() => entities?.Clear();

        public string TestInfos
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                           @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                    StringBuilder strBldr = new StringBuilder();
                    uint entCount = 0;

                    if (entities != null)
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
                        else strBldr.Append($"Total {entities.Count} Entities [{@this._entityId}] are detected");
                    }
                    else strBldr.Append($"No Entity [{@this._entityId}] was found.");

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
                @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity
            Entity closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType,
                                    @this._entityNameType, @this._entitySetType, @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());
            if (closestEntity != null && closestEntity.IsValid)
            {
                sb.Append("ClosectEntity: ").AppendLine(closestEntity.ToString());
                sb.Append("\tName: ").AppendLine(closestEntity.Name);
                sb.Append("\tInternalName: ").AppendLine(closestEntity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(closestEntity.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(closestEntity.IsDead.ToString());
                sb.Append("\tRegion: '").Append(closestEntity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(closestEntity.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(closestEntity.Location.Distance3DFromPlayer.ToString());
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

        private List<CustomRegion> internal_GetCustomRegion_Initializer()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                getCustomRegions = internal_GetCustomRegion_Getter;
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
                return customRegions;
            }
            return null;
        }

        private List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        }
        #endregion
    }
}
