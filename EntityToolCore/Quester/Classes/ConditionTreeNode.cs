using System;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using EntityCore.UCC.Classes;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterCondition"/>
    /// </summary>
    public class ConditionTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
        public bool AllowChildren => false;

        public ConditionTreeNode(QuesterCondition condition)
        {
            Tag = condition;
            var txt = condition.ToString();
            var type = condition.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            SelectIcon(condition);
            Checked = condition.Locked;
        }

        private void SelectIcon(QuesterCondition condition)
        {
            ImageKey = "Condition";
            SelectedImageKey = "Condition";
        }
        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is QuesterCondition condition)
            {
                var txt = condition.ToString();
                var type = condition.GetType();
                if (string.IsNullOrEmpty(txt)
                    || txt == type.FullName)
                    txt = type.Name;
                Text = txt;
                Checked = condition.Locked;
                SelectIcon(condition);
                //TreeView.Refresh();
            }
            else throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(QuesterCondition).FullName}");
        }

        public QuesterCondition ReconstructInternal()
        {
            return Tag as QuesterCondition;
        }

        public override object Clone()
        {
            return new ConditionTreeNode(CopyHelper.CreateDeepCopy((QuesterCondition)Tag));
        }
    }
}
