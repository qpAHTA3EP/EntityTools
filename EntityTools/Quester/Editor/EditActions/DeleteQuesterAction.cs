using System;
using System.Windows.Forms;
using EntityTools.Quester.Editor.TreeViewCustomization;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteQuesterAction : IQEdit
        {
            private readonly ActionBaseTreeNode deletingNode;
            private ActionBaseTreeNode parentNodeBeforeDeleting;
            
            private TreeView treeViewOwner;
            private int nodeIndexBeforeDeleting;
            
            private readonly EditActionEvent onApply;
            private readonly EditActionEvent onRedo;
            public DeleteQuesterAction(ActionBaseTreeNode deletingNode = null, EditActionEvent onApply = null, EditActionEvent onRedo = null)
            {
                this.deletingNode = deletingNode ?? throw new ArgumentException(nameof(deletingNode));
                this.onApply = onApply;
                this.onRedo = onRedo;
            }

            public void Apply()
            {
                if (!Applied)
                {
                    treeViewOwner = deletingNode.TreeView ?? throw new InvalidOperationException($"Node '{deletingNode.Text}' does not attached to the TreeView.");

                    parentNodeBeforeDeleting = deletingNode.Parent as ActionPackTreeNode;
                    nodeIndexBeforeDeleting = deletingNode.Index;

                    treeViewOwner.BeginUpdate();
                    deletingNode.Remove();
                    treeViewOwner.EndUpdate();
                
                    Applied = true;
                    onApply?.Invoke(this);
                }
            }

            public void Undo()
            {
                if (Applied
                    && treeViewOwner != null)
                {
                    if (deletingNode.TreeView != null)
                        throw new InvalidOperationException($"Unable to restore node '{deletingNode.Text}' into the TreeView because it is already attached to the TreeView.");
                    
                    treeViewOwner.BeginUpdate();
                    if (parentNodeBeforeDeleting is null)
                        treeViewOwner.Nodes.Insert(nodeIndexBeforeDeleting, deletingNode);
                    else parentNodeBeforeDeleting.Nodes.Insert(nodeIndexBeforeDeleting, deletingNode);
                    treeViewOwner.EndUpdate();

                    Applied = false;
                    onRedo?.Invoke(this);
                }
            }

            public bool Applied { get; private set; }

            public string Label => $"Delete action '{deletingNode.Text}'";

            public string UndoLabel => $"Restore action '{deletingNode.Text}'";
        }
    }
}
