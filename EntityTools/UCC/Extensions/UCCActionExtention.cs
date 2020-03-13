using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Actions;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityTools.UCC.Actions;
using MyNW.Internals;
using EntityTools.Tools;
using EntityTools.Reflection;

namespace EntityTools.UCC.Extensions
{
    public static class UCCActionExtention
    {
        public static Entity GetTarget(this UCCAction @this)
        {
            if (@this is ApproachEntity approachEntity)
                return approachEntity.UnitRef;
            else if (@this is DodgeFromEntity dodgeFromEntity)
                return dodgeFromEntity.UnitRef;
            else if (@this is Spell spell
                && ReflectionHelper.GetPropertyValue(spell, "TargetEntity", out object result))
                    return result as Entity;
            else return EntityManager.LocalPlayer;
        }
    }
}
