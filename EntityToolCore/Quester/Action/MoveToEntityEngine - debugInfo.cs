﻿#define CORE_INTERFACES
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
        private readonly string hashCode = string.Empty;
#region Данные 
        private Predicate<Entity> checkEntity = null;
        private Func<List<CustomRegion>> getCustomRegions = null;

#if false
        // Запись отладочной информации в поле MoveToEntity.Debug
        // приводит к критической ошибке и аварийному завершению Astral'a
        private readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null; 
#endif

        private List<CustomRegion> customRegions = null;
        private string label = string.Empty;
        private Entity target = null;
        private Timeout timeout = new Timeout(0);
#endregion

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
            @this = m2e;

            hashCode = @this.GetHashCode().ToString("X2");

            @this.PropertyChanged += PropertyChanged;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            //debug = @this.GetInstanceProperty<MoveToEntity, ActionDebug>("Debug");

            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{hashCode}] initialized: {ActionLabel}");
        }

        internal void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(object.ReferenceEquals(sender, @this))
            {
                switch(e.PropertyName)
                {
                    case "EntityID":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        label = String.Empty;
                        break;
                    case "EntityIdType":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "EntityNameType":
                        checkEntity = internal_CheckEntity_Initializer;//EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                        break;
                    case "CustomRegionNames":
                        getCustomRegions = internal_GetCustomRegion_Initializer;// CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
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
                //Команда работает с 2 - мя целями:
                //1 - я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
                //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
                //Если HoldTargetEntity ВЫКЛЮЧЕН, то обе цели совпадают - это ближайшая цель 

                Entity closestEntity = null;
                if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
                {
                    closestEntity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        // 1
                        //debug.Value.AddInfo(string.Concat(MethodBase.GetCurrentMethod().Name, ": Found closest Entity[", closestEntity?.GetHashCode().ToString("X2")?? "NULL", "]"));
                        string debugMsg = string.Empty;
                        if (closestEntity != null)
                            debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Found closest Entity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                        else debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: No entity found");
                        // 2
                        //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        // 3
                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                bool closestValidationResult = closestEntity != null;

                bool targetValidationResult = ValidateEntity(target);
                bool targetHealthResult = targetValidationResult ? !(@this._healthCheck && target.IsDead) : false;
                double targetDistance = targetValidationResult ? target.Location.Distance3DFromPlayer : double.MaxValue;
                bool targetDistanceResult = targetDistance <= @this._distance;

                if (closestValidationResult && !(@this._holdTargetEntity && targetValidationResult && targetHealthResult))
                {
                    // closestEntity - Валидно, а target - Не валидно, Мертво или Не стоит флаг удержания HoldTargetEntity
                    // Заменяем 
                    target = closestEntity;

                    targetValidationResult = closestValidationResult;
                    targetHealthResult = targetValidationResult ? !(@this._healthCheck && target.IsDead) : false;
                    targetDistance = targetValidationResult ? target.Location.Distance3DFromPlayer : double.MaxValue;
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
                    bool closestDisnceResult = closetDistance <= @this._distance;

                    if (closestValidationResult
                         && closetHealthResult
                         && closestDisnceResult)
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Check closest Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                              @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, "] => {",
                                                              closestValidationResult ? "Valid; " : "Invalid; ",
                                                              closetHealthResult ? "Alive; " : "Dead; ",
                                                              closestDisnceResult ? $"Distance({closetDistance})" : "Out_of_Distance", '}');
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
                    if(target != null)
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
        }

        public ActionResult Run()
        {
            if (@this._attackTargetEntity)
            {
                Astral.Logic.NW.Attackers.List.Clear();
                if (@this._attackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Add(target);
                    if (@this._ignoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        // 1
                        //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack target Entity[{target.GetHashCode().ToString("X2")}]");
                        string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, '[', hashCode, "]: Attack target Entity[", @this._entityNameType == EntityNameType.InternalName ? target.InternalName : target.NameUntranslated, ']');
                        // 2
                        //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        // 3
                        ETLogger.WriteLine(LogType.Debug, debugMsg);
                    }
                    Astral.Logic.NW.Combats.CombatUnit(target, null);
                }
                else
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

            if (@this._ignoreCombat)
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

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = $"{MethodBase.GetCurrentMethod().Name}[{hashCode}]: ActionResult => {actionResult}";
                // 2
                //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                // 3
                ETLogger.WriteLine(LogType.Debug, debugMsg);
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
            checkEntity = internal_CheckEntity_Initializer;
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
        internal bool ValidateEntity(Entity e)
        {
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
                
            return  result;

        }

        private bool internal_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                ETLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{hashCode}]: Initialize the Comparer.");
#endif
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = $"{MethodBase.GetCurrentMethod().Name}[{hashCode}]: Initialize Comparer";
                    // 2
                    //Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    // 3
                    ETLogger.WriteLine(LogType.Debug, debugMsg);
                }

                checkEntity = predicate;
                return e != null && checkEntity(e);
            }
#if DEBUG
            else ETLogger.WriteLine(LogType.Error, $"{GetType().Name}[{hashCode}]: Fail to initialize the Comparer.");
#endif
            return false;
        }

        private List<CustomRegion> internal_GetCustomRegion_Initializer()
        {
            if (customRegions == null && @this._customRegionNames != null)
            {
                customRegions = CustomRegionExtentions.GetCustomRegions(@this._customRegionNames);
            }
            getCustomRegions = internal_GetCustomRegion_Getter;
            return customRegions;
        }

        private List<CustomRegion> internal_GetCustomRegion_Getter()
        {
            return customRegions;
        } 
        #endregion
    }
}
