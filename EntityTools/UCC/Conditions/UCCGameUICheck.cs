using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;

using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;

using MyNW.Classes;
// ReSharper disable InconsistentNaming

namespace EntityTools.UCC.Conditions
{
    public class UCCGameUICheck : UCCCondition, ICustomUCCCondition, INotifyPropertyChanged
    {
        #region Опции команды
#if DEVELOPER
        [Description("The Identifier of the Ingame user interface element")]
        [Editor(typeof(UiIdEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public string UiGenId
        {
            get { return uiGenId; }
            set
            {
                if (uiGenId != value)
                {
                    uiGenId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string uiGenId = "Team_Maptransferchoice_Waitingonteamlabel";

#if DEVELOPER
        [Description("The Name of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public string UiGenProperty
        {
            get => _uiGenProperty; set
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
        [DisplayName("PropertyValue")]
        [Category("GuiProperty")]
#else
        [Browsable(false)]
#endif
        public string UiGenPropertyValue
        {
            get => _uiGenPropertyValue; set
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
        [DisplayName("UiGenPropertyType")]
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
        public Condition.Presence PropertySign
        {
            get => _propertySign; set
            {
                if (_propertySign != value)
                {
                    _propertySign = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Condition.Presence _propertySign = Condition.Presence.Equal;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public UiGenCheckType Check
        {
            get => _check;
            set
            {
                if (_check != value)
                {
                    _check = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private UiGenCheckType _check = UiGenCheckType.IsVisible;

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; } = string.Empty;

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.ActionCond Tested { get; set; }
        #endregion
        #endregion




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = default)
        {
            InternalResetOnPropertyChanged(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void InternalResetOnPropertyChanged([CallerMemberName] string propertyName = default)
        {
            if (propertyName == nameof(Check))
                uiGenChecker = initialize_uiGenChecker;
            else if (propertyName == nameof(UiGenProperty))
                uiGenPropertyValueChecker = initialize_CheckUIGenVarName;
            _label = string.Empty;
            uiGen = null;
        }
        #endregion




        public new ICustomUCCCondition Clone()
        {
            return new UCCGameUICheck
            {
                uiGenId = uiGenId,
                _uiGenProperty = _uiGenProperty,
                _uiGenPropertyValue = _uiGenPropertyValue,
                _uiGenPropertyValueType = _uiGenPropertyValueType,
                _propertySign = _propertySign,
                _check = _check,

                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Tested = Tested,
                Value = Value
            };
        }




        #region Данные
        private UIGen uiGen;
        private string _label = string.Empty;
        #endregion





        

        public new bool IsOK(UCCAction refAction)
        {
            var genId = UiGenId;
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
                switch (Check)
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
                                    if (uiVar.IsValid && uiVar.Name.Equals(UiGenProperty, StringComparison.Ordinal))
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
            if (string.IsNullOrEmpty(_label))
            {
                var genId = UiGenId;
                if (string.IsNullOrEmpty(genId))
                    _label = GetType().Name;
                else
                {
                    var genProp = UiGenProperty;
                    if (Check != UiGenCheckType.Property
                        || string.IsNullOrEmpty(genProp))
                    {
                        _label = $"{GetType().Name} [{genId}]";
                    }
                    else
                    {
                        _label = $"{GetType().Name} [{genId}.{genProp}]";
                    }
                }
            }

            return _label;
        }

        public string TestInfos(UCCAction refAction)
        {
            var genId = UiGenId;
            if (!Validate(uiGen))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == genId);
                if (uiGen == null || !uiGen.IsValid)
                    return $"GUI '{genId}' is not found.";
            }

            switch (Check)
            {
                case UiGenCheckType.IsVisible:
                    return uiGen.IsVisible 
                         ? $"GUI '{genId}' is VISIBLE." 
                         : $"GUI '{genId}' is HIDDEN.";
                case UiGenCheckType.IsHidden:
                    return uiGen.IsVisible 
                         ? $"GUI '{genId}' is VISIBLE." 
                         : $"GUI '{genId}' is HIDDEN.";
                case UiGenCheckType.Property:
                    var genPrpt = UiGenProperty;
                    if (uiGen.IsVisible)
                    {
                        foreach (var uiVar in uiGen.Vars)
                            if (uiVar.IsValid && uiVar.Name.Equals(genPrpt, StringComparison.Ordinal))
                            {
                                if (uiGenPropertyValueChecker(uiVar))
                                    return $"The Property '{genId}.{genPrpt}' equals to '{uiVar.Value}'.";
                                else return $"The Property '{genId}.{genPrpt}' equals to '{uiVar.Value}'.";
                            }
                        return $"The Property '{genId}.{genPrpt}' does not founded.";
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
                var checker = UiGenPropertyValue.GetCompareFunc(UiGenPropertyValueType, (UIVar v) => v.Value);

                uiGenPropertyValueChecker = checker ?? (_ => false);
            }

            return uiGenPropertyValueChecker(uiVar);
        }
        private Func<UIVar, bool> uiGenPropertyValueChecker;


        private bool Validate(UIGen uigen)
        {
            return uigen != null && uigen.IsValid && uigen.Name.Equals(UiGenId, StringComparison.Ordinal);
        }
    }
}
