using AcTp0Tools;
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
using static Astral.Quester.Classes.Action;

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
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
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
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be cast to '" + nameof(MoveToEntity) + '\'');
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
            _idStr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

            _key = null;
            _label = string.Empty;
            _specialCheck = null;

            @this.Bind(this);

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
                if (propertyName == nameof(@this.EntityID)
                    || propertyName == nameof(@this.EntityIdType)
                    || propertyName == nameof(@this.EntityNameType))
                {
                    _key = null;
                    _label = string.Empty;
                }
                else _specialCheck = null;

                targetEntity = null;
                closestEntity = null;
            }
        }

        public bool NeedToRun
        {
            get
            {
                bool extendedDebugInfo = ExtendedDebugInfo;
                string currentMethodName = extendedDebugInfo ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                // Команда работает с 2 - мя целями:
                //   1-я цель (targetEntity) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(targetEntity);

                if (extendedDebugInfo)
                {
                    string debugMsg = targetEntity is null ? string.Concat(currentMethodName, ": Target[NULL] processing result: '", entityPreprocessingResult, '\'') 
                        : string.Concat(currentMethodName, ": ", targetEntity.GetDebugString(@this._entityNameType, "Target", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\'');
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                {
                    // targetEntity не валидный - сбрасываем
                    targetEntity = null;
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded && timeout.IsTimedOut)
                {
                    // targetEntity не был обработан
                    Entity entity = SearchCached.FindClosestEntity(EntityKey, SpecialCheck);

                    if (entity != null)
                    {
                        closestEntity = entity;
                        bool closestIsTarget = targetEntity != null && targetEntity.ContainerId == closestEntity.ContainerId;

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Found ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer),
                                closestIsTarget ? " that equals to Target": string.Empty));

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в targetEntity
                            if (extendedDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Change Target[INVALID] to the ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer)));
                            }
                            targetEntity = closestEntity;
                        }
                        else if (!closestIsTarget)
                        {
                            // targetEntity не является ближайшей сущностью
                            if (!@this._holdTargetEntity)
                            {
                                // Фиксация на targetEntity не требуется
                                // ближайшую цель можно сохранить в targetEntity
                                if (extendedDebugInfo)
                                {
                                    string debugMsg = targetEntity is null
                                        ? string.Concat(currentMethodName, ": Change Target[NULL] to the ",
                                            closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity",
                                                EntityDetail.Pointer))
                                        : string.Concat(currentMethodName, ": Change ",
                                            targetEntity.GetDebugString(@this._entityNameType, "Target",
                                                EntityDetail.Pointer),
                                            " to the ",
                                            closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity",
                                                EntityDetail.Pointer));
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                targetEntity = closestEntity;
                            }

                            // обрабатываем ближайшую сущность closestEntity
                            entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                            if (extendedDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity[NULL] processing result: '", entityPreprocessingResult, '\''));
                                else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\''));
                            }
                        }
                    }
                    else if(extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity not found"));

                    timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                bool needToRun = entityPreprocessingResult == EntityPreprocessingResult.Succeeded;

                if (!needToRun)
                {
                    if (@this._ignoreCombat)
                    {
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true);
                        if (@this._abortCombatDistance > @this._distance)
                        {
                            AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat and set abort combat condition"));
                        }
                        else if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat"));
                    }
                }

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Result '", needToRun, '\''));

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

            var entityKey = EntityKey;

            if (entityKey.Validate(closestEntity))
                Attack_Entity(closestEntity);
            else if (entityKey.Validate(targetEntity))
                Attack_Entity(targetEntity);

            ActionResult actionResult = @this._stopOnApproached 
                ? ActionResult.Completed 
                : ActionResult.Running;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ActionResult=", actionResult));

            if (@this._resetCurrentHotSpot)
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
            string currentMethodName = extendedDebugInfo ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            bool validationResult = EntityKey.Validate(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthResult = !@this._healthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= @this._distance;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer), 
                        " verification=", healthResult && distanceResult, " {Valid; ", (@this._healthCheck) ? (healthResult ? "Alive; " : "Dead; ") : "Skip; ",
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
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity Verification=False {Invalid}"));

            return EntityPreprocessingResult.Failed;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Entity(Entity entity)
        {
            bool extendedDebugInfo = ExtendedDebugInfo;
            string currentMethodName = extendedDebugInfo ? string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            if (entity is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity is NULL. Break"));
                return;
            }

            if (@this._attackTargetEntity)
            {
                // entity нужно атаковать
                if (entity.RelationToPlayer == EntityRelation.Foe
                    && entity.IsLineOfSight()
                    && !entity.DoNotDraw
                    && !entity.IsUntargetable)
                {
                    Astral.Logic.NW.Attackers.List.Clear();

                    // entity враждебно и должно быть атаковано
                    string entityStr = string.Empty;
                    if (extendedDebugInfo)
                        entityStr = entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    Astral.Quester.API.IgnoreCombat = false;

                    if (@this._abortCombatDistance > @this._distance)
                    {
                        // устанавливаем прерывание боя
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Set abort combat condition, engage combat and attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2")));
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat and attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2")));

                    // атакуем entity
                    Astral.Logic.NW.Combats.CombatUnit(entity);
                    return;
                }
            }
            if (@this._ignoreCombat)
            {
                // entity в пределах досягаемости, но не может быть атакована (entity.RelationToPlayer != EntityRelation.Foe) 
                // или не должна быть атакована принудительно (!@this._attackTargetEntity)
                if (@this._abortCombatDistance > @this._distance)
                {
                    AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(CombatShouldBeAborted, ShouldRemoveAbortCombatCondition);
                    if (extendedDebugInfo) 
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": SetAbortCombatCondition"));
                }
                else if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat"));

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
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name);

                // Бой не должен прерываться, если  HP < IgnoreCombatMinHP
                var healthPercent = EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent;
                var ignoreCombatMinHP = AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP;
                if (healthPercent < ignoreCombatMinHP)
                {
                    ETLogger.WriteLine(LogType.Debug,
                        string.Concat(currentMethodName, ": Player health (", healthPercent, "%) below IgnoreCombatMinHP (", ignoreCombatMinHP, "%). Continue...")); 
                    return false;
                }

                if (EntityKey.Validate(closestEntity))
                {
                    // Если атакуемая цель combatTarget является closestEntity => продолжаем бой
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    string entityStr = closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer);
                    if (!@this._healthCheck || !closestEntity.IsDead)
                    {
                        // closestEntity является живым или проверка healthCheck не требуется
                        double dist = closestEntity.Location.Distance3DFromPlayer;
                        if (dist <= @this._abortCombatDistance)
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

                    string entityStr = targetEntity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer);
                    if (@this._healthCheck && targetEntity.IsDead)
                    {
                        // targetEntity мертво => прерываем бой
                        ETLogger.WriteLine(LogType.Debug,
                            string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));
                        return true;
                    }

                    double dist = targetEntity.Location.Distance3DFromPlayer;
                    if (dist >= @this._abortCombatDistance)
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

                if (EntityKey.Validate(closestEntity))
                {
                    if (closestEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (@this._healthCheck && closestEntity.IsDead) return true;

                    if (closestEntity.Location.Distance3DFromPlayer <= @this._abortCombatDistance)
                        return false;
                }

                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.ContainerId == combatTargetContainerId)
                        return false;

                    if (@this._healthCheck && targetEntity.IsDead) return true;

                    return targetEntity.Location.Distance3DFromPlayer >= @this._abortCombatDistance;
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
                    _label = $"{@this.GetType().Name} [{@this._entityId}]";
                return _label;
            }
        }

        public bool InternalConditions
        {
            get
            {
                return @this._entityNameType == EntityNameType.Empty || !string.IsNullOrEmpty(@this._entityId);
            }
        }

        public ActionValidity InternalValidity
        {
            get
            {
                if (!InternalConditions)
                    return new ActionValidity($"The combination of the main Entity identifier attribute is not valid.\n\r" +
                        $"Check {nameof(@this.EntityID)} and {nameof(@this.EntityNameType)} properties.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => true;

        public Vector3 InternalDestination
        {
            get
            {
                if (EntityKey.Validate(targetEntity))
                {
                    if (targetEntity.Location.Distance3DFromPlayer > @this._distance)
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

            AstralAccessors.Logic.NW.Combats.RemoveAbortCombatCondition();

            if (ExtendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(_idStr, '.', nameof(InternalReset)));
        }

        public void GatherInfos()
        {
            if (string.IsNullOrEmpty(@this._entityId))
                EntityViewer.GUIRequest(ref @this._entityId, ref @this._entityIdType, ref @this._entityNameType);

            if (@this.HotSpots.Count == 0)
                @this.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            _label = string.Empty;
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            var entityKey = EntityKey;
            if (entityKey.Validate(targetEntity))
            {
                if (graphics is MapperGraphics mapGraphics)
                {
                    float x = targetEntity.Location.X,
                        y = targetEntity.Location.Y,
                        diaD = @this._distance * 2,
                        diaACD = @this._abortCombatDistance * 2;

                    mapGraphics.FillRhombCentered(Brushes.Yellow, targetEntity.Location, 16, 16);
                    if (@this._distance > 11)
                    {
                        mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaD, true);
                        if (@this._abortCombatDistance > @this._distance)
                            mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaACD, true);
                    }
                    
                    if (entityKey.Validate(closestEntity) && targetEntity.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        mapGraphics.FillRhombCentered(Brushes.White, closestEntity.Location, 16, 16);

                        if (@this._distance > 5)
                        {
                            mapGraphics.DrawCircleCentered(Pens.White, x, y, diaD, true);
                            if (@this._abortCombatDistance > @this._distance)
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

                    int diaD = (int)(@this._distance * 2.0f * graphics.Zoom);
                    int diaACD = (int)(@this._abortCombatDistance * 2.0f * graphics.Zoom);
                    if (@this._distance > 5)
                    {
                        graphics.drawEllipse(targetEntity.Location, new Size(diaD, diaD), Pens.Yellow);

                        if (@this._abortCombatDistance > @this._distance)
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

                        if (@this._distance > 5)
                        {
                            graphics.drawEllipse(closestEntity.Location, new Size(diaD, diaD), Pens.Yellow);
                            if (@this._abortCombatDistance > @this._distance)
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
                    _key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType, @this._entityNameType);
                return _key;
            }
        } 
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// Использовать самомодифицирующийся предиката, т.к. предикат передается в <seealso cref="SearchCached.FindClosestEntity(EntityCacheRecordKey, Predicate{Entity})"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheck
        {
            get
            {
                if (_specialCheck is null)
                    _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                            @this._reactionRange, @this._reactionZRange,
                                                            @this._regionCheck,
                                                            @this._customRegionNames);
                return _specialCheck;
            }
        } 
        private Predicate<Entity> _specialCheck;
        #endregion
    }
}
