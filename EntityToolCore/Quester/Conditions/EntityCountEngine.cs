using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Extentions;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Astral.Quester.Classes.Action;
using static Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Conditions
{
    internal class EntityCountEngine : IQuesterConditionEngine, IEntityInfos
    {
        EntityCount @this = null;
        LinkedList<Entity> entities = null;
        private Predicate<Entity> Comparer = null;
        //private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        private List<CustomRegion> customRegions = new List<CustomRegion>();
        private string cachedString = string.Empty;

        internal EntityCountEngine(EntityCount ettc)
        {
            @this = ettc;
            @this.doValidate = Validate;
            @this.doReset = Reset;
            @this.getString = GetString;
            @this.getTestInfos = TestInfos;
            @this.PropertyChanged += PropertyChanged;
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.Sign) || e.PropertyName == nameof(@this.Value))
                    cachedString = $"{nameof(EntityCount)} {@this.Sign} {@this.Value}";
            }
        }

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
                cachedString = $"{nameof(EntityCount)} {@this.Sign} {@this.Value}";
            return cachedString;
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

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this.EntityID);
            sb.Append("EntityIdType: ").AppendLine(@this.EntityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this.EntityNameType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this.HealthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this.ReactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this.ReactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this.RegionCheck.ToString());
            if (customRegions != null && customRegions.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(customRegions[0].Name);
                for (int i = 1; i < customRegions.Count; i++)
                    sb.Append(", ").Append(customRegions[i].Name);
                sb.AppendLine("}");
            }

            sb.AppendLine();
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity
            Entity closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType,
                                    @this.EntityNameType, @this.EntitySetType, @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);
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
    }
}
