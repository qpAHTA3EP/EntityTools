//#define DEBUG_TREEBUILDER

using DevExpress.XtraEditors;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class UIViewer :  XtraForm //*/Form
    {
        private static UIViewer uiViewer;

        public UIViewer()
        {
            InitializeComponent();
        }

        public static void GetUiGen(UIGen parentUiGen = null)
        {
            if (uiViewer == null)
                uiViewer = new UIViewer();
            uiViewer.Show();
        }

        /// <summary>
        /// Функция построения простого дерева с группировкой по первой части имени (до символа '_')
        /// </summary>
        /// <param name="root"></param>
        private void FillTreeView(TreeNode root = null)
        {
            #region Построение древовидной структуры на базе TreeNode с использованием рекурсии занимает слишком много времени (более часа)
            // Элемент UIGen.Parent в большинстве случаев не заполнен
            //if (root == null)
            //{
            //    root = new TreeNode();
            //    root.Text = "Game Interfaces";
            //}
            //UIGen parentUiGen = root.Tag as UIGen;

            //List<UIGen> uiChilds;

            //if(parentUiGen == null || !parentUiGen.IsValid)
            //    uiChilds = MyNW.Internals.UIManager.AllUIGen.FindAll(ui =>
            //                    (ui.IsValid && (!filterVisibleOnly.Checked || ui.IsVisible)
            //                     && !string.IsNullOrEmpty(ui.Name) && (!string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >=0)
            //                     && (ui.Parent == null || !ui.Parent.IsValid)));
            //else uiChilds = MyNW.Internals.UIManager.AllUIGen.FindAll(ui =>
            //                    (ui.IsValid && (!filterVisibleOnly.Checked || ui.IsVisible) 
            //                     //&& !string.IsNullOrEmpty(ui.Name) && (!string.IsNullOrEmpty(filterName.Text) || ui.Name.Contains(filterName.Text))
            //                     && (ui.Parent != null && ui.Parent.Pointer == parentUiGen.Pointer)));

            //foreach (UIGen uiGen in uiChilds)
            //{
            //    TreeNode child = new TreeNode();
            //    child.Text = uiGen.Name;
            //    child.Tag = uiGen;
            //    root.Nodes.Add(child);
            //    FillTreeView(child);
            //}

            //tvInterfaces.Nodes.Add(root);
            #endregion

            #region Группировка и отображение только видимых элементов
            // Список списоков элементов, сгруппированных по первой части имени (до символа '_')
            // Возникает ошибка, поскольку существуют интерфейсы в названии которых отсутствует 
            tvInterfaces.Nodes.Clear();

            int len;
            string MISC = "<Miscelaneouse>";
            var visibleUIGens = MyNW.Internals.UIManager.AllUIGen.FindAll( ui => (!filterVisibleOnly.Checked || ui.IsValid && ui.IsVisible)
                                                                              && (string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                                                                              && !string.IsNullOrEmpty(ui.Name)
                                                                         ).GroupBy(uiGen => ((len = uiGen.Name.IndexOf('_')) > 0) ? uiGen.Name.Substring(0,  len) : MISC);
            // Группа элементов интерфейса, представленных в единственном экземпляре и
            // не включенных в другие группы 
            TreeNode miscGroup = new TreeNode(MISC);
            
            // формирование дерева интерфейсов в tvInterfaces
            foreach (var uiGenGroup in visibleUIGens)
            {
                if (uiGenGroup.Count() > 1 && uiGenGroup.Key != MISC)
                {
                    TreeNode uiGenGroupNode = new TreeNode(uiGenGroup.Key);
                    foreach (UIGen uiGen in uiGenGroup)
                    {
                        TreeNode uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]");
                        uiGenNode.Tag = uiGen;
                        uiGenGroupNode.Nodes.Add(uiGenNode);
                    }
                    tvInterfaces.Nodes.Add(uiGenGroupNode);
                }
                else
                {                    
                    foreach (UIGen uiGen in uiGenGroup)
                    {
                        // Поскольку группа содержит только 1 элемент
                        // добавляем его в узел miscGroup
                        TreeNode uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]");
                        uiGenNode.Tag = uiGen;
                        miscGroup.Nodes.Add(uiGenNode);
                    }
                }
                
                //FillTreeView(uiGenGroupNode);
            }
            // Добавляем узел miscGroup в компонент tvInterfaces
            if(miscGroup.Nodes.Count > 0)
                tvInterfaces.Nodes.Add(miscGroup);

            if (cbSort.Checked)
                tvInterfaces.Sort();
            #endregion
        }

        /// <summary>
        /// Функция построения дерева tvInterfaces с учетом родительских связей
        /// </summary>
        private void RecurciveTreeBuilder()
        {
            tvInterfaces.Nodes.Clear();
            lblGenTime.Text = string.Empty;
#if DEBUG_TREEBUILDER
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
#endif
            // создание копии списка элементов UIGen
            List<UIGen> uiGenList = MyNW.Internals.UIManager.AllUIGen.FindAll(ui => (!filterVisibleOnly.Checked || ui.IsValid && ui.IsVisible)
                                                                              && (string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                                                                              && !string.IsNullOrEmpty(ui.Name));

            //string misc = "<Miscelaneouse>";
            //TreeNode miscSubgroup = new TreeNode(misc);
            TreeNodeCollection treeNodes = tvInterfaces.Nodes;

            while (uiGenList.Count > 0)
            {
                
                UIGen uiGen = uiGenList.First();

                TreeNode currentNode = InsertTreeNode(uiGen, treeNodes, uiGenList);
                if (currentNode == null)
                {
#if DEBUG_TREEBUILDER
                    int removeNam = 0;

                    if ((removeNam = uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer)) > 0)
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                    else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
#else
                    uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer);
#endif
                }
              
            }
            
#if DEBUG_TREEBUILDER
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: The TreeNodes of interfaces was generated in {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Stop();
#endif
            if (cbSort.Checked)
                tvInterfaces.Sort();
#if DEBUG_TREEBUILDER
            stopwatch.Stop();
            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Total time to generated and sort The TreeNodes of interfaces is {stopwatch.ElapsedMilliseconds} ms");
            lblGenTime.Text = $"Generation time is {stopwatch.ElapsedMilliseconds} ms";
#endif
        }

        /// <summary>
        /// Добавление в <see cref="treeNodes"/>, ссылающегося на <see cref="uiGen"/>
        /// </summary>
        /// <param name="uIGen"></param>
        /// <param name="treeNodes"></param>
        /// <param name="uiGenList"></param>
        /// <returns>Вставленный узел</returns>
        private TreeNode InsertTreeNode(UIGen uiGen, TreeNodeCollection treeNodes, List<UIGen> uiGenList)
        {
            if(uiGen != null && uiGen.IsValid && !string.IsNullOrEmpty(uiGen.Name))
            {
                if(uiGen.Parent.IsValid && !string.IsNullOrEmpty(uiGen.Parent.Name))
                {
                    // uiGen имеет родителя
                    TreeNode parentTreeNode = FindTreeNode(uiGen.Parent, treeNodes);
                    if (parentTreeNode == null)
                    {
                        // Дерево не содержит узла, ссылающегося на uiGen.Parent
                        // поэтому его нужно создать, вызвав рекурсивную функцию
                        parentTreeNode = InsertTreeNode(uiGen.Parent, treeNodes, uiGenList);
                    }
                    TreeNode currentNode = parentTreeNode.Nodes.Add(uiGen.Name, $"{uiGen.Name} [{uiGen.Type}]");
                    currentNode.Tag = uiGen;
#if DEBUG_TREEBUILDER
                    int removeNam = 0;
                    if ((removeNam = uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer)) > 0)
                         Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                    else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
#else
                    uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer);
#endif
                    return currentNode;
                }
                else
                {
                    // uiGen не имеет родителя
                    return InsertTreeNodeinGroup(uiGen, treeNodes, uiGenList);
                }
            }
            return null;
        }

        /// <summary>
        /// По иск в <see cref="treeNodes"/> группы элементов, идобавление в ней <see cref="uiGen"/> 
        /// название группы и определяется исходя из первой части <see cref="uiGen"/>, отделенной символом '_'
        /// При отсутствии группы в <see cref="treeNodes"/> она создается
        /// </summary>
        /// <param name="uiGen"></param>
        /// <param name="treeNodes"></param>
        /// <param name="uiGenList"></param>
        /// <returns></returns>
        private TreeNode InsertTreeNodeinGroup(UIGen uiGen, TreeNodeCollection treeNodes, List<UIGen> uiGenList)
        {
            if (uiGen != null && uiGen.IsValid && !string.IsNullOrEmpty(uiGen.Name))
            {
                string groupName = string.Empty;
                TreeNode groupNode = null;
                TreeNode currentNode = null;

                int len = uiGen.Name.IndexOf('_', 1);

                if (len == -1)
                {
                    // Из имени элемента невозможно выделить название Группы (не содержит символа '_')

                    // Пробуем найти группу, совпадающую с именем элемента
                    groupNode = treeNodes.Find(uiGen.Name, true).FirstOrDefault();
                    if (groupNode == null)
                    {
                        // Группа, совпадающая с названием элемента не найдена
                        // Добавляем элемент в корень
                        currentNode = treeNodes.Add(uiGen.Name, $"{uiGen.Name} [{uiGen.Type}]");
                        currentNode.Tag = uiGen;
#if DEBUG_TREEBUILDER
                        int removeNam = 0;
                        if ((removeNam = uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer)) > 0)
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                        else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
#else
                        uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer);
#endif
                        return currentNode;
                    }
                }
                else
                {
                    // Из имени элемента можно выделить название Группы (содержит символ '_')
                    groupName = uiGen.Name.Substring(0, len).Trim('_');
                    groupNode = treeNodes.Find(groupName, true).FirstOrDefault();
                    if (groupNode == null)
                        // Узел группы отсутствует в дереве treeNodes
                        // Добавляем его
                        groupNode = treeNodes.Add(groupName, '<'+groupName+'>');
                }

                // Добавляем элемент в найденную группу
                currentNode = groupNode.Nodes.Add(uiGen.Name, $"{uiGen.Name} [{uiGen.Type}]");
                currentNode.Tag = uiGen;
#if DEBUG_TREEBUILDER
                int removeNam = 0;
                if ((removeNam = uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer)) > 0)
                    Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                else Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
#else
                uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer);
#endif
                return currentNode;
            }
            return null;
        }

        /// <summary>
        /// поиск узла, ссылающегося на uiGen
        /// </summary>
        private TreeNode FindTreeNode(UIGen uiGen, TreeNodeCollection treeNodes)
        {
            foreach(TreeNode node in treeNodes)
            {
                UIGen curUiGen = node.Tag as UIGen;
                if (curUiGen != null && curUiGen.Pointer == uiGen.Pointer)
                    return node;
                TreeNode childNode = FindTreeNode(uiGen, node.Nodes);
                if (childNode != null)
                    return childNode;
            }
            return null;
        }

        private void Refresh(object sender, EventArgs e)
        {
            string selectedKey = tvInterfaces.SelectedNode?.Text;
            //FillTreeView();
            RecurciveTreeBuilder();
            TreeNode selectedNode = (string.IsNullOrEmpty(selectedKey))? null : tvInterfaces.Nodes.Find(selectedKey, true).FirstOrDefault();
            tvInterfaces_AfterSelect(tvInterfaces, new TreeViewEventArgs(selectedNode));
        }

        private void tvInterfaces_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UIGen uiGen = e.Node?.Tag as UIGen;
            if (uiGen != null && uiGen.IsValid)
            {
                pgProperties.SelectedObject = uiGen;
            }
            else pgProperties.SelectedObject = null;
        }

        private void filterName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // запрет на ввод любых символов отличных от алфавитно-цифровых
            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}
