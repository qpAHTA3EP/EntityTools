using AStar;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

namespace EntityTools.Reflection
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        public static partial class Logic
        {
            public static class UCC
            {
                public static class Controllers
                {
                    public static class Movements
                    {
                        private static readonly StaticPropertyAccessor<Entity> specificTarget = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<Entity>("SpecificTarget");
                        public static Entity SpecificTarget
                        {
                            get => specificTarget.Value ?? Empty.Entity;
                            set => specificTarget.Value = value;
                        }
                        private static readonly StaticPropertyAccessor<int> requireRange = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<int>("RequireRange");
                        public static int RequireRange
                        {
                            get => requireRange.Value;
                            set => requireRange.Value = value;
                        }
                        private static readonly StaticPropertyAccessor<bool> rangeIsOk = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<bool>("RangeIsOk");
                        public static bool RangeIsOk
                        {
                            get => rangeIsOk.Value;
                        }
                    }
                }
            }
        }
    }
}
