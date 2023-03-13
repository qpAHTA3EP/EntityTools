using System.Collections.Generic;
using System.Text;
using Infrastructure.Quester;
using Infrastructure.Reflection;
using EntityTools.Tools;
using EntityTools.Enums;
using EntityTools.Quester.Conditions;
using QuesterCondition = Astral.Quester.Classes.Condition;
using EntityTools.Patches;
using Infrastructure.Patches;
using System;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="ConditionPack"/>
    /// </summary>
    internal sealed class ConditionPackTreeNode : ConditionBaseTreeNode
    {
        private readonly QuesterCondition conditionPack;
        private readonly PropertyAccessor<List<QuesterCondition>> conditionAccessor;//= ACTP0Serializer.QuesterConditionPack.GetProperty<List<QuesterCondition>>("Conditions");
        private readonly PropertyAccessor<bool> notAccessor;//= ACTP0Serializer.QuesterConditionPack.GetProperty<bool>("Not");
        private readonly PropertyAccessor<uint> minCountAccessor;//= ACTP0Serializer.QuesterConditionPack.GetProperty<uint>("MinCount");
        private readonly PropertyAccessor testedAccessor;//= ACTP0Serializer.QuesterConditionPack.GetProperty("Tested");

        public ConditionPackTreeNode(BaseQuesterProfileProxy profile, QuesterCondition conditionPack) : base(profile)
        {
            if (!conditionPack.IsConditionPack())
                throw new ArgumentException("Type of condition should be 'ConditionType'");
            Tag = conditionPack;
            this.conditionPack = conditionPack;

            conditionAccessor = this.conditionPack.GetProperty<List<QuesterCondition>>("Conditions");
            notAccessor = this.conditionPack.GetProperty<bool>("Not");
            minCountAccessor = this.conditionPack.GetProperty<uint>("MinCount");
            testedAccessor = this.conditionPack.GetProperty("Tested");

            var conditions = conditionAccessor[conditionPack];
            if (conditions?.Count > 0)
                Nodes.AddRange(conditions.ToTreeNodes(Owner));
            UpdateView();
        }

        public override QuesterCondition Content => conditionPack;

        public override bool IsValid()
        {
            if (Nodes.Count == 0)
                return false;

            var tested = testedAccessor.Value.ToString();
            bool result = tested == "Conjunction";

            if (result)
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
                result = lockTrue && (Nodes.Count == trueNumLock || trueNumUnlock > minCountAccessor - 1);
            }
            return notAccessor ^ result;
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
                sb.Append("Negation flag (Not): ").Append(notAccessor).AppendLine();
                return sb.ToString();
            }
            return "The list 'Conditions' is empty";
        }
        
        public override bool AllowChildren => true;

        public override object Clone()
        {
            var copy = conditionPack.CreateXmlCopy();
            return new ConditionPackTreeNode(Owner, copy);
        }

        public override QuesterCondition ReconstructInternal()
        {
            conditionPack.Locked = Checked;
            conditionAccessor[conditionPack] = Nodes.ToQuesterConditionCollection<List<QuesterCondition>>();
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