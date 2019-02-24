using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityPlugin.Conditions
{
    public class PlayerHealth : Astral.Quester.Classes.Condition
    {
        public float Value { get; set; }

        public Condition.Relation Sign { get; set; }

        public override bool IsValid
        {
            get
            {
                if (!EntityManager.LocalPlayer.IsValid)
                    return false;

                Character character = EntityManager.LocalPlayer.Character;
                if ( !character.IsValid )
                    return false;

                float HealthPercent = (character.AttribsBasic.MaxHealth > 0) ? 100 * character.AttribsBasic.Health / character.AttribsBasic.MaxHealth : 0;

                switch (Sign)
                {
                    case Relation.Equal:
                        return HealthPercent == Value;
                    case Relation.NotEqual:
                        return HealthPercent != Value;
                    case Relation.Inferior:
                        return HealthPercent < Value;
                    case Relation.Superior:
                        return HealthPercent > Value;
                    default:
                        return false;
                }
            }
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return string.Format("Check if PlayerHealth {0} to {1}", Sign, Value);
        }

         public override string TestInfos
        {
            get
            {
                if (!EntityManager.LocalPlayer.IsValid)
                    return "Palyer not valid";

                Character character = EntityManager.LocalPlayer.Character;
                if (!character.IsValid)
                    return "Palyer not valid";

                float HealthPercent = (character.AttribsBasic.MaxHealth > 0) ? character.AttribsBasic.Health / character.AttribsBasic.MaxHealth : 0;

                return string.Format("PlayerHealth is {0} %", HealthPercent);
            }
        }
    }
}
