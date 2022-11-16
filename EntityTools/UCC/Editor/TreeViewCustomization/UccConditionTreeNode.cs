using System;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="UCCCondition"/>
    /// </summary>
    public class UccConditionTreeNode : TreeNode, IUccTreeNode<UCCCondition>
    {
        //public UCCAction Data => (UCCAction)Tag;

        public bool AllowChildren => false;

        public UccConditionTreeNode(UCCCondition condition)
        {
            Tag = condition;
            var txt = condition.ToString();
            if (txt != condition.GetType().FullName)
                Text = txt;
            else Text = $"{condition.Target} [{condition.Tested}] {condition.Sign} '{condition.Value}'";
            SelectIcon(condition);
            Checked = condition.Locked;
        }

        private void SelectIcon(UCCCondition condition)
        {
            switch (condition)
            {
                default:
                    ImageKey = "Condition";
                    SelectedImageKey = "Condition";
                    break;
            }
        }
        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is UCCCondition condition)
            {
                var txt = condition.ToString();
                if (txt != condition.GetType().FullName)
                    Text = txt;
                else Text = $"{condition.Target} [{condition.Tested}] {condition.Sign} '{condition.Value}'";
                Checked = condition.Locked;
                SelectIcon(condition);
                TreeView.Refresh();
            }
            else throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCCondition)}");
        }

        public UCCCondition ReconstructInternal()
        {
            return Tag as UCCCondition;
        }

        public override object Clone()
        {
            return new UccConditionTreeNode((UCCCondition)Tag.CreateDeepCopy());
        }
    }
}
