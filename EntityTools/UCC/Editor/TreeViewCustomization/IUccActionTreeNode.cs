using System.Windows.Forms;
using Astral.Logic.UCC.Classes;

namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    interface IUccActionTreeNode : IUccTreeNode<UCCAction>
    {
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="UCCAction"/>
        /// </summary>
        TreeNode[] ConditionTreeNodes { get; set; }
    }
}
