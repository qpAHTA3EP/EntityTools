using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Actions
{
    [Serializable]
    public class InteractEntities : Astral.Quester.Classes.Action
    {

        //[NonSerialized]
        private bool InCombat { get => (Attackers.List.Count > 0); }

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);

        [Description("Type of the EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; }

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; }

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; }

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity")]
        public bool RegionCheck { get; set; }

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity")]
        public bool HealthCheck { get; set; }

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity")]
        public float ReactionRange { get; set; }

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public List<string> CustomRegionNames { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        [Category("Movement")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        [Category("Movement")]
        public bool IgnoreCombat { get; set; }

        /// Внутренний список игнорируемых Entity, используемый, если InteractionRequirement=Once
        [NonSerialized]
        private TempBlackList<IntPtr> ignoredEntity;

        [Description("True: Complite an action when the Bot has approached and interacted with the Entity\n" +
                     "False: Continue executing the action after the Bot has approached and interacted with the Entity")]
        [Category("Movement")]
        public bool StopOnApproached { get; set; }

        [Description("Select the need for interaction\n" +
            "Forbidden: No interaction is executed\n" +
            "IfPossible: The interaction is executed if Entity is interactable\n" +
            "Once: The interaction is executed only once, and the next time the Entity will be ignored\n" +
            "Obligatory: Interaction is strongly needed. The interaction is repeated until the target is intractable.")]
        [Category("Interaction")]
        public InteractionRequirement InteractionRequirement { get; set; }

        [Description("Select the interaction method\n" +
            "Auto: Consistent use of all other interaction methods\n" +
            "NPC: Interact with an Entity as an NPC\n" +
            "Generic: Interact with an entity using generic interaction method\n" +
            "SimulateFKey: Force 'F' key press to interact an Entity\n" +
            "FollowAndInteractNPC: Follows an entity and interacts with it again and again as long as the interaction is possible\n" +
            "FollowAndSimulateFKey: Follows an entity and interacts with it by simulation 'F' key press again and again as long as the interaction is possible")]
        [Category("Interaction")]
        public InteractionMethod InteractionMethod { get; set; }

        [Description("Time to interact (ms)")]
        [Category("Interaction")]
        public int InteractTime { get; set; }

        [Description("True: The attempt of the interaction is done prior to entering InCombat\n" +
            "False: Interact after combat completed")]
        [Category("Interaction")]
        public bool TryInteractInCombat { get; set; }

        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
        public List<string> Dialogs { get; set; }

        public InteractEntities() : base()
        {
            EntityID = string.Empty;
            Distance = 30;
            IgnoreCombat = true;
            StopOnApproached = false;
            InteractionRequirement = InteractionRequirement.IfPossible;
            InteractionMethod = InteractionMethod.NPC;
            EntityIdType = ItemFilterStringType.Simple;
            InteractTime = 2000;
            TryInteractInCombat = false;
            Dialogs = new List<string>();
            RegionCheck = false;
            HealthCheck = true;
            ignoredEntity = new TempBlackList<IntPtr>();

            CustomRegionNames = new List<string>();
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
            ignoredEntity.Clear();
            target = new Entity(IntPtr.Zero);
            //if (string.IsNullOrEmpty(EntityID))
            //    target = new Entity(IntPtr.Zero);
            //else if (InteractionRequirement == InteractionRequirement.Obligatory || InteractionRequirement == InteractionRequirement.Once)
            //    target = EntitySelectionTools.FindClosestContactEntity(EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, ignoredEntity);
            //else target = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, ignoredEntity, false);
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
                    if (!target.IsValid)
                    {
                        if(InteractionRequirement == InteractionRequirement.Obligatory || InteractionRequirement == InteractionRequirement.Once)
                            target = EntitySelectionTools.FindClosestContactEntity(EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, ignoredEntity);
                        else target = EntitySelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, ignoredEntity, false);
                    }

                }
                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

                if (target.IsValid)
                {
                    if (target.Location.Distance3DFromPlayer > Distance)
                    {
                        if(IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                        return false;
                    }
                    else return true;
                }
                else if(IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = IgnoreCombat;

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

            if (target.Location.Distance3DFromPlayer <= Distance)
            {
                if (InteractionRequirement != InteractionRequirement.Forbidden)
                {
                    if (InCombat)
                    {
                        if (TryInteractInCombat)
                            return InternalInteraction();
                        else
                        {
                            if (IgnoreCombat)
                                Astral.Quester.API.IgnoreCombat = false;
                            return ActionResult.Running;
                        }
                    }
                    else
                    {
                        if (IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = false;
                        return InternalInteraction();
                    }
                }
                else
                {
                    if (IgnoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    if (StopOnApproached)
                        return ActionResult.Completed;
                }
                //if (Astral.Quester.API.IgnoreCombat && !TryInteractInCombat && !InCombat)
                //{
                //    if(IgnoreCombat)
                //        Astral.Quester.API.IgnoreCombat = false;
                //    return ActionResult.Running;
                //}
                //else
                //    Astral.Quester.API.IgnoreCombat = false;

                //if (InteractionRequirement != InteractionRequirement.Forbidden)
                //    actnReslt = InternalInteraction();

                //target = new Entity(IntPtr.Zero);

                //if (StopOnApproached && !InCombat)
                //    actnReslt = ActionResult.Completed;
            }
            else if (IgnoreCombat)
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;

            return ActionResult.Running;
        }

        protected ActionResult InternalInteraction()
        {
            bool anotherInteractionAllowed = false;
            switch (InteractionMethod)
            {
                case InteractionMethod.NPC:
                        anotherInteractionAllowed = EntityInteractionTools.InteractNPC(target, InteractTime, Dialogs);                        
                        break;
                case InteractionMethod.Generic:
                        anotherInteractionAllowed = EntityInteractionTools.InteractGeneric(target, InteractTime, Dialogs);
                        break;
                case InteractionMethod.SimulateFKey:
                        anotherInteractionAllowed = EntityInteractionTools.SimulateFKey(target, InteractTime, Dialogs);
                        break;
                case InteractionMethod.FollowAndInteractNPC:
                        anotherInteractionAllowed = EntityInteractionTools.FollowAndInteractNPC(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));
                        break;
                case InteractionMethod.FollowAndSimulateFKey:
                        anotherInteractionAllowed = EntityInteractionTools.FollowAndSimulateFKey(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));
                        break;                    
            }

            if (anotherInteractionAllowed)
            {
                if (InCombat)
                    return ActionResult.Running;
            }
            else
            {
                if (InteractionRequirement == InteractionRequirement.Once)
                    ignoredEntity.Add(target.Pointer, 60);
                target = new Entity(IntPtr.Zero);
            }
            if (StopOnApproached && !InCombat)
            {
                target = new Entity(IntPtr.Zero);
                return ActionResult.Completed;
            }

            return ActionResult.Running;
        }

        private Approach.BreakInfos CheckCombat()
        {
            if (Attackers.List.Count > 0)
            {
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }
    }
}
