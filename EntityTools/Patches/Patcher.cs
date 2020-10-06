//#define HARMONY
#if PATCH_ASTRAL && HARMONY 
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
using EntityTools.Patches.UCC;
using EntityTools.Patches.Mapper;

namespace EntityTools.Patches
{
#if PATCH_ASTRAL
    /// <summary>
    /// Патчер, содержащий список всех патчей
    /// </summary>
    internal static class ETPatcher
    {
        /// <summary>
        /// Подмена штатного окна Mapper'a
        /// </summary>
        private static readonly Patch patchMapper = (EntityTools.PluginSettings.Mapper.Patch) ?
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
        /// Подмена ActionsPlayer.CheckAlly(..)
        /// </summary>
        private static readonly Patch_ActionsPlayer_CheckAlly patchActionsPlayerCheckAlly = new Patch_ActionsPlayer_CheckAlly();

        private static readonly Patch_AddClass_Show patchAddClassShow = new Patch_AddClass_Show();

        private static readonly Patch_VIP_SealTraderEntity patchVIPSealTraderEntity = new Patch_VIP_SealTraderEntity();

        private static readonly Patch_VIP_ProfessionVendorEntity patchVIPProfessionVendorEntity = new Patch_VIP_ProfessionVendorEntity();

#if true
        private static readonly Patch_Astral_Quester_Core_Save patchAstralQuesterCoreSave = new Patch_Astral_Quester_Core_Save();
#endif
#if true
        // Патч механизмов отрисовки Mapper'a
        private static readonly Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes patchAstralLogicClassesMapFunctionsPictureDrawMeshes = new Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes();
        private static readonly Patch_Astral_Navmesh_DrawRoad patchAstralNavmeshDrawRoad = new Patch_Astral_Navmesh_DrawRoad();
        private static readonly Patch_Astral_Navmesh_DrawHotSpots patchAstralNavmeshDrawHotSpots = new Patch_Astral_Navmesh_DrawHotSpots();
#endif

        static bool Applied = false;
        public static void Apply()
        {
            //TODO: Реализовать механизм отключения патчей (всех)
            // В первую очерещь эта касается патчей Patch_VIP_SealTraderEntity и Patch_VIP_ProfessionVendorEntity


            if (!Applied)
            {
#if true
                foreach (var field in typeof(ETPatcher).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.GetValue(null) is Patch patch
                        && patch != null)
                    {
                        patch.Inject();
                    }
#if false
                    else
                    {
                        ETLogger.WriteLine($"Failed to apply patch '{field.FieldType.Name}' named '{field.Name}'");
                    } 
#endif
                }
#else

                patchMapper?.Inject();

                patchActionsPlayerCheckAlly.Inject();

                PatchAddClassShow.Inject();
#endif

#if Patch_AstralXmlSerializer
                patchXmlSerializerGetExtraTypes.Inject();
#endif
#if HARMONY
                try
                {
                    var harmony = new Harmony(nameof(ETPatcher));
                    harmony.PatchAll();
                }
                catch
                {
                    ETLogger.WriteLine(LogType.Error, "Harmony patches are failed!",true);
                }
#endif
                Applied = true;
            }

        }
    }
#endif
}
