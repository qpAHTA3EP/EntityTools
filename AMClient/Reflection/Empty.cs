using System;
using MyNW.Classes;
using static Astral.Quester.Classes.Action;

namespace AcTp0Tools.Reflection
{
    public static class Empty
    {
        /// <summary>
        /// Заглушка для Entity
        /// </summary>
        public static readonly Entity Entity = new Entity(IntPtr.Zero);

        /// <summary>
        /// Заглушка для сообщения об ошибке в настройках Quester.Action
        /// </summary>
        public static readonly ActionValidity ActionValidity = new ActionValidity();
    }
}
