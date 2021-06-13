//#define DEBUG_INTERACTENTITIES

using System;
using System.Threading;
using Astral.Classes;
using Astral.Logic.NW;
using EntityCore.Entities;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Internals;
using EntityTools.Core.Interfaces;
using Astral.Logic.Classes.Map;
using System.Drawing;
using static Astral.Quester.Classes.Action;
using EntityTools;
using EntityCore.Forms;
using EntityCore.Tools.Navigation;
using System.ComponentModel;
using EntityTools.Quester;

namespace EntityCore.Quester.Action
{
    public class InteractEntitiesEngine :  IQuesterActionEngine
#if IEntityDescriptor
        , IEntityInfos 
#endif
    {
        InteractEntities @this;

        #region Данные ядра
        private readonly TempBlackList<uint> blackList = new TempBlackList<uint>();
        private Entity target;
        private Entity closestEntity;
        private Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
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
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be casted to '" + nameof(InteractEntities) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(InteractEntities ie)
        {
            target = null;
            closestEntity = null;

            // Убираем привязку к старой команде
            @this?.Unbind();

            @this = ie;
            @this.PropertyChanged += OnPropertyChanged;

            _key = null;
            _label = string.Empty;
            _specialCheck = null;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');

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


            target = null;
            timeout.ChangeTime(0);
        }

#if true
        public bool NeedToRun
        {
            get
            {
                var entityKey = EntityKey;

                if (timeout.IsTimedOut)
                {
                    closestEntity = SearchCached.FindClosestEntity(entityKey,
                                                                   //@this._healthCheck,
                                                                   //@this._reactionRange, @this._reactionZRange,
                                                                   //@this._regionCheck,
                                                                   SpecialCheckSelector);
#if DEBUG_INTERACTENTITIES
                    if (closestEntity != null && closestEntity.IsValid)
                        ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}::{MethodBase.GetCurrentMethod().Name}: Found Entity[{closestEntity.ContainerId:X8}] (closest)");
                    
#endif
                    timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);
                }

                if (closestEntity != null && (!@this._holdTargetEntity || !entityKey.Validate(target) || (@this._healthCheck && target.IsDead)))
                    target = closestEntity;

                if (entityKey.Validate(target) && !(@this._healthCheck && target.IsDead))
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
                    _initialPos = target.Location.Clone();
                    return true;
                }
                if (@this._ignoreCombat && entityKey.Validate(closestEntity)
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
            _moved = false;
            _combat = false;
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
                    || Approach.EntityForInteraction(target, BreakInteraction))
#else
                    || Approach.EntityByDistance(target, @this._interactDistance, CheckCombat))  
#endif

                {
#if DEBUG && DEBUG_INTERACTENTITIES
                    ETLogger.WriteLine(LogType.Debug, $"InteractEntitiesEngine::Run: Interact Entity[{target.ContainerId:X8}]");
#endif
#if true
                    target.SmartInteract(interactDistance, @this._interactTime);
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
                if (!_combat && @this._interactOnce || @this._skipMoving && _moved)
                {
                    PushToBlackList(target);

                    target = new Entity(IntPtr.Zero);
                }
                if (@this._resetCurrentHotSpot)
                    @this.CurrentHotSpotIndex = -1;
            }

        }
#else // Новая реализация, аналогичная MoveToEntity

        public bool NeedToRun
        {
            get
            {
                bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugInteractEntities;
                string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                // Команда работает с 2 - мя целями:
                //   1-я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(target);

                if (extendedDebugInfo)
                {
                    string debugMsg;
                    if (target is null)
                        debugMsg = string.Concat(currentMethodName, ": Target[NULL] processing result: '", entityPreprocessingResult, '\'');
                    else debugMsg = string.Concat(currentMethodName, ": ", target.GetDebugString(@this._entityNameType, "Target", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\'');
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                {
                    // target не валидный - сбрасываем
                    target = null;
                    initialPos = Vector3.Empty;
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded)
                {
                    // target не был обработан
#if false
                    Entity entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                                           @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, null, @this._customRegionNames.Count > 0 ? withingCustomRegions : null); 
#else
                    Entity entity = SearchCached.FindClosestEntity(EntityKey,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck,
                                                                   SpecialCheckSelector);
#endif

                    if (entity != null)
                    {
                        closestEntity = entity;
                        bool closestIsTarget = target != null && target.ContainerId == closestEntity.ContainerId;

                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Found ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer),
                                closestIsTarget ? " that equals to Target" : string.Empty));

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в target
                            if (extendedDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Change Target[INVALID] to the ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer)));
                            }
                            target = closestEntity;
                        }
                        else if (!closestIsTarget)
                        {
                            // target не является ближайшей сущностью closestEntity
                            if (!@this._holdTargetEntity)
                            {
                                // Фиксация на target не требуется
                                // ближайшую цель можно сохранить в target
                                if (extendedDebugInfo)
                                {
                                    string debugMsg;
                                    if (target is null)
                                        debugMsg = string.Concat(currentMethodName, ": Change Target[NULL] to the ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer));
                                    else debugMsg = string.Concat(currentMethodName, ": Change ", target.GetDebugString(@this._entityNameType, "Target", EntityDetail.Pointer),
                                            " to the ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer));
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                target = closestEntity;
                            }

                            // обрабатываем ближайшую сущность closestEntity
                            entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                            if(entityPreprocessingResult == EntityPreprocessingResult.Succeeded)
                            {
                                // Обработка closestEntity успешна - будем производить попытку взаимодействия
                                initialPos = closestEntity.Location.Clone();
                            }
                            else initialPos = Vector3.Empty;

                            if (extendedDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity[NULL] processing result: '", entityPreprocessingResult, '\''));
                                else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\''));
                            }
                        }
                    }
                    else if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity not found"));
                }
                else
                {
                    // Обработка target успешна - будем производить попытку взаимодействия
                    initialPos = target.Location.Clone();
                }

                bool needToRun = entityPreprocessingResult == EntityPreprocessingResult.Succeeded;

                if (@this._ignoreCombat && !needToRun)
                {
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true);
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat"));
                }

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Result '", needToRun, '\''));

                return needToRun;
            }
        }

        public ActionResult Run()
        {
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            var entityKey = EntityKey;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

            if (entityKey.Validate(closestEntity))
                Interact_Entity(closestEntity);
            else if (entityKey.Validate(target))
                Interact_Entity(target);

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ActionResult=", actionResult));

            if (@this._resetCurrentHotSpot)
                @this.CurrentHotSpotIndex = -1;
            return actionResult;
        }
        #region Декомпозиция основного функционала
        /// <summary>
        /// Анализ <paramref name="entity"/> на предмет возможности взаимодействия
        /// </summary>
        private EntityPreprocessingResult Preprocessing_Entity(Entity entity)
        {
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugInteractEntities;
            string currentMethodName = extendedDebugInfo ? nameof(Preprocessing_Entity) : string.Empty;

            EntityPreprocessingResult result = EntityPreprocessingResult.Failed;

            var entityKey = EntityKey;

            bool validationResult = entityKey.Validate(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthResult = !@this._healthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= @this._combatDistance;

                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer),
                        " verification=", healthResult && distanceResult, " {Valid; ", (@this._healthCheck) ? (healthResult ? "Alive; " : "Dead; ") : "Skip; ",
                        distanceResult ? "Near (" : "Faraway (", distance.ToString("N2"), ")}"));

                if (healthResult)
                {
                    if (distanceResult)
                    {
                        // entity в пределах заданного расстояния
                        result = EntityPreprocessingResult.Succeeded;
                    }
                    else result = EntityPreprocessingResult.Faraway;
                }
                else result = EntityPreprocessingResult.Failed;
            }
            else
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity Verification=False {Invalid}"));
            }

            return result;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Взаимодействие с сущностью <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Interact_Entity(Entity entity)
        {
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugInteractEntities;
            string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

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
                    //&& !entity.IsUnselectable
                    && !entity.IsUntargetable)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    // entity враждебно и должно быть атаковано
                    string entityStr = string.Empty;
                    if (extendedDebugInfo)
                        entityStr = entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    //Astral.Logic.NW.Attackers.List.Add(entity);
                    //if (@this._ignoreCombat)
                    Astral.Quester.API.IgnoreCombat = false;

                    if (@this._abortCombatDistance > @this._distance)
                        AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(AbortCombatCondition, ShouldRemoveAbortCombatCondition);
                    else
                    {
                        // запускаем бой без прерывания
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat and attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2")));

                        Astral.Logic.NW.Combats.CombatUnit(entity, null);
                    }
                    return;
                }
            }
            if (@this._ignoreCombat)
            {
                // entity в пределах досягаемости, но не может быть атакована (entity.RelationToPlayer != EntityRelation.Foe) 
                // или не должна быть атакована принудительно (!@this._attackTargetEntity)
                if (@this._abortCombatDistance > @this._distance)
                    AstralAccessors.Logic.NW.Combats.SetAbortCombatCondition(AbortCombatCondition, ShouldRemoveAbortCombatCondition);
                else
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat"));

                Astral.Quester.API.IgnoreCombat = false;
            }
        }

        /// <summary>
        /// Делегат, сравнивающий <see cref="MoveToEntity.AbortCombatDistance"/> с расстоянием между игроком и <see cref="closestEntity"/> или <see cref="target"/>,
        /// и прерывающий бой, при удалении персонажа от <paramref name="entity"/>
        /// </summary>
        private bool AbortCombatCondition(Entity combat_target)
        {
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugInteractEntities;
            string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            // Бой может быть прерван, если  HP > IgnoreCombatMinHP
            if (EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent > AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP)
            {
                var entityKey = EntityKey;
                var entity = closestEntity ?? target;
                bool anchorEntityValid = entityKey.Validate(entity);

                if (anchorEntityValid)
                {
                    if (entity.ContainerId == combat_target.ContainerId)
                        return false;

                    string entityStr = extendedDebugInfo ? entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer) : string.Empty;
                    if (!@this._healthCheck || !entity.IsDead)
                    {
                        double dist = entity.Location.Distance3DFromPlayer;
                        if (dist >= @this._abortCombatDistance)
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Player outside ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Combat have to be aborted"));
                            return true;
                        }
                        else
                        {
                            if (extendedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Player withing ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                        }
                    }
                    else
                    {
                        if (extendedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entityStr, " is dead. Combat have to be aborted"));

                        return true;
                    }
                }
                else
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity[INVALID]. Combat have to be aborted"));

                    return true;
                }
            }
            return false;
        }
        private bool ShouldRemoveAbortCombatCondition()
        {
            var entityKey = EntityKey;
            var entity = closestEntity ?? target;
            return @this.Completed || !entityKey.Validate(entity);
        }
        #endregion
        #endregion
#endif

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
                    return new ActionValidity($"{nameof(@this.EntityID)} property is not valid.");
                return new ActionValidity();
            }
        }

        public bool UseHotSpots => true;
        public Vector3 InternalDestination
        {
            get
            {
                if (EntityKey.Validate(target))
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
            _key = null;
            _label = string.Empty;
        }

        public void GatherInfos()
        {
            if (@this.HotSpots.Count == 0)
                @this.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            if (string.IsNullOrEmpty(@this._entityId))
                EntitySelectForm.GUIRequest(ref @this._entityId, ref @this._entityIdType, ref @this._entityNameType);

            _label = string.Empty;
        }

        public void OnMapDraw(GraphicsNW graphics)
        {
            if (EntityKey.Validate(target))
                graphics.drawFillEllipse(target.Location, new Size(10, 10), Brushes.Beige);
        }


        #region Вспомогательный инструменты
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey
        {
            get
            {
                if (_key is null)
                    _key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType, @this._entityNameType, @this._entitySetType);
                return _key;
            }
        }
        private EntityCacheRecordKey _key;

        /// <summary>
        /// Функтор дополнительной проверки <seealso cref="Entity"/> 
        /// на предмет нахождения в черном списке <see cref="blackList"/>,
        /// а также в пределах области, заданной <see cref="InteractEntities.CustomRegionNames"/>
        /// </summary>        
        private Predicate<Entity> SpecialCheckSelector
        {
            get
            {
                if (_specialCheck is null)
                {
                    if (@this._customRegionNames.Count > 0)
                        _specialCheck = SearchHelper.Construct_EntityAttributePredicate(@this._healthCheck,
                                                                @this._reactionRange, @this._reactionZRange,
                                                                @this._regionCheck,
                                                                @this._customRegionNames,
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
            return !blackList.Contains(e.ContainerId) && @this._customRegionNames.Within(e);
        }
        private bool CheckBlacklist(Entity e)
        {
            return !blackList.Contains(e.ContainerId);
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
        /// Метод, используемый для прерывания взаимодействия с <see cref="target"/>
        /// </summary>
        /// <returns></returns>
        private Approach.BreakInfos BreakInteraction()
        {
            if (Attackers.InCombat)
            {
                _combat = true;
                return Approach.BreakInfos.ApproachFail;
            }
            if (@this._skipMoving && target.Location.Distance3D(_initialPos) > 3.0)
            {
                _moved = true;
                return Approach.BreakInfos.ApproachFail;
            }
            return Approach.BreakInfos.Continue;
        }
        private Vector3 _initialPos = Vector3.Empty;
        private bool _combat;
        private bool _moved;
        #endregion

#if IEntityDescriptor
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

            var entityKey = EntityKey;
            var entityCheck = SpecialCheckSelector;
            // список всех Entity, удовлетворяющих условиям
            var entities = SearchCached.FindAllEntity(entityKey, entityCheck);

            // Количество Entity, удовлетворяющих условиям
            if (entities?.Count > 0)
                sb.Append("Found Entities which matched to ID '" + nameof(@this.EntityID) + '\'').AppendLine(entities.Count.ToString());
            else sb.Append("Found Entities which matched to ID '" + nameof(@this.EntityID) + "': 0");
            sb.AppendLine();

            target = SearchCached.FindClosestEntity(entityKey, entityCheck);

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
#endif
    }
}
