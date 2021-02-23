using System;
using System.Collections.Generic;
using System.Threading;
using Astral.Classes;
using Astral.Logic;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Tools.Navigation
{
    /// <summary>
    /// Полная копия класса
    /// Astral.Logic.Classes.FSM.Navigator
    /// отвечающего за перемещение по пути <seealso cref="_road"/>
    /// </summary>
    public class CustomNavigator
    {
        public CustomNavigator()
        {
            Loop = true;
            Wait = false;
            LockObject = new object();
        }

        public object LockObject { get; private set; }

        public bool Wait { get; set; }

        private void threadloop()
        {
            while (true)
            {
                try
                {
                    if (!Arrived && !Wait)
                    {
                        Pulse();
                    }
                    else if (Navigator.IsRunning)
                    {
                        Navigator.StopNavigator();
                    }
                    Thread.Sleep(1);
                }
                catch (ThreadAbortException)
                {
                }
                catch
                {
                }
            }
        }

        public double TotalDistanceToDest
        {
            get
            {
                List<Vector3> list = new List<Vector3>();
                for (int i = CurrentWaypointIndex; i < road.Waypoints.Count; i++)
                {
                    list.Add(road.Waypoints[i]);
                }
                return Navmesh.TotalDistance(list);
            }
        }

        public void Start()
        {
            object obj = locktoggle;
            lock (obj)
            {
                Wait = false;
                if (!IsRunning)
                {
                    try
                    {
                        thread.Abort();
                    }
                    catch
                    {
                    }
                    thread = new Thread(new ThreadStart(threadloop));
                    thread.Start();
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return thread != null && thread.IsAlive;
            }
        }

        public void Stop()
        {
            object obj = locktoggle;
            lock (obj)
            {
                try
                {
                    object lockObject = LockObject;
                    lock (lockObject)
                    {
                        thread.Abort();
                    }
                }
                catch
                {
                }
                if (Navigator.IsRunning)
                {
                    Navigator.StopNavigator();
                }
            }
        }

        public Vector3 Next_Pos
        {
            get
            {
                Vector3 result;
                try
                {
                    result = road.Waypoints[CurrentWaypointIndex];
                }
                catch
                {
                    try
                    {
                        if (!Loop && CurrentWaypointIndex > road.Waypoints.Count - 1)
                        {
                            result = road.Waypoints[road.Waypoints.Count - 1];
                        }
                        else
                        {
                            SetNextWaypoint();
                            result = road.Waypoints[CurrentWaypointIndex];
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

        public Road road
        {
            get
            {
                return _road;
            }
            set
            {
                object lockObject = LockObject;
                lock (lockObject)
                {
                    _road = value;
                }
            }
        }

        public bool IsLastWaypoints
        {
            get
            {
                return CurrentWaypointIndex >= road.Waypoints.Count - 1;
            }
        }

        public bool Arrived
        {
            get
            {
                return road.Waypoints.Count == 0 || (!Loop && IsLastWaypoints && NextWaypointDistance < ChangeWPDist);
            }
        }

        public double NextWaypointDistance
        {
            get
            {
                return Next_Pos.Distance2DFromPlayer;
            }
        }

        public Vector3 LastWaypoint
        {
            get
            {
                if (road.Waypoints.Count > 0)
                {
                    return road.Waypoints[road.Waypoints.Count - 1];
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
            object lockObject = LockObject;
            lock (lockObject)
            {
                road = new Road();
                CurrentWaypointIndex = 0;
            }
        }

        public int GetNearestWaypointIndex
        {
            get
            {
                return Astral.Logic.General.GetNearestIndexInPositionList(road.Waypoints, EntityManager.LocalPlayer.Location);
            }
        }

        public void UseNearestWaypoint(int radius = -1, bool natural = false)
        {
            if (radius >= 0)
            {
                double distance3DFromPlayer = road.Waypoints[CurrentWaypointIndex].Distance3DFromPlayer;
                int num = CurrentWaypointIndex;
                int num2 = CurrentWaypointIndex - radius;
                if (num2 < 0)
                {
                    num2 = 0;
                }
                int num3 = CurrentWaypointIndex + radius;
                if (num3 > road.Waypoints.Count - 1)
                {
                    num3 = road.Waypoints.Count - 1;
                }
                for (int i = num2; i < num3; i++)
                {
                    if (road.Waypoints[i].Distance3DFromPlayer < distance3DFromPlayer)
                    {
                        distance3DFromPlayer = road.Waypoints[i].Distance3DFromPlayer;
                        num = i;
                    }
                }
                if (natural)
                {
                    num += 3;
                    if (num > road.Waypoints.Count - 1)
                    {
                        num = 0;
                    }
                }
                CurrentWaypointIndex = num;
            }
            if (radius == -1 || road.Waypoints[CurrentWaypointIndex].Distance3DFromPlayer > 2000.0)
            {
                CurrentWaypointIndex = GetNearestWaypointIndex;
            }
        }

        private double ChangeWPDist
        {
            get
            {
                if (EntityManager.LocalPlayer.IsMounted)
                {
                    return Astral.API.CurrentSettings.MountedChangeWPDist;
                }
                return Astral.API.CurrentSettings.ChangeWaypointDist;
            }
        }

        private List<CharClassCategory> speedClasses
        {
            get
            {
                return new List<CharClassCategory>
                {
                    CharClassCategory.ControlWizard,
                    CharClassCategory.DevotedCleric,
                    CharClassCategory.HunterRanger,
                    CharClassCategory.SourgeWarlock,
                    CharClassCategory.TricksterRogue
                };
            }
        }

        private void Pulse()
        {
            if (Astral.API.CurrentSettings.UsePathfinding3 && Astral.API.CurrentSettings.MovementSpeed && speedClasses.Contains(EntityManager.LocalPlayer.Character.Class.Category) && road.Waypoints.Count > 0 && !EntityManager.LocalPlayer.IsMounted && EntityManager.LocalPlayer.Character.AttribsBasic.Stamina > 50f && checkSpeedTo.IsTimedOut && speedTo.IsTimedOut)
            {
                bool flag = NextWaypointDistance > 25.0;
                Vector3 vector = Next_Pos.Clone();
                if (!flag)
                {
                    for (int i = CurrentWaypointIndex + 1; i < road.Waypoints.Count; i++)
                    {
                        if (road.Waypoints[i].Distance3DFromPlayer > 25.0)
                        {
                            flag = true;
                            vector = road.Waypoints[i].Clone();
                            break;
                        }
                    }
                }
                if (flag)
                {
                    Vector3 vector2 = new Vector3();
                    if (!PathFinding.CheckDirection(EntityManager.LocalPlayer.Location, vector, ref vector2) || vector2.Distance3DFromPlayer > 25.0)
                    {
                        try
                        {
                            MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                            vector.Face();
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
            if (NextWaypointDistance < ChangeWPDist)
            {
                SetNextWaypoint();
                return;
            }
            Navigator.SetDestination(Next_Pos);
            Navigator.StartNavigator();
        }

        private Thread thread;

        private object locktoggle = new object();

        public int CurrentWaypointIndex;

        private Road _road = new Road();

        private Astral.Classes.Timeout checkSpeedTo = new Astral.Classes.Timeout(500);

        private Astral.Classes.Timeout speedTo = new Astral.Classes.Timeout(6000);
    }
}
