#if DEBUG
#define DEBUG_ExecuteSpecificPower
#endif
//#define REFLECTION_ACCESS
#define SMART_REFLECTION_ACCESS

using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Reflection;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityCore.UCC.Actions
{
    public class ExecuteSpecificPowerEngine : IEntityInfos, IUccActionEngine
    {
        #region Данные
        private ExecuteSpecificPower @this;

        private Predicate<Entity> checkEntity { get; set; } = null;
        private Entity entity = null;
        private bool slotedState = false;
        private string label = string.Empty;
        private string actionIDstr;

        private int attachedGameProcessId = 0;
        private Power power = null;
        #endregion

        internal ExecuteSpecificPowerEngine(ExecuteSpecificPower esp)
        {
#if false
            @this = esp;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}"); 
#else
            InternalRebase(esp); 
            ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} initialized: {Label()}");
#endif
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(ReferenceEquals(sender, @this))
            {
                if (ReferenceEquals(sender, @this))
                {
                    switch (e.PropertyName)
                    {
                        case nameof(@this.EntityID):
                            checkEntity = initialize_CheckEntity;
                            label = string.Empty;
                            break;
                        case nameof(@this.EntityIdType):
                            checkEntity = initialize_CheckEntity;
                            break;
                        case nameof(@this.EntityNameType):
                            checkEntity = initialize_CheckEntity;
                            break;
                        case nameof(@this.PowerId):
                            power = null;
                            label = string.Empty;
                            break;
                        case nameof(@this.CheckInTray):
                            label = string.Empty;
                            break;
                    }
                    entity = null;
                    power = null;
                }
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is ExecuteSpecificPower execPower)
            {
                if (InternalRebase(execPower))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(ExecuteSpecificPower) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(ExecuteSpecificPower execPower)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = new EntityTools.Core.Proxies.UccActionProxy(@this);
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

            checkEntity = initialize_CheckEntity;

            actionIDstr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                Power p = GetCurrentPower();

                return ValidatePower(p)
                        && (!@this._checkPowerCooldown || !p.IsOnCooldown())
                        && (!@this._checkInTray || p.IsInTray);
            }
        }

        public bool Run()
        {
#if DEBUG_ExecuteSpecificPower
            ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower::Run() starts");
#endif
            Power currentPower = GetCurrentPower();

            if (!ValidatePower(currentPower))
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"{actionIDstr}: Fail to get Power '{@this._powerId}' by 'PowerId'");
#endif
                return false;
            }
            Power entActivatedPower = currentPower.EntGetActivatedPower();
            PowerDef powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();
            if (@this.Target != Unit.Player)
            {
                switch (@this.Target)
                {
                    case Unit.MostInjuredAlly:
                        AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.MostInjuredAlly;
                        break;
                    case Unit.StrongestAdd:
                        AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.AnAdd;
                        break;
                    case Unit.StrongestTeamMember:
                        AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.StrongestTeamMember;

                        break;
                    default:
                        AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.StrongestTeamMember;
                        break;
                }
                int effectiveRange = Powers.getEffectiveRange(powerDef);

                if (@this.Range > 0)
                    effectiveRange = @this.Range;

                if (effectiveRange > 1)
                {
                    if (effectiveRange < 7)
                    {
                        effectiveRange = 7;
                    }

                    AstralAccessors.Logic.UCC.Controllers.Movements.RequireRange = effectiveRange - 2;

                    while (AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                    {
                        if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                        {
                            return true;
                        }
                        Thread.Sleep(100);
                        //if (Spell.stopMoveDodgeTO.IsTimedOut && AOECheck.PlayerIsInAOE)
                        //{
                        //    Spell.stopMoveDodgeTO.ChangeTime(3500);
                        //    return true;
                        //}
                    }
                }
            }
            int castingTime;
            if (@this.CastingTime > 0)
            {
                castingTime = @this.CastingTime;
            }
            else
            {
                castingTime = Powers.getEffectiveTimeCharge(powerDef);
            }
            Entity target = new Entity(UnitRef.Pointer);
            if (target.ContainerId != EntityManager.LocalPlayer.ContainerId && !target.Location.IsInYawFace)
            {
                target.Location.Face();
                Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(750);
                while (!target.Location.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }
            double effectiveTimeActivate = (double)Powers.getEffectiveTimeActivate(powerDef) * 1.5;
            Astral.Classes.Timeout castingTimeout = new Astral.Classes.Timeout(castingTime);

            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.Ignorepitch))
                {
                    target.Location.Face();
#if DEBUG_ExecuteSpecificPower
                    ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Activate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                    Powers.ExecPower(currentPower, target, true);
                }
                else
                {
                    Vector3 location = target.Location;
                    location.Z += 3f;
                    location.Face();
#if DEBUG_ExecuteSpecificPower
                    ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Activate ExecPower '{currentPower.PowerDef.InternalName}' on location <{location.X.ToString("0,4:N2")}, {location.Y.ToString("0,4:N2")}, {location.Z.ToString("0,4:N2")}>");
#endif
                    Powers.ExecPower(currentPower, location, true);
                }
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Wait casting time ({castingTime} ms)");
#endif
                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                    {
                        return true;
                    }
                    if (!@this._forceMaintain && ((currentPower.UseCharges() && !currentPower.ChargeAvailable()) || currentPower.IsActive))
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception e)
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Catch an exception trying activate power '{currentPower.PowerDef.InternalName}' \n\r{e.Message}");
#endif
            }
            finally
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                try
                {
                    Powers.ExecPower(currentPower, target, false);
                }
                catch (Exception e)
                {
#if DEBUG_ExecuteSpecificPower
                    ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Catch an exception trying deactivate power '{currentPower.PowerDef.InternalName}'\n\r {e.Message}");
#endif
                }
            }
            if (!@this._forceMaintain)
            {
                Powers.WaitPowerActivation(currentPower, (int)effectiveTimeActivate);
            }
            return true;
        }

        public Entity UnitRef
        {
            get
            {
                if (!string.IsNullOrEmpty(@this._entityId))
                {
                    if (checkEntity == null)
                        checkEntity = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                    if (ValidateEntity(entity))
                        return entity;
                    else entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                 @this._healthCheck, @this._reactionRange,
                                                                 (@this._reactionZRange > 0) ? @this._reactionZRange : Astral.Controllers.Settings.Get.MaxElevationDifference,
                                                                 @this._regionCheck, null, @this._aura.Checker);

                    if (ValidateEntity(entity))
                        return entity;
                }
                switch (@this.Target)
                {
                    case Unit.Player:
                        return EntityManager.LocalPlayer;
                    case Unit.MostInjuredAlly:
                        return ActionsPlayer.MostInjuredAlly;
                    case Unit.StrongestAdd:
                        return ActionsPlayer.AnAdd;
                    case Unit.StrongestTeamMember:
                        return ActionsPlayer.StrongestTeamMember;
                    default:
                        return Astral.Logic.UCC.Core.CurrentTarget;
                }
            }
        }

        public string Label()
        {
            if (!string.IsNullOrEmpty(@this._powerId) && string.IsNullOrEmpty(label))
            {
                Power currentPower = GetCurrentPower();

                if (ValidatePower(currentPower))
                {
                    PowerDef powDef = currentPower.EffectivePowerDef();
                    if (powDef != null && powDef.IsValid)
                        label = string.Concat((@this._checkInTray && (slotedState = СurrentPowerIsSlotted)) ? "[Slotted] " : string.Empty,
                            (powDef.DisplayName.Length > 0) ? powDef.DisplayName : powDef.InternalName);
                }
                else
                {
                    var powerDefByPowerId = Powers.GetPowerDefByPowerId(@this._powerId);
                    if (powerDefByPowerId.IsValid)
                    {
                        label = string.Concat(powerDefByPowerId.DisplayName, " (Unknown Power)");
                    }
                }
                if (string.IsNullOrEmpty(label))
                    label = "Unknow Power";
            }

            return label;
        }
        #endregion

        #region Вспомогательные функции
        [XmlIgnore]
        [Browsable(false)]
        private bool СurrentPowerIsSlotted => power?.IsInTray == true;
        private bool ValidatePower(Power p)
        {
            return attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                && p != null 
                && (p.PowerDef.InternalName == @this._powerId 
                    || p.EffectivePowerDef().InternalName == @this._powerId);
        }
        private bool ValidateEntity(Entity e)
        {
            return e != null && e.IsValid && checkEntity?.Invoke(e) == true;
        }
        private Power GetCurrentPower()
        {
            if (!ValidatePower(power))
            {
                attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                power = Powers.GetPowerByInternalName(@this._powerId);
            }
            return power;
        }
        #endregion


        public bool EntityDiagnosticString(out string infoString)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("EntityID: ").AppendLine(@this._entityId);
            sb.Append("EntityIdType: ").AppendLine(@this._entityIdType.ToString());
            sb.Append("EntityNameType: ").AppendLine(@this._entityNameType.ToString());
            //sb.Append("EntitySetType: ").AppendLine(@this._entitySetType.ToString());
            sb.Append("HealthCheck: ").AppendLine(@this._healthCheck.ToString());
            sb.Append("ReactionRange: ").AppendLine(@this._reactionRange.ToString());
            sb.Append("ReactionZRange: ").AppendLine(@this._reactionZRange.ToString());
            sb.Append("RegionCheck: ").AppendLine(@this._regionCheck.ToString());
            sb.Append("Aura: ").AppendLine(@this._aura.ToString());
            //if (@this._customRegionNames != null && @this._customRegionNames.Count > 0)
            //{
            //    sb.Append("RegionCheck: {").Append(@this._customRegionNames[0]);
            //    for (int i = 1; i < @this._customRegionNames.Count; i++)
            //        sb.Append(", ").Append(@this._customRegionNames[i]);
            //    sb.AppendLine("}");
            //}
            sb.AppendLine();
            //sb.Append("NeedToRun: ").AppendLine(NeedToRun.ToString());
            //sb.AppendLine();

            // список всех Entity, удовлетворяющих условиям
            LinkedList<Entity> entities = SearchCached.FindAllEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                                     @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);


            // Количество Entity, удовлетворяющих условиям
            if (entities != null)
                sb.Append("Founded Entities: ").AppendLine(entities.Count.ToString());
            else sb.Append("Founded Entities: 0");
            sb.AppendLine();

            // Ближайшее Entity (найдено при вызове ie.NeedToRun, поэтому строка ниже закомментирована)
            entity = SearchCached.FindClosestEntity(@this._entityId, @this._entityIdType, @this._entityNameType, EntitySetType.Complete,
                                                    @this._healthCheck, @this._reactionRange, @this._reactionZRange, @this._regionCheck);
            if (entity != null)
            {
                bool distOk = entity.Location.Distance3DFromPlayer < @this._reactionRange;
                bool zOk = @this._reactionZRange <= 0 || Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location) < @this._reactionZRange;
                bool alive = !@this._healthCheck || !entity.IsDead;
                sb.Append("ClosestEntity: ").Append(entity.ToString());
                if (distOk && zOk && alive)
                    sb.AppendLine(" [MATCH]");
                else sb.AppendLine(" [MISMATCH]");
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").Append(entity.IsDead.ToString());
                if (alive)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]"); sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").Append(entity.Location.Distance3DFromPlayer.ToString());
                if (distOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
                sb.Append("\tZAxisDiff: ").Append(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
                if (zOk)
                    sb.AppendLine(" [OK]");
                else sb.AppendLine(" [FAIL]");
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }

        /// <summary>
        /// Метод, инициализирующий функтор <see cref="checkEntity"/>,
        /// использующийся для проверки сущности <paramref name="e"/> на соответствия идентификатору <see cref="EntityID"/>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool initialize_CheckEntity(Entity e)
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
    }
}
