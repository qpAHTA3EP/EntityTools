using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools.Powers;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using PowerResult = EntityTools.Tools.Powers.PowerResult;

namespace EntityTools.UCC.Actions.Engines
{
    public class PluggedSkillEngine : IUccActionEngine
    {
        #region Данные
        private PluggedSkill @this;

        private string _label = string.Empty;
        private string _idStr;

        private int   attachedGameProcessId;
        private uint  characterContainerId;
        private uint  powerId;
        private string powerName;
        private bool isIgnoredPower;
        private Power _power;
        #endregion

        internal PluggedSkillEngine(PluggedSkill artPow)
        {
            InternalRebase(artPow); 
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~PluggedSkillEngine()
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

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ReferenceEquals(sender, @this))
            {
                _power = null;
                _label = string.Empty;
                _targetSelector = null;
                //_specificTargetCache = UccTarget.Auto;
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
            if (action is PluggedSkill artPow)
            {
                if (InternalRebase(artPow))
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

        private bool InternalRebase(PluggedSkill artPow)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = new global::EntityTools.Core.Proxies.UccActionProxy(@this);
            }

            @this = artPow;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');
            _power = null;
            _label = string.Empty;
            _targetSelector = null;
            //_specificTargetCache = UccTarget.Auto;

            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                var currentPower = GetCurrentPower();
                if( currentPower is null)
                    return false;

                //Для умений, целью которых является персонаж, проверку наличия противников вблизи персонажа
                var targetMain = currentPower.PowerDef.TargetMain;
                if (targetMain.Self && Combats.MobCountAround() == 0)
                    return false;

#if CUSTOM_UCC_CONDITION_EDITOR
                bool customConditionOk = ((ICustomUCCCondition)@this._customConditions).IsOK(@this);
                if (!customConditionOk)
                    return false; 
#endif

                return !currentPower.IsOnCooldown();
            }
        }

        public bool Run()
        {
            bool extendedDebugInfo = ExtendedDebugInfo;

            string actionIdStr = extendedDebugInfo
                ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                : string.Empty;

            if (extendedDebugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: starts");

            var currentPower = GetCurrentPower();

            if (currentPower is null)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Fail to get Artifact's power");

                return false;
            }

            var targetEntity = UnitRef;
            if (targetEntity is null || !targetEntity.IsValid || targetEntity.IsDead)
                return true;

#if true
            var powResult = currentPower.ExecutePower(targetEntity, 0, @this.Range, false, extendedDebugInfo);

            switch (powResult)
            {
                case PowerResult.Succeed:
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return true;
                default:
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return false;
            }
#else
            var entActivatedPower = currentPower.EntGetActivatedPower();
            var powerDef = entActivatedPower.EntGetActivatedPower().EffectivePowerDef();

            var player = EntityManager.LocalPlayer;

            if (target.ContainerId != player.ContainerId)
            {
                AstralAccessors.Logic.UCC.Controllers.Movements.SpecificTarget = target;

                int effectiveRange = @this.Range;
                if (effectiveRange <= 5)
                    effectiveRange = Math.Max(Powers.getEffectiveRange(powerDef), 5);

                AstralAccessors.Logic.UCC.Controllers.Movements.RequireRange = effectiveRange;

                while (!AstralAccessors.Logic.UCC.Controllers.Movements.RangeIsOk)
                {
                    if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                    {
                        return true;
                    }
                    Thread.Sleep(100);
                }
            }

#if CastingTime
            int castingTime = @this.CastingTime;
            if (castingTime <= 0)
            {
                castingTime = Powers.getEffectiveTimeCharge(powerDef);
            } 
#else
            int castingTime = Math.Max(Powers.getEffectiveTimeCharge(powerDef), 1000);
#endif

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

            Astral.Classes.Timeout castingTimeout = new Astral.Classes.Timeout(castingTime);

            try
            {
                if (!powerDef.GroundTargeted && !powerDef.Categories.Contains(PowerCategory.Ignorepitch))
                {
                    target.Location.Face();

                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Activate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");

                    Powers.ExecPower(currentPower, target, true);
                }
                else
                {
                    var location = target.Location;
                    location.Z += 3f;
                    location.Face();
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Activate ExecPower '{currentPower.PowerDef.InternalName}' on location <{location.X:0,4:N2}, {location.Y:0,4:N2}, {location.Z:0,4:N2}>");

                    Powers.ExecPower(currentPower, location, true);
                }
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Wait casting time ({castingTime} ms)");

                while (!castingTimeout.IsTimedOut && !Astral.Controllers.AOECheck.PlayerIsInAOE)
                {
                    if (Astral.Logic.UCC.Core.CurrentTarget.IsDead)
                    {
                        return true;
                    }
                    if (currentPower.UseCharges() && !currentPower.ChargeAvailable() || currentPower.IsActive)
                    {
                        break;
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception e)
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Catch an exception trying activate power '{currentPower.PowerDef.InternalName}'\n{e.Message}");
            }
            finally
            {
                if (extendedDebugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Deactivate ExecPower '{currentPower.PowerDef.InternalName}' on target {target.Name}[{target.InternalName}]");
                try
                {
                    Powers.ExecPower(currentPower, target, false);
                }
                catch (Exception e)
                {
                    if (extendedDebugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Catch an exception trying deactivate power '{currentPower.PowerDef.InternalName}'\n{e.Message}");
                }
            }
            //if (!@this._forceMaintain)
            {
                double effectiveTimeActivate = Powers.getEffectiveTimeActivate(powerDef) * 1.5;
                Powers.WaitPowerActivation(currentPower, (int)effectiveTimeActivate);
            }
            return true; 
#endif
        }

        /// <summary>
        /// Выбор <see cref="Entity"/>, соответствующего дополнительному умению
        /// </summary>
        public Entity UnitRef
        {
            get
            {
                if (_targetSelector is null)
                {
                    var pwr = GetCurrentPower();

                    var targetMain = pwr?.EffectivePowerDef()?.TargetMain;

                    if (targetMain is null || !targetMain.IsValid)
                        _targetSelector = () => Astral.Logic.UCC.Core.CurrentTarget;
                    else if (targetMain.Self)
                        _targetSelector = () => EntityManager.LocalPlayer;
                    else if (targetMain.AffectFoe)
                        _targetSelector = () => ActionsPlayer.AnAdd;
                    else if (targetMain.AffectFriend)
                        _targetSelector = () => ActionsPlayer.StrongestTeamMember;
                    else _targetSelector = () => Astral.Logic.UCC.Core.CurrentTarget;
                }

                return _targetSelector();
            }
        }
        private Func<Entity> _targetSelector;
        //private UccTarget _specificTargetCache = UccTarget.Auto;

        /// <summary>
        /// Текстовая метка, соответствующая <see cref="PluggedSkill"/> и отображаемая в GUI
        /// </summary>
        /// <returns></returns>
        public string Label()
        {
            if (string.IsNullOrEmpty(_label))
            {
                var currentPower = GetCurrentPower();

                if (currentPower != null)
                {
                    PowerDef powDef = currentPower.PowerDef;
                    if (powDef != null && powDef.IsValid)
                        _label = string.Concat(@this._source, "Power : ",
                            string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : powDef.DisplayName);
                }
                if (string.IsNullOrEmpty(_label))
                    _label = @this.GetType().Name + " : Indefinite";
            }

            return _label;
        }
        #endregion

        #region Вспомогательные инструменты
        /// <summary>
        /// Метод, проверяющий актуальность кэшированного умения <see cref="_power"/> или выполняющий поиск соответствующего
        /// </summary>
        /// <returns></returns>
        private Power GetCurrentPower()
        {
            var player = EntityManager.LocalPlayer;
            if (player.IsValid)
            {
                // проверяем валидность кэша
                if (attachedGameProcessId == Astral.API.AttachedGameProcess.Id
                       && characterContainerId == player.ContainerId
                       && _power != null
                       && _power.IsValid
                       && _power.PowerId == powerId
                       && string.Equals(_power.PowerDef.InternalName, powerName, StringComparison.Ordinal))
                    goto result;

                // Кэш не валиден и требует обновления
                var isArtifactPower = @this._source == PluggedSkillSource.Artifact;
                var bagSlots = player.GetInventoryBagById(isArtifactPower
                    ? InvBagIDs.ArtifactPrimary
                    : InvBagIDs.MountEquippedActivePower).Slots;
                // Проверка наличия предмета/маунта в слоте
                if (bagSlots.Count > 0)
                {
                    var itemSlot = bagSlots[0];

                    if (itemSlot.Filled)
                    {
                        var item = itemSlot.Item;
                        if (item != null)
                        {
                            var itemInternalName = item.ItemDef.InternalName;
                            // В бою не применяются Артефактные умения:
                            // - "Каталог "Аврора для всех миров"
                            // - "Молот Гонда"
                            isIgnoredPower = isArtifactPower
                                             && (itemInternalName.StartsWith("Artifact_Auroraswholerealmscatalogue",
                                                     StringComparison.Ordinal)
                                                 || itemInternalName.StartsWith("Artifact_Forgehammer_Of_Gond",
                                                     StringComparison.Ordinal));

                            _power = item.Powers.FirstOrDefault();

                            if (_power != null)
                            {
                                powerId = _power.PowerId;
                                powerName = itemInternalName;
                                _label = string.Empty;
                                attachedGameProcessId = Astral.API.AttachedGameProcess.Id;
                                characterContainerId = player.ContainerId;
                                goto result;
                            }
                        }
                    }
                }
            }
            powerId = 0;
            powerName = string.Empty;
            _label = string.Empty;
            attachedGameProcessId = 0;
            characterContainerId = 0;
            _power = null;

            result:
            return isIgnoredPower ? null : _power;
        }


        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = global::EntityTools.EntityTools.Config.Logger;
                return logConf.UccActions.DebugChangeTarget && logConf.Active;
            }
        }
        #endregion
    }
}
