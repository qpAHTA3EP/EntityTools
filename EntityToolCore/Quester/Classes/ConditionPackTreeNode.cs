using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using EntityCore.Tools;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;


namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="ConditionPack"/>
    /// </summary>
    public class ConditionPackTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
        public bool AllowChildren => true;

        public ConditionPackTreeNode(QuesterCondition conditionPack)
        {
            if (!conditionPack.IsConditionPack())
                throw new ArgumentException($"Argument cannot be casted to type {TreeViewHelper.QuesterConditionPackType.FullName}", nameof(conditionPack));
            Tag = conditionPack;
            Text = conditionPack.ToString();
            //BackColor = SystemColors.ControlDark;
            ImageKey = "ConditionList";
            SelectedImageKey = "ConditionList";
            Checked = conditionPack.Locked;
            var conditions = conditionPack.GetConditions();
            if(conditions?.Count > 0)
                Nodes.AddRange(conditions.ToTreeNodes());
        }

        public override object Clone()
        {
            return new ConditionPackTreeNode(CopyHelper.CreateDeepCopy((QuesterCondition)Tag));
        }

        public QuesterCondition ReconstructInternal()
        {
            if (Tag is QuesterCondition conditionPack
                && conditionPack.IsConditionPack())
            {
                conditionPack.SetConditions(Nodes.ToQuesterConditionCollection<List<QuesterCondition>>());
                return conditionPack;
            }
            throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {TreeViewHelper.QuesterConditionPackType.FullName}");
        }

        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is QuesterCondition conditionPack
                && conditionPack.IsConditionPack())
            {
                var name = conditionPack.ToString();
                if (string.IsNullOrEmpty(name))
                    Text = "ConditionPack";
                else Text = name;
                Checked = conditionPack.Locked;

                ImageKey = "ConditionList";
                SelectedImageKey = "ConditionList";

                //TreeView.Refresh();
            }
            else throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {TreeViewHelper.QuesterConditionPackType.FullName}");
        }
    }
}