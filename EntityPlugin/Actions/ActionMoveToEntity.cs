﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public class MoveToEntity : Astral.Quester.Classes.Action
    {
        public override string ActionLabel
        {
            get
            {
                return $"{GetType().Name} [{EntityID}]";
            }
        }

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public override void InternalReset()
        {
            target = new Entity(IntPtr.Zero);
        }

        public override string InternalDisplayName => string.Empty;

        public MoveToEntity()
        {
            EntityID = string.Empty;
            Distance = 0;
            //InteractIfPossible = false;
            //InteractTime = 2000;
            IgnoreCombat = false;
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        public float Distance { get; set; }

        //public bool InteractIfPossible { get; set; }

        //public int InteractTime { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat
        {
            get
            {

                return _ignoreCombat;
            }
            set
            {
                _ignoreCombat = value;
            }
        }

        [Description("Complite an action when Entity had been approached (if true)")]
        public bool StopOnApproached { get; set; }

        public override void GatherInfos()
        {
        }

        protected override bool IntenalConditions => (EntityID != string.Empty);

        public override bool NeedToRun
        {
            get
            {
                if (!target.IsValid)
                {
                    target = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
                }
                return target.IsValid && (!StopOnApproached || target.Location.Distance3DFromPlayer >= Distance);
            }
        }

        public override Action.ActionResult Run()
        {
            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return Action.ActionResult.Fail;
            }

            if (target.Location.Distance3DFromPlayer > Distance || !StopOnApproached)
            {
                //Approach.EntityByDistance(target, Distance, null);
                return Action.ActionResult.Running;
            }
            return Action.ActionResult.Completed;
        }

        public override bool UseHotSpots => false;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (!target.IsValid)
                {
                    target = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
                }
                if (target.Location.Distance3DFromPlayer > Distance)
                {
                    return target.Location;
                }
                return new Vector3();
            }
        }

        protected override Action.ActionValidity InternalValidity
        {
            get
            {
                if (EntityID == string.Empty)
                {
                    return new Action.ActionValidity("EntityID property not set.");
                }
                return new Action.ActionValidity();
            }
        }

        [NonSerialized]
        private Entity target = new Entity(IntPtr.Zero);
        private bool _ignoreCombat;
    }
}
