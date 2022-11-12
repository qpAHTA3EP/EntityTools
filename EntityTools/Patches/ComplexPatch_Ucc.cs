using System;
using System.Reflection;

using ACTP0Tools.Reflection;

using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Panels;

using EntityCore.Forms;

using EntityTools.UCC.Conditions;

using HarmonyLib;
// ReSharper disable InconsistentNaming

namespace EntityTools.Patches
{
    internal static class ComplexPatch_Ucc
    {
        public static bool PatchesWasApplied { get; private set; }

        private static Type tUccCondition;
        private static Type tPatch;
        private static MethodInfo original_IsOK;
        private static MethodInfo prefix_IsOK;

        private static MethodInfo original_Clone;
        private static MethodInfo prefix_Clone;

        private static Type tMainUcc;
        private static MethodInfo original_UccEditor;
        private static MethodInfo prefix_UccEditor;
        //private static MethodInfo MainUccRefreshAll;
        private static Action<object> MainUccRefreshAll;

        /// <summary>
        /// Патч <see cref="UCCCondition.IsOK"/> для перенаправления обработки в <see cref="ICustomUCCCondition.IsOK"/>.
        /// Данный патч нужен поскольку метод <see cref="UCCCondition.IsOK"/> не является виртуальным.
        /// </summary>
        /// <param name="__instance">Экземпляр ucc-условия</param>
        private static bool PrefixIsOK(UCCCondition __instance, ref bool __result, UCCAction refAction)
        {
            if (__instance is UCCGenericCondition genericCondition)
                return true;

            if (__instance is ICustomUCCCondition customUccCondition)
            {
                __result = customUccCondition.IsOK(refAction);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Патч <see cref="UCCCondition.Clone"/> для перенаправления обработки в <see cref="ICustomUCCCondition.Clone"/>.
        /// Данный патч нужен поскольку метод <see cref="UCCCondition.Clone"/> не является виртуальнным.
        /// </summary>
        /// <param name="__instance">Экземпляр ucc-условия</param>
        private static bool PrefixClone(UCCCondition __instance, ref UCCCondition __result)
        {
            if (__instance is ICustomUCCCondition customUccCondition)
            {
                __result = (UCCCondition)customUccCondition.Clone();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Патч <see cref="MainUCC"/> для подмены редактора ucc-профиля
        /// </summary>
        private static bool PrefixUccEditor(MainUCC __instance, object sender, EventArgs e)
        {
            if (EntityTools.Config.Patches.UccComplexPatch)
            {
                UccEditor.Edit(Astral.Logic.UCC.API.CurrentProfile, false);
                MainUccRefreshAll?.Invoke(__instance);
                return false;
            }

            return true;
        }        

        internal static void Apply()
        {
            if (!EntityTools.Config.Patches.UccComplexPatch || PatchesWasApplied) return;

            tUccCondition = typeof(UCCCondition);
            tPatch = typeof(ComplexPatch_Ucc);

            original_IsOK = AccessTools.Method(tUccCondition, nameof(UCCCondition.IsOK));
            if (original_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method '{nameof(UCCCondition.IsOK)}' not found", true);
                return;
            }

            prefix_IsOK = AccessTools.Method(tPatch, nameof(PrefixIsOK));
            if (prefix_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method '{nameof(PrefixIsOK)}' not found", true);
                return;
            }


            original_Clone = AccessTools.Method(tUccCondition, nameof(UCCCondition.Clone));
            if (original_Clone is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method '{nameof(UCCCondition.Clone)}' not found", true);
                return;
            }

            prefix_Clone = AccessTools.Method(tPatch, nameof(PrefixClone));
            if (prefix_Clone is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method '{nameof(PrefixClone)}' not found", true);
                return;
            }

            tMainUcc = typeof(MainUCC);

            original_UccEditor = AccessTools.Method(tMainUcc, "b_Editor_Click");
            if (original_UccEditor is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method 'MainUCC.b_Editor_Click' not found", true);
                return;
            }

            prefix_UccEditor = AccessTools.Method(tPatch, nameof(PrefixUccEditor));
            if (prefix_UccEditor is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' failed. Method '{nameof(PrefixUccEditor)}' not found", true);
                return;
            }


            Action unPatch = null;

            try
            {
                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_IsOK, new HarmonyMethod(prefix_IsOK));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_IsOK}'.", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_IsOK, prefix_IsOK);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_Clone, new HarmonyMethod(prefix_Clone));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Clone}'.", true);
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_Clone, prefix_Clone);
                };

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_UccEditor, new HarmonyMethod(prefix_UccEditor));
                MainUccRefreshAll = tMainUcc.GetAction("refreshAll");


                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Ucc)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(ComplexPatch_Ucc)}' failed", true);
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }

            PatchesWasApplied = true;
        }
    }
}
