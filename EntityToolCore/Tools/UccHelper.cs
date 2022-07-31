using AcTp0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using EntityCore.UCC.Classes;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EntityCore.Tools
{
    public static class UccHelper
    {
        public static TreeNode[] ToTreeNodes(this List<UCCAction> uccActionList, bool clone = false)
        {
            if (uccActionList?.Count > 0)
            {
                return uccActionList.Select(act =>
                {
                    if (act is UCCActionPack actPack)
                        return (TreeNode)new UccActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
                    return (TreeNode)new UccActionTreeNode(clone ? CopyHelper.CreateDeepCopy(act) : act);
                }).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        public static TreeNode MakeTreeNode(this UCCAction action, bool clone = false)
        {
            if (action is UCCActionPack actPack)
                return new UccActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
            return new UccActionTreeNode(clone ? CopyHelper.CreateDeepCopy(action) : action);
        }
        public static TreeNode[] ToTreeNodes(this IEnumerable<UCCCondition> uccConditionList, bool clone = false)
        {
            if (uccConditionList?.Any() == true)
            {
                return uccConditionList.Select(act =>
                {
                    if (act is UCCConditionPack conditionPack)
                        return (TreeNode)new UccConditionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(conditionPack) : conditionPack);
                    return (TreeNode)new UccConditionTreeNode(clone ? CopyHelper.CreateDeepCopy(act) : act);
                }).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        public static TreeNode MakeTreeNode(this UCCCondition condition, bool clone = false)
        {
            if (condition is UCCConditionPack conditionPack)
                return new UccConditionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(conditionPack) : conditionPack);
            return new UccConditionTreeNode(clone ? CopyHelper.CreateDeepCopy(condition) : condition);
        }

        public static List<UCCCondition> ToUccConditionList(this TreeNode[] nodes, bool clone = false)
        {
            if (nodes?.Length > 0)
            {
                var cndList = new List<UCCCondition>(nodes.Length);
                foreach (var cndNode in nodes)
                {
                    if (cndNode is IUccTreeNode<UCCCondition> uccCndNode)
                    {
                        var cnd = uccCndNode.ReconstructInternal();
                        if (cnd != null)
                            cndList.Add(clone ? CopyHelper.CreateDeepCopy(cnd) : cnd);
                    }
                }

                return cndList;
            }

            return new List<UCCCondition>();
        }

        public static List<UCCCondition> ToUccConditionList(this TreeNodeCollection nodes, bool clone = false)
        {
            if (nodes?.Count > 0)
            {
                var cndList = new List<UCCCondition>(nodes.Count);
                foreach (var cndNode in nodes)
                {
                    if (cndNode is IUccTreeNode<UCCCondition> uccCndNode)
                    {
                        var cnd = uccCndNode.ReconstructInternal();
                        if (cnd != null)
                            cndList.Add(clone ? CopyHelper.CreateDeepCopy(cnd): cnd);
                    }
                }

                return cndList;
            }

            return new List<UCCCondition>();
        }
        public static TCollection ToUccConditionCollection<TCollection>(this TreeNodeCollection nodes, bool clone = false) where TCollection : ICollection<UCCCondition>, new()
        {
            if (nodes?.Count > 0)
            {
                var cndList = new TCollection();
                foreach (var cndNode in nodes)
                {
                    if (cndNode is IUccTreeNode<UCCCondition> uccCndNode)
                    {
                        var cnd = uccCndNode.ReconstructInternal();
                        if (cnd != null)
                            cndList.Add(clone ? CopyHelper.CreateDeepCopy(cnd) : cnd);
                    }
                }

                return cndList;
            }

            return new TCollection();
        }
    }
}
