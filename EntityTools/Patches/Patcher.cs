﻿//#define HARMONY

using System.Reflection;
using Astral.Quester.Forms;
using EntityTools.Patches.Mapper;
using EntityTools.Patches.Navmesh;
using EntityTools.Patches.UCC;
using EntityTools.Reflection;
#if PATCH_ASTRAL && HARMONY
using HarmonyLib; 
#else
//#define Patch_AstralXmlSerializer
#endif

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
                typeof(MapperForm).GetMethod("Open", ReflectionHelper.DefaultFlags),
                typeof(MapperFormExt).GetMethod(nameof(MapperFormExt.Open), ReflectionHelper.DefaultFlags))
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
        private static readonly Patch_ActionsPlayer_CheckAlly Patch_ActionsPlayer_CheckAlly = new Patch_ActionsPlayer_CheckAlly();

        private static readonly Patch_AddClass_Show patchAddCPatch_AddClass_ShowlassShow = new Patch_AddClass_Show();

        private static readonly Patch_VIP_SealTraderEntity Patch_VIP_SealTraderEntity = new Patch_VIP_SealTraderEntity();

        private static readonly Patch_VIP_ProfessionVendorEntity Patch_VIP_ProfessionVendorEntity = new Patch_VIP_ProfessionVendorEntity();

#if true
        private static readonly Patch_Astral_Quester_Core_Save Patch_Astral_Quester_Core_Save = new Patch_Astral_Quester_Core_Save();
#endif
#if true
        // Патч механизмов отрисовки Mapper'a
        private static readonly Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes = new Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes();
        private static readonly Patch_Astral_Logic_Navmesh_DrawRoad Patch_Astral_Logic_Navmesh_DrawRoad = new Patch_Astral_Logic_Navmesh_DrawRoad();
        private static readonly Patch_Astral_Logic_Navmesh_DrawHotSpots Patch_Astral_Logic_Navmesh_DrawHotSpots = new Patch_Astral_Logic_Navmesh_DrawHotSpots();
#endif
#if true
        // Патч методов построения пути
        private static readonly Patch_Astral_Logic_Navmesh_getPath Patch_Astral_Logic_Navmesh_getPath = new Patch_Astral_Logic_Navmesh_getPath();
        private static readonly Patch_Astral_Logic_Navmesh_GenerateRoad Patch_Astral_Logic_Navmesh_GenerateRoad = new Patch_Astral_Logic_Navmesh_GenerateRoad();

        private static readonly Patch_Astral_Logic_Navmesh_TotalDistance Patch_Astral_Logic_Navmesh_TotalDistance = new Patch_Astral_Logic_Navmesh_TotalDistance();

        private static readonly Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition = new Patch_Astral_Logic_Navmesh_GetNearestNodeFromPosition();

        private static readonly Patch_Quester_Controllers_Road_GenerateRoadFromPlayer Patch_Quester_Controllers_Road_GenerateRoadFromPlayer = new Patch_Quester_Controllers_Road_GenerateRoadFromPlayer();
        private static readonly Patch_Quester_Controllers_Road_PathDistance Patch_Quester_Controllers_Road_PathDistance = new Patch_Quester_Controllers_Road_PathDistance();
#endif
        static bool _applied;
        public static void Apply()
        {
            //TODO: Реализовать механизм отключения патчей (всех)

            if (!_applied)
            {
                foreach (var field in typeof(ETPatcher).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.GetValue(null) is Patch patch)
                    {
                        patch.Inject();
                    }

                }

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
                _applied = true;
            }

        }
    }
#endif
}
