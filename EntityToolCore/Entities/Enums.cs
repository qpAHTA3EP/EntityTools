using System;

namespace EntityCore.Enums
{
    /// <summary>
    /// Перечисление полей Entity, по которым производится поиск
    /// </summary>
    [Serializable]
    public enum coreEntityNameType
    {
        NameUntranslated,
        InternalName,
        Empty
    }

    /// <summary>
    /// Тип сопоставления
    /// </summary>
    [Serializable]
    public enum coreMatchType
    {
        Match,
        Mismatch
    }

    /// <summary>
    /// Перечисление 
    /// </summary>
    [Serializable]
    public enum coreEntityPropertyType
    {
        Distance,
        ZAxis,
        HealthPercent
    }

    /// <summary>
    /// Переключатель множетв Entity в которых производится поиск
    /// </summary>
    public enum coreEntitySetType
    {
        /// <summary>
        /// Все Entity из EntityManager.GetEntities()
        /// </summary>
        Complete,
        /// <summary>
        /// Entity из подмножеств NearbyContacts и NearbyInteractCritterEnts
        /// </summary>
        Contacts
    }
}