﻿#define DEBUG_INTERACTENTITIES
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
#if DEBUG && PROFILING
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
        [Description("ID of the Entity for the search")]
        [Editor(typeof(EntityIdEditor), typeof(UITypeEditor))]
        [Category("Entity")]
        public string EntityID
        {
            get => entityId;
            set
            {
                if (entityId != value)
                {
                    entityId = value;
                    if (!string.IsNullOrEmpty(entityId))
                        Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                    else Comparer = null;

                }
            }
        }

        [Description("Type of the EntityID:\n" +
            "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
            "Regex: Regular expression")]
        [Category("Entity")]
        public ItemFilterStringType EntityIdType
        {
            get => entityIdType;
            set
            {
                if(entityIdType != value)
                { 
                    entityIdType = value;
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }
            }
        }

        [Description("The switcher of the Entity filed which compared to the property EntityID")]
        [Category("Entity")]
        public EntityNameType EntityNameType
        {
            get => entityNameType;
            set
            {
                if(entityNameType != value)
                { 
                    entityNameType = value;
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }
            }
        }

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

        [Description("The maximum ZAxis difference from the withing which the Entity is searched\n" +
            "The default value is 0, which disables ZAxis checking")]
        [Category("Optional")]
        public float ReactionZRange { get; set; } = 0;

        [Description("CustomRegion names collection")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public List<string> CustomRegionNames
        {
            get => customRegionNames;
            set
            {
                if (customRegionNames != value)
                {
                    customRegions = CustomRegionTools.GetCustomRegions(value);
                    customRegionNames = value;
                }
            }
        }

        [Description("A subset of entities that are searched for a target\n" +
            "Contacts: Only interactable Entities\n" +
            "Complete: All possible Entities")]
        [Editor(typeof(CustomRegionListEditor), typeof(UITypeEditor))]
        [Category("Optional")]
        public EntitySetType EntitySetType { get; set; } = EntitySetType.Contacts;

        [Description("Time between searches of the Entity (ms)")]
        [Category("Optional")]
        public int SearchTimeInterval { get; set; } = 100;

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

        [XmlIgnore]
        [Browsable(false)]
        public override string Category => "Basic";

        [XmlIgnore]
        internal EntityComparerToPattern Comparer { get; private set; } = null;

        [NonSerialized]
        //(1) private TempBlackList<IntPtr> blackList = new TempBlackList<IntPtr>();
        /*(2)*/
        private TempBlackList<uint> blackList = new TempBlackList<uint>();
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

        [NonSerialized]
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        [NonSerialized]
        private ItemFilterStringType entityIdType = ItemFilterStringType.Simple;
        [NonSerialized]
        private string entityId = string.Empty;
        [NonSerialized]
        private EntityNameType entityNameType = EntityNameType.InternalName;
        [NonSerialized]
        private List<string> customRegionNames = new List<string>();
        [NonSerialized]
        private List<CustomRegion> customRegions = new List<CustomRegion>();


        public override bool NeedToRun
        {
            get
            {
                if (customRegionNames != null && (customRegions == null || customRegions.Count != customRegionNames.Count))
                    customRegions = CustomRegionTools.GetCustomRegions(customRegionNames);

                if (Comparer == null && !string.IsNullOrEmpty(entityId))
                {
#if DEBUG
                    Logger.WriteLine(Logger.LogType.Debug, "InteractEntities::NeedToRun: Comparer is null. Initialize.");
#endif
                    Comparer = new EntityComparerToPattern(entityId, entityIdType, entityNameType);
                }

                Entity closestEntity = null;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {
                    closestEntity = SearchCached.FindClosestEntity(entityId, entityIdType, entityNameType, EntitySetType.Complete,
                                                                HealthCheck, ReactionRange, ReactionZRange, RegionCheck, customRegions, IsNotInBlackList);
#if DEBUG_INTERACTENTITIES
                    if(closestEntity!=null && closestEntity.IsValid)
                        Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::NeedToRun: Found Entity[{closestEntity.ContainerId.ToString("X8")}] (closest)");
#endif
                    timeout.ChangeTime(SearchTimeInterval);
                }

                if (!HoldTargetEntity || !Validate(target) || (HealthCheck && target.IsDead))
                    target = closestEntity;

                if (Validate(target) && !(HealthCheck && target.IsDead))
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
                else if (IgnoreCombat && Validate(closestEntity)
                         && !(HealthCheck && closestEntity.IsDead)
                         && (closestEntity.Location.Distance3DFromPlayer <= CombatDistance))
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    Astral.Quester.API.IgnoreCombat = false;
                }
                else if (IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = true;

                return false;
            }
        }

        public override ActionResult Run()
        {
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
                moved = false;
                combat = false;
#if DEBUG && DEBUG_INTERACTENTITIES
                Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::Run: Approach Entity[{target.ContainerId.ToString("X8")}] for interaction");
#endif
                if (Approach.EntityForInteraction(target, new Func<Approach.BreakInfos>(CheckCombat)))
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::Run: Interact Entity[{target.ContainerId.ToString("X8")}]");
#endif
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
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::Run: Engage combat");
#endif
                    Astral.Quester.API.IgnoreCombat = false;
                    return ActionResult.Running;
                }
                if (combat)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::Run: Player in combat...");
#endif
                    return ActionResult.Running;
                }
                if (moved)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::Run: Entity[{target.ContainerId.ToString("X8")}] moved, skip...");
#else
                    Logger.WriteLine("Entity moved, skip...");
#endif
                    return ActionResult.Fail;
                }
                return ActionResult.Fail;
            }
            finally
            {
                if (InteractOnce || (SkipMoving && moved))
                {
                    PushToBlackList(target);

                    target = new Entity(IntPtr.Zero);
                }
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
#if DEBUG && DEBUG_INTERACTENTITIES
            Logger.WriteLine(Logger.LogType.Debug, $"{GetType().Name}::PushToBlackList: Entity[{target.ContainerId.ToString("X8")}]");
#endif
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

        private bool Validate(Entity e)
        {
            return e != null && e.IsValid && Comparer.Check(e);
        }

        public override string ActionLabel => $"{GetType().Name} [{entityId}]";
        public override string InternalDisplayName => string.Empty;
        protected override Vector3 InternalDestination
        {
            get
            {
                if (Validate(target)
                    && IgnoreCombat && (target.Location.Distance3DFromPlayer > CombatDistance))
                    return target.Location.Clone();
                return new Vector3();
            }
        }
        public override bool UseHotSpots => true;
        protected override bool IntenalConditions => Comparer != null;//!string.IsNullOrEmpty(EntityID);
        public override void OnMapDraw(GraphicsNW graph)
        {
            if (Validate(target))
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
                if (string.IsNullOrEmpty(entityId))
                {
                    return new ActionValidity("EntityID property not set.");
                }
                return new ActionValidity();
            }
        }
        public override void GatherInfos()
        {
            //XtraMessageBox.Show("Target Entity and press ok.");
            Form editor = Application.OpenForms.Find<Astral.Quester.Forms.Editor>();
            TargetSelectForm.TargetGuiRequest("Target Entity and press ok.", editor);
            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (betterEntityToInteract.IsValid)
            {
                if (EntityNameType == EntityNameType.NameUntranslated)
                    entityId = betterEntityToInteract.NameUntranslated;
                else entityId = betterEntityToInteract.InternalName;
                if (base.HotSpots.Count == 0)
                {
                    base.HotSpots.Add(betterEntityToInteract.Location.Clone());
                }
            }
            if (XtraMessageBox.Show(editor, "Add a dialog ? (open the dialog window before)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DialogEdit.Show(Dialogs);
            }
        }

        public InteractEntities() { }
    }
}
