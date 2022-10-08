using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using EntityCore.Quester.Classes;
using EntityCore.UCC.Classes;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Tools
{
    public static class TreeViewHelper
    {
        #region ConditionPack
        /// <summary>
        /// Тип группирующего quester-условия ConditionPack, реализованного в плагине QuesterAssistant
        /// </summary>
        public static readonly Type QuesterConditionPackType;
        /// <summary>
        /// Функтор доступа к свойству Conditions объекта
        /// </summary>
        private static readonly PropertyAccessor<List<QuesterCondition>> QuesterConditionPackItemsAccessor;

        /// <summary>
        /// Проверка соответствия <paramref name="condition"/> типу ConditionPack
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool IsConditionPack(this QuesterCondition condition) =>
            condition.GetType() == QuesterConditionPackType;
        public static bool IsConditionPack(this object obj) =>
            obj.GetType() == QuesterConditionPackType;

        /// <summary>
        /// Получение значение свойствя Conditions объекта <paramref name="conditionPack"/>
        /// </summary>
        /// <param name="conditionPack"></param>
        /// <returns></returns>
        public static List<QuesterCondition> GetConditions(this QuesterCondition conditionPack)
        {
            if (QuesterConditionPackItemsAccessor?.IsValid == true
                && conditionPack?.IsConditionPack() == true)
            {
                return QuesterConditionPackItemsAccessor.GetValueFrom(conditionPack);
            }

            return new List<Condition>();
        }
        /// <summary>
        /// Присвоение свойству Conditions объекта <paramref name="conditionPack"/> значения <paramref name="conditions"/>
        /// </summary>
        /// <param name="conditionPack"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static bool SetConditions(this QuesterCondition conditionPack, List<QuesterCondition> conditions)
        {
            if (QuesterConditionPackItemsAccessor?.IsValid == true
                && conditionPack?.IsConditionPack() == true)
            {
                QuesterConditionPackItemsAccessor.SetValueTo(conditionPack, conditions);
                return true;
            }

            return false;
        } 
        #endregion

        static TreeViewHelper()
        {
            QuesterConditionPackType = ACTP0Serializer.QuesterConditionPack;

            if (QuesterConditionPackType != null)
                QuesterConditionPackItemsAccessor = QuesterConditionPackType.GetProperty<List<QuesterCondition>>("Conditions");
        }

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка ucc-команд <paramref name="uccActionList"/>
        /// </summary>
        /// <param name="uccActionList"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-команд для соответствующего узла дерева</param>
        /// <returns></returns>
        public static TreeNode[] ToTreeNodes(this List<UCCAction> uccActionList, bool clone = false)
        {
            if (uccActionList?.Count > 0)
            {
                return uccActionList.Select(item =>
                {
                    if (item is UCCActionPack actPack)
                        return (TreeNode)new UccActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
                    return (TreeNode)new UccActionTreeNode(clone ? CopyHelper.CreateDeepCopy(item) : item);
                }).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка quester-команд <paramref name="questerActionList"/>
        /// </summary>
        /// <param name="questerActionList"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-команд для соответствующего узла дерева</param>
        /// <returns></returns>
        public static TreeNode[] ToTreeNodes(this IEnumerable<QuesterAction> questerActionList, bool clone = false)
        {
            if (questerActionList?.Any() == true)
            {
                TreeNode Selector(QuesterAction action)
                {
                    if (action is ActionPack actPack)
                        return new ActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
                    return new ActionTreeNode(clone ? CopyHelper.CreateDeepCopy(action) : action);
                }

                return questerActionList.Select(Selector).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        /// <summary>
        /// Конструирование узла дерева для отображения ucc-команды <paramref name="action"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-команды для узла дерева</param>
        /// <returns></returns>
        public static TreeNode MakeTreeNode(this UCCAction action, bool clone = false)
        {
            if (action is UCCActionPack actPack)
                return new UccActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
            return new UccActionTreeNode(clone ? CopyHelper.CreateDeepCopy(action) : action);
        }
        /// <summary>
        /// Конструирование узла дерева для отображения quester-команды <paramref name="action"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-команды для узла дерева</param>
        /// <returns></returns>
        public static TreeNode MakeTreeNode(this QuesterAction action, bool clone = false)
        {
            if (action is ActionPack actPack)
                return new ActionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(actPack) : actPack);
            return new ActionTreeNode(clone ? CopyHelper.CreateDeepCopy(action) : action);
        }

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка ucc-условий <paramref name="uccConditionList"/>
        /// </summary>
        /// <param name="uccConditionList"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия для соответствующего узла дерева</param>
        /// <returns></returns>
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

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка quester-условий <paramref name="questerConditionList"/>
        /// </summary>
        /// <param name="questerConditionList"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия для соответствующего узла дерева</param>
        /// <returns></returns>
        public static TreeNode[] ToTreeNodes(this IEnumerable<QuesterCondition> questerConditionList, bool clone = false)
        {
            if (questerConditionList?.Any() == true)
            {
                return questerConditionList.Select(item =>
                {
                    if (item.IsConditionPack())
                        return (TreeNode)new ConditionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(item) : item);
                    return (TreeNode)new ConditionTreeNode(clone ? CopyHelper.CreateDeepCopy(item) : item);
                }).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        /// <summary>
        /// Конструирование узла дерева для отображения ucc-условия <paramref name="condition"/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия для узла дерева</param>
        /// <returns></returns>
        public static TreeNode MakeTreeNode(this UCCCondition condition, bool clone = false)
        {
            if (condition is UCCConditionPack conditionPack)
                return new UccConditionPackTreeNode(clone ? CopyHelper.CreateDeepCopy(conditionPack) : conditionPack);
            return new UccConditionTreeNode(clone ? CopyHelper.CreateDeepCopy(condition) : condition);
        }

        /// <summary>
        /// Конструирование узла дерева для отображения quester-условия <paramref name="condition"/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия для узла дерева</param>
        /// <returns></returns>
        public static TreeNode MakeTreeNode(this QuesterCondition condition, bool clone = false)
        {
            var instance = clone ? CopyHelper.CreateDeepCopy(condition) : condition;
            if (condition.IsConditionPack())
                return new ConditionPackTreeNode(instance);
            return new ConditionTreeNode(instance);
        }

        /// <summary>
        /// Конструирования списка ucc-условий из коллекции узлов дерева <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия из соответствующего узла дерева</param>
        /// <returns></returns>
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

        /// <summary>
        /// Конструирования списка quester-условий из коллекции узлов дерева <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия из соответствующего узла дерева</param>
        /// <returns></returns>
        public static List<QuesterCondition> ToQuesterConditionList(this TreeNode[] nodes, bool clone = false)
        {
            if (nodes?.Length > 0)
            {
                var cndList = new List<QuesterCondition>(nodes.Length);
                foreach (var node in nodes)
                {
                    if (node is ITreeNode<QuesterCondition> cndNode)
                    {
                        var cnd = cndNode.ReconstructInternal();
                        if (cnd != null)
                            cndList.Add(clone ? CopyHelper.CreateDeepCopy(cnd) : cnd);
                    }
                }

                return cndList;
            }

            return new List<QuesterCondition>();
        }

        /// <summary>
        /// Конструирования списка ucc-условий из коллекции узлов дерева <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия из соответствующего узла дерева</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Конструирования коллекции ucc-условий из коллекции узлов дерева <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия из соответствующего узла дерева</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Конструирования коллекции quester-условий из коллекции узлов дерева <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия из соответствующего узла дерева</param>
        /// <returns></returns>
        public static TCollection ToQuesterConditionCollection<TCollection>(this TreeNodeCollection nodes, bool clone = false) where TCollection : ICollection<QuesterCondition>, new()
        {
            if (nodes?.Count > 0)
            {
                var cndList = new TCollection();
                foreach (var node in nodes)
                {
                    if (node is ITreeNode<QuesterCondition> cndNode)
                    {
                        var cnd = cndNode.ReconstructInternal();
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
