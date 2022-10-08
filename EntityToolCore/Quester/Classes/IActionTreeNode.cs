using System.Windows.Forms;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;



namespace EntityCore.Quester.Classes
{
    /// <summary>
    /// 
    /// </summary>
    interface IActionTreeNode : ITreeNode<QuesterAction>
    {
        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="QuesterCondition"/>
        /// </summary>
        TreeNode[] ConditionTreeNodes { get; set; }
    }
}
