using Astral.Quester.Classes;
using EntityTools.Tools;
using EntityTools.Editors;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using Astral.Logic.UCC.Classes;
using EntityTools.Enums;
using EntityTools.UCC.Conditions;
using ConditionList = System.Collections.Generic.List<Astral.Logic.UCC.Classes.UCCCondition>;
using System.Xml.Serialization;

namespace EntityTools.Conditions
{
    [Serializable]
    public class UCCConditionPack : Astral.Logic.UCC.Classes.UCCCondition, ICustomUCCCondition
    {
        [Description("Displayed name of the ConditionPack")]
        public string Name { get; set; }

        [Description("The negation of the result of the ConditionPack")]
        public bool Not { get; set; }

        [Description("Logical rule of the Conditions checks\n" +
            "Conjunction: All Conditions have to be True (Logical AND)\n" +
            "Disjunction: At least one of the Conditions have to be True (Logical OR)")]
        public LogicRule TestRule { get; set; }

        [Description("The list of the Conditions")]
        [TypeConverter(typeof(CollectionTypeConverter))]
        [Editor(typeof(UCCConditionListEditor), typeof(UITypeEditor))]
        public ConditionList Conditions { get; set; } = new ConditionList();

        #region ICustomUCCCondition
        bool ICustomUCCCondition.IsOK(UCCAction refAction = null)
        {
            if (Conditions != null && Conditions.Count > 0)
                if (TestRule == LogicRule.Disjunction)
                {
                    int lockedNum = 0;
                    int okUnlockedNum = 0;
                    foreach (UCCCondition c in Conditions)
                    {
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (iCond.Loked)
                            {
                                if (!iCond.IsOK(refAction))
                                    return false;
                                lockedNum++;
                            }
                            else if (c.IsOK(refAction))
                                okUnlockedNum++;
                        }
                        else
                        {
                            if (c.Locked)
                            {
                                if (!c.IsOK(refAction))
                                    return false;
                                lockedNum++;
                            }
                            else if (c.IsOK(refAction))
                                okUnlockedNum++;
                        }
                    }

                    // Если множетство незалоченных условий пустое, тогда условие истино
                    // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истиное
                    return (Conditions.Count > lockedNum) ? okUnlockedNum > 0 : true;
                }
                else
                {
                    // Проверка всех условий
                    foreach (UCCCondition c in Conditions)
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (!iCond.IsOK(refAction))
                                return false;
                        }
                        else if (!c.IsOK(refAction))
                            return false;

                    return true;
                }
            return false;
        }

        bool ICustomUCCCondition.Loked { get => base.Locked; set => base.Locked = value; }

        string ICustomUCCCondition.TestInfos(UCCAction refAction = null)
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
                    sb.Append(cond.ToString()).Append(" | Result: ");
                    if (cond is ICustomUCCCondition iCond)
                        sb.Append(iCond.IsOK(refAction));
                    else sb.Append(cond.IsOK(refAction));
                    sb.AppendLine();
                } 
                sb.Append("Negation flag (Not): ").Append(Not).AppendLine();
                return sb.ToString();
            }
            else return "The list 'Conditions' is empty";
        }
        #endregion
        //public override bool IsValid
        //{
        //    get
        //    {
        //        if (Conditions.Count == 0)
        //            return false;

        //        bool result = (Tested == LogicRule.Conjunction);

        //        if (Tested == LogicRule.Conjunction)
        //        {
        //            foreach (UCCCondition cond in Conditions)
        //                if (!cond.IsValid)
        //                {
        //                    result = false;
        //                    break;
        //                }
        //        }
        //        else
        //        {
        //            int trueNumUnlock = 0,
        //                trueNumLock = 0;
        //            bool lockTrue = true;

        //            foreach (UCCCondition cond in Conditions)
        //            {
        //                if (cond.IsValid)
        //                {
        //                    if (cond.Locked)
        //                    { trueNumLock++;}
        //                    else trueNumUnlock++;
        //                }
        //                else
        //                {
        //                    if (cond.Locked)
        //                    {
        //                        lockTrue = false;
        //                        break;
        //                    }
        //                }
        //            }

        //            result = lockTrue && (Conditions.Count == trueNumLock || trueNumUnlock > 0);
        //        }
                
        //        return (Not)? !result : result;
        //    }
        //}

        //public override void Reset() { }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return "ConditionPack";
            else return $"ConditionPack: {Name}";
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
