using Astral.Quester.Classes;
using EntityTools.Tools;
using EntityTools.Editors;
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
    public class ConditionPack : Astral.Quester.Classes.Condition
    {
        [Description("Displayed name of the ConditionPack")]
        public string Name { get; set; }

        [Description("The negation of the result of the ConditionPack")]
        public bool Not { get; set; }

        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
        public ConditionCheck Tested { get; set; }

        [Description("The list of the Conditions")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        [Editor(typeof(ConditionListEditor), typeof(UITypeEditor))]
        public ConditionList Conditions { get; set; } = new ConditionList();

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
                            { trueNumLock++;}
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

                    result = lockTrue && (Conditions.Count == trueNumLock || trueNumUnlock > 0);
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
