using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
using MyNW.Classes;
using System.Text.RegularExpressions;
using EntityTools.Extensions;
using static Astral.Quester.Classes.Condition;
using EntityTools;

namespace EntityCore.UCC.Conditions
{
    public class UCCGameUICheckEngine : IUCCConditionEngine
    {
        #region Данные
        private UCCGameUICheck @this;

        private UIGen uiGen;
        private string label = string.Empty;
        #endregion

        internal UCCGameUICheckEngine(UCCGameUICheck eck)
        {
            @this = eck;
            @this.Engine = this;
            @this.PropertyChanged += PropertyChanged;

            ETLogger.WriteLine(LogType.Debug, $"{@this.GetType().Name}[{@this.GetHashCode().ToString("X2")}] initialized: {Label()}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(sender, @this))
            {
                if (e.PropertyName == nameof(@this.UiGenID))
                    label = string.Empty;
                uiGen = null;
            }
        }

        public bool IsOK(UCCAction refAction)
        {
            if (!Validate(uiGen) && !string.IsNullOrEmpty(@this._uiGenID))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == @this._uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return false;
            }

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
        }

        public string Label()
        {
            if (string.IsNullOrEmpty(label))
                label = $"{@this.GetType().Name} [{@this._uiGenID}]";
            else label = @this.GetType().Name;

            return label;
        }

        public string TestInfos(UCCAction refAction)
        {
            if (!Validate(uiGen))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == @this._uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return $"GUI '{@this._uiGenID}' is not found."; ;
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
                                if (CheckUiGenPropertyValue(uiVar))
                                    return $"The Property '{@this._uiGenID}.{@this._uiGenProperty}' equals to '{uiVar.Value}'.";
                                else return $"The Property '{@this._uiGenID}.{@this._uiGenProperty}' equals to '{uiVar.Value}'.";
                            }
                        return $"The Property '@this.{@this._uiGenID}.{@this._uiGenProperty}' does not founded.";
                    }
                    else return $"GUI '{@this._uiGenID}' is HIDDEN. The Property '{@this._uiGenProperty}' did not checked.";
            }

            return $"GUI '{@this._uiGenID}' is not valid.";
        }

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

        private bool Validate(UIGen uiGen)
        {
            return uiGen != null && uiGen.IsValid && uiGen.Name == @this._uiGenID;
        }
    }
}
