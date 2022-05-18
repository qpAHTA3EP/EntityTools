using AcTp0Tools;
using AStar;
using Astral.Classes;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using EntityCore.Entities;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityCore.Forms;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using EntityCore.Tools.Navigation;
using static Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Action
{
    public class InteractEntitiesEngine :  IQuesterActionEngine
    {
        InteractEntities @this;

        #region Данные ядра
        private readonly TempBlackList<uint> blackList = new TempBlackList<uint>();
        private Entity targetEntity;
        private Entity closestEntity;
        private readonly Astral.Classes.Timeout internalCacheTimer = new Astral.Classes.Timeout(0);
        private Astral.Classes.Timeout entityAbsenceTimer;
        private string _label = string.Empty;
        private string _idStr = string.Empty;
        #endregion

        public InteractEntitiesEngine(InteractEntities ie)
        {
            InternalRebase(ie);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {ActionLabel}");
        }
        ~InteractEntitiesEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.Unbind();
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
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }
            string debugStr =
                $"Rebase failed. {action.GetType().Name}[{action.ActionID}] can't be cast to '{nameof(InteractEntities)}'";
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(InteractEntities ie)
        {
            targetEntity = null;
            closestEntity = null;

            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = ie;
            @this.PropertyChanged += OnPropertyChanged;

            _key = null;
            _label = string.Empty;
            _specialCheck = null;

            _idStr = $"{@this.GetType().Name}[{@this.ActionID}]";

            @this.Bind(this);

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        }
        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (!ReferenceEquals(sender, @this)) return;
            _key = null;
            _label = string.Empty;
            _specialCheck = null;


            targetEntity = null;
            internalCacheTimer.ChangeTime(0);
        }

#if false
        public bool NeedToRun
        {
            get
            {
                var entityKey = EntityKey;

                if (internalCacheTimer.IsTimedOut)
                {
                    closestEntity = SearchCached.FindClosestEntity(entityKey,
                                                                   SpecialEntityCheck);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::{MethodBase.GetCurrentMethod().Name}: Found Entity[{closestEntity.ContainerId:X8}] (closest)");
                    
#endif
                    internalCacheTimer.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                if (closestEntity != null && (!@this.HoldTargetEntity || !entityKey.Validate(targetEntity) || (@this.HealthCheck && targetEntity.IsDead)))
                    targetEntity = closestEntity;

                var ignoreCombat = @this.IgnoreCombat;
                if (entityKey.Validate(targetEntity) && !(@this.HealthCheck && targetEntity.IsDead))
                {
                    if (ignoreCombat && CheckingIgnoreCombatCondition())
                    {
                        if (targetEntity.Location.Distance3DFromPlayer > @this.CombatDistance)
                        {
                            AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, @this.IgnoreCombatMinHP, 5_000);
                            return false;
                        }
                        Astral.Logic.NW.Attackers.List.Clear();
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                    }
                    _initialPos = targetEntity.Location.Clone();
                    return true;
                }
                if (ignoreCombat && entityKey.Validate(closestEntity)
                                 && !(@this.HealthCheck && closestEntity.IsDead)
                                 && closestEntity.Location.Distance3DFromPlayer <= @this.CombatDistance)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                }
                else if (ignoreCombat)
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, @this.IgnoreCombatMinHP, 5_000);

                return false;
            }
        }

        public ActionResult Run()
        {
            _moved = false;
            _combat = false;
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
#if DEBUG && DEBUG_INTERACTENTITIES
                ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Approach Entity[{targetEntity.ContainerId:X8}] for interaction");
#endif
                //TODO: Заменить EntityForInteraction собственной функцией перемещения и взаимодействия
                var interactDistance = Math.Max(@this.InteractDistance, 5);
                var ignoreCombat = @this.IgnoreCombat;
                if (targetEntity?.Location.Distance3DFromPlayer < interactDistance
#if true
                    || Approach.EntityForInteraction(targetEntity, BreakInteraction))
#else
                    || Approach.EntityByDistance(targetEntity, @this._interactDistance, CheckCombat))  
#endif

                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{targetEntity.ContainerId:X8}]");
#endif
#if true
                    targetEntity.SmartInteract(interactDistance, @this.InteractTime);
#else
                    targetEntity.Interact();
                    Thread.Sleep(@this._interactTime);
#endif
                    var dialogs = @this.Dialogs;
                    if (dialogs.Count > 0)
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
                        foreach (string key in dialogs)
                        {
                            EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                            Thread.Sleep(1000);
                        }
                    }
                    EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.Close();
                    return ActionResult.Completed;
                }
                if (ignoreCombat && targetEntity.Location.Distance3DFromPlayer <= @this.CombatDistance)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Engage combat");
#endif
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                    return ActionResult.Running;
                }
                if (_combat)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Player in combat...");
#endif
                    return ActionResult.Running;
                }
                if (_moved)
                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Entity[{targetEntity.ContainerId:X8}] moved, skip...");
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
                if (!_combat && @this.InteractOnce || @this.SkipMoving && _moved)
                {
                    PushToBlackList(targetEntity);
                    targetEntity = new Entity(IntPtr.Zero);
                }
                if (@this.ResetCurrentHotSpot)
                    @this.CurrentHotSpotIndex = -1;
            }
        }
#else // Новая реализация, аналогичная MoveToEntity
        public bool NeedToRun
        {
            get
            {
                bool extendedDebugInfo = ExtendedDebugInfo;
                string currentMethodName = extendedDebugInfo 
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun)}"
                    : string.Empty;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

                // Команда работает с 2 - мя целями:
                //   1-я цель (targetEntity) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(targetEntity);

                var entityNameType = @this.EntityNameType;
                var holdTargetEntity = @this.HoldTargetEntity;

                if (extendedDebugInfo)
                {
                    string debugMsg = targetEntity is null 
                        ? $"{currentMethodName}: Target[NULL] processing result: '{entityPreprocessingResult}'"
                        : $"{currentMethodName}: {targetEntity.GetDebugString(entityNameType, "Target", EntityDetail.Pointer)} processing result: '{entityPreprocessingResult}'";
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                {
                    // targetEntity не валидный - сбрасываем
                    targetEntity = null;
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded && internalCacheTimer.IsTimedOut)
                {
                    // targetEntity не был обработан
                    Entity entity = SearchCached.FindClosestEntity(EntityKey, SpecialEntityCheck);

                    if (entity != null)
                    {
                        ResetEntitySearchTimer();
                        closestEntity = entity;
                        bool closestIsTarget = targetEntity != null && targetEntity.ContainerId == closestEntity.ContainerId;

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Found {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}{(closestIsTarget ? " that equals to Target" : string.Empty)}");

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в targetEntity
                            if (extendedDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Change Target[INVALID] to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}");
                            }
                            targetEntity = closestEntity;
                        }
                        else if (!closestIsTarget)
                        {
                            // targetEntity не является ближайшей сущностью
                            if (!holdTargetEntity)
                            {
                                // Фиксация на targetEntity не требуется
                                // ближайшую цель можно сохранить в targetEntity
                                if (extendedDebugInfo)
                                {
                                    string debugMsg = targetEntity is null
                                        ? $"{currentMethodName}: Change Target[NULL] to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}"
                                        : $"{currentMethodName}: Change {targetEntity.GetDebugString(entityNameType, "Target", EntityDetail.Pointer)} to the {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)}";
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                targetEntity = closestEntity;
                            }

                            // обрабатываем ближайшую сущность closestEntity
                            entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                            if (extendedDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug,
                                        $"{currentMethodName}: ClosestEntity[NULL] processing result: '{entityPreprocessingResult}'");
                                else ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: {closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer)} processing result: '{entityPreprocessingResult}'");
                            }
                        }
                    }
                    else
                    {
                        ResetEntitySearchTimer();
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ClosestEntity not found");

                    }

                    internalCacheTimer.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                bool needToRun = entityPreprocessingResult == EntityPreprocessingResult.Succeeded;

                if (!needToRun)
                {
                    if (@this.IgnoreCombat && CheckingIgnoreCombatCondition())
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, @this.IgnoreCombatMinHP, 5_000);
                        if (@this.AbortCombatDistance > @this.InteractDistance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Engage combat and set abort combat condition");
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Disable combat");
                    }
                }

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Result '{needToRun}'");

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

            var entityKey = EntityKey;

            if (entityKey.Validate(closestEntity))
                Interact_Entity(closestEntity);
            else if (entityKey.Validate(targetEntity))
                Interact_Entity(targetEntity);

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult={ActionResult.Running}");

            //  таймера остановки поиска
            var entitySearchTime = @this.EntitySearchTime;
            if (entitySearchTime > 0)
                entityAbsenceTimer.ChangeTime(entitySearchTime * 1_000);

            if (@this.ResetCurrentHotSpot)
                @this.CurrentHotSpotIndex = -1;
            return ActionResult.Running;
        }

        #region Декомпозиция основного функционала
        /// <summary>
        /// Анализ <paramref name="entity"/> на предмет возможности вступления в бой
        /// </summary>
        private EntityPreprocessingResult Preprocessing_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Preprocessing_Entity)}" : string.Empty;

            bool validationResult = EntityKey.Validate(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthCheck = @this.HealthCheck;
                bool healthResult = !@this.HealthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= @this.InteractDistance;
                var entityNameType = @this.EntityNameType;
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entity.GetDebugString(entityNameType, "Entity", EntityDetail.Pointer), 
                        " verification=", healthResult && distanceResult, " {Valid; ", healthCheck ? (healthResult ? "Alive; " : "Dead; ") : "Skip; ",
                        distanceResult ? "Near (" : "Faraway (", distance.ToString("N2"), ")}"));

                if (!healthResult) return EntityPreprocessingResult.Failed;
                
                if (distanceResult)
                {
                    // entity в пределах заданного расстояния
                    return EntityPreprocessingResult.Succeeded;
                }
                return EntityPreprocessingResult.Faraway;
            }
            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity Verification=False {{Invalid}}");

            return EntityPreprocessingResult.Failed;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Взаимодействие с сущностью <paramref name="entity"/>
        /// </summary>
        private void Interact_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = string.Empty, 
                   entityDebugId = string.Empty;
            if (extendedDebugInfo)
            {
                currentMethodName = $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Interact_Entity)}";
                entityDebugId = entity.GetDebugString(@this.EntityNameType, "Entity", EntityDetail.Pointer);
            }
            
            if (entity is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity is NULL. Break");
                return;
            }

            var player = EntityManager.LocalPlayer.Player;

            _moved = false;
            _combat = false;
            try
            {
#if DEBUG && PROFILING
                RunCount++;
#endif
                var interactDistance = Math.Max(@this.InteractDistance, 5);
                var ignoreCombat = @this.IgnoreCombat;
                
                if (entity.Location.Distance3DFromPlayer < interactDistance)
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Approaching {entityDebugId} for interaction");

#if true
                    //TODO: Заменить EntityForInteraction собственной функцией перемещения и взаимодействия
                    if (Approach.EntityForInteraction(targetEntity, BreakInteraction)
                        && targetEntity.SmartInteract(interactDistance, @this.InteractTime))
#else
                    if (Approach.EntityByDistance(targetEntity, @this._interactDistance, CheckCombat)) 
#endif
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Interaction with {entityDebugId} succeeded");

#if false
                    targetEntity.Interact();
                    Thread.Sleep(@this._interactTime);
#endif
                        var dialogs = @this.Dialogs;
                        if (dialogs.Count > 0)
                        {
                            Astral.Classes.Timeout timer = new Astral.Classes.Timeout(5000);
                            while (player.InteractInfo.ContactDialog.Options.Count == 0)
                            {
                                if (timer.IsTimedOut)
                                {
                                    return;
                                }
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(500);
                            foreach (string key in dialogs)
                            {
                                player.InteractInfo.ContactDialog.SelectOptionByKey(key, "");
                                Thread.Sleep(1000);
                            }
                        }
                        player.InteractInfo.ContactDialog.Close();
                        return; 
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Interaction failed");
                    if (ignoreCombat && targetEntity.Location.Distance3DFromPlayer <= @this.CombatDistance)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Engage combat");

                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);

                        if (@this.AbortCombatDistance > @this.InteractDistance)
                        {
                            // устанавливаем прерывание боя
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Set abort combat condition and engage combat at the distance {entity.CombatDistance3:N2} from {entityDebugId}");
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Engage combat at the distance {entity.CombatDistance3:N2} from {entityDebugId}");

                        return;
                    }
                    if (_combat)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Player in combat...");

                        return;
                    }
                    if (_moved && @this.SkipMoving)
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {entityDebugId} moved, skip...");
                        else ETLogger.WriteLine("Entity moved, skip...", true);
                    }
                }
            }
            finally
            {
                // В случае неудачного интеракта из-за боя InteractOnce не должно помещать в черный список до повторной попытки
                if (!_combat && @this.InteractOnce || @this.SkipMoving && _moved)
                {
                    PushToBlackList(targetEntity);
                    targetEntity = null;
                }
                if (@this.ResetCurrentHotSpot)
                    @this.CurrentHotSpotIndex = -1;
            }
        }

        /// <summary>
        /// Делегат, сравнивающий <see cref="MoveToEntity.AbortCombatDistance"/> с расстоянием между игроком и <see cref="closestEntity"/> или <see cref="targetEntity"/>,
        /// и прерывающий бой, при удалении персонажа от <paramref name="combatTarget"/>
        /// </summary>
        /// <returns>true, если нужно прервать бой</returns>
        private bool CombatShouldBeAborted(ref Entity combatTarget)
        {
            var combatTargetContainerId = combatTarget?.ContainerId ?? uint.MaxValue;

            if (ExtendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(CombatShouldBeAborted));

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player health (", healthPercent, "%) below IgnoreCombatMinHP (", ignoreCombatMinHP, "%). Continue...")); 
                    return false;
                }

                var entityNameType = @this.EntityNameType;
                var abortCombatDistance = @this.AbortCombatDistance;
                var healthCheck = @this.HealthCheck;

                if (EntityKey.Validate(closestEntity))
                {
                    // Если атакуемая цель combatTarget является closestEntity => продолжаем бой
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    string entityStr = closestEntity.GetDebugString(entityNameType, "ClosestEntity", EntityDetail.Pointer);
                    if (!@this.HealthCheck || !closestEntity.IsDead)
                    {
                        // closestEntity является живым или проверка healthCheck не требуется
                        double dist = closestEntity.Location.Distance3DFromPlayer;
                        if (dist <= abortCombatDistance)
                        {
                            // расстояние до closestEntity меньше дистанции прерывания боя => продолжаем бой 
                            ETLogger.WriteLine(LogType.Debug,
                                string.Concat(currentMethodName, ": Player withing ", nameof(@this.AbortCombatDistance),
                                    " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                            return false;
                        }
                        //else
                        //{
                        //    // расстояние до closestEntity больше дистанции прерывания боя => можно прервать бой, но лучше проверим targetEntity  
                        //    ETLogger.WriteLine(LogType.Debug,
                        //        string.Concat(currentMethodName, ": Player outside ", nameof(@this.AbortCombatDistance),
                        //            " (", dist.ToString("N2"), " to ", entityStr, "). Combat have to be aborted"));
                        //    return true;
                        //}
                    }

                    // ClosestEntity мертво или далеко => можно прервать бой, но лучше проверим targetEntity
                    //ETLogger.WriteLine(LogType.Debug,
                    //    string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));
                    //return true;
                }

                if (EntityKey.Validate(targetEntity))
                {
                    // Если атакуемая цель combatTarget является targetEntity => продолжаем бой
                    if (targetEntity.ContainerId == combatTargetContainerId)
                        return false;

                    string entityStr = targetEntity.GetDebugString(entityNameType, "Entity", EntityDetail.Pointer);
                    if (healthCheck && targetEntity.IsDead)
                    {
                        // targetEntity мертво => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));
                        return true;
                    }

                    double dist = targetEntity.Location.Distance3DFromPlayer;
                    if (dist >= abortCombatDistance)
                    {
                        // расстояние до targetEntity больше дистанции прерывания боя => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": Player outside ", nameof(@this.AbortCombatDistance),
                                " (", dist.ToString("N2"), " to ", entityStr, "). Combat have to be aborted"));
                        return true;
                    }

                    // targetEntity - живое и расстояние до него меньше дистанции прерывания боя => ghjljk;ftv ,jq
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player withing ", nameof(@this.AbortCombatDistance),
                            " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                    return false;
                }

                // targetEntity и closestEntity невалидны => прерываем бой
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity[INVALID]. Combat have to be aborted"));
                return true;
            }
            else
            {
                // проверка необходимости перерывания боя без вывода отладочной информации
                if (EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent <
                      AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP) return false;

                var abortCombatDistance = @this.AbortCombatDistance;
                var healthCheck = @this.HealthCheck;

                if (EntityKey.Validate(closestEntity))
                {
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (healthCheck && closestEntity.IsDead) return true;

                    if (closestEntity.Location.Distance3DFromPlayer <= abortCombatDistance)
                        return false;
                }

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (healthCheck && targetEntity.IsDead) return true;

                    return targetEntity.Location.Distance3DFromPlayer >= abortCombatDistance;
                }
                return true;
            }
        }
        private bool ShouldRemoveAbortCombatCondition()
        {
            return @this.Completed;
        }
        #endregion
        #endregion
#endif

        public string ActionLabel
        {
            get
            {
                if (string.IsNullOrEmpty(_label))
                    _label = $"{@this.GetType().Name} [{@this.EntityID}]";
                return _label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                if (@this.EntityNameType != EntityNameType.Empty
                    && string.IsNullOrEmpty(@this.EntityID))
                    return false;

                var player = EntityManager.LocalPlayer;

                var map = @this.CurrentMap;
                if (!string.IsNullOrEmpty(map) &&
                    !map.Equals(player.MapState.MapName, StringComparison.Ordinal))
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(InternalConditions)}: Player is out of map '{map}' .");
                    }
                    return false;
                }

                if (@this.CustomRegionsPlayerCheck && @this.CustomRegionNames.Outside(player.Location))
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(InternalConditions)}: {nameof(@this.CustomRegionsPlayerCheck)} failed. Player is outside CustomRegions.");
                    }
                    return false;
                }

                if (entityAbsenceTimer != null && entityAbsenceTimer.IsTimedOut)
                {
                    if (ExtendedDebugInfo)
                    {
                        ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(InternalConditions)}: {nameof(@this.EntitySearchTime)} is out.");
                    }
                    return false;
                }

                return true;
            }
        }
        
        public ActionValidity InternalValidity
        {
            get
            {
                if (string.IsNullOrEmpty(@this.EntityID) && @this.EntityNameType != EntityNameType.Empty)
                    //return new ActionValidity($"The entity identifier {nameof(@this.EntityID)} is not valid.");
                    return new ActionValidity($"The Entity identifier is not valid.\n" +
                                              $"Check options '{nameof(@this.EntityID)}' and '{nameof(@this.EntityNameType)}'.");
                return new ActionValidity();
            }

        }

        public bool UseHotSpots => true;
        public Vector3 InternalDestination
        {
            get
            {
                var map = @this.CurrentMap;
                var player = EntityManager.LocalPlayer;
                if (!string.IsNullOrEmpty(map) &&
                    !map.Equals(player.MapState.MapName, StringComparison.Ordinal))
                    return Vector3.Empty;

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.Location.Distance3DFromPlayer > @this.InteractDistance)
                        return targetEntity.Location.Clone();
                    return player.Location.Clone();
                }
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            targetEntity = null;
            _key = null;
            _label = string.Empty;
        }

        public void GatherInfos()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid && !player.IsLoading)
            {
                if (@this.HotSpots.Count == 0)
                {
                    var pos = player.Location.Clone();
                    Node node;
                    if ((node = AstralAccessors.Quester.Core.Meshes.ClosestNode(pos.X, pos.Y, pos.Z, out double distance, false)) != null
                        && distance < 10)
                        @this.HotSpots.Add(node.Position);
                    else @this.HotSpots.Add(pos);
                }

                @this.CurrentMap = player.MapState.MapName;

                var entityId = @this.EntityID;
                if (string.IsNullOrEmpty(entityId))
                {
                    var entityIdType = @this.EntityIdType;
                    var entityNameType = @this.EntityNameType;
                    if (EntityViewer.GUIRequest(ref entityId, ref entityIdType, ref entityNameType) != null)
                    {
                        @this.EntityID = entityId;
                        @this.EntityIdType = entityIdType;
                        @this.EntityNameType = entityNameType;
                    }
                } 
            }

            _label = string.Empty;
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            if (EntityKey.Validate(targetEntity))
                graphics.drawFillEllipse(targetEntity.Location, new Size(10, 10), Brushes.Beige);
        }


        #region Вспомогательный инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugInteractEntities && logConf.Active;
            }
        }

        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType, @this.EntityNameType, @this.EntitySetType);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в черном списке <see cref="blackList"/>,
        /// а также в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialEntityCheck
        {
            get
            {
                if (_specialCheck is null)
                {
                    var customRegionNames = @this.CustomRegionNames;
                    if (customRegionNames.Count > 0)
                        _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this.HealthCheck,
                                                                @this.ReactionRange, @this.ReactionZRange,
                                                                @this.RegionCheck,
                                                                customRegionNames,
                                                                false,
                                                                CheckBlacklist);
                    else _specialCheck = CheckBlacklist;
                }
                return _specialCheck;
            }
        }
        private Predicate<Entity> _specialCheck;

        private bool CheckBlacklistAndCustomRegions(Entity e)
        {
            return !blackList.Contains(e.ContainerId) && @this.CustomRegionNames.Within(e);
        }
        private bool CheckBlacklist(Entity e)
        {
            return !blackList.Contains(e.ContainerId);
        }

        private void PushToBlackList(Entity ent)
        {
            if (targetEntity != null && targetEntity.IsValid)
            {
                blackList.Add(targetEntity.ContainerId, @this.InteractingTimeout);
#if DEBUG && DEBUG_INTERACTENTITIES
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::PushToBlackList: Entity[{targetEntity.ContainerId:X8}]");
#endif
            }
        }

        /// <summary>
        /// Метод, используемый для прерывания взаимодействия с <see cref="targetEntity"/>
        /// </summary>
        /// <returns></returns>
        private Approach.BreakInfos BreakInteraction()
        {
            if (Attackers.InCombat)
            {
                _combat = true;
                return Approach.BreakInfos.ApproachFail;
            }
            if (@this.SkipMoving && targetEntity.Location.Distance3D(_initialPos) > 3.0)
            {
                _moved = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }
        private Vector3 _initialPos = Vector3.Empty;
        private bool _combat;
        private bool _moved;
        
        /// <summary>
        /// Обновление таймера остановки поиска 
        /// </summary>
        private void ResetEntitySearchTimer()
        {
            var entitySearchTime = @this.EntitySearchTime;
            if (entitySearchTime > 0)
            {
                if (entityAbsenceTimer is null)
                    entityAbsenceTimer = new Astral.Classes.Timeout(entitySearchTime);
                else entityAbsenceTimer.ChangeTime(entitySearchTime);
            }
        }

        /// <summary>
        /// Проверка условия отключения боя <see cref="MoveToEntity.IgnoreCombatCondition"/>
        /// </summary>
        /// <returns>Результат проверки <see cref="MoveToEntity.IgnoreCombatCondition"/> либо True, если оно не задано.</returns>
        private bool CheckingIgnoreCombatCondition()
        {
            var check = @this.IgnoreCombatCondition;
            if (check != null)
                return check.IsValid;
            return true;
        }
        #endregion
    }
}
