using Astral.Classes;
using HarmonyLib;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;

// ReSharper disable CheckNamespace

namespace ACTP0Tools
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
                    static readonly Action<bool> abortCombat = typeof(Astral.Logic.NW.Combats).GetStaticAction<bool>(nameof(AbortCombat));

                    public delegate bool AbortCombatConditionDelegate(ref Entity entity);

                    /// <summary>
                    /// Прерывание потока, управляющего персонажем в режиме боя
                    /// </summary>
                    /// <param name="stopMove"></param>
                    public static void AbortCombat(bool stopMove = false)
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
                        set { blAttackersList.Value = value ?? (() => Astral.Quester.API.CurrentProfile.BlackList); }
                    }
                    private static readonly StaticFieldAccessor<Func<List<string>>> blAttackersList;

                    public static void SetAbortCombatCondition(AbortCombatConditionDelegate cond, Func<bool> removeCond, int resetTime = 0)
                    {
                        abortCombatCondition = cond;
                        shouldRemoveAbortCombatCondition = removeCond;
                        if (resetTime <= 0)
                            resetTime = AbortCombatConditionResetTime;
                        abortCombatConditionResetTimeout.ChangeTime(resetTime);
                    }
                    public static void RemoveAbortCombatCondition()
                    {
                        abortCombatCondition = null;
                        shouldRemoveAbortCombatCondition = null;
                        abortCombatConditionResetTimeout.ChangeTime(0);
                    }
                    private static bool PrefixCombatUnit(ref bool __result, ref Entity target, ref Func<bool> breakCond)
                    {
                        __result = false;

                        if (abortCombatCondition is null) return true;

                        if (abortCombatCondition(ref target))
                        {
                            Quester.FSM.States.Combat.SetIgnoreCombat(true);
                            return false;
                        }
                        if (target != null)
                        {
                            var trg = new Entity(target.Pointer);
                            breakCond += () => abortCombatCondition(ref trg);
                        }
                        return true;
                    }
                    private static void PostfixCombatUnit(bool __result, Entity target, Func<bool> breakCond)
                    {
                        if (abortCombatCondition == null) return;

                        if (abortCombatConditionResetTimeout.IsTimedOut 
                            || (shouldRemoveAbortCombatCondition != null && shouldRemoveAbortCombatCondition()))
                        {
                            abortCombatCondition = null;
                            shouldRemoveAbortCombatCondition = null;
                            abortCombatConditionResetTimeout.ChangeTime(0);
                        }
                    }

                    /// <summary>
                    /// Условие прерывания боя
                    /// </summary>
                    private static AbortCombatConditionDelegate abortCombatCondition;
                    private static Func<bool> shouldRemoveAbortCombatCondition;

                    /// <summary>
                    /// Время до сброса <see cref="abortCombatCondition"/>
                    /// Таймер обновляется каждый раз при вызове <see cref="SetAbortCombatCondition"/>
                    /// </summary>
                    public static int AbortCombatConditionResetTime { get; set; } = 300_000;
                    private static readonly Timeout abortCombatConditionResetTimeout = new Timeout(0);

                    public static string AbortCombatCondition_DebugInfo()
                    {
                        if (abortCombatCondition is null)
                            return "Not set";
                        var subscriber = abortCombatCondition.Target;
                        var currentTarget = Astral.Logic.UCC.Core.CurrentTarget;
                        string abortCombatConditionStr = string.Concat("AbortCombatCondition: ", Environment.NewLine, 
                                             '\t', subscriber.GetType().FullName, '.', abortCombatCondition.Method.Name, Environment.NewLine,
                                             "\t\t(", subscriber.ToString(), ") ", Environment.NewLine,
                                             "\tResult = ", abortCombatCondition(ref currentTarget));
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
                        var tAstralCombats = typeof(Astral.Logic.NW.Combats);
                        var tPatch = typeof(Combats);

                        var originalCombatUnit = AccessTools.Method(tAstralCombats, nameof(Astral.Logic.NW.Combats.CombatUnit));
                        var prefixCombatUnit = AccessTools.Method(tPatch, nameof(Combats.PrefixCombatUnit));
                        var postfixCombatUnit = AccessTools.Method(tPatch, nameof(Combats.PostfixCombatUnit));

                        blAttackersList = tAstralCombats.GetStaticField<Func<List<string>>>("BLAttackersList");

                        if (originalCombatUnit != null)
                            ACTP0Patcher.Harmony.Patch(originalCombatUnit, new HarmonyMethod(prefixCombatUnit), new HarmonyMethod(postfixCombatUnit));
                    }
                }
            }
        }
    }
}
