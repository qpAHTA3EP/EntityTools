using HarmonyLib;
using System;
using System.Reflection;

namespace EntityTools.Patches
{
    /// <summary>
    /// Патч, заменяющий штатное окно выбора ауры 'AuraDetector' на модифицированное 'AuraViewer'
    /// <see href="https://qpahta3ep.github.io/EntityToolsDocs/MainPanel/AuraViewer-RU.html"/>
    /// </summary>
    public static class Patch_AuraDetector
    {
        public static bool PatchesWasApplied { get; private set; }

        private static Type tAuraDetector;

        private static MethodInfo original_AuraDetector_GetAura;
        private static MethodInfo prefix_AuraDetector_GetAura;

        internal static void Apply()
        {
            if (!EntityTools.Config.Patches.AuraDetector || PatchesWasApplied) return;

            try
            {
                tAuraDetector = ACTP0Tools.Reflection.ReflectionHelper.GetTypeByName("\u0003.\u0004", true);

                if (tAuraDetector is null)
                {
                    ETLogger.WriteLine($@"Patch '{nameof(Patch_AuraDetector)}' failed. Class '{@"\u0003.\u0004"}' not found", true);
                    return;
                }

                //original_AuraDetector_GetAura = AccessTools.Method(tAuraDetector, "\u0001", new Type[] { });
                foreach (var methodInfo in tAuraDetector.GetMethods())
                {
                    if (methodInfo.Name == "\u0001"
                        && methodInfo.ReturnType == typeof(string)
                        && methodInfo.GetParameters().Length == 0)
                    {
                        original_AuraDetector_GetAura = methodInfo;
                        break;
                    }
                }
                if (original_AuraDetector_GetAura is null)
                {
                    ETLogger.WriteLine($@"Patch '{nameof(Patch_AuraDetector)}' failed. Method '{@"\u0001"}' not found", true);
                    return;
                }

                var tPatch = typeof(Patch_AuraDetector);
                prefix_AuraDetector_GetAura = AccessTools.Method(tPatch, nameof(GetAura));
                if (original_AuraDetector_GetAura is null)
                {
                    ETLogger.WriteLine($@"Patch '{nameof(Patch_AuraDetector)}' failed. Method '{nameof(GetAura)}' not found", true);
                    return;
                }

                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_AuraDetector_GetAura, new HarmonyMethod(prefix_AuraDetector_GetAura));

                ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Navigation)}' succeeded", true);
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }
            PatchesWasApplied = true;
        }

        public static bool GetAura(ref string __result)
        {
            if (!EntityTools.Core.UserRequest_SelectAuraId(ref __result))
            {
                __result = string.Empty;
            }
            return false;
        }
    }
}
