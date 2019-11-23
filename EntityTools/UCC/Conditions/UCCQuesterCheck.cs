using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;

namespace EntityTools.UCC.Conditions
{
    public class UCCQuesterCheck : UCCCondition
    {
        internal class QuesterConditionEditor : EnyTypeEditor<Condition>
        { }

        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Condition Condition { get; set; } = null;

        public new bool IsOK(UCCAction refAction = null)
        {
            return Condition?.IsValid == true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Enums.Sign Sign { get; set; }

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
    }
}
