using System.Windows.Forms;
using Infrastructure.Quester;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public abstract class ConditionBaseTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
        protected readonly BaseQuesterProfileProxy Owner;
        protected readonly ActionBaseTreeNode OwnedAction;

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

        protected ConditionBaseTreeNode(BaseQuesterProfileProxy profile/*, ActionBaseTreeNode ownedAction*/)
        {
            Owner = profile;
            //OwnedAction = ownedAction;
        }

        public abstract bool IsValid();
        public abstract string TestInfo();
        public abstract bool AllowChildren { get; }
        public abstract void UpdateView();
        public abstract QuesterCondition ReconstructInternal();
    }
}
