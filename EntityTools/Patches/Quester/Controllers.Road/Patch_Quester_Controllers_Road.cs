using System;
using System.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using AcTp0Tools;
using Astral;
using HarmonyLib;
using EntityTools.Patches.Logic.Navmesh;

namespace EntityTools.Patches.Quester.Controllers.Road
{
    internal static class Patch_Quester_Controllers_Road
    {
        private static readonly MethodInfo original_PathDistance;
        private static readonly MethodInfo original_GenerateRoadFromPlayer;
        private static readonly MethodInfo prepatch_PathDistance;
        private static readonly MethodInfo prepatch_GenerateRoadFromPlayer;

        static Patch_Quester_Controllers_Road()
        {
            var tRoad = typeof(Astral.Quester.Controllers.Road);
            original_PathDistance = AccessTools.Method(tRoad, "PathDistance");
            original_GenerateRoadFromPlayer = AccessTools.Method(tRoad, "GenerateRoadFromPlayer");
            var tPatch = typeof(Patch_Quester_Controllers_Road);
            prepatch_PathDistance = AccessTools.Method(tPatch, nameof(PathDistance));
            prepatch_GenerateRoadFromPlayer = AccessTools.Method(tPatch, nameof(GenerateRoadFromPlayer));
        }

        internal static void Patch()
        {
            if (!EntityTools.Config.Patches.Navigation) return;


            if (original_PathDistance != null && prepatch_PathDistance != null
                && original_GenerateRoadFromPlayer != null
                && prepatch_GenerateRoadFromPlayer != null)
            {
                Action unpatch = null;

                try
                {
                    var patch_PathDistance = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_PathDistance, new HarmonyMethod(prepatch_PathDistance));
                    unpatch = () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_PathDistance, prepatch_PathDistance);

                    var patch_GenerateRoadFromPlayer = AcTp0Tools.Patches.AcTp0Patcher.Harmony.Patch(original_GenerateRoadFromPlayer, new HarmonyMethod(prepatch_GenerateRoadFromPlayer));
                    unpatch += () => AcTp0Tools.Patches.AcTp0Patcher.Harmony.Unpatch(original_GenerateRoadFromPlayer, prepatch_GenerateRoadFromPlayer);
                }
                catch (Exception)
                {
                    unpatch?.Invoke();
                    throw;
                }

            }
        }

    }
}
