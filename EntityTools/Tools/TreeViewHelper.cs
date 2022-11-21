using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using Astral.Quester.Classes.Conditions;
using DevExpress.XtraEditors;
using EntityTools.Quester.Conditions;
using EntityTools.Quester.Editor.TreeViewCustomization;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using EntityTools.UCC.Editor.TreeViewCustomization;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Tools
{
    public static class TreeViewHelper
    {
        #region QuesterAssistant.ConditionPack access and manipulation
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

            return new List<QuesterCondition>();
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

        public static bool IsPushProfileToStackAndLoad(this QuesterAction action) =>
            action.GetType() == ACTP0Serializer.PushProfileToStackAndLoad;


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
        public static TreeNode[] ToUccTreeNodes(this List<UCCAction> uccActionList, bool clone = false)
        {
            if (uccActionList?.Count > 0)
            {
                TreeNode Selector(UCCAction action)
                {
                    if (clone)
                        action = action.CreateXmlCopy();

                    if (action is UCCActionPack actPack)
                        return (TreeNode)new UccActionPackTreeNode(actPack);
                    return (TreeNode)new UccActionTreeNode(action);

                }

                return uccActionList.Select(Selector).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка quester-команд <paramref name="questerActionList"/>
        /// </summary>
        /// <param name="clone">Флаг принудительного создания копии ucc-команд для соответствующего узла дерева</param>
        /// <returns></returns>
        public static TreeNode[] ToTreeNodes(this QuesterProfileProxy profile, bool clone = false)
        {
            var actions = profile.Actions;
            if (actions.Any())
            {
                TreeNode Selector(QuesterAction action)
                {
                    if (clone)
                        action = action.CreateXmlCopy();

                    if (action is ActionPack actPack)
                        return new ActionPackTreeNode(profile, actPack);
                    return new ActionTreeNode(profile, action);
                }

                return actions.Select(Selector).ToArray();
            }

            return Array.Empty<TreeNode>();
        }
        public static TreeNode[] ToTreeNodes(QuesterProfileProxy profile, IEnumerable<QuesterAction> questerActionList, bool clone = false)
        {
            if (questerActionList?.Any() == true)
            {
                TreeNode Selector(QuesterAction action)
                {
                    if (clone)
                        action = action.CreateXmlCopy();

                    if (action is ActionPack actPack)
                        return new ActionPackTreeNode(profile, actPack);
                    if (action.IsPushProfileToStackAndLoad())
                        return new ActionPushProfileToStackAndLoadTreeNode(profile, action);
                    return new ActionTreeNode(profile, action);
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
            if (clone)
                action = action.CreateXmlCopy();

            if (action is UCCActionPack actPack)
                return new UccActionPackTreeNode(actPack);
            return new UccActionTreeNode(action);
        }
        /// <summary>
        /// Конструирование узла дерева для отображения quester-команды <paramref name="action"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-команды для узла дерева</param>
        /// <returns></returns>
        public static ActionBaseTreeNode MakeTreeNode(this QuesterAction action, QuesterProfileProxy profile, bool clone = false)
        {
            if (clone)
                action = action.CreateXmlCopy();

            if (action is ActionPack actionPack)
                return new ActionPackTreeNode(profile, actionPack);
            if(action.IsPushProfileToStackAndLoad())
                return new ActionPushProfileToStackAndLoadTreeNode(profile, action);
            return new ActionTreeNode(profile, action);
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
                TreeNode Selector(UCCCondition condition)
                {
                    if (clone)
                        condition = condition.CreateXmlCopy();

                    if (condition is UCCConditionPack conditionPack)
                        return (TreeNode)new UccConditionPackTreeNode(conditionPack);
                    return (TreeNode)new UccConditionTreeNode(condition);
                }

                return uccConditionList.Select(Selector).ToArray();
            }

            return Array.Empty<TreeNode>();
        }

        /// <summary>
        /// Конструирование коллекции узлов дерева для отображения списка quester-условий <paramref name="questerConditionList"/>
        /// </summary>
        /// <param name="questerConditionList"></param>
        /// <param name="profile"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия для соответствующего узла дерева</param>
        /// <returns></returns>
        public static TreeNode[] ToTreeNodes(
            this IEnumerable<Condition> questerConditionList,
            QuesterProfileProxy profile, 
            bool clone = false)
        {
            return questerConditionList.Select(item => MakeTreeNode(item, profile, clone)).Cast<TreeNode>().ToArray();
        }

        /// <summary>
        /// Конструирование узла дерева для отображения ucc-условия <paramref name="condition"/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="clone">Флаг принудительного создания копии ucc-условия для узла дерева</param>
        /// <returns></returns>
        public static TreeNode MakeTreeNode(this UCCCondition condition, bool clone = false)
        {
            if (clone)
                condition = condition.CreateXmlCopy();

            if (condition is UCCConditionPack conditionPack)
                return new UccConditionPackTreeNode(conditionPack);
            return new UccConditionTreeNode(condition);
        }

        /// <summary>
        /// Конструирование узла дерева для отображения quester-условия <paramref name="condition"/>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="profile"></param>
        /// <param name="clone">Флаг принудительного создания копии quester-условия для узла дерева</param>
        /// <returns></returns>
        public static ConditionBaseTreeNode MakeTreeNode(
            this Condition condition, 
            QuesterProfileProxy profile,
            bool clone = false)
        {
            if (clone)
                condition = condition.CreateXmlCopy();

            switch (condition)
            {
                case ConditionPack conditionPack:
                    return new ConditionPackTreeNode(profile, conditionPack);
                case IsInCustomRegion isInCustomRegion:
                    return new ConditionIsInCustomRegionTreeNode(profile, isInCustomRegion);
                case IsInCustomRegionSet isInCustomRegionSet:
                    return new ConditionIsInCustomRegionSetTreeNode(profile, isInCustomRegionSet);
                default:
                    return new ConditionTreeNode(profile, condition);
            }
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
                UCCCondition Selector(TreeNode node)
                {
                    if (node is IUccTreeNode<UCCCondition> uccConditionNode)
                    {
                        var condition = uccConditionNode.ReconstructInternal();
                        if (condition != null)
                            return clone 
                                 ? condition.CreateXmlCopy() 
                                 : condition;
                    }

                    return null;
                }

                return nodes.Select(Selector).Where(item => item != null).ToList();
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
                QuesterCondition Selector(TreeNode node)
                {
                    if (node is ConditionBaseTreeNode conditionNode)
                    {
                        var condition = conditionNode.ReconstructInternal();
                        if (condition != null)
                            return clone
                                 ? condition.CreateXmlCopy()
                                 : condition;
                    }

                    return null;
                }

                return nodes.Select(Selector).Where(item => item != null).ToList();
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
#if false
                UCCCondition Selector(TreeNode node)
                {
                    if (node is IUccTreeNode<UCCCondition> conditionNode)
                    {
                        var condition = conditionNode.ReconstructInternal();
                        if (condition != null)
                            return clone
                                ? condition.CreateXmlCopy()
                                : condition;
                    }

                    return null;
                }

                return nodes.Select(Selector).Where(item => item != null).ToList();
#else
                var cndList = new List<UCCCondition>(nodes.Count);
                foreach (var cndNode in nodes)
                {
                    if (cndNode is IUccTreeNode<UCCCondition> uccCndNode)
                    {
                        var cnd = uccCndNode.ReconstructInternal();
                        if (cnd != null)
                            cndList.Add(clone ? cnd.CreateXmlCopy() : cnd);
                    }
                }

                return cndList; 
#endif
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
                            cndList.Add(clone ? cnd.CreateXmlCopy() : cnd);
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
                            cndList.Add(clone ? cnd.CreateXmlCopy() : cnd);
                    }
                }

                return cndList;
            }

            return new TCollection();
        }

        public static ActionBaseTreeNode FindActionNode(this TreeNodeCollection nodes, Guid actionId)
        {
            foreach (ActionBaseTreeNode node in nodes)
            {
                if (node.Content.ActionID == actionId)
                {
                    node.TreeView.SelectedNode = node;
                    node.EnsureVisible();
                    return node;
                }

                if (node.AllowChildren
                    && node.Nodes.Count > 0)
                {
                    var innerNode = FindActionNode(node.Nodes, actionId);
                    if (innerNode != null)
                        return innerNode;
                }
            }
            return null;
        }


        public class Callback
        {
            public delegate void SimpleCallback();
            private SimpleCallback _callback = DoNothing;
            private static void DoNothing() { }
            public void Invoke() => _callback();

            public void Set(SimpleCallback callback)
            {
                if (callback is null)
                    _callback = DoNothing;
                else
                {
                    var target = callback.Target;
                    if (target is TreeNode node)
                    {
                        _callback = () =>
                        {
                            node.TreeView?.BeginUpdate();
                            callback();
                            node.TreeView?.EndUpdate();
                        };
                    }
                    else if (target is ListBoxControl listBox)
                    {
                        _callback = () =>
                        {
                            listBox.BeginUpdate();
                            callback();
                            listBox.EndUpdate();
                        };
                    }
                    else _callback = callback;
                }
            }
        }

        public class Callback<T>
        {
            private ParameterizedCallback _callback = DoNothing;
            private static void DoNothing(T _) { }

            public delegate void ParameterizedCallback(T input);

            public void Invoke(T input) => _callback(input);

            public void Set(ParameterizedCallback callback)
            {
                if (callback is null)
                    _callback = DoNothing;
                else
                {
                    var target = callback.Target;
                    if (target is TreeNode node)
                    {
                        _callback = input =>
                        {
                            node.TreeView?.BeginUpdate();
                            callback(input);
                            node.TreeView?.EndUpdate();
                        };
                    }
                    else if (target is ListBoxControl listBox)
                    {
                        _callback = input =>
                        {
                            listBox.BeginUpdate();
                            callback(input);
                            listBox.EndUpdate();
                        };
                    }
                    else _callback = callback;
                }
            }
        }
    }
}
