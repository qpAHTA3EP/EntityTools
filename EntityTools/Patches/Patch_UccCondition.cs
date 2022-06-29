using Astral.Logic.UCC.Classes;
using EntityTools.UCC.Conditions;
using HarmonyLib;
using System;
using System.Reflection;

namespace EntityTools.Patches
{
    internal static class Patch_UccCondition
    {
        public static bool PatchesWasApplied { get; private set; }

        private static Type tUccCondition;
        private static Type tPatch;
        private static MethodInfo original_IsOK;
        private static MethodInfo prefix_IsOK;

        private static MethodInfo original_Clone;
        private static MethodInfo prefix_Clone;

        /// <summary>
        /// Патч <see cref="UCCCondition.IsOK"/> для перенаправления обработки в <see cref="ICustomUCCCondition.IsOK"/>.
        /// Данный патч нужен поскольку метод <see cref="UCCCondition.IsOK"/> не является виртуальным.
        /// </summary>
        /// <param name="__instance">Экземпляр ucc-условия</param>
        /// <param name="__result"></param>
        /// <param name="refAction"></param>
        /// <returns></returns>
        private static bool PrefixIsOK(UCCCondition __instance, ref bool __result, UCCAction refAction)
        {
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
        /// <param name="__result"></param>
        /// <returns></returns>
        private static bool PrefixClone(UCCCondition __instance, ref UCCCondition __result)
        {
            if (__instance is ICustomUCCCondition customUccCondition)
            {
                __result = (UCCCondition)customUccCondition.Clone();
                return false;
            }

            return true;
        }        

        internal static void Apply()
        {
            if (!EntityTools.Config.Patches.UccCondition || PatchesWasApplied) return;

            tUccCondition = typeof(UCCCondition);

            original_IsOK = AccessTools.Method(tUccCondition, nameof(UCCCondition.IsOK));
            if (original_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(Patch_UccCondition)}' failed. Method '{nameof(UCCCondition.IsOK)}' not found", true);
                return;
            }

            prefix_IsOK = AccessTools.Method(tPatch, nameof(PrefixIsOK));
            if (prefix_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(Patch_UccCondition)}' failed. Method '{nameof(PrefixIsOK)}' not found", true);
                return;
            }


            original_Clone = AccessTools.Method(tUccCondition, nameof(UCCCondition.Clone));
            if (original_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(Patch_UccCondition)}' failed. Method '{nameof(UCCCondition.Clone)}' not found", true);
                return;
            }

            prefix_Clone = AccessTools.Method(tPatch, nameof(PrefixClone));
            if (prefix_IsOK is null)
            {
                ETLogger.WriteLine($@"Patch '{nameof(Patch_UccCondition)}' failed. Method '{nameof(PrefixClone)}' not found", true);
                return;
            }
            Action unPatch = null;

            try
            {
                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_IsOK, new HarmonyMethod(prefix_IsOK));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_IsOK}'.", true);
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_IsOK, prefix_IsOK);
                };

                AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_Clone, new HarmonyMethod(prefix_Clone));

                ETLogger.WriteLine($@"Patch '{nameof(Patch_UccCondition)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(Patch_UccCondition)}' failed", true);
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }

            PatchesWasApplied = true;
        }
    }
}
