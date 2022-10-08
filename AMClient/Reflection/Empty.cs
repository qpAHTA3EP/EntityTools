using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Astral.Quester.Classes.Action;

namespace ACTP0Tools.Reflection
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
