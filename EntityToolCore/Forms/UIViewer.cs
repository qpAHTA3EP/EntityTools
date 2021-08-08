//#define DEBUG_TREEBUILDER

using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EntityCore.Forms
{
    public partial class UIViewer : XtraForm //*/Form
    {
        //private static UIViewer uiViewer;

        private string currentUiGenId;

        public UIViewer()
        {
            InitializeComponent();
        }

        public static string GUIRequest(string uiGenId)
        {
            UIViewer @this = new UIViewer();

            if (!string.IsNullOrEmpty(uiGenId) && !string.IsNullOrWhiteSpace(uiGenId))
                @this.currentUiGenId = uiGenId;
            else @this.currentUiGenId = string.Empty;

            if (@this.ShowDialog() == DialogResult.OK)
            {
                UIGen uiGen = @this.tvInterfaces.SelectedNode.Tag as UIGen;
                if (uiGen != null && uiGen.IsValid)
                    return uiGen.Name;
            }
            return string.Empty;
        }

        #region Интерфейс
        #region изменение TreeNode
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
            // Вариант FindAll().GroupBuy() скорее всего медленне чем
            // вариант Where().GroupBuy() 

#if FindAll_GroupBy
            var visibleUIGens = MyNW.Internals.UIManager.AllUIGen.FindAll(ui => (!filterVisibleOnly.Checked || ui.IsValid && ui.IsVisible)
                                                                                 && (string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                                                                                 && !string.IsNullOrEmpty(ui.Name)
                                                                             ).GroupBy(uiGen => ((len = uiGen.Name.IndexOf('_')) > 0) ? uiGen.Name.Substring(0, len) : MISC); 
#else
            Func<UIGen, bool> predicate;
            if(filterVisibleOnly.Checked)
            {
                if(string.IsNullOrEmpty(filterName.Text))
                {
                    predicate = (UIGen ui) => ui.IsValid  && ui.IsVisible
                                                && !string.IsNullOrEmpty(ui.Name);
                }
                else
                {
                    predicate = (UIGen ui) => ui.IsValid && ui.IsVisible
                                                && !string.IsNullOrEmpty(ui.Name) 
                                                && ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(filterName.Text))
                {
                    predicate = (UIGen ui) => ui.IsValid && !string.IsNullOrEmpty(ui.Name);
                }
                else
                {
                    predicate = (UIGen ui) => ui.IsValid && !string.IsNullOrEmpty(ui.Name)
                                                && ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            var visibleUIGens = MyNW.Internals.UIManager.AllUIGen.Where(predicate)
                .GroupBy(ui => ((len = ui.Name.IndexOf('_')) > 0) ? ui.Name.Substring(0, len) : MISC);
#endif
            // Группа элементов интерфейса, представленных в единственном экземпляре и
            // не включенных в другие группы 
            TreeNode miscGroup = new TreeNode(MISC);

            // формирование дерева интерфейсов в tvInterfaces
#if false
            foreach (var uiGenGroup in visibleUIGens)
            {
                if (uiGenGroup.Count() > 1 && uiGenGroup.Key != MISC)
                {
                    TreeNode uiGenGroupNode = new TreeNode(uiGenGroup.Key);
                    foreach (UIGen uiGen in uiGenGroup)
                    {
                        TreeNode uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]") { Tag = uiGen };
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
                        TreeNode uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]") { Tag = uiGen };
                        miscGroup.Nodes.Add(uiGenNode);
                    }
                }

                //FillTreeView(uiGenGroupNode);
            } 
#else
            foreach (var uiGenGroup in visibleUIGens)
            {
                using (var uiGenEnumr = uiGenGroup.GetEnumerator())
                {
                    if(uiGenEnumr.MoveNext())
                    {
                        UIGen uiGenFirst = uiGenEnumr.Current;
                        if(uiGenEnumr.MoveNext())
                        {
                            // В группе uiGenGroup содержится больше одного элемента
                            // Создаем для группы узел дерева
                            TreeNode uiGenGroupNode = new TreeNode(uiGenGroup.Key);

                            TreeNode uiGenNode = new TreeNode($"{uiGenFirst.Name} [{uiGenFirst.Type}]") { Tag = uiGenFirst };
                            uiGenGroupNode.Nodes.Add(uiGenNode);

                            // uiGenEnumr.Current уже указывает на 2 элемент группы
                            // обрабатываем его и пытаемся выбрать следующий
                            do
                            {
                                UIGen uiGen = uiGenEnumr.Current;
                                uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]") { Tag = uiGen };
                                uiGenGroupNode.Nodes.Add(uiGenNode);
                            }
                            while (uiGenEnumr.MoveNext());

                            // Добавляем узел группы в дерево
                            tvInterfaces.Nodes.Add(uiGenGroupNode);
                        }
                        else
                        {
                            // в группе uiGenGroup только один элемент uiGenFirst
                            // добавляем его в узел miscGroup
                            TreeNode uiGenNode = new TreeNode($"{uiGenFirst.Name} [{uiGenFirst.Type}]") { Tag = uiGenFirst };
                            miscGroup.Nodes.Add(uiGenNode);
                        }
                    }
                }
            }
#endif

                // Добавляем узел miscGroup в компонент tvInterfaces
                if (miscGroup.Nodes.Count > 0)
                    tvInterfaces.Nodes.Add(miscGroup);

            if (cbSort.Checked)
                tvInterfaces.Sort();
            #endregion
        }
        private void AddChieldNodes(TreeNode uiGenGroupNode, IGrouping<string, UIGen> uiGenGroup)
        {
            foreach (UIGen uiGen in uiGenGroup)
            {
                TreeNode uiGenNode = new TreeNode($"{uiGen.Name} [{uiGen.Type}]") { Tag = uiGen };
                uiGenGroupNode.Nodes.Add(uiGenNode);
            }
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
            List<UIGen> uiGenList = MyNW.Internals.UIManager.AllUIGen.FindAll(ui => ui.IsValid && (!filterVisibleOnly.Checked || ui.IsVisible)
                                                                              && !string.IsNullOrEmpty(ui.Name)
                                                                              && (string.IsNullOrEmpty(filterName.Text) || ui.Name.IndexOf(filterName.Text, StringComparison.OrdinalIgnoreCase) >= 0));

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
                        EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                    else EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
#else
                    uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer);
#endif
                }

            }

#if DEBUG_TREEBUILDER
            stopwatch.Stop();
            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: The TreeNodes of interfaces was generated in {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Stop();
#endif
            if (cbSort.Checked)
                tvInterfaces.Sort();
#if DEBUG_TREEBUILDER
            stopwatch.Stop();
            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Total time to generated and sort The TreeNodes of interfaces is {stopwatch.ElapsedMilliseconds} ms");
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
            if (uiGen != null && uiGen.IsValid && !string.IsNullOrEmpty(uiGen.Name))
            {
                if (uiGen.Parent.IsValid && !string.IsNullOrEmpty(uiGen.Parent.Name))
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
                         EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                    else EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
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
                            EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                        else EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
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
                        groupNode = treeNodes.Add(groupName, '<' + groupName + '>');
                }

                // Добавляем элемент в найденную группу
                currentNode = groupNode.Nodes.Add(uiGen.Name, $"{uiGen.Name} [{uiGen.Type}]");
                currentNode.Tag = uiGen;
#if DEBUG_TREEBUILDER
                int removeNam = 0;
                if ((removeNam = uiGenList.RemoveAll(ui => ui.Pointer == uiGen.Pointer)) > 0)
                    EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was removed from 'uiGenList' {removeNam} times");
                else EntityToolsLogger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: '{uiGen.Name}' was not removed from 'uiGenList'");
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
            foreach (TreeNode node in treeNodes)
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
        #endregion

        #region Обработчики
        private void event_Refresh(object sender, EventArgs e)
        {
            string selectedKey = (string.IsNullOrEmpty(currentUiGenId)) ? tvInterfaces.SelectedNode?.Text : currentUiGenId;
            //FillTreeView();
            RecurciveTreeBuilder();
            TreeNode selectedNode = (string.IsNullOrEmpty(selectedKey)) ? null : tvInterfaces.Nodes.Find(selectedKey, true).FirstOrDefault();
            event_AfterSelectNode(tvInterfaces, new TreeViewEventArgs(selectedNode));
        }

        private void event_AfterSelectNode(object sender, TreeViewEventArgs e)
        {
            UIGen uiGen = e.Node?.Tag as UIGen;
            if (uiGen != null && uiGen.IsValid)
            {
                pgProperties.SelectedObject = uiGen;
            }
            else pgProperties.SelectedObject = null;
        }

        private void event_filterName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // запрет на ввод любых символов отличных от алфавитно-цифровых
            if (!char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;
        }

        private void event_Execute(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbCommand.Text))
                GameCommands.Execute(tbCommand.Text);
        }

        private void event_Select(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void event_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }  
        #endregion
        #endregion
    }
}
