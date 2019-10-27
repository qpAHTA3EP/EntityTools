using System;

namespace EntityTools.Tools
{
    /// <summary>
    /// Перечисление полей Entity, по которым производится поиск
    /// </summary>
    [Serializable]
    public enum EntityNameType
    {
        NameUntranslated,
        InternalName
    }

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
        IsHidden
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


}