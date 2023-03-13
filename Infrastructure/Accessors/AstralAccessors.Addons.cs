using System;
using Infrastructure.Reflection;
using Astral.Logic.Classes.Map;

namespace Infrastructure
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
#if false
        public static class Addons
        {
            /// <summary>
            /// доступ к членам и методам класса <seealso cref="Astral.Addons.Role"/>
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
#endif
    }
}
