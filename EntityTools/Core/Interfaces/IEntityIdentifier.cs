using Astral.Classes.ItemFilter;
using EntityTools.Enums;

namespace EntityTools.Core.Interfaces
{
    /// <summary>
    /// Интерфейс, опередляющий набор свойств, идентифицирующих <seealso cref="Entity"/>
    /// </summary>
    public interface IEntityIdentifier
    {
        /// <summary>
        /// Текстовый идентификатор <seealso cref="Entity"/>
        /// </summary>
        string EntityID { get; set; }

        /// <summary>
        /// Идентификатор типа <see cref="EntityID"/>
        /// </summary>
        ItemFilterStringType EntityIdType { get; set; }

        /// <summary>
        /// Переключатель свойства <seealso cref="Entity"/>, с которым сопоставляется <see cref="EntityID"/>
        /// </summary>
        EntityNameType EntityNameType { get; set; }
    }
}
