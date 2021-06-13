#define DEBUG_CHANGE_TARGET

using AcTp0Tools;
using AcTp0Tools.Classes.UCC;
using Astral.Logic.UCC.Classes;
using EntityCore.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Actions.TargetSelectors;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Reflection;
using EntityCore.Entities;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;

namespace EntityCore.UCC.Actions
{
    internal class ChangeTargetEngine : IUccActionEngine
    {
        #region Данные
        private ChangeTarget @this;

#if DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object PlayerTarget => new SimpleEntityWrapper(AstralAccessors.Logic.UCC.Core.CurrentTarget);

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public UccTargetProcessor TargetProcessor => _targetProcessor;
#endif
        private UccTargetProcessor _targetProcessor;

        private string _idStr;
#endregion

        internal ChangeTargetEngine(ChangeTarget changeTarget)
        {
            InternalRebase(changeTarget);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~ChangeTargetEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (@this != null)
            {
                @this.PropertyChanged -= OnPropertyChanged;
                @this.Engine = null;
                @this = null;
            }
            _targetProcessor?.Dispose();
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, @this)) return;
            switch (e.PropertyName)
            {
                case nameof(ChangeTarget.TargetSelector):
                    _targetProcessor?.Dispose();
                    switch (@this.targetSelector)
                    {
                        case EntityTarget ettTrgSelector:
                            _targetProcessor = new EntityTargetProcessor(@this, ettTrgSelector);
                            break;
#if false
                        case AssistToLeader assist2LeaderSelector:
                            _targetProcessor = new UccAssistToLeaderTargetProcessor(@this, assist2LeaderSelector);
                            break; 
#endif
                        case TeammateSupport protectMember:
                            _targetProcessor = new TeammateSupportTargetProcessor(@this, protectMember);
                            break;
                        default:
                            var prc = @this.targetSelector.GetDefaultProcessor(@this);
                            if (prc != null)
                                _targetProcessor = prc;
                            else throw new Exception($"Can't realized the processor for the '{@this.targetSelector.GetType()}'");
                            break;
                    }
                    break;
                default:
                    _targetProcessor.Reset();
                    break;
            }
        }

        public bool Rebase(UCCAction action)
        {
            if (action is null)
                return false;
            if (ReferenceEquals(action, @this))
                return true;
            if (action is ChangeTarget changeTarget)
            {
                if (InternalRebase(changeTarget))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }

                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", action.GetType().Name, '[', action.GetHashCode().ToString("X2"), "] can't be cast to '" + nameof(ChangeTarget) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(ChangeTarget changeTarget)
        {
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= OnPropertyChanged;
                @this.Engine = null;
            }
            _targetProcessor?.Dispose();

            @this = changeTarget;
            @this.PropertyChanged += OnPropertyChanged;

            switch (@this.targetSelector)
            {
                case EntityTarget entityTarget:
                    _targetProcessor = new EntityTargetProcessor(@this, entityTarget);
                    break;
                case TeammateSupport teammateSupport:
                    _targetProcessor = new TeammateSupportTargetProcessor(@this, teammateSupport);
                    break;
                default:
                    var prc = @this.targetSelector.GetDefaultProcessor(@this);
                    if (prc != null)
                        _targetProcessor = prc;
                    else throw new Exception($"Can't realized the processor for the '{@this.targetSelector.GetType()}'");
                    break;
            }

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Engine = this;

            return true;
        }

#region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                var target = Astral.Logic.UCC.Core.CurrentTarget;

                bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.UccActions.DebugChangeTarget;
                if (extendedDebugInfo)
                {
                    string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name);

                    bool customConditionOK = ((ICustomUCCCondition)@this._customConditions).IsOK(@this);
                    if (!customConditionOK)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": CustomConditions check failed. Skip... "), true);
                        return false;
                    }

                    var targetStr = target is null || !target.IsValid
                        ? "Target[NULL]"
                        : target.GetDebugString(EntityNameType.InternalName, "Target", EntityDetail.Alive | EntityDetail.Pointer | EntityDetail.Distance);
                    bool isMatch =_targetProcessor.IsMatch(target);
                    if (isMatch)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", targetStr, " is MATCH. Skip... "), true);
                        return false;
                    }

                    var newTarget = _targetProcessor.GetTarget();
                    if (newTarget is null || !newTarget.IsValid)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", targetStr, " MISMATCH and no suitable target found. Skip... "), true);
                        return false;
                    }
                    
                    var newTargetStr = newTarget.GetDebugString(EntityNameType.InternalName, "NewTarget", EntityDetail.Alive | EntityDetail.Pointer | EntityDetail.Distance);
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", targetStr, " MISMATCH. Found ", newTargetStr, ". NeedToRun... "), true);
                    return true;
                }
                
                return ((ICustomUCCCondition)@this._customConditions).IsOK(@this) &&
                       _targetProcessor.IsTargetMismatchedAndCanBeChanged(target);
            }
        }

        public bool Run()
        {
            var target = _targetProcessor.GetTarget();

            bool extendedDebugInfo = EntityTools.EntityTools.Config.Logger.UccActions.DebugChangeTarget;
            if (extendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name);
                if (target != null && target.IsValid)
                {
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ChangeTarget to ", target.GetDebugString(EntityNameType.InternalName, "NewTarget", EntityDetail.Alive | EntityDetail.Pointer | EntityDetail.Distance)), true);
                    //AstralAccessors.Logic.UCC.Core.CurrentTarget = target;
                    AstralAccessors.Logic.UCC.Core.QueryTargetChange(target, _idStr, 1);
                    return true;
                }
                else ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": No suitable target found.Skip..."), true);
            }
            else if (target != null && target.IsValid)
            {
                //AstralAccessors.Logic.UCC.Core.CurrentTarget = target;
                AstralAccessors.Logic.UCC.Core.QueryTargetChange(target, _idStr, 1);
                return true;
            }

            // Если вернуть false, то команда может быть активироваться повторно
            return false;
        }

        public Entity UnitRef => _targetProcessor.GetTarget() ?? Astral.Logic.UCC.Core.CurrentTarget;

        public string Label()
        {
            return _targetProcessor.Label();
        }
#endregion

#region Вспомогательные инструменты
#endregion
    }
}
