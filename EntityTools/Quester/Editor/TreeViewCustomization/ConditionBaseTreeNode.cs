using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public abstract class ConditionBaseTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
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
        public abstract bool IsValid(QuesterProfileProxy profile);
        public abstract string TestInfo(QuesterProfileProxy profile);
        public abstract bool AllowChildren { get; }
        public abstract void UpdateView();
        public abstract QuesterCondition ReconstructInternal();
    }
}
