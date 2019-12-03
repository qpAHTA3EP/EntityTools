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
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Logger;

namespace EntityTools.Actions
{
    [Serializable]
    public class InteractEntities : Astral.Quester.Classes.Action
    {
#if PROFILING
        public static int RunCount = 0;
        public static void ResetWatch()
        {
            RunCount = 0;
            Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesCachedTimeout::ResetWatch()");
        }

        public static void LogWatch()
        {
            if (RunCount > 0)
                Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesCachedTimeout: RunCount: {RunCount}");
        }
#endif
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
        [Category("Optional")]
        public bool RegionCheck { get; set; } = false;

        [Description("Check if Entity's health greater than zero:\n" +
            "True: Only Entities with nonzero health are detected\n" +
            "False: Entity's health does not checked during search")]
        [Category("Optional")]
        public bool HealthCheck { get; set; } = true;

        [Description("True: Do not change the target Entity while it is alive or until the Bot within Distance of it\n" +
                    "False: Constantly scan an area and target the nearest Entity")]
        [Category("Optional")]
        public bool HoldTargetEntity { get; set; } = true;

        [Description("Check if Entity is moving:\n" +
            "True: Only standing Entities are detected\n" +
            "False: Both moving and stationary Entities are detected")]
        [Category("Optional")]
        public bool SkipMoving { get; set; } = false;

        [Description("The maximum distance from the character within which the Entity is searched\n" +
            "The value equals 0(zero) disables distance checking")]
        [Category("Optional")]
        public float ReactionRange { get; set; } = 150;

        [XmlIgnore]
        private List<string> customRegionNames = null;
        [XmlIgnore]
        private List<CustomRegion> customRegions = null;

        [Description("CustomRegion names collection")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if(customRegionNames != value)
                {
                    if (value != null
                        && value.Count > 0)
                        customRegions = Astral.Quester.API.CurrentProfile.CustomRegions.FindAll((CustomRegion cr) =>
                                    value.Exists((string regName) => regName == cr.Name));
                    else customRegions = null;
                    customRegionNames = value;
                }
            }
        }

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(MultiCustomRegionSelectEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType { get; set; } = EntitySetType.Contacts;

        [Description("Distance to the Entity by which it is necessary to approach to disable 'IgnoreCombat' mode\n" +
            "Ignored if 'IgnoreCombat' does not True")]
        [Category("Interruptions")]
        public float CombatDistance { get; set; } = 30;

        [Description("Enable IgnoreCombat mode while distance to the closest Entity greater then 'CombatDistance'")]
        [Category("Interruptions")]
        public bool IgnoreCombat { get; set; } = true;

        [Category("Interaction")]
        [Description("Only one interaction with Entity is possible in 'InteractitTimeout' period")]
        public bool InteractOnce { get; set; } = false;

        [Category("Interaction")]
        [Description("Interaction timeout (sec) if InteractitOnce flag is set")]
        public int InteractingTimeout { get; set; } = 60;

        [Description("Time to interact (ms)")]
        [Category("Interaction")]
        public int InteractTime { get; set; } = 2000;

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

        [XmlIgnore]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(500);

        [Description("Time between searches of the Entity (ms)")]
        public int SearchTimeInterval { get; set; } = 500;

        public override bool NeedToRun
        {
            get
            {
                if (timeout.IsTimedOut 
                    && (!HoldTargetEntity || target == null || !target.IsValid || (HealthCheck && target.IsDead)))
                {
                    Entity closestEntity = SearchCached.FindClosestEntity(EntityID, EntityIdType, EntityNameType, EntitySetType,
                                                                HealthCheck, ReactionRange, RegionCheck, customRegions,
                                                                IsNotInBlackList);
                    timeout.ChangeTime(SearchTimeInterval);

                    if (closestEntity != null && closestEntity.IsValid)
                        target = new Entity(closestEntity.Pointer);
                    else
                    {
                        target = null;
                        if (IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = true;
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
                else if(IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = true;

                return false;
            }
        }

        public override ActionResult Run()
        {
            try
            {
#if PROFILING
                RunCount++;
#endif
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
                if (IgnoreCombat && target.Location.Distance3DFromPlayer <= CombatDistance)
                {
                    Astral.Quester.API.IgnoreCombat = false;
                }
                if (combat)
                {
                    return ActionResult.Running;
                }
                if (moved)
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
        public override string InternalDisplayName => string.Empty;
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
        public override void InternalReset() { }
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
            //XtraMessageBox.Show("Target Entity and press ok.");
            TargetSelectForm.TargetGuiRequest("Target Entity and press ok.");
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
