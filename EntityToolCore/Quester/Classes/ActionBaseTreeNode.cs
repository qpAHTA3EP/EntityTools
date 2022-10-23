using MyNW.Classes;
using System.Collections.Generic;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using QuesterAction = Astral.Quester.Classes.Action;

namespace EntityCore.Quester.Classes
{
    public abstract class ActionBaseTreeNode : TreeNode, ITreeNode<QuesterAction>
    {
        public abstract QuesterAction Content { get; }
        public abstract bool UseHotSpots { get; }
        public abstract List<Vector3> HotSpots { get; }

        public abstract QuesterAction.ActionValidity IsValid { get; }

        public abstract bool AllowChildren { get; }

        public abstract TreeNode[] GetConditionTreeNodes();
        public virtual void CopyConditionNodesFrom(TreeNodeCollection nodes)
        {
            if (nodes?.Count > 0)
            {
                TreeNode[] condNodes = new TreeNode[nodes.Count];
                nodes.CopyTo(condNodes, 0);
                conditionTreeNodes = condNodes;
            }
        }
        protected TreeNode[] conditionTreeNodes;

        public virtual bool Disabled
        {
            get => Content.Disabled;
            set
            {
                Content.Disabled = value;
                if (Checked == value)
                    Checked = !value;
            }
        }

        public virtual void GatherActionInfo(QuesterProfileProxy profile)
        {
            Content.GatherInfos();
            UpdateView();
        }

        public abstract void NewID();

        public abstract void UpdateView();

        public abstract QuesterAction ReconstructInternal();
    }
}
