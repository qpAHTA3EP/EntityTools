using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace EntityTools.UCC.Conditions
{
    public class UCCConditionGameUICheck : UCCCondition
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
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"GameUICheck [{UiGenID}]";
        }
    }
}
