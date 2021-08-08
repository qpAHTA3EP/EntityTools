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
using EntityCore.Forms;
using EntityCore.Tools.Navigation;

namespace EntityCore.Quester.Action
{
    public class InteractEntitiesEngine : IEntityInfos, IQuesterActionEngine
    {
        InteractEntities @this;

        #region Данные ядра
        private Func<List<CustomRegion>> GetCustomRegions;
        private Predicate<Entity> CheckEntity;

        private readonly TempBlackList<uint> blackList = new TempBlackList<uint>();
        private bool combat;
        private bool moved;
        private Entity target;
        private Vector3 initialPos = Vector3.Empty;
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
        private List<CustomRegion> customRegions;
        private string label = string.Empty;
        private string actionIDstr = string.Empty;
        #endregion

        public InteractEntitiesEngine(InteractEntities ie)
        {
#if false
            @this = ie;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;

            CheckEntity = initializer_CheckEntity;
            GetCustomRegions = initializer_GetCustomRegion;
            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']'); 
#else
            InternalRebase(ie);
#endif
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized: {ActionLabel}");
        }
        ~InteractEntitiesEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is InteractEntities ie)
            {
                if (InternalRebase(ie))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} rebase failed");
                return false;
            }
#if false
            else ETLogger.WriteLine(LogType.Debug, $"Rebase failed. '{action}' has type '{action.GetType().Name}' which not equals to '{nameof(InteractEntities)}'");
            return false; 
#else
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be casted to '" + nameof(InteractEntities) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
#endif

        }

        private bool InternalRebase(InteractEntities ie)
        {
            // Убираем привязку к старой команде
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = ie;
            @this.PropertyChanged += PropertyChanged;

            CheckEntity = initialize_CheckEntity;
            GetCustomRegions = initialize_GetCustomRegion;
            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            @this.Engine = this;

            return true;
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, @this)) return;
            switch (e.PropertyName)
            {
                case nameof(@this.EntityID):
                    CheckEntity = initialize_CheckEntity;
                    label = string.Empty;
                    break;
                case nameof(@this.EntityIdType):
                    CheckEntity = initialize_CheckEntity;
                    break;
                case nameof(@this.EntityNameType):
                    CheckEntity = initialize_CheckEntity; 
                    break;
                case nameof(@this.CustomRegionNames):
                    GetCustomRegions = initialize_GetCustomRegion;
                    break;
            }

            target = null;
            timeout.ChangeTime(0);
        }

        public bool NeedToRun
        {
            get
            {
                Entity closestEntity = null;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {
                    closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, GetCustomRegions(), IsNotInBlackList);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::{MethodBase.GetCurrentMethod().Name}: Found Entity[{closestEntity.ContainerId:X8}] (closest)");
                    
#endif
                    timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
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
                        Astral.Logic.NW.Attackers.List.Clear();
                        Astral.Quester.API.IgnoreCombat = false;
                    }
                    initialPos = target.Location/*.Clone()*/;
                    return true;
                }
                if (@this._ignoreCombat && ValidateEntity(closestEntity)
                    && !(@this._healthCheck && closestEntity.IsDead)
                    && closestEntity.Location.Distance3DFromPlayer <= @this._combatDistance)
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
                //TODO: Заменить EntityForInteraction собственной функцией перемещения и взаимодействия
                var interactDistance = Math.Max(@this._interactDistance, 5);
                if (target?.Location.Distance3DFromPlayer < interactDistance
#if true
                    || Approach.EntityForInteraction(target, internal_BreakInteraction))
#else
                    || Approach.EntityByDistance(target, @this._interactDistance, CheckCombat))  
#endif

                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{target.ContainerId:X8}]");
#endif
#if true
                    target.SmartInteract(interactDistance, @this._interactTime);
                    //Thread.Sleep(@this._interactTime);
#else
                    target.Interact();
                    Thread.Sleep(@this._interactTime);
#endif
                    if (@this._dialogs.Count > 0)
                    {
                        Astral.Classes.Timeout timer = new Astral.Classes.Timeout(5000);
                        while (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Options.Count == 0)
                        {
                            if (timer.IsTimedOut)
                            {
                                return ActionResult.Fail;
                            }
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(500);
                        foreach (string key in @this._dialogs)
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
                if(@this._resetCurrentHotSpot)
                    @this.CurrentHotSpotIndex = -1;
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
                    return EntityManager.LocalPlayer.Location.Clone();
                }
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            target = null;
            CheckEntity = EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            GetCustomRegions = initialize_GetCustomRegion;
            label = string.Empty;
        }

        public void GatherInfos()
        {
            if (@this.HotSpots.Count == 0)
                @this.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            if (string.IsNullOrEmpty(@this._entityId))
                EntitySelectForm.GUIRequest(ref @this._entityId, ref @this._entityIdType, ref @this._entityNameType);

            label = string.Empty;
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (ValidateEntity(target))
                graph.drawFillEllipse(target.Location, new Size(10, 10), Brushes.Beige);
        }

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
                                                             @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, GetCustomRegions(), IsNotInBlackList);

#else
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                             false, 0, 0, @this._regionCheck, GetCustomRegions(), IsNotInBlackList);
#endif
            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, GetCustomRegions(), IsNotInBlackList);
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
#if false
            return e != null && e.IsValid
                    //&& e.Critter.IsValid  <- Некоторые Entity, например игроки, имеют априори невалидный Critter
                    && CheckEntity(e); 
#else
            return e != null && e.IsValid
                    && (e.Character.IsValid || e.Critter.IsValid || e.Player.IsValid)
                    && CheckEntity(e);
#endif
        }

        private bool initialize_CheckEntity(Entity e)
        {
            Predicate<Entity> predicate = EntityComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Comparer does not defined. Initialize.");
#endif
                CheckEntity = predicate;
                return e!= null && CheckEntity(e);
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

        /// <summary>
        /// Методы, используемый для прерывания взаимодействия с целевой Entity
        /// </summary>
        /// <returns></returns>
        private Approach.BreakInfos internal_BreakInteraction()
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

        internal List<CustomRegion> initialize_GetCustomRegion()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                GetCustomRegions = internal_GetCustomRegion_Getter;
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
