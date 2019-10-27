using EntityTools.Editors;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Conditions
{
    public class CheckGameGUI : Astral.Quester.Classes.Condition
    {
        private string _uiGenID;
        private UIGen uiGen;

        public CheckGameGUI() { }


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
                    if(uiGen != null)
                        _uiGenID = value;
                }                
            }
        }

        public UiGenCheckType Tested { get; set; } = UiGenCheckType.IsVisible;

        public override bool IsValid
        {
            get
            {
                if(uiGen == null && !string.IsNullOrEmpty(_uiGenID))
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
                if (uiGen == null && !string.IsNullOrEmpty(_uiGenID))
                    uiGen = MyNW.Internals.UIManager.AllUIGen.Find(x => x.Name == _uiGenID);
                if (uiGen != null && uiGen.IsValid)
                {
                    switch (Tested)
                    {
                        case UiGenCheckType.IsVisible:
                            if(uiGen.IsVisible)
                                return $"GUI '{_uiGenID}' is VISIBLE.";
                            else return $"GUI '{_uiGenID}' is HIDDEN.";
                        case UiGenCheckType.IsHidden:
                            if (uiGen.IsVisible)
                                return $"GUI '{_uiGenID}' is VISIBLE.";
                            else return $"GUI '{_uiGenID}' is HIDDEN.";
                    }
                }

                return $"GUI '{_uiGenID}' is not valid.";
            }
        }
    }
}
