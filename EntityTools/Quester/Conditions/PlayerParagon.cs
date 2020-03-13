using Astral.Quester.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EntityTools.Tools;
using System.ComponentModel;
using System.Drawing.Design;
using MyNW.Classes;
using EntityTools.Editors;

namespace EntityTools.Conditions
{
    [Serializable]
    public class PlayerParagon : Condition
    {

        [Editor(typeof(ParagonSelectEditor), typeof(UITypeEditor))]
        [Description("Select one or several paragons")]
        public SelectedParagons Paragons { get; set; } = new SelectedParagons();

        public Condition.Presence Tested { get; set; }

        [XmlIgnore]
        public override bool IsValid
        {
            get
            {
                return Paragons.IsValid;
            }
        }

        public override void Reset() { }

        public override string ToString()
        {
            return GetType().Name;
        }

        public override string TestInfos
        {
            get
            {
                AdditionalCharacterPath currentParagon = EntityManager.LocalPlayer.Character.CurrentPowerTreeBuild.SecondaryPaths.FirstOrDefault();
                if (currentParagon != null && currentParagon.IsValid)
                    return $"Current Paragon is '{currentParagon.Path.DisplayName}({currentParagon.Path.Name})'";
                return "No valid information";
            }
        }
    }
}
