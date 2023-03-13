using System;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Enums
{
    /// <summary>
    /// Перечисление типов проверки внутриигровых интрейфейсов
    /// </summary>
    public enum UiGenCheckType
    {
        /// <summary>
        /// Видимый
        /// </summary>
        IsVisible,
        /// <summary>
        /// Невидимый
        /// </summary>
        IsHidden,
        /// <summary>
        /// Проверка значений свойства 
        /// </summary>
        Property
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TestTimer
    {
        /// <summary>
        /// Сколько времни осталось до истечения таймаута
        /// </summary>
        Left,
        /// <summary>
        /// Сколько времени прошло со старта таймера
        /// </summary>
        Passed
    }

    /// <summary>
    /// Перечисление 
    /// </summary>
    [Serializable]
    public enum InteractionRequirement
    {
        /// <summary>
        /// Взаимодействие не производится.
        /// </summary>
        Forbidden,
        /// <summary>
        /// Взаимодействовать, если возможно.
        /// </summary>
        IfPossible,
        /// <summary>
        /// Взаимодействовать, если возможно, однократно.
        /// </summary>
        Once,
        /// <summary>
        /// Взаимодействие обязательно.
        /// </summary>
        Obligatory
    }

    /// <summary>
    /// Перечисление методов взаимодействия с Entity
    /// </summary>
    [Serializable]
    public enum InteractionMethod
    {
        /// <summary>
        /// Перебор методов NPC, Generic и SimulateFKey
        /// </summary>
        Auto,
        /// <summary>
        /// Взаимодействие с Entity как с NPC
        /// </summary>
        NPC,
        /// <summary>
        /// Взаимодействие с Entity как с неопределенным объектом
        /// </summary>
        Generic,
        /// <summary>
        /// Эмуляция нажатия кнопки 'F'
        /// </summary>
        SimulateFKey,
        /// <summary>
        /// Следование за Entity и повторение попыток взаимодействия с ним как с NPC
        /// </summary>
        FollowAndInteractNPC,
        /// <summary>
        /// Следование за Entity и повторение попыток взаимодействия с ним эмулируя нажатие кнопки 'F'
        /// </summary>
        FollowAndSimulateFKey
    }



    /// <summary>
    /// Тип местоположения простого шаблона(подстроки) в исходной строке
    /// </summary>
    public enum SimplePatternPos
    {
        /// <summary>
        /// Не задано, некорректное значение
        /// </summary>
        None,
        /// <summary>
        /// Полное совпадение
        /// </summary>
        Full,
        /// <summary>
        /// В начале строки
        /// </summary>
        Start,
        /// <summary>
        /// В конце строки
        /// </summary>
        End,
        /// <summary>
        /// В середине строки
        /// </summary>
        Middle
    }

    /// <summary>
    /// Логические правила для управления набором условий
    /// </summary>
    [Serializable]
    public enum LogicRule
    {
        /// <summary>
        /// Конъюнкция - Логическое И
        /// </summary>
        Conjunction,
        /// <summary>
        /// Дизъюнкция - Логическое ИЛИ
        /// </summary>
        Disjunction
    } 

    /// <summary>
    /// список команд для смены настроек бота
    /// </summary>
    [Serializable]
    public enum PluginSettingsCommand
    {
        DisableSlideMonitor,
        DisableUnstuckSpell,
        EntityCacheTime,
        EntityCacheCombatTime
    }

    /// <summary>
    /// Перечисление полей Entity, по которым производится поиск
    /// </summary>
    [Serializable]
    public enum EntityNameType
    {
        InternalName,
        NameUntranslated,
        Empty
    }

    /// <summary>
    /// Тип сопоставления
    /// </summary>
    [Serializable]
    public enum MatchType
    {
        Match,
        Mismatch
    }

    /// <summary>
    /// Перечисление 
    /// </summary>
    [Serializable]
    public enum EntityPropertyType
    {
        Distance,
        ZAxis,
        HealthPercent
    }

    /// <summary>
    /// Переключатель множеств Entity в которых производится поиск
    /// </summary>
    [Serializable]
    public enum EntitySetType : uint
    {
#if false
        /// <summary>
        /// Только дружелюбные Entity (Entity.RelationToPlayer == EntityRelation.Friend)
        /// </summary>
        Friends = 1,
        /// <summary>
        /// Только враждебные Entity (Entity.RelationToPlayer == EntityRelation.Foe)
        /// </summary>
        Enemies = 2,
        /// <summary>
        /// Нейтральные Entity (Entity.RelationToPlayer == EntityRelation.Neutral)
        /// </summary>
        Neutral = 4, 
#endif
        /// <summary>
        /// Entity из подмножеств NearbyContacts и NearbyInteractCritterEnts
        /// </summary>
        Contacts = 8,
        /// <summary>
        /// Все <seealso cref="Entity"/> из <seealso cref="EntityManager.GetEntities()"/>
        /// </summary>
        Complete = uint.MaxValue
    }
}
