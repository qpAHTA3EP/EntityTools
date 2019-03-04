using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityPlugin.Conditions
{
    [Serializable]
    public class PlayerHealth : Condition
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
                if (character.IsValid)
                {
                    switch (Sign)
                    {
                        case Relation.Equal:
                            return character.AttribsBasic.HealthPercent == Value;
                        case Relation.NotEqual:
                            return character.AttribsBasic.HealthPercent != Value;
                        case Relation.Inferior:
                            return character.AttribsBasic.HealthPercent < Value;
                        case Relation.Superior:
                            return character.AttribsBasic.HealthPercent > Value;
                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        public override void Reset()
        {
        }

        public override string ToString()
        {
            return $"Check if {GetType().Name} {Sign} to {Value}";
        }

         public override string TestInfos
        {
            get
            {
                if (!EntityManager.LocalPlayer.IsValid)
                    return "'LocalPlayer' not valid";

                Character character = EntityManager.LocalPlayer.Character;
                if (!character.IsValid)
                    return "'LocalPlayer.Character' not valid";

                return $"{GetType().Name} is {character.AttribsBasic.HealthPercent} %";
            }
        }
    }
}
