using Astral.Classes.ItemFilter;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System.ComponentModel;
using System.Drawing.Design;
using static Astral.Quester.Classes.Condition;

namespace EntityTools.UCC.Conditions
{
    public class UCCConditionGameUICheck : CustomUCCCondition
    {
        private string _uiGenID = "Team_Maptransferchoice_Waitingonteamlabel";
        private UIGen uiGen;

        [Description("The Identifier of the Ingame user interface element")]
        [Editor(typeof(UiIdEditor), typeof(UITypeEditor))]
        public string UiGenID
        {
            get { return _uiGenID; }
            set
            {
                if (_uiGenID != value)
                {
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == value);
                    if (uiGen != null)
                        _uiGenID = value;
                }
            }
        }

        public UiGenCheckType Tested { get; set; } = UiGenCheckType.IsVisible;

        [Browsable(false)]
        public override bool IsOK(UCCAction refAction = null)
        {
            if (uiGen == null && !string.IsNullOrEmpty(_uiGenID))
                uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == _uiGenID);
            if (uiGen != null && uiGen.IsValid)
            {
                switch (Tested)
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
            return $"{GetType().Name} [{UiGenID}]";
        }
    }
}
