
using System;
using MyNW.Internals;
using Astral.Classes;
using MyNW.Classes;
using EntityTools.Enums;

#if PATCH_ASTRAL && HARMONY
using HarmonyLib;
#endif

namespace EntityTools.Servises.SlideMonitor
{
#if HARMONY
    /// <summary>
    /// Патч getter'a свойства Astral.Logic.Classes.FSM.Navigation.ChangeWPDist
    /// </summary>
    [HarmonyPatch(typeof(Astral.Logic.Classes.FSM.Navigation), "get_ChangeWPDist")] 
    internal class HarmonyPatch_Astral_Logic_Classes_FSM_Navigation_ChangeWPDist
    {
        [HarmonyPrefix] 
        internal static bool get_ChangeWPDist(out double __result)
        {
            // TODO: Проверять карту, на которой находится персонаж, чтобы исключить лишние поиски ауры и т.п.
            // "Плавание под парусом" - только в "Море льда" и в "Сошенстар"
            // "Скользкая земля" - на "Перевале в IWD", на зимнем празднике, в Иллюзионисте и еще хз 
            // "Адская машина" - Авернсус (?)
            
            var state = EntityTools.Config.SlideMonitor.State;

            bool isMounted = EntityManager.LocalPlayer.CostumeRef.pMountCostume > 0;
            if (state == SlideMonitorState.Disabled)
            {
                if (isMounted)
                {
                    __result = Astral.API.CurrentSettings.MountedChangeWPDist;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: (IsMounted) => {__result}");
#endif
                }
                else
                {
                    __result = Astral.API.CurrentSettings.ChangeWaypointDist;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: (UnMounted) => {__result}");
#endif
                }
                return false;
            }

            int id = Astral.API.AttachedGameProcess.Id;
            uint playerId = EntityManager.LocalPlayer.ContainerId;
            if(attachedGameProcessId != id || playerContainerId != playerId)
            {
                boatAura = null;
                slipperyAura = null;
                infernalMachineAura = null;
                carAura = null;
                timeout.ChangeTime(0);
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: Reset");
#endif
            }
            attachedGameProcessId = id;
            playerContainerId = playerId;

            int filter = EntityTools.Config.SlideMonitor.BoatFilter;
            
            if (timeout.IsTimedOut)
            {
                if (state == SlideMonitorState.ActivateByAura)
                {
                    // Определяем скольжение по ауре
                    cachedWPD = CheckAuras();
                }
                else
                {
                    // Определяем скольжение по средству передвижения
                    if (isMounted)
                    {
                        var mountDef = EntityManager.LocalPlayer.GetMountCostume();
                        if (string.IsNullOrEmpty(mountDef.InternalName))
                        {
                            EntityTools.Config.SlideMonitor.State = SlideMonitorState.ActivateByAura;
                            cachedWPD = Astral.API.CurrentSettings.MountedChangeWPDist;
#if DEBUG && ChangeWPDist_LOG
                            ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: GetMountCostume failed. Switch to 'ActivateByAura'");
#endif
                        }
                        else
                        {
                            var mType = mountDef.Type;
                            switch (mType)
                            {
                                case MountType.None:

#if false
                                    // Скользский грунт
                                    if (pGroundAura == 0u)
                                    {
                                        AttribModNet attribModNet = EntityManager.LocalPlayer.Character.Mods.Find((AttribModNet m) => m.PowerDef.InternalName.Contains("Volume_Ground_Slippery"));
                                        pGroundAura = (attribModNet != null) ? attribModNet.pPowerDef : 0u;
                                    }
                                    if (EntityManager.LocalPlayer.Character.HasAura(pGroundAura))
                                    {
                                        cachedWPD = 50.0;
                                    }
                                    else cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist;
#else
                                    cachedWPD = CheckAuras();
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (None) => {cachedWPD}");
#endif
#endif
                                    break;
                                case MountType.BoatWhite:
                                    cachedWPD = filter * 0.75d;
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (BoatWhite) => {cachedWPD}");
#endif
                                    break;
                                case MountType.BoatGreen:
                                    cachedWPD = filter * 1d;
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (BoatGreen) => {cachedWPD}");
#endif
                                    break;
                                case MountType.BoatPurple:
                                    cachedWPD = filter * 1.5d;
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (BoatPurple) => {cachedWPD}");
#endif
                                    break;
                                case MountType.InfernalCar:
                                    cachedWPD = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (InfernalCar) => {cachedWPD}");
#endif
                                    break;
                                case MountType.Mount:
                                    cachedWPD = Astral.API.CurrentSettings.MountedChangeWPDist;
#if DEBUG && ChangeWPDist_LOG
                                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ActivateByMountType (Mount) => {cachedWPD}");
#endif
                                    break;
                            }
                        }
                    }
                    else
                    {
                        cachedWPD = CheckAuras();
#if DEBUG && ChangeWPDist_LOG
                        ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: UnMounted. CheckAuras => {cachedWPD}");
#endif
                    }
                }
                timeout.ChangeTime(EntityTools.Config.SlideMonitor.Timeout);
            }
#if DEBUG && ChangeWPDist_LOG
            else ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: Cached => {cachedWPD}");
#endif

            __result = Math.Min(cachedWPD, Math.Max(Astral.Quester.API.Engine.Navigation.LastWaypoint.Distance2DFromPlayer, 3.0));

            return false;
        }

#if false
        [HarmonyPrefix]
        internal static bool get_ChangeWPDist_2(out double __result)
        {
            var state = EntityTools.Config.SlideMonitor.State;

            bool isMounted = EntityManager.LocalPlayer.CostumeRef.pMountCostume > 0;
            if (state == SlideMonitorState.Disabled)
            {
                if (isMounted)
                {
                    __result = Astral.API.CurrentSettings.MountedChangeWPDist;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: (IsMounted) => {__result}");
#endif
                }
                else
                {
                    __result = Astral.API.CurrentSettings.ChangeWaypointDist;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: (UnMounted) => {__result}");
#endif
                }
                return false;
            }

            int id = Astral.API.AttachedGameProcess.Id;
            uint playerId = EntityManager.LocalPlayer.ContainerId;
            if (attachedGameProcessId != id || playerContainerId != playerId)
            {
                boatAura = null;
                slipperyAura = null;
                infernalMachineAura = null;
                carAura = null;
                timeout.ChangeTime(0);
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: Reset");
#endif
            }
            attachedGameProcessId = id;
            playerContainerId = playerId;

            int filter = EntityTools.Config.SlideMonitor.BoatFilter;


            if (timeout.IsTimedOut)
            {
                if (isMounted)
                {
                    string mountCostume = EntityManager.LocalPlayer.CostumeRef.GetMountCostumeInternalName();
                    if (!string.IsNullOrEmpty(mountCostume))
                    {

                    }
                }

            }
            return false;
        }

        internal static double GetWPDistanceCombined()
        {

        } 
#endif

        private static double CheckAuras()
        {
            int filter = EntityTools.Config.SlideMonitor.BoatFilter;
            double result = Astral.API.CurrentSettings.ChangeWaypointDist;

            // Аура скользкой земли
            // Может присутствовать и работать как в спешеном, так и верховом положении
            bool isValid = slipperyAura != null && slipperyAura.IsValid && slipperyAura.PowerDef.InternalName.StartsWith(SLIPPERY_AURA);
            if (isValid)
            {
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: '{SLIPPERY_AURA}' is valid => {filter}");
#endif
                return filter;
            }
            // Аура "плавание под парусом"
             isValid = boatAura != null && boatAura.IsValid && boatAura.PowerDef.InternalName.StartsWith(BOAT_AURA);
            if (isValid)
            {
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: '{BOAT_AURA}' is valid => {filter}");
#endif
                return filter;
            }

            // Аура "адской машины"
            isValid = infernalMachineAura != null && infernalMachineAura.IsValid &&
                      infernalMachineAura.PowerDef.InternalName.StartsWith(INFERNAL_MACHINE_AURA);
            if (isValid)
            {
                result = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: '{INFERNAL_MACHINE_AURA}' is valid => {result}");
#endif
                return result;
            }

            // Аура "адской машины"
            isValid = carAura != null && carAura.IsValid &&
                      carAura.PowerDef.InternalName.StartsWith(CAR_AURA);
            if (isValid)
            {
                result = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
#if DEBUG && ChangeWPDist_LOG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: '{CAR_AURA}' is valid => {result}");
#endif
                return result;
            }

            foreach (var aura in EntityManager.LocalPlayer.Character.Mods)
            {
                var inName = aura.PowerDef.InternalName;
                if (inName.StartsWith(SLIPPERY_AURA))
                {
                    slipperyAura = aura;
                    result = filter;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: Found '{SLIPPERY_AURA}' => {result}");
#endif
                    break;
                }
                if (inName.StartsWith(BOAT_AURA))
                {
                    boatAura = aura;
                    result = filter;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: Found '{BOAT_AURA}' => {result}");
#endif
                    break;
                }
                if (inName.StartsWith(INFERNAL_MACHINE_AURA))
                {
                    infernalMachineAura = aura;
                    result = filter * 4d;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: Found '{INFERNAL_MACHINE_AURA}' => {result}");
#endif
                    break;
                }
                if (inName.StartsWith(CAR_AURA))
                {
                    carAura = aura;
                    result = filter * 4d;
#if DEBUG && ChangeWPDist_LOG
                    ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist::CheckAuras: Found '{CAR_AURA}' => {result}");
#endif
                    break;
                }
            }

            return result;
        }

        #region Данные
        private static int attachedGameProcessId = 0;
        private static uint playerContainerId = 0;

        private static readonly Timeout timeout = new Timeout(0);

#if false
        private const string MOUNT_AURA = "Mount_Base_Universal";
#endif
        // Высота уровня воды 
        // в "Море льда": z = 4
        // в "Сошенстар": я = -289
        private const string BOAT_AURA = "M10_Becritter_Boat";
        private const string SLIPPERY_AURA = "Volume_Ground_Slippery";
        // Аура 'M19_Becritter_Infernal_Machine_Apply' - активна на всей карте Авернуса и не связана с перемещением "в машине"
        private const string INFERNAL_MACHINE_AURA = "M19_Becritter_Infernal_Machine_Val";
        private const string CAR_AURA = "Vol_Car";

        private static AttribModNet slipperyAura; 
        private static AttribModNet infernalMachineAura;
        private static AttribModNet carAura;
        private static AttribModNet boatAura;

        private static double cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist; 
        #endregion
    }
#endif
}
