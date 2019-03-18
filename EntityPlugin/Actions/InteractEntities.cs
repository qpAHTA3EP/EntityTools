using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using Astral;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using EntityPlugin.Tools;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public enum InteractionRequirement
    {
        /// <summary>
        /// Взаимодействие не производится.
        /// </summary>
        Forbidden,
        /// <summary>
        /// Взаимодействовать, если возможно.
        /// </summary>
        IfPossible,
        /// <summary>
        /// Взаимодействовать, если возможно, однократно.
        /// </summary>
        Once,
        /// <summary>
        /// Взаимодействие обязательно.
        /// </summary>
        Obligatory
    }
    
    public enum InteractionMethod
    {
        Auto,
        NPC,
        //Node,
        Generic,
        SimulateFKey
    }

    public class InteractEntities : Action
    {
        public InteractEntities() : base()
        {
            EntityID = string.Empty;
            Distance = 30;
            IgnoreCombat = true;
            StopOnApproached = true;
            InteractionRequirement = InteractionRequirement.IfPossible;
            OneInteractionByEntity = false;
            InteractTime = 2000;
            Dialogs = new List<string>();
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat { get; set; }

        //[Description("Interact each Entity only once time")]
        //public bool OneInteractionByEntity { get; set; }

        /// Список игнорируемых Entity, используемый, если InteractionRequirement=Once
        [NonSerialized]
        private TempBlackList<IntPtr> ignoredEntity;

        [Description("Complite an action when Entity had been approached (if true)")]
        public bool StopOnApproached { get; set; }

        [Description("Select the need for interaction\n" +
            "'Forbidden' - No interaction is executed\n" +
            "'IfPossible' - The interaction is executed if Entity is interactable\n" +
            "'Once' - The interaction is executed only once, and the next time the Entity will be ignored\n" +
            "'Obligatory' - Interaction is strongly needed. The interaction is repeated until the target is intractable.")]
        public InteractionRequirement InteractionRequirement { get; set; }

        [Description("Select the interaction method\n" +
            "'Auto' - Consistent use of all other interaction methods\n" +
            "'NPC' - Interact with an Entity as an NPC\n" +
        //    "'Node' - Interact with an Entity as an Node\n" +
            "'Generic' - Interact with an entity using generic interaction method\n" +
            "'SimulateFKey' - Force 'F' key press to interact an Entity")]
        public InteractionMethod InteractionMethod { get; set; }

        [Description("Time to interact (ms)")]
        public int InteractTime { get; set; }

        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        public List<string> Dialogs { get; set; }


        public override string ActionLabel => $"{GetType().Name} [{EntityID}]";

        public override void OnMapDraw(GraphicsNW graph)
        {
        }

        public override void InternalReset()
        {
            ignoredEntity.Clear();
            if (string.IsNullOrEmpty(EntityID))
                target = new Entity(IntPtr.Zero);
            else
                target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
        }

        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID);

        public override string InternalDisplayName => string.Empty;

        public override bool UseHotSpots => true;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (target.IsValid && target.Location.IsValid && target.Location.Distance3DFromPlayer > Distance)
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
                    return new ActionValidity("EntityID property not set.");
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
                {
                    switch (InteractionRequirement)
                    {
                        case InteractionRequirement.Obligatory:
                            target = SelectionTools.FindClosestInteractableEntity(EntityManager.GetEntities(), EntityID, ignoredEntity);
                            break;
                        default:
                            target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, ignoredEntity);
                            break;
                    }
                }
                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

                if (target.IsValid)
                {
                    if (target.Location.Distance3DFromPlayer >= Distance)
                    {
                        Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                        return false;
                    }
                    else return true;
                }
                else return false;
            }
        }

        public override ActionResult Run()
        {
            //target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return ActionResult.Fail;
            }
            ActionResult actnReslt = ActionResult.Running;

            if (target.Location.Distance3DFromPlayer < Distance)
            {
                Astral.Quester.API.IgnoreCombat = false;

                if(InteractionRequirement != InteractionRequirement.Forbidden)
                {

                    actnReslt = InternalInteraction();

                    if (StopOnApproached)
                        actnReslt = ActionResult.Completed;
                }
                else
                {
                    if (StopOnApproached)
                        actnReslt = ActionResult.Completed;
                }
            }
            else
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;

            return actnReslt;
        }

        protected ActionResult InternalInteraction()
        {
            ActionResult actnReslt = ActionResult.Running;

            switch (InteractionMethod)
            {
                case InteractionMethod.NPC:
                    if(InteractionRequirement == InteractionRequirement.Once && !InteractionTools.InteractNPC(target, InteractTime, Dialogs))
                    {
                        ignoredEntity.Add(target.Pointer);
                        target = new Entity(IntPtr.Zero);
                    }
                    break;
                case InteractionMethod.Generic:
                    if (InteractionRequirement == InteractionRequirement.Once && !InteractionTools.InteractGeneric(target, InteractTime, Dialogs))
                    {
                        ignoredEntity.Add(target.Pointer);
                        target = new Entity(IntPtr.Zero);
                    }
                    break;
                case InteractionMethod.SimulateFKey:
                    InteractionTools.SimulateFKey(target, InteractTime, Dialogs);
                    break;
                default:
                    InteractionTools.InteractGeneric(target, InteractTime, Dialogs);
                    InteractionTools.InteractNPC(target, InteractTime, Dialogs);
                    InteractionTools.SimulateFKey(target, InteractTime, Dialogs);
                    break;
            }
            return actnReslt;
        }

        private Entity Entity(IntPtr zero)
        {
            throw new NotImplementedException();
        }

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);
    }
}
