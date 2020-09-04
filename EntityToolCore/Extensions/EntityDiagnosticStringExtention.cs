using EntityCore.Entities;
using EntityTools.Enums;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore.Extensions
{
    public static class EntityDiagnosticStringExtention
    {
        public static string EntityDiagnosticString<T>(this T @this)
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
            else sb.AppendLine("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            Entity entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                           @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);
            if (entity != null)
            {
                sb.Append("Target: ").AppendLine(entity.ToString());
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(entity.IsDead.ToString());
                sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(entity.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            return sb.ToString();
        }
    }
}
