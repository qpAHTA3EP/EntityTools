#define DEBUG_INTERACTENTITIES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Astral;
using Astral.Classes;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityTools.Extentions;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using static Astral.Quester.Classes.Action;
using EntityCore;
using System.Windows.Forms;
using EntityCore.Forms;
using DevExpress.XtraEditors;
using EntityTools.Core.Interfaces;

namespace EntityCore.Quester.Action
{
    public class InteractEntitiesEngine_delegates : IQuesterActionEngine, IEntityInfos
    {
        InteractEntities @this = null;
        private TempBlackList<uint> blackList = new TempBlackList<uint>();
        private bool combat;
        private bool moved;
        private Entity target = new Entity(IntPtr.Zero);
        private Vector3 initialPos = new Vector3();
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        private Predicate<Entity> Comparer { get; set; } = null;
        private List<CustomRegion> customRegions = new List<CustomRegion>();
        private string label = string.Empty;

        public InteractEntitiesEngine_delegates(InteractEntities ie)
        {
            @this = ie;
            @this.coreNeedToRun = NeedToRun;
            @this.coreRun = Run;
            @this.coreInternalConditions = Validate;
            @this.coreReset = Reset;
            @this.coreGatherInfos = GatherInfos;
            @this.coreLabel = Label;
            @this.coreTarget = Target;

            @this.PropertyChanged += PropertyChanged;
        }

        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case "EntityID":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        label = string.Empty;
                        break;
                    case "EntityIdType":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        break;
                    case "EntityNameType":
                        Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                        break;
                    case "CustomRegionNames":
                        customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);
                        break;
                }
            }
        }

        public bool NeedToRun()
        {
            if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

            if (Comparer == null && !string.IsNullOrEmpty(@this.EntityID))
            {
#if DEBUG
                Logger.WriteLine(Logger.LogType.Debug, "InteractEntitiesEngine::NeedToRun: Comparer is null. Initialize.");
#endif
                Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
            }

            Entity closestEntity = null;
            if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
            {
                closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                               @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, customRegions, IsNotInBlackList);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::NeedToRun: Found Entity[{closestEntity.ContainerId.ToString("X8")}] (closest)");
#endif
                timeout.ChangeTime(@this.SearchTimeInterval);
            }

            if (!@this._holdTargetEntity || !ValidateEntity(target) || (@this._healthCheck && target.IsDead))
                target = closestEntity;

            if (ValidateEntity(target) && !(@this._healthCheck && target.IsDead))
            {
                if (@this.IgnoreCombat)
                {
                    if (target.Location.Distance3DFromPlayer > @this._combatDistance)
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
            else if (@this._ignoreCombat && ValidateEntity(closestEntity)
                     && !(@this._healthCheck && closestEntity.IsDead)
                     && (closestEntity.Location.Distance3DFromPlayer <= @this._combatDistance))
            {
                Astral.Logic.NW.Attackers.List.Clear();
                Astral.Quester.API.IgnoreCombat = false;
            }
            else if (@this._ignoreCombat)
                Astral.Quester.API.IgnoreCombat = true;

            return false;
        }

        public ActionResult Run()
        {
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
                moved = false;
                combat = false;
#if DEBUG && DEBUG_INTERACTENTITIES
                Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Approach Entity[{target.ContainerId.ToString("X8")}] for interaction");
#endif
                if (Approach.EntityForInteraction(target, CheckCombat/*new Func<Approach.BreakInfos>(CheckCombat)*/))
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{target.ContainerId.ToString("X8")}]");
#endif
                    target.Interact();
                    Thread.Sleep(@this._interactTime);
                    Interact.WaitForInteraction();
                    if (@this.Dialogs.Count > 0)
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
                        foreach(string key in @this.Dialogs)
                        {
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                            Thread.Sleep(1000);
                        }
                    }
                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                    return ActionResult.Completed;
                }
                if (@this._ignoreCombat && target.Location.Distance3DFromPlayer <= @this._combatDistance)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Engage combat");
#endif
                    Astral.Quester.API.IgnoreCombat = false;
                    return ActionResult.Running;
                }
                if (combat)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Player in combat...");
#endif
                    return ActionResult.Running;
                }
                if (moved)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    Logger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Entity[{target.ContainerId.ToString("X8")}] moved, skip...");
#else
                    Logger.WriteLine("Entity moved, skip...");
#endif
                    return ActionResult.Fail;
                }
                return ActionResult.Fail;
            }
            finally
            {
                if (@this._interactOnce || (@this._skipMoving && moved))
                {
                    PushToBlackList(target);

                    target = new Entity(IntPtr.Zero);
                }
            }
        }

        private bool IsNotInBlackList(Entity ent)
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

        private void PushToBlackList(Entity ent)
        {
            /* 2 */
            blackList.Add(target.ContainerId, @this._interactingTimeout);
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
            if (@this._skipMoving && target.Location.Distance3D(initialPos) > 3.0)
            {
                moved = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }

        public Entity Target()
        {
            return target;
        }

        public bool TargetValidate()
        {
            return ValidateEntity(target);
        }
        public bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && Comparer?.Invoke(e) == true;
        }

        public void Reset()
        {
            combat = false;
            moved = false;
            target = null;
            initialPos = new Vector3();
            timeout.ChangeTime(0);
        }

        public void GatherInfos()
        {
            //XtraMessageBox.Show("Target Entity and press ok.");
            //Form editor = Application.OpenForms.Find<Astral.Quester.Forms.Editor>();
            TargetSelectForm.TargetGuiRequest("Target Entity and press ok."/*, editor*/);
            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
            if (betterEntityToInteract.IsValid)
            {
                if (@this._entityNameType == EntityNameType.NameUntranslated)
                    @this._entityId = betterEntityToInteract.NameUntranslated;
                else @this._entityId = betterEntityToInteract.InternalName;
                if (@this.HotSpots.Count == 0)
                    @this.HotSpots.Add(betterEntityToInteract.Location.Clone());
            }
            if (XtraMessageBox.Show(/*editor, */"Add a dialog ? (open the dialog window before)", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Astral.Quester.UIEditors.Forms.DialogEdit.Show(@this.Dialogs);
        }

        public string Label()
        {
            if(string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} [{@this._entityId}]";
            return label;
        }

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            Reset();
            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            sb.Append("EntitySetType: ").AppendLine(@this._entitySetType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            {
                sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
                for (int i = 1; i < @this._customRegionNames.Count; i++)
                    sb.Append(", ").Append(@this._customRegionNames[i]);
                sb.AppendLine("}");
            }
            sb.AppendLine();
            //sb.Append("NeedToRun: ").AppendLine(NeedToRun.ToString());
            sb.AppendLine();

            // список всех Entity, удовлетворяющих условиям
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                                     @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, customRegions);


            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, customRegions, IsNotInBlackList);
            if (target != null && target.IsValid)
            {
                sb.Append("Target: ").AppendLine(target.ToString());
                sb.Append("\tName: ").AppendLine(target.Name);
                sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
                sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }

        public bool InternalConditions()
        {
            return !string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty;
        }

        public ActionValidity ActionValidity()
        {
            if (!InternalConditions())
                return new ActionValidity($"'{nameof(@this.EntityID)}' property not valid.");
            return new ActionValidity();
        }
    }
}
