using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Patches
{
    /// <summary>
    /// Патчер, содержащий список всех патчей
    /// </summary>
    internal static class Patcher
    {
        private static Patch Patch_Mapper =
            new Patch(
                typeof(Astral.Quester.Forms.MapperForm).GetMethod("Mapper",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic),
                typeof(Patches.Patch_Mapper).GetMethod(nameof(Patches.Patch_Mapper.Mapper),
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic));

        public static void Apply()
        {
            foreach(var field in typeof(Patcher).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if(field.GetValue(null) is Patch patch)
                {
                    patch.Inject();
                }
            }
        }
    }
}
