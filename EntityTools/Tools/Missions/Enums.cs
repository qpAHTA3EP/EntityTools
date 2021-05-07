using System;

// ReSharper disable once CheckNamespace
namespace EntityTools.Enums
{ 
    [Serializable]
    public enum MissionGiverType
    {
        None,
        /// <summary>
        /// Персонаж
        /// </summary>
        // ReSharper disable once InconsistentNaming
        NPC,
#if false
        /// <summary>
        /// Предмет
        /// </summary>
        Item, 
#endif
        /// <summary>
        /// Дистанционный 
        /// </summary>
        Remote,
    }
}