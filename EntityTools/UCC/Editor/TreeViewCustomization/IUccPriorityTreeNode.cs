using System.Windows.Forms;
using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    interface IUccPriorityTreeNode : IUccTreeNode<TargetPriorityEntry>
    {
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="TargetPriorityEntry"/>
        /// </summary>
        TreeNode[] ConditionTreeNodes { get; set; }
    }
}
