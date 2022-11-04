using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Editor.TreeViewExtension
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterCondition"/>
    /// </summary>
    internal sealed class ConditionTreeNode : ConditionBaseTreeNode
    {
        public ConditionTreeNode(QuesterCondition condition)
        {
            Tag = condition;
            this.condition = condition;
            UpdateView();
        }

        public override QuesterCondition Content => condition;
        private readonly QuesterCondition condition;

        public override bool IsValid(QuesterProfileProxy profile) => condition.IsValid;

        public override string TestInfo(QuesterProfileProxy profile) => condition.TestInfos;
        
        public override bool AllowChildren => false;

        public override void UpdateView()
        {
            var txt = condition.ToString();
            var type = condition.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            if(Checked != condition.Locked)
                Checked = condition.Locked;
            ImageKey = "Condition";
            SelectedImageKey = "Condition";
        }

        public override QuesterCondition ReconstructInternal()
        {
            condition.Locked = Checked;
            return condition;
        }

        public override object Clone()
        {
            return new ConditionTreeNode(CopyHelper.CreateDeepCopy(condition));
        }
    }
}
