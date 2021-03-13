using Astral.Classes.ItemFilter;
using MyNW.Classes;
using System;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// доступ к членам и методам класса 
        /// Astral.Logic.NW.VIP
        /// </summary>
        public static class VIP
        {
            static readonly Func<string, Entity> getNearestEntityByCostume = typeof(Astral.Logic.NW.VIP).GetStaticFunction<string, Entity>("GetNearestEntityByCostume");
            public static Entity GetNearestEntityByCostume(string costume)
            {
                return getNearestEntityByCostume?.Invoke(costume);
            }
        }

        /// <summary>
        /// Доступ к членам и методам класса
        /// Astral.Classes.ItemFilter
        /// </summary>
        public static class ItemFilter
        {
            public static readonly Func<ItemFilterCore, Func<Item, bool>> IsMatch = InstanceAccessor<ItemFilterCore>.GetFunction<Item, bool>("\u0001");
        }
    }
}
