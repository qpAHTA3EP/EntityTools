#if HARMONY
using HarmonyLib; 
#else
//#define Patch_AstralXmlSerializer
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityTools.Patches
{
#if DEVELOPER
    /// <summary>
    /// Патчер, содержащий список всех патчей
    /// </summary>
    internal static class ETPatcher
    {
        /// <summary>
        /// Подмена штатного окна Mapper'a
        /// </summary>
        private static Patch Patch_Mapper = (EntityTools.PluginSettings.Mapper.Patch) ?
            new Patch(
                typeof(Astral.Quester.Forms.MapperForm).GetMethod("Open",
                    BindingFlags.Static | BindingFlags.Public),
                typeof(Mapper.MapperFormExt).GetMethod(nameof(Mapper.MapperFormExt.Open),
                    BindingFlags.Static | BindingFlags.Public))
            : null;

#if Patch_AstralXmlSerializer
        /// <summary>
        /// Подмена метода, формирующего список типов для сериализации профилей
        /// </summary>
        private static Patch Patch_AstralXmlSerializer = new Patch(
                    typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic/*, 
                        null, new Type[] { typeof(int) }, null*/),
                    typeof(XmlSerializer.AstralXmlSerializerPatch).GetMethod(nameof(XmlSerializer.AstralXmlSerializerPatch.GetExtraTypes),
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

#endif
        static bool Applied = false;
        public static void Apply()
        {
            if (!Applied)
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

#if Patch_AstralXmlSerializer
                Patch_AstralXmlSerializer?.Inject();
#endif
#if HARMONY
                var harmony = new Harmony(nameof(EntityToolsPatcher));
                harmony.PatchAll(); 
#endif
                Applied = true;
            }

        }
    }
#endif
}
