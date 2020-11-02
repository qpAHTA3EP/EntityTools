using MyNW.Classes;

namespace EntityTools.Patches.Logic.Classes.FSM.Navigation
{
    public static class CharacterEx
    {
        public static bool HasAura(this Character @this, string auraInternalName)
        {
            return !string.IsNullOrEmpty(auraInternalName) && @this.Mods.Exists((AttribModNet m) => m.PowerDef.InternalName.Contains(auraInternalName));
        }

        public static bool HasAura(this Character @this, uint pPowerDef)
        {
            return pPowerDef > 0u && @this.Mods.Exists((AttribModNet m) => m.pPowerDef == pPowerDef);
        }
    }
}
