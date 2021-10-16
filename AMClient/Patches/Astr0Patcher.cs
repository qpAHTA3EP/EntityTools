using HarmonyLib;

namespace AcTp0Tools.Patches
{
    public static class AcTp0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(AcTp0Tools));

        public static void Apply()
        {
            Harmony.PatchAll();

            //TODO заменить на алгоритм перебора вложенных типов
            AstralAccessors.Quester.Core.ApplyPatches();
            Astral_Functions_XmlSerializer_GetExtraTypes.ApplyPatches();
            
            //Часто вызывает ошибку чтения памяти игры, по следующей причине:
            //При вызове патча игровой клиент еще не загружен
            //MyNW_Internals_WorldDrawing.ApplyPatches();
        }
    }
}
