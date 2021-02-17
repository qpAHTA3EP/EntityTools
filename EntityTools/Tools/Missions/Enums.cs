using System;

namespace EntityTools.Enums
{ 
    [Serializable]
    public enum MissionGiverType
    {
        None,
        /// <summary>
        /// Персонаж
        /// </summary>
        NPC,
        /// <summary>
        /// Предмет
        /// </summary>
        Item,
        /// <summary>
        /// Дистанционный 
        /// </summary>
        Remote,
    }
}