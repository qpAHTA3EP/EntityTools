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
            if (!Validate(uiGen) && !string.IsNullOrEmpty(@this._uiGenID))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == @this._uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return false;
            }

#if true
            return uiGenChecker(uiGen);
#else
            switch (@this._check)
            {
                case UiGenCheckType.IsVisible:
                    return uiGen.IsVisible;
                case UiGenCheckType.IsHidden:
                    return !uiGen.IsVisible;
                case UiGenCheckType.Property:
                    if (uiGen.IsVisible)
                    {
                        bool result = false;
                        foreach (var uiVar in uiGen.Vars)
                            if (uiVar.IsValid && uiVar.Name == @this._uiGenProperty)
                            {
                                result = CheckUiGenPropertyValue(uiVar);
                                break;
                            }
                        return result;
                    }
                    break;
            } 

            return false;
#endif
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
                switch (@this._check)
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
                                    if (uiVar.IsValid && uiVar.Name == @this._uiGenProperty)
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
                if (string.IsNullOrEmpty(@this._uiGenID))
                    label = @this.GetType().Name;
                else
                {
                    if (@this.Check != UiGenCheckType.Property
                        || string.IsNullOrEmpty(@this._uiGenProperty))
                    {
                        label = $"{@this.GetType().Name} [{@this._uiGenID}]";
                    }
                    else 
                    {
                        label = $"{@this.GetType().Name} [{@this._uiGenID}.{@this._uiGenProperty}]";
                    }
                }
            }

            return label;
        }

        public string TestInfos(UCCAction refAction)
        {
            if (!Validate(uiGen))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == @this._uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return $"GUI '{@this._uiGenID}' is not found.";
            }

            switch (@this._check)
            {
                case UiGenCheckType.IsVisible:
                    if (uiGen.IsVisible)
                        return $"GUI '{@this._uiGenID}' is VISIBLE.";
                    else return $"GUI '{@this._uiGenID}' is HIDDEN.";
                case UiGenCheckType.IsHidden:
                    if (uiGen.IsVisible)
                        return $"GUI '{@this._uiGenID}' is VISIBLE.";
                    else return $"GUI '{@this._uiGenID}' is HIDDEN.";
                case UiGenCheckType.Property:
                    if (uiGen.IsVisible)
                    {
                        foreach (var uiVar in uiGen.Vars)
                            if (uiVar.IsValid && uiVar.Name == @this._uiGenProperty)
                            {
                                if (uiGenPropertyValueChecker(uiVar))
                                    return $"The Property '{@this._uiGenID}.{@this._uiGenProperty}' equals to '{uiVar.Value}'.";
                                else return $"The Property '{@this._uiGenID}.{@this._uiGenProperty}' equals to '{uiVar.Value}'.";
                            }
                        return $"The Property '@this.{@this._uiGenID}.{@this._uiGenProperty}' does not founded.";
                    }
                    else return $"GUI '{@this._uiGenID}' is HIDDEN. The Property '{@this._uiGenProperty}' did not checked.";
            }

            return $"GUI '{@this._uiGenID}' is not valid.";
        }

#if true
        /// <summary>
        /// Инициализация функтора <see cref="uiGenPropertyValueChecker"/>? проверяющего значения переменной элемента интерфейса
        /// </summary>
        /// <param name="uiVar"></param>
        /// <returns></returns>
        private bool initialize_CheckUIGenVarName(UIVar uiVar)
        {
            if (uiGenPropertyValueChecker is null)
            {
                var checker = @this._uiGenPropertyValue.GetCompareFunc(@this._uiGenPropertyValueType, (UIVar v) => v.Value);
                
                uiGenPropertyValueChecker = checker ?? ((_) => false);
            }

            return uiGenPropertyValueChecker(uiVar);
        }
        private Func<UIVar, bool> uiGenPropertyValueChecker;
#else
        private bool CheckUiGenPropertyValue(UIVar uiVar)
        {
            if (uiVar == null || !uiVar.IsValid)
                return false;

            bool result = false;
            if (string.IsNullOrEmpty(uiVar.Value) && string.IsNullOrEmpty(@this._uiGenPropertyValue))
                result = true;
            else switch (@this._uiGenPropertyValueType)
                {
                    case ItemFilterStringType.Simple:
                        result = uiVar.Value.CompareToSimplePattern(@this._uiGenPropertyValue);
                        break;
                    case ItemFilterStringType.Regex:
                        result = Regex.IsMatch(uiVar.Value, @this._uiGenPropertyValue);
                        break;
                }

            if (@this._propertySign == Presence.Equal)
                return result;
            return !result;
        } 
#endif

        private bool Validate(UIGen uigen)
        {
            return uigen != null && uigen.IsValid && uigen.Name == @this._uiGenID;
        }
    }
}
