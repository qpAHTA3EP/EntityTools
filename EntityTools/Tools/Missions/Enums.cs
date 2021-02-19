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