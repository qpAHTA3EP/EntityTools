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
    public abstract partial class PowerExecutionEngine<TUccAction> : IUccActionEngine
    {

    }
    public abstract partial class PowerExecutionEngine<TUccAction> where TUccAction : UCCAction, INotifyPropertyChanged
    {
        #region Данные
        private TUccAction @this;

        private string _label = string.Empty;
        private string _idStr;

        private int   attachedGameProcessId;
        private uint  characterContainerId;
        private uint  powerId;
        private Power _power;
        #endregion

        internal PowerExecutionEngine(ExecutePowerSmart esp)
        {
            InternalRebase(esp); 
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~PowerExecutionEngine()
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

            var target = UnitRef;
            if (target is null || !target.IsValid || target.IsDead)
                return true;

            var player = EntityManager.LocalPlayer;

            if(target.ContainerId != player.ContainerId)
            {
                AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = target;

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
            
            if (target.ContainerId != player.ContainerId && !target.Location.IsInYawFace)
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
        abstract public Entity UnitRef { get; }


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
        protected abstract bool ValidatePower(Power p);

        /// <summary>
        /// Метод, проверяющий актуальность кэшированного умения <see cref="_power"/> или выполняющий поиск соответствующего
        /// </summary>
        /// <returns></returns>
        protected abstract Power GetCurrentPower();

        #endregion
    }
}
