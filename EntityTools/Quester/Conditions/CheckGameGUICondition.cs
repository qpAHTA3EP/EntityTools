using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;

using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Extensions;

using MyNW.Classes;
// ReSharper disable InconsistentNaming

namespace EntityTools.Quester.Conditions
{
    public class CheckGameGUI : Condition
    {
        #region Опции команды
#if DEVELOPER
        [Description("The Identifier of the Ingame user interface element")]
        [Editor(typeof(UiIdEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public string UiGenID
        {
            get { return _uiGenID; }
            set
            {
                if (_uiGenID != value)
                {
                    _uiGenID = value;
                    label = string.Empty;
                    uiGen = null;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _uiGenID;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public UiGenCheckType Tested
        {
            get => _tested; set
            {
                _tested = value;
                NotifyPropertyChanged();
            }
        }
        private UiGenCheckType _tested = UiGenCheckType.IsVisible;

#if DEVELOPER
        [Description("The Name of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public string UiGenProperty
        {
            get => _uiGenProperty;
            set
            {
                if (_uiGenProperty != value)
                {
                    _uiGenProperty = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _uiGenProperty;

#if DEVELOPER
        [Description("The Value of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public string UiGenPropertyValue
        {
            get => _uiGenPropertyValue;
            set
            {
                if (_uiGenPropertyValue != value)
                {
                    _uiGenPropertyValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _uiGenPropertyValue = string.Empty;

#if DEVELOPER
        [Description("Type of and UiGenPropertyValue:\n" +
                     "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
                     "Regex: Regular expression\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public ItemFilterStringType UiGenPropertyValueType
        {
            get => _uiGenPropertyValueType;
            set
            {
                if (_uiGenPropertyValueType != value)
                {
                    _uiGenPropertyValueType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ItemFilterStringType _uiGenPropertyValueType = ItemFilterStringType.Simple;

#if DEVELOPER
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public Presence PropertySign
        {
            get => _propertySign;
            set
            {
                if (_propertySign != value)
                {
                    _propertySign = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Presence _propertySign = Presence.Equal;
        #endregion

        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private UIGen uiGen;
        private string label = string.Empty;



        public override bool IsValid
        {
            get
            {
                if (uiGen == null && !string.IsNullOrEmpty(_uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == _uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    switch (_tested)
                    {
                        case UiGenCheckType.IsVisible:
                            return uiGen.IsVisible;
                        case UiGenCheckType.IsHidden:
                            return !uiGen.IsVisible;
                        case UiGenCheckType.Property:
                            if (uiGen.IsVisible)
                            {
                                bool result = false;
                                var uiVar = uiGen.Vars.FirstOrDefault(v => v.Name == _uiGenProperty);
                                if (uiVar != null)
                                    result = CheckUiGenPropertyValue(uiVar);
                                return result;
                            }
                            break;
                    }
                }
                return false;
            }
        }

        public override string TestInfos
        {
            get
            {
                if (uiGen == null && !string.IsNullOrEmpty(_uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == _uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    switch (_tested)
                    {
                        case UiGenCheckType.IsVisible:
                            return uiGen.IsVisible 
                                 ? $"GUI '{_uiGenID}' is VISIBLE." 
                                 : $"GUI '{_uiGenID}' is HIDDEN.";
                        case UiGenCheckType.IsHidden:
                            return uiGen.IsVisible 
                                 ? $"GUI '{_uiGenID}' is VISIBLE." 
                                 : $"GUI '{_uiGenID}' is HIDDEN.";
                        case UiGenCheckType.Property:
                            if (!uiGen.IsVisible)
                                return $"GUI '{_uiGenID}' is HIDDEN. The Property '{_uiGenProperty}' did not checked.";
                            var uiVar = uiGen.Vars.FirstOrDefault(v => v.IsValid && v.Name == _uiGenProperty);
                            if (uiVar == null) 
                                return $"The Property '{_uiGenID}.{_uiGenProperty}' does not founded.";
                            
                            return CheckUiGenPropertyValue(uiVar) 
                                 ? $"The Property '{_uiGenID}.{_uiGenProperty}' {_propertySign} to '{uiVar.Value}'." 
                                 : $"The Property '{_uiGenID}.{_uiGenProperty}' not {_propertySign} to '{uiVar.Value}'.";
                    }
                }

                return $"GUI '{_uiGenID}' is not valid.";
            }
        }

        public override string ToString()
        {
            label = !string.IsNullOrEmpty(label) 
                  ? $"{GetType().Name} [{_uiGenID}]" 
                  : GetType().Name;

            return label;
        }

        public override void Reset()
        {
            uiGen = null;
        }

        private bool CheckUiGenPropertyValue(UIVar uiVar)
        {
            if (uiVar == null || !uiVar.IsValid)
                return false;

            bool result = false;
            if (string.IsNullOrEmpty(uiVar.Value) && string.IsNullOrEmpty(_uiGenPropertyValue))
                result = true;
            else switch (_uiGenPropertyValueType)
            {
                case ItemFilterStringType.Simple:
                    result = uiVar.Value.CompareToSimplePattern(_uiGenPropertyValue);
                    break;
                case ItemFilterStringType.Regex:
                    //TODO Использовать предкомпилированный Regex
                    result = Regex.IsMatch(uiVar.Value, _uiGenPropertyValue);
                    break;
            }

            if (_propertySign == Presence.Equal)
                return result;
            return !result;
        }
    }
}
