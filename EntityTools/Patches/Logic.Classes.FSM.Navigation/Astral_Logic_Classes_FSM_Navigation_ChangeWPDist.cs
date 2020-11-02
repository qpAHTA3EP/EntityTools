
using System;
using MyNW.Internals;
using Astral.Classes;
using MyNW.Classes;
using EntityTools.Enums;

#if PATCH_ASTRAL && HARMONY
using HarmonyLib; 
#endif

namespace EntityTools.Patches.Logic.Classes.FSM.Navigation
{
#if HARMONY
    /// <summary>
    /// Патч getter'a свойства Astral.Logic.Classes.FSM.Navigation.ChangeWPDist
    /// </summary>
    [HarmonyPatch(typeof(Astral.Logic.Classes.FSM.Navigation), "get_ChangeWPDist")] 
    internal class Astral_Logic_Classes_FSM_Navigation_ChangeWPDist
    {

        [HarmonyPrefix] 
        internal static bool get_ChangeWPDist(out double __result)
        {
            var state = EntityTools.Config.SlideMonitor.State;

            bool isMounted = EntityManager.LocalPlayer.CostumeRef.pMountCostume > 0;
            if (state == SlideMonitorState.Disabled)
            {
                if (isMounted)
                    __result = Astral.API.CurrentSettings.MountedChangeWPDist;
                else __result = Astral.API.CurrentSettings.ChangeWaypointDist;
                return false;
            }

            int id = Astral.API.AttachedGameProcess.Id;
            bool newGameProcess = attachedGameProcessId != id;

            int filter = EntityTools.Config.SlideMonitor.Filter;
            
            if (timeout.IsTimedOut)
            {
                if (state == SlideMonitorState.ActivateByAura)
                {
                    // Определяем скольжение по ауре

                    // Аура "Плавание под парусом" InternaName = "M10_Becritter_Boat"
                    // или "M10_Becritter_Boat_Costume"
                    // Аура "Скакун" InternalName = Mount_Base_Universal
                    if (isMounted)
                    {
                        if (newGameProcess || boatAura == null || !boatAura.IsValid)
                        {
                            foreach (var aura in EntityManager.LocalPlayer.Character.Mods)
                            {
                                var inName = aura.PowerDef.InternalName;
                                if (inName.StartsWith(BOAT_AURA))
                                {
                                    boatAura = aura;
                                    cachedWPD = filter;
                                    break;
                                }
                            }
                        }
                        else if (boatAura.IsValid)
                            cachedWPD = 80;
                        else boatAura = null;
                    }
                    else cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist;
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
                        }
                        else
                        {
                            var mType = mountDef.Type;
                            switch (mType)
                            {
                                case MountCostumeDef.MountType.None:
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
                        else 
#endif
                                    cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist;
                                    break;
                                case MountCostumeDef.MountType.BoatWhite:
                                    cachedWPD = filter;
                                    break;
                                case MountCostumeDef.MountType.BoatGreen:
                                    cachedWPD = filter * 1.5;
                                    break;
                                case MountCostumeDef.MountType.BoatPurple:
                                    cachedWPD = filter * 2;
                                    break;
                                case MountCostumeDef.MountType.InfernalCar:
                                    cachedWPD = filter * 4;
                                    break;
                                case MountCostumeDef.MountType.Mount:
                                    cachedWPD = Astral.API.CurrentSettings.MountedChangeWPDist;
                                    break;
                            }
                        }
                    }
                    else cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist;
                }
#if false && DEBUG
                ETLogger.WriteLine(LogType.Debug, $"Astral_Logic_Classes_FSM_Navigation_ChangeWPDist: ({mountDef.InternalName}, {mountDef.Category}) => {mType}");
#endif
                timeout.ChangeTime(EntityTools.Config.SlideMonitor.Timeout);
            }
            __result = Math.Min(cachedWPD, Math.Max(Astral.Quester.API.Engine.Navigation.LastWaypoint.Distance2DFromPlayer, 3.0));

            return false;
        }
        private const string GROUND_AURA = "Volume_Ground_Slippery";
        private static readonly string BOAT_AURA = "M10_Becritter_Boat";
        private static readonly string MOUNT_AURA = "Mount_Base_Universal";

        private static uint attachedGameProcessId = 0;

        private static uint pGroundAura;

        private static AttribModNet boatAura;

        private static Timeout timeout = new Timeout(0);

        private static double cachedWPD = Astral.API.CurrentSettings.ChangeWaypointDist;
    }
#endif
}
