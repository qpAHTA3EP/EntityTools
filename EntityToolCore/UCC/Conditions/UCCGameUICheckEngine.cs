using Astral.Logic.UCC.Classes;
using EntityTools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using System;

namespace EntityCore.UCC.Conditions
{
    public class UccGameUiCheckEngine : IUccConditionEngine
    {
        #region Данные
        private UCCGameUICheck @this;

        private UIGen uiGen;
        private string label = string.Empty;
        private string _idStr;
        #endregion

        internal UccGameUiCheckEngine(UCCGameUICheck uiGenCheck)
        {
            InternalRebase(uiGenCheck);
            ETLogger.WriteLine(LogType.Debug, $"{_idStr} initialized: {Label()}");
        }
        ~UccGameUiCheckEngine()
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
                var propName = e.PropertyName;
                if (propName == nameof(@this.Check))
                    uiGenChecker = initialize_uiGenChecker;
                else if (propName == nameof(@this.UiGenProperty))
                    uiGenPropertyValueChecker = initialize_CheckUIGenVarName;
                label = string.Empty;
                uiGen = null;
            }
        }

        public bool Rebase(UCCCondition condition)
        {
            if (condition is null)
                return false;
            if (ReferenceEquals(condition, @this))
                return true;
            if (condition is UCCGameUICheck uiGenCheck)
            {
                if (InternalRebase(uiGenCheck))
                {
                    ETLogger.WriteLine(LogType.Debug, $"{_idStr} reinitialized");
                    return true;
                }
                ETLogger.WriteLine(LogType.Debug, $"{_idStr} rebase failed");
                return false;
            }

            string debugStr = string.Concat("Rebase failed. ", condition.GetType().Name, '[', condition.GetHashCode().ToString("X2"), "] can't be casted to '" + nameof(UCCGameUICheck) + '\'');
            ETLogger.WriteLine(LogType.Error, debugStr);
            throw new InvalidCastException(debugStr);
        }

        private bool InternalRebase(UCCGameUICheck execPower)
        {
            uiGen = null;
            // Убираем привязку к старому условию
            if (@this != null)
            {
                @this.PropertyChanged -= PropertyChanged;
                @this.Engine = null;
            }

            @this = execPower;
            @this.PropertyChanged += PropertyChanged;

            _idStr = string.Concat(@this.GetType().Name, '[', @this.GetHashCode().ToString("X2"), ']');

            label = string.Empty;
            uiGenChecker = initialize_uiGenChecker;
            uiGenPropertyValueChecker = initialize_CheckUIGenVarName;

            @this.Engine = this;

            return true;
        }

        public bool IsOK(UCCAction refAction)
        {
            var genId = @this.UiGenId;
            if (!Validate(uiGen) && !string.IsNullOrEmpty(genId))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name.Equals(genId, StringComparison.Ordinal));
                if (uiGen == null || !uiGen.IsValid)
                    return false;
            }

            return uiGenChecker(uiGen);
        }

        /// <summary>
        /// Инициализация функтора <see cref="uiGenChecker"/>, проверяющего истинность заданного условия
        /// </summary>
        /// <param name="uigen"></param>
        /// <returns></returns>
        private bool initialize_uiGenChecker(UIGen uigen)
        {
            if (uiGenChecker is null)
            {
                Func<UIGen, bool> checker = null;
                switch (@this.Check)
                {
                    case UiGenCheckType.IsVisible:
                        checker = uig => uig.IsVisible;
                        break;
                    case UiGenCheckType.IsHidden:
                        checker = uig => !uig.IsVisible;
                        break;
                    case UiGenCheckType.Property:
                        checker = uig =>
                        {
                            if (uig.IsVisible)
                            {
                                foreach (var uiVar in uig.Vars)
                                    if (uiVar.IsValid && uiVar.Name.Equals(@this.UiGenProperty, StringComparison.Ordinal))
                                    {
                                        return uiGenPropertyValueChecker(uiVar);
                                    }
                            }
                            return false;
                        };
                        break;
                }

                uiGenChecker = checker ?? ((_) => false);
            }

            return uiGenChecker(uigen);
        }
        private Func<UIGen, bool> uiGenChecker;

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
            {
                var genId = @this.UiGenId;
                if (string.IsNullOrEmpty(genId))
                    label = @this.GetType().Name;
                else
                {
                    var genProp = @this.UiGenProperty;
                    if (@this.Check != UiGenCheckType.Property
                        || string.IsNullOrEmpty(genProp))
                    {
                        label = $"{@this.GetType().Name} [{genId}]";
                    }
                    else 
                    {
                        label = $"{@this.GetType().Name} [{genId}.{genProp}]";
                    }
                }
            }

            return label;
        }

        public string TestInfos(UCCAction refAction)
        {
            var genId = @this.UiGenId;
            if (!Validate(uiGen))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == genId);
                if (uiGen == null || !uiGen.IsValid)
                    return $"GUI '{genId}' is not found.";
            }

            switch (@this.Check)
            {
                case UiGenCheckType.IsVisible:
                    if (uiGen.IsVisible)
                        return $"GUI '{genId}' is VISIBLE.";
                    else return $"GUI '{genId}' is HIDDEN.";
                case UiGenCheckType.IsHidden:
                    if (uiGen.IsVisible)
                        return $"GUI '{genId}' is VISIBLE.";
                    else return $"GUI '{genId}' is HIDDEN.";
                case UiGenCheckType.Property:
                    var genPrpt = @this.UiGenProperty;
                    if (uiGen.IsVisible)
                    {
                        foreach (var uiVar in uiGen.Vars)
                            if (uiVar.IsValid && uiVar.Name.Equals(genPrpt, StringComparison.Ordinal))
                            {
                                if (uiGenPropertyValueChecker(uiVar))
                                    return $"The Property '{genId}.{genPrpt}' equals to '{uiVar.Value}'.";
                                else return $"The Property '{genId}.{genPrpt}' equals to '{uiVar.Value}'.";
                            }
                        return $"The Property '@this.{genId}.{genPrpt}' does not founded.";
                    }
                    else return $"GUI '{genId}' is HIDDEN. The Property '{genPrpt}' did not checked.";
            }

            return $"GUI '{genId}' is not valid.";
        }

        /// <summary>
        /// Инициализация функтора <see cref="uiGenPropertyValueChecker"/>? проверяющего значения переменной элемента интерфейса
        /// </summary>
        /// <param name="uiVar"></param>
        /// <returns></returns>
        private bool initialize_CheckUIGenVarName(UIVar uiVar)
        {
            if (uiGenPropertyValueChecker is null)
            {
                var checker = @this.UiGenPropertyValue.GetCompareFunc(@this.UiGenPropertyValueType, (UIVar v) => v.Value);
                
                uiGenPropertyValueChecker = checker ?? (_ => false);
            }

            return uiGenPropertyValueChecker(uiVar);
        }
        private Func<UIVar, bool> uiGenPropertyValueChecker;


        private bool Validate(UIGen uigen)
        {
            return uigen != null && uigen.IsValid && uigen.Name.Equals(@this.UiGenId, StringComparison.Ordinal);
        }
    }
}
