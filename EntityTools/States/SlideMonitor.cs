//#define CHANGE_WAYPOINT_DIST_SETTING
#define NEXT_WAYPOING

using Astral;
using Astral.Logic.Classes.FSM;
using Astral.Logic.Classes.Map;
using Astral.Quester;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace EntityTools.States
{
    class SlideMonitor : Astral.Logic.Classes.FSM.State
    {
        public override int Priority => 73;
        public override string DisplayName => GetType().Name;
        public override bool NeedToRun => (CheckTO.IsTimedOut && IsSliding);

        /// <summary>
        // Интервал между проверками ауры во время скольжения
        /// </summary>
        public static int CheckTimeSlide { get; set; } = 100;
        /// <summary>
        /// Интервал между проверками ауры без скольжения
        /// </summary>
        public static int CheckTimeNotSlide { get; set; } = 3000;

        private int checkInterval;
        public override int CheckInterval => checkInterval;

        public override bool StopNavigator => false;

        /// <summary>
        /// Расстояние между последовательными точками пути при скольжении
        /// </summary>
        public static float Filter = 60;

        /// <summary>
        /// Cписок аур, указывающий на режим скольжения
        /// </summary>
        public static string[] SlidingAuras = { "M10_Becritter_Boat_Costume",   // Плавание под парусом (Море льда, Сошенстар в Чалте)
                                                "Volume_Ground_Slippery",        // Скольжение на льду (Долина дворфов, ДЛВ)
                                                "Volume_Ground_Slippery_Playeronly" // Скольжение на льду (Гамбит иллюзиониста, Схватка)
                                              };     
        
        private static bool beforeStartEngineSubscribed = false;
        private static readonly SlideMonitor monitor = new SlideMonitor();

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
                    // Включение монитора
                    if (!beforeStartEngineSubscribed)
                    {
                        Astral.Quester.API.BeforeStartEngine += API_BeforeStartEngine;
                        beforeStartEngineSubscribed = true;
                    }
                    Logger.WriteLine($"{typeof(SlideMonitor).Name} activated"); 
                    if (Astral.Quester.API.Engine.Running && !Astral.Quester.API.Engine.States.Contains(monitor))
                        Astral.Quester.API.Engine.AddState(monitor);
                }
                else
                {
                    // Выключение монитора
                    Astral.Quester.API.BeforeStartEngine -= API_BeforeStartEngine;
                    beforeStartEngineSubscribed = false;
                    if (Astral.Quester.API.Engine.Running)
                        Logger.WriteLine($"{typeof(SlideMonitor).Name} will be deactivated after the Astral will stop");
                    else Logger.WriteLine($"{typeof(SlideMonitor).Name} deactivated");
                }
            }
        }

        private static void API_BeforeStartEngine(object sender, BeforeEngineStart e)
        {
            if (!Astral.Quester.API.Engine.States.Contains(monitor))
                Astral.Quester.API.Engine.AddState(monitor);
        }

        public override void Run()
        {

#region NEXT_WAYPOING
#if NEXT_WAYPOING
            if (Astral.Quester.API.Engine.Navigation.road.Waypoints.Count > 0 
                && !Astral.Quester.API.Engine.Navigation.IsLastWaypoints)
            {
                //Astral.Quester.API.Engine.Navigation.Wait = true;
#if DEBUG_SLIDEMONITOR
                int prevWPIndex = Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex;
#endif
                while (Astral.Quester.API.Engine.Navigation.NextWaypointDistance <= Filter 
                    && !Astral.Quester.API.Engine.Navigation.IsLastWaypoints )
                {
                    Astral.Quester.API.Engine.Navigation.SetNextWaypoint();
                }
#if DEBUG_SLIDEMONITOR
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Skip {Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex- prevWPIndex} of waipoints. CurrentWaypointIndex{Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex}");
#endif
                //Astral.Quester.API.Engine.Navigation.Wait = false;
            }
#endif
#endregion
        }

        /// <summary>
        /// Конечная точка маршрута
        /// </summary>
        internal Vector3 TargetPosition
        {
            get
            {
                return Astral.Quester.API.Engine.Navigation.LastWaypoint;
            }
        }

        /// <summary>
        /// Проверка состояния "скольжения"
        /// </summary>
        /// <returns></returns>
        internal bool IsSliding
        {
            get
            {
                // Ищим любую ауру скольжения
#if X32
                AttribMod mod = EntityManager.LocalPlayer.Character.Mods.Find(x => SlidingAuras.Contains(x.PowerDef.InternalName)); // х32
#endif
#if X64
                AttribModNet mod = EntityManager.LocalPlayer.Character.Mods.Find(x => SlidingAuras.Contains(x.PowerDef.InternalName)); //х64
#endif

#if DEBUG_SLIDEMONITOR
                if (mod != null && mod.IsValid)
                    Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Found sliding aura '{mod.PowerDef.InternalName}'");
                else Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}:  No sliding auras was found");
#endif
                if (mod != null && mod.IsValid)
                {
                    if (checkInterval > CheckTimeSlide)
                        CheckTO.ChangeTime(CheckTimeSlide);
                    checkInterval = CheckTimeSlide;
                }
                else
                {
                    if (checkInterval < CheckTimeNotSlide)
                        CheckTO.ChangeTime(CheckTimeNotSlide);
                    checkInterval = CheckTimeNotSlide;
                }
                return (mod != null && mod.IsValid);
            }
        }
    }
}
