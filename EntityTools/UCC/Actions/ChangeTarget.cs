#define DEBUG_CHANGE_TARGET

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Infrastructure;
using Infrastructure.Annotations;

using Astral.Logic.UCC.Classes;

using EntityTools.Editors;
using EntityTools.Editors.TestEditors;
using EntityTools.Enums;
using EntityTools.Tools.Entities;
using EntityTools.Tools.Extensions;
using EntityTools.Tools.Navigation;
using EntityTools.Tools.Targeting;
using EntityTools.UCC.Conditions;

using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.UCC.Actions
{
    [Serializable]
    public class ChangeTarget : UCCAction, INotifyPropertyChanged
    {
        #region Опции команды
        [Category("General")]
        [Editor(typeof(UccTargetSelectorEditor), typeof(UITypeEditor))]
        [Description("Target selector algorithm.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlElement(typeof(EntityTarget))]
        [XmlElement(typeof(TeammateSupport))]
        public TargetSelector TargetSelector
        {
            get => targetSelector;
            set
            {
                if (targetSelector != value)
                {
                    targetSelector = value;
                    OnPropertyChanged();
                } 
            }
        }
        private TargetSelector targetSelector = new EntityTarget();

        public new int Range
        {
            get => base.Range;
            set
            {
                if (value < 0)
                    value = 0;
                if (value != base.Range)
                {
                    base.Range = value;
                    OnPropertyChanged();
                }
            }
        }

        [Description("Cooldown on action activation (ms). Minimum is 500 ms")]
        public new int CoolDown
        {
            get => base.CoolDown;
            set
            {
                if (value < 1000)
                    value = 1000;
                if (value != base.CoolDown)
                {
                    base.CoolDown = value;
                    OnPropertyChanged();
                }
            }
        }
        
        [Browsable(false)]
        public UCCConditionPack CustomConditions
        {
            get => null;
            set
            {
                if (value != null)
                    Conditions.Add(value);
            }
        }
        public bool ShouldSerializeCustomConditions() => false;

        [Category("General")]
        [XmlIgnore]
        [Editor(typeof(TargetSelectorTestEditor), typeof(UITypeEditor))]
        public string TestInfos => @"Push button '...' =>";
        #endregion




        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string ActionName { get; set; }
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            switch (propertyName)
            {
                case nameof(TargetSelector):
                    TargetProcessor?.Dispose();
                    _targetProcessor = null;
                    break;
                case nameof(Range):
                    TargetProcessor.SpecialCheck = GetSpecialTeammateCheck();
                    break;
                default:
                    TargetProcessor.Reset();
                    break;
            }

            _idStr = string.Concat(GetType().Name, '[', GetHashCode().ToString("X2"), ']');
        }
        #endregion


        public override UCCAction Clone()
        {
            var tarClone = targetSelector.Clone();
            return BaseClone(new ChangeTarget { targetSelector = tarClone });
        }

        #region Данные
        [XmlIgnore]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object PlayerTarget => new SimpleEntityWrapper(TargetProcessor.GetTarget());

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TargetProcessor TargetProcessor
        {
            get
            {
                if (_targetProcessor is null || _targetProcessor.IsDisposed)
                {
                    switch (TargetSelector)
                    {
                        case EntityTarget entityTarget:
                            _targetProcessor = new EntityTargetProcessor(entityTarget, GetSpecialTeammateCheck());
                            break;
                        case TeammateSupport protectMember:
                            _targetProcessor = new TeammateSupportTargetProcessor(protectMember, GetSpecialTeammateCheck());
                            break;
                        default:
                            var prc = TargetSelector.GetDefaultProcessor(this);
                            if (prc != null)
                                _targetProcessor = prc;
                            else throw new Exception($"Can't realized the processor for the '{TargetSelector.GetType()}'");
                            break;
                    }
                }
                return _targetProcessor;
            }
        }

        [XmlIgnore]
        [NonSerialized]
        private TargetProcessor _targetProcessor;

        private string _idStr;
        #endregion
        


        #region IUCCActionEngine
        public override bool NeedToRun
        {
            get
            {
                var target = Astral.Logic.UCC.Core.CurrentTarget;
                bool extendedDebugInfo = ExtendedDebugInfo;
                if (extendedDebugInfo)
                {
                    string currentMethodName = string.Concat(_idStr, '.', MethodBase.GetCurrentMethod()?.Name ?? nameof(NeedToRun));

#if CUSTOM_UCC_CONDITION_EDITOR
                    bool customConditionOk = ((ICustomUCCCondition)CustomConditions).IsOK(@this);
                    if (!customConditionOk)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": CustomConditions check failed. Skip... "), true);
                        return false;
                    } 
#endif

                    var targetStr = target is null || !target.IsValid
                        ? "Target[NULL]"
                        : target.GetDebugString(EntityNameType.InternalName, "Target", EntityDetail.Alive | EntityDetail.Pointer | EntityDetail.Distance);
                    bool isMatch = TargetProcessor.IsMatch(target);
                    if (isMatch)
                    {
                        ETLogger.WriteLine(LogType.Debug, string.Concat(currentMethodName, ": ", targetStr, " is MATCH. Skip... "), true);
                        return false;
                    }

                    var newTarget = TargetProcessor.GetTarget();
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
                return ((ICustomUCCCondition)CustomConditions).IsOK(@this) &&
                               TargetProcessor.IsTargetMismatchedAndCanBeChanged(target); 
#else
                return TargetProcessor.IsTargetMismatchedAndCanBeChanged(target);
#endif
            }
        }

        public override bool Run()
        {
            var target = TargetProcessor.GetTarget();

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

        [XmlIgnore]
        public Entity UnitRef => TargetProcessor.GetTarget() ?? Astral.Logic.UCC.Core.CurrentTarget;

        public override string ToString() => TargetProcessor.Label();

        #endregion

        #region Вспомогательные инструменты
        /// <summary>
        /// Флаг настроек вывода расширенной отлаточной информации
        /// </summary>
        private bool ExtendedDebugInfo
        {
            get
            {
                var logConf = EntityTools.Config.Logger;
                return logConf.UccActions.DebugChangeTarget && logConf.Active;
            }
        }

        private Predicate<Entity> GetSpecialTeammateCheck()
        {
            Predicate<Entity> specialCheck = null;

            var range = Range;
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
