using EntityTools.Quester.Editor.Classes;
using EntityTools.Quester.Editor.TreeViewCustomization;
using EntityTools.Tools;
using System;
using System.Windows.Forms;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor
    {
        private class DeleteQuesterCondition : IEditAct
        {
            private readonly QuesterEditor editor;

            private readonly ConditionBaseTreeNode deletingNode;
            private TreeNodePosition path;
            private int deletingNodeIndex = -1;

            private readonly ActionBaseTreeNode actionNode;

            private readonly EditActionEvent onApply;
            private readonly EditActionEvent onRedo;

            public DeleteQuesterCondition(
                QuesterEditor editor, 
                ConditionBaseTreeNode deletingNode, 
                EditActionEvent onApply = null, 
                EditActionEvent onRedo = null)
            {
                this.editor = editor 
                           ?? throw new ArgumentException(nameof(editor));
                this.deletingNode = deletingNode 
                                 ?? throw new ArgumentException(nameof(deletingNode));
                this.actionNode = editor.treeActions.SelectedNode as ActionBaseTreeNode 
                               ?? throw new Exception("No action selected");

                this.onApply = onApply;
                this.onRedo = onRedo;
            }

            public bool Prepare(QuesterEditor editorForm)
            {
                deletingNodeIndex = deletingNode.Index;
                path = deletingNode.Parent?.MakePath();

                return deletingNode != null && path != null;
            }

            public void Apply(QuesterEditor editorForm)
            {
                if (Applied || !IsReady)
                    return;

                var treeViewOwner = deletingNode.TreeView 
                            ?? throw new InvalidOperationException($"Node '{deletingNode.Text}' does not attached to the TreeView.");

                treeViewOwner.BeginUpdate();
                deletingNode.Remove();
                treeViewOwner.EndUpdate();
                
                onApply?.Invoke(this);                
            }

            public void Undo(QuesterEditor editorForm)
            {
                if (!Applied)
                    return;
                
                if(actionNode is null)
                    throw new InvalidOperationException($"Not found an Action for which condition should be restored.");

                if (deletingNode.TreeView != null)
                    throw new InvalidOperationException($"Unable to restore node '{deletingNode.Text}' into the TreeView because it is already attached to the TreeView.");

                editor.SetSelectedActionTo(actionNode);

                var treeConditions = editor.treeConditions;

                // Если path is null, то удаленный узел был в корне дерева
                TreeNodeCollection nodes = path is null
                                         ? treeConditions.Nodes
                                         : treeConditions.Nodes.Select(path)?.Nodes;

                if (nodes is null)
                    throw new Exception("Not found path to restore deleting node.");

                treeConditions.BeginUpdate();
                nodes.Insert(deletingNodeIndex, deletingNode);
                treeConditions.EndUpdate();

                path = null;
                deletingNodeIndex = -1;
                onRedo?.Invoke(this);
                
            }

            public bool IsReady => deletingNode.TreeView != null;

            public bool Applied => deletingNodeIndex >= 0 || path != null;

            public string Label
            {
                get => deletingNode != null 
                    ? $"Delete condition '{deletingNode.Text}'"
                    : "Not Initialized"; 
            }

            public string UndoLabel => deletingNode != null
                ? $"Restore condition '{deletingNode.Text}'"
                : "Not Initialized";
        }
    }
}
