#if DEBUG
#define DEBUG_ExecuteSpecificPower
#endif

#define SMART_REFLECTION_ACCESS

using AcTp0Tools;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes;
using EntityTools.UCC.Conditions;
using Timeout = Astral.Classes.Timeout;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityCore.UCC.Actions
{
    public class ExecuteSpecificPowerEngine : IUccActionEngine
    {
        #region Данные
        private ExecuteSpecificPower @this;

        //private Entity entity;
        private string label = string.Empty;
        private string _idStr;

        private int   attachedGameProcessId;
        private uint  characterContainerId;
        private uint  powerId;
        private Power power;
        #endregion

        internal ExecuteSpecificPowerEngine(ExecuteSpecificPower esp)
        {
            InternalRebase(esp); 
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~ExecuteSpecificPowerEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
                @this = null;
            }
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
    #if false
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
    #elif EntityTarget
                string prName = e.PropertyName;
                if (prName == nameof(@this.EntityID) || prName == nameof(@this.EntityIdType) || prName == nameof(@this.EntityNameType))
                    _key = null;
                else if (prName == nameof(@this.PowerId) || prName == nameof(@this.CheckInTray))
                {
                    power = null;
                    label = string.Empty;
                }
    #endif
                //entity = null;
                power = null;
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
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
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
                @this.Engine = null;//new EntityTools.Core.Proxies.UccActionProxy(@this);
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

#if EntityTarget
            _key = null; 
#endif

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                var p = GetCurrentPower();

                if (p is null)
                    return false;

                if (@this._checkPowerCooldown && p.IsOnCooldown()
                    || @this._checkInTray && !p.IsInTray)
                    return false;

                return ((ICustomUCCCondition)@this._customConditions).IsOK(@this);
            }
        }

        public bool Run()
        {
#if DEBUG_ExecuteSpecificPower
            ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower::Run() starts");
#endif
            var currentPower = GetCurrentPower();

            if (currentPower is null)
            {
#if DEBUG_ExecuteSpecificPower
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Fail to get Power '{@this._powerId}' by 'PowerId'");
#endif
                return false;
            }
            var entActivatedPower = currentPower.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();
            // Устанавливаем цель для перемещения персонажа к ней
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
                        AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = null;//ActionsPlayer.StrongestTeamMember;
                        break;
                }

                // Вычисляем эффективный радиус действия команды
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

                    // Пытаемся приблизиться к цели
                    // Запуск Astral.Logic.UCC.Controllers.Movements.Start()
                    // выполняется перед вызовом метода Run() текущей команды в 
                    // Astral.Logic.UCC.Classes.ActionsPlayer.playActionList()
                    var movingTimeout = new Timeout(1050);
                    while (!AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                    {
                        if (Astral.Logic.UCC.Core.CurrentTarget.IsDead || movingTimeout.IsTimedOut)
                        {
                            // Завершаем команду, если цель мертва, или попытка приблизиться к ней неудачна
                            return true;
                        }
                        Thread.Sleep(100);
                    }
                }
            }
            
            int castingTime = @this.CastingTime > 0 
                ? @this.CastingTime 
                : Powers.getEffectiveTimeCharge(powerDef);

            Entity target = new Entity(UnitRef.Pointer);
            if (target.ContainerId != EntityManager.LocalPlayer.ContainerId && !target.Location.IsInYawFace)
            {
                target.Location.Face();
                var timeout = new Timeout(750);
                while (!target.Location.IsInYawFace && !timeout.IsTimedOut)
                {
                    Thread.Sleep(20);
                }
                Thread.Sleep(100);
            }

            double effectiveTimeActivate = Powers.getEffectiveTimeActivate(powerDef) * 1.5;
            var castingTimeout = new Timeout(castingTime);

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
#if DEBUG_ExecuteSpecificPower
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Debug, $"ExecuteSpecificPower: Catch an exception trying activate power '{currentPower.PowerDef.InternalName}' \n\r{e.Message}");
            }
#endif
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
            if (string.IsNullOrEmpty(@this._powerId))
                label = $"{nameof(@this.PowerId)} not defined";
            else if (string.IsNullOrEmpty(label))
            {
                var currentPower = GetCurrentPower();

                if (currentPower != null)
                {
                    var powDef = currentPower.PowerDef;
                    if (powDef != null && powDef.IsValid)
                        label = string.Concat(@this._checkInTray && CurrentPowerIsSlotted ? "[Slotted] " : string.Empty,
                            string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : $"{powDef.DisplayName} [{powDef.InternalName}]");
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
                    label = "Unknown Power";
            }

            return label;
        }
        #endregion

        #region Вспомогательные функции
        [XmlIgnore]
        [Browsable(false)]
        private bool CurrentPowerIsSlotted => power?.IsInTray == true;

        private Power GetCurrentPower()
        {
            var player = EntityManager.LocalPlayer;
            var processId = Astral.API.AttachedGameProcess.Id;

            if (!(attachedGameProcessId == processId
                  && characterContainerId == player.ContainerId
                  && power != null
                  && (power.PowerId == powerId
                      || string.Equals(power.PowerDef.InternalName, @this._powerId, StringComparison.Ordinal)
                      || string.Equals(power.EffectivePowerDef().InternalName, @this._powerId, StringComparison.Ordinal))))
            {
                power = Powers.GetPowerByInternalName(@this._powerId);
                if (power != null)
                {
                    powerId = power.PowerId;
                    label = string.Empty;
                    attachedGameProcessId = processId;
                    characterContainerId = player.ContainerId;
                }
                else
                {
                    powerId = 0;
                    label = string.Empty;
                    attachedGameProcessId = 0;
                    characterContainerId = 0;
                }
            }
            return power;
        }
        #endregion


#if EntityTarget
        /// <summary>
        /// Комплексный (составной) идентификатор, используемый для поиска <see cref="Entity"/> в кэше
        /// </summary>
        public EntityCacheRecordKey EntityKey =>
            _key ?? (_key = new EntityCacheRecordKey(@this._entityId, @this._entityIdType,
                @this._entityNameType, EntitySetType.Complete));

        private EntityCacheRecordKey _key; 
#endif
    }
}
