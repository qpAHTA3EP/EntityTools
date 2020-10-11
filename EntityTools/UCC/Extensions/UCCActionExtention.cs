using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityTools.Reflection;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC.Extensions
{
    public static class UCCActionExtention
    {
        public static Entity GetTarget(this UCCAction @this)
        {
            if (@this is ApproachEntity approachEntity)
                return approachEntity.UnitRef;
            if (@this is DodgeFromEntity dodgeFromEntity)
                return dodgeFromEntity.UnitRef;
            if (@this is Spell spell
                && ReflectionHelper.GetPropertyValue(spell, "TargetEntity", out object result))
                return result as Entity;
            return EntityManager.LocalPlayer;
        }
    }
}
