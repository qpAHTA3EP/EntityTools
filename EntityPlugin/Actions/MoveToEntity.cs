using System;
using System.Collections.Generic;
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
            if (string.IsNullOrEmpty(EntityID))                
                target = new Entity(IntPtr.Zero);
            else
                target = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);
        }

        public override string InternalDisplayName => string.Empty;

        public MoveToEntity()
        {
            EntityID = string.Empty;
            Distance = 0;
            InteractIfPossible = false;
            InteractTime = 2000;
            IgnoreCombat = false;
        }

        [Description("ID (an internal untranslated name) of the Entity for the search (regex)")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        public string EntityID { get; set; }

        [Description("Distance to the Entity by which it is necessary to approach")]
        public float Distance { get; set; }

        [Description("Try interaction to Entity if possible")]
        public bool InteractIfPossible { get; set; }

        [Description("Time to interact (ms)")]
        public int InteractTime { get; set; }


        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        public List<string> Dialogs { get; set; }

        [Description("Enable IgnoreCombat profile value while playing action")]
        public bool IgnoreCombat { get; set; }

        [Description("Complite an action when Entity had been approached (if true)")]
        public bool StopOnApproached { get; set; }

        public override void GatherInfos()
        {
        }

        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID) ;

        public override bool NeedToRun
        {
            get
            {
                if (string.IsNullOrEmpty(EntityID))
                    target = new Entity(IntPtr.Zero);
                else
                    target = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

                //в команде ChangeProfielValue:IgnoreCombat используется код:
                //Combat.SetIgnoreCombat(IgnoreCombat, -1, 0);

                //Нашел доступный способ управлять запретом боя
                //Astral.Quester.API.IgnoreCombat = IgnoreCombat;

                if (target.IsValid)
                {
                    if(target.Location.Distance3DFromPlayer >= Distance)
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
            //target = Tools.FindClosestEntity(EntityManager.GetEntities(), EntityID);

            if (!target.IsValid)
            {
                Logger.WriteLine($"Entity [{EntityID}] not founded.");
                return ActionResult.Fail;
            }
            ActionResult actnReslt = ActionResult.Running;

            if (target.Location.Distance3DFromPlayer < Distance)
            {
                if (InteractIfPossible && target.IsValid && target.InteractOption.IsValid && Approach.EntityForInteraction(target, null))
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

                Astral.Quester.API.IgnoreCombat = false;
                if (StopOnApproached)
                    actnReslt = ActionResult.Completed;
            }

            Astral.Quester.API.IgnoreCombat = false;
            return actnReslt;
        }

        public override bool UseHotSpots => false;

        protected override Vector3 InternalDestination
        {
            get
            {
                if (target.IsValid)
                {
                    return target.Location.Clone();
                }
                return new Vector3();
            }
        }

        protected override Action.ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(EntityID))
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
