using Astral;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System.Linq;
using System.Threading;

namespace EntityTools.States
{
    public class SpellStuckMonitor : Astral.Logic.Classes.FSM.State
    {
        /// <summary>
        /// Строковые идентификаторы отслеживаемых параметров Клирика
        /// </summary>
        private static readonly string Cleric_Arbiter = "Paragon_Divineoracle";      //Парагон "Арбитр" (ДД) ? Player_Secondary_DivineOracle
        private static readonly string Cleric_Arbiter_Mechanic = "Devoted_Mechanic_Dps_Scales"; //Часть идентификатора ауры лучистого/огненного усиления
        private static readonly string Cleric_Devout = "Paragon_Anointedchampion";   //Парагон "Благочестивец" (Хил)
        private static readonly string Cleric_Channeldivinity = "Devoted_Special_Channeldivinity"; //Умение "Сила с выше"

        /// <summary>
        /// Строковые идентификаторы отслеживаемых параметров Паладина
        /// </summary>
        private static readonly string Paladin_Justicar = "Paragon_Oathofprotection"; //Парагон "Юстициар" (Танк)
        private static readonly string Paladin_Justicar_Mechanic = "Paladin_Special_Divinechampion_Feat_B"; //Аура "Щита"
        private static readonly string Paladin_Oathkeeper = "Paragon_Oathofdevotion"; //Парагон "Клятвохранитель" (Хил)
        private static readonly string Paladin_Oathkeeper_Mechanic = "Paladin_Special_Divinecall"; //Умение "Сила с выше"
        private static readonly string Paladin_Shift = "Paladin_Shift_Sanctuary";     // Аура "Блок"

        public override int Priority => 90;
        public override string DisplayName => GetType().Name;
        private bool needToRun;
        public override bool NeedToRun
        {
            get
            {
                if (!needToRun)
                    needToRun = EntityManager.LocalPlayer.InCombat;
#if DEBUG_SPELLSTUCKMONITOR
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}.{nameof(NeedToRun)}={needToRun && beforeStartEngineSubscribed && CheckTO.IsTimedOut && !EntityManager.LocalPlayer.InCombat}" +
                    $" because {nameof(needToRun)}[{needToRun}], {nameof(beforeStartEngineSubscribed)}[{beforeStartEngineSubscribed}]," +
                    $" Not{nameof(EntityManager.LocalPlayer.InCombat)}[{!EntityManager.LocalPlayer.InCombat}] ");
#endif
                return needToRun && beforeStartEngineSubscribed && CheckTO.IsTimedOut && !EntityManager.LocalPlayer.InCombat;
            }
        }
        public override int CheckInterval => 1000;
        public override bool StopNavigator => false;

        private static bool beforeStartEngineSubscribed = false;
        private static SpellStuckMonitor monitor = new SpellStuckMonitor();

        public static bool Activate
        {
            get
            {
                return beforeStartEngineSubscribed || Astral.Quester.API.Engine.States.Contains(monitor);
            }
            set
            {
                if (value)
                {
                    if (!beforeStartEngineSubscribed)
                    {
                        Astral.Quester.API.BeforeStartEngine += API_BeforeStartEngine;
                        Astral.Logic.UCC.API.AfterCallCombat += ArterCallCombat;
                        beforeStartEngineSubscribed = true;
                    }
                    Logger.WriteLine($"{typeof(SpellStuckMonitor).Name} activated");
                    if (Astral.Quester.API.Engine.Running)
                        LoadState();
                }
                else
                {
                    // Попытка выгрузки State во время выполнения вызывает исключение
                    //if (Astral.Quester.API.Engine.States.RemoveAll(state => state is SpellStuckMonitor) > 0)
                    //    Logger.WriteLine("SpellStuckMonitor deactivated");
                    //else Logger.WriteLine("SpellStuckMonitor deactivated FAILURE!");

                    Astral.Quester.API.BeforeStartEngine -= API_BeforeStartEngine;
                    Astral.Logic.UCC.API.AfterCallCombat -= ArterCallCombat;
                    beforeStartEngineSubscribed = false;
                    monitor.needToRun = false;
                    if (Astral.Quester.API.Engine.Running)
                        Logger.WriteLine($"{typeof(SpellStuckMonitor).Name} will be deactivated after the Astral will stop.");
                    else Logger.WriteLine($"{typeof(SpellStuckMonitor).Name} deactivated");
                }
            }
        }

        private static void LoadState()
        {
            if (!Astral.Quester.API.Engine.Running && !Astral.Quester.API.Engine.States.Contains(monitor))
                Astral.Quester.API.Engine.AddState(monitor);
        }

        private static void API_BeforeStartEngine(object sender, Astral.Logic.Classes.FSM.BeforeEngineStart e)
        {
            LoadState();
        }

        //Евент активируется в момент активации UCC в боевом режиме
        public static void ArterCallCombat(object sender, Astral.Logic.UCC.API.AfterCallCombatEventArgs arg)
        {
            monitor.needToRun = true;
        }

        public override void Run()
        {
#if DEBUG
            Logger.WriteLine(Logger.LogType.Debug, $"Play {GetType().Name}.{nameof(Run)}()");
#endif

            var player = EntityManager.LocalPlayer;

            switch (player.Character.Class.Category)
            {
                case CharClassCategory.DevotedCleric:
                    {
#if DEBUG_SPELLSTUCKMONITOR
                        Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Character class is '{player.Character.Class.Category}'");
#endif
                        // Флаги, предотвращающие повторное "выключение" умений
                        bool searchChanneldivinity = true;
                        bool searchArbiterMechanic = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Cleric_Arbiter;
#if X64
                        foreach (AttribModNet mod in player.Character.Mods) // x64 
#elif X32
                        foreach (AttribMod mod in player.Character.Mods) // x32
#endif

                        {
                            // Поиск ауры 'Devoted_Special_Channeldivinity'
                            if (searchChanneldivinity && mod.PowerDef.InternalName.StartsWith(Cleric_Channeldivinity))
                            {    // отключение скила 'Channeldivinity'
                                GameCommands.Execute("specialClassPower 0");
                                Logger.WriteLine($"{GetType().Name}: Deactivate SpecialClassPower[{Cleric_Channeldivinity}]");
                                searchChanneldivinity = false;
                            }

                            // Поиск ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                            if (searchArbiterMechanic && mod.PowerDef.InternalName.StartsWith(Cleric_Arbiter_Mechanic))
                            {
                                GameCommands.Execute("specialClassPower 1");
                                Thread.Sleep(100);
                                GameCommands.Execute("specialClassPower 0");
                                Logger.WriteLine($"{GetType().Name}: Convert '{Cleric_Arbiter_Mechanic}' to [Devinity]");
                                searchArbiterMechanic = false;
                            }

                            if (!searchChanneldivinity && !searchArbiterMechanic)
                                break;
                        }

#if DEBUG_SPELLSTUCKMONITOR
                        //Если все Mods перебрали, а флаги не сброшены, значит соответствующие им умения неактивны
                        if (searchChanneldivinity)
                            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: SpecialClassPower[{Cleric_Channeldivinity}] not detected");
                        if (searchArbiterMechanic)
                            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Aura '{Cleric_Arbiter_Mechanic}' not detected");
#endif

                        break;
                    }
                case CharClassCategory.OathboundPaladin:
                    {
#if DEBUG_SPELLSTUCKMONITOR
                        Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Character class is '{player.Character.Class.Category}'");
#endif
                        // Если активно умение 'Paladin_Special_Divinepalisade'
                        //                        Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinepalisade"));
                        //                        if (power != null && power.IsValid && power.IsActive)
                        //                        {
                        //                            GameCommands.Execute("specialClassPower 0");
                        //                            Logger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Special_Divinepalisade'");
                        //                        }
                        //#if DEBUG_SPELLSTUCKMONITOR
                        //                        else Logger.WriteLine($"[DEBUG_SPELLSTUCKMONITOR] {GetType().Name}: Power 'Paladin_Special_Divinepalisade' not active");
                        //#endif

                        // Если активно умение "Paladin_Shift_Sanctuary"
                        //                            Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Shift_Sanctuary"));
                        //                            if (power != null && power.IsValid && power.IsActive)
                        //                            {
                        //                                GameCommands.Execute("tacticalSpecial 0");
                        //                                Logger.WriteLine($"{GetType().Name}: Deactivate TacticalSpecial[Paladin_Shift_Sanctuary]");
                        //                            }
                        //#if DEBUG_SPELLSTUCKMONITOR
                        //                            else Logger.WriteLine($"[DEBUG_SPELLSTUCKMONITOR] {GetType().Name}: TacticalSpecial[Paladin_Shift_Sanctuary] not active");
                        //#endif

                        // Флаги, предотвращающие повторное "выключение" умений
                        bool searchSanctuary = true;
                        bool searchDivinechampion = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Justicar;
#if X64
                        foreach (AttribModNet mod in player.Character.Mods) // x64
#elif X32
                        foreach (AttribMod mod in player.Character.Mods) // x32
#endif
                        {

                            // Поиск ауры 'Paladin_Shift_Sanctuary' 
                            if (searchSanctuary && mod.PowerDef.InternalName.StartsWith(Paladin_Shift))
                            {
                                GameCommands.Execute("tacticalSpecial 0");
                                Logger.WriteLine($"{GetType().Name}: Deactivate TacticalSpecial[{Paladin_Shift}]");
                                searchSanctuary = false;
                            }

                            // Поиск ауры 'Paladin_Special_Divinechampion_Feat_B' 
                            if (searchDivinechampion && mod.PowerDef.InternalName.StartsWith(Paladin_Justicar_Mechanic))
                            {
                                GameCommands.Execute("specialClassPower 1");
                                Thread.Sleep(100);
                                GameCommands.Execute("specialClassPower 0");
                                Logger.WriteLine($"{GetType().Name}: Deactivate SpecialClassPower[{Paladin_Justicar_Mechanic}]");
                                searchDivinechampion = false;
                            }
                            if (!searchSanctuary && !searchDivinechampion)
                                break;
                        }
#if DEBUG_SPELLSTUCKMONITOR
                        //Если все Mods перебрали, а флаги не сброшены, значит соответствующие им умения неактивны
                        if (searchSanctuary)
                            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Aura '{Paladin_Shift}' not detected");
                        if (searchDivinechampion)
                            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Aura '{Paladin_Justicar_Mechanic}' not detected");
#endif

                        if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Oathkeeper)
                        {
                            // Поиск умения "Paladin_Special_Divinecall"
                            Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith(Paladin_Oathkeeper_Mechanic));
                            if (power != null && power.IsValid && power.IsActive)
                            {
                                GameCommands.Execute("specialClassPower 0");
                                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Deactivate SpecialClassPower[{Paladin_Oathkeeper_Mechanic}]");
                            }
#if DEBUG_SPELLSTUCKMONITOR
                            else Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: SpecialClassPower[{Paladin_Oathkeeper_Mechanic}] not active");
#endif
                        }
                        break;
                    }
                    //default:
                    //    GameCommands.Execute("specialClassPower 0");
                    //    Logger.WriteLine($"{GetType().Name}: Execute 'specialClassPower 0'");
                    //    GameCommands.Execute("tacticalSpecial 0");
                    //    Logger.WriteLine($"{GetType().Name}: Execute 'tacticalSpecial 0'");
                    //    break;
            }

            //сбрасываем таймер проверки
            CheckTO.Reset();

            //сбрасываем флаг необходимости проверки
            needToRun = false;
#if DEBUG_SPELLSTUCKMONITOR
            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Wait Combat mode.");
#endif
        }
    }
}
