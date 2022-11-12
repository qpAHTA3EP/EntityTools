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

    /// <summary>
    /// Частицы, повышающие шанс улучшения предметов
    /// </summary>
    [Flags]
    public enum MoteType
    {
        None = 0,
        /// <summary>
        /// Примитивная частица - шанс улучшения: 1%.<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_1'.
        /// </summary>
        Mote_1 = 1,
        /// <summary>
        /// Обычная частица - шанс улучшения: 2%.<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_2'.
        /// </summary>
        Mote_2 = 2,
        /// <summary>
        /// Улучшенная частица - шанс улучшения: 5%.<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_5'.
        /// </summary>
        Mote_5 = 4,
        /// <summary>
        /// Усиленная частица - шанс улучшения: 10%.<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_10'.
        /// </summary>
        Mote_10 = 8,
        /// <summary>
        /// Любая частица, дающая шанс улучшения менее 100%.<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_'.
        /// </summary>
        AnyProbableMote = 16,
        /// <summary>
        /// Цельная пыль - шанс улучшения: 100%<br/>
        /// Идентификатор которой начинается с 'Fuse_Ward_Coalescent'.<br/>
        /// (бывший Полный катализатор)
        /// </summary>
        Mote_100 = 256
    }


    /// <summary>
    /// Слоты компонентов для улучшения предметов (Evolving)
    /// </summary>
    [Flags]
    public  enum CatalystSlots
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3
    }

    /// <summary>
    /// Результаты поиска предмета
    /// </summary>
    [Flags]
    public enum SearchResult
    {
        /// <summary>
        /// Предмет не найден
        /// </summary>
        Absent = 0,
        /// <summary>
        /// Непривязанный предмет
        /// </summary>
        Unbounded = 1,
        /// <summary>
        /// Привязанный к аккаунту предмет
        /// </summary>
        Bounded2Account = 2,
        /// <summary>
        /// Привязанные к персонажу предмет
        /// </summary>
        Bounded2Character = 4,

        /// <summary>
        /// Предмет найден, но статус обработки не определен (ProgressionLogic не валидна) 
        /// </summary>
        Indefinite = 256
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
        Head = InvBagIDs.Head,
        /// <summary>
        /// Ожерелье (шея)
        /// </summary>
        Neck = InvBagIDs.Neck,
        /// <summary>
        /// Броня (тело)
        /// </summary>
        Body = InvBagIDs.Armor,
        /// <summary>
        /// Наручи (руки)
        /// </summary>
        Arms = InvBagIDs.Arms,
        /// <summary>
        /// Пояс
        /// </summary>
        Waist = InvBagIDs.Waist,
        /// <summary>
        /// Поножи (ноги)
        /// </summary>
        Feet = InvBagIDs.Feet,
        //Hands = InvBagIDs.AdventuringHands,
        /// <summary>
        /// Основное оружие (для правой руки)
        /// </summary>
        WeaponMain = InvBagIDs.Melee,
        /// <summary>
        /// Вспомогательное оружие или предмет (для левой руки)
        /// </summary>
        WeaponSecondary = InvBagIDs.Melee + 1,
        /// <summary>
        /// Рубашка
        /// </summary>
        Shirt = InvBagIDs.Shirt,
        /// <summary>
        /// Штаны
        /// </summary>
        Paints = InvBagIDs.Pants,
        /// <summary>
        /// Левое кольцо (верхнее, слот 0)
        /// </summary>
        RingLeft = InvBagIDs.Ring,
        /// <summary>
        /// Правое кольцо (нижнее, слот 1)
        /// </summary>
        RingRight = InvBagIDs.Ring + 1,
        ///// <summary>
        ///// Основной (активный) артефакт
        ///// </summary>
        ////ArtifactPrimary = InvBagIDs.ArtifactPrimary,
        ///// <summary>
        ///// Дополнительный артефакт (правый, слот 0)
        ///// </summary>
        ////ArtifactSecondary1 = InvBagIDs.ArtifactSecondary,
        ///// <summary>
        ///// Дополнительный артефакт (средний, слот 1)
        ///// </summary>
        ////ArtifactSecondary2 = InvBagIDs.ArtifactSecondary + 1,
        ///// <summary>
        ///// Дополнительный артефакт (левый, слот 2)
        ///// </summary>
        ////ArtifactSecondary3 = InvBagIDs.ArtifactSecondary + 2
    }

    /// <summary>
    /// Типы инсигний (знаков скакунов)
    /// </summary>
    public enum InsigniaType
    {
        Empty = 0,
        Crescent = 262144,
        Regal = 524288,
        Barbed = 1048576,
        Illuminated = 2097152,
        Enlightened = 4194304,
        Universal = 8126464
    }
}