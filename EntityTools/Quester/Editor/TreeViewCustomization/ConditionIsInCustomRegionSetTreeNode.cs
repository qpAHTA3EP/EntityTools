using Infrastructure.Quester;
using Infrastructure.Reflection;
using EntityTools.Quester.Conditions;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="QuesterCondition"/>
    /// </summary>
    internal sealed class ConditionIsInCustomRegionSetTreeNode : ConditionBaseTreeNode
    {
        public ConditionIsInCustomRegionSetTreeNode(
            BaseQuesterProfileProxy profile, 
            //ActionBaseTreeNode ownedAction, 
            IsInCustomRegionSet condition
        ) : base (profile/*, ownedAction*/)
        {
            Tag = condition;
            this.condition = condition;
            UpdateView();
        }

        public override QuesterCondition Content => condition;
        private readonly IsInCustomRegionSet condition;

        public override bool IsValid()
        {
            condition.CustomRegions.DesignContext = Owner;
            return condition.IsValid;
        }

        public override string TestInfo()
        {
            condition.CustomRegions.DesignContext = Owner;
            return condition.TestInfos;
        }

        public override bool AllowChildren => false;

        public override void UpdateView()
        {
            condition.CustomRegions.DesignContext = Owner;
            var txt = condition.ToString();
            var type = condition.GetType();
            if (string.IsNullOrEmpty(txt)
                || txt == type.FullName)
                txt = type.Name;
            Text = txt;
            if (Checked != condition.Locked)
                Checked = condition.Locked;
            ImageKey = @"Condition";
            SelectedImageKey = @"Condition";
        }

        public override QuesterCondition ReconstructInternal()
        {
            condition.Locked = Checked;
            return condition;
        }

        public override object Clone()
        {
            var copy = condition.CreateXmlCopy();
            return new ConditionIsInCustomRegionSetTreeNode(Owner, /*OwnedAction,*/ (IsInCustomRegionSet)copy);
        }
    }
}
