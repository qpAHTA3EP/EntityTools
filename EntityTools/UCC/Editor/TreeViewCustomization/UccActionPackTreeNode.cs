using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infrastructure.Reflection;
using Astral.Logic.UCC.Classes;
using EntityTools.Tools;
using EntityTools.UCC.Actions;

namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="UCCActionPack"/>
    /// </summary>
    public class UccActionPackTreeNode : TreeNode, IUccActionTreeNode
    {
        public bool AllowChildren => true;

        public UccActionPackTreeNode(UCCActionPack actionPack, bool clone = false)
        {
            if (clone)
                // Специальная перегрузка для UCCActionPack отсутствует, 
                // поэтому требуется преобразование к UCCAction для вызова нужной версии CreateXmlCopy()
                actionPack = (UCCActionPack)((UCCAction)actionPack).CreateXmlCopy();

            UCCActionPack actPack = actionPack;
            Tag = actPack;
            Text = actPack.ToString();
            //BackColor = SystemColors.ControlDark;
            ImageKey = "Box";
            SelectedImageKey = "Box";
            Checked = actPack.Enabled;
            Nodes.AddRange(actPack.Actions.ToUccTreeNodes());
        }

        public override object Clone()
        {
            return new UccActionPackTreeNode((UCCActionPack)ReconstructInternal().CreateXmlCopy());
        }

        public UCCAction ReconstructInternal()
        {
            if (Tag is UCCActionPack actionPack)
            {
                var actList = new List<UCCAction>(Nodes.Count);
                foreach (TreeNode node in Nodes)
                {
                    if (node is IUccTreeNode<UCCAction> uccNode)
                        actList.Add(uccNode.ReconstructInternal());
                }
                actionPack.Actions = actList;

                if(_conditionTreeNodes != null)
                    actionPack.Conditions = _conditionTreeNodes.ToUccConditionList();

                return actionPack;
            }
            throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCActionPack)}");
        }

        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is UCCActionPack actionPack)
            {
                Text = actionPack.ToString();
                Checked = actionPack.Enabled;
                TreeView.Refresh();
            }
            else throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCActionPack)}");
        }

        /// <summary>
        /// Список узлов дерева, соответствующих условиям <see cref="UCCAction"/>.<see cref="UCCAction.Conditions"/>
        /// </summary>
        public TreeNode[] ConditionTreeNodes
        {
            get => _conditionTreeNodes ?? (_conditionTreeNodes = (Tag as UCCActionPack)?.Conditions.ToTreeNodes().ToArray()) ?? Array.Empty<TreeNode>(); 
            set => _conditionTreeNodes = value;
        }
        private TreeNode[] _conditionTreeNodes;
    }
}