using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public abstract class ConditionBaseTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
        protected readonly QuesterProfileProxy Owner;

        public abstract QuesterCondition Content { get; }

        public virtual bool Locked
        {
            get => Content.Locked;
            set
            {
                Content.Locked = value;
                if (Checked != value)
                    Checked = value;
            }
        }

        protected ConditionBaseTreeNode(QuesterProfileProxy profile)
        {
            Owner = profile;
        }

        public abstract bool IsValid();
        public abstract string TestInfo();
        public abstract bool AllowChildren { get; }
        public abstract void UpdateView();
        public abstract QuesterCondition ReconstructInternal();
    }
}
