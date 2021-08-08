using EntityCore.Enums;
using EntityTools.Enums;
using MyNW.Classes;
using System;

namespace EntityCore.Enums
{
    /// <summary>
    /// Набор флагов, определяющих детализацию описания <seealso cref="Entity"/>, возвращаемого <seealso cref="Extensions.EntityExtensions.GetDebugString"/>
    /// </summary>
    [Flags]
    public enum EntityDetail
    {
        Nope = 0b00_00_00_00,
        Pointer = 0b00_00_00_01,
        RelationToPlayer = 0b00_00_00_10,
        Alive = 0b00_00_01_00,
        Health = 0b00_00_10_00,
        HealthPersent = 0b00_01_10_00,
        Distance = 0b00_10_00_00
    }
}

namespace EntityCore.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Краткая отладочная информация об <paramref name="entity"/>, используемая в логах
        /// </summary>
        public static string GetDebugString(this Entity entity, EntityNameType nameType = EntityNameType.InternalName, string entityLabel = "", EntityDetail detail = EntityDetail.Nope)
        {
            //if (entity is null)
            //    return entityLabel + "[NULL]";
            //if (!entity.IsValid)
            //    return entityLabel + "[INVALID]";
            return string.Concat(entityLabel, "[",
                    ((detail & EntityDetail.Pointer) > 0) ? entity.ContainerId + "; " : string.Empty,
                    (nameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated,
                    ((detail & EntityDetail.RelationToPlayer) > 0) ? "; " + entity.RelationToPlayer : string.Empty,
                    ((detail & EntityDetail.Alive) > 0) ? (entity.IsDead ? "; Dead" : "; Alive") : string.Empty,
                    ((detail & EntityDetail.Health) > 0) ? entity.Character.AttribsBasic.Health.ToString("; N0 HP") : string.Empty,
                    ((detail & EntityDetail.HealthPersent) > 0) ? entity.Character.AttribsBasic.HealthPercent.ToString("; N0 HP%") : string.Empty,
                    ((detail & EntityDetail.Distance) > 0) ? "; " + entity.CombatDistance3.ToString("N2") : string.Empty,
                    ']');

        }
    }
}
