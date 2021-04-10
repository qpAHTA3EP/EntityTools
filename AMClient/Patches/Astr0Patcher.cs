using HarmonyLib;

namespace AcTp0Tools.Patches
{
    public static class AcTp0Patcher
    {
        public static Harmony Harmony { get; } = new Harmony(nameof(AcTp0Tools));
    }
}
