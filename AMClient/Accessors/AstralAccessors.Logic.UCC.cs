using MyNW.Classes;
using AcTp0Tools.Reflection;
using HarmonyLib;

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

                public static class Core
                {
#if CombatUnitEvent
                    /// <summary>
                    /// Делегат, вызываемый перед вызовом <seealso cref="UCC.Core.CombatUnit(Entity target)"/>
                    /// </summary>
                    /// <returns>указывает на необходимость вызывать <seealso cref="UCC.Core.CombatUnit(Entity target)"/></returns>
                    public delegate bool BeforeCombatUnitEvent(Entity target);
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
                    public delegate void AfterCombatUnitEvent(Entity target);
                    public static event AfterCombatUnitEvent AfterCombatUnit;
                    private static void postfixCombatUnit(Astral.Logic.UCC.Core ___instance, ref bool __result, Entity target)
                    {
                        AfterCombatUnit?.Invoke(target);
                    }

#endif
                    /// <summary>
                    /// Текущая боевая цель персонажа
                    /// </summary>
                    public static Entity CurrentTarget
                    {
                        get => Astral.Logic.UCC.Core.CurrentTarget;
                        set => _currentTarget.Value = value;
                    }

                    private static readonly StaticPropertyAccessor<Entity> _currentTarget =
                        typeof(Astral.Logic.UCC.Core).GetStaticProperty<Entity>(
                            nameof(Astral.Logic.UCC.Core.CurrentTarget));


                    /// <summary>
                    /// Запланировать смену цели на <param name="newTarget"/>
                    /// </summary>
                    /// <param name="newTarget"></param>
                    /// <param name="reason">Текстовое описание причины смены цели (для лога)</param>
                    /// <param name="specificCD">Кулдаун смены цели после установки <param name="newTarget"/>. Измеряется в СЕКУНДАХ</param>
                    public static void QueryTargetChange(Entity newTarget, string reason, int specificCD = 0)
                    {
                        Astral.Logic.UCC.Core.Get.queryTargetChange(newTarget, reason, specificCD);
                    }

                    static Core()
                    {
#if CombatUnitEvent
                        var originalCombatUnit = typeof(Astral.Logic.UCC.Core).GetMethod(nameof(Astral.Logic.UCC.Core.CombatUnit));
                        var prefixCombatUnit = typeof(Core).GetMethod(nameof(Core.prefixCombatUnit));
                        var postfixCombatUnit = typeof(Core).GetMethod(nameof(Core.postfixCombatUnit));

                        if (originalCombatUnit != null)
                            Ast0Patcher.Harmony.Patch(originalCombatUnit, new HarmonyMethod(prefixCombatUnit), new HarmonyMethod(postfixCombatUnit)); 
#endif
                    }
                } 
            }
        }
    }
}
