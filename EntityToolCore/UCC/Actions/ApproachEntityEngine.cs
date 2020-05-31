using Astral.Classes;
using Astral.Logic.NW;
using EntityCore.Entities;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.UCC.Actions
{
    internal class ApproachEntityEngine : IEntityInfos
#if CORE_INTERFACES
                 , IUCCActionEngine
#endif
    {
        #region Данные
        private ApproachEntity @this;

        private Predicate<Entity> checkEntity = null;
        private Entity entity = null;
        private Timeout timeout = new Timeout(0);
        private string label = string.Empty;
        #endregion
        
        internal ApproachEntityEngine(ApproachEntity ae)
        {
            @this = ae;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        label = string.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "EntityNameType":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                }
                entity = null;
                timeout.ChangeTime(0);
            }
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    if (timeout.IsTimedOut)
                    {
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                @this._healthCheck, @this._reactionRange, (@this._reactionZRange > 0) ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                                @this._regionCheck, null, @this._aura.Checker);

                        timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.CombatCacheTime);
                    }

                    return ValidateTarget(entity) && !(@this._healthCheck && entity.IsDead) && entity.CombatDistance > @this._entityRadius;
                }
                return false;
            }
        }

        public bool Run()
        {
            if (entity.Location.Distance3DFromPlayer >= @this._entityRadius)
                return Approach.EntityByDistance(entity, @this._entityRadius);
            return true;
        }

        public Entity UnitRef
        {
            get
            {
                if (ValidateTarget(entity))
                    return entity;
                else
                {
                    if (!string.IsNullOrEmpty(@this._entityId))
                    {
                        entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                @this._healthCheck, @this._reactionRange,
                                                                (@this._reactionZRange > 0) ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                                @this._regionCheck, null, @this._aura.Checker);
                        return entity;
                    }
                }
                return null;
            }
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(@this._entityId))
                label = GetType().Name;
            else label = GetType().Name + " [" + @this._entityId + ']';
            return label;
        }
        #endregion

        private bool ValidateTarget(Entity e)
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

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            //sb.Append("EntitySetType: ").AppendLine(@this._entitySetType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            sb.Append("Aura: ").AppendLine(@this._aura.ToString());
            //if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            //{
            //    sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
            //    for (int i = 1; i < @this._customRegionNames.Count; i++)
            //        sb.Append(", ").Append(@this._customRegionNames[i]);
            //    sb.AppendLine("}");
            //}
            sb.AppendLine();
            //sb.Append("NeedToRun: ").AppendLine(NeedToRun.ToString());
            //sb.AppendLine();

            // список всех Entity, удовлетворяющих условиям
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                     @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);


            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);
            if (entity != null)
            {
                bool distOk = entity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !entity.IsDead;
                sb.Append("ClosestEntity: ").Append(entity.ToString());
                if (distOk && zOk && alive)
                    sb.AppendLine(" [MATCH]");
                else sb.AppendLine(" [MISMATCH]");
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").Append(entity.IsDead.ToString());
                if (alive)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").Append(entity.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
                if (zOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }
    }
}
