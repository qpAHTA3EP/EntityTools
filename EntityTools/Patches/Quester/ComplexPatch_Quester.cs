using ACTP0Tools.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace EntityTools.Patches
{
    internal class ComplexPatch_Quester
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
                    //Action unpatch = null;

                    ACTP0Patcher.Harmony.Patch(miGetTypes, new HarmonyMethod(miPrefixGetPatch));

                    //unpatch += () =>
                    //{
                    //    ETLogger.WriteLine(LogType.Error, $@"Unpatch method '{original_UccProfile_ToString}'", true);
                    //    AcTp0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_UccProfile_ToString,
                    //            prefix_UccProfile_ToString);
                    //}; 
                    ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester)}' succeeded", true);
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine($@"Patch '{nameof(ComplexPatch_Quester)}' failed", true);
                    //unpatch?.Invoke();
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
