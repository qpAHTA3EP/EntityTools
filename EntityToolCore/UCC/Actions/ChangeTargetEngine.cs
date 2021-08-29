﻿#define DEBUG_CHANGE_TARGET

using AcTp0Tools;
using AcTp0Tools.Classes.Targeting;
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
        private ChangeTarget action;

#if DEVELOPER && DEBUG_CHANGE_TARGET
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object PlayerTarget => new SimpleEntityWrapper(AstralAccessors.Logic.UCC.Core.CurrentTarget);

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
            if (action != null)
            {
                action.PropertyChanged -= OnPropertyChanged;
                action.Engine = null;
                action = null;
            }
            _targetProcessor?.Dispose();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ReferenceEquals(sender, action)) return;
            switch (e.PropertyName)
            {
                case nameof(ChangeTarget.TargetSelector):
                    _targetProcessor?.Dispose();
                    switch (action.targetSelector)
                    {
                        case EntityTarget entityTarget:
                            _targetProcessor = new EntityTargetProcessor(entityTarget, GetSpecialTeammateCheck());
                            break;
                        case TeammateSupport protectMember:
                            _targetProcessor = new TeammateSupportTargetProcessor(protectMember, GetSpecialTeammateCheck());
                            break;
                        default:
                            var prc = action.targetSelector.GetDefaultProcessor(action);
                            if (prc != null)
                                _targetProcessor = prc;
                            else throw new Exception($"Can't realized the processor for the '{action.targetSelector.GetType()}'");
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
            if (ReferenceEquals(action, this.action))
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
            if (action != null)
            {
                action.PropertyChanged -= OnPropertyChanged;
                action.Engine = null;
            }
            _targetProcessor?.Dispose();

            action = changeTarget;
            action.PropertyChanged += OnPropertyChanged;

            switch (action.targetSelector)
            {
                case EntityTarget entityTarget:
                    _targetProcessor = new EntityTargetProcessor(entityTarget, GetSpecialTeammateCheck());
                    break;
                case TeammateSupport teammateSupport:
                    _targetProcessor = new TeammateSupportTargetProcessor(teammateSupport, GetSpecialTeammateCheck());
                    break;
                default:
                    var prc = action.targetSelector.GetDefaultProcessor(action);
                    if (prc != null)
                        _targetProcessor = prc;
                    else throw new Exception($"Can't realized the processor for the '{action.targetSelector.GetType()}'");
                    break;
            }

            _idStr = string.Concat(action.GetType().Name, '[', action.GetHashCode().ToString("X2"), ']');

            action.Engine = this;

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
                    string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name);

                    bool customConditionOK = ((ICustomUCCCondition)action._customConditions).IsOK(action);
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
                
                return ((ICustomUCCCondition)action._customConditions).IsOK(action) &&
                       _targetProcessor.IsTargetMismatchedAndCanBeChanged(target);
            }
        }

        public bool Run()
        {
            var target = _targetProcessor.GetTarget();

            bool extendedDebugInfo = ExtendedDebugInfo;
            if (extendedDebugInfo)
            {
                string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod().Name);
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

            var range = action.Range;
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