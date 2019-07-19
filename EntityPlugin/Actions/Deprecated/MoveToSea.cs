﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms;
using Astral.Logic.Classes.FSM;
using Astral.Logic.Classes.Map;
using Astral.Quester;
using Astral.Quester.UIEditors;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Actions.Deprecated
{
    [Serializable]
    public class MoveToSea : Astral.Quester.Classes.Action
    {
        private Astral.Quester.Classes.Actions.MoveTo moveTo;

        public MoveToSea() { }

        public override string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(Desctiption))
                    return $"SailTo";
                else return $"Sail [" + Desctiption + ']';
            }
        }

        public override string InternalDisplayName => string.Empty;
        public override bool NeedToRun => moveTo.NeedToRun;
        public override bool UseHotSpots => moveTo.UseHotSpots;
        protected override bool IntenalConditions => false;
        protected override Vector3 InternalDestination => moveTo.Position;
        protected override ActionValidity InternalValidity
        {
            get
            {
                return new ActionValidity($"Action '{GetType().Name}' is {Properties.Resources.CategoryDeprecated}. " +
                    $"Use action '{typeof(Astral.Quester.Classes.Actions.MoveTo).Name}' instead. " +
                    $"The '{typeof(States.SlideMonitor)}' will filter the waypoint automatically.");
            }
        }

        public override void InternalReset() { moveTo.InternalReset(); }

        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Description("Final destination")]
        public Vector3 Position => moveTo.Position;

        [Description("Minimum distance between waypoints")]
        public int Filter { get; set; } = 90;

        [Description("Stop moving if True")]
        public bool IgnoreCombat => moveTo.IgnoreCombat;

        [Description("Description of the target location (not necessary)")]
        public string Desctiption { get; set; }

        public override ActionResult Run()
        {
            States.SlideMonitor.Activate = true;
            return moveTo.Run();
        }

        public override void GatherInfos()
        {
            MessageBox.Show("Place mark on the InGame's map and press OK");
            moveTo.Position = EntityManager.LocalPlayer.Player.MyFirstWaypoint.Position.Clone();
        }


        public override void OnMapDraw(GraphicsNW graph)
        {
            moveTo.OnMapDraw(graph);
        }

    }
#if NO_SAIL_MONITOR
    [Serializable]
    public class MoveToSea : Astral.Quester.Classes.Action
    {
        public MoveToSea()
        {
            Position = new Vector3();
            Filter = 90;
            IgnoreCombat = false;
            Desctiption = string.Empty;
        }
        public override string ActionLabel
        {
            get
            {
                if(string.IsNullOrEmpty(Desctiption))
                    return "SailTo";
                else return "Sail [" + Desctiption + ']';
            }
        }

        public override string InternalDisplayName => string.Empty;
        public override bool NeedToRun => true;
        public override bool UseHotSpots => false;
        protected override bool IntenalConditions => true;
        protected override Vector3 InternalDestination => new Vector3();
        protected override ActionValidity InternalValidity => new ActionValidity();
        public override void InternalReset() { }

        [Editor(typeof(PositionEditor), typeof(UITypeEditor))]
        [Description("Final destination")]
        public Vector3 Position { get; set; }

        [Description("Minimum distance between waypoints")]
        public int Filter { get; set; }

        [Description("Stop moving if True")]
        public bool IgnoreCombat { get; set; }

        [Description("Description of the target location (not necessary)")]
        public string Desctiption { get; set; }

        public override ActionResult Run()
        {
            Road roadFromPlayer = API.GetRoadFromPlayer(Position);
            List<Vector3> list = new List<Vector3>();
            list.Clear();
            for (int i = 0; i < roadFromPlayer.Waypoints.Count; i++)
            {
                bool flag = i == roadFromPlayer.Waypoints.Count - 1;
                if (flag)
                {
                    list.Add(roadFromPlayer.Waypoints[roadFromPlayer.Waypoints.Count - 1]);
                }
                else
                {
                    bool flag2 = list.Count == 0;
                    if (flag2)
                    {
                        bool flag3 = roadFromPlayer.Waypoints[i].Distance2DFromPlayer >= (double)Filter;
                        if (flag3)
                        {
                            list.Add(roadFromPlayer.Waypoints[i]);
                        }
                    }
                    else
                    {
                        Vector3 from = list[list.Count - 1];
                        Vector3 to = roadFromPlayer.Waypoints[i];
                        bool flag4 = MathHelper.Distance2D(from, to) >= (double)Filter;
                        if (flag4)
                        {
                            list.Add(roadFromPlayer.Waypoints[i]);
                        }
                    }
                }
            }
            API.Engine.Navigation.Stop();
            API.Engine.Navigation.Reset();
            Road road = new Road();
            road.Waypoints = list;
            API.Engine.Navigation.road = road;
            API.Engine.Navigation.Start();
            while (Position.Distance2DFromPlayer >= 7.0)
            {
                bool flag5 = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent < 1f;
                ActionResult result;
                if (flag5)
                {
                    result = ActionResult.Running;
                }
                else
                {
                    bool flag6 = EntityManager.LocalPlayer.InCombat & !IgnoreCombat;
                    if (!flag6)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    result = ActionResult.Running;
                }
                return result;
            }
            return ActionResult.Completed;
        }

        public override void GatherInfos()
        {
            MessageBox.Show("Place mark on the InGame's map and press OK");
            Position = EntityManager.LocalPlayer.Player.MyFirstWaypoint.Position.Clone();
        }


        public override void OnMapDraw(GraphicsNW graph)
        {
            graph.drawFillEllipse(Position.Clone(), new Size(10, 10), Brushes.Beige);
        }
    }
#endif
}
