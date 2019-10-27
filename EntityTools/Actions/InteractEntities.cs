using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms;
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

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID { get; set; } = string.Empty;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType { get; set; } = EntityNameType.InternalName;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("Entity")]
        public bool RegionCheck { get; set; }

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Entity")]
        public bool HealthCheck { get; set; } = true;

        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Entity")]
        public bool SkipMoving { get; set; } = false;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("Entity")]
        public float ReactionRange { get; set; } = 150;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public List<string> CustomRegionNames { get; set; } = new List<string>();

        //[Description("Distance to the Entity by which it is necessary to approach")]
        //[Category("Movement")]
        //public float Distance { get; set; }

        //[Description("Enable IgnoreCombat profile value while playing action")]
        //[Category("Movement")]
        //public bool IgnoreCombat { get; set; }

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
        public bool InteractitOnce { get; set; } = false;

        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set")]
        public int InteractitTimeout { get; set; } = 60;

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

        [NonSerialized]
        private TempBlackList<IntPtr> talked = new TempBlackList<IntPtr>();
        [NonSerialized]
        private bool combat;
        [NonSerialized]
        private bool moved;
        [NonSerialized]
        private Entity target = new Entity(IntPtr.Zero);
        [NonSerialized]
        private Vector3 initialPos = new Vector3();

        public override bool NeedToRun
        {
            get
            {
                Vector3 playerLoc = EntityManager.LocalPlayer.Location;
                target = EntitySelectionTools.FindClosestContactEntity(EntityID, EntityIdType, EntityNameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, talked);

                if (target != null && target.IsValid)
                {
                    initialPos = target.Location/*.Clone()*/;
                    return true;
                }

                return false;
            }
        }

        public override ActionResult Run()
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
                if (InteractitOnce)
                {
                    talked.Add(target.Pointer, InteractitTimeout);
                }
                target = new Entity(IntPtr.Zero);
                return ActionResult.Completed;
            }
            if (combat)
            {
                return ActionResult.Running;
            }
            if (moved)
            {
                Logger.WriteLine("Entity moved, skip...");
                talked.Add(target.Pointer, InteractitTimeout);
                return ActionResult.Fail;
            }
            return ActionResult.Fail;
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
        protected override Vector3 InternalDestination => new Vector3();
        public override bool UseHotSpots => true;
        protected override bool IntenalConditions => true;
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
            target = new Entity(IntPtr.Zero);
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
}
