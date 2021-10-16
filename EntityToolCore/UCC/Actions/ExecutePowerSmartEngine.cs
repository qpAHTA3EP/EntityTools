#define ExecutePowerSmargDebug

using AcTp0Tools;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.UCC.Actions;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.ComponentModel;
using System.Threading;
using System.Xml.Serialization;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityCore.UCC.Actions
{
    public class ExecutePowerSmartEngine : IUccActionEngine
    {
        #region Данные
        private ExecutePowerSmart @this;

        private string _label = string.Empty;
        private string _idStr;

        private int   attachedGameProcessId;
        private uint  characterContainerId;
        private uint  powerId;
        private Power _power;
        #endregion

        internal ExecutePowerSmartEngine(ExecutePowerSmart esp)
        {
            InternalRebase(esp); 
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~ExecutePowerSmartEngine()
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
                _power = null;
                _powerComparer = null;
                _label = string.Empty;
                _specificTargetCache = UccTarget.Auto;
            }
            else if (sender is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged -= PropertyChanged;
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is ExecutePowerSmart execPower)
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

        private bool InternalRebase(ExecutePowerSmart execPower)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = new EntityTools.Core.Proxies.UccActionProxy(@this);
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');
            _power = null;
            _powerComparer = null;
            _label = string.Empty;
            _specificTargetCache = UccTarget.Auto;

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                var p = GetCurrentPower();

                return ValidatePower(p)
                        && (!@this._checkPowerCooldown || !p.IsOnCooldown())
                        && (!@this._checkInTray || p.IsInTray);
            }
        }

        public bool Run()
        {
#if DEBUG && ExecutePowerSmargDebug
            ETLogger.WriteLine(LogType.Debug, $"{_idStr}:Run() starts");
#endif
            var currentPower = GetCurrentPower();

            if (!ValidatePower(currentPower))
            {
#if DEBUG && ExecutePowerSmargDebug
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Fail to get Power '{@this._powerId}' by 'PowerId'");
#endif
                return false;
            }
            var entActivatedPower = currentPower.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();

            var unitRef = UnitRef;
            if (unitRef is null || !unitRef.IsValid || unitRef.IsDead)
                return true;

            if (@this.SpecificTarget != UccTarget.Player)
            {
                AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = unitRef;

                int effectiveRange = @this.Range;
                if (effectiveRange <= 0)
                    effectiveRange = Powers.getEffectiveRange(powerDef);

                if (effectiveRange > 1)
                {
                    if (effectiveRange < 7)
                    {
                        effectiveRange = 7;
                    }

                    AstralAccessors.Logic.UCC.Controllers.Movements.RequireRange = effectiveRange - 2;

                    while (!AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                    {
                        if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                        {
                            return true;
                        }
                        Thread.Sleep(100);
                    }
                }
            }
            
            int castingTime = @this.CastingTime;
            if (castingTime <= 0)
            {
                castingTime = Powers.getEffectiveTimeCharge(powerDef);
            }

            Entity target = new Entity(unitRef.Pointer);
            
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

            double effectiveTimeActivate = Powers.getEffectiveTimeActivate(powerDef) * 1.5;
            Astral.Classes.Timeout castingTimeout = new Astral.Classes.Timeout(castingTime);

            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.Ignorepitch))
                {
                    target.Location.Face();
#if DEBUG && ExecutePowerSmargDebug
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Activate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                    Powers.ExecPower(currentPower, target, true);
                }
                else
                {
                    var location = target.Location;
                    location.Z += 3f;
                    location.Face();
#if DEBUG && ExecutePowerSmargDebug
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Activate ExecPower '{currentPower.PowerDef.InternalName}' on location <{location.X:0,4:N2}, {location.Y:0,4:N2}, {location.Z:0,4:N2}>");
#endif
                    Powers.ExecPower(currentPower, location, true);
                }
#if DEBUG && ExecutePowerSmargDebug
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Wait casting time ({castingTime} ms)");
#endif
                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                    {
                        return true;
                    }
                    if (!@this._forceMaintain && (currentPower.UseCharges() && !currentPower.ChargeAvailable() || currentPower.IsActive))
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
            }
#if DEBUG && ExecutePowerSmargDebug
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Catch an exception trying activate power '{currentPower.PowerDef.InternalName}'\n{e.Message}");
            }
#endif
            finally
            {
#if DEBUG && ExecutePowerSmargDebug
                ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
#endif
                try
                {
                    Powers.ExecPower(currentPower, target, false);
                }
                catch (Exception e)
                {
#if DEBUG && ExecutePowerSmargDebug
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr}: Catch an exception trying deactivate power '{currentPower.PowerDef.InternalName}'\n{e.Message}");
#endif
                }
            }
            if (!@this._forceMaintain)
            {
                Powers.WaitPowerActivation(currentPower, (int)effectiveTimeActivate);
            }
            return true;
        }

        /// <summary>
        /// Выбор <see cref="Entity"/>, соответствующего <see cref="ExecutePowerSmart.SpecificTarget"/>
        /// </summary>
        public Entity UnitRef
        {
            get
            {
                if (_specificTargetCache == UccTarget.Auto)
                {
                    _specificTargetCache = @this.SpecificTarget;

                    var pwr = GetCurrentPower();

                    var targetMain = pwr.EffectivePowerDef()?.TargetMain;

                    if (targetMain is null || !targetMain.IsValid)
                        return Astral.Logic.UCC.Core.CurrentTarget;

                    if (targetMain.Self)
                        _specificTargetCache = UccTarget.Player;
                    else if (targetMain.AffectFoe)
                        _specificTargetCache = UccTarget.Target;
                    else if (targetMain.AffectFriend)
                        _specificTargetCache = UccTarget.StrongestTeamMember;
                }


                switch (_specificTargetCache)
                {
                    case UccTarget.Player:
                        @this.Target = Unit.Player;
                        return EntityManager.LocalPlayer;
                    case UccTarget.MostInjuredAlly:
                        @this.Target = Unit.MostInjuredAlly;
                        return ActionsPlayer.MostInjuredAlly;
                    case UccTarget.StrongestAdd:
                        @this.Target = Unit.StrongestAdd;
                        return ActionsPlayer.AnAdd;
                    case UccTarget.StrongestTeamMember:
                        @this.Target = Unit.StrongestTeamMember;
                        return ActionsPlayer.StrongestTeamMember;
                    default:
                        return Astral.Logic.UCC.Core.CurrentTarget;
                }
            }
        }
        private UccTarget _specificTargetCache = UccTarget.Auto;

        /// <summary>
        /// Текстовая метка, соответствующая <see cref="@this"/> и отображаемая в GUI
        /// </summary>
        /// <returns></returns>
        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                if (!string.IsNullOrEmpty(@this._powerId))
                {
                    Power currentPower = GetCurrentPower();

                    if (currentPower != null)
                    {
                        PowerDef powDef = currentPower.EffectivePowerDef();
                        if (powDef != null && powDef.IsValid)
                            _label = string.Concat(@this._checkInTray && SpellIsSlotted ? "[Slotted] " : string.Empty,
                                string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : powDef.DisplayName);
                    }
                    else
                    {
                        var powerDefByPowerId = Powers.GetPowerDefByPowerId(@this._powerId);
                        if (powerDefByPowerId.IsValid)
                        {
                            _label = string.Concat(powerDefByPowerId.DisplayName, " (Unknown Power)");
                        }
                    } 
                }
                if (string.IsNullOrEmpty(_label))
                    _label = "Unknown Power";
            }

            return _label;
        }
        #endregion

        #region Вспомогательные функции
        /// <summary>
        /// Свойство-псевдоним для проверки признака нахождения умения в активном слоте (только для Spell и Артефактов. Не работает для )
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        private bool SpellIsSlotted => _power?.IsInTray == true;

        /// <summary>
        /// Проверка 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool ValidatePower(Power p)
        {
            if (!(attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                  && characterContainerId == EntityManager.LocalPlayer.ContainerId
                  && p != null
                  && p.IsValid
                  && p.PowerId == powerId))
                return false;

            return PowerComparer(p);
        }
        /// <summary>
        /// Метод, проверяющий актуальность кэшированного умения <see cref="_power"/> или выполняющий поиск соответствующего
        /// </summary>
        /// <returns></returns>
        private Power GetCurrentPower()
        {
            if (!ValidatePower(_power))
            {
                attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                characterContainerId = EntityManager.LocalPlayer.ContainerId;
                _power = Powers.PlayerPowers.Find(PowerComparer);//Powers.GetPowerByInternalName(@this._powerId);

                powerId = _power?.PowerId ?? 0;
            }
            return _power;
        }

        /// <summary>
        /// Функтор, проверяющий соответствие <see cref="Power"/> заданному идентификатору <see cref="ExecutePowerSmart.PowerId"/>
        /// </summary>
        private Predicate<Power> PowerComparer
        {
            get
            {
                if (_powerComparer is null)
                {
                    if (!string.IsNullOrEmpty(@this._powerId))
                    {
                        var predicate = @this._powerId.GetComparer(@this._idType);

                        if (predicate != null)
                        {
                            return _powerComparer = pwr => predicate(pwr.PowerDef.InternalName) 
                                                        || predicate(pwr.EffectivePowerDef().InternalName);
                        }
                    }

                    _powerComparer = pwr => false;
                }

                return _powerComparer;
            }
        }
        private Predicate<Power> _powerComparer;
        #endregion
    }
}
