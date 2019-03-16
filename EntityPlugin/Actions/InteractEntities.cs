using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using Astral;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using EntityPlugin.Editors;
using MyNW.Classes;
using MyNW.Internals;
using Action = Astral.Quester.Classes.Action;

namespace EntityPlugin.Actions
{
    public enum InteractionRequirement
    {
        /// <summary>
        /// Выбирать цели, недоступные для взаимодействия
        /// </summary>
        Unavailable,
        /// <summary>
        /// Взаимодействие запрещено.
        /// Выбираются все цели, но взаимодействие не производится
        /// </summary>
        Forbidden,
        /// <summary>
        /// Взаимодействовать, если возможно.
        /// Выбираются все цели.
        /// </summary>
        IfPossible,
        /// <summary>
        /// Взаимодействие обязательно.
        /// Пропускаются цели, недоступные для взаимодействия
        /// </summary>
        Obligatory
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

        [Description("Complite an action when Entity had been approached (if true)")]
        public bool StopOnApproached { get; set; }

        [Description("Try interaction to Entity if possible")]
        public InteractionRequirement InteractionRequirement { get; set; }

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
            if (string.IsNullOrEmpty(EntityID))
                target = new Entity(IntPtr.Zero);
            else
                target = EntityPluginTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
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
                        case InteractionRequirement.Forbidden:
                            target = EntityPluginTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
                            break;
                        case InteractionRequirement.IfPossible:
                            target = EntityPluginTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
                            break;
                        case InteractionRequirement.Unavailable:
                            target = EntityPluginTools.FindClosestUninteractableEntity(EntityManager.GetEntities(), EntityID);
                            break;
                        case InteractionRequirement.Obligatory:
                            target = EntityPluginTools.FindClosestInteractableEntity(EntityManager.GetEntities(), EntityID);
                            break;
                        default:
                            target = new Entity(IntPtr.Zero);
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
            //target = EntityPluginTools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return ActionResult.Fail;
            }
            ActionResult actnReslt = ActionResult.Running;

            if (target.Location.Distance3DFromPlayer < Distance)
            {
                Astral.Quester.API.IgnoreCombat = false;

                switch (InteractionRequirement)
                {
                    case InteractionRequirement.Forbidden:
                        actnReslt = InternalInteraction();
                        break;
                    case InteractionRequirement.IfPossible:
                        actnReslt = InternalInteraction();
                        break;
                    case InteractionRequirement.Unavailable:
                        actnReslt = ActionResult.Running;
                        break;
                    case InteractionRequirement.Obligatory:
                        actnReslt = InternalInteraction();
                        break;
                    default:
                        actnReslt = ActionResult.Running;
                        break;
                }

                actnReslt = InternalInteraction();

                if (StopOnApproached)
                    actnReslt = ActionResult.Completed;
            }
            else
                Astral.Quester.API.IgnoreCombat = IgnoreCombat;

            return actnReslt;
        }

        protected ActionResult InternalInteraction()
        {
            ActionResult actnReslt = ActionResult.Running;

            if (InteractionRequirement == InteractionRequirement.IfPossible && target.IsValid && target.InteractOption.IsValid && Approach.EntityForInteraction(target, null))
            {
                MyNW.Internals.Movements.StopNavTo();
                Thread.Sleep(500);
                target.Interact();
                Thread.Sleep(InteractTime);
                Interact.WaitForInteraction();
                if (Dialogs.Count > 0)
                {
                    Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(5000);
                    while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                    {
                        if (timeout.IsTimedOut)
                        {
                            actnReslt = ActionResult.Running;
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    using (List<string>.Enumerator enumerator = Dialogs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string key = enumerator.Current;
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                            Thread.Sleep(1000);
                        }
                    }
                }
                EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                target = new Entity(IntPtr.Zero);
            }
            return actnReslt;
        }

        [NonSerialized]
        protected Entity target = new Entity(IntPtr.Zero);
    }
}
