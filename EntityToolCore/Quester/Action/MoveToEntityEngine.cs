#define ActionID_Identifier

using Astral.Quester.Classes;
using EntityCore.Entities;
using EntityCore.Extensions;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using MyNW.Classes;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using EntityTools.Core.Interfaces;
using Astral.Logic.Classes.Map;
using MyNW.Internals;
using System.Drawing;
using static Astral.Quester.Classes.Action;
using EntityTools;
using EntityTools.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityCore.Forms;
using EntityTools.Patches.Mapper;
using System.Reflection;
using EntityCore.Enums;

namespace EntityCore.Quester.Action
{
    public class MoveToEntityEngine : IEntityInfos, IQuesterActionEngine
    {
        /// <summary>
        /// ссылка на команду, для которой предоставляется функционал ядра
        /// </summary>
        private MoveToEntity @this;
#region Данные 
        private Predicate<Entity> checkEntity = null;
        private Func<List<CustomRegion>> getCustomRegions = null;

#if M2EE_ActionDebug
        // Запись отладочной информации в поле MoveToEntity.Debug
        // приводит к критической ошибке и аварийному завершению Astral'a
        //private readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null; 
        internal readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null; 
#endif

        private string actionIDstr = string.Empty;
        private List<CustomRegion> customRegions = null;
        private string label = string.Empty;
        private Entity target = null;
        private Entity closestEntity = null;
#if timeout
        private Timeout timeout = new Timeout(0); 
#endif
        #endregion

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
#if false
            @this = m2e;
            @this.PropertyChanged += PropertyChanged;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');
#if M2EE_ActionDebug
            debug = @this.GetInstanceProperty<MoveToEntity, ActionDebug>("Debug");
            debug.Value.AddInfo($"{actionIDstr} initialized"); 
#endif

            checkEntity = functor_CheckEntity_Initializer;
            getCustomRegions = functor_GetCustomRegion_Initializer;

#if timeout
            timeout.ChangeTime(0); 
#endif
            @this.ActionEngine = this;
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized"); 
#else
            InternalRebase(m2e);
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized");
#endif
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
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} rebase failed");
                return false;
            }
#if false
            else ETLogger.WriteLine(LogType.Debug, $"Rebase failed. '{action}' has type '{action.GetType().Name}' which not equals to '{nameof(MoveToEntity)}'");
            return false; 
#else
            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.ActionID, "] can't be casted to '" + nameof(MoveToEntity) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
#endif

        }

        private bool InternalRebase(MoveToEntity m2e)
        {
            // Убираем привязку к старой команде
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = new EntityTools.Core.Proxies.QuesterActionProxy(@this);
            }

            @this = m2e;
            @this.PropertyChanged += PropertyChanged;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']');
#if M2EE_ActionDebug
            debug = @this.GetInstanceProperty<MoveToEntity, ActionDebug>("Debug");
            debug.Value.AddInfo($"{actionIDstr} initialized");
#endif

            checkEntity = initializer_CheckEntity;
            getCustomRegions = initializer_GetCustomRegion;

#if timeout
            timeout.ChangeTime(0);
#endif
            @this.Engine = this;

            return true;
        }

        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(ReferenceEquals(sender, @this))
            {
                switch(e.PropertyName)
                {
                    case nameof(@this.EntityID):
                        checkEntity = initializer_CheckEntity;
                        label = string.Empty;
                        break;
                    case nameof(@this.EntityIdType):
                        checkEntity = initializer_CheckEntity;
                        break;
                    case nameof(@this.EntityNameType):
                        checkEntity = initializer_CheckEntity;
                        break;
                    case nameof(@this.CustomRegionNames):
                        getCustomRegions = initializer_GetCustomRegion;
                        break;
                }

                target = null;
                closestEntity = null;
#if timeout
                timeout.ChangeTime(0); 
#endif
            }
        }

        public bool NeedToRun
        {
            get
            {
                bool extedndedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
                string currentMethodName = extedndedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

                if (extedndedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

                // Команда работает с 2 - мя целями:
                //   1-я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(target);

                if (extedndedDebugInfo)
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
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded)
                {
                    // target не был обработан
                    // перерыв между поисками ближайшей сущности истек
                    Entity entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                    if (entity != null)
                    {
                        closestEntity = entity;
                        bool closestIsTarget = target != null && target.ContainerId == closestEntity.ContainerId;

                        if (extedndedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Found ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer),
                                closestIsTarget ? " that equals to Target": string.Empty));

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в target
                            if (extedndedDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Change Target[INVALID] to the ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer)));
                            }
                            target = closestEntity;
                        }
                        else if (!closestIsTarget)
                        {
                            // target не является ближайшей сущностью
                            if (!@this._holdTargetEntity)
                            {
                                // Фиксация на target не требуется
                                // ближайшую цель можно сохранить в target
                                if (extedndedDebugInfo)
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
                            if (extedndedDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity[NULL] processing result: '", entityPreprocessingResult, '\''));
                                else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", closestEntity.GetDebugString(@this._entityNameType, "ClosestEntity", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\''));
                            }
                        }
                    }
                    else if(extedndedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ClosestEntity not found"));
                }

                bool needToRun = entityPreprocessingResult == EntityPreprocessingResult.Succeeded;

                if (@this._ignoreCombat && !needToRun)
                {
#if false
                    Astral.Quester.API.IgnoreCombat = true; 
#else
                    AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true);
#endif
                    if (extedndedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Disable combat"));
                }

                if (extedndedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Result '", needToRun, '\''));

                return needToRun;
            }
        }

        #region Декомпозиция основного функционала
        private enum EntityPreprocessingResult
        {
            /// <summary>
            /// Некорректная сущность, обработка провалена
            /// </summary>
            Failed,
            /// <summary>
            /// Сущность в пределах заданной дистации и "обработана" в соответствии с настройками
            /// </summary>
            Succeeded,
            /// <summary>
            /// Суoнщсть не достигнута (находится за пределами заданой дистанции)
            /// </summary>
            Faraway
        }

        /// <summary>
        /// Анализ <paramref name="entity"/> на предмет возможности вступления в бой
        /// </summary>
        private EntityPreprocessingResult Preprocessing_Entity(Entity entity)
        {
            bool extedndedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extedndedDebugInfo ? nameof(Preprocessing_Entity) : string.Empty;

            EntityPreprocessingResult result = EntityPreprocessingResult.Failed;

            bool validationResult = ValidateEntity(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthResult = !@this._healthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= @this._distance;

                if (extedndedDebugInfo)
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
                if (extedndedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity Verification=False {Invalid}"));
            }

            return result;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Entity(Entity entity)
        {
            bool extedndedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extedndedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;

            if(entity is null)
            {
                if (extedndedDebugInfo)
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
                    if (extedndedDebugInfo)
                        entityStr = entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    //Astral.Logic.NW.Attackers.List.Add(entity);
                    //if (@this._ignoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    
                    if (@this._abortCombatDistance > @this._distance)
                    {
                        // запускаем бой с прерыванием за пределами AbortCombatDistance
                        if (abortCombatTask is null || abortCombatTask.Status != TaskStatus.Running)
                        {
                            abortCombatTask = new Task(() => MonitorAbortCombatDistance(entity));

                            if (extedndedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"), " and initialize AbortCombatTask#", abortCombatTask.Id));

                            abortCombatTask.Start();
                        }
                        else if (extedndedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"), " and continue AbortCombatTask#", abortCombatTask.Id));
                        
                        Astral.Logic.NW.Combats.CombatUnit(entity, null);
                    }
                    else
                    {
                        // запускаем бой без прерывания
                        if (extedndedDebugInfo)
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
                {
                    if (abortCombatTask is null || abortCombatTask.Status != TaskStatus.Running)
                    {
                        abortCombatTask = new Task(() => MonitorAbortCombatDistance(entity));

                        if (extedndedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat, and initialize AbortCombatTask#", abortCombatTask.Id));

                        abortCombatTask.Start();
                    }
                    else
                    {
                        if (extedndedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat and continue AbortCombatTask#", abortCombatTask.Id));
                    } 
                }
                else if (extedndedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Engage combat"));

                Astral.Quester.API.IgnoreCombat = false;
            }
        }

        #region AbortCombatDistance_by_Task
        Task abortCombatTask;

        /// <summary>
        /// Делегат, сравнивающий <see cref="MoveToEntity.AbortCombatDistance"/> с расстоянием между игроком и <paramref name="entity"/>,
        /// и прерывающий бой, при удалении персонажа от <paramref name="entity"/>
        /// </summary>
        internal void MonitorAbortCombatDistance(Entity entity)
        {
            bool extedndedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extedndedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;
            // Реализовано прерывание боя через статический метод Astral.Logic.NW.Combats.AbordCombat(bool stopMove = true)
            // по аналогии с 
            //Astral.Logic.UCC.Actions.AbordCombat.Run()
            //{
            //    Combat.SetIgnoreCombat(true, this.IgnoreCombatMinHP, this.IgnoreCombatTime * 1000);
            //    Logger.WriteLine("IgnoreCombatMinHP set to " + this.IgnoreCombatMinHP);
            //    base.CurrentTimeout = new Timeout(base.CoolDown);
            //    Combats.AbordCombat(true);
            //    return true;
            //}

            string entityStr = entity.GetDebugString(@this._entityNameType, "Entity", EntityDetail.Pointer);
            Thread.Sleep(1000);
            while (EntityManager.LocalPlayer.InCombat || Astral.Logic.NW.Attackers.InCombat)
            {
                // Бой может быть прерван, если  HP > IgnoreCombatMinHP
                if (EntityManager.LocalPlayer.Character.AttribsBasic.HealthPercent > AstralAccessors.Quester.FSM.States.Combat.IgnoreCombatMinHP)
                {
                    bool anchorEntityValid = ValidateEntity(entity);
                    if (anchorEntityValid)
                    {
                        if (!@this._healthCheck || !entity.IsDead)
                        {
                            double dist = entity.Location.Distance3DFromPlayer;
                            if (dist >= @this._abortCombatDistance)
                            {
                                if (extedndedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Player outside ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Break combat"));
#if false
                                Astral.Quester.API.IgnoreCombat = true; 
#else
                                AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, -1, 500);
#endif
                                AstralAccessors.Logic.NW.Combats.AbordCombat(true);
                            }
                            else
                            {
                                if (extedndedDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Player withing ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                                Astral.Quester.API.IgnoreCombat = false;
                            }
                        }
                        else
                        {
                            if (extedndedDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", entityStr, " is dead. Break combat. Finish AbortCombatTask#", Task.CurrentId));
#if false
                                Astral.Quester.API.IgnoreCombat = true; 
#else
                            AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, -1, 500);
#endif
                            AstralAccessors.Logic.NW.Combats.AbordCombat(true);
                            return;
                        }
                    }
                    else
                    {
                        if (extedndedDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Entity[INVALID]. Break combat. Finish AbortCombatTask#", Task.CurrentId));
#if false
                                Astral.Quester.API.IgnoreCombat = true; 
#else
                        AstralAccessors.Quester.FSM.States.Combat.SetIgnoreCombat(true, -1, 500);
#endif
                        AstralAccessors.Logic.NW.Combats.AbordCombat(true);
                        return;
                    }
                }

                Thread.Sleep(500);
            }
            if (extedndedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Combat ended. Finish AbortCombatTask#", Task.CurrentId));
            Astral.Quester.API.IgnoreCombat = true;
        }
        #endregion
        #endregion
        #endregion

        public ActionResult Run()
        {
            bool extedndedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extedndedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name)  : string.Empty;

            if (extedndedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Begins"));

            if (ValidateEntity(closestEntity))
                Attack_Entity(closestEntity);
            else if (ValidateEntity(target))
                Attack_Entity(target); 

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (extedndedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ActionResult=", actionResult));

            if(@this._resetCurrentHotSpot)
                @this.CurrentHotSpotIndex = -1;
            return actionResult;
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
                if (ValidateEntity(target))
                {
                    if (target.Location.Distance3DFromPlayer > @this._distance)
                        return target.Location.Clone();
                    return EntityManager.LocalPlayer.Location.Clone();
                }
                return new Vector3();
            }
        }

        public void InternalReset()
        {
            target = null;
            closestEntity = null;
            checkEntity = initializer_CheckEntity;
            getCustomRegions = initializer_GetCustomRegion;
            label = string.Empty;
            if (EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(InternalReset)));
        }

        public void GatherInfos()
        {
            if (string.IsNullOrEmpty(@this._entityId))
                EntitySelectForm.GUIRequest(ref @this._entityId, ref @this._entityIdType, ref @this._entityNameType);

            // TODO: Нужно проверить корректность загрузки профилей с функцией автодобавления HotSpot'a
            if (@this.HotSpots.Count == 0)
                @this.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            label = string.Empty;
        }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (ValidateEntity(target))
            {
                if (graph is MapperGraphics mapGraphics)
                {
                    float x = target.Location.X,
                        y = target.Location.Y,
                        diaD = @this._distance * 2,
                        diaACD = @this._abortCombatDistance * 2;

                    //mapGraphics.FillUpsideTriangleCentered(Brushes.Yellow, target.Location, 10);
                    mapGraphics.FillRhombCentered(Brushes.Yellow, target.Location, 16, 16);
                    if (@this._distance > 11)
                    {
                        mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaD, true);
                        if (@this._abortCombatDistance > @this._distance)
                            mapGraphics.DrawCircleCentered(Pens.Yellow, x, y, diaACD, true);
                    }
                    
                    if (ValidateEntity(closestEntity) && target.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        mapGraphics.FillRhombCentered(Brushes.White, closestEntity.Location, 16, 16);

                        if (@this._distance > 11)
                        {
                            mapGraphics.DrawCircleCentered(Pens.White, x, y, diaD, true);
                            if (@this._abortCombatDistance > @this._distance)
                                mapGraphics.DrawCircleCentered(Pens.White, x, y, diaACD, true);
                        }
                    }
                }
                else
                {
                    float x = target.Location.X,
                                  y = target.Location.Y;
                    List<Vector3> coords = new List<Vector3>() {
                        new Vector3(x, y - 5, 0),
                        new Vector3(x - 4.33f, y + 2.5f, 0),
                        new Vector3(x + 4.33f, y + 2.5f, 0)
                    };
                    graph.drawFillPolygon(coords, Brushes.Yellow);

                    int diaD = (int)(@this._distance * 2.0f * graph.Zoom);
                    int diaACD = (int)(@this._abortCombatDistance * 2.0f * graph.Zoom);
                    if (@this._distance > 11)
                    {
                        graph.drawEllipse(target.Location, new Size(diaD, diaD), Pens.Yellow);

                        if (@this._abortCombatDistance > @this._distance)
                        {
                            graph.drawEllipse(target.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    }

                    if (ValidateEntity(closestEntity) && target.ContainerId != closestEntity.ContainerId)
                    {
                        x = closestEntity.Location.X;
                        y = closestEntity.Location.Y;

                        coords[0].X = x; coords[0].Y = y - 5;
                        coords[1].X = x - 4.33f; coords[1].Y = y + 2.5f;
                        coords[2].X = x + 4.33f; coords[2].Y = y + 2.5f;
                        graph.drawFillPolygon(coords, Brushes.LightYellow);

                        if (@this._distance > 11)
                        {
                            graph.drawEllipse(closestEntity.Location, new Size(diaD, diaD), Pens.Yellow);
                            if (@this._abortCombatDistance > @this._distance)
                                graph.drawEllipse(closestEntity.Location, new Size(diaACD, diaACD), Pens.Orange);
                        }
                    } 
                }
            }
        }

        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
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
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                     @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

#if false
            /// Ближайшее Entity (найдено при вызове mte.NeedToRun, поэтому строка ниже закомментирована)
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());
#else
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                        false, 0, 0, @this._regionCheck, getCustomRegions());

#endif
            closestEntity = null;

            if (ValidateEntity(target))
            {
                bool distOk = @this._reactionRange<= 0 || target.Location.Distance3DFromPlayer < @this._reactionRange;
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

        #region Вспомогательные методы
        /// <summary>
        /// Поверка валидности сущности <paramref name="entity"/> и её соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        internal bool ValidateEntity(Entity entity)
        {
#if true
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            bool result = false;
            if (extendedDebugInfo)
            {
                string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;
                bool isNull = entity is null;
                bool isValid = isNull ? false : entity.IsValid;
                bool critterOk = isValid ? entity.Critter.IsValid : false;
                bool characterOk = isValid ? entity.Character.IsValid : false;
                bool playerOk = isValid ? entity.Player.IsValid : false;
                bool checkOk = critterOk || characterOk || playerOk ? checkEntity(entity) : false;

                result = checkOk;
                if (!result && extendedDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": FAIL => ",
                                                    isNull ? "NULL" : string.Empty,
                                                    isValid ? string.Empty : ", Invalid",
                                                    isValid ? (critterOk ? ", Critter" : ", Not Critter") : string.Empty,
                                                    isValid ? (playerOk ? ", Player" : ", Not Player") : string.Empty,
                                                    isValid ? (characterOk ? ", Character" : ", Not Character") : string.Empty,
                                                    checkOk ? (@this._entityNameType == EntityNameType.InternalName
                                                                    ? entity.InternalName
                                                                    : entity.NameUntranslated) : string.Empty);
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }
            }
            else result = entity != null && entity.IsValid
                            && (entity.Character.IsValid || entity.Critter.IsValid || entity.Player.IsValid)
                            && checkEntity(entity);

            return result;
#elif false
            return entity != null && entity.IsValid 
                //&& entity.Critter.IsValid <- Некоторые Entity, например игроки, имеют априори невалидный Critter
                //&& !entity.DoNotDraw
                && checkEntity(entity);
#else
            return entity != null && entity.IsValid
                && (entity.Character.IsValid || entity.Critter.IsValid || entity.Player.IsValid)
                && checkEntity(entity);
#endif
        }

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="checkEntity"/>,
        /// использующийся для проверки сущности <paramref name="e"/> на соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool initializer_CheckEntity(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.QuesterActions.DebugMoveToEntity;
            string currentMethodName = extendedDebugInfo ? string.Concat(actionIDstr, '.', MethodBase.GetCurrentMethod().Name) : string.Empty;
            if (predicate != null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": Initialize '" + nameof(checkEntity) + '\''));
                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
            else if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Error, string.Concat(currentMethodName, ": Fail to initialize " + nameof(checkEntity) + '\''));
            return false;
        }

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="getCustomRegions"/>,
        /// который будет возвращать список объектов CustomRegion, 
        /// соответствующих списку их строковых названий <see cref="CustomRegions"/>
        /// </summary>
        /// <returns></returns>
        private List<CustomRegion> initializer_GetCustomRegion()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
            }
            getCustomRegions = functor_GetCustomRegion_Getter;
            return customRegions;
        }

        /// <summary>
        /// Метод, используемый в функторе <see cref="getCustomRegions"/> 
        /// и возвращающий список объектов CustomRegion, соответствующих списку их строковых названий
        /// </summary>
        /// <returns></returns>
        private List<CustomRegion> functor_GetCustomRegion_Getter()
        {
            return customRegions;
        }
        #endregion
    }
}
