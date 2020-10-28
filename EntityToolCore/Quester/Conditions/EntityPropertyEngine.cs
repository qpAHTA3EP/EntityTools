using System;
using System.Collections.Generic;
using System.Text;
using Astral.Classes;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Enums;
using EntityCore.Extensions;
using EntityTools.Quester.Conditions;
using MyNW.Classes;
using static Astral.Quester.Classes.Condition;
using EntityTools;

namespace EntityCore.Quester.Conditions
{
    internal class EntityPropertyEngine : IEntityInfos
#if CORE_INTERFACES
        , IQuesterConditionEngine
#endif
    {
        private EntityProperty @this = null;

        #region Данные ядра

        private Predicate<Entity> checkEntity = null;
        private Func<List<CustomRegion>> getCustomRegions = null;

        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private List<CustomRegion> customRegions = null;

        private string label = string.Empty; 
        #endregion

        internal EntityPropertyEngine(EntityProperty ettPr)
        {
            @this = ettPr;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.EntityID)
                    || e.PropertyName == nameof(@this.PropertyType)
                    || e.PropertyName == nameof(@this.Sign)
                    || e.PropertyName == nameof(@this.Value))
                    label = string.Empty;
                else if (e.PropertyName == nameof(@this.EntityID)
                        || e.PropertyName == nameof(@this.EntityIdType)
                        || e.PropertyName == nameof(@this.EntityNameType))
                {
                    label = string.Empty;
                    checkEntity = internal_CheckEntity_Initializer;
                }
                else if (e.PropertyName == nameof(@this.CustomRegionNames))
                    getCustomRegions = internal_GetCustomRegion_Initializer;

                entity = null;
                timeout.ChangeTime(0);
            }
        }

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
            if (@this.CustomRegionNames != null && @this.CustomRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this.CustomRegionNames[0]);
                for (int i = 1; i < @this.CustomRegionNames.Count; i++)
                    sb.Append(", ").Append(@this.CustomRegionNames[i]);
                sb.AppendLine("}");
            }

            sb.AppendLine();
            
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType,
                @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, getCustomRegions());

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity
#if false
            Entity closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType,
                            @this.EntityNameType, @this.EntitySetType, @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, getCustomRegions());
#else
            Entity closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType,
                @this.EntityNameType, @this.EntitySetType, false, 0, 0, @this.RegionCheck, getCustomRegions());

#endif
            if (closestEntity != null && closestEntity.IsValid)
            {
                bool distOk = @this._reactionRange <= 0 || closestEntity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(closestEntity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !closestEntity.IsDead;
                sb.Append("ClosestEntity: ").Append(closestEntity.ToString());
                if (distOk && zOk && alive)
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
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }

#if CORE_INTERFACES
        public bool IsValid
        {
            get
            {
                if (timeout.IsTimedOut || !ValidateEntity(entity))
                {
                    if (!string.IsNullOrEmpty(@this._entityId))
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                                @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                if (ValidateEntity(entity))
                {
                    switch (@this.PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Location.Distance3DFromPlayer - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Location.Distance3DFromPlayer < @this._value;
                                case Relation.Superior:
                                    return entity.Location.Distance3DFromPlayer > @this._value;
                            }
                            break;
                        case EntityPropertyType.HealthPercent:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Character.AttribsBasic.HealthPercent - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Character.AttribsBasic.HealthPercent < @this._value;
                                case Relation.Superior:
                                    return entity.Character.AttribsBasic.HealthPercent > @this._value;
                            }
                            break;
                        case EntityPropertyType.ZAxis:
                            switch (@this.Sign)
                            {
                                case Relation.Equal:
                                    return Math.Abs(entity.Location.Z - @this._value) < 0.1;
                                case Relation.NotEqual:
                                    return Math.Abs(entity.Location.Z - @this._value) > 0.1;
                                case Relation.Inferior:
                                    return entity.Location.Z < @this._value;
                                case Relation.Superior:
                                    return entity.Location.Z > @this._value;
                            }
                            break;
                    }
                }
                else return @this._propertyType == EntityPropertyType.Distance && @this._sign == Relation.Superior;

                return false;
            }
        }

        public void Reset() { }

        public string TestInfos
        {
            get
            {
                entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                        @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                if (ValidateEntity(entity))
                {
#if false
                    StringBuilder sb = new StringBuilder("Found closest Entity");
                    sb.Append(" [");
                    switch (@this._entityNameType)
                    {
                        case EntityNameType.NameUntranslated:
                            sb.Append(entity.NameUntranslated);
                            break;
                        case EntityNameType.InternalName:
                            sb.Append(entity.InternalName);
                            break;
                    }
                    sb.Append(']').Append(" which ").Append(@this.PropertyType).Append(" = ");
                    switch (@this.PropertyType)
                    {
                        case EntityPropertyType.Distance:
                            sb.Append(entity.Location.Distance3DFromPlayer);
                            break;
                        case EntityPropertyType.ZAxis:
                            sb.Append(entity.Location.Z);
                            break;
                        case EntityPropertyType.HealthPercent:
                            sb.Append(entity.Character.AttribsBasic.HealthPercent);
                            break;
                    }
                    return sb.ToString(); 
#else
                    return string.Concat("Found closest Entity [",
                        @this._entityNameType == EntityNameType.NameUntranslated ? entity.NameUntranslated : entity.InternalName,
                        "] which ", @this.PropertyType, " = ",
                        @this.PropertyType == EntityPropertyType.Distance ? entity.Location.Distance3DFromPlayer.ToString("N2") :
                        @this.PropertyType == EntityPropertyType.ZAxis ? entity.Location.Z .ToString("N2") : entity.Character.AttribsBasic.HealthPercent.ToString("N2"));
#endif
                }
                else
                {
#if false
                    StringBuilder sb = new StringBuilder("No one Entity matched to");
                    sb.Append(" [").Append(@this._entityId).Append(']').AppendLine();
                    if (@this.PropertyType == EntityPropertyType.Distance)
                        sb.AppendLine("The distance to the missing Entity is considered equal to infinity.");
                    return sb.ToString(); 
#else
                    return string.Concat("No one Entity matched to [", @this._entityId, ']', Environment.NewLine,
                        @this.PropertyType == EntityPropertyType.Distance ? "The distance to the missing Entity is considered equal to infinity." : string.Empty);
#endif
                }
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"Entity [{@this._entityId}] {@this._propertyType} {@this._sign} to {@this._value}";
            return label;
        }
#endif


        #region Вспомогательные методы
        internal bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid
                //&& e.Critter.IsValid  <- Некоторые Entity, например игроки, имеют априори невалидный Critter
                && checkEntity(e);
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
                return e != null && checkEntity(e);
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
