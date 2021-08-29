using System;

namespace EntityTools.Enums
{
    public enum ContactHaveMissionCheckType
    {
        /// <summary>
        /// Не выполнять проверку наличия миссий
        /// </summary>
        Disabled,
        /// <summary>
        /// Проверять только главные миссии
        /// </summary>
        Main,
        /// <summary>
        /// Проверять наличие повторяемых миссий
        /// </summary>
        RepeatablesOnly,
        /// <summary>
        /// Проверять наличие любых миссий
        /// </summary>
        Any
    }

    [Flags]
    public enum WardType
    {
        None,

        /// <summary>
        /// Страхующий катализатор, идентификатор которого начинается с 'Fuse_Ward_Preservation'
        /// </summary>
        Preservation,
        /// <summary>
        /// Полный катализацтор, идентификатор которого начинается с 'Fuse_Ward_Coalescent'
        /// </summary>
        Coalescent
    }

    /// <summary>
    /// Разультаты поиска предмета
    /// </summary>
    public enum SearchResult
    {
        /// <summary>
        /// Предмет не найден
        /// </summary>
        Absent,
        /// <summary>
        /// Найден незаполненный предмет
        /// </summary>
        Unfilled,
        /// <summary>
        /// Найден частично заполненный предмет
        /// </summary>
        PartialFilled,
        /// <summary>
        /// Найден полностью заколненный предмет, готовый к обработке
        /// </summary>
        FullFilled,
        /// <summary>
        /// Предмет найден, но стату обработки неопределен
        /// </summary>
        Indefinite
    }
}