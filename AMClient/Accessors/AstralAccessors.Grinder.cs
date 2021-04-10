﻿using Astral.Grinder.Classes;
using AcTp0Tools.Reflection;

namespace AcTp0Tools
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Grinder
        /// </summary>
        public static class Grinder
        {
            public static class Core
            {
                static readonly StaticPropertyAccessor<GrinderProfile> profile = typeof(Astral.Grinder.Core).GetStaticProperty<GrinderProfile>("Profile");
                public static GrinderProfile Profile => profile.Value;
            }
        }
    }
}
