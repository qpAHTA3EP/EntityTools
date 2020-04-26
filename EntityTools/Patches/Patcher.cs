//#define HARMONY
#if HARMONY
using HarmonyLib; 
#else
//#define Patch_AstralXmlSerializer
#endif

using EntityTools.Reflection;
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
        private static Patch patchMapper = (EntityTools.PluginSettings.Mapper.Patch) ?
            new Patch(
                typeof(Astral.Quester.Forms.MapperForm).GetMethod("Open", ReflectionHelper.DefaultFlags),
                typeof(Mapper.MapperFormExt).GetMethod(nameof(Mapper.MapperFormExt.Open), ReflectionHelper.DefaultFlags))
            : null;

#if Patch_AstralXmlSerializer
        /// <summary>
        /// Подмена метода, формирующего список типов для сериализации профилей
        /// </summary>
#if false
		private static Patch patchXmlSerializerGetExtraTypes = new Patch(
            typeof(Astral.Functions.XmlSerializer).GetMethod("GetExtraTypes",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic/*, 
                null, new Type[] { typeof(int) }, null*/),
            typeof(Patch_XmlSerializer_GetExtraTypes).GetMethod(nameof(Patch_XmlSerializer_GetExtraTypes.GetExtraTypes),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));  
#else
        private static Patch_XmlSerializer_GetExtraTypes patchXmlSerializerGetExtraTypes = new Patch_XmlSerializer_GetExtraTypes();
#endif

#endif
        /// <summary>
        /// Подмена 
        /// </summary>
        private static Patch_ActionsPlayer_CheckAlly patchActionsPlayerCheckAlly = new Patch_ActionsPlayer_CheckAlly();

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

                patchMapper?.Inject();

                patchActionsPlayerCheckAlly.Inject();

#if Patch_AstralXmlSerializer
                patchXmlSerializerGetExtraTypes.Inject();
#endif
#if HARMONY
                var harmony = new Harmony(nameof(ETPatcher));
                harmony.PatchAll(); 
#endif
                Applied = true;
            }

        }
    }
#endif
}
