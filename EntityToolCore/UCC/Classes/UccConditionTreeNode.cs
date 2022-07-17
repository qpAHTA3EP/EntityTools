﻿using AcTp0Tools.Reflection;
using Astral.Logic.UCC.Actions;
using Astral.Logic.UCC.Classes;
using EntityTools.UCC.Actions;
using System;
using System.Windows.Forms;

namespace EntityCore.UCC.Classes
{
    /// <summary>
    /// Узел дерева, отображающий <see cref="UCCCondition"/>
    /// </summary>
    public class UccConditionTreeNode : TreeNode, IUccTreeNode<UCCCondition>
    {
        //public UCCAction Data => (UCCAction)Tag;

        public bool AllowChildren => false;

        public UccConditionTreeNode(UCCCondition condition)
        {
            Tag = condition;
            var txt = condition.ToString();
            if (txt != condition.GetType().FullName)
                Text = txt;
            else Text = $"{condition.Target} [{condition.Tested}] {condition.Sign} '{condition.Value}'";
            SelectIcon(condition);
            Checked = condition.Locked;
        }

        private void SelectIcon(UCCCondition condition)
        {
            switch (condition)
            {
                default:
                    ImageKey = "Condition";
                    SelectedImageKey = "Condition";
                    break;
            }
        }
        public void UpdateView()
        {
            if (Tag is UCCCondition condition)
            {
                var txt = condition.ToString();
                if (txt != condition.GetType().FullName)
                    Text = txt;
                else Text = $"{condition.Target} [{condition.Tested}] {condition.Sign} '{condition.Value}'";
                Checked = condition.Locked;
                SelectIcon(condition);
                TreeView.Refresh();
            }
            else throw new Exception($"TreeNode[{Index}] does not contains {nameof(UCCCondition)}");
        }

        public UCCCondition ReconstructInternal()
        {
            return Tag as UCCCondition;
        }

        public override object Clone()
        {
            return new UccConditionTreeNode(CopyHelper.CreateDeepCopy((UCCCondition)Tag));
        }
    }
}
