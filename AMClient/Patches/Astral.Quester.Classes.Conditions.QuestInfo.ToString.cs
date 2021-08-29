﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astral.Quester.Classes.Conditions;

namespace AcTp0Tools.Patches
{
    /// <summary>
    /// Патч метода Astral.Quester.Classes.Conditions.QuestInfo.ToString()
    /// </summary>
    [HarmonyPatch(typeof(QuestInfo), "ToString")]
    public static class Patch_Quester_Classes_Conditions_QuestInfo_ToString
    {
        private static bool Prefix(QuestInfo __instance, ref string __result)
        {
            __result = string.Concat(nameof(QuestInfo) + ": ", __instance.Tested, " [",
                                     __instance.QuestID, "]");
            return false;
        }
    }
}