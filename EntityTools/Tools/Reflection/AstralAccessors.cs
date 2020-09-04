using Astral.Classes.ItemFilter;
using EntityTools.Reflection;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityTools.Reflection;

namespace EntityTools.Tools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    internal static class AstralAccessors
    {
        /// <summary>
        /// доступ к членам и методам класса 
        /// Astral.Logic.NW.VIP
        /// </summary>
        internal static class VIP
        {
            internal static readonly Func<string, Entity> GetNearestEntityByCostume = typeof(Astral.Logic.NW.VIP).GetStaticFunction<string, Entity>("GetNearestEntityByCostume");
        }


        internal static class Quester
        {
#if false
            internal static class Action
            {
                internal static readonly InstancePropertyAccessor<Astral.Quester.Classes.Action, Astral.Quester.Classes.ActionDebug> ActionDebug = null;

                static Action()
                {
                    ActionDebug = typeof(Astral.Quester.Classes.Action).GetInstanceProperty<Astral.Quester.Classes.ActionDebug>("Debug");
                }
            } 
#endif
            //Astral.Quester.FSM.States.Combat.ignoreCombatMinHP

            internal static class FSM
            {
                internal static class States
                {
                    internal static class Combat
                    {
                        internal static readonly StaticFieldAccessor<int> IgnoreCombatMinHP =
                            typeof(Astral.Quester.FSM.States.Combat).GetStaticField<int>("ignoreCombatMinHP");
                    }
                }
            }
        } 

        /// <summary>
        /// Доступ к членам и методам класса
        /// Astral.Classes.ItemFilter
        /// </summary>
        internal static class ItemFilter
        {
            internal static readonly Func<ItemFilterCore, Func<Item, bool>> IsMatch = InstanceAccessor<ItemFilterCore>.GetFunction<Item, bool>("\u0001");
        }

    }
}
