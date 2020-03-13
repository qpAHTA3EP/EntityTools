using System;

namespace EntityCore.Enums
{
    /// <summary>
    /// Перечисление полей Entity, по которым производится поиск
    /// </summary>
    [Serializable]
    public enum EntityNameType
    {
        NameUntranslated,
        InternalName,
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
    /// Переключатель множетв Entity в которых производится поиск
    /// </summary>
    public enum EntitySetType
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