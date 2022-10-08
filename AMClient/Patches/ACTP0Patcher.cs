using HarmonyLib;

namespace ACTP0Tools.Patches
{
    public static class ACTP0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(ACTP0Tools));

        public static void Apply()
        {
            Harmony.PatchAll();

            //TODO заменить на алгоритм перебора вложенных типов
            AstralAccessors.Quester.Core.ApplyPatches();
            ACTP0Serializer.ApplyPatches();
            Astral_Core_Before3DDraw.ApplyPatches();
        }
    }
}
