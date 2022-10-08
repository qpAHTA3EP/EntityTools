using ACTP0Tools;
using Astral.Logic.Classes.Map;
using EntityCore.Entities;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityCore.Forms;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using AStar;
using Astral.Classes;
using Astral.Logic.NW;
using EntityCore.Tools.Powers;
using static Astral.Quester.Classes.Action;
// ReSharper disable InconsistentNaming

namespace EntityCore.Quester.Action
{
    //TODO Добавить опцию удержания цели по аналогии с ChangeTarget
    public class MoveToEntityEngine : IQuesterActionEngine
    {
        /// <summary>
        /// ссылка на команду, для которой предоставляется функционал ядра
        /// </summary>
        private MoveToEntity @this;

        #region Данные 
        private string _idStr = string.Empty;
        private string _label = string.Empty;
        private Entity targetEntity;
        private Entity closestEntity;
        private readonly Timeout internalCacheTimer = new Timeout(0);
        private Timeout entityAbsenceTimer;
        private readonly PowerCache powerCache = new PowerCache(string.Empty);
        #endregion

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
            InternalRebase(m2e);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized");
        }
        ~MoveToEntityEngine()
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

            powerCache.Reset();
        }

        public bool Rebase(Astral.Quester.Classes.Action action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is MoveToEntity m2e)
            {
                if (InternalRebase(m2e))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }
            string debugStr =
                $"Rebase failed. {action.GetType().Name}[{action.ActionID}] can't be cast to '{nameof(MoveToEntity) }'";
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(MoveToEntity m2e)
        {
            targetEntity = null;
            closestEntity = null;

            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = m2e;
            _idStr = $"{@this.GetType().Name}[{@this.ActionID}]";

            _key = null;
            _label = string.Empty;
            _specialEntityCheck = null;

            @this.Bind(this);
            powerCache.PowerIdPattern = m2e.PowerId;
            internalCacheTimer.ChangeTime(0);
            entityAbsenceTimer = null;

            return true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender as Astral.Quester.Classes.Action, e.PropertyName);
        }
        public void OnPropertyChanged(Astral.Quester.Classes.Action sender, string propertyName)
        {
            if (ReferenceEquals(sender, @this))
            {
                switch (propertyName)
                {
                    case nameof(@this.EntityID):
                    case nameof(@this.EntityIdType):
                    case nameof(@this.EntityNameType):
                        _key = null;
                        _label = string.Empty;
                        break;
                    case nameof(@this.PowerId):
                        powerCache.PowerIdPattern = @this.PowerId;
                        break;
                    case nameof(@this.EntitySearchTime):
                        entityAbsenceTimer = null;
                        break;
                    default:
                        _specialEntityCheck = null;
                        break;
                }

                targetEntity = null;
                closestEntity = null;
            }
        }

        public bool NeedToRun
        {
            get
            {
                bool extendedDebugInfo = ExtendedDebugInfo;
                string currentMethodName = extendedDebugInfo 
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun)}"
                    : string.Empty;

                if (!InternalConditions)
                {
                    // Чтобы завершить команду нужно перейти к Run() и вернуть ActionResult.Fail
                    // Переход к Run() возможен только при возврате true
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(InternalConditions)} are False. Force calling of {nameof(Run)}");
                    return true;
                }

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
                    // targetEntity недействительный - сбрасываем
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
                        StartEntitySearchTimer();
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
                        if (@this.AbortCombatDistance > @this.Distance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug,
                                    $"{currentMethodName}: Disable combat and set abort combat condition");
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

            if (!InternalConditions)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: {nameof(InternalConditions)} failed. ActionResult={ActionResult.Fail}.");
                if (@this.IgnoreCombat)
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(false);
                return ActionResult.Fail;
            }

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Begins");

            var entityKey = EntityKey;

            if (entityKey.Validate(closestEntity))
                Attack_Entity(closestEntity);
            else if (entityKey.Validate(targetEntity))
                Attack_Entity(targetEntity);

            ActionResult actionResult = @this.StopOnApproached 
                ? ActionResult.Completed 
                : ActionResult.Running;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: ActionResult={actionResult}");

            // Возобновление таймера остановки поиска
            ResetEntitySearchTimer();

            if (@this.ResetCurrentHotSpot)
                @this.CurrentHotSpotIndex = -1;
            return actionResult;
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
                bool distanceResult = distance <= @this.Distance;
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
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo 
                ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Attack_Entity)}"
                : string.Empty;

            if (entity is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity is NULL. Break");
                return;
            }

            if (@this.AttackTargetEntity || powerCache.IsInitialized)
            {
                // entity нужно атаковать
                if (entity.IsLineOfSight()
                    && !entity.DoNotDraw
                    && !entity.IsUntargetable)
                {
                    Attackers.List.Clear();

                    // entity враждебно и должно быть атаковано
                    string entityStr = string.Empty;
                    if (extendedDebugInfo)
                        entityStr = entity.GetDebugString(@this.EntityNameType, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    Astral.Quester.API.IgnoreCombat = false;

                    if (@this.AbortCombatDistance > @this.Distance)
                    {
                        // устанавливаем прерывание боя
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Set abort combat condition, engage combat and attack {entityStr} at the distance {entity.CombatDistance3:N2}");
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Engage combat and attack {entityStr} at the distance {entity.CombatDistance3:N2}");

                    // Применяем умение на targetEntity
                    if (powerCache.IsInitialized)
                    {
                        var pow = powerCache.Power;
                        if (pow != null)
                        {
                            if (extendedDebugInfo)
                            {
                                var powDef = pow.PowerDef;
                                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Activating Power '{powDef.DisplayName}'[{powDef.InternalName}] on {entityStr}");
                            }

                            pow.ExecutePower(entity, @this.CastingTime, (int)@this.Distance, false, extendedDebugInfo);
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug,
                                $"{currentMethodName}: Fail to get Power '{@this.PowerId}' by '"+ nameof(@this.PowerId)+"'");
                    }
                    // атакуем враждебное targetEntity
                    if(entity.RelationToPlayer == EntityRelation.Foe)
                        Astral.Logic.NW.Combats.CombatUnit(entity);
                    
                    return;
                }
            }
            if (@this.IgnoreCombat && CheckingIgnoreCombatCondition())
            {
                // entity в пределах досягаемости, но не может быть атакована (entity.RelationToPlayer != EntityRelation.Foe) 
                // или не должна быть атакована принудительно (!@this.AttackTargetEntity)
                if (@this.AbortCombatDistance > @this.Distance)
                {
                    AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                    if (extendedDebugInfo) 
                        ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: SetAbortCombatCondition");
                }
                else if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Engage combat");

                Astral.Quester.API.IgnoreCombat = false;
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
                string currentMethodName =
                    $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(CombatShouldBeAborted)}";

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Player health ({healthPercent}%) below IgnoreCombatMinHP ({ignoreCombatMinHP}%). Continue..."); 
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
                                $"{currentMethodName}: Player withing {nameof(@this.AbortCombatDistance)} ({dist:N2} to {entityStr}). Continue...");
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
                            $"{currentMethodName}: {entityStr} is dead. Combat have to be aborted");
                        return true;
                    }

                    double dist = targetEntity.Location.Distance3DFromPlayer;
                    if (dist >= abortCombatDistance)
                    {
                        // расстояние до targetEntity больше дистанции прерывания боя => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            $"{currentMethodName}: Player outside {nameof(@this.AbortCombatDistance)} ({dist:N2} to {entityStr}). Combat have to be aborted");
                        return true;
                    }

                    // targetEntity - живое и расстояние до него меньше дистанции прерывания боя => ghjljk;ftv ,jq
                    ETLogger.WriteLine(LogType.Debug,
                        $"{currentMethodName}: Player withing {nameof(@this.AbortCombatDistance)} ({dist:N2} to {entityStr}). Continue...");
                    return false;
                }

                // targetEntity и closestEntity невалидны => прерываем бой
                ETLogger.WriteLine(LogType.Debug, $"{currentMethodName}: Entity[INVALID]. Combat have to be aborted");
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
                if (!InternalConditions)
                    return Vector3.Empty;

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.Location.Distance3DFromPlayer > @this.Distance)
                        return targetEntity.Location.Clone();
                    return EntityManager.LocalPlayer.Location.Clone();
                }
                return Vector3.Empty;
            }
        }

        public void InternalReset()
        {
            targetEntity = null;
            closestEntity = null;
            ResetEntitySearchTimer();
            AstralAccessors.Logic.NW.Combats.RemoveAbortCombatCondition();

            if (ExtendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}.{nameof(InternalReset)}");
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
            var entityKey = EntityKey;
            if (entityKey.Validate(targetEntity))
            {
                var distance = @this.Distance;
                var abortCombatDistance = @this.AbortCombatDistance;

                if (graphics is MapperGraphics mapGraphics)
                {
                    float x = targetEntity.Location.X,
                        y = targetEntity.Location.Y,
                        diaD = distance * 2,
                        diaACD = abortCombatDistance * 2;

                    mapGraphics.FillRhombCentered(Brushes.Yellow, targetEntity.Location, 16, 16);
                    if (distance > 11)
                    {
                        mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaD, true);
                        if (abortCombatDistance > distance)
                            mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaACD, true);
                    }
                    
                    if (entityKey.Validate(closestEntity) && targetEntity.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        mapGraphics.FillRhombCentered(Brushes.White, closestEntity.Location, 16, 16);

                        if (distance > 5)
                        {
                            mapGraphics.DrawCircleCentered(Pens.White, x, y, diaD, true);
                            if (abortCombatDistance > distance)
                                mapGraphics.DrawCircleCentered(Pens.White, x, y, diaACD, true);
                        }
                    }
                }
                else
                {
                    float x = targetEntity.Location.X,
                                  y = targetEntity.Location.Y;
                    List<Vector3> coords = new List<Vector3>() {
                        new Vector3(x, y - 5, 0),
                        new Vector3(x - 4.33f, y + 2.5f, 0),
                        new Vector3(x + 4.33f, y + 2.5f, 0)
                    };
                    graphics.drawFillPolygon(coords, Brushes.Yellow);

                    int diaD = (int)(distance * 2.0f * graphics.Zoom);
                    int diaACD = (int)(abortCombatDistance * 2.0f * graphics.Zoom);
                    if (distance > 5)
                    {
                        graphics.drawEllipse(targetEntity.Location, new Size(diaD, diaD), Pens.Yellow);

                        if (abortCombatDistance > distance)
                        {
                            graphics.drawEllipse(targetEntity.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    }

                    if (entityKey.Validate(closestEntity) && targetEntity.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        coords[0].X = x; coords[0].Y = y - 5;
                        coords[1].X = x - 4.33f; coords[1].Y = y + 2.5f;
                        coords[2].X = x + 4.33f; coords[2].Y = y + 2.5f;
                        graphics.drawFillPolygon(coords, Brushes.LightYellow);

                        if (distance > 5)
                        {
                            graphics.drawEllipse(closestEntity.Location, new Size(diaD, diaD), Pens.Yellow);
                            if (abortCombatDistance > distance)
                                graphics.drawEllipse(closestEntity.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    } 
                }
            }
        }

        #region Вспомогательные инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной ниформации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.QuesterActions.DebugMoveToEntity && logConf.Active;
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
                {
                    _key = new EntityCacheRecordKey(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
                }
                return _key;
            }
        } 
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// Использовать самомодифицирующийся предиката, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialEntityCheck
        {
            get
            {
                if (_specialEntityCheck is null)
                    _specialEntityCheck = SearchHelper.Construct_EntityAttributePredicate(@this.HealthCheck,
                                                            @this.ReactionRange, @this.ReactionZRange,
                                                            @this.RegionCheck,
                                                            @this.CustomRegionNames);
                return _specialEntityCheck;
            }
        } 
        private Predicate<Entity> _specialEntityCheck;

        /// <summary>
        /// Обновление таймера остановки поиска 
        /// </summary>
        private void ResetEntitySearchTimer()
        {
            var entitySearchTime = @this.EntitySearchTime;
            if (entitySearchTime > 0)
            {
                if (entityAbsenceTimer != null)
                    entityAbsenceTimer.ChangeTime(entitySearchTime);
                else entityAbsenceTimer = new Timeout(entitySearchTime);
            }
        }
        /// <summary>
        /// Запуск таймера остановки поиска.
        /// Если тайме запущен, то отсчет продолжается
        /// </summary>
        private void StartEntitySearchTimer()
        {
            var entitySearchTime = @this.EntitySearchTime;
            if (entitySearchTime > 0)
            {
                if (entityAbsenceTimer is null)
                    entityAbsenceTimer = new Timeout(entitySearchTime);
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
