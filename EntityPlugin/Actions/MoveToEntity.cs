using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
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
        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);

        [NonSerialized]
        protected Timeout timer = new Timeout(1000);

        [NonSerialized]
        [Description("Period of time until the closest entity is searched again")]
        protected readonly int SearchTimeout = 1000;

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a mask (char '*' means any chars)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; }

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; }

        [Description("VisibilityDistance to the Entity by which it is necessary to approach")]
        [Category("Movement")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        [Category("Movement")]
        public bool IgnoreCombat { get; set; }

        [Description("True: Complite an action when the object is closer than 'VisibilityDistance'\n" +
                     "False: Follow an Entity regardless of its distance")]
        [Category("Movement")]
        public bool StopOnApproached { get; set; }

        [Description("Check Entity's Region:\n" +
            "True: All Entities located in the same Region as Player are ignored\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity")]
        public bool RegionCheck { get; set; }

        public MoveToEntity()
        {
            EntityID = string.Empty;
            Distance = 30;
            IgnoreCombat = true;
            StopOnApproached = false;
            EntityIdType = ItemFilterStringType.Simple;
        }

        public override string ActionLabel => $"{GetType().Name} [{EntityID}]";

        public override void OnMapDraw(GraphicsNW graph)
        {
            if (target.IsValid && target.Location.IsValid)
            {
                Brush beige = Brushes.Beige;
                graph.drawFillEllipse(target.Location, new Size(10, 10), beige);
            }
        }

        public override void InternalReset()
        {
            if (string.IsNullOrEmpty(EntityID))
            {
                target = new Entity(IntPtr.Zero);
                timer.ChangeTime(SearchTimeout);
            }
            else
            {
                target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);
                timer.ChangeTime(SearchTimeout);
            }
        }

        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID);

        public override string InternalDisplayName => string.Empty;


        public override bool UseHotSpots => true;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (target.IsValid)
                {
                    if (target.Location.Distance3DFromPlayer > Distance)
                        return target.Location.Clone();
                    else return EntityManager.LocalPlayer.Location.Clone();
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

        public override void GatherInfos() { }

        public override bool NeedToRun
        {
            get
            {
                // Поиск Entity по таймеру
                //if (string.IsNullOrEmpty(EntityID))
                //    target = new Entity(IntPtr.Zero);
                //else if (target == null || !target.IsValid || timer.IsTimedOut)
                //{
                //    target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);
                //    timer.Reset();
                //}

                // Поиск Entity при каждой проверке
                target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);

                if(target != null && target.IsValid)
                {
                    if (target.Location.Distance3DFromPlayer > Distance)
                    {
                        Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                        return false;
                    }
                    else return true;
                }
                else Astral.Quester.API.IgnoreCombat = IgnoreCombat;

                return false;
            }
        }

        public override ActionResult Run()
        {
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
    }
}
