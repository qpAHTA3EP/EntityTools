using Astral.Classes;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Enums;
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
    internal class EntityPropertyEngine : IQuesterConditionEngine ,IEntityInfos
    {
        EntityProperty @this = null;

        private string cachedString = string.Empty;

        internal EntityPropertyEngine(EntityProperty ettPr)
        {
            @this = ettPr;
            @this.doValidate = Validate;
            @this.doReset = Reset;
            @this.getString = GetString;
            @this.getTestInfos = TestInfos;
            @this.PropertyChanged += PropertyChanged;
        }

        Entity closestEntity = null;
        private Timeout timeout = new Timeout(0);
        private Predicate<Entity> Comparer = null;
        private List<CustomRegion> customRegions = new List<CustomRegion>();

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.EntityID) 
                    || e.PropertyName == nameof(@this.PropertyType) 
                    || e.PropertyName == nameof(@this.Sign) 
                    || e.PropertyName == nameof(@this.Value))
                    cachedString = $"Entity [{@this.EntityID}] {@this.PropertyType} {@this.Sign} to {@this.Value}"; 
            }
        }

        internal bool Validate()
        {
            if (timeout.IsTimedOut || (closestEntity != null && !Validate(closestEntity)))
            {
                if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                    customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

                if (Comparer == null && !string.IsNullOrEmpty(@this.EntityID))
                    Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);

                if (!string.IsNullOrEmpty(@this.EntityID))
                    closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                                                            @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);

                timeout.ChangeTime(@this.SearchTimeInterval);
            }

            if (Validate(closestEntity))
            {
                bool result = false;
                switch (@this.PropertyType)
                {
                    case EntityPropertyType.Distance:
                        switch (@this.Sign)
                        {
                            case Relation.Equal:
                                return result = (closestEntity.Location.Distance3DFromPlayer == @this.Value);
                            case Relation.NotEqual:
                                return result = (closestEntity.Location.Distance3DFromPlayer != @this.Value);
                            case Relation.Inferior:
                                return result = (closestEntity.Location.Distance3DFromPlayer < @this.Value);
                            case Relation.Superior:
                                return result = (closestEntity.Location.Distance3DFromPlayer > @this.Value);
                        }
                        break;
                    case EntityPropertyType.HealthPercent:
                        switch (@this.Sign)
                        {
                            case Relation.Equal:
                                return result = (closestEntity.Character?.AttribsBasic?.HealthPercent == @this.Value);
                            case Relation.NotEqual:
                                return result = (closestEntity.Character?.AttribsBasic?.HealthPercent != @this.Value);
                            case Relation.Inferior:
                                return result = (closestEntity.Character?.AttribsBasic?.HealthPercent < @this.Value);
                            case Relation.Superior:
                                return result = (closestEntity.Character?.AttribsBasic?.HealthPercent > @this.Value);
                        }
                        break;
                    case EntityPropertyType.ZAxis:
                        switch (@this.Sign)
                        {
                            case Relation.Equal:
                                return result = (closestEntity.Location.Z == @this.Value);
                            case Relation.NotEqual:
                                return result = (closestEntity.Location.Z != @this.Value);
                            case Relation.Inferior:
                                return result = (closestEntity.Location.Z < @this.Value);
                            case Relation.Superior:
                                return result = (closestEntity.Location.Z > @this.Value);
                        }
                        break;
                }
            }
            else return (@this.PropertyType == EntityPropertyType.Distance && @this.Sign == Relation.Superior);

            return false;
        }

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer?.Invoke(e) == true;
        }

        internal string GetString()
        {
            if (string.IsNullOrEmpty(cachedString))
                cachedString = $"Entity [{@this.EntityID}] {@this.PropertyType} {@this.Sign} to {@this.Value}"; ;
            return cachedString;
        }

        internal string TestInfos()
        {
            if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

            closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                                                    @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);

            if (Validate(closestEntity))
            {
                StringBuilder sb = new StringBuilder("Found closect Entity");
                sb.Append(" [").Append(closestEntity.NameUntranslated).Append(']').Append(" which ").Append(@this.PropertyType).Append(" = ");
                switch (@this.PropertyType)
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
