using System;
using System.Windows.Forms;
using EntityTools.Quester.Editor.TreeViewCustomization;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class MoveQuesterAction : IEditAct
        {
            private readonly ActionBaseTreeNode movingNode;
            private readonly ActionBaseTreeNode targetNode;
            private readonly bool suppressMovingIn;

            private TreeView treeViewOwnedNodeBeforeMoving;
            private ActionBaseTreeNode parentNodeBeforeMoving;
            private int nodeIndexBeforeMoving;

            private readonly EditActionEvent onApply;
            private readonly EditActionEvent onRedo;
            public MoveQuesterAction(ActionBaseTreeNode movingNode, ActionBaseTreeNode targetNode, bool suppressMovingIn, EditActionEvent onApply = null, EditActionEvent onRedo = null)
            {
                this.movingNode = movingNode ?? throw new ArgumentException(nameof(movingNode));
                this.targetNode = targetNode ?? throw new ArgumentException(nameof(targetNode));
                this.suppressMovingIn = suppressMovingIn;
                this.onApply = onApply;
                this.onRedo = onRedo;
            }

            public void Apply()
            {
                if (!Applied)
                {
                    var targetTreeView = targetNode.TreeView ?? throw new InvalidOperationException($"Target node '{targetNode.Text}' does not attached to the TreeView.");

                    parentNodeBeforeMoving = movingNode.Parent as ActionPackTreeNode;
                    nodeIndexBeforeMoving = movingNode.Index;
                    treeViewOwnedNodeBeforeMoving = movingNode.TreeView;

                    targetTreeView.BeginUpdate();
                    movingNode.Remove();
                    if (!suppressMovingIn && targetNode.AllowChildren)
                        targetNode.Nodes.Insert(0, movingNode);
                    else
                    {
                        var targetParent = targetNode.Parent;
                        if (targetParent != null)
                            targetParent.Nodes.Insert(targetParent.Index + 1, movingNode);
                        else targetTreeView.Nodes.Add(movingNode);
                    }
                    targetTreeView.EndUpdate();
                    
                    Applied = true;
                    onApply?.Invoke(this);
                }
            }

            public void Undo()
            {
#if true
                if (Applied)
                {
                    if (movingNode.TreeView is null)
                        throw new InvalidOperationException($"Unable to remove node '{movingNode.Text}' from the TreeView because it does not attached to any.");

                    treeViewOwnedNodeBeforeMoving.BeginUpdate();
                    movingNode.Remove();
                    if (parentNodeBeforeMoving is null)
                        treeViewOwnedNodeBeforeMoving.Nodes.Insert(nodeIndexBeforeMoving, movingNode);
                    else parentNodeBeforeMoving.Nodes.Insert(nodeIndexBeforeMoving, movingNode);
                    treeViewOwnedNodeBeforeMoving.EndUpdate();
                    
                    Applied = false;
                    onRedo?.Invoke(this);
                } 
#else
                throw new NotImplementedException();
#endif
            }

            public bool Applied { get; private set; }

            public string Label => $"Move action '{movingNode.Text}'";

            public string UndoLabel => $"Move back action '{movingNode.Text}'";
        }
    }
}
