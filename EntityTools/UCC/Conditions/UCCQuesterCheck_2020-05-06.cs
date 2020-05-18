using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Ressources;
using System.ComponentModel;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityTools.Editors;
using System.Drawing.Design;
using System;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.UCC.Conditions
{
    public class UCCQuesterCheck : UCCCondition, ICustomUCCCondition
    {
#if DEVELOPER
        internal class QuesterConditionEditor : AddTypeCommonEditor<QuesterCondition>
        { }

        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
#else
        [Browsable(false)]
#endif
        public QuesterCondition Condition { get; set; } = null;

#region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            return Condition?.IsValid == true;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (Condition != null)
                return Condition.TestInfos;
            return $"{nameof(Condition)} does not set.";
        }
#endregion

        public override string ToString()
        {
            return GetType().Name;
        }

#region Hide Inherited Properties
        [XmlIgnore]
        [Browsable(false)]
        public new Astral.Logic.UCC.Ressources.Enums.Sign Sign { get; set; }

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
