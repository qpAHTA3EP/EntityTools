using System.Collections.Generic;
using System.Collections.ObjectModel;
using Astral.Logic.UCC.Classes;
using System.Windows.Forms;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// 
    /// </summary>
    interface IUccActionTreeNode : IUccTreeNode<UCCAction>
    {
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="UCCAction"/>
        /// </summary>
        TreeNode[] ConditionTreeNodes { get; set; }
    }
}
