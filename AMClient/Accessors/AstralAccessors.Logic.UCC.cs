using MyNW.Classes;
using AcTp0Tools.Reflection;

namespace AcTp0Tools
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

#if false
                public static class Core
                {
                    /// <summary>
                    /// Делегат, вызываемый перед вызовом <seealso cref="UCC.Core.CombatUnit(Entity target)"/>
                    /// </summary>
                    /// <returns>указывает на необходимость вызывать <seealso cref="UCC.Core.CombatUnit(Entity target)"/></returns>
#if true
                    public delegate bool BeforeCombatUnitEvent(Entity target);
#else
                    public delegate bool BeforeCombatUnitEvent(Astral.Logic.UCC.Core core, Entity target, ref bool result); 
#endif
                    public static event BeforeCombatUnitEvent BeforeCombatUnit;
                    private static bool prefixCombatUnit(Astral.Logic.UCC.Core ___instance, ref bool __result, Entity target)
                    {
                        bool needCallCombatUnit = true;
                        if (BeforeCombatUnit != null)
                        {
                            needCallCombatUnit = BeforeCombatUnit(target);
                        }
                        return needCallCombatUnit;
                    }


                    /// <summary>
                    /// Делегат, вызываемый после вызовом <seealso cref="UCC.Core.CombatUnit(Entity target)"/>
                    /// </summary>
                    /// <param name="target"></param>
#if true
                    public delegate void AfterCombatUnitEvent(Entity target);
#else
                    public delegate void AfterCombatUnitEvent(Astral.Logic.UCC.Core core, Entity target, ref bool result); 
#endif
                    public static event AfterCombatUnitEvent AfterCombatUnit;
                    private static void postfixCombatUnit(Astral.Logic.UCC.Core ___instance, ref bool __result, Entity target)
                    {
                        AfterCombatUnit?.Invoke(target);
                    }

                    static Core()
                    {
                        var originalCombatUnit = typeof(Astral.Logic.UCC.Core).GetMethod(nameof(Astral.Logic.UCC.Core.CombatUnit));
                        var prefixCombatUnit = typeof(Core).GetMethod(nameof(Core.prefixCombatUnit));
                        var postfixCombatUnit = typeof(Core).GetMethod(nameof(Core.postfixCombatUnit));

                        if (originalCombatUnit != null)
                            Ast0Patcher.Harmony.Patch(originalCombatUnit, new HarmonyMethod(prefixCombatUnit), new HarmonyMethod(postfixCombatUnit));
                    }
                } 
#endif
            }
        }
    }
}
