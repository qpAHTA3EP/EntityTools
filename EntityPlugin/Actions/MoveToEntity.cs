﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral;
using Astral.Logic.Classes.Map;
using EntityPlugin.Editors;
using EntityPlugin.Tools;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public class MoveToEntity : Astral.Quester.Classes.Action
    {
        public MoveToEntity()
        {
            EntityID = string.Empty;
            Distance = 30;
            IgnoreCombat = true;
            StopOnApproached = false;
        }

        public override string ActionLabel => $"{GetType().Name} [{EntityID}]";

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public override void InternalReset()
        {
            if (string.IsNullOrEmpty(EntityID))
                target = new Entity(IntPtr.Zero);
            else
                target = SelectionTools.FindClosestEntityRegex(EntityManager.GetEntities(), EntityID);
        }

        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID);

        public override string InternalDisplayName => string.Empty;


        public override bool UseHotSpots => true;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (target.IsValid/* && target.Location.IsValid && target.Location.Distance3DFromPlayer > Distance*/)
                {
                    return target.Location.Clone();
                }
                return new Vector3();
            }
        }

        protected override ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(EntityID))
                {
                    return new ActionValidity($"Рroperty '{nameof(EntityID)}' not set.");
                }
                return new ActionValidity();
            }
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat { get; set; }

        [Description("Complite an action when Entity had been approached (if true)")]
        public bool StopOnApproached { get; set; }

        public override void GatherInfos()
        {
            //При активации данного кода в HotSpots неконтролируемо добавляются новые точки 
            //if (EntityManager.LocalPlayer.IsValid && EntityManager.LocalPlayer.Location.IsValid)
            //    HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());
        }

        public override bool NeedToRun
        {
            get
            {
                if (string.IsNullOrEmpty(EntityID))
                    target = new Entity(IntPtr.Zero);
                else
                    target = SelectionTools.FindClosestEntityRegex(EntityManager.GetEntities(), EntityID);

                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

                if (target.IsValid)
                {
                    if (target.Location.Distance3DFromPlayer > Distance)
                    {
                        Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                        return false;
                    }
                    else return true;
                }
                return false;
            }
        }

        public override ActionResult Run()
        {
            //target = SelectionTools.FindClosestEntityRegex(EntityManager.GetEntities(), EntityID);

            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return ActionResult.Fail;
            }

            if (target.Location.Distance3DFromPlayer < Distance)
            {
                Astral.Quester.API.IgnoreCombat = false;

                if (StopOnApproached)
                    return ActionResult.Completed;
                else return ActionResult.Running;
            }
            else
            {
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                return ActionResult.Running;
            }
        }

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);
    }
}
