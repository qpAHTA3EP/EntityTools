using Astral.Logic.NW;
using Astral.Logic.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using EntityTools.Enums;
using EntityTools.Tools.Powers;
using Unit = Astral.Logic.UCC.Ressources.Enums.Unit;

namespace EntityCore.UCC.Actions
{
    public class ExecuteSpecificPowerEngine : IUccActionEngine
    {
        #region Данные
        private ExecuteSpecificPower @this;
#if EntityTarget
        private Entity entity; 
#endif
        private string label = string.Empty;
        private string _idStr;
        private readonly PowerCache powerCache = new PowerCache(string.Empty); 
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

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                entity = null;
#endif
                label = string.Empty;
                powerCache.PowerIdPattern = @this.PowerId; 
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

            string debugStr =
                $"Rebase failed. {action.GetType().Name}[{action.GetHashCode():X2}] can't be cast to '{nameof(ExecuteSpecificPower)}'";
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

            powerCache.PowerIdPattern = @this.PowerId; 
            @this.Engine = this;

            return true;
        }

        #region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                bool debugInfo = EntityTools.EntityTools.Config.Logger.UccActions.DebugExecuteSpecificPower;

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

                if (@this.CheckInTray && !pwr.IsInTray)
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Power is not Slotted.");
                    return false;
                }

                if (@this.CheckPowerCooldown && pwr.IsOnCooldown())
                {
                    if (debugInfo)
                        ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Power is on Cooldown.");
                    return false;
                }

#if CUSTOM_UCC_CONDITION_EDITOR
                if (debugInfo)
                {
                    var conditions = @this.CustomConditions.Conditions;
                    var sb = new StringBuilder();
                    bool result = true;
                    if (conditions.Count > 0)
                    {
                        if (@this.CustomConditions.TestRule == LogicRule.Disjunction)
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
                    sb.Append("Negation flag (Not): ").AppendLine(@this.CustomConditions.Not.ToString());

                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: CustomConditions are {result}:\n{sb}");
                    return result;
                }

                return @this.CustomConditions.IsOK(@this); 
#else
                return true;
#endif
            }
        }

        public bool Run()
        {
            bool debugInfo = EntityTools.EntityTools.Config.Logger.UccActions.DebugExecuteSpecificPower;

            string actionIdStr = debugInfo
                    ? $"{_idStr}.{MethodBase.GetCurrentMethod()?.Name ?? nameof(Run)}"
                    : string.Empty;

            if(debugInfo)
                ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: starts");

            var pwr = powerCache.GetPower(); 

            if (pwr is null)
            {
                if(debugInfo)
                    ETLogger.WriteLine(LogType.Debug, $"{actionIdStr}: Fail to get Power '{@this.PowerId}' by 'PowerId'");

                return false;
            }

            var targetEntity = UnitRef;

            var powResult = pwr.ExecutePower(targetEntity, @this.CastingTime, @this.Range, @this.ForceMaintain, debugInfo);

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
            var pwrId = @this.PowerId;
            if (string.IsNullOrEmpty(pwrId))
                label = $"{nameof(@this.PowerId)} not defined";
            else if (string.IsNullOrEmpty(label))
            {
                var pwr = powerCache.GetPower(); 

                if (pwr != null)
                {
                    var powDef = pwr.PowerDef;
                    if (powDef != null && powDef.IsValid)
                        label = string.Concat(@this.CheckInTray && pwr.IsInTray ? "[Slotted] " : string.Empty,
                                              string.IsNullOrEmpty(powDef.DisplayName) ? powDef.InternalName : $"{powDef.DisplayName} [{powDef.InternalName}]");
                }
                else
                {
                    var powerDefByPowerId = Powers.GetPowerDefByPowerId(pwrId);
                    if (powerDefByPowerId.IsValid)
                    {
                        label = string.Concat(powerDefByPowerId.DisplayName, " (Unknown Power)");
                    }
                }
                if (string.IsNullOrEmpty(label))
                    label = $"{pwrId} (Unknown Power)";
            }

            return label;
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
