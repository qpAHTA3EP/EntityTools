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


namespace EntityPlugin.States
{
    class SlideMonitor : Astral.Logic.Classes.FSM.State
    {
        public override int Priority => 100;
        public override string DisplayName => typeof(SlideMonitor).Name;
#if CHANGE_WAYPOINT_DIST_SETTING
        public override bool NeedToRun => CheckTO.IsTimedOut;
#else
        public override bool NeedToRun => (CheckTO.IsTimedOut && IsSliding);
#endif
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

#if CHANGE_WAYPOINT_DIST
        /// <summary>
        /// Значение по-умолчанию CurrentSettings.ChangeWaypointDist
        /// </summary>
        internal static float DefaultChangeWaypointDist;
#endif

#if FILTERING
        /// <summary>
        /// Количество ключевых точек пути "при скольжении",
        /// после формирования которых фильтрация прекращается.
        /// При нулевом значении фильтруется весь путь.
        /// </summary>
        public static uint FilteingDepth = 5;

        /// <summary>
        /// Минимальное расстояние до целевой точки
        /// </summary>
        public static double MinTargetDistance = 10;
#endif
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
#if CHANGE_WAYPOINT_DIST_SETTING
                    Astral.API.CurrentSettings.ChangeWaypointDist = States.SlideMonitor.DefaultChangeWaypointDist;
#endif
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
#region CHANGE_WAYPOINT_DISTANCE
#if CHANGE_WAYPOINT_DIST_SETTING
            if (IsSliding)
            {
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Set ChangeWaypointDist = {Filter}");
#endif
                Astral.API.CurrentSettings.ChangeWaypointDist = Filter;
            }
            else
            {
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Set ChangeWaypointDist = {DefaultChangeWaypointDist}");
#endif
                Astral.API.CurrentSettings.ChangeWaypointDist = DefaultChangeWaypointDist;
            }
#endif
#endregion
#region NEXT_WAYPOING
#if NEXT_WAYPOING
            if (Astral.Quester.API.Engine.Navigation.road.Waypoints.Count > 0 
                && !Astral.Quester.API.Engine.Navigation.IsLastWaypoints)
            {
                //Astral.Quester.API.Engine.Navigation.Wait = true;
#if DEBUG
                int prevWPIndex = Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex;
#endif
                while (Astral.Quester.API.Engine.Navigation.NextWaypointDistance <= Filter 
                    && !Astral.Quester.API.Engine.Navigation.IsLastWaypoints )
                {
                    Astral.Quester.API.Engine.Navigation.SetNextWaypoint();
                }
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}: Skip {Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex- prevWPIndex} of waipoints. CurrentWaypointIndex{Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex}");
#endif
                //Astral.Quester.API.Engine.Navigation.Wait = false;
            }
#endif
#endregion
#region FILTERING
#if FILTERING
            // Резервная копия пути
            List<Vector3> waypoints = new List<Vector3>(Astral.Quester.API.Engine.Navigation.road.Waypoints);

            try
            {
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, $"[{GetType().Name}] Befor Filtering: Waipoints number={Astral.Quester.API.Engine.Navigation.road.Waypoints.Count}; " +
                                 $"CurrentWaypointIndex={Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex}; " +
                                 $"TotalDistanceToDest={Astral.Quester.API.Engine.Navigation.TotalDistanceToDest}");
#endif
                if (FiterWaypoints(waypoints, Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex))
                {
                    // замена точек пути
                    Astral.Quester.API.Engine.Navigation.Stop();
                    Astral.Quester.API.Engine.Navigation.Reset();
                    Road newRoad = new Road();
                    newRoad.Waypoints = waypoints;
                    Astral.Quester.API.Engine.Navigation.road = newRoad;
                    Astral.Quester.API.Engine.Navigation.Start();
#if DEBUG
                    Logger.WriteLine(Logger.LogType.Debug, $"[{GetType().Name}] After Filtering: Waipoints number={Astral.Quester.API.Engine.Navigation.road.Waypoints.Count}; " +
                                     $"CurrentWaypointIndex={Astral.Quester.API.Engine.Navigation.CurrentWaypointIndex}; " +
                                     $"TotalDistanceToDest={Astral.Quester.API.Engine.Navigation.TotalDistanceToDest}");
#else
                    Logger.WriteLine($"{GetType().Name}: Waypoints was filtered ({nameof(Filter)}={Filter}, {nameof(FilteingDepth)}={FilteingDepth})");
#endif
                }
                else
                {
                    // Новый список построить не удалось
                    Logger.WriteLine($"[{GetType().Name}] Waypoints filtering was unsuccessful!");
                    // Astral.Quester.API.Engine.Navigation.Reset();
                    // Astral.Quester.API.Engine.Navigation.road.Waypoints = waypoints;
                }
            }
            catch(Exception e)
            {
                Logger.WriteLine($"[{GetType().Name}] in the method '{nameof(FiterWaypoints)}' catch an exception: \n{e.Message}");

                // Astral.Quester.API.Engine.Navigation.Reset();
                // Astral.Quester.API.Engine.Navigation.road.Waypoints = waypoints;
            }
#endif
#endregion

            //CheckTO.Reset();
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

#if DEBUG
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

#if FILTERING
        /// <summary>
        /// Фильтрация (прореживание) точек пути для "скользящего" движения 
        /// </summary>
        /// <param name="waypoints">Прореживаемый путь</param>
        /// <param name="currentWaypointIndex">Текущая точка пути</param>
        /// <returns>True, если новый путь корректен</returns>
        internal bool FiterWaypoints(List<Vector3> waypoints, int currentWaypointIndex = 0)
        {
            if (waypoints == null || currentWaypointIndex >= waypoints.Count)
                return false;

            Vector3 targetPoint = TargetPosition.Clone();
            if (targetPoint.Distance3DFromPlayer <= Filter)
            {
                waypoints.Clear();
                waypoints.Add(targetPoint);
                return true;
            }

            if (currentWaypointIndex > 0)
                waypoints.RemoveRange(0, currentWaypointIndex - 1);

            // Вариан реализации "модификацией полученного списка"
            // Сохраняются ключевые точки пути в количестве FilteingDepth и все последующие точки пути.
            // Если FilteingDepth не задано (т.е. равно нулю), то количество ключевых точек пути вычисляется как 
            //      (Astral.Quester.API.Engine.Navigation.TotalDistanceToDest / Filter) + 1
            // Ключевые точки пути выбираются таким образом. чтобы расстояние между ними было не меньше Filter
            // Все остальные точки пути, расположенные между ключевыми удаляются
            uint num = (FilteingDepth > 0) ? FilteingDepth : (uint)(Astral.Quester.API.Engine.Navigation.TotalDistanceToDest / Filter) + 1;
            int currInd = 0;
            int nextInd = 1;
            while (currInd < num && nextInd < waypoints.Count) // Сложность О(1)
            {
                Vector3 currPoint = waypoints[currInd]; // Сложность О(1)
                Vector3 nextPoint = waypoints[nextInd]; // Сложность О(1)

                if (nextPoint.Distance3DFromPlayer < currPoint.Distance3DFromPlayer)
                {
                    // найдена точка nextPoint, расположенная по отношению к LocalPlayer ближе чем currPoint,
                    // поэтому из списка waipoints нужно удалить все точки до nextPoint
                    waypoints.RemoveRange(0, nextInd); // Сложность О(nextInd)
                    currInd = 0;
                    nextInd = 1;
                    continue;
                }

                if ( nextInd != waypoints.Count - 1 
                    || MathHelper.Distance3D(currPoint, nextPoint) > Filter )
                {
                    // Найденная точка nextPoint расположена от currPoint на расстоянии большем чем Filter 
                    // или является последней в списке
                    // то есть является "ключевой".
                    // поэтому все промежуточные точки нужно удалить
                    // Число промежуточных точек вычисляется как (nextInd - currInd - 1)
                    waypoints.RemoveRange(currInd + 1, nextInd - currInd - 1); // Сложность О(nextInd-currInd-1)
                    currInd++;
                    nextInd = currInd + 1;
                }
                else nextInd++;
            }

            return waypoints.Count > 0;
        }
#endif
    }
}
