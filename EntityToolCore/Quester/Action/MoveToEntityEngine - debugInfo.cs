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
using EntityTools.Logger;
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

        private readonly InstancePropertyAccessor<MoveToEntity, ActionDebug> debug = null;

        private List<CustomRegion> customRegions = null;
        private string label = string.Empty;
        private Entity target = null;
        private Timeout timeout = new Timeout(0);
#endregion

        internal MoveToEntityEngine(MoveToEntity m2e)
        {
            @this = m2e;
            @this.PropertyChanged += PropertyChanged;

#if CORE_DELEGATES
            @this.coreNeedToRun = NeedToRun;
            @this.coreRun = Run;
            @this.coreValidate = Validate;
            @this.coreReset = Reset;
            @this.coreGatherInfos = GatherInfos;
            @this.getString = GetString;
            @this.getTarget = Target; 
#endif
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            debug = @this.GetInstanceProperty<MoveToEntity, ActionDebug>("Debug");
 
            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;
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

#if CORE_DELEGATES
        internal bool NeedToRun()
        {
            //Команда работает с 2 - мя целями:
            //1 - я цель (target) определяет навигацию. Если она зафиксированная(HoldTargetEntity), то не сбрасывается пока жива и не достигнута
            //2 - я ближайшая цель (closest) управляет флагом IgnoreCombat
            //Если HoldTargetEntity ВЫКЛЮЧЕН, то обе цели совпадают - это ближайшая цель 

            if (customRegions == null)
                customRegions = CustomRegionExtentions.GetCustomRegions(@this.CustomRegionNames);

            if (Comparer == null && !string.IsNullOrEmpty(@this.EntityID))
            {
#if DEBUG
                EntityToolsLogger.WriteLine(Logger.LogType.Debug, "MoveToEntityEngine::NeedToRun: Comparer is null. Initialize.");
#endif
                Comparer = EntityToPatternComparer.Get(@this.EntityID, @this.EntityIdType, @this.EntityNameType);
            }


            Entity closestEntity = null;
            if (timeout.IsTimedOut/* || (target != null && (!Validate(target) || (HealthCheck && target.IsDead)))*/)
            {
                closestEntity = SearchCached.FindClosestEntity(@this.EntityID, @this.EntityIdType, @this.EntityNameType, EntitySetType.Complete,
                                                            @this.HealthCheck, @this.ReactionRange, @this.ReactionZRange, @this.RegionCheck, customRegions);
                timeout.ChangeTime(@this.SearchTimeInterval);
            }

            if (!@this.HoldTargetEntity || !Validate(target) || (@this.HealthCheck && target.IsDead))
                target = closestEntity;

            if (Validate(target)
                && !(@this.HealthCheck && target.IsDead)
                && (target.Location.Distance3DFromPlayer <= @this.Distance))
            {
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    if (@this.AttackTargetEntity && target.RelationToPlayer == EntityRelation.Foe)
                    {
                        Astral.Logic.NW.Attackers.List.Add(target);
                        if (@this.IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = false;
                        Astral.Logic.NW.Combats.CombatUnit(target, null);
                    }
                }
                else if (@this.IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = false;
            }
            else if (Validate(closestEntity)
                     && !(@this.HealthCheck && closestEntity.IsDead)
                     && (closestEntity.Location.Distance3DFromPlayer <= @this.Distance))
            {
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Clear();
                    if (@this.AttackTargetEntity && closestEntity.RelationToPlayer != EntityRelation.Friend)
                    {
                        Astral.Logic.NW.Attackers.List.Add(closestEntity);
                        if (@this.IgnoreCombat)
                            Astral.Quester.API.IgnoreCombat = false;
                        Astral.Logic.NW.Combats.CombatUnit(closestEntity, null);
                    }
                }
                else if (@this.IgnoreCombat)
                    Astral.Quester.API.IgnoreCombat = false;
            }
            else if (@this.IgnoreCombat)
                Astral.Quester.API.IgnoreCombat = true;

            return (Validate(target) && (target.Location.Distance3DFromPlayer < @this.Distance));
        }

        internal ActionResult Run()
        {
            if (@this.AttackTargetEntity)
            {
                Astral.Logic.NW.Attackers.List.Clear();
                if (@this.AttackTargetEntity)
                {
                    Astral.Logic.NW.Attackers.List.Add(target);
                    if (@this.IgnoreCombat)
                        Astral.Quester.API.IgnoreCombat = false;
                    Astral.Logic.NW.Combats.CombatUnit(target, null);
                }
                else Astral.Quester.API.IgnoreCombat = false;
            }

            if (@this.IgnoreCombat)
                Astral.Quester.API.IgnoreCombat = false;

            if (@this.StopOnApproached)
                return ActionResult.Completed;
            else return ActionResult.Running;
        }

        internal Entity Target()
        {
            return target;
        }

        internal bool Validate()
        {
            return ValidateEntity(target);
        }

        internal bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && (Comparer?.Invoke(e) == true);
        }
#endif
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
                    {//debug.Value.AddInfo(string.Concat(MethodBase.GetCurrentMethod().Name, ": Found closest Entity[", closestEntity?.GetHashCode().ToString("X2")?? "NULL", "]"));
                        string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Found closest Entity[", (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, "]");
                        Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    }
                    timeout.ChangeTime(EntityTools.EntityTools.PluginSettings.EntityCache.LocalCacheTime);
                }

                if (!@this._holdTargetEntity || !ValidateEntity(target) || (@this._healthCheck && target.IsDead))
                    target = closestEntity;

                bool targetValidationResult = ValidateEntity(target);
                bool targetHealthResult = targetValidationResult ? !(@this._healthCheck && target.IsDead) : false;
                double targetDistance = targetValidationResult ? target.Location.Distance3DFromPlayer : double.MaxValue;
                bool targetDistanceResult = targetDistance <= @this._distance;

                if (targetValidationResult
                    && targetHealthResult
                    && targetDistanceResult)
                {
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Check target Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                        (@this._entityNameType == EntityNameType.InternalName) ? closestEntity.InternalName : closestEntity.NameUntranslated, "] => {",
                                                        targetValidationResult ? "Valid; " : "Invalid; ",
                                                        targetHealthResult ? "Alive; " : "Dead; ",
                                                        targetDistanceResult ? $"Distance({targetDistance})" : "Out_of_Distance", '}');
                        Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    }
                    if (@this._attackTargetEntity)
                    {
                        Astral.Logic.NW.Attackers.List.Clear();
                        if (@this._attackTargetEntity && target.RelationToPlayer == EntityRelation.Foe)
                        {
                            Astral.Logic.NW.Attackers.List.Add(target);
                            if (@this._ignoreCombat)
                                Astral.Quester.API.IgnoreCombat = false;
                            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                            {
                                //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack target Entity[{target.GetHashCode().ToString("X2")}]");
                                string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Attack target Entity[", @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                                Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                            }
                            Astral.Logic.NW.Combats.CombatUnit(target, null);
                        }
                    }
                    else if (@this._ignoreCombat)
                    {
                        Astral.Quester.API.IgnoreCombat = false;
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Enable combat";
                            Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        }
                    }
                }
                else
                {
                    bool closestValidationResult = ValidateEntity(closestEntity);
                    bool closetHealthResult = closestValidationResult ? !(@this._healthCheck && closestEntity.IsDead) : false;
                    double closetDistance = closestValidationResult ? closestEntity.Location.Distance3DFromPlayer : double.MaxValue;
                    bool closestDisnceResult = closetDistance <= @this._distance;

                    if (closestValidationResult
                         && closetHealthResult
                         && closestDisnceResult)
                    {
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Check closest Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                              @this._entityNameType == EntityNameType.InternalName ? target.InternalName : target.NameUntranslated, "] => {",
                                                              closestValidationResult ? "Valid; " : "Invalid; ",
                                                              closetHealthResult ? "Alive; " : "Dead; ",
                                                              closestDisnceResult ? $"Distance({closetDistance})" : "Out_of_Distance", '}');
                            Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
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
                                    //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack closet Entity[{closestEntity.GetHashCode().ToString("X2")}]");
                                    string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Attack closest Entity[", @this._entityNameType == EntityNameType.InternalName ? closestEntity.InternalName : closestEntity.NameUntranslated, ']');
                                    Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                                }
                            Astral.Logic.NW.Combats.CombatUnit(closestEntity, null);
                            }
                        }
                        else if (@this._ignoreCombat)
                        {
                            Astral.Quester.API.IgnoreCombat = false;
                            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                            {
                                string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Enable combat";
                                Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                            }
                        }
                    }
                    else if (@this._ignoreCombat)
                    {
                        Astral.Quester.API.IgnoreCombat = true;
                        if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                        {
                            string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Disable combat";
                            Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                        }
                    }
                }

                bool needToRun = targetValidationResult && targetDistanceResult;
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": NeedToRun =>", NeedToRun, "; Entity[", //target.GetHashCode().ToString("X2"), "] => {",
                                                        @this._entityNameType == EntityNameType.InternalName ? target.InternalName : target.NameUntranslated, "] => {",
                                                        targetValidationResult ? "Valid; " : "Invalid; ",
                                                        // targetHealthResult ? "Alive; " : "Dead; ",
                                                        targetDistanceResult ? $"Distance({targetDistance})" : "Out_of_Distance", '}');

                    Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                }
                return needToRun;
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
                        //debug.Value.AddInfo($"{MethodBase.GetCurrentMethod().Name}: Attack target Entity[{target.GetHashCode().ToString("X2")}]");
                        string debugMsg = string.Concat(MethodBase.GetCurrentMethod().Name, ": Attack target Entity[", @this._entityNameType == EntityNameType.InternalName ? target.InternalName : target.NameUntranslated, ']');
                        Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    }
                    Astral.Logic.NW.Combats.CombatUnit(target, null);
                }
                else
                {
                    Astral.Quester.API.IgnoreCombat = false;
                    if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                    {
                        string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Enable combat";
                        Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                    }
                }
            }

            if (@this._ignoreCombat)
            {
                Astral.Quester.API.IgnoreCombat = false;
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Enable combat";
                    Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                }
            }

            ActionResult actionResult;
            if (@this._stopOnApproached)
                actionResult = ActionResult.Completed;
            else actionResult = ActionResult.Running;

            if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
            {
                string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: ActionResult => {actionResult}";
                Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
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
            checkEntity = internal_CheckEntity_Initializer;
            getCustomRegions = internal_GetCustomRegion_Initializer;
            label = String.Empty;
        }

        public void GatherInfos() { }

        public void OnMapDraw(GraphicsNW graph)
        {
            if (checkEntity(target))
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

            /// Ближайшее Entity (найдено при вызове mte.NeedToRun, поэтому строка ниже закомментирована)
            target = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck, getCustomRegions());
            if (ValidateEntity(target))
            {
                sb.Append("Target: ").AppendLine(target.ToString());
                sb.Append("\tName: ").AppendLine(target.Name);
                sb.Append("\tInternalName: ").AppendLine(target.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(target.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(target.IsDead.ToString());
                sb.Append("\tRegion: '").Append(target.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(target.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(target.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(target.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }

        #region Вспомогательные методы
        internal bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity(e);
        }

        private bool internal_CheckEntity_Initializer(Entity e)
        {
            Predicate<Entity> predicate = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
            if (predicate != null)
            {
#if DEBUG
                EntityToolsLogger.WriteLine(LogType.Debug, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Initialize the Comparer.");
#endif
                if (EntityTools.EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo)
                {
                    string debugMsg = $"{MethodBase.GetCurrentMethod().Name}: Initialize Comparer";
                    Astral.Controllers.Forms.InvokeOnMainThread(() => debug.Value.AddInfo(debugMsg));
                }

                checkEntity = predicate;
                return checkEntity(e);
            }
#if DEBUG
            else EntityToolsLogger.WriteLine(LogType.Error, $"{GetType().Name}[{this.GetHashCode().ToString("X2")}]: Fail to initialize the Comparer.");
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
