using Infrastructure.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Infrastructure;

namespace EntityTools.Patches
{
    internal static class ComplexPatch_Quester
    {
        public static bool PatchesWasApplied { get; private set; }

        public static void Apply()
        {
            if (PatchesWasApplied)
                return;

            if (true)
            {
                var tPatch = typeof(ComplexPatch_Quester);
                var tAddAction = typeof(Astral.Quester.Forms.AddAction);

                var miGetTypes = AccessTools.Method(tAddAction, "GetTypes");
                var miPrefixGetPatch = AccessTools.Method(tPatch, nameof(PrefixGetTypes));

                try
                {
                    ACTP0Patcher.Harmony.Patch(miGetTypes, new HarmonyMethod(miPrefixGetPatch));
                    ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester)}' succeeded", true);
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester)}' failed", true);
                    ETLogger.WriteLine(LogType.Error, e.ToString());
                } 
            }

            PatchesWasApplied = true;
        }

        private static bool PrefixGetTypes(ref List<Type> __result)
        {
            __result = ACTP0Serializer.QuesterTypes;
            return false;
        }
    }
}
