using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AStar.Tools;
using Astral.Classes;
using Astral.Logic;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using EntityTools;
using EntityTools.Servises.SlideMonitor;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityCore.Tools.Navigation
{
    /// <summary>
    /// Полная копия класса
    /// Astral.Logic.Classes.FSM.Navigator
    /// отвечающего за перемещение по пути <seealso cref="_road"/>
    /// </summary>
    public class CustomNavigator
    {
        private CancellationTokenSource tokenSource;
        private System.Threading.Tasks.Task navigationTask;
        private RWLocker rwLocker = new RWLocker();

        public CustomNavigator(bool loop = false, bool wait = false)
        {
            Loop = loop;
            Wait = wait;
        }

        public bool Wait { get; set; }

        private void work_Naviganion(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (Navigator.IsRunning)
                    {
                        Navigator.StopNavigator();
                    }
                    if (!Arrived && !Wait)
                    {
                        work_Pulse();
                    }
                    Thread.Sleep(10);
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine(LogType.Error, e.ToString());
                }
            }
        }

        private void work_Pulse()
        {
#if false
            var waypoints = Road.Waypoints;
            if (Astral.API.CurrentSettings.UsePathfinding3
                    && Astral.API.CurrentSettings.MovementSpeed
                    && IsSpeedClass(EntityManager.LocalPlayer.Character.Class.Category)
                    && waypoints.Count > 0
                    && !EntityManager.LocalPlayer.IsMounted
                    && EntityManager.LocalPlayer.Character.AttribsBasic.Stamina > 50f
                    && checkSpeedTo.IsTimedOut && speedTo.IsTimedOut)
            {
                // Использование классового уклонения (Shift) для ускоренного перемещения
                //TODO: НЕобходимо анализировать "кривизну" пути и использовать классовое уклонения только по прямой
                Vector3 nextWp = NextWaypoint.Clone();
                bool flag = nextWp.Distance3DFromPlayer > 25.0;
                if (!flag)
                {
                    for (int i = CurrentWaypointIndex + 1; i < waypoints.Count; i++)
                    {
                        var wp = waypoints[i];
                        if (wp.Distance3DFromPlayer > 25.0)
                        {
                            flag = true;
                            nextWp = wp.Clone();
                            break;
                        }
                    }
                }
                if (flag)
                {
                    Vector3 vector2 = new Vector3();
                    if (!PathFinding.CheckDirection(EntityManager.LocalPlayer.Location, nextWp, ref vector2)
                        || vector2.Distance3DFromPlayer > 25.0)
                    {
                        try
                        {
                            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                            nextWp.Face();
                            Thread.Sleep(50);
                            Reset();
                            GameCommands.ToggleTacticalSpecial(true);
                            Thread.Sleep(1000);
                        }
                        finally
                        {
                            GameCommands.ToggleTacticalSpecial(false);
                            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                            Reset();
                        }
                        speedTo.Reset();
                        return;
                    }
                }
                checkSpeedTo.Reset();
            } 
#endif
            var nextPos = NextWaypoint.Clone();
            if (nextPos.Distance3DFromPlayer < ChangeWPDistance)
            {
                SetNextWaypoint();
                return;
            }
            Navigator.SetDestination(nextPos);
            Navigator.StartNavigator();
        }

        public double TotalDistanceToDest
        {
            get
            {
                List<Vector3> list = new List<Vector3>();
                for (int i = CurrentWaypointIndex; i < Road.Waypoints.Count; i++)
                {
                    list.Add(Road.Waypoints[i]);
                }
                return Navmesh.TotalDistance(list);
            }
        }

        public void Start()
        {
            using (rwLocker.WriteLock())
            {
                Wait = false;
                if (!IsRunning)
                {
                    tokenSource = new CancellationTokenSource();
                    navigationTask = System.Threading.Tasks.Task.Run(() => work_Naviganion(tokenSource.Token), tokenSource.Token);
                }
            }
        }

        public bool IsRunning
        {
            get
            {
#if false
                return thread != null && thread.IsAlive; 
#endif
                return navigationTask != null && navigationTask.Status == TaskStatus.Running;
            }
        }

        public void Stop()
        {
            using (rwLocker.WriteLock())
            {
                tokenSource.Cancel();
            }
            Navigator.StopNavigator();
        }

        public Vector3 NextWaypoint
        {
            get
            {
                Vector3 result;
                try
                {
                    result = Road.Waypoints[CurrentWaypointIndex];
                }
                catch
                {
                    try
                    {
                        if (!Loop && CurrentWaypointIndex > Road.Waypoints.Count - 1)
                        {
                            result = Road.Waypoints[Road.Waypoints.Count - 1];
                        }
                        else
                        {
                            SetNextWaypoint();
                            result = Road.Waypoints[CurrentWaypointIndex];
                        }
                    }
                    catch
                    {
                        result = new Vector3();
                    }
                }
                return result;
            }
        }

        public Road Road
        {
            get
            {
                return _road;
            }
            set
            {
                using (rwLocker.WriteLock())
                {
                    _road = value;
                }
            }
        }

        public bool IsLastWaypoints =>  CurrentWaypointIndex >= Road.Waypoints.Count - 1;

        public bool Arrived =>  Road.Waypoints.Count == 0 || !Loop && IsLastWaypoints && NextWaypointDistance < ChangeWPDistance;

        public double NextWaypointDistance =>  NextWaypoint.Distance2DFromPlayer;

        public Vector3 LastWaypoint
        {
            get
            {
                if (Road.Waypoints.Count > 0)
                {
                    return Road.Waypoints[Road.Waypoints.Count - 1];
                }
                return new Vector3();
            }
        }

        public bool Loop { get; set; }

        public void SetNextWaypoint()
        {
            if (!IsLastWaypoints)
            {
                CurrentWaypointIndex++;
                return;
            }
            if (Loop)
            {
                CurrentWaypointIndex = 0;
            }
        }

        public void Reset()
        {
            using (rwLocker.WriteLock())
            {
                tokenSource.Cancel();
                Road = new Road();
                CurrentWaypointIndex = 0;
            }
        }

        public int GetNearestWaypointIndex =>  Astral.Logic.General.GetNearestIndexInPositionList(Road.Waypoints, EntityManager.LocalPlayer.Location);

        public void UseNearestWaypoint(int radius = -1, bool natural = false)
        {
            if (radius == 0)
                return;
            if (radius > 0)
            {
                double distance3DFromPlayer = Road.Waypoints[CurrentWaypointIndex].Distance3DFromPlayer;
                int currentWpIndex = CurrentWaypointIndex;
                int lowWpIndex = currentWpIndex - radius;
                if (lowWpIndex < 0)
                {
                    lowWpIndex = 0;
                }
                int upperWpIndex = currentWpIndex + radius;
                var waypoints = Road.Waypoints;

                if (upperWpIndex > waypoints.Count - 1)
                {
                    upperWpIndex = waypoints.Count - 1;
                }
                for (int i = lowWpIndex; i < upperWpIndex; i++)
                {
                    if (waypoints[i].Distance3DFromPlayer < distance3DFromPlayer)
                    {
                        distance3DFromPlayer = waypoints[i].Distance3DFromPlayer;
                        currentWpIndex = i;
                    }
                }
                if (natural)
                {
                    currentWpIndex += 3;
                    if (currentWpIndex > waypoints.Count - 1)
                    {
                        currentWpIndex = waypoints.Count - 1;
                    }
                }
                CurrentWaypointIndex = currentWpIndex;
            }
            if (radius == -1 || Road.Waypoints[CurrentWaypointIndex].Distance3DFromPlayer > 2000.0)
            {
                CurrentWaypointIndex = GetNearestWaypointIndex;
            }
        }

        private double ChangeWPDistance
        {
            get
            {
                HarmonyPatch_Astral_Logic_Classes_FSM_Navigation_ChangeWPDist.get_ChangeWPDist(out double distance);
                return distance;
            }
        }

        private readonly CharClassCategory[] speedClasses = {
                    CharClassCategory.ControlWizard,
                    CharClassCategory.DevotedCleric,
                    CharClassCategory.HunterRanger,
                    CharClassCategory.SourgeWarlock,
                    CharClassCategory.TricksterRogue
                };

        private bool IsSpeedClass(CharClassCategory charCategory)
        {
            return charCategory == CharClassCategory.ControlWizard
                   || charCategory == CharClassCategory.DevotedCleric
                   || charCategory == CharClassCategory.HunterRanger
                   || charCategory == CharClassCategory.SourgeWarlock
                   || charCategory == CharClassCategory.TricksterRogue;
        }

        public int CurrentWaypointIndex { get; private set; }

        private Road _road = new Road();

        private Astral.Classes.Timeout checkSpeedTo = new Astral.Classes.Timeout(500);
        private Astral.Classes.Timeout speedTo = new Astral.Classes.Timeout(6000);
    }
}
