using System;

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

    [Serializable]
    public enum LogicRule
    {
        Conjunction,
        Disjunction
    }

    /// <summary>
    /// список команд для смены настроек бота
    /// </summary>
    [Serializable]
    public enum PluginSettingsCommand
    {
        DisableSlideMonitor,
        DisableSpellStuckMonitor
    }
}