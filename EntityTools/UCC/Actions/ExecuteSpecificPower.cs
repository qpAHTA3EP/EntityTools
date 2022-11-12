using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;

using EntityTools.Editors;
using EntityTools.Tools.Powers;
using EntityTools.UCC.Conditions;

using MyNW.Classes;
using MyNW.Internals;

using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ExecuteSpecificPower : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Editor(typeof(PowerIdEditor), typeof(UITypeEditor))]
        [Category("Power")]
#else
        [Browsable(false)]
#endif
        public string PowerId
        {
            get => _powerId;
            set
            {
                if (_powerId != value)
                {
                    _powerId = value;
                    _label = string.Empty;
                    powerCache.PowerIdPattern = _powerId;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _powerId = string.Empty;

#if DEVELOPER
        [Category("Power")]
        [DisplayName("CastingTime (ms)")]
#else
        [Browsable(false)]
#endif
        public int CastingTime
        {
            get => _castingTime;
            set
            {
                if (_castingTime != value)
                {
                    _castingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _castingTime;

#if DEVELOPER
        [Category("Power")]
#else
        [Browsable(false)]
#endif
        public bool ForceMaintain
        {
            get => _forceMaintain;
            set
            {
                if (_forceMaintain != value)
                {
                    _forceMaintain = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _forceMaintain;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckPowerCooldown
        {
            get => _checkPowerCooldown;
            set
            {
                if (_checkPowerCooldown != value)
                {
                    _checkPowerCooldown = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _checkPowerCooldown;

#if DEVELOPER
        [Category("Optional")]
#else
        [Browsable(false)]
#endif
        public bool CheckInTray
        {
            get => _checkInTray;
            set
            {
                if (_checkInTray != value)
                {
                    _checkInTray = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _checkInTray;
        
        /// <summary>
        /// Объявление <see cref="CustomConditions"/> для обратной совместимости.
        /// Старый список условий объединен со встроенным <see cref="UCCAction.Conditions"/>.
        /// </summary>
        [Browsable(false)]
        public UCCConditionPack CustomConditions
        {
            get => null;
            set
            {
                if (value != null)
                {
                    Conditions.Add(value);
                }
            }
        }
        public bool ShouldSerializeCustomConditions() => false;

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        #endregion
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InternalResetOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _label = string.Empty;
            _idStr = $"{GetType().Name}[{GetHashCode():X2}]";
            powerCache.PowerIdPattern = PowerId;
        }
        #endregion




        public override UCCAction Clone()
        {
            return BaseClone(new ExecuteSpecificPower {
                _powerId = _powerId,
                _checkPowerCooldown = _checkPowerCooldown,
                _checkInTray = _checkInTray,
                _castingTime = _castingTime,
                _forceMaintain = _forceMaintain,
            });
        }

        #region Данные
        private string _label = string.Empty;
        private string _idStr;
        private readonly PowerCache powerCache = new PowerCache(string.Empty);
        #endregion
        



        #region IUCCActionEngine
        public override bool NeedToRun
        {
            get
            {
                bool debugInfo = global::EntityTools.EntityTools.Config.Logger.UccActions.DebugExecuteSpecificPower;

                string actionIdStr = debugInfo
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun)}"
                    : string.Empty;

                var pwr = powerCache.GetPower();

                if (pwr is null)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Power does not found.");
                    return false;
                }

                if (CheckInTray && !pwr.IsInTray)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Power is not Slotted.");
                    return false;
                }

                if (CheckPowerCooldown && pwr.IsOnCooldown())
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Power is on Cooldown.");
                    return false;
                }

#if CUSTOM_UCC_CONDITION_EDITOR
                if (debugInfo)
                {
                    var conditions = CustomConditions.Conditions;
                    var sb = new StringBuilder();
                    bool result = true;
                    if (conditions.Count > 0)
                    {
                        if (CustomConditions.TestRule == LogicRule.Disjunction)
                        {
                            int lockedNum = 0;
                            int okUnlockedNum = 0;
                            bool lockedTrue = true;
                            foreach (var cond in conditions)
                            {
                                if (cond.Locked)
                                    sb.Append("\t[L] ");
                                else sb.Append("\t[U] ");
                                sb.Append(cond).Append(" | Result: ");
                                bool ok;
                                if (cond is ICustomUCCCondition iCond)
                                {
                                    ok = iCond.IsOK(@this);
                                    if (iCond.Locked)
                                    {
                                        if (!ok)
                                        {
                                            lockedTrue = false;
                                        }
                                        lockedNum++;
                                    }
                                    else if (ok)
                                        okUnlockedNum++;
                                }
                                else
                                {
                                    ok = cond.IsOK(@this);
                                    if (cond.Locked)
                                    {
                                        if (!ok)
                                        {
                                            lockedTrue = false;
                                        }
                                        lockedNum++;
                                    }
                                    else if (ok)
                                        okUnlockedNum++;
                                }
                                sb.AppendLine(ok.ToString());
                            }
                            result = lockedTrue && (conditions.Count == lockedNum || okUnlockedNum > 0);
                        }
                        else
                        {
                            foreach (UCCCondition cond in conditions)
                            {
                                if (cond.Locked)
                                    sb.Append("\t[L] ");
                                else sb.Append("\t[U] ");
                                sb.Append(cond).Append(" | Result: ");
                                bool ok;
                                if (cond is ICustomUCCCondition iCond)
                                {
                                    ok = iCond.IsOK(@this);
                                    if (!ok)
                                    {
                                        result = false;
                                    }
                                }
                                else
                                {
                                    ok = cond.IsOK(@this);
                                    if (!ok)
                                    {
                                        result = false;
                                    }
                                }
                                sb.AppendLine(ok.ToString());
                            }
                        }
                    }
                    sb.Append("Negation flag (Not): ").AppendLine(CustomConditions.Not.ToString());

                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: CustomConditions are {result}:\n{sb}");
                    return result;
                }

                return CustomConditions.IsOK(@this); 
#else
                return true;
#endif
            }
        }

        public override bool Run()
        {
            bool debugInfo = global::EntityTools.EntityTools.Config.Logger.UccActions.DebugExecuteSpecificPower;

            string actionIdStr = debugInfo
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                    : string.Empty;

            if (debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: starts");

            var pwr = powerCache.GetPower();

            if (pwr is null)
            {
                if (debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Fail to get Power '{PowerId}' by 'PowerId'");

                return false;
            }

            var targetEntity = UnitRef;

            var powResult = pwr.ExecutePower(targetEntity, CastingTime, Range, ForceMaintain, debugInfo);

            switch (powResult)
            {
                case PowerResult.Succeed:
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return true;
                default:
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Result => {powResult}");
                    return false;
            }
        }

        public Entity UnitRef
        {
            get
            {
                switch (Target)
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

        public override string ToString()
        {
            var pwrId = PowerId;
            if (string.IsNullOrEmpty(pwrId))
                _label = $"{nameof(PowerId)} not defined";
            else if (string.IsNullOrEmpty(_label))
            {
                var pwr = powerCache.GetPower();

                if (pwr != null)
                {
                    var powDef = pwr.PowerDef;
                    if (powDef != null && powDef.IsValid)
                        _label = string.Concat(CheckInTray && pwr.IsInTray ? "[Slotted] " : string.Empty,
                                              string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : $"{powDef.DisplayName} [{powDef.InternalName}]");
                }
                else
                {
                    var powerDefByPowerId = Powers.GetPowerDefByPowerId(pwrId);
                    if (powerDefByPowerId.IsValid)
                        _label = string.Concat(powerDefByPowerId.DisplayName, " (Unknown Power)");
                }
                if (string.IsNullOrEmpty(_label))
                    _label = $"{pwrId} (Unknown Power)";
            }

            return _label;
        }
        #endregion
    }
}
