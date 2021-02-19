using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Core.Proxies;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Patches;

namespace EntityTools.Quester.Conditions
{
    public class CheckGameGUI : Condition
    {
        #region Взаимодействие с ядром EntityTools
        public event PropertyChangedEventHandler PropertyChanged;
        internal IQuesterConditionEngine Engine;

        public CheckGameGUI()
        {
            Engine = new QuesterConditionProxy(this);
        }
        #endregion

        static CheckGameGUI()
        {
            // Пременение патча на этапе десериализации (до инициализации плагина)
            ETPatcher.Apply();
        }

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
        internal string _uiGenID;

#if !DEVELOPER
        [Browsable(false)]
#endif
        public UiGenCheckType Tested
        {
            get => _tested; set
            {
                _tested = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tested)));
            }
        }
        internal UiGenCheckType _tested = UiGenCheckType.IsVisible;

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
            get => _uiGenPropertyValue;
            set
            {
                if (_uiGenPropertyValue != value)
                {
                    _uiGenPropertyValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenProperty)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UiGenPropertyValueType)));
                }
            }
        }
        internal ItemFilterStringType _uiGenPropertyValueType = ItemFilterStringType.Simple;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertySign)));
                }
            }
        }
        internal Presence _propertySign = Presence.Equal;
        #endregion


        public override bool IsValid => Engine.IsValid;

        public override void Reset() => Engine.Reset();

        public override string ToString() => Engine.Label();

        public override string TestInfos => Engine.TestInfos;
    }
}
