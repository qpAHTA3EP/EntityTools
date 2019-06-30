using Astral;
using Astral.Logic.Classes.FSM;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EntityPlugin.States
{
    public class SpellStuckMonitor : Astral.Logic.Classes.FSM.State
    {
        public override int Priority => 75;
        public override string DisplayName => GetType().Name;
        public override bool NeedToRun => CheckTO.IsTimedOut;
        public override int CheckInterval => 3000;
        public override bool StopNavigator => false;

        public override void Run()
        {
#if DEBUG
            Logger.WriteLine($"[DEBUG] {GetType().Name}.{nameof(Run)}");
#endif

            var player = EntityManager.LocalPlayer;
            if (!player.InCombat)
            {

                switch (player.Character.Class.Category)
                {
                    case CharClassCategory.DevotedCleric:
#if DEBUG
                        Logger.WriteLine($"[DEBUG] {GetType().Name}: '{player.Character.Class.Category}' detected");
#endif
                        // Поиск ауры 'Devoted_Special_Channeldivinity'
                        AttribMod mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Devoted_Special_Channeldivinity"));
                        if (mod != null && mod.IsValid)
                        {    // отключение скила 'Channeldivinity'
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Devoted_Special_Channeldivinity'");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Aura 'Devoted_Special_Channeldivinity' not detected");
#endif

                        // Поиск ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                        mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Devoted_Mechanic_Dps_Scales"));
                        if (mod != null && mod.IsValid)
                        {    
                            GameCommands.Execute("specialClassPower 1");
                            Thread.Sleep(500);
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Convert 'Devoted_Mechanic_Dps_Scales_Radiant/Fire' to [Devinity]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Auras 'Devoted_Mechanic_Dps_Scales_Radiant/Fire' not detected");
#endif
                        break;
                    case CharClassCategory.OathboundPaladin:
#if DEBUG
                        Logger.WriteLine($"[DEBUG] {GetType().Name}: '{player.Character.Class.Category}' detected");
#endif

                        // Если активно умение 'Paladin_Special_Divinepalisade'
                        Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinepalisade"));
                        if (power != null && power.IsValid && power.IsActive)
                        {
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Special_Divinepalisade'");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: 'Paladin_Special_Divinepalisade' not detected");
#endif

                        // Если активно умение "Paladin_Shift_Sanctuary"
                        power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Shift_Sanctuary"));
                        if (power != null && power.IsValid && power.IsActive)
                        {
                            GameCommands.Execute("tacticalSpecial 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Shift_Sanctuary'");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: 'Paladin_Shift_Sanctuary' not detected");
#endif

                        // Если активно умение "Paladin_Special_Divinecall"
                        power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinecall"));
                        if (power != null && power.IsValid && power.IsActive)
                        {
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Special_Divinecall'");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: 'Paladin_Special_Divinecall' not detected");
#endif
                        break;
                        //default:
                        //    GameCommands.Execute("specialClassPower 0");
                        //    Logger.WriteLine($"{GetType().Name}: Execute 'specialClassPower 0'");
                        //    GameCommands.Execute("tacticalSpecial 0");
                        //    Logger.WriteLine($"{GetType().Name}: Execute 'tacticalSpecial 0'");
                        //    break;
                }
            }
            CheckTO.Reset();
        }
    }
}
