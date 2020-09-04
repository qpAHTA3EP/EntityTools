#if PATCH_ASTRAL && HARMONY
using HarmonyLib;
# endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace EntityTools.Patches.Mapper
{
#if false && PATCH_ASTRAL && HARMONY
    [HarmonyPatch(typeof(Astral.Logic.Classes.Map.GraphicsNW), "set_ImageWidth")]
    internal class Patch_GraphicsNW_set_ImageWidth
    {
        [HarmonyPrefix]
        internal bool set_ImageWidth(int value)
        {
            if(value <= 0)
            {
                // Значение некорректно
                Thread.CurrentThread.Abort();
                return false;
            }
            return true;
        }
    }
#endif
}
