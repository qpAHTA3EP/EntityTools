using System;
using MyNW.Patchables.Enums;

namespace EntityTools.Enums
{
    public enum ContactHaveMissionCheckType
    {
        /// <summary>
        /// Не выполнять проверку наличия миссий
        /// </summary>
        Disabled,
        /// <summary>
        /// Проверять только главные миссии (Приключения, Adventure)
        /// </summary>
        Main,
        /// <summary>
        /// Проверять наличие повторяемых миссий
        /// </summary>
        RepeatablesOnly,
        /// <summary>
        /// Проверять только миссии "событи"
        /// </summary>
        Event,
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

    /// <summary>
    /// Выбор предметов
    /// </summary>
    public enum ItemSelectorType
    {
        Any,
        Min,
        Max,
        //Slot,
        All
    }

    public enum SpecificBags : uint
    {
        /// <summary>
        /// Все сумки персонажа, включая Inventory, PlayerBags, PlayerBags1~9
        /// </summary>
        Inventory,
        //PlayerBags,
        //PlayerBag1,
        //PlayerBag2,
        //PlayerBag3,
        //PlayerBag4,
        //PlayerBag5,
        //PlayerBag6,
        //PlayerBag7,
        //PlayerBag8,
        //PlayerBag9,
        /// <summary>
        /// Персональный банк, включая Bank, Bank1~9
        /// </summary>
        Bank,
        //Bank1,
        //Bank2,
        //Bank3,
        //Bank4,
        //Bank5,
        //Bank6,
        //Bank7,
        //Bank8,
        //Bank9,
        /// <summary>
        /// Шлем (голова)
        /// </summary>
        Head = InvBagIDs.AdventuringHead,
        /// <summary>
        /// Ожерелье (шея)
        /// </summary>
        Neck = InvBagIDs.AdventuringNeck,
        /// <summary>
        /// Броня (тело)
        /// </summary>
        Body = InvBagIDs.AdventuringArmor,
        /// <summary>
        /// Наручи (руки)
        /// </summary>
        Arms = InvBagIDs.AdventuringArms,
        /// <summary>
        /// Пояс
        /// </summary>
        Waist = InvBagIDs.AdventuringWaist,
        /// <summary>
        /// Поножи (ноги)
        /// </summary>
        Feet = InvBagIDs.AdventuringFeet,
        //Hands = InvBagIDs.AdventuringHands,
        /// <summary>
        /// Основное оружие (для правой руки)
        /// </summary>
        WeaponMain = InvBagIDs.AdventuringHands,
        /// <summary>
        /// Вспомогательное оружие или предмет (для левой руки)
        /// </summary>
        WeaponSecondary = InvBagIDs.AdventuringHands + 1,
        /// <summary>
        /// Рубашка
        /// </summary>
        Shirt = InvBagIDs.AdventuringShirt,
        /// <summary>
        /// Штаны
        /// </summary>
        Paints = InvBagIDs.AdventuringTrousers,
        /// <summary>
        /// Левое кольцо (верхнее, слот 0)
        /// </summary>
        RingLeft = InvBagIDs.AdventuringRings,
        /// <summary>
        /// Правое кольцо (нижнее, слот 1)
        /// </summary>
        RingRight = InvBagIDs.AdventuringRings + 1,
        /// <summary>
        /// Основной (активный) артефакт
        /// </summary>
        //ArtifactPrimary = InvBagIDs.ArtifactPrimary,
        /// <summary>
        /// Дополнительный артефакт (правый, слот 0)
        /// </summary>
        //ArtifactSecondary1 = InvBagIDs.ArtifactSecondary,
        /// <summary>
        /// Дополнительный артефакт (средний, слот 1)
        /// </summary>
        //ArtifactSecondary2 = InvBagIDs.ArtifactSecondary + 1,
        /// <summary>
        /// Дополнительный артефакт (левый, слот 2)
        /// </summary>
        //ArtifactSecondary3 = InvBagIDs.ArtifactSecondary + 2
    }
}