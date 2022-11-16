using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using EntityCore.Tools;
using EntityTools.UCC.Conditions;

namespace EntityTools.UCC.Editor.TreeViewCustomization
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="UCCConditionPack"/>
    /// </summary>
    public class UccConditionPackTreeNode : TreeNode, IUccTreeNode<UCCCondition>
    {
        //public UCCAction Data => (UCCAction)Tag;

        public bool AllowChildren => true;

        public UccConditionPackTreeNode(UCCConditionPack conditionPack)
        {
            Tag = conditionPack;
            Text = conditionPack.ToString();
            //BackColor = SystemColors.ControlDark;
            ImageKey = "ConditionList";
            SelectedImageKey = "ConditionList";
            Checked = conditionPack.Locked;
            Nodes.AddRange(conditionPack.Conditions.ToTreeNodes());
        }

        public override object Clone()
        {
            return new UccConditionPackTreeNode((UCCConditionPack)Tag.CreateDeepCopy());
        }

        public UCCCondition ReconstructInternal()
        {
            if (Tag is UCCConditionPack conditionPack)
            {
                conditionPack.Conditions = Nodes.ToUccConditionCollection<ObservableCollection<UCCCondition>>();
                return conditionPack;
            }
            throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCConditionPack)}");
        }

        public void UpdateView()
        {
            if (TreeView is null)
                return;

            if (Tag is UCCConditionPack actionPack)
            {
                if (string.IsNullOrEmpty(actionPack.Name))
                    Text = "ConditionPack";
                else Text = actionPack.Name; //actionPack.ToString();
                Checked = actionPack.Locked;
                TreeView.Refresh();
            }
            else throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCConditionPack)}");
        }
    }
}