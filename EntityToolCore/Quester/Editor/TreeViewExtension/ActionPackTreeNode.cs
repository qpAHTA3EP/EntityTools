using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using Astral.Quester.Classes;
using EntityCore.Tools;
using MyNW.Classes;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;


namespace EntityCore.Quester.Editor.TreeViewExtension
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="ActionPack"/>
    /// </summary>
    public sealed class ActionPackTreeNode : ActionBaseTreeNode
    {
        private readonly ActionPack actionPack;

        public ActionPackTreeNode(QuesterProfileProxy profile, ActionPack actionPack, bool clone = false) : base(profile)
        {
            ActionPack actPack = clone ? CopyHelper.CreateDeepCopy(actionPack) : actionPack;
            Tag = actPack;
            this.actionPack = actPack;
            Nodes.AddRange(TreeViewHelper.ToTreeNodes(owner, actPack.Actions, clone));
            UpdateViewInternal(null);
        }

        public override QuesterAction Content => actionPack;

        public override bool UseHotSpots => actionPack.UseHotSpots;

        public override List<Vector3> HotSpots => actionPack.HotSpots;

        public override QuesterAction.ActionValidity IsValid => actionPack.IsValid;

        public override bool AllowChildren => true;

        public override object Clone()
        {
            var copy = CopyHelper.CreateDeepCopy((ActionPack) ReconstructInternal());
            UpdateId(copy);

            return new ActionPackTreeNode(owner, copy);
        }

        public override void NewID() => UpdateId(actionPack);
     
        private static void UpdateId(ActionPack actionPack)
        {
            actionPack.ActionID = Guid.NewGuid();
            foreach (var action in actionPack.Actions)
            {
                if (action is ActionPack internalPack)
                    UpdateId(internalPack);
                else action.ActionID = Guid.NewGuid();
            }
        }

        public override QuesterAction ReconstructInternal()
        {
            actionPack.Disabled = !Checked;
            var actions = new List<QuesterAction>(Nodes.Count);
            foreach (ActionBaseTreeNode node in Nodes)
            {
                actions.Add(node.ReconstructInternal());
            }
            actionPack.Actions = actions;

            if (conditionTreeNodes != null)
                actionPack.Conditions = conditionTreeNodes.ToListOf<QuesterCondition>();

            return actionPack;
        }
        
        public override void UpdateView()
        {
            UpdateViewInternal(null);
        }

        private void UpdateViewInternal(ActionPackTreeNode fromChildNode)
        {
            // ToolTipText отображает текст ошибки,
            // поэтому fromChildNode считается валидным, если ToolTipText пустое
            bool fromNodeIsValid = string.IsNullOrEmpty(fromChildNode?.ToolTipText);

            Text = actionPack.ToString();
            if (Checked == actionPack.Disabled)
                Checked = !actionPack.Disabled;

            if (fromNodeIsValid && AreInternalsValid(Nodes, fromChildNode))
            {
                ImageKey = "Box";
                SelectedImageKey = "Box";
                ToolTipText = string.Empty;
            }
            else
            {
                ImageKey = "BoxRed";
                SelectedImageKey = "BoxRed";
                ToolTipText = "There are some internal Action problem.";
            }
            if (Parent is ActionPackTreeNode parentActionPackNode)
                parentActionPackNode.UpdateViewInternal(this);
        }
        private static bool AreInternalsValid(TreeNodeCollection nodes, TreeNode fromChildNode)
        {
            if (nodes?.Count > 0)
            {
                foreach (ActionBaseTreeNode node in nodes)
                {
                    if (ReferenceEquals(node, fromChildNode)
                        && !string.IsNullOrEmpty(fromChildNode?.ToolTipText))
                        return false;
                        
                    if (node.AllowChildren)
                    {
                        if(!AreInternalsValid(node.Nodes, null))
                            return false;
                    }
                    else if (!node.IsValid.IsValid)
                        return false;
                }
            }
            return true;
        }


#if false
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="QuesterAction"/>.<see cref="QuesterAction.Conditions"/>
        /// </summary>
        public TreeNode[] ConditionTreeNodes
        {
            get => _conditionTreeNodes ?? (_conditionTreeNodes = actionPack.Conditions
                                                                           .ToTreeNodes()
                                                                           .ToArray());
            set => _conditionTreeNodes = value;
        }
#else
        public override TreeNode[] GetConditionTreeNodes()
        {
            return conditionTreeNodes ?? (conditionTreeNodes = actionPack.Conditions
                                                                         .ToTreeNodes()
                                                                         .ToArray());
        }
#endif
    }
}