using Astral.Quester.Classes;
using EntityTools.Editors;
using EntityTools.Tools;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using ConditionList = System.Collections.Generic.List<Astral.Quester.Classes.Condition>;

namespace EntityTools.Conditions
{

    [Serializable]
    public enum ConditionCheck
    {
        Conjunction,
        Disjunction
    }

    [Serializable]
    public class ComplexCondition : Astral.Quester.Classes.Condition
    {
        [Description("Displayed name of the ComplexCondition")]
        public string Name { get; set; }

        [Description("The negation of the result of the ComplexCondition")]
        public bool Not { get; set; }

        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
        public ConditionCheck Tested { get; set; }

        [Description("The list of the Conditions")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        [Editor(typeof(ConditionListEditor), typeof(UITypeEditor))]
        public ConditionList Conditions { get; set; }

        public override bool IsValid
        {
            get
            {
                if (Conditions.Count == 0)
                    return false;

                bool result = (Tested == ConditionCheck.Conjunction);

                if (Tested == ConditionCheck.Conjunction)
                {
                    foreach (Condition cond in Conditions)
                        if (cond.IsValid)
                            continue;
                        else { result = false; break; }
                }
                else
                {
                    foreach (Condition cond in Conditions)
                    {
                        if (cond.IsValid)
                            result = true;
                        else if (cond.Locked)
                            { result = false; break; }
                        else continue;
                    }
                }
                
                return (Not)? !result : result;
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
                        sb.Append(cond.ToString()).Append(" | Result: ").Append(cond.IsValid).AppendLine();
                    }
                    sb.Append("Negation flag (Not): ").Append(Not).AppendLine();
                    return sb.ToString();
                }
                else return "The list 'Conditions' is empty";
            }
        }

    }
}
