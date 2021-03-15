using Astral.Classes;
using EntityTools.Patches;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Collections.Generic;

namespace EntityTools.Reflection
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

#if false
                    /// <summary>
                    /// Таймер до сброса <see cref="abortCombatCondition"/>
                    /// </summary>
                    private static Timeout abortCombatResetTimeout = new Timeout(0);
                    private static int resetTime = 5_000;

                    public static void SetAbortCombatCondition(Func<Entity, bool> cond, int time = 5_000)
                    {
                        abortCombatCondition = cond;
                        if (abortCombatCondition is null)
                            abortCombatResetTimeout.ChangeTime(0);
                        else
                        {
                            if (time > 0)
                                resetTime = time;
                            else resetTime = 5_000;
                            abortCombatResetTimeout.ChangeTime(int.MaxValue);
                        }
                    }
                    public static void RemoveAbortCombatCondition(Func<Entity, bool> cond)
                    {
                        if (cond is null)
                            return;

                        if (ReferenceEquals(abortCombatCondition, cond))
                        {
                            abortCombatCondition = null;
                            resetTime = 0;
                            abortCombatResetTimeout.ChangeTime(0);
                        }
                    }
                    private static bool prefixCombatUnit(ref bool __result, ref Entity target, ref Func<bool> breakCond)
                    {
                        bool needCallCombatUnit = true;
                        if (abortCombatResetTimeout.IsTimedOut)
                            abortCombatCondition = null;
                        else if (abortCombatCondition != null)
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
                        if (abortCombatCondition != null && resetTime > 0)
                        {
                            bool abortCombat = abortCombatCondition(target);
                            if (abortCombat)
                                //AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, -1, 500);
                                AbordCombat();

                            abortCombatResetTimeout.ChangeTime(resetTime);
                            resetTime = 0;
                        }
                    } 
#else
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
#endif
                    /// <summary>
                    /// Уловие прерывания боя
                    /// </summary>
                    private static Func<Entity, bool> abortCombatCondition;
                    private static Func<bool> shouldRemoveAbortCombatCondition;

                    static Combats()
                    {
#if false
                        var originalCombatUnit = typeof(Astral.Logic.NW.Combats).GetMethod(nameof(Astral.Logic.NW.Combats.CombatUnit));
                        var prefixCombatUnit = typeof(Combats).GetMethod(nameof(Combats.prefixCombatUnit)); 
#else
                        var originalCombatUnit = AccessTools.Method(typeof(Astral.Logic.NW.Combats), nameof(Astral.Logic.NW.Combats.CombatUnit));
                        var prefixCombatUnit = AccessTools.Method(typeof(Combats), nameof(Combats.prefixCombatUnit));
                        var postfixCombatUnit = AccessTools.Method(typeof(Combats), nameof(Combats.postfixCombatUnit));
#endif
                        if (originalCombatUnit != null)
                            ETPatcher.Harmony.Patch(originalCombatUnit, new HarmonyMethod(prefixCombatUnit), new HarmonyMethod(postfixCombatUnit));
                    }
                }
            }
        }
    }
}
