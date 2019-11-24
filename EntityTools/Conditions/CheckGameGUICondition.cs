using Astral.Classes.ItemFilter;
using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EntityTools.Conditions
{
    public class CheckGameGUI : Astral.Quester.Classes.Condition
    {
        private string uiGenID;
        private UIGen uiGen;

        public CheckGameGUI() { }


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
                    if(uiGen != null)
                        uiGenID = value;
                }                
            }
        }

        public UiGenCheckType Tested { get; set; } = UiGenCheckType.IsVisible;

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
        public Condition.Presence PropertySign { get; set; } = Presence.Equal;

        public override bool IsValid
        {
            get
            {
                if(uiGen == null && !string.IsNullOrEmpty(uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    switch (Tested)
                    {
                        case UiGenCheckType.IsVisible:
                            return uiGen.IsVisible;
                        case UiGenCheckType.IsHidden:
                            return !uiGen.IsVisible;
                        case UiGenCheckType.Property:
                            if(uiGen.IsVisible)
                            {
                                bool result = false;
                                foreach(var uiVar in uiGen.Vars)
                                    if (uiVar.Name == UiGenProperty)
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
        }


        public override void Reset() { }

        public override string ToString()
        {
            return $"{GetType().Name} [{UiGenID}]";
        }

        public override string TestInfos
        {
            get
            {
                if (uiGen == null && !string.IsNullOrEmpty(uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    switch (Tested)
                    {
                        case UiGenCheckType.IsVisible:
                            if(uiGen.IsVisible)
                                return $"GUI '{uiGenID}' is VISIBLE.";
                            else return $"GUI '{uiGenID}' is HIDDEN.";
                        case UiGenCheckType.IsHidden:
                            if (uiGen.IsVisible)
                                return $"GUI '{uiGenID}' is VISIBLE.";
                            else return $"GUI '{uiGenID}' is HIDDEN.";
                        case UiGenCheckType.Property:
                            if (uiGen.IsVisible)
                            {
                                foreach (var uiVar in uiGen.Vars)
                                    if (uiVar.IsValid && uiVar.Name == UiGenProperty)
                                    {
                                        if(CheckUiGenPropertyValue(uiVar))
                                            return $"The Property '{uiGenID}.{UiGenProperty}' equals to '{uiVar.Value}'.";
                                        else return $"The Property '{uiGenID}.{UiGenProperty}' equals to '{uiVar.Value}'.";
                                    }
                                return $"The Property '{uiGenID}.{UiGenProperty}' does not founded.";
                            }
                            else return $"GUI '{uiGenID}' is HIDDEN. The Property '{UiGenProperty}' did not checked.";
                    }
                }

                return $"GUI '{uiGenID}' is not valid.";
            }
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

            if (PropertySign == Presence.Equal)
                return result;
            else return !result;
        }
    }
}
