//#define DEBUG_CHANGE_TARGET

using ACTP0Tools;
using Astral.Logic.UCC.Classes;
using EntityCore.Entities;
using EntityCore.Enums;
using EntityCore.Extensions;
using EntityCore.Tools.Targeting;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools.Navigation;
using EntityTools.Tools.Targeting;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Reflection;

namespace EntityCore.UCC.Actions
{
    internal class ChangeTargetEngine : IUccActionEngine
    {
        #region Данные
        private ChangeTarget @this;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object PlayerTarget => new SimpleEntityWrapper(_targetProcessor.GetTarget());
#if DEVELOPER && DEBUG_CHANGE_TARGET

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TargetProcessor TargetProcessor => _targetProcessor;
#endif
        private TargetProcessor _targetProcessor;

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
                @this.Unbind();
                @this = null;
            }
            _targetProcessor?.Dispose();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, @this)) return;
            switch (e.PropertyName)
            {
                case nameof(ChangeTarget.TargetSelector):
                    _targetProcessor?.Dispose();
                    var trgSelector = @this.TargetSelector;
                    switch (trgSelector)
                    {
                        case EntityTarget entityTarget:
                            _targetProcessor = new EntityTargetProcessor(entityTarget, GetSpecialTeammateCheck());
                            break;
                        case TeammateSupport protectMember:
                            _targetProcessor = new TeammateSupportTargetProcessor(protectMember, GetSpecialTeammateCheck());
                            break;
                        default:
                            var prc = trgSelector.GetDefaultProcessor(@this);
                            if (prc != null)
                                _targetProcessor = prc;
                            else throw new Exception($"Can't realized the processor for the '{trgSelector.GetType()}'");
                            break;
                    }
                    break;
                case nameof(ChangeTarget.Range):
                {
                    _targetProcessor.SpecialCheck = GetSpecialTeammateCheck();
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
                @this.Unbind();
            }
            _targetProcessor?.Dispose();

            @this = changeTarget;
            @this.PropertyChanged += OnPropertyChanged;

            var trgSelector = @this.TargetSelector;
            switch (trgSelector)
            {
                case EntityTarget entityTarget:
                    _targetProcessor = new EntityTargetProcessor(entityTarget, GetSpecialTeammateCheck());
                    break;
                case TeammateSupport teammateSupport:
                    _targetProcessor = new TeammateSupportTargetProcessor(teammateSupport, GetSpecialTeammateCheck());
                    break;
                default:
                    var prc = trgSelector.GetDefaultProcessor(@this);
                    if (prc != null)
                        _targetProcessor = prc;
                    else throw new Exception($"Can't realized the processor for the '{trgSelector.GetType()}'");
                    break;
            }

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            @this.Bind(this);

            return true;
        }

#region IUCCActionEngine
        public bool NeedToRun
        {
            get
            {
                var target = Astral.Logic.UCC.Core.CurrentTarget;
                bool extendedDebugInfo = ExtendedDebugInfo;
                if (extendedDebugInfo)
                {
                    string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun));

#if CUSTOM_UCC_CONDITION_EDITOR
                    bool customConditionOk = ((ICustomUCCCondition)@this.CustomConditions).IsOK(@this);
                    if (!customConditionOk)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": CustomConditions check failed. Skip... "), true);
                        return false;
                    } 
#endif

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

#if CUSTOM_UCC_CONDITION_EDITOR
                return ((ICustomUCCCondition)@this.CustomConditions).IsOK(@this) &&
                               _targetProcessor.IsTargetMismatchedAndCanBeChanged(target); 
#else
                return _targetProcessor.IsTargetMismatchedAndCanBeChanged(target);
#endif
            }
        }

        public bool Run()
        {
            var target = _targetProcessor.GetTarget();

            bool extendedDebugInfo = ExtendedDebugInfo;
            if (extendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(Run));
                if (target != null && target.IsValid)
                {
                    ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ChangeTarget to ", target.GetDebugString(EntityNameType.InternalName, "NewTarget", EntityDetail.Alive | EntityDetail.Pointer | EntityDetail.Distance)), true);
                    AstralAccessors.Logic.UCC.Core.QueryTargetChange(target, _idStr, 1);
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": No suitable target found.Skip..."), true);
            }
            else if (target != null && target.IsValid)
            {
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
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.EntityTools.Config.Logger;
                return logConf.UccActions.DebugChangeTarget && logConf.Active;
            }
        }

        private Predicate<Entity> GetSpecialTeammateCheck()
        {
            Predicate<Entity> specialCheck = null;

            var range = @this.Range;
            if (range > 0)
            {
                range *= range;
                specialCheck = (ett) =>
                    NavigationHelper.SquareDistance3D(EntityManager.LocalPlayer.Location, ett.Location) < range;
            }

            return specialCheck;
        }
        #endregion
    }
}
