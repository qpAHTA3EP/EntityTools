using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using System.Xml.Serialization;
using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;

namespace EntityTools.UCC.Conditions
{
    public class UCCGameUICheck : UCCCondition, ICustomUCCCondition
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenId)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenProperty)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenPropertyValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenPropertyValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertySign)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Check)));
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

        #region Взаимодействие с EntityToolsCore
        [NonSerialized]
        internal IUccConditionEngine Engine;

        public event PropertyChangedEventHandler PropertyChanged;

        public UCCGameUICheck()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
            Engine = new UccConditionProxy(this);
        }
        private IUccConditionEngine MakeProxy()
        {
            return new UccConditionProxy(this);
        }
        #endregion

        #region ICustomUCCCondition
        public new bool IsOK(UCCAction refAction) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).IsOK(refAction);

        public new bool Locked { get => base.Locked; set => base.Locked = value; }

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

        public string TestInfos(UCCAction refAction) => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).TestInfos(refAction);
        #endregion

        public override string ToString() => LazyInitializer.EnsureInitialized(ref Engine, MakeProxy).Label();
    }
}
