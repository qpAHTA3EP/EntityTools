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
                return string.Format("MoveToEntity [{0}]", EntityID);
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
            //IgnoreCombat = false;
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        public string EntityID { get; set; }

        public float Distance { get; set; }

        //public bool InteractIfPossible { get; set; }

        //public int InteractTime { get; set; }

        //[Description("Enable IgnoreCombat profile value while playing action")]
        //public bool IgnoreCombat { get; set; }

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
                    
                    //foreach (Entity entity in from e in EntityManager.GetEntities()
                    //                          orderby e.Location.Distance3D(NPCInfos.Position)
                    //                          select e)
                    //{
                    //    if (entity.RelationToPlayer == EntityRelation.Friend && entity.Type == GlobalType.ENTITYCRITTER && (string.IsNullOrEmpty(NPCInfos.CostumeName) || entity.CostumeRef.CostumeName == NPCInfos.CostumeName))
                    //    {
                    //        target = entity;
                    //        break;
                    //    }
                    //}
                }
                return target.IsValid && ( !StopOnApproached || target.Location.Distance3DFromPlayer >= Distance);
            }
        }

        public override Action.ActionResult Run()
        {
            if (!target.IsValid)
            {
                Logger.WriteLine(string.Format("Entity [{0}] not founded.", EntityID));
                return Action.ActionResult.Skip;
            }

            //if (target.IsValid && !(StopOnApproached && target.Location.Distance3DFromPlayer < Distance))
            //{
            //    if (InteractIfPossible && target.IsValid && target.InteractOption.IsValid && Approach.EntityForInteraction(target, null))
            //    {
            //        MyNW.Internals.Movements.StopNavTo();
            //        Thread.Sleep(500);
            //        target.Interact();
            //        Thread.Sleep(InteractTime);
            //    }
            //    return Action.ActionResult.Running;
            //}
            if (!Approach.EntityByDistance(target, Distance, null))
            {
                if (StopOnApproached && (target.Location.Distance3DFromPlayer < Distance))
                    return Action.ActionResult.Completed;
            }
            return Action.ActionResult.Running;
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
    }
}
