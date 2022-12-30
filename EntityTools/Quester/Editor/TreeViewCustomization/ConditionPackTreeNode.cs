using System.Collections.Generic;
using System.Text;
using Infrastructure.Quester;
using Infrastructure.Reflection;
using EntityTools.Tools;
using EntityTools.Enums;
using EntityTools.Quester.Conditions;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="ConditionPack"/>
    /// </summary>
    internal sealed class ConditionPackTreeNode : ConditionBaseTreeNode
    {
        private readonly ConditionPack conditionPack;

        public ConditionPackTreeNode(BaseQuesterProfileProxy profile, ConditionPack conditionPack) : base(profile)
        {
            Tag = conditionPack;
            this.conditionPack = conditionPack;
            var conditions = conditionPack.Conditions;
            if (conditions?.Count > 0)
                Nodes.AddRange(conditions.ToTreeNodes(Owner));
            UpdateView();
        }

        public override QuesterCondition Content => conditionPack;

        public override bool IsValid()
        {
            if (Nodes.Count == 0)
                return false;

            bool result = conditionPack.Tested == LogicRule.Conjunction;

            if (conditionPack.Tested == LogicRule.Conjunction)
            {
                foreach (ConditionBaseTreeNode node in Nodes)
                    if (!node.IsValid())
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

                foreach (ConditionBaseTreeNode node in Nodes)
                {
                    if (node.IsValid())
                    {
                        if (node.Checked)
                            trueNumLock++;
                        else trueNumUnlock++;
                    }
                    else if (node.Checked)
                    {
                        lockTrue = false;
                        break;
                    }
                }
                result = lockTrue && (Nodes.Count == trueNumLock || trueNumUnlock > conditionPack.MinCount - 1);
            }
            return conditionPack.Not ^ result;
        }

        public override string TestInfo()
        {
            if (Nodes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Text);
                sb.Append(" includes:").AppendLine();
                foreach (ConditionBaseTreeNode node in Nodes)
                {
                    sb.Append(node.Checked ? "\t[L] " : "\t[U] ");
                    sb.Append(node).Append(" | Result: ").Append(node.IsValid()).AppendLine();
                }
                sb.Append("Negation flag (Not): ").Append(conditionPack.Not).AppendLine();
                return sb.ToString();
            }
            return "The list 'Conditions' is empty";
        }
        
        public override bool AllowChildren => true;

        public override object Clone()
        {
            var copy = ((QuesterCondition)conditionPack).CreateXmlCopy();
            return new ConditionPackTreeNode(Owner, (ConditionPack)copy);
        }

        public override QuesterCondition ReconstructInternal()
        {
            conditionPack.Locked = Checked;
            conditionPack.Conditions = Nodes.ToQuesterConditionCollection<List<QuesterCondition>>();
            return conditionPack;
        }

        public override void UpdateView()
        {
            //if (TreeView is null)
            //    return;

            var name = conditionPack.ToString();
            Text = string.IsNullOrEmpty(name) 
                 ? "ConditionPack" 
                 : name;
            if (Checked != conditionPack.Locked)
                Checked = conditionPack.Locked;

            ImageKey = "ConditionList";
            SelectedImageKey = "ConditionList";
        }
    }
}