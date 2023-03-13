using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Tools;
using ConditionList = System.Collections.Generic.List<Astral.Quester.Classes.Condition>;

namespace EntityTools.Quester.Conditions
{
    public class ConditionPack : Condition
    {
        [Description("Displayed name of the ConditionPack")]
        public string Name { get; set; }

        [Description("Minimum count 'true' for Disjunction logic")]
        public uint MinCount { get; set; } = 1;

        [Description("The negation of the result of the ConditionPack")]
        public bool Not { get; set; }

        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
        public LogicRule Tested { get; set; }

        [Browsable(false)]
        [Description("The list of the Conditions")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        //[Editor(typeof(ConditionListEditor), typeof(UITypeEditor))]
        public ConditionList Conditions { get; set; } = new ConditionList();

        public override bool IsValid
        {
            get
            {
                if (Conditions.Count == 0)
                    return false;

                bool result = Tested == LogicRule.Conjunction;

                if (Tested == LogicRule.Conjunction)
                {
                    foreach (Condition cond in Conditions)
                        if (!cond.IsValid)
                        {
                            result = false;
                            break;
                        }
                }
                else
                {
                    int trueNumUnlock = 0,
                        trueNumLock = 0;
                    bool lockTrue = true;

                    foreach (Condition cond in Conditions)
                    {
                        if (cond.IsValid)
                        {
                            if (cond.Locked)
                                trueNumLock++;
                            else trueNumUnlock++;
                        }
                        else
                        {
                            if (cond.Locked)
                            {
                                lockTrue = false;
                                break;
                            }
                        }
                    }
                    result = lockTrue && (Conditions.Count == trueNumLock || trueNumUnlock > MinCount - 1);
                }
                return Not ^ result;
            }
        }

        public override void Reset() { }

        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }

        public override string TestInfos
        {
            get
            {
                if (Conditions.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(GetType().Name);
                    if (!string.IsNullOrEmpty(Name))
                        sb.Append(" '").Append(Name).Append("' ");
                    sb.Append(" includes:").AppendLine();
                    foreach (Condition cond in Conditions)
                    {
                        if (cond.Locked)
                            sb.Append("\t[L] ");
                        else sb.Append("\t[U] ");
                        sb.Append(cond).Append(" | Result: ").Append(cond.IsValid).AppendLine();
                    }
                    sb.Append("Negation flag (Not): ").Append(Not).AppendLine();
                    return sb.ToString();
                }
                return "The list 'Conditions' is empty";
            }
        }
    }
}
