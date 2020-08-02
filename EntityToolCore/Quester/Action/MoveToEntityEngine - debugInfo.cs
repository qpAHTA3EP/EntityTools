#define ActionID_Identifier

using System.Linq;
using Astral;
using Astral.Classes;
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
using System.Reflection;

namespace EntityCore.Quester.Action
{
    public class MoveToEntityEngine : IEntityInfos
#if CORE_INTERFACES
        , IQuesterActionEngine
#endif
    {
        /// <summary>
        /// ссылка на команду, для которой предоставляется функционал ядра
        /// </summary>
        private MoveToEntity @this;
#region Данные 
        private Predicate<Entity> checkEntity = null;
        private Func<List<CustomRegion>> getCustomRegions = null;


        // Запись отладочной информации в поле MoveToEntity.Debug
        // приводит к критической ошибке и аварийному завершению Astral'a
        //private readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null; 
        internal readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null;

#if ActionID_Identifier
        private readonly string actionIDstr = string.Empty; 
#endif
#if HashCode_Identifier
        private readonly string hashCode = string.Empty; 
#endif

        private List<CustomRegion> customRegions = null;
        private string label = string.Empty;
        private Entity target = null;
        private Timeout timeout = new Timeout(0);
#endregion

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
            @this = m2e;

#if HashCode_Identifier
            hashCode = @this.GetHashCode().ToString("X2"); 
#endif
#if ActionID_Identifier
            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.ActionID, ']'); 
#endif

            @this.PropertyChanged += PropertyChanged;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            debug = @this.GetInstanceProperty<MoveToEntity, ActionDebug>("Debug");
            debug.Value.AddInfo($"{actionIDstr} initialized");

            checkEntity = functor_CheckEntity_Initializer;
            getCustomRegions = functor_GetCustomRegion_Initializer;

#if HashCode_Identifier
            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{hashCode}] initialized: {ActionLabel}"); 
#endif
#if ActionID_Identifier
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized");
#endif
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(object.ReferenceEquals(sender, @this))
            {
                switch(e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = functor_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        label = String.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = functor_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "EntityNameType":
                        checkEntity = functor_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "CustomRegionNames":
                        getCustomRegions = functor_GetCustomRegion_Initializer;// CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
                        break;
                }

                target = null;
                timeout.ChangeTime(0);
            }
        }

#if CORE_INTERFACES
        public bool NeedToRun
        {
            get
            {
                string currentMethodName = MethodBase.GetCurrentMethod().Name;
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Begins");

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }
                //Команда работает с 2 - мя целями:
                //1 - я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
                //Если HoldTargetEntity ВЫКЛЮЧЕН и обе цели совпадают - это ближайшая цель 
                EntityPreprocessingResult entityPreprocessingResult = Preprocessing_Entity(target);
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Empty;
                    if (target is null)
                        debugMsg = string.Concat(currentMethodName, ": Target[NULL] processing result: '", entityPreprocessingResult, '\'');
                    else debugMsg = string.Concat(currentMethodName, ": Target[", (@this._entityNameType == EntityNameType.InternalName) ? target.InternalName : target.NameUntranslated, "] processing result: '", entityPreprocessingResult, '\'');

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }

                Entity closestEntity = null;

                if (entityPreprocessingResult != EntityPreprocessingResult.Completed
                    && timeout.IsTimedOut)
                {
                    // target не был обработан
                    // перерыв между поисками ближайшей сущности истек
                    closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                   @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Empty;
                        if (closestEntity is null)
                            debugMsg = string.Concat(currentMethodName, ": ClosestEntity not found");
                        else debugMsg = string.Concat(currentMethodName, ": Found ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');

                        debug.Value.AddInfo(debugMsg);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }

                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                if (closestEntity != null)
                {
                    if (entityPreprocessingResult == EntityPreprocessingResult.Failed)
                    {
                        // сохраняем ближайшую сущность в target
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Change Target[INVALID] to the ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        target = closestEntity;
                    }
                    else if (target != closestEntity)
                    {
                        // target не является ближайшей сущностью
                        if (!@this._holdTargetEntity)
                        {
                            // Фиксация на target не требуется
                            // ближайшую цель можно сохранить в target
                            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                            {
                                string debugMsg = string.Empty;
                                if(target is null)
                                    string.Concat(currentMethodName, ": Change Target[NULL] to the ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                                else string.Concat(currentMethodName, ": Change Target[", (@this._entityNameType == EntityNameType.InternalName) ? target.InternalName : target.NameUntranslated, 
                                    "] to the ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');

                                debug.Value.AddInfo(debugMsg);
                                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                            }
                            target = closestEntity;
                        }

                        // обрабатываем ближайшую сущность closestEntity
                        entityPreprocessingResult = Preprocessing_Entity(closestEntity);
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, "] processing result: '", entityPreprocessingResult, '\'');

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                    }
                    //if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    //{
                    //    string debugMsg = string.Concat(currentMethodName, ": Result '", entityPreprocessingResult == EntityPreprocessingResult.Completed, '\'');

                    //    debug.Value.AddInfo(debugMsg);
                    //    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    //}
                    //entityPreprocessingResult == EntityPreprocessingResult.Completed;
                }
                if (@this._ignoreCombat && entityPreprocessingResult != EntityPreprocessingResult.Completed)
                {
                    Astral.Quester.API.IgnoreCombat = true;
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(currentMethodName, ": Disable combat");

                        debug.Value.AddInfo(debugMsg);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }
                }

                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Result '", entityPreprocessingResult == EntityPreprocessingResult.Completed, '\'');

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }
                return entityPreprocessingResult == EntityPreprocessingResult.Completed;
            }
#if disabled_20200707
            get
            {
                Entity closestEntity = null;
                bool closestValidationResult = false;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {

                    closestValidationResult = closestEntity != null;

                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        // 1
                        //debug.Value.AddInfo(string.Concat(MethodBase.GetCurrentMethod().Name, ": Found closest Entity[", closestEntity?.GetHashCode().ToString("X2")?? "NULL", "]"));
                        string debugMsg = string.Empty;
                        if (closestValidationResult)
                            debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Found closest Entity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                        else debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: No entity found");
                        // 2
                        //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        // 3
                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                bool targetValidationResult = ValidateEntity(target);
                bool targetHealthResult = targetValidationResult ? !(@this._healthCheck && target.IsDead) : false;
                double targetDistance = targetValidationResult ? target.Location.Distance3DFromPlayer : double.MaxValue;
                bool targetDistanceResult = targetDistance <= @this._distance;

                if (closestValidationResult && !(@this._holdTargetEntity && targetValidationResult && targetHealthResult))
                {
                    // closestEntity - Валидно, а target - Не валидно, Мертво или Не стоит флаг удержания HoldTargetEntity
                    // Заменяем 
                    target = closestEntity;

                    if (targetValidationResult = closestValidationResult)
                    {
                        targetHealthResult = !(@this._healthCheck && target.IsDead);
                        targetDistance = target.Location.Distance3DFromPlayer;
                    }
                    else
                    {
                        targetHealthResult = false;
                        targetDistance = double.MaxValue;
                    }
                    targetDistanceResult = targetDistance <= @this._distance;
                }


                if (targetValidationResult
                    && targetHealthResult
                    && targetDistanceResult)
                {
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Check target Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                        (@this._entityNameType == EntityNameType.InternalName) ? target.InternalName : target.NameUntranslated, "] => {",
                                                        targetValidationResult ? "Valid; " : "Invalid; ",
                                                        targetHealthResult ? "Alive; " : "Dead; ",
                                                        targetDistanceResult ? $"Distance({targetDistance})" : "Out_of_Distance", '}');
                        // 2 
                        //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        // 3 
                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    if (@this._attackTargetEntity)
                    {
                        Astral.Logic.NW.Attackers.List.Clear();
                        if (@this._attackTargetEntity && target.RelationToPlayer != EntityRelation.Friend)
                        {
                            Astral.Logic.NW.Attackers.List.Add(target);
                            if (@this._ignoreCombat)
                                Astral.Quester.API.IgnoreCombat = false;
                            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                            {
                                // 1
                                //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack target Entity[{target.GetHashCode().ToString("X2")}]");

                                string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Attack target Entity[", @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                                // 2
                                //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                                // 3
                                ETLogger.WriteLine(LogType.Debug, debugMsg);
                            }
                            Astral.Logic.NW.Combats.CombatUnit(target, null);
                        }
                    }
                    else if (@this._ignoreCombat)
                    {
                        Astral.Quester.API.IgnoreCombat = false;
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = $"{MethodBase.GetCurrentMethod().Name}[{hashCode}]: Enable combat";
                            // 2
                            //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                            // 3
                            ETLogger.WriteLine(LogType.Debug, debugMsg);
                        }
                    }
                }
                else
                {
                    bool closetHealthResult = closestValidationResult ? !(@this._healthCheck && closestEntity.IsDead) : false;
                    double closetDistance = closestValidationResult ? closestEntity.Location.Distance3DFromPlayer : double.MaxValue;
                    bool closestDistanceResult = closetDistance <= @this._distance;

                    if (closestValidationResult
                         && closetHealthResult
                         && closestDistanceResult)
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Check closest Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                              @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, "] => {",
                                                              closestValidationResult ? "Valid; " : "Invalid; ",
                                                              closetHealthResult ? "Alive; " : "Dead; ",
                                                              closestDistanceResult ? $"Distance({closetDistance})" : "Out_of_Distance", '}');
                            // 2
                            //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                            // 3
                            ETLogger.WriteLine(LogType.Debug, debugMsg);
                        }

                        if (@this._attackTargetEntity)
                        {
                            Astral.Logic.NW.Attackers.List.Clear();
                            if (@this._attackTargetEntity && closestEntity.RelationToPlayer != EntityRelation.Friend)
                            {
                                Astral.Logic.NW.Attackers.List.Add(closestEntity);
                                if (@this._ignoreCombat)
                                    Astral.Quester.API.IgnoreCombat = false;
                                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                                {
                                    // 1
                                    //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack closet Entity[{closestEntity.GetHashCode().ToString("X2")}]");
                                    string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Attack closest Entity[", @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                                    // 2
                                    //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                                    // 3
                                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                                }
                                Astral.Logic.NW.Combats.CombatUnit(closestEntity, null);
                            }
                        }
                        else if (@this._ignoreCombat)
                        {
                            Astral.Quester.API.IgnoreCombat = false;
                            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                            {
                                string debugMsg = $"{MethodBase.GetCurrentMethod().Name}[{hashCode}]: Enable combat";
                                // 2
                                //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                                // 3
                                ETLogger.WriteLine(LogType.Debug, debugMsg);
                            }
                        }
                    }
                    else if (@this._ignoreCombat)
                    {
                        Astral.Quester.API.IgnoreCombat = true;
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = $"{MethodBase.GetCurrentMethod().Name}[{hashCode}]: Disable combat";
                            // 2
                            //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                            // 3
                            ETLogger.WriteLine(LogType.Debug, debugMsg);
                        }
                    }
                }

                bool result = targetValidationResult && targetDistanceResult;
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Empty;
                    if (target != null)
                        debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: NeedToRun =>", result, "; Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                        @this._entityNameType == EntityNameType.InternalName ? target.InternalName : target.NameUntranslated, "] => {",
                                                        targetValidationResult ? "Valid; " : "Invalid; ",
                                                        // targetHealthResult ? "Alive; " : "Dead; ",
                                                        targetDistanceResult ? $"Distance({targetDistance})" : "Out_of_Distance", '}');
                    else debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: NeedToRun =>", result, "; Entity not found");
                    // 2
                    //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    // 3
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }
                return result; 
            }
#endif
        }

        #region Декомпозиция основного функционала
        enum EntityPreprocessingResult
        {
            /// <summary>
            /// Некорректная сущность, обработка провалена
            /// </summary>
            Failed,
            /// <summary>
            /// Сущность в пределах заданной дистации и "обработана" в соответствии с настройками
            /// </summary>
            Completed,
            /// <summary>
            /// Суoнщсть не достигнута (находится за пределами заданой дистанции)
            /// </summary>
            Faraway
        }

        /// <summary>
        /// Анализ <paramref name="entity"/> на предмет возможности вступления в бой
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private EntityPreprocessingResult Preprocessing_Entity(Entity entity)
        {
            string currentMethodName = MethodBase.GetCurrentMethod().Name;

            EntityPreprocessingResult result = EntityPreprocessingResult.Failed;

            bool validationResult = ValidateEntity(entity);
            bool healthResult = false;
            double distance = double.MaxValue;
            bool distanceResult = false;

            if (validationResult)
            {
                // entity валидно
                healthResult = @this._healthCheck || !entity.IsDead;
                distance = entity.Location.Distance3DFromPlayer;
                distanceResult = distance <= @this._distance;

                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": Verification=", healthResult && distanceResult, " {Validation=", validationResult,
                                                    "; HealthCheck=", (@this._healthCheck) ? healthResult.ToString() : "Skip",
                                                    "; DistanceCheck=", distanceResult, '(', distance.ToString("N2"), ")}");

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }

                if (healthResult && distanceResult)
                {
                    // entity в пределах заданного расстояния
                    // производим попытку активировать бой и атаковать entity
                    Attack_Entity(entity);

                    result = EntityPreprocessingResult.Completed;
                }
                else result = EntityPreprocessingResult.Faraway;
            }
            else if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = string.Concat(currentMethodName, ": Verification=False {Validation=False}");

                debug.Value.AddInfo(debugMsg);
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
            }

            return result;
        }

        /// <summary>
        /// Нападение на сущность <paramref name="entity"/> в зависимости от настроек команды
        /// </summary>
        /// <param name="entity"></param>
        private void Attack_Entity(Entity entity)
        {
            string currentMethodName = MethodBase.GetCurrentMethod().Name;

            if (entity != null && @this._attackTargetEntity)
            {
                // entity нужно атаковать
                Astral.Logic.NW.Attackers.List.Clear();
                if (entity.RelationToPlayer == EntityRelation.Foe)
                {
                    string entityStr = string.Empty;
                    // entity враждебно и должно быть атаковано
                    Astral.Logic.NW.Attackers.List.Add(entity);
                    if (@this._ignoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        entityStr = Get_DebugStringOfEntity(entity, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer); //string.Concat("Entity[", @this._entityNameType == EntityNameType.InternalName ? entity.InternalName : entity.NameUntranslated, "; ", entity.RelationToPlayer, ']');
                    // атака entity
                    if (@this._abortCombatDistance > @this._distance)
                    {
                        // запускаем бой с прерыванием за пределами AbortCombatDistance

                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"), " and MaintainCombatDistance");

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
#if disabled_20200801_1146
                        if (Astral.Logic.NW.Combats.CombatUnit(entity, () => Check_PlayerOutsideCombatRadius_And_BreakCombat(entity))) 
#else
                        Astral.Logic.NW.Combats.CombatUnit(entity, () => Check_PlayerOutsideCombatRadius_And_BreakCombat(entity));
#endif
                        {
                            if (@this._ignoreCombat)
                            {
                                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                                {
                                    string debugMsg = string.Empty;
                                    if (entity.IsDead)
                                        debugMsg = string.Concat(currentMethodName, ": Target ", entityStr, " is DEAD. ",
                                            (@this._ignoreCombat) ? "Disable combat and continue." : "Continue");
                                    else debugMsg = string.Concat(currentMethodName, ": Target ", entityStr, " is still ALIVE at the distance ", entity.CombatDistance3.ToString("N2"),
                                        (@this._ignoreCombat) ? ". Disable combat and continue." : ". Continue");
                                    debug.Value.AddInfo(debugMsg);
                                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                                }
                                Astral.Quester.API.IgnoreCombat = true;
                            }
                        }
                    }
                    else
                    {
                        // запускаем бой без прерывания

                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Engage combat and attack ", entityStr, " at the distance ", entity.CombatDistance3.ToString("N2"));

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
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
                    Entity closestEnemy = Search_ClosestEnemy(entity);
                    if (closestEnemy != null)
                    {
                        Astral.Quester.API.IgnoreCombat = false;
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Engage combat, attack closest enemy ",
                                closestEnemy," and MaintainCombatDistance");
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        Astral.Logic.NW.Combats.CombatUnit(closestEnemy, () => Check_PlayerOutsideCombatRadius_And_BreakCombat(closestEnemy));
                        Astral.Quester.API.IgnoreCombat = true;
                    }
                    else if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(currentMethodName, ": No enemies was found, ignoring combat");
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }
                }
                else
                {
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = $"{currentMethodName}: Engage combat";
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }

                    Astral.Quester.API.IgnoreCombat = false;
                }
            }
        }

        /// <summary>
        /// Поиск ближайшего противника, рядом с <paramref name="anchorEntity"/>
        /// </summary>
        /// <param name="anchorEntity">Целевая сущность, рядом с которой активируется бой</param>
        /// <returns></returns>
        private Entity Search_ClosestEnemy(Entity anchorEntity)
        {
            string currentMethodName = MethodBase.GetCurrentMethod().Name;
            double combatDist = Math.Max(@this._distance, @this._abortCombatDistance);
            Entity enemy = null;
            if (anchorEntity != null && anchorEntity.IsValid)
            {
                // 1. Поиск противника в списке атакующих
                if (Astral.Logic.NW.Attackers.List?.Count > 0)
                {
                    double dist = double.MaxValue;
                    double curDist = double.MaxValue;
                    double curTarDist = double.MaxValue;
                    foreach (Entity ett in Astral.Logic.NW.Attackers.List)
                    {
                        if (ett.IsValid && !ett.IsDead)
                        {
                            curDist = ett.Location.Distance3DFromPlayer;
                            curTarDist = MathHelper.Distance3D(anchorEntity.Location, ett.Location);
                            if (curTarDist <= combatDist
                                && dist > curDist)
                            {
                                dist = curDist;
                                enemy = ett;
                            } 
                        }
                    }
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Empty;

                        if(enemy != null && enemy.IsValid)
                             string.Concat(currentMethodName, ": Select ", Get_DebugStringOfEntity(enemy, "Attacker", EntityDetail.Pointer), " (", dist.ToString("N2"), " from Player)");
                        else string.Concat(currentMethodName, ": No Attackers was found");
                        debug.Value.AddInfo(debugMsg);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }
                }
                // 2. Поиск противника в пределах 
                if(enemy is null || !enemy.IsValid )
                {
                    enemy = SearchDirect.ClosestEnemy((Entity ett) => Check_EntityInCombatRadius(anchorEntity, ett));
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Empty;
                        if (enemy != null && enemy.IsValid)
                             debugMsg = string.Concat(currentMethodName, ": Select ", Get_DebugStringOfEntity(enemy, "Enemy", EntityDetail.Pointer), " (", enemy.Location.Distance3DFromPlayer.ToString("N2"), " from Player)");
                        else debugMsg = string.Concat(currentMethodName, ": No enemies was found");
                        debug.Value.AddInfo(debugMsg);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }
                } 
            }
            return enemy;
        }

        /// <summary>
        /// Проверка <paramref name="entity"/> на предмет нахождения в пределах <seealso cref="AbortCombatDistance"/> от <paramref name="anchorEntity"/>
        /// </summary>
        /// <param name="anchorEntity"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Check_EntityInCombatRadius(Entity anchorEntity, Entity entity)
        {
            double tarDist = MathHelper.Distance3D(anchorEntity.Location, entity.Location);
            double plDist = entity.Location.Distance3DFromPlayer;
            return tarDist<@this._abortCombatDistance && plDist< 80;
        }

        /// <summary>
        /// Делегат, реализующий обработку <see cref="AbortCombatDistance"/>
        /// То есть прерывающий бой, при удалении персонажа от <paramref name="anchorEntity"/>
        /// </summary>
        /// <param name="anchorEntity">Целевая сущность, рядом с которой активируется бой</param>
        /// <returns></returns>
        private bool Check_PlayerOutsideCombatRadius_And_BreakCombat(Entity anchorEntity)
        {
            string currentMethodName = MethodBase.GetCurrentMethod().Name;
            string entityStr = string.Empty;
            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                entityStr = Get_DebugStringOfEntity(anchorEntity, "Entity", EntityDetail.Pointer | EntityDetail.RelationToPlayer);// string.Concat("Entity[", (@this._entityNameType == EntityNameType.InternalName) ? anchorEntity.InternalName : anchorEntity.NameUntranslated, ']');

            bool anchorEntityValid = anchorEntity != null && anchorEntity.IsValid;
            if (anchorEntityValid )
            {
                if (!@this._healthCheck || !anchorEntity.IsDead)
                {
                    double dist = anchorEntity.Location.Distance3DFromPlayer;
                    if (dist >= @this._abortCombatDistance)
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Player outside ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Break combat");

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        return true;
                    }
                    else
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": Player withing ", nameof(@this.AbortCombatDistance), " (", dist.ToString("N2"), " to ", entityStr, "). Continue...");

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        return false;
                    } 
                }
                else if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": ", entityStr, " is dead. Break combat");

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }
            }
#if disabled_20200731_1709
            else
            {
                Entity entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                              false, 0, 0, @this._regionCheck, getCustomRegions());
                if (entity != null)
                {
                    target = entity;
                    entityStr = string.Concat("ClosestEntity[", (@this._entityNameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated, ']');
                    double dist = entity.Location.Distance3DFromPlayer;
                    if (dist >= @this.Distance)
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": ", entityStr, " at the Distance ", dist.ToString("N2"), ". Break combat");

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        return true;
                    }
                    else
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(currentMethodName, ": ", entityStr, " at the Distance ", dist.ToString("N2"), ". Continue...");

                            debug.Value.AddInfo(debugMsg);
                            ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                        }
                        return false;
                    }
                }
                else
                {
                    entityStr = "";
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(currentMethodName, ": ClosestEntity[NULL]. Break combat");

                        debug.Value.AddInfo(debugMsg);
                        ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                    }
                    return true;
                }
            } 
#else
            else
            {
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(currentMethodName, ": ", entityStr, " is not valid. Break combat");

                    debug.Value.AddInfo(debugMsg);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
                }
            }
#endif
            return true;
        }
        #endregion

        public ActionResult Run()
        {
            string currentMethodName = MethodBase.GetCurrentMethod().Name;
            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = string.Concat(currentMethodName, ": Begins");

                debug.Value.AddInfo(debugMsg);
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
            }

            Attack_Entity(target);

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = string.Concat(currentMethodName, ": ActionResult=", actionResult);

                debug.Value.AddInfo(debugMsg);
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg));
            } 

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
                    else return EntityManager.LocalPlayer.Location.Clone();
                }
                return new Vector3();
            }
        }

        public void InternalReset()
        {
            target = null;
            checkEntity = functor_CheckEntity_Initializer;
            getCustomRegions = functor_GetCustomRegion_Initializer;
            label = String.Empty;
        }

        public void GatherInfos() { }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (ValidateEntity(target))
                graph.drawFillEllipse(target.Location, new Size(10, 10), Brushes.Beige);
            //TODO: Отображение на карте радиусов Distance и AbortCombatDistance
        }
#endif
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
#if ExtendedActionDebugInfo
            bool isNull = e is null;
            bool isValid = isNull ? false : e.IsValid;
            bool checkOk = isNull ? false : checkEntity(e);

            bool result = !isNull && isValid && checkOk;
            if (!result && EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: FAIL => ",
                                                                isNull ? "NULL" : string.Empty,
                                                                isNull || isValid ? string.Empty : "Invalid ",
                                                                isNull || checkOk ? string.Empty : (@this._entityNameType == EntityNameType.InternalName ? e.InternalName : e.NameUntranslated));
                // 2
                //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                // 3
                ETLogger.WriteLine(LogType.Debug, debugMsg);
            } 

            return result;
#else
            return e != null && checkEntity(e);
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
#if HashCode_Identifier
                string debugMsg = $"{GetType().Name}[{hashCode}]: Initialize the Comparer."; 
#elif ActionID_Identifier
                string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name,": Initialize the Comparer.");
#endif
                debug.Value.AddInfo(debugMsg);
                ETLogger.WriteLine(LogType.Debug, string.Concat(actionIDstr, '.', debugMsg)); 
#endif
                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
#if DEBUG 
            else
            {
#if HashCode_Identifier
            string debugMsg = $"{GetType().Name}[{hashCode}]: Fail to initialize the Comparer.";
#elif ActionID_Identifier
                string debugMsg = string.Concat(actionIDstr, "Fail to initialize the Comparer.");
#endif
                debug.Value.AddInfo(debugMsg);
                ETLogger.WriteLine(LogType.Error, string.Concat(actionIDstr, '.', debugMsg));
            }
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

        enum EntityDetail
        {
            Nope = 0,
            Pointer = 1,
            RelationToPlayer = 2
        }

        /// <summary>
        /// Краткая отладочная информация об <paramref name="entity"/>, используемая в логах
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string Get_DebugStringOfEntity(Entity entity, string entityLabel = "", bool ptr = false, bool foe = false)
        {
            return string.Concat(entityLabel, "[", 
                (ptr) ? entity.Pointer.ToString() + "; " : string.Empty, 
                (@this._entityNameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated, 
                (foe) ? entity.RelationToPlayer.ToString() : string.Empty,
                ']');
        }

        private string Get_DebugStringOfEntity(Entity entity, string entityLabel = "", EntityDetail detail = EntityDetail.Nope)
        {
            return string.Concat(entityLabel, "[",
                ((detail & EntityDetail.Pointer) > 0) ? entity.Pointer.ToString() + "; " : string.Empty,
                (@this._entityNameType == EntityNameType.InternalName) ? entity.InternalName : entity.NameUntranslated,
                ((detail & EntityDetail.RelationToPlayer) > 0) ? entity.RelationToPlayer.ToString() : string.Empty,
                ']');
        }
        #endregion
    }
}
