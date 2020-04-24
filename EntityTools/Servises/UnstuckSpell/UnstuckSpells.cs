﻿#if DEBUG
//#define DEBUG_SPELLSTUCKMONITOR
#endif

#define UnstuckSpell_Tasks
using Astral;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityTools.Services
{
    public static class UnstuckSpells// : Astral.Logic.Classes.FSM.State
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
        //private static readonly string Paladin_Justicar_Mechanic = "Paladin_Special_Divinechampion_Feat_B"; //Аура "Щита"
        private static readonly string Paladin_Justicar_Mechanic = "Paladin_Special_Divinechampion"; //Аура "Щита"
        private static readonly string Paladin_Oathkeeper = "Paragon_Oathofdevotion"; //Парагон "Клятвохранитель" (Хил)
        private static readonly string Paladin_Oathkeeper_Mechanic = "Paladin_Special_Divinecall"; //Умение "Сила с выше"
        private static readonly string Paladin_Shift = "Paladin_Shift_Sanctuary";     // Аура "Блок"

        #region Реализация через System.Threading.Tasks
#if UnstuckSpell_Tasks
        /// <summary>
        /// Реализация через System.Threading.Tasks
        /// и Astral.Logic.UCC.API.AfterCallCombat
        /// </summary>
        private static Task monitor = null;
        private static bool afterCallCombatSubcription = false;

        public static void Start()
        {
            if (!afterCallCombatSubcription)
            {
                Astral.Logic.UCC.API.AfterCallCombat += ArterCallCombat;
                afterCallCombatSubcription = true;
                Astral.Logger.WriteLine($"{nameof(UnstuckSpells)} activated");
                ETLogger.WriteLine($"{nameof(UnstuckSpells)} activated");
            }
        }
        public static void Stop()
        {
                Astral.Logic.UCC.API.AfterCallCombat -= ArterCallCombat;
                afterCallCombatSubcription = false;
                Astral.Logger.WriteLine($"{nameof(UnstuckSpells)} deactivated");
                ETLogger.WriteLine($"{nameof(UnstuckSpells)} deactivated");
        }

        /// <summary>
        /// Эвент активируется в момент активации UCC в боевом режиме
        /// Запускает монитор "окончания боя"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private static void ArterCallCombat(object sender, Astral.Logic.UCC.API.AfterCallCombatEventArgs arg)
        {
            if (EntityTools.PluginSettings.UnstuckSpells.Active)
            {
                afterCallCombatSubcription = true;
                if (monitor == null || (monitor.Status != TaskStatus.Running))
                {
                    monitor = Task.Factory.StartNew(() => Work());
#if DEBUG_SPELLSTUCKMONITOR
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}::ArterCallCombat: Start Task");
#endif
                }
            }
            else
            {
                afterCallCombatSubcription = false;
                Astral.Logic.UCC.API.AfterCallCombat -= ArterCallCombat;
            }
        }

        /// <summary>
        /// Основной цикл
        /// </summary>
        private static void Work()
        {
#if DEBUG_SPELLSTUCKMONITOR
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]::Run() starts");
#endif
            Thread.Sleep(EntityTools.PluginSettings.UnstuckSpells.CheckInterval * 2);
            while (EntityTools.PluginSettings.UnstuckSpells.Active)
            {
                if (NeedToRun)
                {
                    // Отключение с предварительной проверкой
                    //DisableSpells_AurasCheck();

                    // Отключение без предварительных проверок
                    DisableSpells();
                    break;
                }
                Thread.Sleep(EntityTools.PluginSettings.UnstuckSpells.CheckInterval);
            }

#if DEBUG_SPELLSTUCKMONITOR
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]::Run() completed");
#endif
        }

        /// <summary>
        /// Проверка состояния боя и необходимости "отключать умения"
        /// </summary>
        private static bool NeedToRun
        {
            get
            {
                if (EntityTools.PluginSettings.UnstuckSpells.Active)
                {
                    bool result = !(EntityManager.LocalPlayer.InCombat
                            || Astral.Logic.NW.Attackers.InCombat
                            || (Astral.Logic.UCC.Core.CurrentTarget != null
                                && Astral.Logic.UCC.Core.CurrentTarget.IsValid
                                && !Astral.Logic.UCC.Core.CurrentTarget.IsDead
                                && Astral.Logic.UCC.Core.CurrentTarget.Location.Distance3DFromPlayer < Astral.Quester.API.CurrentProfile.KillRadius));
#if DEBUG_SPELLSTUCKMONITOR
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]::{nameof(NeedToRun)} = {result}");
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tLocalPlayer.InCombat:\t{EntityManager.LocalPlayer.InCombat}");
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tAttackers.InCombat:\t{Astral.Logic.NW.Attackers.InCombat}");
                    if (Astral.Logic.UCC.Core.CurrentTarget == null)
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget:\tnull");
                    else
                    {
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget:\t{Astral.Logic.UCC.Core.CurrentTarget.ToString()}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.IsValid:\t{Astral.Logic.UCC.Core.CurrentTarget.IsValid}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.IsDead:\t{Astral.Logic.UCC.Core.CurrentTarget.IsDead}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.Distance:\t{Astral.Logic.UCC.Core.CurrentTarget.Location.Distance3DFromPlayer}");
                    }
#endif
                    return result;
                }
                else Astral.Logic.UCC.API.AfterCallCombat -= ArterCallCombat;
                return false;
            }
        }

        /// <summary>
        /// Отключение активных умений в зависимости от класса
        /// Перед отключением проводится проверка ауры/активности
        /// </summary>
        private static void DisableSpells_AurasCheck()
        {
            var player = EntityManager.LocalPlayer;

#if DEBUG_SPELLSTUCKMONITOR
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}::DisableSpells_AurasCheck()");
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: Character class is '{player.Character.Class.Category}'");
#endif
            switch (player.Character.Class.Category)
            {
                case CharClassCategory.DevotedCleric:
                    {

                        // Флаги, предотвращающие повторное "выключение" умений
                        bool searchChanneldivinity = true;
                        bool searchArbiterMechanic = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Cleric_Arbiter;

                        foreach (AttribModNet mod in player.Character.Mods) // x64 
                        {
                            // Поиск ауры 'Devoted_Special_Channeldivinity'
                            if (searchChanneldivinity && mod.PowerDef.InternalName.StartsWith(Cleric_Channeldivinity))
                            {    // отключение скила 'Channeldivinity'
                                GameCommands.Execute("specialClassPower 0");
                                ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Deactivate SpecialClassPower[{Cleric_Channeldivinity}]");
                                searchChanneldivinity = false;
                            }

                            // Поиск ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                            if (searchArbiterMechanic && mod.PowerDef.InternalName.StartsWith(Cleric_Arbiter_Mechanic))
                            {
                                try
                                {
                                    GameCommands.Execute("specialClassPower 1");
                                    Thread.Sleep(100);
                                }
                                catch { }
                                finally
                                {
                                    GameCommands.Execute("specialClassPower 0");
                                    ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Convert '{Cleric_Arbiter_Mechanic}' to [Devinity]");
                                    searchArbiterMechanic = false;
                                }
                            }

                            if (!searchChanneldivinity && !searchArbiterMechanic)
                                break;
                        }

#if DEBUG_SPELLSTUCKMONITOR
                        //Если все Mods перебрали, а флаги не сброшены, значит соответствующие им умения неактивны
                        if (searchChanneldivinity)
                            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: SpecialClassPower[{Cleric_Channeldivinity}] not detected");
                        if (searchArbiterMechanic)
                            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: Aura '{Cleric_Arbiter_Mechanic}' not detected");
#endif

                        break;
                    }
                case CharClassCategory.OathboundPaladin:
                    {
                        // Если активно умение 'Paladin_Special_Divinepalisade'
                        //                        Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Special_Divinepalisade"));
                        //                        if (power != null && power.IsValid && power.IsActive)
                        //                        {
                        //                            GameCommands.Execute("specialClassPower 0");
                        //                            EntityToolsLogger.WriteLine($"{GetType().Name}: Deactivate 'Paladin_Special_Divinepalisade'");
                        //                        }
                        //#if DEBUG_SPELLSTUCKMONITOR
                        //                        else EntityToolsLogger.WriteLine($"[DEBUG_SPELLSTUCKMONITOR] {GetType().Name}: Power 'Paladin_Special_Divinepalisade' not active");
                        //#endif

                        // Если активно умение "Paladin_Shift_Sanctuary"
                        //                            Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith("Paladin_Shift_Sanctuary"));
                        //                            if (power != null && power.IsValid && power.IsActive)
                        //                            {
                        //                                GameCommands.Execute("tacticalSpecial 0");
                        //                                EntityToolsLogger.WriteLine($"{GetType().Name}: Deactivate TacticalSpecial[Paladin_Shift_Sanctuary]");
                        //                            }
                        //#if DEBUG_SPELLSTUCKMONITOR
                        //                            else EntityToolsLogger.WriteLine($"[DEBUG_SPELLSTUCKMONITOR] {GetType().Name}: TacticalSpecial[Paladin_Shift_Sanctuary] not active");
                        //#endif

                        // Флаги, предотвращающие повторное "выключение" умений
                        bool searchSanctuary = true;
                        bool searchDivinechampion = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Justicar;

                        foreach (AttribModNet mod in player.Character.Mods) // x64
                        {

                            // Поиск ауры 'Paladin_Shift_Sanctuary' 
                            if (searchSanctuary && mod.PowerDef.InternalName.StartsWith(Paladin_Shift))
                            {
                                GameCommands.Execute("tacticalSpecial 0");
                                ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Deactivate TacticalSpecial[{Paladin_Shift}]");
                                searchSanctuary = false;
                            }

                            // Поиск ауры 'Paladin_Special_Divinechampion_Feat_B' 
                            if (searchDivinechampion && mod.PowerDef.InternalName.StartsWith(Paladin_Justicar_Mechanic))
                            {
                                try
                                {
                                    GameCommands.Execute("specialClassPower 1");
                                    Thread.Sleep(100);
                                }
                                catch { }
                                finally
                                {
                                    GameCommands.Execute("specialClassPower 0");
                                    ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Deactivate SpecialClassPower[{Paladin_Justicar_Mechanic}]");
                                    searchDivinechampion = false;
                                }
                            }
                            if (!searchSanctuary && !searchDivinechampion)
                                break;
                        }
#if DEBUG_SPELLSTUCKMONITOR
                        //Если все Mods перебрали, а флаги не сброшены, значит соответствующие им умения неактивны
                        if (searchSanctuary)
                            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: Aura '{Paladin_Shift}' not detected");
                        if (searchDivinechampion)
                            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: Aura '{Paladin_Justicar_Mechanic}' not detected");
#endif

                        if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Oathkeeper)
                        {
                            // Поиск умения "Paladin_Special_Divinecall"
                            Power power = player.Character.Powers.Find(pow => pow.PowerDef.InternalName.StartsWith(Paladin_Oathkeeper_Mechanic));
                            if (power != null && power.IsValid && power.IsActive)
                            {
                                GameCommands.Execute("specialClassPower 0");
                                ETLogger.WriteLine(LogType.Debug, $"{nameof(UnstuckSpells)}: Deactivate SpecialClassPower[{Paladin_Oathkeeper_Mechanic}]");
                            }
#if DEBUG_SPELLSTUCKMONITOR
                            else EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: SpecialClassPower[{Paladin_Oathkeeper_Mechanic}] not active");
#endif
                        }
                        break;
                    }
                    //default:
                    //    GameCommands.Execute("specialClassPower 0");
                    //    EntityToolsLogger.WriteLine($"{GetType().Name}: Execute 'specialClassPower 0'");
                    //    GameCommands.Execute("tacticalSpecial 0");
                    //    EntityToolsLogger.WriteLine($"{GetType().Name}: Execute 'tacticalSpecial 0'");
                    //    break;
            }
        }

        /// <summary>
        /// Отключение активных умений в зависимости от класса
        /// Проверка ауры/активности умений не проводится (Кроме паладина)
        /// </summary>
        private static void DisableSpells()
        {
            var player = EntityManager.LocalPlayer;

#if DEBUG_SPELLSTUCKMONITOR
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}::DisableSpells_AurasCheck()");
            EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}: Character class is '{player.Character.Class.Category}'");
#endif
            switch (player.Character.Class.Category)
            {
                case CharClassCategory.DevotedCleric:
                    {
                        // Отключение скила 'Channeldivinity'
                        GameCommands.Execute("specialClassPower 0");
                        ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: '{player.Character.Class.Category}' deactivate SpecialClassPower[{Cleric_Channeldivinity}]");

                        // Ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                        if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Cleric_Arbiter)
                        {
                            try
                            {
                                GameCommands.Execute("specialClassPower 1");
                                Thread.Sleep(100);
                            }
                            catch { }
                            finally
                            {
                                GameCommands.Execute("specialClassPower 0");
                                ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: '{player.Character.Class.Category}' convert '{Cleric_Arbiter_Mechanic}' to [Devinity]");
                            }
                        }

                        break;
                    }
                case CharClassCategory.OathboundPaladin:
                    {
                        // Отключение 'Paladin_Shift_Sanctuary' 
                        GameCommands.Execute("tacticalSpecial 0");
                        ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Deactivate TacticalSpecial[{Paladin_Shift}]");

                        if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Oathkeeper)
                        {
                            // Paladin_Oathkeeper
                            // Отключаем "Paladin_Special_Divinecall"
                            GameCommands.Execute("specialClassPower 0");
                            ETLogger.WriteLine(LogType.Debug, $"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X")??"null"}]: '{player.Character.Class.Category}' deactivate SpecialClassPower[{Paladin_Oathkeeper_Mechanic}]");
                        }
                        else
                        {
                            // Paladin_Justicar
                            // Поиск ауры 'Paladin_Special_Divinechampion_Feat_B' 

                            foreach (AttribModNet mod in player.Character.Mods)
                            {
                                if (mod.PowerDef.InternalName.StartsWith(Paladin_Justicar_Mechanic))
                                {
                                    try
                                    {
                                        GameCommands.Execute("specialClassPower 1");
                                        Thread.Sleep(100);
                                    }
                                    catch { }
                                    finally
                                    {
                                        GameCommands.Execute("specialClassPower 0");
                                        ETLogger.WriteLine($"{nameof(UnstuckSpells)}[{Task.CurrentId?.ToString("X") ?? "null"}]: Deactivate SpecialClassPower[{Paladin_Justicar_Mechanic}]");
                                    }
                                    break;
                                }
                            }
                        }

                        break;
                    }
            }
        }
#endif
        #endregion

        #region Реализация через Astral.Quester.API.Engine.OnLeaveState
#if UnstuckSpell_OnLeaveState
        private static bool subscribed = false;
        public static bool Activate
        {
            get
            {
                return subscribed;
            }
            set
            {
                if (value)
                {
                    if (!subscribed)
                    {
                        Astral.Quester.API.Engine.OnLeaveState += DisableSpellsDelegate;
                        subscribed = true;
                    EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)} activated");
                    }
                }
                else
                {
                    Astral.Quester.API.Engine.OnLeaveState -= DisableSpellsDelegate;
                    subscribed = false;
                    EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)} deactivated");
                }
            }
        }

        /// <summary>
        /// Проверка состояния боя и необходимости "отключать умения"
        /// </summary>
        private static bool NeedToRun
        {
            get
            {
                if (subscribed)
                {
                    bool result = !(EntityManager.LocalPlayer.InCombat
                            || Astral.Logic.NW.Attackers.InCombat
                            /*|| (Astral.Logic.UCC.Core.CurrentTarget != null
                                && Astral.Logic.UCC.Core.CurrentTarget.IsValid
                                && !Astral.Logic.UCC.Core.CurrentTarget.IsDead
                                && Astral.Logic.UCC.Core.CurrentTarget.Location.Distance3DFromPlayer < Astral.Quester.Core.Profile.KillRadius)*/);
#if DEBUG_SPELLSTUCKMONITOR
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]::{nameof(NeedToRun)} = {result}");
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tLocalPlayer.InCombat:\t{EntityManager.LocalPlayer.InCombat}");
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tAttackers.InCombat:\t{Astral.Logic.NW.Attackers.InCombat}");
                    /*if (Astral.Logic.UCC.Core.CurrentTarget == null)
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget:\tnull");
                    else
                    {
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget:\t{Astral.Logic.UCC.Core.CurrentTarget.ToString()}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.IsValid:\t{Astral.Logic.UCC.Core.CurrentTarget.IsValid}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.IsDead:\t{Astral.Logic.UCC.Core.CurrentTarget.IsDead}");
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"\tUCC.Core.CurrentTarget.Distance:\t{Astral.Logic.UCC.Core.CurrentTarget.Location.Distance3DFromPlayer}");
                    }*/
#endif
                    return result;
                }
                return false;
            }
        }

        private static void DisableSpellsDelegate(object sender, Engine.EngineArgs e)
        {
            //if(e.state is Astral.Quester.FSM.States.Combat combat)
            if(e.state.DisplayName == "Combat")
            {
#if DEBUG_SPELLSTUCKMONITOR
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: LeaveState '{e.state.DisplayName}' ... check UnstuckSpells");
#endif
                if (NeedToRun)
                {
                    var player = EntityManager.LocalPlayer;

                    switch (player.Character.Class.Category)
                    {
                        case CharClassCategory.DevotedCleric:
                            {
#if DEBUG_SPELLSTUCKMONITOR
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: Character class is '{player.Character.Class.Category}'");
#endif
                                // Отключение скила 'Channeldivinity'
                                GameCommands.Execute("specialClassPower 0");
                                EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: '{player.Character.Class.Category}' deactivate SpecialClassPower[{Cleric_Channeldivinity}]");

                                // Поиск ауры 'Devoted_Mechanic_Dps_Scales_Radiant' или 'Devoted_Mechanic_Dps_Scales_Fire'
                                if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Cleric_Arbiter)
                                {
                                    GameCommands.Execute("specialClassPower 1");
                                    Thread.Sleep(100);
                                    GameCommands.Execute("specialClassPower 0");
                                    EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: '{player.Character.Class.Category}' convert '{Cleric_Arbiter_Mechanic}' to [Devinity]");
                                }

                                break;
                            }
                        case CharClassCategory.OathboundPaladin:
                            {
#if DEBUG_SPELLSTUCKMONITOR
                                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: Character class is '{player.Character.Class.Category}'");
#endif
                                // Отключение 'Paladin_Shift_Sanctuary' 
                                GameCommands.Execute("tacticalSpecial 0");
                                EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)}: Deactivate TacticalSpecial[{Paladin_Shift}]");

                                if (EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault()?.Path.PowerTree.Name == Paladin_Oathkeeper)
                                {
                                    // Paladin_Oathkeeper
                                    // Отключаем "Paladin_Special_Divinecall"
                                    GameCommands.Execute("specialClassPower 0");
                                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: '{player.Character.Class.Category}' deactivate SpecialClassPower[{Paladin_Oathkeeper_Mechanic}]");
                                }
                                else
                                {
                                    // Paladin_Justicar
                                    // Поиск ауры 'Paladin_Special_Divinechampion_Feat_B' 
                                    foreach (AttribModNet mod in player.Character.Mods)
                                    {
                                        if (mod.PowerDef.InternalName.StartsWith(Paladin_Justicar_Mechanic))
                                        {
                                            GameCommands.Execute("specialClassPower 1");
                                            Thread.Sleep(100);
                                            GameCommands.Execute("specialClassPower 0");
                                            EntityToolsLogger.WriteLine($"{nameof(UnstuckSpells)}: Deactivate SpecialClassPower[{Paladin_Justicar_Mechanic}]");
                                            break;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
#if DEBUG_SPELLSTUCKMONITOR
                else EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: Character is in Combat ... skip");
#endif
            }
            //#if DEBUG_SPELLSTUCKMONITOR
            //            else EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"{nameof(UnstuckSpells)}[{Thread.CurrentThread.ManagedThreadId.ToString("X")}]: LeaveState '{e.state.DisplayName}' ... skip");
            //#endif
        }
#endif
        #endregion
    }
}
