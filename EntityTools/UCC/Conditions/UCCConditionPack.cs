using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Astral.Logic.UCC.Classes;
using EntityTools.Editors;
using EntityTools.Enums;
using EntityTools.Tools;
using ConditionList = System.Collections.ObjectModel.ObservableCollection<Astral.Logic.UCC.Classes.UCCCondition>;

namespace EntityTools.UCC.Conditions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class UCCConditionPack : UCCCondition, ICustomUCCCondition
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
        [Editor(typeof(UccConditionListEditor), typeof(UITypeEditor))]
        public ConditionList Conditions { get; set; } = new ConditionList();

#region ICustomUCCCondition
        public new bool IsOK(UCCAction refAction)
        {
            bool result = true;
            if (Conditions?.Count > 0)
            {
                if (TestRule == LogicRule.Disjunction)
                {
                    int okLockedNum = 0;
                    int okUnlockedNum = 0;
                    bool lockedTrue = true;
                    foreach (UCCCondition c in Conditions)
                    {
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (iCond.Locked)
                            {
                                if (!iCond.IsOK(refAction))
                                {
                                    lockedTrue = false;
                                    break;
                                }
                                okLockedNum++;
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
                                okLockedNum++;
                            }
                            else if (c.IsOK(refAction))
                                okUnlockedNum++;
                        }
                    }

                    // Если множество незалоченных условий пустое, тогда условие истино
                    // Если оно НЕ пустое, тогда должно встретиться хотя бы одно истиное
                    result = lockedTrue && (Conditions.Count == okLockedNum || okUnlockedNum > 0);
                }
                else
                {
                    // Проверка всех условий
                    foreach (UCCCondition c in Conditions)
                    {
                        if (c is ICustomUCCCondition iCond)
                        {
                            if (iCond.IsOK(refAction)) continue;
                            result = false;
                            break;
                        }
                        if (!c.IsOK(refAction))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            return Not ? !result : result;
        }

        public new bool Locked { get => base.Locked; set => base.Locked = value; }

        public new ICustomUCCCondition Clone()
        {
            var copy = new UCCConditionPack
            {
                Name = Name,
                Not = Not,
                TestRule = TestRule,
                Sign = Sign,
                Locked = base.Locked,
                Target = Target,
                Tested = Tested,
                Value = Value,
                Conditions = new ConditionList(Conditions.Select(cnd => cnd is ICustomUCCCondition cstCnd
                                                                                      ? (UCCCondition)cstCnd.Clone()
                                                                                      : cnd.Clone()))
            };

            return copy;
        }

        public string TestInfos(UCCAction refAction/* = null*/)
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
                return $"ConditionPack";
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
