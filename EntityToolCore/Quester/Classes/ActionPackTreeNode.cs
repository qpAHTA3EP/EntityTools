using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Quester.Classes;
using EntityCore.Forms;
using EntityCore.Tools;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;


namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="ActionPack"/>
    /// </summary>
    public class ActionPackTreeNode : TreeNode, IActionTreeNode
    {
        public bool AllowChildren => true;

        public ActionPackTreeNode(ActionPack actionPack, bool clone = false)
        {
            ActionPack actPack = clone ? CopyHelper.CreateDeepCopy(actionPack) : actionPack;
            Tag = actPack;
            Text = actPack.ToString();
            //BackColor = SystemColors.ControlDark;
            ImageKey = "Box";
            SelectedImageKey = "Box";
            Checked = !actPack.Disabled;
            Nodes.AddRange(actPack.Actions.ToTreeNodes());

            //_conditionTreeNodes = ReconstructInternal().Conditions.ToTreeNodes().ToArray();
        }

        public override object Clone()
        {
            var actionPack = CopyHelper.CreateDeepCopy((ActionPack) ReconstructInternal());
            UpdateId(actionPack);

            return new ActionPackTreeNode(actionPack);
        }

        private void UpdateId(ActionPack actionPack)
        {
            actionPack.ActionID = Guid.NewGuid();
            foreach (var action in actionPack.Actions)
            {
                if (action is ActionPack internalPack)
                    UpdateId(internalPack);
                else action.ActionID = Guid.NewGuid();
            }
        }

        public QuesterAction ReconstructInternal()
        {
            if (Tag is ActionPack actionPack)
            {
                var actList = new List<QuesterAction>(Nodes.Count);
                foreach (TreeNode node in Nodes)
                {
                    if (node is ITreeNode<QuesterAction> questerNode)
                        actList.Add(questerNode.ReconstructInternal());
                }
                actionPack.Actions = actList;

                actionPack.Conditions = _conditionTreeNodes.ToQuesterConditionList();

                return actionPack;
            }
            throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(ActionPack).FullName}");
        }

        public void UpdateView()
        {
            if (TreeView is null)
                return;

            UpdateViewInternal(null);
        }

        private void UpdateViewInternal(ActionPackTreeNode fromChildNode)
        {
            // ToolTipText отображает текст ошибки,
            // поэтому fromChildNode считается валидным, если ToolTipText пустое
            bool fromNodeIsValid = string.IsNullOrEmpty(fromChildNode?.ToolTipText);

            if (Tag is ActionPack actionPack)
            {
                Text = actionPack.ToString();
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
                if(Parent is ActionPackTreeNode parentActionPackNode)
                    parentActionPackNode.UpdateViewInternal(this);
                //TreeView.Refresh();
            }
            else throw new InvalidCastException($"TreeNode[{Index}]({Tag?.GetType().FullName ?? "NULL"}) does not contains {typeof(ActionPack).FullName}");
        }
        private bool AreInternalsValid(TreeNodeCollection nodes, TreeNode fromChildNode)
        {
            if (nodes?.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    if (ReferenceEquals(node, fromChildNode)
                        && !string.IsNullOrEmpty(fromChildNode?.ToolTipText))
                        return false;
                        
                    if (node.Tag is ActionPack)
                    {
                        if(!AreInternalsValid(node.Nodes, null))
                            return false;
                    }
                    else if (node.Tag is QuesterAction action
                             && !action.IsValid.IsValid)
                        return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="QuesterAction"/>.<see cref="QuesterAction.Conditions"/>
        /// </summary>
        public TreeNode[] ConditionTreeNodes
        {
            get => _conditionTreeNodes ?? (_conditionTreeNodes = (Tag as ActionPack)?.Conditions
                                                                                     .ToTreeNodes()
                                                                                     .ToArray())
                                       ?? Array.Empty<TreeNode>(); 
            set => _conditionTreeNodes = value;
        }
        private TreeNode[] _conditionTreeNodes;
    }
}