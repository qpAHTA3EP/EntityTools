using System.Windows.Forms;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Quester.Classes
{
    internal abstract class ConditionBaseTreeNode : TreeNode, ITreeNode<QuesterCondition>
    {
        public abstract bool IsValid { get; }
        public abstract string TestInfo { get; }
        public abstract bool Locked { get; set; }
        public abstract bool AllowChildren { get; }
        public abstract void UpdateView();
        public abstract QuesterCondition ReconstructInternal();
    }
}
