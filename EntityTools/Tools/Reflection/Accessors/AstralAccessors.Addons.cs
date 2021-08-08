using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using AStar;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Grinder.Classes;
using Astral.Logic.Classes.Map;
using HarmonyLib;
using MyNW.Classes;
using static Astral.Quester.Classes.Action;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        public static class Addons
        {
            /// <summary>
            /// доступ к членам и методам класса 
            /// Astral.Addons.Role
            /// </summary>
            public static class Role
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Addons.Role).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Addons.Role).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Addons.Role).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Addons.Role).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Addons.Role).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Addons.Role).GetAction("TooMuchStuckReaction");
            }
        }
    }
}
