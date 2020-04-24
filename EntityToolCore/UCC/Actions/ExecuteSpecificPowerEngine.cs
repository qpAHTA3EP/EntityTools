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
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityCore.UCC.Actions
{
    public class ExecuteSpecificPowerEngine : IEntityInfos
#if CORE_INTERFACES
                 , IUCCActionEngine
#endif
    {
        #region Данные
        private ExecuteSpecificPower @this;

        private Predicate<Entity> checkEntity { get; set; } = null;
        private Entity entity = null;
        private bool slotedState = false;
        private string label = string.Empty;

        private int attachedGameProcessId = 0;
        private Power power = null;
#if REFLECTION_ACCESS
        private static Type movementsType = null;
#elif SMART_REFLECTION_ACCESS
        private static readonly StaticPropertyAccessor<Entity> movementSpecificTarget = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<Entity>("SpecificTarget");
        private static readonly StaticPropertyAccessor<int> movementRequireRange = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<int>("RequireRange");
        private static readonly StaticPropertyAccessor<bool> movementRangeIsOk = typeof(Astral.Logic.UCC.Controllers.Movements).GetStaticProperty<bool>("RangeIsOk");
        #endif

        #endregion

        internal ExecuteSpecificPowerEngine(ExecuteSpecificPower esp)
        {
            @this = esp;
#if CORE_INTERFACES
            @this.Engine = this;
#endif
            @this.PropertyChanged += PropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(object.ReferenceEquals(sender, @this))
            {
                if (object.ReferenceEquals(sender, @this))
                {
                    switch (e.PropertyName)
                    {
                        case "EntityID":
                            checkEntity = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                            label = string.Empty;
                            break;
                        case "EntityIdType":
                            checkEntity = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                            break;
                        case "EntityNameType":
                            checkEntity = EntityToPatternComparer.Get(@this._entityId, @this._entityIdType, @this._entityNameType);
                            break;
                        case "PowerID":
                            power = null;
                            label = string.Empty;
                            break;
                        case "CheckInTray":
                            label = string.Empty;
                            break;
                    }
                    entity = null;
                    power = null;
                }
            }
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
#if REFLECTION_ACCESS
            if (movementsType == null)
                movementsType = typeof(Astral.Logic.UCC.Controllers.Movements);
#endif

            Power currentPower = GetCurrentPower();

            if (!ValidatePower(currentPower))
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Fail to get Power '{@this._powerId}' by 'PowerId'");
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
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.MostInjuredAlly/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUG_ExecuteSpecificPower
                            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'MostInjuredAlly'");
#endif
                            return false;
                        }
#elif SMART_REFLECTION_ACCESS
                        movementSpecificTarget.Value = ActionsPlayer.MostInjuredAlly;
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.MostInjuredAlly;
#endif
                        break;
                    case Unit.StrongestAdd:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.AnAdd/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUG_ExecuteSpecificPower
                            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'AnAdd'");
#endif
                            return false;
                        }
#elif SMART_REFLECTION_ACCESS
                        movementSpecificTarget.Value = ActionsPlayer.AnAdd;
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.AnAdd;
#endif
                        break;
                    case Unit.StrongestTeamMember:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", ActionsPlayer.StrongestTeamMember/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUG
                            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'StrongestTeamMember'");
#endif
                            return false;
                        }
#elif SMART_REFLECTION_ACCESS
                        movementSpecificTarget.Value = ActionsPlayer.StrongestTeamMember;
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = ActionsPlayer.StrongestTeamMember;
#endif
                        break;
                    default:
#if REFLECTION_ACCESS
                        if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "SpecificTarget", null/*, BindingFlags.Static | BindingFlags.NonPublic*/))
                        {
#if DEBUG_ExecuteSpecificPower
                            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.SpecificTarget to 'null'");
#endif
                            return false;
                        }
#elif SMART_REFLECTION_ACCESS
                        movementSpecificTarget.Value = ActionsPlayer.StrongestTeamMember;
#else
                        Astral.Logic.UCC.Controllers.Movements.SpecificTarget = null;
#endif
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
#if REFLECTION_ACCESS
                    if (!ReflectionHelper.SetStaticPropertyValue(movementsType, "RequireRange", effectiveRange - 2/*, BindingFlags.Static | BindingFlags.SetProperty*/))
                    {
#if DEBUG_ExecuteSpecificPower
                        EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"ExecuteSpecificPower: Fail to set UCC.Controllers.Movements.RequireRange to '{effectiveRange - 2}'");
#endif
                        return false;
                    }
#elif SMART_REFLECTION_ACCESS
                    movementRequireRange.Value = effectiveRange - 2;
#else
                    Astral.Logic.UCC.Controllers.Movements.RequireRange = range - 2;
#endif
#if REFLECTION_ACCESS
                    while (ReflectionHelper.GetPropertyValue(movementsType, "RangeIsOk", out object RangeIsOk) && RangeIsOk.Equals(true))
#elif SMART_REFLECTION_ACCESS
                    while ((bool)movementRangeIsOk)
#else
                    while (Astral.Logic.UCC.Controllers.Movements.RangeIsOk)
#endif
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
            Entity target = new Entity(UnitRef.Pointer);//new Entity(TargetEntity.Pointer);
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
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.IgnorePitch))
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
            catch { }
            finally
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                Powers.ExecPower(currentPower, target, false);
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
            if (string.IsNullOrEmpty(label))
            {
                Power currentPower = GetCurrentPower();

                if (ValidatePower(currentPower))
                {
                    StringBuilder str = new StringBuilder();
                    if (@this._checkInTray && (slotedState = checkIsSlotted))
                        str.Append("[Slotted] ");
                    PowerDef powDef = currentPower.EffectivePowerDef();
                    if (powDef.DisplayName.Length > 0)
                        str.Append(powDef.DisplayName);
                    else str.Append(powDef.InternalName);

                    label = str.ToString();
                }

                label = "Unknow Power";
            }

            return label;
        }
        #endregion

        #region Вспомогательные функции
        [XmlIgnore]
        [Browsable(false)]
        private bool checkIsSlotted => power?.IsInTray == true;
        private bool ValidatePower(Power p)
        {
            return attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                && p != null && p.IsValid && p.PowerDef.InternalName == @this._powerId;
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
                sb.Append("Target: ").AppendLine(entity.ToString());
                sb.Append("\tName: ").AppendLine(entity.Name);
                sb.Append("\tInternalName: ").AppendLine(entity.InternalName);
                sb.Append("\tNameUntranslated: ").AppendLine(entity.NameUntranslated);
                sb.Append("\tIsDead: ").AppendLine(entity.IsDead.ToString());
                sb.Append("\tRegion: '").Append(entity.RegionInternalName).AppendLine("'");
                sb.Append("\tLocation: ").AppendLine(entity.Location.ToString());
                sb.Append("\tDistance: ").AppendLine(entity.Location.Distance3DFromPlayer.ToString());
                sb.Append("\tZAxisDiff: ").AppendLine(Astral.Logic.General.ZAxisDiffFromPlayer(entity.Location).ToString());
            }
            else sb.AppendLine("Closest Entity not found!");

            infoString = sb.ToString();
            return true;
        }
    }
}
