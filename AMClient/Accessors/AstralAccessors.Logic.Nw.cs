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

                    public static void SetAbortCombatCondition(Func<Entity, bool> cond, Func<bool> removeCond, int resetTime = 0)
                    {
                        abortCombatCondition = cond;
                        shouldRemoveAbortCombatCondition = removeCond;
                        if (resetTime > 0)
                            AbortCombatConditionResetTime = resetTime;
                        abortCombatConditionResetTimeout.ChangeTime(AbortCombatConditionResetTime);
                    }
                    public static void RemoveAbortCombatCondition()
                    {
                        abortCombatCondition = null;
                        shouldRemoveAbortCombatCondition = null;
                        abortCombatConditionResetTimeout.ChangeTime(0);
                    }
                    private static bool prefixCombatUnit(ref bool __result, ref Entity target, ref Func<bool> breakCond)
                    {
                        if (abortCombatCondition != null)
                        {
                            if (abortCombatCondition(target))
                            {
                                Quester.FSM.States.Combat.SetIgnoreCombat(true);
                                return false;
                            }
                            else if (target != null)
                            {
                                var trg = new Entity(target.Pointer);
                                breakCond += () => abortCombatCondition(trg);
                            }
                        }
                        return true;
                    }
                    private static void postfixCombatUnit(bool __result, Entity target, Func<bool> breakCond)
                    {
                        if (abortCombatCondition != null)
                        {
                            if (abortCombatConditionResetTimeout.IsTimedOut 
                                || (shouldRemoveAbortCombatCondition != null && shouldRemoveAbortCombatCondition()))
                            {
                                abortCombatCondition = null;
                                shouldRemoveAbortCombatCondition = null;
                                abortCombatConditionResetTimeout.ChangeTime(0);
                            }
                        }
                    }

                    /// <summary>
                    /// Уловие прерывания боя
                    /// </summary>
                    private static Func<Entity, bool> abortCombatCondition;
                    private static Func<bool> shouldRemoveAbortCombatCondition;

                    /// <summary>
                    /// Время до сброса <see cref="abortCombatCondition"/>
                    /// Таймер обнавляется каждый раз при вызове <see cref="SetAbortCombatCondition"/>
                    /// </summary>
                    public static int AbortCombatConditionResetTime { get; set; } = 30_000;
                    private static Timeout abortCombatConditionResetTimeout = new Timeout(0);

                    public static string AbortCombatCondition_DebugInfo()
                    {
                        if (abortCombatCondition is null)
                            return "Not set";
                        var subscriber = abortCombatCondition.Target;
                        var currentTarget = Astral.Logic.UCC.Core.CurrentTarget;
                        string abortCombatConditionStr = string.Concat("AbortCombatCondition: ", Environment.NewLine, 
                                             '\t', subscriber.GetType().FullName, '.', abortCombatCondition.Method.Name, Environment.NewLine,
                                             "\t\t(", subscriber.ToString(), ") ", Environment.NewLine,
                                             "\tResult = ", abortCombatCondition(currentTarget));
                        string shouldRemoveAbortCombatConditionStr;
                        if (shouldRemoveAbortCombatCondition != null)
                        {
                            subscriber = shouldRemoveAbortCombatCondition.Target;
                            shouldRemoveAbortCombatConditionStr = string.Concat(Environment.NewLine, "ShouldRemoveAbortCombatCondition: ", Environment.NewLine,
                                                                              '\t', subscriber.GetType().FullName, '.', shouldRemoveAbortCombatCondition.Method.Name, Environment.NewLine,
                                                                              "\t\t(", subscriber.ToString(), ") ", Environment.NewLine,
                                                                              "\tResult = ", shouldRemoveAbortCombatCondition());
                        }
                        else shouldRemoveAbortCombatConditionStr = "\n\rShouldRemoveAbortCombatCondition: not set";
                        return abortCombatConditionStr + shouldRemoveAbortCombatConditionStr;
                    }

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
