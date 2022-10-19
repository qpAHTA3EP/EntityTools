using ACTP0Tools.Reflection;
using System;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterCondition"/>
    /// </summary>
    internal class ConditionTreeNode : ConditionBaseTreeNode
    {
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

        public override bool IsValid => ((QuesterCondition) Tag).IsValid;

        public override string TestInfo => ((QuesterCondition) Tag).TestInfos;

        public override bool Locked
        {
            get => ((QuesterCondition)Tag).Locked; 
            set => ((QuesterCondition)Tag).Locked = value;
        }
        public override bool AllowChildren => false;

        private void SelectIcon(QuesterCondition condition)
        {
            ImageKey = "Condition";
            SelectedImageKey = "Condition";
        }
        public override void UpdateView()
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
            }
            else throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(QuesterCondition).FullName}");
        }

        public override QuesterCondition ReconstructInternal()
        {
            return Tag as QuesterCondition;
        }

        public override object Clone()
        {
            return new ConditionTreeNode(CopyHelper.CreateDeepCopy((QuesterCondition)Tag));
        }
    }
}
