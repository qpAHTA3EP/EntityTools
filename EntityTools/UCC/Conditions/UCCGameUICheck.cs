using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace EntityTools.UCC.Conditions
{
    public class UCCGameUICheck : UCCCondition
    {
        private string uiGenID = "Team_Maptransferchoice_Waitingonteamlabel";
        private UIGen uiGen;

        [Description("The Identifier of the Ingame user interface element")]
        [Editor(typeof(UiIdEditor), typeof(UITypeEditor))]
        public string UiGenID
        {
            get { return uiGenID; }
            set
            {
                if (uiGenID != value)
                {
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == value);
                    uiGenID = value;
                }
            }
        }

        [Description("The Name of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
        public string UiGenProperty { get; set; }

        [Description("The Value of the GUI element's property which is checked\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
        public string UiGenPropertyValue { get; set; } = string.Empty;

        [Description("Type of and UiGenPropertyValue:\n" +
                     "Simple: Simple test string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
                     "Regex: Regular expression\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
        public ItemFilterStringType UiGenPropertyValueType { get; set; } = ItemFilterStringType.Simple;

        [Category("GuiProperty")]
        public Condition.Presence PropertySign { get; set; } = Condition.Presence.Equal;


        public UiGenCheckType Check { get; set; } = UiGenCheckType.IsVisible;


        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new string Value { get; set; } = string.Empty;

        [XmlIgnore]
        [Browsable(false)]
        public new Enums.Unit Target { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public new Enums.ActionCond Tested { get; set; }
        #endregion

        public new bool IsOK(UCCAction refAction = null)
        {
            if (uiGen == null && !string.IsNullOrEmpty(uiGenID))
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
            if (uiGen != null && uiGen.IsValid)
            {
                switch (Check)
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
                                if (uiVar.IsValid && uiVar.Name == UiGenProperty)
                                {
                                    result = CheckUiGenPropertyValue(uiVar);
                                    break;
                                }
                            return result;
                        }
                        break;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"GameUICheck [{UiGenID}]";
        }

        private bool CheckUiGenPropertyValue(UIVar uiVar)
        {
            if (uiVar == null || !uiVar.IsValid)
                return false;

            bool result = false;
            if (string.IsNullOrEmpty(uiVar.Value) && string.IsNullOrEmpty(UiGenPropertyValue))
                result = true;
            else switch (UiGenPropertyValueType)
                {
                    case ItemFilterStringType.Simple:
                        result = uiVar.Value.CompareToSimplePattern(UiGenPropertyValue);
                        break;
                    case ItemFilterStringType.Regex:
                        result = Regex.IsMatch(uiVar.Value, UiGenPropertyValue);
                        break;
                }

            if (PropertySign == Condition.Presence.Equal)
                return result;
            else return !result;
        }

    }
}
