using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using Astral.Quester.Classes;
using EntityTools;
using EntityTools.Extentions;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace EntityTools.UCC.Conditions
{
    public class UCCGameUICheck : UCCCondition, ICustomUCCCondition
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
                     "Simple: Simple text string with a wildcard at the beginning or at the end (char '*' means any symbols)\n" +
                     "Regex: Regular expression\n" +
                     "Ignored if property 'Tested' is not equals to 'Property'")]
        [Category("GuiProperty")]
        public ItemFilterStringType UiGenPropertyValueType { get; set; } = ItemFilterStringType.Simple;

        [Category("GuiProperty")]
        public Condition.Presence PropertySign { get; set; } = Condition.Presence.Equal;


        public UiGenCheckType Check { get; set; } = UiGenCheckType.IsVisible;


        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            if (!Validate(uiGen) && !string.IsNullOrEmpty(uiGenID))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return false;
            }
            
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
            
            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (!Validate(uiGen))
            {
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == uiGenID);
                if (uiGen == null || !uiGen.IsValid)
                    return $"GUI '{uiGenID}' is not found."; ;
            }

            switch (Check)
            {
                case UiGenCheckType.IsVisible:
                    if (uiGen.IsVisible)
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
                                if (CheckUiGenPropertyValue(uiVar))
                                    return $"The Property '{uiGenID}.{UiGenProperty}' equals to '{uiVar.Value}'.";
                                else return $"The Property '{uiGenID}.{UiGenProperty}' equals to '{uiVar.Value}'.";
                            }
                        return $"The Property '{uiGenID}.{UiGenProperty}' does not founded.";
                    }
                    else return $"GUI '{uiGenID}' is HIDDEN. The Property '{UiGenProperty}' did not checked.";
            }
            
            return $"GUI '{uiGenID}' is not valid.";
        }
        #endregion


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

        private bool Validate(UIGen uiGen)
        {
            return uiGen != null && uiGen.IsValid && uiGen.Name == uiGenID;
        }

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

    }
}
