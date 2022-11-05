using EntityTools.Patches.Logic.General;
using EntityTools.Patches.Mapper;
using EntityTools.Patches.Quester;
using EntityTools.Patches.UCC;
using System.Reflection;
using EntityTools.Servises.SlideMonitor;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace EntityTools.Patches
{
#if PATCH_ASTRAL
    /// <summary>
    /// Патчер, содержащий список всех патчей
    /// </summary>
    internal static partial class ETPatcher
    {
        private static readonly Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly = new Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly();
        
        private static readonly Patch_Logic_UCC_Forms_AddClass_Show Patch_Logic_UCC_Forms_AddClass_Show = new Patch_Logic_UCC_Forms_AddClass_Show();

        private static readonly Patch_VIP_SealTraderEntity Patch_VIP_SealTraderEntity = new Patch_VIP_SealTraderEntity();
        private static readonly Patch_VIP_ProfessionVendorEntity Patch_VIP_ProfessionVendorEntity = new Patch_VIP_ProfessionVendorEntity();

        // Патч метода выбора текущего индекса
        private static readonly Astral_Logic_General_GetNearestIndexInPositionList Astral_Logic_General_GetNearestIndexInPositionList = new Astral_Logic_General_GetNearestIndexInPositionList();


        static bool _applied;
        public static void Apply()
        {
            if (!_applied)
            {
                foreach (var field in typeof(ETPatcher).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (field.GetValue(null) is Patch patch)
                    {
                        patch.Inject();
                    }

                }


                SlideMonitor.Apply();

                //if (Assembly.ReflectionOnlyLoadFrom("AStar.dll").GetName().Version >= requiredAStar)
                {
                    // Изменение алгоритмов навигации
                    ComplexPatch_Navigation.Apply();
                    // Подмена штатного окна Mapper'a
                    ComplexPatch_Mapper.Apply(); 
                }

                // Изменение команды квестера AddUCCAction и сопутствующие патчи
                ComplexPatch_Quester_UccEditing.Apply();
                // Изменение окна добавления quester-команды/условия
                ComplexPatch_Quester.Apply();
                ComplexPatch_Quester_Editor.Apply();

                // Подмена окна выбора Ауры
                Patch_AuraDetector.Apply();

                // Патч методов UCCCondition
                ComplexPatch_Ucc.Apply();


                try
                {
#if true
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.PatchAll();
#else
                    var harmony = new HarmonyLib.Harmony(nameof(EntityTools));
                    harmony.PatchAll(); 
#endif
                }
                catch
                {
                    ETLogger.WriteLine(LogType.Error, "Harmony patches are failed!",true);
                }
                _applied = true;
            }
        }
    }
#endif
}
