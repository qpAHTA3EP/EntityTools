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
    internal static class ETPatcher
    {
        private static readonly Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly = new Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly();
        
        private static readonly Patch_Logic_UCC_Forms_AddClass_Show Patch_Logic_UCC_Forms_AddClass_Show = new Patch_Logic_UCC_Forms_AddClass_Show();

        private static readonly Patch_VIP_SealTraderEntity Patch_VIP_SealTraderEntity = new Patch_VIP_SealTraderEntity();
        private static readonly Patch_VIP_ProfessionVendorEntity Patch_VIP_ProfessionVendorEntity = new Patch_VIP_ProfessionVendorEntity();

        //private static readonly Patch_Astral_Quester_Core_Save Patch_Astral_Quester_Core_Save = new Patch_Astral_Quester_Core_Save();


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


                // Изменение алгоритмов навигации
                ComplexPatch_Navigation.Apply();
                // Подмена штатного окна Mapper'a
                ComplexPatch_Mapper.Apply();
                // Изменение команды квестера AddUCCAction и сопутствующие патчи
                ComplexPatch_Quester_UccEditing.Apply();
                // Перехват исключений в MyNW.Internals.WorldDrawing.\u0001()
                // Не работает, т.к. метод '\u0001()' запускается в статическом конструкторе класса WorldDrawing
                // то есть до применения патча
                //MyNW_Internals_WorldDrawing.ApplyPatches();
                //TODO Добавить патч AuraDetector

                try
                {
#if true
                    AcTp0Tools.Patches.AcTp0Patcher.Harmony.PatchAll();
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
