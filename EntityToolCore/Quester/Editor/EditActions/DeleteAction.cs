using System;
using System.Windows.Forms;
using EntityCore.Quester.Editor.TreeViewExtension;

namespace EntityCore.Quester.Editor.EditActions
{
    public class DeleteAction : IQEdit
    {
        private readonly ActionBaseTreeNode node;
        private ActionBaseTreeNode parentNode;
        private TreeView treeView;
        private int index;

        public DeleteAction(ActionBaseTreeNode treeNode)
        {
            node = treeNode ?? throw new ArgumentException(nameof(treeNode));
        }

        public void Apply()
        {
            if (!Applied)
            {
                treeView = node.TreeView;
                parentNode = node.Parent as ActionPackTreeNode;
                index = node.Index;
                node.Remove();
                Applied = true;
                parentNode?.UpdateView(); 
            }
        }

        public void Undo()
        {
            if (Applied)
            {
                if (node.TreeView != null)
                    throw new InvalidOperationException($"Node '{node.Text}' is attached to the Tree.");
                if (parentNode is null)
                    treeView.Nodes.Insert(index, node);
                else parentNode.Nodes.Insert(index, node);
                Applied = false;
                node.UpdateView(); 
            }
        }

        public bool Applied { get; private set; }

        public string Label => $"Delete action '{node.Text}'";

        public string UndoLabel => $"Restore action '{node.Text}'";
    }
}
