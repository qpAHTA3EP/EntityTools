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

        public override int Priority => 90;
        public override string DisplayName => GetType().Name;
        private bool needToRun;
        public override bool NeedToRun
        {
            get
            {
                if(!needToRun)
                    needToRun = EntityManager.LocalPlayer.InCombat;
                return needToRun && CheckTO.IsTimedOut && !EntityManager.LocalPlayer.InCombat;
            }
        }

        public override int CheckInterval => 3000;
        public override bool StopNavigator => false;
        

        private static SpellStuckMonitor monitor = new SpellStuckMonitor();

        public static void Activate()
        {
            if (monitor == null)
                monitor = new SpellStuckMonitor();
            Astral.Quester.API.Engine.AddState(monitor);
            Logger.WriteLine("SpellStuckMonitor activated");
        }

        public static void Deactivate()
        {
            if(Astral.Quester.API.Engine.States.Remove(monitor))
                Logger.WriteLine("SpellStuckMonitor deactivated");
            else Logger.WriteLine("SpellStuckMonitor deactivated FAILURE!");
        }
        public override void Run()
        {
#if DEBUG
            Logger.WriteLine($"[DEBUG] Play {GetType().Name}.{nameof(Run)}()");
#endif

            var player = EntityManager.LocalPlayer;

            switch (player.Character.Class.Category)
            {
                case CharClassCategory.DevotedCleric:
                    {
#if DEBUG
                        Logger.WriteLine($"[DEBUG] {GetType().Name}: Character class is '{player.Character.Class.Category}'");
#endif
                        // Поиск ауры 'Devoted_Special_Channeldivinity'
                        AttribModNet mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Devoted_Special_Channeldivinity"));
                        if (mod != null && mod.IsValid)
                        {    // отключение скила 'Channeldivinity'
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate SpecialClassPower[Devoted_Special_Channeldivinity]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: SpecialClassPower[Devoted_Special_Channeldivinity] not detected");
#endif

                        // Поиск ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                        mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Devoted_Mechanic_Dps_Scales"));
                        if (mod != null && mod.IsValid)
                        {
                            GameCommands.Execute("specialClassPower 1");
                            Thread.Sleep(100);
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Convert 'Devoted_Mechanic_Dps_Scales_Radiant/Fire' to [Devinity]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Auras 'Devoted_Mechanic_Dps_Scales_Radiant/Fire' not detected");
#endif
                        break;
                    }
                case CharClassCategory.OathboundPaladin:
                    {
#if DEBUG
                        Logger.WriteLine($"[DEBUG] {GetType().Name}: Character class is '{player.Character.Class.Category}'");
#endif

                        // Если активно умение 'Paladin_Special_Divinepalisade'
                        //                        Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinepalisade"));
                        //                        if (power != null && power.IsValid && power.IsActive)
                        //                        {
                        //                            GameCommands.Execute("specialClassPower 0");
                        //                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Special_Divinepalisade'");
                        //                        }
                        //#if DEBUG
                        //                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Power 'Paladin_Special_Divinepalisade' not active");
                        //#endif

                        // Поиск ауры 'Paladin_Special_Divinechampion_Feat_B'
                        AttribModNet mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Paladin_Special_Divinechampion_Feat_B"));
                        if (mod != null && mod.IsValid)
                        {
                            GameCommands.Execute("specialClassPower 1");
                            Thread.Sleep(100);
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate SpecialClassPower[Paladin_Special_Divinechampion_Feat_B]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Aura 'Paladin_Special_Divinechampion_Feat_B' not detected");
#endif
                        // Поиск ауры 'Devoted_Special_Channeldivinity'
                        mod = player.Character.Mods.Find(x => x.PowerDef.InternalName.Contains("Paladin_Shift_Sanctuary"));
                        if (mod != null && mod.IsValid)
                        {
                            GameCommands.Execute("tacticalSpecial 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate TacticalSpecial[Paladin_Shift_Sanctuary]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: Aura 'Paladin_Shift_Sanctuary' not detected");
#endif

                        // Если активно умение "Paladin_Shift_Sanctuary"
//                            Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Shift_Sanctuary"));
//                            if (power != null && power.IsValid && power.IsActive)
//                            {
//                                GameCommands.Execute("tacticalSpecial 0");
//                                Logger.WriteLine($"{GetType().Name}: Deactivate TacticalSpecial[Paladin_Shift_Sanctuary]");
//                            }
//#if DEBUG
//                            else Logger.WriteLine($"[DEBUG] {GetType().Name}: TacticalSpecial[Paladin_Shift_Sanctuary] not active");
//#endif

                        // Если активно умение "Paladin_Special_Divinecall"
                        Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinecall"));
                        if (power != null && power.IsValid && power.IsActive)
                        {
                            GameCommands.Execute("specialClassPower 0");
                            Logger.WriteLine($"{GetType().Name}: Deactivate SpecialClassPower[Paladin_Special_Divinecall]");
                        }
#if DEBUG
                        else Logger.WriteLine($"[DEBUG] {GetType().Name}: SpecialClassPower[Paladin_Special_Divinecall] not active");
#endif
                        break;
                    }
                    //default:
                    //    GameCommands.Execute("specialClassPower 0");
                    //    Logger.WriteLine($"{GetType().Name}: Execute 'specialClassPower 0'");
                    //    GameCommands.Execute("tacticalSpecial 0");
                    //    Logger.WriteLine($"{GetType().Name}: Execute 'tacticalSpecial 0'");
                    //    break;
            }
            
            CheckTO.Reset();

            //сбрасываем флаг необходимости проверки
            needToRun = false;
#if DEBUG
            Logger.WriteLine($"[DEBUG] {GetType().Name}: Wait Combat mode.");
#endif
            }
        }
    }
