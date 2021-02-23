using EntityCore.Enums;
using EntityTools.Enums;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Enums
{
    /// <summary>
    /// Набор флагов, определяющих детализацию описания <seealso cref="Entity"/>, возвращаемого <seealso cref="MoveToEntityEngine.Get_DebugStringOfEntity"/>
    /// </summary>
    [Flags]
    public enum EntityDetail
    {
        Nope = 0,
        Pointer = 1,
        RelationToPlayer = 2,
        Alive = 4
    }
}

namespace EntityCore.Extensions
{
    public static class EntityExtentions
    {
        /// <summary>
        /// Краткая отладочная информация об <paramref name="entity"/>, используемая в логах
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityLabel"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public static string GetDebugString(this Entity entity, EntityNameType nameType = EntityNameType.InternalName, string entityLabel = "", EntityDetail detail = EntityDetail.Nope)
        {
            return string.Concat(entityLabel, "[",
                ((detail & EntityDetail.Pointer) > 0) ? /*entity.Pointer*/entity.ContainerId + "; " : string.Empty,
                (nameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated,
                ((detail & EntityDetail.RelationToPlayer) > 0) ? "; " + entity.RelationToPlayer : string.Empty,
                ((detail & EntityDetail.Alive) > 0) ? (entity.IsDead ? "; Dead" : "; Alive") : string.Empty,
                ']');
        }
    }
}
