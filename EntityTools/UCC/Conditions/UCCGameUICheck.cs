using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using Astral.Quester.Classes;
using EntityTools;
using EntityTools.Extensions;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using EntityTools.Core.Interfaces;
using EntityTools.Core.Proxies;

namespace EntityTools.UCC.Conditions
{
    public class UCCGameUICheck : UCCCondition, ICustomUCCCondition
    {
        #region Взаимодействие с EntityToolsCore
#if CORE_INTERFACES
        [NonSerialized]
        internal IUCCConditionEngine Engine;
#endif
        public event PropertyChangedEventHandler PropertyChanged;

        public UCCGameUICheck()
        {
            Sign = Astral.Logic.UCC.Ressources.Enums.Sign.Superior;
#if CORE_INTERFACES
            Engine = new UCCConditionProxy(this);
#endif
            // EntityTools.Core.Initialize(this);
        }
        #endregion


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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenID)));
                }
            }
        }
        internal string _uiGenID = "Team_Maptransferchoice_Waitingonteamlabel";

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
        internal string _uiGenProperty;

#if DEVELOPER
        [Description("The Value of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
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
        internal string _uiGenPropertyValue = string.Empty;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenPropertyValue)));
                }
            }
        }
        internal ItemFilterStringType _uiGenPropertyValueType = ItemFilterStringType.Simple;

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
        internal Condition.Presence _propertySign = Condition.Presence.Equal;

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
        internal UiGenCheckType _check = UiGenCheckType.IsVisible;

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


        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/) => Engine.IsOK(refAction);

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction) => Engine.TestInfos(refAction);
        #endregion


        public override string ToString() => Engine.Label();
    }
}
