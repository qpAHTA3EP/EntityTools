//#define DEBUG_INTERACTENTITIES

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Astral;
using Astral.Classes;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityCore.Extensions;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using EntityTools.Core.Interfaces;
using Astral.Logic.Classes.Map;
using System.Drawing;
using static Astral.Quester.Classes.Action;
using EntityTools;
using System.Reflection;

namespace EntityCore.Quester.Action
{
    public class InteractEntitiesEngine : IEntityInfos
#if CORE_INTERFACES
        , IQuesterActionEngine
#endif
    {
        InteractEntities @this = null;

        #region Данные ядра
        private Func<List<CustomRegion>> getCustomRegions = null;
        private Predicate<Entity> checkEntity = null;

        private TempBlackList<uint> blackList = new TempBlackList<uint>();
        private bool combat;
        private bool moved;
        private Entity target = null;
        private Vector3 initialPos = new Vector3();
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        private List<CustomRegion> customRegions = null;
        private string label = string.Empty; 
        #endregion

        public InteractEntitiesEngine(InteractEntities ie)
        {
            @this = ie;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {ActionLabel}");
        }

        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                switch (e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        label = string.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = internal_CheckEntity_Initializer; //EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "EntityNameType":
                        checkEntity = internal_CheckEntity_Initializer; //EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "CustomRegionNames":
                        getCustomRegions = internal_GetCustomRegion_Initializer; //CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
                        break;
                }

                target = null;
                timeout.ChangeTime(0);
            }
        }

#if CORE_DELEGATES
        #region IQuesterActionEngine
        public bool NeedToRun
        {
            get
            {
                if (@this.CustomRegionNames != null && (customRegions == null || customRegions.Count != @this.CustomRegionNames.Count))
                    customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

                if (checkEntity == null && !string.IsNullOrEmpty(@this.EntityID))
                {
#if DEBUG
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, "InteractEntitiesEngine::NeedToRun: Comparer is null. Initialize.");
#endif
                    checkEntity = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                }

                Entity closestEntity = null;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {
                    closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, customRegions, IsNotInBlackList);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::NeedToRun: Found Entity[{closestEntity.ContainerId:X8}] (closest)");
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
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Approach Entity[{target.ContainerId:X8}] for interaction");
#endif
                if (Approach.EntityForInteraction(target, CheckCombat/*new Func<Approach.BreakInfos>(CheckCombat)*/))
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{target.ContainerId:X8}]");
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
                        foreach (string key in @this.Dialogs)
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
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Engage combat");
#endif
                    Astral.Quester.API.IgnoreCombat = false;
                    return ActionResult.Running;
                }
                if (combat)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Player in combat...");
#endif
                    return ActionResult.Running;
                }
                if (moved)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    EntityToolsLogger.WriteLine(Logger.LogType.Debug, $"InteractEntitiesEngine::Run: Entity[{target.ContainerId:X8}] moved, skip...");
#else
                    EntityToolsLogger.WriteLine("Entity moved, skip...");
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

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = $"{@this.GetType().Name} [{@this._entityId}]";
                return label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                return !string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty;
            }
        }

        public ActionValidity InternalValidity
        {
            get
            {
                if (!InternalConditions)
                    return new ActionValidity($"'{nameof(@this.EntityID)}' property not valid.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => true;

        public Vector3 InternalDestination
        {
            get
            {
                throw new NotImplementedException();
            }
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
            return e != null && e.IsValid && checkEntity?.Invoke(e) == true;
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

        public void InternalReset()
        {
            throw new NotImplementedException();
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            throw new NotImplementedException();
        }
        #endregion
#endif
#if CORE_INTERFACES
        public bool NeedToRun
        {
            get
            {
                Entity closestEntity = null;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {
                    closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions(), IsNotInBlackList);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::{MethodBase.GetCurrentMethod().Name}: Found Entity[{closestEntity.ContainerId:X8}] (closest)");
                    
#endif
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                if (closestEntity != null && (!@this._holdTargetEntity || !ValidateEntity(target) || (@this._healthCheck && target.IsDead)))
                    target = closestEntity;

                if (ValidateEntity(target) && !(@this._healthCheck && target.IsDead))
                {
                    if (@this._ignoreCombat)
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
        }

        public ActionResult Run()
        {
            moved = false;
            combat = false;
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
#if DEBUG && DEBUG_INTERACTENTITIES
                ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Approach Entity[{target.ContainerId:X8}] for interaction");
#endif
                if (Approach.EntityForInteraction(target, CheckCombat/*new Func<Approach.BreakInfos>(CheckCombat)*/))
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{target.ContainerId:X8}]");
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
                        foreach (string key in @this.Dialogs)
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
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Engage combat");
#endif
                    Astral.Quester.API.IgnoreCombat = false;
                    return ActionResult.Running;
                }
                if (combat)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Player in combat...");
#endif
                    return ActionResult.Running;
                }
                if (moved)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Entity[{target.ContainerId:X8}] moved, skip...");
#else
                    ETLogger.WriteLine("Entity moved, skip...", true);
#endif
                    return ActionResult.Fail;
                }
                return ActionResult.Fail;
            }
            finally
            {
                // В случае неудачного интеракта из-за боя InteractOnce не должно помещать в черный список до повторной попытки
                if (!combat && @this._interactOnce || @this._skipMoving && moved)
                {
                    PushToBlackList(target);

                    target = new Entity(IntPtr.Zero);
                }
            }

        }

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(label))
                    label = $"{@this.GetType().Name} [{@this._entityId}]";
                return label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                return !string.IsNullOrEmpty(@this._entityId) || @this._entityNameType == EntityNameType.Empty;
            }
        }
        public ActionValidity InternalValidity
        {
            get
            {
                if (!InternalConditions)
                    return new ActionValidity($"{nameof(@this.EntityID)} property is not valid.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => true;
        public Vector3 InternalDestination
        {
            get
            {
                if (ValidateEntity(target))
                {
                    if (target.Location.Distance3DFromPlayer > @this._combatDistance)
                        return target.Location.Clone();
                    else return EntityManager.LocalPlayer.Location.Clone();
                }
                return new Vector3();
            }
        }

        public void InternalReset()
        {
            target = null;
            checkEntity = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            getCustomRegions = internal_GetCustomRegion_Initializer;
            label = String.Empty;
        }
        public void GatherInfos() { }
        public void OnMapDraw(GraphicsNW graph)
        {
            if (ValidateEntity(target))
                graph.drawFillEllipse(target.Location, new Size(10, 10), Brushes.Beige);
        }
#endif

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            InternalReset();
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
            //sb.AppendLine();

            // список всех Entity, удовлетворяющих условиям
#if false
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                             @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions(), IsNotInBlackList);

#else
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                             false, 0, 0, @this._regionCheck, getCustomRegions(), IsNotInBlackList);
#endif
            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions(), IsNotInBlackList);
            if (target != null && target.IsValid)
            {
                bool distOk = @this._reactionRange <= 0 || target.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(target.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !target.IsDead;
                sb.Append("ClosestEntity: ").Append(target.ToString());
                if (distOk && zOk && alive)
                    sb.AppendLine(" [MATCH]");
                else sb.AppendLine(" [MISMATCH]");
                sb.Append("\tName: ").AppendLine(target.Name);
                sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                sb.Append("\tIsDead: ").Append(target.IsDead.ToString());
                if (alive)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                sb.Append("\tDistance: ").Append(target.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
                if (zOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }

#region Вспомогательный функции
        public bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity(e);
        }


        private bool internal_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Comparer does not defined. Initialize.");
#endif
                checkEntity = predicate;
                return e!= null && checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Fail to initialize the Comparer.");
#endif
            return false;
        }


        private bool IsNotInBlackList(Entity ent)
        {
            return !blackList.Contains(ent.ContainerId);
        }

        private void PushToBlackList(Entity ent)
        {
            if (target != null && target.IsValid)
            {
                blackList.Add(target.ContainerId, @this._interactingTimeout);
#if DEBUG && DEBUG_INTERACTENTITIES
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::PushToBlackList: Entity[{target.ContainerId:X8}]");
#endif
            }
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

        internal List<CustomRegion> internal_GetCustomRegion_Initializer()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                getCustomRegions = internal_GetCustomRegion_Getter;
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
                return customRegions;
            }
            return null;
        }

        internal List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        }
        #endregion
    }
}
