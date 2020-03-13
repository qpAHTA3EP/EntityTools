

using Astral;
using Astral.Classes;
using Astral.Classes.ItemFilter;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using Astral.Quester.UIEditors;
using Astral.Quester.UIEditors.Forms;
using DevExpress.XtraEditors;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace EntityTools.Actions
{
    [Serializable]
    public class InteractNPCext : Astral.Quester.Classes.Action
    {
        [Description("Type of the EntityID:\n" +
            "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("NPC")]
        public ItemFilterStringType NpcIdType { get; set; } = ItemFilterStringType.Simple;

        [Description("ID (an internal untranslated name) of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("NPC")]
        public string NpcId { get; set; } = string.Empty;

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("NPC")]
        public EntityNameType NameType { get; set; } = EntityNameType.InternalName;

        [Description("Check Entity's Ingame Region (Not CustomRegion):\n" +
            "True: Only Entities located in the same Region as Player are detected\n" +
            "False: Entity's Region does not checked during search")]
        [Category("NPC")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("NPC")]
        public bool HealthCheck { get; set; } = true;

        [Description("Check if NPC is moving:\n" +
            "True: Only standing NPCs are detected\n" +
            "False: Both moving and stationary NPCs are detected")]
        [Category("NPC")]
        public bool SkipMoving { get; set; } = false;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The default value is 0, which disables distance checking")]
        [Category("NPC")]
        public float ReactionRange { get; set; } = 60;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("NPC")]
        public List<string> CustomRegionNames { get; set; }

        [Editor(typeof(DialogEditor), typeof(UITypeEditor))]
        [Category("Interaction")]
        public List<string> Dialogs { get; set; } = new List<string>();

        [Category("Interaction")]
        public bool OneInteractionByNpc { get; set; }

        [Category("Interaction")]
        public int InteractTime { get; set; } = 2000;

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
                target = EntitySelectionTools.FindClosestContactEntity(NpcId, NpcIdType, NameType, HealthCheck, ReactionRange, RegionCheck, CustomRegionNames, talked);

                if(target != null && target.IsValid)
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
                if (OneInteractionByNpc)
                {
                    talked.Add(target.Pointer, 60);
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
                Logger.WriteLine("NPC Moved, skip...");
                talked.Add(target.Pointer, 60);
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

        public InteractNPCext() { }
        public override string ActionLabel => $"{GetType().Name} [{NpcId}]";
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
                if (string.IsNullOrEmpty(NpcId))
                {
                    return new ActionValidity("EntityID property not set.");
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos()
        {
            XtraMessageBox.Show("Target npc and press ok.");
            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (betterEntityToInteract.IsValid)
            {
                if (NameType == EntityNameType.NameUntranslated)
                    NpcId = betterEntityToInteract.NameUntranslated;
                else NpcId = betterEntityToInteract.InternalName;
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