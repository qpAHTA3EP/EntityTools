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
    public class UCCConditionTemplate : UCCCondition
    {
        //internal class QuesterConditionEditor : EnyTypeEditor<Condition>
        //{ }

        //[Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //public Condition Condition { get; set; } = null;

        public new bool IsOK(UCCAction refAction = null)
        {
            return false;
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
