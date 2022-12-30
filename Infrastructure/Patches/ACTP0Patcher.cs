using HarmonyLib;

namespace Infrastructure.Patches
{
    public static class ACTP0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(Infrastructure));

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
