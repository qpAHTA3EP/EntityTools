using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Infrastructure.Quester;
using MyNW.Classes;
using QuesterAction = Astral.Quester.Classes.Action;

namespace EntityTools.Quester.Editor.TreeViewCustomization
{
    public abstract class ActionBaseTreeNode : TreeNode, ITreeNode<QuesterAction>
    {
        protected readonly BaseQuesterProfileProxy Owner;
        public abstract QuesterAction Content { get; }
        public abstract bool UseHotSpots { get; }
        public abstract List<Vector3> HotSpots { get; }

        protected ActionBaseTreeNode(BaseQuesterProfileProxy profile)
        {
            Owner = profile;
        }

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
            else conditionTreeNodes = Array.Empty<TreeNode>();
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

        public virtual void GatherActionInfo(BaseQuesterProfileProxy profile)
        {
            Content.GatherInfos();
            UpdateView();
        }

        public abstract void RegenActionID(); 

        public abstract void UpdateView();

        public abstract QuesterAction ReconstructInternal();
    }
}
