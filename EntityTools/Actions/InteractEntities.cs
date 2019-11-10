using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.UIEditors;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Logger;

namespace EntityTools.Actions
{
    [Serializable]
    public class InteractEntities : Astral.Quester.Classes.Action
    {
        [Description("Type of the EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType { get; set; } = ItemFilterStringType.Simple;

        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; } = string.Empty;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; } = EntityNameType.InternalName;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity optional checks")]
        public bool RegionCheck { get; set; }

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity optional checks")]
        public bool HealthCheck { get; set; } = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within Distance of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Entity optional checks")]
        public bool HoldTargetEntity { get; set; } = true;

        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Entity optional checks")]
        public bool SkipMoving { get; set; } = false;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity optional checks")]
        public float ReactionRange { get; set; } = 150;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Entity optional checks")]
        public List<string> CustomRegionNames { get; set; } = new List<string>();

        [Description("Distance to the Entity by which it is necessary to approach to disable 'IgnoreCombat' mode\n" +
            "Ignored if 'IgnoreCombat' does not True")]
        [Category("Interruptions")]
        public float CombatDistance { get; set; } = 30;

        [Description("Enable IgnoreCombat mode while distance to the closest Entity greater then 'CombatDistance'")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        ///// Внутренний список игнорируемых Entity, используемый, если InteractionRequirement=Once
        //[NonSerialized]
        //private TempBlackList<IntPtr> ignoredEntity;

        //[Description("True: Complite an action when the Bot has approached and interacted with the Entity\n" +
        //             "False: Continue executing the action after the Bot has approached and interacted with the Entity")]
        //[Category("Movement")]
        //public bool StopOnApproached { get; set; }

        //[Description("Select the need for interaction\n" +
        //    "Forbidden: No interaction is executed\n" +
        //    "IfPossible: The interaction is executed if Entity is interactable\n" +
        //    "Once: The interaction is executed only once, and the next time the Entity will be ignored\n" +
        //    "Obligatory: Interaction is strongly needed. The interaction is repeated until the target is intractable.")]
        //[Category("Interaction")]
        //public InteractionRequirement InteractionRequirement { get; set; }

        //[Description("Select the interaction method\n" +
        //    "Auto: Consistent use of all other interaction methods\n" +
        //    "NPC: Interact with an Entity as an NPC\n" +
        //    "Generic: Interact with an entity using generic interaction method\n" +
        //    "SimulateFKey: Force 'F' key press to interact an Entity\n" +
        //    "FollowAndInteractNPC: Follows an entity and interacts with it again and again as long as the interaction is possible\n" +
        //    "FollowAndSimulateFKey: Follows an entity and interacts with it by simulation 'F' key press again and again as long as the interaction is possible")]
        //[Category("Interaction")]
        //public InteractionMethod InteractionMethod { get; set; }

        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractitTimeout' period")]
        public bool InteractOnce { get; set; } = false;

        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set")]
        public int InteractingTimeout { get; set; } = 60;

        [Description("Time to interact (ms)")]
        [Category("Interaction")]
        public int InteractTime { get; set; } = 2000;

        //[Description("True: The attempt of the interaction is done prior to entering InCombat\n" +
        //    "False: Interact after combat completed")]
        //[Category("Interaction")]
        //public bool TryInteractInCombat { get; set; } = true;

        [Description("Answers in dialog while interact with Entity")]
        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
        public List<string> Dialogs { get; set; } = new List<string>();

        [XmlIgnore]
        [Editor(typeof(EntityTestEditor), typeof(UITypeEditor))]
        [Description("Нажми на кнопку '...' чтобы увидеть тестовую информацию")]
        public string TestInfo { get; } = "Нажми на кнопку '...' чтобы увидеть больше =>";

        [NonSerialized]
        //(1) private TempBlackList<IntPtr> blackList = new TempBlackList<IntPtr>();
        /*(2)*/ private TempBlackList<uint> blackList = new TempBlackList<uint>();
        /*(3) private TempBlackList<BlackEntityDef> talked = new TempBlackList<BlackEntityDef>();*/
        /*(4) private List<BlackEntityDef> blackList = new List<BlackEntityDef>();*/

        [NonSerialized]
        private bool combat;
        [NonSerialized]
        private bool moved;
        [NonSerialized]
        internal Entity target = new Entity(IntPtr.Zero);
        [NonSerialized]
        private Vector3 initialPos = new Vector3();

        public override bool NeedToRun
        {
            get
            {
                if (!HoldTargetEntity || target == null || !target.IsValid || (HealthCheck && target.IsDead))
                {
                    Entity entity = EntitySelectionTools.FindClosestContactEntity(EntityID, EntityIdType, EntityNameType, 
                                                                HealthCheck, ReactionRange, RegionCheck, CustomRegionNames,
                                                                IsNotInBlackList);

                    if (entity != null && entity.IsValid)
                        target = new Entity(entity.Pointer);
                    else
                    {
                        target = null;
                        return false;
                    }
                }

                 if (target != null && target.IsValid && !(HealthCheck && target.IsDead))
                {
                    if (IgnoreCombat)
                    {
                        if (target.Location.Distance3DFromPlayer > CombatDistance)
                        {
                            Astral.Quester.API.IgnoreCombat = true;
                            return false;
                        }
                        else
                        {
                            Astral.Logic.NW.Attackers.List.Clear();
                            Astral.Quester.API.IgnoreCombat = false;
                        }
                    }
                    initialPos = target.Location/*.Clone()*/;
                    return true;
                }

                return false;
            }
        }

        public override ActionResult Run()
        {
            try
            {
                moved = false;
                combat = false;
                if (Approach.EntityForInteraction(target, new Func<Approach.BreakInfos>(CheckCombat)))
                {
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
                                return ActionResult.Fail;
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
                            goto IL_FA;
                        }
                        return ActionResult.Fail;
                    }
                    IL_FA:
                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                    return ActionResult.Completed;
                }
                if (combat)
                {
                    return ActionResult.Running;
                }
                if (!SkipMoving || moved)
                {
                    Logger.WriteLine("Entity moved, skip...");
                    return ActionResult.Fail;
                }
                return ActionResult.Fail;
            }
            finally
            {
                if (InteractOnce)
                    PushToBlackList(target);
                target = new Entity(IntPtr.Zero);
            }
        }

        internal bool IsNotInBlackList(Entity ent)
        {
            /* 2 */
            return !blackList.Contains(ent.ContainerId);

            /* 4
            BlackEntityDef def = blackList.Find(x => x.Equals(ent));
            if (def != null && def.IsTimedOut)
            {
                blackList.Remove(def);
                return true;
            }
            return def == null; */
        }

        internal void PushToBlackList(Entity ent)
        {
            /* 2 */
            blackList.Add(target.ContainerId, InteractingTimeout);
        }

        private Approach.BreakInfos CheckCombat()
        {
            if (Attackers.InCombat)
            {
                combat = true;
                return Approach.BreakInfos.ApproachFail;
            }
            if (SkipMoving && target.Location.Distance3D(initialPos) > 3.0)
            {
                moved = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }

        public InteractEntities() { }
        public override string ActionLabel => $"{GetType().Name} [{EntityID}]";
        public override string InternalDisplayName => GetType().Name;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (target != null && target.IsValid
                    && IgnoreCombat && (target.Location.Distance3DFromPlayer > CombatDistance))
                        return target.Location.Clone();
                return new Vector3();
            }
        }
        public override bool UseHotSpots => true;
        protected override bool IntenalConditions => !string.IsNullOrEmpty(EntityID);
        public override void OnMapDraw(GraphicsNW graph)
        {
            if (target != null && target.IsValid)
            {
                Brush beige = Brushes.Beige;
                graph.drawFillEllipse(target.Location, new Size(10, 10), beige);
            }
        }
        public override void InternalReset()
        {
            target = null;
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
            XtraMessageBox.Show("Target Entity and press ok.");
            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (betterEntityToInteract.IsValid)
            {
                if (EntityNameType == EntityNameType.NameUntranslated)
                    EntityID = betterEntityToInteract.NameUntranslated;
                else EntityID = betterEntityToInteract.InternalName;
                if (base.HotSpots.Count == 0)
                {
                    base.HotSpots.Add(betterEntityToInteract.Location.Clone());
                }
            }
            if (XtraMessageBox.Show("Add a dialog ? (open the dialog window before)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DialogEdit.Show(Dialogs);
            }
        }
    }

    /// <summary>
    /// Упрощенное описание игнорируемых Entity для черного списка 
    /// </summary>
    internal class BlackEntityDef
    {
        public BlackEntityDef(Entity ent, int t = 0)
        {
            if (ent != null)
            {
                id = ent.ContainerId;
                ptr = ent.Pointer;
                pos = ent.Location.Clone();
            }
            else
            {
                id = 0;
                ptr = IntPtr.Zero;
                pos = new Vector3();
            }

            if (t > 0)
                timeout = new Astral.Classes.Timeout(t);
        }

        internal Astral.Classes.Timeout timeout;
        internal uint id;
        internal IntPtr ptr;
        internal Vector3 pos;

        public bool IsTimedOut
        {
            get
            {
                if(timeout != null)
                    return timeout.IsTimedOut;
                return false;
            }
        }

        public bool Equals(BlackEntityDef bkEnt)
        {
            return id == bkEnt.id
                    || ptr == bkEnt.ptr
                    || pos.Distance3D(bkEnt.pos) <= 1;
        }

        public override bool Equals(object obj)
        {
            if (obj is BlackEntityDef bkEnt)
            {
                return id == bkEnt.id
                    || ptr == bkEnt.ptr
                    || pos.Distance3D(bkEnt.pos) <= 1;
            }
            if (obj is Entity ent)
            {
                return id == ent.ContainerId
                    || ptr == ent.Pointer
                    || pos.Distance3D(ent.Location) <= 1;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() & ptr.GetHashCode() & pos.GetHashCode();
            //return base.GetHashCode();
        }
    }
}
