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
        /// <summary>
        /// Подмена штатного окна Mapper'a
        /// </summary>
        private static Patch Patch_Mapper = (EntityTools.PluginSettings.Mapper.Patch) ?
            new Patch(
                typeof(Astral.Quester.Forms.MapperForm).GetMethod("Open",
                    BindingFlags.Static | BindingFlags.Public),
                typeof(Patches.Mapper.MapperFormExt).GetMethod(nameof(Patches.Mapper.MapperFormExt.Open),
                    BindingFlags.Static | BindingFlags.Public)) 
            : null;

        public static void Apply()
        {
            //foreach(var field in typeof(Patcher).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            //{
            //    if (field.GetValue(null) is Patch patch
            //        && patch != null)
            //    {
            //        patch.Inject();
            //    }
            //}

            Patch_Mapper?.Inject();
        }
    }
}
