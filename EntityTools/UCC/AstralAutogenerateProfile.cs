using System;
using System.Collections.Generic;
using Astral;
using Astral.Logic.NW;
using Astral.Logic.UCC;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using ns21;
using ns22;
using Core = Astral.Logic.UCC.Core;

// Оригинальный код Астрала для генерации ucc-профиля
#if false
namespace Astral.Logic.UCC
{
    /// <summary>
    /// AutoUCC
    /// </summary>
    class Entrypoint2
    {

        internal static void checkProfile()
        {
            if (Entrypoint2.lastPlayer != Class1.LocalPlayer.ContainerId)
            {
                Entrypoint2.lastClass = new AutoUCCEntry();
                Entrypoint2.lastProfile = string.Empty;
                Entrypoint2.lastPlayer = Class1.LocalPlayer.ContainerId;
            }
            AutoUCCEntry currentEntry = Class34.CurrentEntry;
            string validProfile = currentEntry.GetValidProfile();
            if (!(Entrypoint2.lastClass.Name != currentEntry.Name) && !(Entrypoint2.lastProfile != validProfile))
            {
                if (Entrypoint2.lastProfile == "Auto")
                {
                    Class34.smethod_3(false, false);
                }
                return;
            }
            Entrypoint2.lastClass = Class34.CurrentEntry;
            Entrypoint2.lastProfile = validProfile;
            if (Entrypoint2.lastProfile == "Auto")
            {
                Class34.smethod_3(true, false);
                return;
            }
            if (!string.IsNullOrEmpty(validProfile) && File.Exists(validProfile))
            {
                Logger.WriteLine("[AutoUCC] Load : " + validProfile);
                Core.Get.mProfil.Load(validProfile);
                return;
            }
            Logger.WriteLine("[AutoUCC] No valid profile for this class, auto generate...");
            Class34.smethod_3(true, false);
        }
    }
} 
#endif

namespace ns21
{
    internal static class Class34
    {
        private static Spell generate_ActionSpell(int slotInd)
        {
            Power powerBySlot = Powers.GetPowerBySlot(slotInd);
            Spell spell = new Spell();
            if (actions.ContainsKey(slotInd))
            {
                if (powerBySlot.PowerDef.InternalName == actions[slotInd].SpellID)
                {
                    return actions[slotInd];
                }

                spell = actions[slotInd];
            }
            else
            {
                actions.Add(slotInd, spell);
            }

            if (powerBySlot.IsValid)
            {
                spell.Conditions.Clear();
                spell.CoolDown = 0;
                spell.CastingTime = 0;
                PowerDef powerDef = powerBySlot.EntGetActivatedPower().PowerDef;
                spell.Enabled = true;
                spell.SpellID = powerBySlot.PowerDef.InternalName;
                if (slotInd > 0)
                {
                    spell.CoolDown = 2000;
                }

                UCCCondition ally_HealthPercent = new UCCCondition(Enums.Unit.MostInjuredAlly, Enums.ActionCond.HealthPercent,
                    Enums.Sign.Inferior, "50");
                UCCCondition mobCountAround_Player = new UCCCondition(Enums.Unit.Player, Enums.ActionCond.MobCountAround,
                    Enums.Sign.Superior, "1");
                UCCCondition mobCountAround_Target = new UCCCondition(Enums.Unit.Target, Enums.ActionCond.MobCountAround,
                    Enums.Sign.Superior, "1");
                UCCCondition mobCountAround_strongestAdd = new UCCCondition(Enums.Unit.StrongestAdd, Enums.ActionCond.MobCountAround,
                    Enums.Sign.Superior, "1");

                if (powerDef.TargetAffected.AffectFriend && !powerDef.TooltipDamagePowerDef.IsValid)
                {
                    // Условие для умения, нацеленного на дружественного игрока (хилящее умение)
                    spell.Target = Enums.Unit.MostInjuredAlly;
                    spell.Conditions.Add(ally_HealthPercent.Clone());
                }

                bool affectFoe = powerDef.TargetAffected.AffectFoe || powerDef.TooltipDamagePowerDef.IsValid;


                if (powerDef.RangeMax == 0f && powerDef.Radius > 0U && powerDef.TargetMain.Self)
                {
                    UCCCondition distance2Target = new UCCCondition(affectFoe ? Enums.Unit.Target : Enums.Unit.MostInjuredAlly, Enums.ActionCond.Distance,
                        Enums.Sign.Inferior, powerDef.Radius.ToString());

                    spell.Conditions.Add(distance2Target);
                }

                if (affectFoe && (slotInd == 1 || slotInd == 5 || slotInd == 6) 
                         && (powerDef.RangeSecondary > 0f 
                             || powerDef.CursorLocationTargetRadius > 0f 
                             || powerDef.Radius > 0U 
                             || (powerDef.TooltipDamagePowerDef.IsValid 
                                 && powerDef.TooltipDamagePowerDef.Radius > 0U)))
                {
                    if (powerDef.RangeMax == 0f && powerDef.RangeSecondary == 0f)
                    {
                        spell.Conditions.Add(mobCountAround_Player);
                    }
                    else if ((powerDef.RangeMax > 40f || powerDef.RangeSecondary > 40f) && slotInd != 1)
                    {
                        spell.Conditions.Add(mobCountAround_strongestAdd);
                    }
                    else
                    {
                        spell.Conditions.Add(mobCountAround_Target);
                    }
                }

                if (slotInd == 5 || slotInd == 6)
                {
                    if (!affectFoe)
                    {
                        spell.Target = Enums.Unit.Player;
                        spell.Conditions.Add(ally_HealthPercent.Clone());
                    }
                    else if (powerDef.RangeMax <= 40f && powerDef.RangeSecondary <= 40f)
                    {
                        spell.AddCondition(Enums.Unit.Target, Enums.ActionCond.HealthPercent, Enums.Sign.Superior,
                            "20");
                        spell.AddCondition(Enums.Unit.Target, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                    }
                    else
                    {
                        spell.Target = Enums.Unit.StrongestAdd;
                        spell.AddCondition(Enums.Unit.StrongestAdd, Enums.ActionCond.HealthPercent, Enums.Sign.Superior,
                            "20");
                        spell.AddCondition(Enums.Unit.StrongestAdd, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal,
                            "1");
                    }
                }

                if (((slotInd >= 2 && slotInd <= 4) || slotInd == 11) && powerDef.TimeRecharge > 15f &&
                    spell.Target == Enums.Unit.Target)
                {
                    spell.AddCondition(Enums.Unit.Target, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                }
            }
            else
            {
                spell.SpellID = string.Empty;
                spell.Enabled = false;
            }

            Class35.smethod_0(spell);
            return spell;
        }

        private static void generate_ActionDodge(AutoUCCEntry autoUCCEntry, bool bool_0)
        {
            Dodge dodge = new Dodge();
            Core.Get.mProfil.ActionsCombat.Add(dodge);
            AutoUCCProfile autoUCCProfile = Entrypoint2.Profile;
            if (autoUCCProfile == null)
            {
                autoUCCProfile = new AutoUCCProfile();
            }

            UCCCondition item =
                new UCCCondition(Enums.Unit.Player, Enums.ActionCond.ShouldDodge, Enums.Sign.Equal, "1");
            dodge.Conditions.Add(item);
            bool flag = true;
            if (autoUCCEntry.Mode == Enums.AutoUCCMode.Custom && !bool_0)
            {
                dodge.CoolDown = autoUCCEntry.DodgeCooldown;
                dodge.Direction = autoUCCEntry.DodgeType;
                flag = false;
            }
            else
            {
                dodge.CoolDown = autoUCCProfile.DodgeCooldown;
                dodge.Direction = Enums.DodgeDirection.DodgeSmart;
            }

            dodge.MovingTime = 200;
            CharClassCategory category = EntityManager.LocalPlayer.Character.Class.Category;
            if (category != CharClassCategory.OathboundPaladin)
            {
                if (category != CharClassCategory.GuardianFighter)
                {
                    if (category == CharClassCategory.GreatWeaponFigher)
                    {
                        dodge.MovingTime = 800;
                        return;
                    }

                    if (category == CharClassCategory.SourgeWarlock)
                    {
                        dodge.MovingTime = 800;
                        return;
                    }

                    return;
                }
            }

            if (flag)
            {
                dodge.Direction = Enums.DodgeDirection.DodgeSmartBack;
            }

            dodge.MovingTime = 1500;
        }

        private static void smethod_2()
        {
            UCCCondition ucccondition =
                new UCCCondition(Enums.Unit.Player, Enums.ActionCond.PlayerLevel, Enums.Sign.Superior, "9");
            switch (EntityManager.LocalPlayer.Character.Class.Category)
            {
                case CharClassCategory.TricksterRogue:
                {
                    Special special = new Special();
                    Core.Get.mProfil.ActionsCombat.Add(special);
                    special.Action = Enums.SpecialUCCAction.TabSpell;
                    special.CoolDown = 8000;
                    special.Conditions.Add(ucccondition.Clone());
                    special.AddCondition(Enums.Unit.Target, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                    special.AddCondition(Enums.Unit.Target, Enums.ActionCond.Distance, Enums.Sign.Inferior, "35");
                    special.AddCondition(Enums.Unit.Player, Enums.ActionCond.StealthPercent, Enums.Sign.Superior, "99");
                    return;
                }
                case CharClassCategory.GuardianFighter:
                {
                    Special special2 = new Special();
                    Core.Get.mProfil.ActionsCombat.Add(special2);
                    special2.Action = Enums.SpecialUCCAction.TabSpell;
                    special2.CoolDown = 6000;
                    special2.Conditions.Add(ucccondition.Clone());
                    special2.AddCondition(Enums.Unit.Target, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                    return;
                }
                case CharClassCategory.GreatWeaponFigher:
                {
                    Special special3 = new Special();
                    Core.Get.mProfil.ActionsCombat.Add(special3);
                    special3.Action = Enums.SpecialUCCAction.TabSpell;
                    special3.CoolDown = 6000;
                    special3.AddCondition(Enums.Unit.Player, Enums.ActionCond.DeterminationPercent, Enums.Sign.Superior,
                        "80");
                    special3.AddCondition(Enums.Unit.Player, Enums.ActionCond.HealthPercent, Enums.Sign.Inferior, "90");
                    special3.AddCondition(Enums.Unit.StrongestAdd, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                    special3.AddCondition(Enums.Unit.StrongestAdd, Enums.ActionCond.HealthPercent, Enums.Sign.Superior,
                        "20");
                    special3.Conditions.Add(ucccondition.Clone());
                    return;
                }
                case CharClassCategory.DevotedCleric:
                case (CharClassCategory) 5U:
                case CharClassCategory.ControlWizard:
                    break;
                case CharClassCategory.SourgeWarlock:
                {
                    Special special4 = new Special();
                    Core.Get.mProfil.ActionsCombat.Add(special4);
                    special4.Action = Enums.SpecialUCCAction.TabSpell;
                    special4.CoolDown = 6000;
                    special4.Conditions.Add(ucccondition.Clone());
                    special4.AddCondition(Enums.Unit.Target, Enums.ActionCond.IsHighHPMob, Enums.Sign.Equal, "1");
                    special4.AddCondition(Enums.Unit.Target, Enums.ActionCond.HealthPercent, Enums.Sign.Superior, "35");
                    return;
                }
                case CharClassCategory.HunterRanger:
                {
                    Special special5 = new Special();
                    special5.CoolDown = 4000;
                    Core.Get.mProfil.ActionsCombat.Add(special5);
                    special5.Action = Enums.SpecialUCCAction.TabSpell;
                    special5.AddCondition(Enums.Unit.Target, Enums.ActionCond.Distance, Enums.Sign.Inferior, "10");
                    special5.AddCondition(Enums.Unit.Player, Enums.ActionCond.HasAura, Enums.Sign.NotEqual,
                        "Ranger_Mode_Melee");
                    special5.Conditions.Add(ucccondition.Clone());
                    Special special6 = new Special();
                    special6.CoolDown = 4000;
                    Core.Get.mProfil.ActionsCombat.Add(special6);
                    special6.Action = Enums.SpecialUCCAction.TabSpell;
                    special6.AddCondition(Enums.Unit.Target, Enums.ActionCond.Distance, Enums.Sign.Superior, "10");
                    special6.AddCondition(Enums.Unit.Player, Enums.ActionCond.HasAura, Enums.Sign.Equal,
                        "Ranger_Mode_Melee");
                    special6.Conditions.Add(ucccondition.Clone());
                    return;
                }
                case CharClassCategory.OathboundPaladin:
                {
                    Special special7 = new Special();
                    Core.Get.mProfil.ActionsCombat.Add(special7);
                    special7.Action = Enums.SpecialUCCAction.TabSpell;
                    special7.CoolDown = 6000;
                    special7.Conditions.Add(ucccondition.Clone());
                    special7.AddCondition(Enums.Unit.Player, Enums.ActionCond.DivineCallPercent, Enums.Sign.Superior,
                        "34");
                    special7.AddCondition(Enums.Unit.Target, Enums.ActionCond.Distance, Enums.Sign.Inferior, "30");
                    special7.AddCondition(Enums.Unit.Target, Enums.ActionCond.TargetingPlayer, Enums.Sign.Equal, "0");
                    break;
                }
                default:
                    return;
            }
        }

        private static CharacterPath CurrentPath
        {
            get
            {
                using (List<AdditionalCharacterPath>.Enumerator enumerator = EntityManager.LocalPlayer.Character
                    .CurrentPowerTreeBuild.SecondaryPaths.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current.Path;
                    }
                }

                return EntityManager.LocalPlayer.Character.Path;
            }
        }

        internal static AutoUCCEntry CurrentEntry
        {
            get
            {
                if (Entrypoint2.Profile == null)
                {
                    return new AutoUCCEntry();
                }

                return Entrypoint2.Profile.GetEntryByClass(EntityManager.LocalPlayer.Character.Class)
                    .GetEntryByPath(Class34.CurrentPath);
            }
        }

        public static void generate_UccProfile(bool bool_0, bool bool_1 = false)
        {
            AutoUCCProfile autoUCCProfile = Entrypoint2.Profile;
            if (autoUCCProfile == null)
            {
                autoUCCProfile = new AutoUCCProfile();
            }

            AutoUCCEntry currentEntry = Class34.CurrentEntry;
            if (Environment.TickCount - Class34.int_0 > 10000)
            {
                bool_0 = true;
            }

            if (bool_0)
            {
                Class34.int_0 = Environment.TickCount;
                Class34.actions.Clear();
                Core.Get.mProfil.New();
                Core.Get.mProfil.UsePotions = autoUCCProfile.UsePotion;
                Core.Get.mProfil.SmartPotionUse = autoUCCProfile.SmartPotion;
                Core.Get.mProfil.PotionHealth = (int) autoUCCProfile.PotionHealth;
                if (currentEntry.Mode == Enums.AutoUCCMode.Custom && !bool_1)
                {
                    if (currentEntry.UseDodge)
                    {
                        Class34.generate_ActionDodge(currentEntry, bool_1);
                    }

                    Core.Get.mProfil.UsePotions = currentEntry.UsePotion;
                    Core.Get.mProfil.SmartPotionUse = currentEntry.SmartPotion;
                    Core.Get.mProfil.PotionHealth = (int) currentEntry.PotionHealth;
                }
                else if (autoUCCProfile.UseDodge)
                {
                    Class34.generate_ActionDodge(currentEntry, bool_1);
                }

                Class34.smethod_2();
            }

            List<UCCAction> actionsCombat = Core.Get.mProfil.ActionsCombat;
            if (EntityManager.LocalPlayer.Character.Class.Category == CharClassCategory.ControlWizard)
            {
                Spell item = Class34.generate_ActionSpell(11);
                if (!actionsCombat.Contains(item))
                {
                    actionsCombat.Add(item);
                }
            }

            for (int i = 6; i >= 0; i--)
            {
                Spell item2 = Class34.generate_ActionSpell(i);
                if (!actionsCombat.Contains(item2))
                {
                    actionsCombat.Add(item2);
                }
            }
        }

        private static Dictionary<int, Spell> actions = new Dictionary<int, Spell>();
        private static int int_0 = 0;
    }
}

namespace ns22
{
    internal static class Class35
    {
        public static void smethod_0(Spell spell)
        {
            bool flag = true;
            string spellID = spell.SpellID;
            switch (spellID)
            {
                case "Controller_Encounter_Conduitofice":
                    spell.Target = Enums.Unit.StrongestAdd;
                    break;
                case "Paladin_Atwill_Radiantstrike":
                    spell.Conditions.Clear();
                    spell.AddCondition(Enums.Unit.Target, Enums.ActionCond.MobCountAround, Enums.Sign.Superior, "2");
                    spell.CoolDown = 0;
                    break;
                case "Greatweapon_Atwill_Reapingstrike":
                    spell.CastingTime = 2000;
                    break;
                case "Greatweapon_Atwill_Wickedstrike":
                    spell.CoolDown = 0;
                    spell.Conditions.Clear();
                    spell.AddCondition(Enums.Unit.Player, Enums.ActionCond.MobCountAround,
                        Enums.Sign.Superior, "2");
                    break;
                default:
                    flag = false;
                    break;
            }

            if (flag)
            {
                Logger.WriteLine("Use UCC power database for this spell !");
            }
        }
    }
}
