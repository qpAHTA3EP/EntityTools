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
        /// Полный катализатор, идентификатор которого начинается с 'Fuse_Ward_Coalescent'
        /// </summary>
        Coalescent
    }

    /// <summary>
    /// Результаты поиска предмета
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
        /// Найден полностью заполненный предмет, готовый к обработке
        /// </summary>
        FullFilled,
        /// <summary>
        /// Предмет найден, но статус обработки не определен (ProgressionLogic не валидна) 
        /// </summary>
        Indefinite
    }

    /// <summary>
    /// Способы передачи лидерства ("короны") другому члену группы
    /// </summary>
    public enum PromotionType
    {
        /// <summary>
        /// Передать "корону" первому члену группы в списке
        /// </summary>
        FirstAvailable,
        /// <summary>
        /// Передать "корону"  члену группы, следующему за лидером в порядке следования
        /// </summary>
        NextOne,
        /// <summary>
        /// Передать "корону" члену группы, следующему за лидером в алфавитном порядке
        /// </summary>
        NextAlphabetical,
        /// <summary>
        /// Передать "корону" случайному члену группы
        /// </summary>
        Random
    }
}