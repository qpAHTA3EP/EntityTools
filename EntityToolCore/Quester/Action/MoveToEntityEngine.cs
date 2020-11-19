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
            @this.Engine = this;
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized"); 
#else
            InternalRebase(m2e);
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized");
#endif
        }

        public void Rebase(MoveToEntity m2e)
        {
            InternalRebase(m2e);
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
        }

        private void InternalRebase(MoveToEntity m2e)
        {
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
            @this.Engine = this;
        }

        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(ReferenceEquals(sender, @this))
            {
                switch(e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = functor_CheckEntity_Initializer;
                        label = string.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = functor_CheckEntity_Initializer;
                        break;
                    case "EntityNameType":
                        checkEntity = functor_CheckEntity_Initializer;
                        break;
                    case "CustomRegionNames":
                        getCustomRegions = functor_GetCustomRegion_Initializer;
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
                string currentMethodName = nameof(NeedToRun);// MethodBase.GetCurrentMethod().Name;
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Begins"));

                // Команда работает с 2 - мя целями:
                //   1-я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //   2-я ближайшая цель (closest) управляет флагом IgnoreCombat
                // Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(target);

                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg;
                    if (target is null)
                        debugMsg = string.Concat(actionIDstr, '.', currentMethodName, ": Target[NULL] processing result: '", entityPreprocessingResult, '\'');
                    else debugMsg = string.Concat(actionIDstr, '.', currentMethodName, ": ", Get_DebugStringOfEntity(target, "Target", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\'');
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                {
                    // target не валидный - сбрасываем
                    target = null;
                }

                if (entityPreprocessingResult != EntityPreprocessingResult.Succeeded
                    /*&& timeout.IsTimedOut*/)
                {
                    // target не был обработан
                    // перерыв между поисками ближайшей сущности истек
                    Entity entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                    

                    //timeout.ChangeTime(EntityTools.EntityTools.Config.EntityCache.LocalCacheTime);

                    if (entity != null)
                    {
                        closestEntity = entity;
                        bool closestIsTarget = target != null && target.ContainerId != closestEntity.ContainerId;

                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Found ", Get_DebugStringOfEntity(closestEntity, "ClosestEntity", EntityDetail.Pointer),
                                closestIsTarget ? " that equals to Target": string.Empty));

                        if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                        {
                            // сохраняем ближайшую сущность в target
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            {
                                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Change Target[INVALID] to the ", Get_DebugStringOfEntity(closestEntity, "ClosestEntity", EntityDetail.Pointer)));
                            }
                            target = closestEntity;
                        }
                        else if (closestIsTarget)
                        {
                            // target не является ближайшей сущностью
                            if (!@this._holdTargetEntity)
                            {
                                // Фиксация на target не требуется
                                // ближайшую цель можно сохранить в target
                                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                {
                                    string debugMsg;
                                    if (target is null)
                                        debugMsg = string.Concat(actionIDstr, '.', currentMethodName, ": Change Target[NULL] to the ", Get_DebugStringOfEntity(closestEntity, "ClosestEntity", EntityDetail.Pointer));
                                    else debugMsg = string.Concat(actionIDstr, '.', currentMethodName, ": Change ", Get_DebugStringOfEntity(target, "Target", EntityDetail.Pointer),
                                            " to the ", Get_DebugStringOfEntity(closestEntity, "ClosestEntity", EntityDetail.Pointer));
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                target = closestEntity;
                            }

                            // обрабатываем ближайшую сущность closestEntity
                            entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            {
                                if (closestEntity is null)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": ClosestEntity[NULL] processing result: '", entityPreprocessingResult, '\''));
                                else ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": ", Get_DebugStringOfEntity(closestEntity, "ClosestEntity", EntityDetail.Pointer), " processing result: '", entityPreprocessingResult, '\''));
                            }
                        }
                    }
                    else if(EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": ClosestEntity not found"));
                }
                if (@this._ignoreCombat && entityPreprocessingResult != EntityPreprocessingResult.Succeeded)
                {
                    Astral.Quester.API.IgnoreCombat = true;
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Disable combat"));
                }

                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Result '", entityPreprocessingResult == EntityPreprocessingResult.Succeeded, '\''));
                return entityPreprocessingResult == EntityPreprocessingResult.Succeeded;
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
            string currentMethodName = nameof(Preprocessing_Entity);

            EntityPreprocessingResult result = EntityPreprocessingResult.Failed;

            bool validationResult = ValidateEntity(entity);

            if (validationResult)
            {
                // entity валидно
                bool healthResult = !@this._healthCheck || !entity.IsDead;
                double distance = entity.Location.Distance3DFromPlayer;
                bool distanceResult = distance <= @this._distance;

                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": ", Get_DebugStringOfEntity(entity, "Entity", EntityDetail.Pointer), 
                        " verification=", healthResult && distanceResult, " {Valid; ", (@this._healthCheck) ? (healthResult ? "Alive; " : "Dead; ") : "Skip; ",
                        distanceResult ? "Near (" : "Faraway (", distance.ToString("N2"), ")}"));

                if (healthResult)
                {
                    if (distanceResult)
                    {
                        // entity в пределах заданного расстояния
#if disabled_20200804_2348
                    // производим попытку активировать бой и атаковать entity
                    Attack_Entity(entity);
#endif

                        result = EntityPreprocessingResult.Succeeded;
                    }
                    else result = EntityPreprocessingResult.Faraway;
                }
                else result = EntityPreprocessingResult.Failed;
            }
            else
            {
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Entity Verification=False {Invalid}"));
            }

            return result;
        }

        #region обработка AbortCombatDistance
        /// <summary>
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        private void Attack_Entity(Entity entity)
        {
            string currentMethodName = nameof(Attack_Entity);
            if(entity is null)
            {
                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Entity is NULL. Break"));
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
                    if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                        entityStr = Get_DebugStringOfEntity(entity, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);

                    //Astral.Logic.NW.Attackers.List.Add(entity);
                    //if (@this._ignoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    
                    if (@this._abortCombatDistance > @this._distance)
                    {
                        // запускаем бой с прерыванием за пределами AbortCombatDistance
                        if (abortCombatTask is null || abortCombatTask.Status != TaskStatus.Running)
                        {
                            abortCombatTask = new Task(() => MonitorAbortCombatDistance(entity));

                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"), " and initialize AbortCombatTask#", abortCombatTask.Id));

                            abortCombatTask.Start();
                        }
                        else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"), " and continue AbortCombatTask#", abortCombatTask.Id));
                        
                        Astral.Logic.NW.Combats.CombatUnit(entity, null);
                    }
                    else
                    {
                        // запускаем бой без прерывания
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Engage combat and attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2")));

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

                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Engage combat, and initialize AbortCombatTask#", abortCombatTask.Id));

                        abortCombatTask.Start();
                    }
                    else
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Engage combat and continue AbortCombatTask#", abortCombatTask.Id));
                    } 
                }
                else if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Engage combat"));

                Astral.Quester.API.IgnoreCombat = false;
            }
        }

        #region AbortCombatDistance_by_Task
        Task abortCombatTask;
        static readonly Action<bool> abortCombat = typeof(Astral.Logic.NW.Combats).GetStaticAction<bool>("AbordCombat");

        /// <summary>
        /// Делегат, сравнивающий <see cref="MoveToEntity.AbortCombatDistance"/> с расстоянием между игроком и <paramref name="entity"/>,
        /// и прерывающий бой, при удалении персонажа от <paramref name="entity"/>
        /// </summary>
        internal void MonitorAbortCombatDistance(Entity entity)
        {
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

            string entityStr = Get_DebugStringOfEntity(entity, "Entity", EntityDetail.Pointer);
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
                                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(MonitorAbortCombatDistance), ": Player outside ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Break combat"));
                                Astral.Quester.API.IgnoreCombat = true;
                                abortCombat(true);
                            }
                            else
                            {
                                if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(MonitorAbortCombatDistance), ": Player withing ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Continue..."));
                                Astral.Quester.API.IgnoreCombat = false;
                            }
                        }
                        else
                        {
                            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(MonitorAbortCombatDistance), ": ", entityStr, " is dead. Break combat. Finish AbortCombatTask#", Task.CurrentId));
                            Astral.Quester.API.IgnoreCombat = true;
                            abortCombat(true);
                            return;
                        }
                    }
                    else
                    {
                        if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(MonitorAbortCombatDistance), ": Entity[INVALID]. Break combat. Finish AbortCombatTask#", Task.CurrentId));
                        Astral.Quester.API.IgnoreCombat = true;
                        abortCombat(true);
                        return;
                    }
                }

                Thread.Sleep(500);
            }
            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(MonitorAbortCombatDistance), ": Combat ended. Finish AbortCombatTask#", Task.CurrentId));
            Astral.Quester.API.IgnoreCombat = true;
        }
        #endregion
        #endregion
        #endregion

        public ActionResult Run()
        {
            string currentMethodName = nameof(Run);
            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": Begins"));

#if true//disabled_20200804_2329
            if (ValidateEntity(closestEntity))
                Attack_Entity(closestEntity);
            else if (ValidateEntity(target))
                Attack_Entity(target); 
#endif

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', currentMethodName, ": ActionResult=", actionResult));

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
            checkEntity = functor_CheckEntity_Initializer;
            getCustomRegions = functor_GetCustomRegion_Initializer;
            label = string.Empty;
        }

        public void GatherInfos()
        {
            if (@this.HotSpots.Count == 0)
                @this.HotSpots.Add(EntityManager.LocalPlayer.Location.Clone());

            if (string.IsNullOrEmpty(@this._entityId))
                EntitySelectForm.GUIRequest(ref @this._entityId, ref @this._entityIdType, ref @this._entityNameType);
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
        /// Поверка валидности сущности <paramref name="e"/> и её соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool ValidateEntity(Entity e)
        {
#if DEBUG && ExtendedActionDebugInfo
            bool isNull = e is null;
            bool isValid = isNull ? false : e.IsValid;
            bool critterOk = isNull ? false : e.Critter.IsValid;
            bool checkOk = isNull ? false : checkEntity(e);

            bool result = isValid && checkOk;
            if (!result && EntityTools.EntityTools.Config.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = string.Concat(actionIDstr, '.', nameof(ValidateEntity), ": FAIL => ",
                                                                isNull ? "NULL" : string.Empty,
                                                                isNull || isValid ? string.Empty : "Invalid ",
                                                                isNull || checkOk ? string.Empty : (@this._entityNameType == EntityNameType.InternalName ? e.InternalName : e.NameUntranslated));
                ETLogger.WriteLine(LogType.Debug, debugMsg);
            } 

            return result;
#elif false
            return e != null && e.IsValid 
                //&& e.Critter.IsValid <- Некоторые Entity, например игроки, имеют априори невалидный Critter
                //&& !e.DoNotDraw
                && checkEntity(e);
#else
            return e != null && e.IsValid
                && (e.Character.IsValid || e.Critter.IsValid || e.Player.IsValid)
                && checkEntity(e);
#endif
        }

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="checkEntity"/>,
        /// использующийся для проверки сущности <paramref name="e"/> на соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool functor_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', nameof(functor_CheckEntity_Initializer), ": Initialize ", nameof(checkEntity)));
#endif
                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
#if DEBUG 
            else ETLogger.WriteLine(LogType.Error, string.Concat(actionIDstr, '.', nameof(functor_CheckEntity_Initializer), "Fail to initialize ", nameof(checkEntity)));
#endif
            return false;
        }

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="getCustomRegions"/>,
        /// который будет возвращать список объектов CustomRegion, 
        /// соответствующих списку их строковых названий <see cref="CustomRegions"/>
        /// </summary>
        /// <returns></returns>
        private List<CustomRegion> functor_GetCustomRegion_Initializer()
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

        /// <summary>
        /// Набор флагов, определяющих детализацию описания <seealso cref="Entity"/>, возвращаемого <seealso cref="MoveToEntityEngine.Get_DebugStringOfEntity"/>
        /// </summary>
        [Flags]
        enum EntityDetail
        {
            Nope = 0,
            Pointer = 1,
            RelationToPlayer = 2,
            Alive = 4
        }

        /// <summary>
        /// Краткая отладочная информация об <paramref name="entity"/>, используемая в логах
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entityLabel"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        private string Get_DebugStringOfEntity(Entity entity, string entityLabel = "", EntityDetail detail = EntityDetail.Nope)
        {
            return string.Concat(entityLabel, "[",
                ((detail & EntityDetail.Pointer) > 0) ? /*entity.Pointer*/entity.ContainerId + "; " : string.Empty,
                (@this._entityNameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated,
                ((detail & EntityDetail.RelationToPlayer) > 0) ? "; " + entity.RelationToPlayer : string.Empty,
                ((detail & EntityDetail.Alive) > 0) ? (entity.IsDead ? "; Dead" : "; Alive") : string.Empty,
                ']');
        }
        #endregion
    }
}
