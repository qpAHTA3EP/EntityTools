using AcTp0Tools.Patches;
using Astral.Classes;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Collections.Generic;
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
            public static class NW
            {
                public static class Movements
                {
                    public static readonly StaticPropertyAccessor<List<Astral.Logic.NW.Movements.DodgeLosTestResult>> LastValidPoses
                        = typeof(Astral.Logic.NW.Movements).GetStaticProperty<List<Astral.Logic.NW.Movements.DodgeLosTestResult>>("LastValidPoses");
                    public static readonly StaticFieldAccessor<Timeout> LastValidPosesTimeout
                        = typeof(Astral.Logic.NW.Movements).GetStaticField<Timeout>("lastvlidposto");
                }

                public static class Combats
                {
                    static readonly Action<bool> abortCombat = typeof(Astral.Logic.NW.Combats).GetStaticAction<bool>(nameof(AbordCombat));

                    /// <summary>
                    /// Прерывание потока, управляющего персонажем в режиме боя
                    /// </summary>
                    /// <param name="stopMove"></param>
                    public static void AbordCombat(bool stopMove = false)
                    {
                        abortCombat?.Invoke(stopMove);
                    }

                    /// <summary>
                    /// Механизм доступа к функтору <seealso cref="Astral.Logic.NW.Combats.BLAttackersList"/>,
                    /// который передает в боевую подсистему список игнорируемых врагов
                    /// </summary>
                    public static Func<List<string>> BLAttackersList
                    {
                        get => blAttackersList.Value;
                        set
                        {
                            if (value is null)
                                blAttackersList.Value = () => Astral.Quester.API.CurrentProfile.BlackList;
                            else blAttackersList.Value = value;
                        }
                    }
                    static Traverse<Func<List<string>>> blAttackersList = null;

                    public static void SetAbortCombatCondition(Func<Entity, bool> cond, Func<bool> removeCond)
                    {
                        abortCombatCondition = cond;
                        shouldRemoveAbortCombatCondition = removeCond;
                    }
                    public static void RemoveAbortCombatCondition()
                    {
                        abortCombatCondition = null;
                        shouldRemoveAbortCombatCondition = null;
                    }
                    private static bool prefixCombatUnit(ref bool __result, ref Entity target, ref Func<bool> breakCond)
                    {
                        bool needCallCombatUnit = true;
                        if (abortCombatCondition != null)
                        {
                            needCallCombatUnit = !abortCombatCondition(target);
                            if (needCallCombatUnit && target != null)
                            {
                                var trg = new Entity(target.Pointer);
                                breakCond = () => abortCombatCondition(trg);
                            }
                        }
                        return needCallCombatUnit;
                    }
                    private static void postfixCombatUnit(bool __result, Entity target, Func<bool> breakCond)
                    {
                        if (shouldRemoveAbortCombatCondition != null)
                        {
                            if (shouldRemoveAbortCombatCondition())
                            {
                                abortCombatCondition = null;
                                shouldRemoveAbortCombatCondition = null;
                            }
                        }
                    }

                    /// <summary>
                    /// Уловие прерывания боя
                    /// </summary>
                    private static Func<Entity, bool> abortCombatCondition;
                    private static Func<bool> shouldRemoveAbortCombatCondition;

                    static Combats()
                    {
                        var originalCombatUnit = AccessTools.Method(typeof(Astral.Logic.NW.Combats), nameof(Astral.Logic.NW.Combats.CombatUnit));
                        var prefixCombatUnit = AccessTools.Method(typeof(Combats), nameof(Combats.prefixCombatUnit));
                        var postfixCombatUnit = AccessTools.Method(typeof(Combats), nameof(Combats.postfixCombatUnit));

                        if (originalCombatUnit != null)
                            AcTp0Patcher.Harmony.Patch(originalCombatUnit, new HarmonyMethod(prefixCombatUnit), new HarmonyMethod(postfixCombatUnit));
                    }
                }
            }
        }
    }
}
