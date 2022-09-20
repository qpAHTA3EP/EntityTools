using System.Collections.Generic;
using System.Collections.ObjectModel;
using Astral.Logic.UCC.Classes;
using System.Windows.Forms;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// 
    /// </summary>
    interface IUccPriorityTreeNode : IUccTreeNode<TargetPriorityEntry>
    {
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="TargetPriorityEntry"/>
        /// </summary>
        TreeNode[] ConditionTreeNodes { get; set; }
    }
}
