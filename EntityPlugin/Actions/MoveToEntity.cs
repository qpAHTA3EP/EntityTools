using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral;
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

        [Description("Type of and EntityID:\n" +
            "Simple: Simple test string with a mask (char '*' means any chars)\n" +
            "Regex: Regular expression")]
        public ItemFilterStringType EntityIdType { get; set; }

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat { get; set; }

        [Description("True: Complite an action when the object is closer than 'Distance'\n" +
                     "False: Follow an Entity regardless of its distance")]
        public bool StopOnApproached { get; set; }

        [Description("Check Entity's Region:\n" +
            "True: All Entities located in the same Region as Player are ignored\n" +
            "False: Entity's Region does not checked during search")]
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
        }

        public override void InternalReset()
        {
            if (string.IsNullOrEmpty(EntityID))
                target = new Entity(IntPtr.Zero);
            else
                target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);
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
                    target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck);

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
