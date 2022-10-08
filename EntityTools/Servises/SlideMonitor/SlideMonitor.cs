
using System;
using MyNW.Internals;
using Astral.Classes;
using MyNW.Classes;
using EntityTools.Enums;
using System.Reflection;
using HarmonyLib;

namespace EntityTools.Servises.SlideMonitor
{
    /// <summary>
    /// Монитор скольжения, корректирующий движение персонажа через автоматическое увеличение
    /// "расстояние переключения следующей путевой точки"
    /// Astral.Logic.Classes.FSM.Navigation.ChangeWPDist
    /// </summary>
    internal static class SlideMonitor
    {
        public static bool PatchesWasApplied { get; private set; }

        private const string methodName_ChangeWPDist = "get_ChangeWPDist";

        private static MethodInfo original_Navigation_ChangeWPDist;
        private static MethodInfo prefix_Navigation_ChangeWPDist;

        public static void Apply()
        {
            if (PatchesWasApplied) return;

            var tNavigation = typeof(Astral.Logic.Classes.FSM.Navigation);
            var tPatch = typeof(SlideMonitor);

            original_Navigation_ChangeWPDist = AccessTools.Method(tNavigation, methodName_ChangeWPDist);
            if (original_Navigation_ChangeWPDist is null)
            {
                ETLogger.WriteLine(
                    $@"Patch '{nameof(SlideMonitor)}' failed. Method '{methodName_ChangeWPDist}' not found");
                return;
            }

            prefix_Navigation_ChangeWPDist = AccessTools.Method(tPatch, nameof(ChangeWPDistance));
            if (original_Navigation_ChangeWPDist is null)
            {
                ETLogger.WriteLine(
                    $@"Patch '{nameof(SlideMonitor)}' failed. Method '{nameof(ChangeWPDistance)}' not found");
                return;
            }

            Action unPatch = null;

            try
            {
                ACTP0Tools.Patches.ACTP0Patcher.Harmony.Patch(original_Navigation_ChangeWPDist,
                    new HarmonyMethod(prefix_Navigation_ChangeWPDist));
                unPatch = () =>
                {
                    ETLogger.WriteLine(LogType.Debug, $@"Unpatch method '{original_Navigation_ChangeWPDist}'.");
                    ACTP0Tools.Patches.ACTP0Patcher.Harmony.Unpatch(original_Navigation_ChangeWPDist,
                        prefix_Navigation_ChangeWPDist);
                };
            }

            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $@"Patch '{nameof(SlideMonitor)}' failed");
                unPatch?.Invoke();
                ETLogger.WriteLine(LogType.Error, e.ToString());
                throw;
            }

            PatchesWasApplied = true;
            ETLogger.WriteLine($@"Patch '{nameof(SlideMonitor)}' succeeded");
        }

        internal static bool ChangeWPDistance(out double __result)
        {
            bool debugInfo = EntityTools.Config.Logger.DebugSlideMonitor;
            var methodName = debugInfo ? $"{nameof(SlideMonitor)}.{MethodBase.GetCurrentMethod().Name}" : string.Empty;

            // TODO: Проверять карту, на которой находится персонаж, чтобы исключить лишние поиски ауры и т.п.
            // "Плавание под парусом" - только в "Море льда" и в "Сошенстар"
            // "Скользкая земля" - на "Перевале в IWD", на зимнем празднике, в Иллюзионисте и еще хз 
            // "Адская машина" - Авернус (?)

            if (debugInfo)
            {
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Begins");
            }

            var state = EntityTools.Config.SlideMonitor.State;

            var player = EntityManager.LocalPlayer;

            bool isMounted = player.CostumeRef.pMountCostume > 0;
            if (state == SlideMonitorState.Disabled)
            {
                __result = isMounted ? Astral.API.CurrentSettings.MountedChangeWPDist 
                                     : Astral.API.CurrentSettings.ChangeWaypointDist;
                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: (IsMounted) => {__result}");
                }
                return false;
            }

            int id = Astral.API.AttachedGameProcess.Id;
            uint playerId = player.ContainerId;
            if (cachedGameProcessId != id || cachedPlayerId != playerId)
            {
                boatAura = null;
                slipperyAura = null;
                infernalMachineAura = null;
                carAura = null;
                timeout.ChangeTime(0);

                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: Reset");
                }
            }
            cachedGameProcessId = id;
            cachedPlayerId = playerId;

            int filter = EntityTools.Config.SlideMonitor.BoatFilter;
            
            if (timeout.IsTimedOut)
            {
                if (state == SlideMonitorState.ActivateByAura)
                {
                    // Определяем скольжение по ауре
                    cachedWPD = CheckAuras();
                    if (debugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: CheckAuras => {cachedWPD}");
                    }
                }
                else
                {
                    // Определяем скольжение по средству передвижения
                    if (isMounted)
                    {
                        var mountDef = player.GetMountCostume();
                        if (string.IsNullOrEmpty(mountDef.InternalName))
                        {
                            EntityTools.Config.SlideMonitor.State = SlideMonitorState.ActivateByAura;
                            cachedWPD = Astral.API.CurrentSettings.MountedChangeWPDist;

                            if (debugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: GetMountCostume failed. Switch to '{SlideMonitorState.ActivateByAura}'");
                            }
                        }
                        else
                        {
                            var mType = mountDef.Type;
                            switch (mType)
                            {
                                case MountType.None:
                                    cachedWPD = CheckAuras();
                                    break;
                                case MountType.BoatWhite:
                                    cachedWPD = filter * 0.75d;
                                    break;
                                case MountType.BoatGreen:
                                    cachedWPD = filter * 1d;
                                    break;
                                case MountType.BoatPurple:
                                    cachedWPD = filter * 1.5d;
                                    break;
                                case MountType.InfernalCar:
                                    cachedWPD = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
                                    break;
                                case MountType.Mount:
                                    cachedWPD = Astral.API.CurrentSettings.MountedChangeWPDist;
                                    break;
                            }

                            if (debugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, $"{methodName}: ActivateByMountType ({mType}) => {cachedWPD}");
                            }
                        }
                    }
                    else
                    {
                        cachedWPD = CheckAuras();
                        if (debugInfo)
                        {
                            ETLogger.WriteLine(LogType.Debug, $"{methodName}: UnMounted. CheckAuras => {cachedWPD}");
                        }
                    }
                }
                timeout.ChangeTime(EntityTools.Config.SlideMonitor.Timeout);
            }
            else if (debugInfo)
            {
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Cached {nameof(ChangeWPDistance)} => {cachedWPD}");
            }

            __result = Math.Min(cachedWPD, Math.Max(Astral.Quester.API.Engine.Navigation.LastWaypoint.Distance2DFromPlayer, 3.0));

            return false;
        }

        private static double CheckAuras()
        {
            bool debugInfo = EntityTools.Config.Logger.DebugSlideMonitor;
            var methodName = debugInfo ? $"{nameof(SlideMonitor)}.{MethodBase.GetCurrentMethod().Name}" : string.Empty;

            var player = EntityManager.LocalPlayer;

            int filter =  EntityTools.Config.SlideMonitor.BoatFilter;
            double result = player.IsMounted ? Astral.API.CurrentSettings.MountedChangeWPDist
                                             : Astral.API.CurrentSettings.ChangeWaypointDist;

            // Аура скользкой земли
            // Может присутствовать и работать как в спешеном, так и верховом положении
            bool isValid = slipperyAura != null && slipperyAura.IsValid && slipperyAura.PowerDef.InternalName.StartsWith(SLIPPERY_AURA, StringComparison.Ordinal);
            if (isValid)
            {
                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: '{SLIPPERY_AURA}' cache is valid => {filter}");
                }
                return filter;
            }
            // Аура "плавание под парусом"
             isValid = boatAura != null && boatAura.IsValid && boatAura.PowerDef.InternalName.StartsWith(BOAT_AURA, StringComparison.Ordinal);
            if (isValid)
            {
                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: '{BOAT_AURA}' cache is valid => {filter}");
                }
                return filter;
            }

            // Аура "адской машины"
            isValid = infernalMachineAura != null && infernalMachineAura.IsValid &&
                      infernalMachineAura.PowerDef.InternalName.StartsWith(INFERNAL_MACHINE_AURA);
            if (isValid)
            {
                result = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: '{INFERNAL_MACHINE_AURA}' cache is valid => {result}");
                }
                return result;
            }

            // Аура "адской машины"
            isValid = carAura != null && carAura.IsValid &&
                      carAura.PowerDef.InternalName.StartsWith(CAR_AURA);
            if (isValid)
            {
                result = EntityTools.Config.SlideMonitor.InfernalMachineFilter;
                if (debugInfo)
                {
                    ETLogger.WriteLine(LogType.Debug, $"{methodName}: '{CAR_AURA}' cache is valid => {result}");
                }
                return result;
            }

            if (debugInfo)
            {
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: Beginning aura search");
            }

            bool auraFounded = false;
            foreach (var aura in player.Character.Mods)
            {
                var inName = aura.PowerDef.InternalName;
                if (inName.StartsWith(SLIPPERY_AURA, StringComparison.Ordinal))
                {
                    slipperyAura = aura;
                    result = filter;
                    if (debugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Found '{SLIPPERY_AURA}' => {result}");
                    }

                    auraFounded = true;
                    break;
                }
                if (inName.StartsWith(BOAT_AURA, StringComparison.Ordinal))
                {
                    boatAura = aura;
                    result = filter;
                    if (debugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Found '{BOAT_AURA}' => {result}");
                    }

                    auraFounded = true;
                    break;
                }
                if (inName.StartsWith(INFERNAL_MACHINE_AURA, StringComparison.Ordinal))
                {
                    infernalMachineAura = aura;
                    result = filter * 4d;
                    if (debugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Found '{INFERNAL_MACHINE_AURA}' => {result}");
                    }
                    auraFounded = true;
                    break;
                }
                if (inName.StartsWith(CAR_AURA, StringComparison.Ordinal))
                {
                    carAura = aura;
                    result = filter * 4d;
                    if (debugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{methodName}: Found '{CAR_AURA}' => {result}");
                    }
                    auraFounded = true;
                    break;
                }
            }

            if (debugInfo && !auraFounded)
            {
                ETLogger.WriteLine(LogType.Debug, $"{methodName}: No sliding auras found => {result}");
            }
            return result;
        }

        #region Данные
        private static int cachedGameProcessId ;
        private static uint cachedPlayerId ;

        private static readonly Timeout timeout = new Timeout(0);

#if false
        private const string MOUNT_AURA = "Mount_Base_Universal";
#endif
        // Высота уровня воды 
        // в "Море льда": z = 4
        // в "Сошенстар": z = -289
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
}
