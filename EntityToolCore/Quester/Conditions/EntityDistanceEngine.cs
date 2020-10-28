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
using MyNW.Internals;
using EntityTools;

namespace EntityCore.Quester.Conditions
{
    internal class EntityDistanceEngine : IEntityInfos
#if CORE_INTERFACES
        , IQuesterConditionEngine
#endif
    {
        #region Данные ядра
        private EntityDistance @this = null;

        private string label = string.Empty; 
        #endregion

        internal EntityDistanceEngine(EntityDistance ettDist)
        {
            @this = ettDist;
#if CORE_DELEGATES
            @this.doValidate = Validate;
            @this.doReset = Reset;
            @this.getString = GetString;
            @this.getTestInfos = TestInfos;
            @this.PropertyChanged += PropertyChanged;
#endif
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.EntityID) 
                    || e.PropertyName == nameof(@this.Sign) 
                    || e.PropertyName == nameof(@this.Distance))

                    label = string.Empty;
            }
        }

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this.EntityID);
            sb.Append("EntityIdType: ").AppendLine(@this.EntityIdType.ToString());
            //sb.Append("EntityNameType: ").AppendLine(@this.EntityNameType.ToString());
            //sb.Append("HealthCheck: ").AppendLine(@this.HealthCheck.ToString());
            //sb.Append("ReactionRange: ").AppendLine(@this.ReactionRange.ToString());
            //sb.Append("ReactionZRange: ").AppendLine(@this.ReactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this.RegionCheck.ToString());
            //if (customRegions != null && customRegions.Count > 0)
            //{
            //    sb.Append("RegionCheck: {").Append(customRegions[0].Name);
            //    for (int i = 1; i < customRegions.Count; i++)
            //        sb.Append(", ").Append(customRegions[i].Name);
            //    sb.AppendLine("}");
            //}

            sb.AppendLine();

#if false
            List<Entity> entities = /*SearchCached.FindAllEntity(@this.EntityID, @this.EntityIdType, EntityNameType.co, @this.EntitySetType,
                @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);*/
                                EntitySelectionTools.FindAllEntities(EntityManager.GetEntities(), @this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, false, @this._regionCheck);
#else
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, @this._regionCheck);
#endif
            // Количество Entity, удовлетворяющих условиям
            if (entities != null && entities.Count > 0)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

#if false
            // Ближайшее Entity
            Entity closestEntity = /*SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType,
                                    @this.EntityNameType, @this.EntitySetType, @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);*/
                                    EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), @this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, false, 0, @this._regionCheck);
#else
            Entity closestEntity = SearchCached.FindClosestEntity(@this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, @this._regionCheck);
#endif
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

#if CORE_INTERFACES
        public bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityID))
                {
#if false
                    Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), @this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, false, 0, @this._regionCheck);
#else
                    Entity closestEntity = SearchCached.FindClosestEntity(@this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, @this._regionCheck);
#endif
                    switch (@this._sign)
                    {
                        case Relation.Equal:
                            return closestEntity != null && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer == @this._distance);
                        case Relation.NotEqual:
                            return closestEntity != null && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer != @this._distance);
                        case Relation.Inferior:
                            return closestEntity != null && closestEntity.IsValid && (closestEntity.Location.Distance3DFromPlayer < @this._distance);
                        case Relation.Superior:
                            return closestEntity == null || !closestEntity.IsValid || (closestEntity.Location.Distance3DFromPlayer > @this._distance);
                    }
                }

                return false;
            }
        }

        public void Reset() { }

        public string TestInfos
        {
            get
            {
#if false
                Entity closestEntity = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), @this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, false, 0, @this._regionCheck);
#else
                Entity closestEntity = SearchCached.FindClosestEntity(@this._entityID, @this._entityIdType, EntityNameType.NameUntranslated, EntitySetType.Complete, false, 0, 0, @this._regionCheck);
#endif
                if (closestEntity.IsValid)
                     return $"Found closect Entity [{closestEntity.NameUntranslated}] at the {nameof(@this.Distance)} = {closestEntity.Location.Distance3DFromPlayer}";
                return $"No one Entity matched to [{@this._entityID}]";
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"[Deprecated] Entity [{@this._entityID}] Distance {@this._sign} to {@this._distance}";
            else label = $"[Deprecated] {@this.GetType().Name}";
            return label;
        }
#endif
    }
}
