using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <summary>
    /// Перечисление 
    /// </summary>
    [Serializable]
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

    /// <summary>
    /// Перечисление методов взаимодействия с Entity
    /// </summary>
    [Serializable]
    public enum InteractionMethod
    {
        /// <summary>
        /// Перебор методов NPC, Generic и SimulateFKey
        /// </summary>
        Auto,
        /// <summary>
        /// Взаимодействие с Entity как с NPC
        /// </summary>
        NPC,
        /// <summary>
        /// Взаимодействие с Entity как с неопределенным объектом
        /// </summary>
        Generic,
        /// <summary>
        /// Эмуляция нажатия кнопки 'F'
        /// </summary>
        SimulateFKey,
        /// <summary>
        /// Следование за Entity и повторение попыток взаимодействия с ним как с NPC
        /// </summary>
        FollowAndInteractNPC,
        /// <summary>
        /// Следование за Entity и повторение попыток взаимодействия с ним эмулируя нажатие кнопки 'F'
        /// </summary>
        FollowAndSimulateFKey
    }

    [Serializable]
    public class InteractEntities : Astral.Quester.Classes.Action
    {

        //[NonSerialized]
        private bool InCombat { get => (Attackers.List.Count > 0); }

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);

        [Description("Type of the EntityID:\n" +
            "Simple: Simple test string with a mask (char '*' means any chars)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; }

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; }

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

        [Description("Check Entity's Region:\n" +
            "True: All Entities located in the same Region as Player are ignored\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity")]
        public bool RegionCheck { get; set; }

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
            ignoredEntity = new TempBlackList<IntPtr>();
        }

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
                        target = SelectionTools.FindClosestInteractableEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck, ignoredEntity);
                        break;
                    default:
                        target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck, ignoredEntity);
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
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(NeedToRun)}>: Target is not valid.");
#endif
                        switch (InteractionRequirement)
                        {
                            case InteractionRequirement.Obligatory:
                                {

                                    target = SelectionTools.FindClosestInteractableEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck, ignoredEntity);
#if DEBUG
                                    if (EntityTools.DebugInfoEnabled)
                                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(NeedToRun)}>: Target [{target.Pointer}] was selected by '{nameof(SelectionTools.FindClosestInteractableEntity)}'");
#endif
                                    break;
                                }
                            default:
                                {
                                    target = SelectionTools.FindClosestEntity(EntityManager.GetEntities(), EntityID, EntityIdType, RegionCheck, ignoredEntity);
#if DEBUG
                                    if (EntityTools.DebugInfoEnabled)
                                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(NeedToRun)}>: Target [{target.Pointer}] was selected by '{nameof(SelectionTools.FindClosestEntity)}'");
#endif
                                    break;
                                }
                        }
                    }
#if DEBUG
                    else if (EntityTools.DebugInfoEnabled)
                    {
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(NeedToRun)}>: Target [{target.Pointer}] is valid");
                    }
#endif
                }
                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

#if DEBUG
                if (EntityTools.DebugInfoEnabled)
                {
                    if (target.IsValid)
                    {
                        if (target.Location.Distance3DFromPlayer > Distance)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: Distance to the Target is {target.Location.Distance3DFromPlayer} and >'{Distance}' therefore '{nameof(Astral.Quester.API.IgnoreCombat)}' set to {IgnoreCombat}");
                            Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: '{nameof(NeedToRun)}' returns 'false'");
                            return false;
                        }
                        else
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: '{nameof(NeedToRun)}' returns 'true'");
                            return true;
                        }
                    }
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: '{nameof(NeedToRun)}' returns 'false'");
                    return false;
                }
                else
#endif
                {
                    if (target.IsValid)
                    {
                        if (target.Location.Distance3DFromPlayer > Distance)
                        {
                            Astral.Quester.API.IgnoreCombat = IgnoreCombat;
                            return false;
                        }
                        else return true;
                    }
                    else Astral.Quester.API.IgnoreCombat = IgnoreCombat;
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

            ActionResult actnReslt = ActionResult.Running;

            if (target.Location.Distance3DFromPlayer <= Distance)
            {
#if DEBUG
                if (EntityTools.DebugInfoEnabled)
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: '{nameof(Astral.Quester.API.IgnoreCombat)}' set to 'false'");
#endif                    
                if (Astral.Quester.API.IgnoreCombat && !TryInteractInCombat && !InCombat)
                {
#if DEBUG
                    if (EntityTools.DebugInfoEnabled)
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: {nameof(TryInteractInCombat)} is {TryInteractInCombat} therefore return '{nameof(ActionResult.Running)}'");
#endif
                    Astral.Quester.API.IgnoreCombat = false;
                    return ActionResult.Running;
                }
                else
                    Astral.Quester.API.IgnoreCombat = false;

                if (InteractionRequirement != InteractionRequirement.Forbidden)
                {
                    actnReslt = InternalInteraction();
                }

#if DEBUG
                if (EntityTools.DebugInfoEnabled)
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: Reset Target [{target.Pointer}]");
#endif
                target = new Entity(IntPtr.Zero);

                if (StopOnApproached && !InCombat)
                    actnReslt = ActionResult.Completed;
            }
            else
            {
#if DEBUG
                if (EntityTools.DebugInfoEnabled)
                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: '{nameof(Astral.Quester.API.IgnoreCombat)}' set to '{IgnoreCombat}'");
#endif
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;
            }

#if DEBUG
            if (EntityTools.DebugInfoEnabled)
                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(Run)}>: Return '{actnReslt}'");
#endif
            return actnReslt;
        }

        protected ActionResult InternalInteraction()
        {
            switch (InteractionMethod)
            {
                case InteractionMethod.NPC:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.InteractNPC)}'");

                            bool result = InteractionTools.InteractNPC(target, InteractTime, Dialogs);

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.InteractNPC)}' is {result}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push Entity [{target.Pointer}] to '{nameof(ignoredEntity)}'");

                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");

                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return '{ActionResult.Completed}'");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                            && !InCombat
                            && !InteractionTools.InteractNPC(target, InteractTime, Dialogs))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
                case InteractionMethod.Generic:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.InteractGeneric)}'");

                            bool result = InteractionTools.InteractGeneric(target, InteractTime, Dialogs);

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.InteractGeneric)}' is {result}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push entity '{target.Pointer}' to '{nameof(ignoredEntity)}'");
                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");
                                target = new Entity(IntPtr.Zero);

                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return {ActionResult.Completed}");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                                && !InCombat
                                && !InteractionTools.InteractGeneric(target, InteractTime, Dialogs))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
                case InteractionMethod.SimulateFKey:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.SimulateFKey)}'");

                            bool result = InteractionTools.SimulateFKey(target, InteractTime, Dialogs);

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.SimulateFKey)}' is {result}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push Entity[{target.Pointer}] to '{nameof(ignoredEntity)}'");
                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");
                                target = new Entity(IntPtr.Zero);

                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return '{ActionResult.Completed}'");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                                && !InCombat
                                && !InteractionTools.SimulateFKey(target, InteractTime, Dialogs))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
                case InteractionMethod.FollowAndInteractNPC:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.FollowAndInteractNPC)}'");

                            bool result = InteractionTools.FollowAndInteractNPC(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.FollowAndInteractNPC)}' is '{result}'");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push Entity[{target.Pointer}] to '{nameof(ignoredEntity)}'");
                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");
                                target = new Entity(IntPtr.Zero);

                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return '{ActionResult.Completed}'");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                                && !InCombat
                                && !InteractionTools.FollowAndInteractNPC(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat)))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
                case InteractionMethod.FollowAndSimulateFKey:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.FollowAndSimulateFKey)}'");

                            bool result = InteractionTools.FollowAndSimulateFKey(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat));

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.FollowAndSimulateFKey)}' is {result}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push Entity[{target.Pointer}' to '{nameof(ignoredEntity)}'");
                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");
                                target = new Entity(IntPtr.Zero);

                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return '{ActionResult.Completed}'");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                                && !InCombat
                                && !InteractionTools.FollowAndSimulateFKey(target, InteractTime, Distance, Dialogs, new Func<Approach.BreakInfos>(CheckCombat)))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
                default:
                    {
#if DEBUG
                        if (EntityTools.DebugInfoEnabled)
                        {
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.InteractNPC)}'");
                            bool result = false,
                                partResult = InteractionTools.InteractNPC(target, InteractTime, Dialogs);
                            result |= partResult;
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.InteractNPC)}' is {partResult}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.InteractGeneric)}'");
                            partResult = InteractionTools.InteractGeneric(target, InteractTime, Dialogs);
                            result |= partResult;
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.InteractGeneric)}' is {partResult}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Call '{nameof(InteractionTools.SimulateFKey)}'");
                            partResult = InteractionTools.SimulateFKey(target, InteractTime, Dialogs);
                            result |= partResult;
                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Result of '{nameof(InteractionTools.SimulateFKey)}' is {partResult}");

                            Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Is in InCombat: {InCombat}");

                            if (InteractionRequirement == InteractionRequirement.Once && !result && !InCombat)
                            {
                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Push Entity[{target.Pointer}] to '{nameof(ignoredEntity)}'");
                                ignoredEntity.Add(target.Pointer);

                                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Reset '{nameof(target)}'");
                                target = new Entity(IntPtr.Zero);

                                if (StopOnApproached && !InCombat)
                                {
                                    Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Return '{ActionResult.Completed}'");
                                    return ActionResult.Completed;
                                }
                            }
                        }
                        else
#endif
                        {
                            if (InteractionRequirement == InteractionRequirement.Once
                              && !InCombat
                              && !(InteractionTools.InteractNPC(target, InteractTime, Dialogs) ||
                              InteractionTools.InteractGeneric(target, InteractTime, Dialogs) ||
                              InteractionTools.SimulateFKey(target, InteractTime, Dialogs)))
                            {
                                ignoredEntity.Add(target.Pointer);
                                target = new Entity(IntPtr.Zero);
                                if (StopOnApproached && !InCombat)
                                    return ActionResult.Completed;
                            }
                        }
                        break;
                    }
            }
#if DEBUG
            if (EntityTools.DebugInfoEnabled)
                Astral.Logger.WriteLine(Logger.LogType.Debug, $"<{GetType().Name}.{nameof(InternalInteraction)}>: Returns '{ActionResult.Running}'");
#endif
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
