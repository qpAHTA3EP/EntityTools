using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;
using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.UCC.Conditions
{
    [Serializable]
    public class UCCConditionPack : UCCCondition, ICustomUCCCondition
    {
#if DEVELOPER
        [Description("Displayed name of the ConditionPack")]
#else
        [Browsable(false)]
#endif
        public string Name { get; set; }

#if DEVELOPER
        [Description("The negation of the result of the ConditionPack")]
#else
        [Browsable(false)]
#endif
        public bool Not { get; set; }

#if DEVELOPER
        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
#else
        [Browsable(false)]
#endif
        public LogicRule TestRule { get; set; }

#if DEVELOPER
        [Description("The list of the Conditions")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        [Editor(typeof(UCCConditionListEditor), typeof(UITypeEditor))]
#else
        [Browsable(false)]
#endif
        public ConditionList Conditions { get; set; } = new ConditionList();

#region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction/* = null*/)
        {
            bool result = true;
            if (Conditions != null && Conditions.Count > 0)
                if (TestRule == LogicRule.Disjunction)
                {
                    int lockedNum = 0;
                    int okUnlockedNum = 0;
                    bool lockedTrue = true;
                    foreach (UCCCondition c in Conditions)
                    {
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (iCond.Loked)
                            {
                                if (!iCond.IsOK(refAction))
                                {
                                    lockedTrue = false;
                                    break;
                                }
                                lockedNum++;
                            }
                            else if (iCond.IsOK(refAction))
                                okUnlockedNum++;
                        }
                        else
                        {
                            if (c.Locked)
                            {
                                if (!c.IsOK(refAction))
                                {
                                    lockedTrue = false;
                                    break;
                                }
                                lockedNum++;
                            }
                            else if (c.IsOK(refAction))
                                okUnlockedNum++;
                        }
                    }

                    // Если множетство незалоченных условий пустое, тогда условие истино
                    // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истиное
                    result = lockedTrue && (Conditions.Count == lockedNum || okUnlockedNum > 0);
                }
                else
                {
                    // Проверка всех условий
                    foreach (UCCCondition c in Conditions)
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (!iCond.IsOK(refAction))
                            {
                                result = false;
                                break;
                            }
                        }
                        else if (!c.IsOK(refAction))
                        {
                            result = false;
                            break;
                        }
                }
            return (Not) ? !result : result;
        }

        bool ICustomUCCCondition.Loked { get => Locked; set => Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction/* = null*/)
        {
            if (Conditions.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(GetType().Name);
                if (!string.IsNullOrEmpty(Name))
                    sb.Append(" '").Append(Name).Append("' ");
                sb.Append(" includes:").AppendLine();
                foreach (UCCCondition cond in Conditions)
                {
                    if (cond.Locked)
                        sb.Append("\t[L] ");
                    else sb.Append("\t[U] ");
                    sb.Append(cond).Append(" | Result: ");
                    if (cond is ICustomUCCCondition iCond)
                        sb.Append(iCond.IsOK(refAction));
                    else sb.Append(cond.IsOK(refAction));
                    sb.AppendLine();
                } 
                sb.Append("Negation flag (Not): ").Append(Not).AppendLine();
                return sb.ToString();
            }

            return "The list 'Conditions' is empty";
        }
#endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return "ConditionPack";
            return $"ConditionPack: {Name}";
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
