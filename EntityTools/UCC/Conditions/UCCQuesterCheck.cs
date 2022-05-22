using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using AcTp0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.UCC.Conditions
{
    public class UCCQuesterCheck : UCCCondition, ICustomUCCCondition
    {
#if DEVELOPER
        [Editor(typeof(QuesterConditionEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [XmlElement("QuesterCondition", typeof(QuesterCondition))]
#else
        [Browsable(false)]
#endif
        public QuesterCondition Condition { get; set; }

#region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            return Condition?.IsValid == true;
        }

        bool ICustomUCCCondition.Locked { get => base.Locked; set => base.Locked = value; }

        ICustomUCCCondition ICustomUCCCondition.Clone()
        {
            var copy = new UCCQuesterCheck
            {
                Sign = Sign,
                Locked = Locked,
                Target = Target,
                Tested = Tested,
                Value = Value,
            };
            if (Condition != null)
                copy.Condition = CopyHelper.CreateDeepCopy(Condition);

            return copy;
        }

        string ICustomUCCCondition.TestInfos(UCCAction refAction)
        {
            if (Condition != null)
                return Condition.TestInfos;
            return $"{nameof(Condition)} does not set.";
        }
#endregion

        public override string ToString()
        {
            var cond = Condition;
            if(cond is null)
                return GetType().Name;
            return cond.ToString();
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
