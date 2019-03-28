#define ShowDebugMsg

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
        Generic,
        SimulateFKey,
        FollowAndInteractNPC,
        FollowAndSimulateFKey
    }

    public class InteractEntities : Action
    {
        public InteractEntities() : base()
        {
            EntityID = string.Empty;
            Distance = 30;
            IgnoreCombat = true;
            StopOnApproached = false;
            InteractionRequirement = InteractionRequirement.IfPossible;
            InteractTime = 2000;
            Dialogs = new List<string>();
            ignoredEntity = new TempBlackList<IntPtr>();
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        public float Distance { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat { get; set; }

        /// Внутренний список игнорируемых Entity, используемый, если InteractionRequirement=Once
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
            "'Generic' - Interact with an entity using generic interaction method\n" +
            "'SimulateFKey' - Force 'F' key press to interact an Entity\n" +
            "'FollowAndInteractNPC' - Follows an entity and interacts with it again and again as long as the interaction is possible\n" +
            "'FollowAndSimulateFKey' - Follows an entity and interacts with it by simulation 'F' key press again and again as long as the interaction is possible")]
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
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(NeedToRun)}]: Target is not valid.");
#endif
                        switch (InteractionRequirement)
                        {
                            case InteractionRequirement.Obligatory:
                                {

                                    target = SelectionTools.FindClosestInteractableEntity(EntityManager.GetEntities(), EntityID, ignoredEntity);
#if ShowDebugMsg
                                    Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(NeedToRun)}]: Target [{target.Pointer}] was selected by '{nameof(SelectionTools.FindClosestInteractableEntity)}'");
#endif
                                    break;
                                }
                            default:
                                {
                                    target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, ignoredEntity);
#if ShowDebugMsg
                                    Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(NeedToRun)}]: Target [{target.Pointer}] was selected by '{nameof(SelectionTools.FindClosestEntity)}'");
#endif
                                    break;
                                }
                        }
                    }
#if ShowDebugMsg
                    else
                    {
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(NeedToRun)}]: Target [{target.Pointer}] is valid");
                    }
#endif
                }
                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

#if ShowDebugMsg
                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(Run)}]: '{nameof(Astral.Quester.API.IgnoreCombat)}' set to {IgnoreCombat}");
#endif
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;

#if ShowDebugMsg
                bool result = (target.IsValid && target.Location.Distance3DFromPlayer <= Distance);
                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(Run)}]: '{nameof(NeedToRun)}' returns {result}");
                return result;
#else
                return (target.IsValid && target.Location.Distance3DFromPlayer <= Distance);                
#endif
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

            if (target.Location.Distance3DFromPlayer <= Distance)
            {
#if ShowDebugMsg
                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(Run)}]: '{nameof(Astral.Quester.API.IgnoreCombat)}' set to 'false'");
#endif
                Astral.Quester.API.IgnoreCombat = false;

                if (InteractionRequirement != InteractionRequirement.Forbidden)
                {
                    actnReslt = InternalInteraction();

                    if (StopOnApproached && !combat)
                    {
                        actnReslt = ActionResult.Completed;
                    }
                }
                else
                {
                    if (StopOnApproached && !combat)
                        actnReslt = ActionResult.Completed;
                }
            }
            else
            {
#if ShowDebugMsg
                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(Run)}]: '{nameof(Astral.Quester.API.IgnoreCombat)}' set to '{IgnoreCombat}'");
#endif
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;
            }

            return actnReslt;
        }

        protected ActionResult InternalInteraction()
        {
            switch (InteractionMethod)
            {
                case InteractionMethod.NPC:
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.InteractNPC)}'");

                        bool result = InteractionTools.InteractNPC(target, InteractTime, Dialogs);

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.InteractNPC)}' is {result}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");

                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");

                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once 
                            && !combat
                            && !InteractionTools.InteractNPC(target, InteractTime, Dialogs))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
                case InteractionMethod.Generic:
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.InteractGeneric)}'");

                        bool result = InteractionTools.InteractGeneric(target, InteractTime, Dialogs);

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.InteractGeneric)}' is {result}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");
                            target = new Entity(IntPtr.Zero);

                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once 
                            && !combat
                            && !InteractionTools.InteractGeneric(target, InteractTime, Dialogs))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
                case InteractionMethod.SimulateFKey:
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.SimulateFKey)}'");

                        bool result = InteractionTools.SimulateFKey(target, InteractTime, Dialogs);

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.SimulateFKey)}' is {result}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");
                            target = new Entity(IntPtr.Zero);

                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once
                            && !combat
                            && !InteractionTools.SimulateFKey(target, InteractTime, Dialogs))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
                case InteractionMethod.FollowAndInteractNPC:
                    { 
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.FollowAndInteractNPC)}'");

                        bool result = InteractionTools.FollowAndInteractNPC(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.FollowAndInteractNPC)}' is {result}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");
                            target = new Entity(IntPtr.Zero);

                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once
                            && !combat
                            && !InteractionTools.FollowAndInteractNPC(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat)))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
                case InteractionMethod.FollowAndSimulateFKey:
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.FollowAndSimulateFKey)}'");

                        bool result = InteractionTools.FollowAndSimulateFKey(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.FollowAndSimulateFKey)}' is {result}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");
                            target = new Entity(IntPtr.Zero);

                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once 
                            && !combat
                            && !InteractionTools.FollowAndSimulateFKey(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat)))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
                default:
                    {
#if ShowDebugMsg
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.InteractNPC)}'");
                        bool result = false,
                            partResult = InteractionTools.InteractNPC(target, InteractTime, Dialogs);
                        result |= partResult;
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.InteractNPC)}' is {partResult}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.InteractGeneric)}'");
                        partResult = InteractionTools.InteractGeneric(target, InteractTime, Dialogs);
                        result |= partResult;
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.InteractGeneric)}' is {partResult}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: call '{nameof(InteractionTools.SimulateFKey)}'");
                        partResult = InteractionTools.SimulateFKey(target, InteractTime, Dialogs);
                        result |= partResult;
                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Result of '{nameof(InteractionTools.SimulateFKey)}' is {partResult}");

                        Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Is in combat: {combat}");

                        if (InteractionRequirement == InteractionRequirement.Once && !result && !combat)
                        {
                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                            ignoredEntity.Add(target.Pointer);

                            Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Reset '{nameof(target)}'");
                            target = new Entity(IntPtr.Zero);

                            if (StopOnApproached && !combat)
                            {
                                Astral.Logger.WriteLine($"[{GetType().Name}.{nameof(InternalInteraction)}]: Return {ActionResult.Completed}");
                                return ActionResult.Completed;
                            }
                        }
#else
                        if (InteractionRequirement == InteractionRequirement.Once 
                            && !combat
                            && !(InteractionTools.InteractNPC(target, InteractTime, Dialogs) ||
                            InteractionTools.InteractGeneric(target, InteractTime, Dialogs) ||
                            InteractionTools.SimulateFKey(target, InteractTime, Dialogs)))
                        {
                            ignoredEntity.Add(target.Pointer);
                            target = new Entity(IntPtr.Zero);
                            if (StopOnApproached && !combat)
                                return ActionResult.Completed;
                        }
#endif
                        break;
                    }
            }
            return ActionResult.Running;
        }

        private Entity Entity(IntPtr zero)
        {
            throw new NotImplementedException();
        }

        private Approach.BreakInfos CheckCombat()
        {
            if (Attackers.List.Count > 0)
            {
                this.combat = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }

        [NonSerialized]
        private bool combat;

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);
    }
}
