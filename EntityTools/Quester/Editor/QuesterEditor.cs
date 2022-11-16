﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using AStar;
using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Classes.Actions;
using Astral.Quester.Classes.Conditions;
using Astral.Quester.Forms;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EntityCore.Forms;
using EntityCore.Tools;
using EntityTools.Annotations;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Patches.Mapper;
using EntityTools.Quester.Editor.Classes;
using EntityTools.Quester.Editor.TreeViewCustomization;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;

using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityTools.Quester.Editor
{
    public partial class QuesterEditor : XtraForm
    {
        //BUG Копирование команд/условие через CopyHelper приводит к копированию Engine, что может приводить к ошибкам, поэтому копирование нужно реализовать путем копирования публичных свойств, имеющий сеттеры,

        /// <summary>
        /// Редактируемый профиль
        /// </summary>
        public QuesterProfileProxy Profile => profile;
        private readonly ProfileProxy profile;
        private readonly Guid startActionId;
        private Stack<IQEdit> undoStack = new Stack<IQEdit>();
        private Stack<IQEdit> redoStack = new Stack<IQEdit>();

        // Функтор, выполняемый при изменении pgProperties
        private System.Action propertyChangedCallback;

        // Функтор, выполняемый при изменении списка treeConditions
        private Action<TreeNodeCollection> conditionListChangedCallback;

        // Скопированная команда
        private static QuesterAction actionCache;

        // Скопированная условие
        private static QuesterCondition conditionCache;

        #region Инициализация
#if false   //Попытка подмены PropertyDescriptor'a для установки редактора свойств типа CustomRegionCollection
        private static readonly List<TypeDescriptionProvider> descriptorProvider = new List<TypeDescriptionProvider>();
        static QuesterEditor()
        {
            // Ни один из испробованных способов не привел к вызову редактора CustomRegionSetEditor,
            // определенного в EntityCore. Вместе с тем успешно прикреплялся редактор CustomRegionCollectionEditor,
            // определенный в EntityTools, однако, его можно указать непосредственно в атрибутах свойств нужных классов
            // и нет необходимости использовать механизмы TypeDescriptor
#if true
            var tCustomRegionCollection = typeof(CustomRegionCollection);
            var editor = new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor));
            bool PropertyPredicate(PropertyDescriptor pd) => pd.PropertyType == tCustomRegionCollection;
            typeof(IsInCustomRegionSet).DecoratePropertyWithAttribute(PropertyPredicate, editor);
            typeof(MoveToEntity).DecoratePropertyWithAttribute(PropertyPredicate, editor);
#elif false
            //var cond = new IsInCustomRegionSet();
            var type = typeof(IsInCustomRegionSet);
            var tEditor = typeof(CustomRegionSetEditor);
#elif false
            // Декорирование свойств типов для вызова корректного редактора
            var tCustomRegionCollection = typeof(CustomRegionCollection);
            //var editorAttribute = new EditorAttribute(typeof(CustomRegionSetEditor),
            //                                         typeof(UITypeEditor));
            foreach (Type type in ACTP0Serializer.QuesterTypes)
            {
                bool shouldOverrideProperty = false;
                PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(type).GetTypeDescriptor(type));
                // iterate through properties in the supplied object/type
                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(type))
                {
                    // for every property that complies to our criteria
                    if (pd.PropertyType == tCustomRegionCollection)
                    {
                        // we first construct the custom PropertyDescriptor with the TypeDescriptor's built-in capabilities
                        var newPD = TypeDescriptor.CreateProperty(
                                tCustomRegionCollection, // or just _settings, if it's already a type
                                pd,                      // base property descriptor to which we want to add attributes
                                                         // The PropertyDescriptor which we'll get will just wrap that
                                                         // base one returning attributes we need.
                                new EditorAttribute(typeof(CustomRegionCollectionEditor),//typeof(CustomRegionSetEditor),//
                                    typeof(UITypeEditor)));

                        // and then we tell our new PropertyOverridingTypeDescriptor to override that property
                        ctd.OverrideProperty(newPD);
                        shouldOverrideProperty = true;
                    }
                }

                // then we add new descriptor provider that will return our descriptor instead of default
                if (shouldOverrideProperty)
                {
                    var descriptor = new TypeDescriptorOverridingProvider(ctd);
                    descriptorProvider.Add(descriptor);
                    TypeDescriptor.AddProviderTransparent(descriptor, type);
                }
            } 
#endif
        } 
#endif

        public QuesterEditor(Profile profile, string fileName, Guid actionId)
        {
            InitializeComponent();

            this.profile = profile is null 
                ? new ProfileProxy()
                : new ProfileProxy(profile, fileName);

            var settingFile = FileTools.QuesterEditorSettingsFile;
            if (File.Exists(settingFile))
            {
                dockManager.RestoreLayoutFromXml(settingFile);
            }

            bool isChecked = panHotSpots.CustomHeaderButtons["Edit Coordinates"].IsChecked == true;
            gridViewHotSpots.OptionsBehavior.Editable = isChecked;

            startActionId = actionId;
        }

        /// <summary>
        /// Редактирование quester-профиля <paramref name="profile"/>
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="param">Дополнительные аргументы:<br/>
        /// - адресная строка файла редактируемого профиля;<br/>
        /// - флаг модального режима</param>
        /// <returns></returns>
        public static bool Edit(Profile profile = null, params object[] param)
        {
            string profileName = string.Empty;
            bool modal = false;
            Guid actionId = Guid.Empty;
            if (param != null)
            {
                for (int i = 0; i < 2 && i < param.Length; i++)
                {
                    switch (param[i])
                    {
                        case string str:
                            profileName = str;
                            break;
                        case bool b:
                            modal = b;
                            break;
                        case QuesterAction action:
                            actionId = action.ActionID;
                            break;
                        case Guid guid:
                            actionId = guid;
                            break;
                    }
                }
            }

            var editor = new QuesterEditor(profile, profileName, actionId);
            if (modal)
                return editor.ShowDialog() == DialogResult.OK;
            editor.Show();
            return true;
        }

        /// <summary>
        /// Отображение профиля <see cref="Profile"/> в окне редактора.
        /// </summary>
        private void UI_fill()
        {
            if (profile is null)
            {
                UI_reset();
                return;
            }

            // Отображение набора команд
            treeActions.Nodes.Clear();
            if (profile.Actions.Any())
                treeActions.Nodes.AddRange(profile.ToTreeNodes(true));

            treeConditions.Nodes.Clear();

            listBlackList.DataSource = profile.BlackList;
            listCustomRegions.DataSource = profile.CustomRegions;
            listVendor.DataSource = profile.Vendors;

            gridHotSpots.DataSource = null;
            
            pgSettings.SelectedObject = profile;
            pgProperties.SelectedObject = null;

            propertyChangedCallback = null;
            conditionListChangedCallback = null;

            UpdateWindowCaption();
            ResetFilter();
        }

        /// <summary>
        /// Очистка содержимого списков и деревьев
        /// </summary>
        private void UI_reset()
        {
            treeActions.Nodes.Clear();
            treeConditions.Nodes.Clear();

            listBlackList.DataSource = null;
            listCustomRegions.DataSource = null;
            listVendor.DataSource = null;

            gridHotSpots.DataSource = null;

            pgProperties.SelectedObject = null;
            pgSettings.SelectedObject = null;

            propertyChangedCallback = null;
            conditionListChangedCallback = null;

            UpdateWindowCaption();
            ResetFilter();
        }

        private void UpdateWindowCaption()
        {
            if (profile is null
                || string.IsNullOrEmpty(profile.FileName))
                Text = "* New profile";
            else Text = (profile.Saved ? string.Empty : "* ")
                       + profile.FileName;
        }

        private void handler_Form_Load(object sender, EventArgs e)
        {
            // Подмена метаданных для IsInCustomRegion
            // https://stackoverflow.com/questions/46099675/can-you-how-to-specify-editor-at-runtime-for-propertygrid-for-pcl
            var provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(IsInCustomRegion),
                typeof(IsInCustomRegionMetadataType));
            TypeDescriptor.AddProvider(provider, typeof(IsInCustomRegion));

            provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(LoadProfile),
                typeof(LoadProfileMetadataType));
            TypeDescriptor.AddProvider(provider, typeof(LoadProfile));

            var tPushProfileToStackAndLoad = ACTP0Serializer.PushProfileToStackAndLoad;
            if (tPushProfileToStackAndLoad != null)
            {
                provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                    tPushProfileToStackAndLoad,
                    typeof(PushProfileToStackAndLoadMetadataType));
                TypeDescriptor.AddProvider(provider, tPushProfileToStackAndLoad);
            }

            UI_fill();

            if (startActionId != Guid.Empty)
            {
                var node = treeActions.Nodes.FindActionNode(startActionId);
                if(node != null)
                {
                    treeActions.SelectedNode = node;
                    treeActions.SelectedNode.EnsureVisible();
                }
            }
        }

        private void handler_Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (!profile.Saved)
            {
                switch (XtraMessageBox.Show("Profile was modified but did not saved!\n" +
                                            "Would you like to save it ?\n" +
                                            "Press 'Cancel' button to continue profile edition.", "Caution!",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        SaveProfile();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            dockManager.SaveLayoutToXml(FileTools.QuesterEditorSettingsFile);
            mapperForm?.Close();
        }
        #endregion




        #region TreeViewNode Drag & Drop
        private void handler_TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            propertyChangedCallback?.Invoke();
            conditionListChangedCallback?.Invoke(treeConditions.Nodes);

            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else if (e.Button == MouseButtons.Right)
            {
                DoDragDrop(e.Item, DragDropEffects.Copy);
            }
        }

        private void handler_TreeView_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                // Извлечение перетаскиваемого узла
                TreeNode draggedNode = (TreeNode) (e.Data.GetData(typeof(ActionTreeNode))
                                                   ?? e.Data.GetData(typeof(ActionPackTreeNode))
                                                   ?? e.Data.GetData(typeof(ConditionPackTreeNode))
                                                   ?? e.Data.GetData(typeof(ConditionTreeNode))
                                                   ?? e.Data.GetData(typeof(TreeNode)));

                // Запрет копировая/перепещения узлов между разными деревьями
                if (!ReferenceEquals(treeView, draggedNode.TreeView))
                    return;

                // Определение координат курсора мыши в пространстве treeView
                Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

                // Определение целевого узла дерева, расположенного под курсором мыши
                TreeNode targetNode = treeView.GetNodeAt(targetPoint);

                // Проверка нажтия кнопки ALT
                bool altHolded = (e.KeyState & 32) != 0;

                // Проверяем что перемещаемый узел не является родительским по отношению к целевому узлу,
                // расположенному под курсором мыши
                if (targetNode != null)
                {
                    if (draggedNode.Equals(targetNode)
                        || ContainsNode(draggedNode, targetNode))
                        return;

                    treeView.BeginUpdate();
                    if (targetNode is ActionPackTreeNode actionPackNode
                        && !altHolded)
                    {
                        // Если удерживается ALT, то dtaggedNode помещается после actionPack'a

                        // Целевой узел является ActionPack'ом
                        // Пепремещаем узел во нутрь целевого узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            var parent = draggedNode.Parent;
                            draggedNode.Remove();
                            if(parent is ActionPackTreeNode parentActionPackTreeNode)
                                parentActionPackTreeNode.UpdateView();
                            actionPackNode.Nodes.Insert(0, draggedNode);
                            profile.Saved = false;
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Insert(0, (TreeNode) draggedNode.Clone());
                            profile.Saved = false;
                        }

                        actionPackNode.UpdateView();

                        // Раскрываем целевой узел дерева, в который перемещен/скопирован перетаскиваемый узел
                        targetNode.Expand();
                    }
                    else if (targetNode is ConditionPackTreeNode conditionPackNode
                             && !altHolded)
                    {
                        // Если удерживается ALT, то dtaggedNode помещается после actionPack'a

                        // Целевой узел является ConditionPack'ом
                        // Пепремещаем узел во нутрь целевого узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            draggedNode.Remove();
                            conditionPackNode.Nodes.Insert(0, draggedNode);
                            profile.Saved = false;
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Insert(0, (TreeNode) draggedNode.Clone());
                            profile.Saved = false;
                        }

                        // Раскрываем целевой узел дерева, в который перемещен/скопирован перетаскиваемый узел
                        targetNode.Expand();
                    }
                    else
                    {
                        // Выбираем коллекцию узлов, в которую нужно добавить перетаскиваемый узел
                        TreeNodeCollection treeNodeCollection = targetNode.Parent?.Nodes ?? treeView.Nodes;

                        // Если нажата клавиша ALT, вставляем ПЕРЕД targetNode.
                        // В противном случае - после targetNode.
                        //var targetInd = targetNode.Index + (e.KeyState & 32) == 32 ? 0 : 1;// Проверяем нажатие клавиши ALT

                        // Перемещение узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            var parent = draggedNode.Parent;
                            draggedNode.Remove();
                            if (parent is ActionPackTreeNode draggedParentTreeNode)
                                draggedParentTreeNode.UpdateView();
                            treeNodeCollection.Insert(targetNode.Index + 1, draggedNode);
                            profile.Saved = false;
                        }
                        // Копирование узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            treeNodeCollection.Insert(targetNode.Index + 1, (TreeNode)draggedNode.Clone());
                            profile.Saved = false;
                        }
                        if (targetNode.Parent is ActionPackTreeNode targetParentTreeNode)
                            targetParentTreeNode.UpdateView();
                    }
                    UpdateWindowCaption();
                    treeView.EndUpdate();
                }
                else
                {
                    treeView.BeginUpdate();
                    // Перемещаем в конец списка 
                    if (e.Effect == DragDropEffects.Move)
                    {
                        draggedNode.Remove();

                        treeView.Nodes.Add(draggedNode);
                        profile.Saved = false;
                        UpdateWindowCaption();
                    }
                    // Копирование узла
                    else if (e.Effect == DragDropEffects.Copy)
                    {
                        treeView.Nodes.Add((TreeNode) draggedNode.Clone());
                        profile.Saved = false;
                        UpdateWindowCaption();
                    }
                    treeView.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Проверяем иерархическое отношене узла <paramref name="parentNode"/> по отношению к <paramref name="childNode"/>
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        /// <returns>True, если <paramref name="childNode"/> является дочерним для <paramref name="parentNode"/>.</returns>
        private bool ContainsNode(TreeNode parentNode, TreeNode childNode)
        {
            if (childNode is null || parentNode is null) return false;

            // Проверяем наличие родителя у "childNode"
            if (childNode.Parent == null) return false;

            if (childNode.Parent.Equals(parentNode)) return true;

            // Производим рекурсивную проверку иерархического отношения "parentNode" по отношению к родительскому узлу "childNode"
            return ContainsNode(parentNode, childNode.Parent);
        }

        /// <summary>
        /// Опеределяем тип эффекта перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

                // Select the node at the mouse position.
                var targetTreeNode = treeView.GetNodeAt(targetPoint);
                pgProperties.SelectedObject = targetTreeNode.Tag;
                e.Effect = e.AllowedEffect;
            }
        }

        /// <summary>
        /// Выделяем и разворачиваем узел дерева под курсором мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                // Извлечение перетаскиваемого узла
                TreeNode draggedNode = (TreeNode) (e.Data.GetData(typeof(ActionTreeNode))
                                                   ?? e.Data.GetData(typeof(ActionPackTreeNode))
                                                   ?? e.Data.GetData(typeof(ConditionPackTreeNode))
                                                   ?? e.Data.GetData(typeof(ConditionTreeNode))
                                                   ?? e.Data.GetData(typeof(TreeNode)));

                if (ReferenceEquals(treeView, draggedNode.TreeView))
                {
                    // Определяем координаты курсора мыши относительно treeView
                    Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

                    var targetNode = treeView.GetNodeAt(targetPoint);

                    // Определяем узел дерева, расположенный под курсором мыши
                    treeView.SelectedNode = targetNode;

                    // Если нажата левая кнопка мыши, то Drag&Drop в режиме перемещения узла
                    if ((e.KeyState & 1) > 0)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    // Если нажата правая кнопка мыши, то Drag&Drop в режиме копирования узла
                    else if ((e.KeyState & 2) > 0)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
                else e.Effect = DragDropEffects.None;
            }
        }
        #endregion




        #region Conditions  manipulation
        private void handler_Condition_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            switch (e.Button.Properties.Caption)
            {
                case "Add Condition":
                    AddCondition(sender);
                    break;
                case "Delete Condition":
                    DeleteCondition(sender);
                    break;
                case "Delete all Conditions":
                    if (XtraMessageBox.Show("Confirm the deletion of all Conditions from the list", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        treeConditions.Nodes.Clear();
                    }

                    break;
                case "Copy Condition":
                    CopyCondition(sender);
                    break;
                case "Paste Condition":
                    PasteCondition(sender);
                    break;
                case "Test Condition":
                    TestCondition(sender);
                    break;
                case "Test all Conditions":
                    TestAllConditions(sender);
                    break;
            }
        }

        private void AddCondition(object sender, EventArgs e = null)
        {
            // Добавление 
            //if (ItemSelectForm.GetAnInstance(out QuesterCondition newCondition, false))
            if (Astral.Quester.Forms.AddAction.Show(typeof(QuesterCondition)) is QuesterCondition newCondition)
            {
                InvokeConditionCallback();

                var selectedNode = treeConditions.SelectedNode as ConditionBaseTreeNode;
                var newTreeNode = newCondition.MakeTreeNode();

                InsertCondition(selectedNode, newTreeNode);

                profile.Saved = false;
                
                SetSelectedConditionTo(newTreeNode);

                UpdateWindowCaption();
            }
        }

        private void DeleteCondition(object sender, EventArgs e = null)
        {
            if (treeConditions.SelectedNode is ConditionBaseTreeNode conditionNode)
            {
                if (conditionNode.Nodes.Count > 0)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ConditionPack",
                        "Confirmation",
                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }

                if (pgProperties.SelectedObject is QuesterCondition)
                {
                    propertyChangedCallback = null;
                    pgProperties.SelectedObject = null;
                }

                treeConditions.BeginUpdate();
                conditionNode.Remove();
                treeConditions.EndUpdate();

                profile.Saved = false;
                UpdateWindowCaption();
            }
        }

        private void CopyCondition(object sender, EventArgs e = null)
        {
            if (treeConditions.SelectedNode is ConditionBaseTreeNode conditionNode)
            {
                InvokeConditionCallback();

                if (conditionNode.AllowChildren)
                    conditionListChangedCallback?.Invoke(treeConditions.Nodes);
                    
                conditionCache = conditionNode.Content.CreateDeepCopy();
            }
        }

        private void PasteCondition(object sender, EventArgs e = null)
        {
            if (conditionCache != null)
            {
                InvokeConditionCallback();

                // Добавляем команду
                var newCondition = conditionCache.CreateDeepCopy();
                var newNode = newCondition.MakeTreeNode();
                var selectedNode = treeConditions.SelectedNode as ConditionBaseTreeNode;
                
                InsertCondition(selectedNode, newNode);

                SetSelectedConditionTo(newNode);

                UpdateWindowCaption();
            }
        }

        private void handler_Condition_ShortCut(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                {
                    CopyCondition(sender);
                }
                else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                {
                    PasteCondition(sender);
                }
                else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                {
                    DeleteCondition(sender);
                }

                return;
            }

            if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
            {
                AddCondition(sender);
            }
        }

        private void TestCondition(object sender, EventArgs e = null)
        {
            InvokeConditionCallback();
            if (treeConditions.SelectedNode is ConditionBaseTreeNode selectedNode)
            {
                var result = selectedNode.IsValid(profile);
                var testInfo = selectedNode.TestInfo(profile);
                var msg = $"{selectedNode.Text}\n{testInfo}\nResult: {result}";
                XtraMessageBox.Show(msg, "Condition Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLog.AppendText(msg);
                return;
            }

            XtraMessageBox.Show(
                "No condition selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtLog.AppendText("\nNo condition selected.");
        }

        private void TestAllConditions(object sender, EventArgs e = null)
        {
            InvokeConditionCallback();

            if (treeConditions.Nodes.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                foreach (ConditionBaseTreeNode node in treeConditions.Nodes)
                {
                    // Тестирование выбранного условия
                    bool result = node.IsValid(profile);
                    stringBuilder.Append('\t')
                                 .Append(node.Text)
                                 .Append(" | Result: ")
                                 .AppendLine(result.ToString());
                }

                var msg = stringBuilder.ToString();
                XtraMessageBox.Show(msg, "Conditions Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLog.AppendText($"\n{msg}");
                return;
            }

            XtraMessageBox.Show(
                "The condition list is empty.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtLog.AppendText("\nThe condition list is empty.");
        }

        private void handler_Condition_Selecting(object sender, TreeViewCancelEventArgs e)
        {
            if (treeConditions.SelectedNode is ConditionBaseTreeNode node)
            {
                InvokeConditionCallback();
            }
        }

        private void handler_Condition_Selected(object sender, TreeViewEventArgs e)
        {
            if (treeConditions.SelectedNode is ConditionBaseTreeNode node)
            {
                SetSelectedConditionTo(node);
            }
        }

        private void handler_Condition_GetFocus(object sender, EventArgs e)
        {
            if (treeConditions.SelectedNode is ConditionBaseTreeNode node)
            {
                if (!ReferenceEquals(node.Content, pgProperties.SelectedObject))
                {
                    InvokeConditionCallback();
                    SetSelectedConditionTo(node);
                }
            }
        }
        
        private void InsertCondition(ConditionBaseTreeNode selectedNode, ConditionBaseTreeNode insertingNode)
        {
            treeConditions.BeginUpdate();
            if (selectedNode != null)
            {
                if (selectedNode.AllowChildren)
                {
                    // добавляем новое условие в список его узлов ConditionPackTreeNode
                    selectedNode.Nodes.Insert(0, insertingNode);
                }
                else
                {
                    // добавляем новое условие после выделенного узла
                    if (selectedNode.Parent is null)
                        treeConditions.Nodes.Insert(selectedNode.Index + 1, insertingNode);
                    else selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, insertingNode);
                }
            }
            // добавляем новое условие в конец списка узлов дерева
            else treeConditions.Nodes.Add(insertingNode);
            treeConditions.EndUpdate();

            profile.Saved = false;
        }

        private void InvokeConditionCallback()
        {
            treeConditions.BeginUpdate();
            propertyChangedCallback?.Invoke();
            treeConditions.EndUpdate();
        }

        private void SetSelectedConditionTo(ConditionBaseTreeNode node)
        {
            if (node is null)
            {
                if (pgProperties.SelectedObject is QuesterCondition)
                {
                    pgProperties.SelectedObject = null;
                    propertyChangedCallback = null;
                }
            }
            else
            {
                pgProperties.SelectedObject = node.Content;
                propertyChangedCallback = node.UpdateView;
            }
        }
        #endregion




        #region Action lists manipulation
        private void handler_Action_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            switch (e.Button.Properties.Caption)
            {
                case "Add Action":
                    AddAction(sender);
                    break;
                case "Delete Action":
                    DeleteAction(sender);
                    break;
                case "Gather Info":
                    GatherActionInfo(sender);
                    break;
                case "Copy Action":
                    CopyAction(sender);
                    break;
                case "Paste Action":
                    PasteAction(sender);
                    break;
                case "Test Action":
                    TestAction(sender);
                    break;
                case "Test all Actions":
                    TestAllActions(sender);
                    break;
                case "Edit XML":
                    EditActionXml(sender);
                    break;
            }
        }

        private void AddAction(object sender, EventArgs e = null)
        {
            if (Astral.Quester.Forms.AddAction.Show(typeof(QuesterAction)) is QuesterAction newAction)
            {
                InvokeActionCallback();

                newAction.GatherInfos();
                var newNode = newAction.MakeTreeNode(profile);

                var selectedNode = treeActions.SelectedNode as ActionBaseTreeNode;

                InsertAction(selectedNode, newNode);

                profile.Saved = false;

                SetSelectedActionTo(newNode);

                UpdateWindowCaption();
            }
        }

        private void DeleteAction(object sender, EventArgs e = null)
        {
            if (treeActions.SelectedNode is ActionBaseTreeNode actionNode)
            {
                if (actionNode is ActionPackTreeNode)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ActionPack",
                        "Confirmation",
                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }

                ResetSelectedAction();

                var deleteAction = new DeleteQuesterAction(actionNode);
                deleteAction.Apply();

                AfterQuesterActionEdited(deleteAction);
            }
        }

        private void GatherActionInfo(object sender, EventArgs e = null)
        {
            InvokeActionCallback();

            if (treeActions.SelectedNode is ActionBaseTreeNode actionNode)
                actionNode.GatherActionInfo(profile);
        }

        private void CopyAction(object sender, EventArgs e = null)
        {
            if (treeActions.SelectedNode is ActionBaseTreeNode actionNode)
            {
                InvokeActionCallback();

                var action = actionNode.ReconstructInternal();

                actionCache = action.CreateDeepCopy();
            }
        }

        private void PasteAction(object sender, EventArgs e = null)
        {
            if (actionCache != null)
            {
                InvokeActionCallback();

                // Добавляем команду
                var newAction = actionCache.CreateDeepCopy();
                var newNode = newAction.MakeTreeNode(profile);
                newNode.NewID();

                var selectedNode = treeActions.SelectedNode as ActionBaseTreeNode;
                InsertAction(selectedNode, newNode);

                pgProperties.SelectedObject = newAction;
                SetSelectedActionTo(newNode);

                UpdateWindowCaption();
            }
        }

        private void handler_Action_ShortCut(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                {
                    CopyAction(sender);
                }
                else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                {
                    PasteAction(sender);
                }
                else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                {
                    DeleteAction(sender);
                }
            }

            if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
            {
                AddAction(sender);
            }
        }

        private void TestAction(object sender, EventArgs e = null)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TestAllActions(object sender, EventArgs e = null)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditActionXml(object sender, EventArgs e = null)
        {
            InvokeActionCallback();

            if (treeActions.SelectedNode is ActionBaseTreeNode actionNode
                && XMLEdit.Show(actionNode.Content) is QuesterAction modifiedAction)
            {
                var newActionNode = modifiedAction.MakeTreeNode(profile);
                int selectedInd = actionNode.Index;
                var parentNode = actionNode.Parent as ActionBaseTreeNode;
                
                treeActions.BeginUpdate();
                actionNode.Remove();
                if (parentNode !=  null)
                {
                    parentNode.Nodes.Insert(selectedInd, newActionNode);
                    parentNode.UpdateView();
                }
                else treeActions.Nodes.Insert(selectedInd, newActionNode);
                treeActions.EndUpdate();

                SetSelectedActionTo(newActionNode);
            }
        }

        private void handler_Action_Selecting(object sender, TreeViewCancelEventArgs e)
        {
            InvokeActionCallback();
        }

        private void handler_Action_Selected(object sender, TreeViewEventArgs e)
        {
            if (e.Node is ActionBaseTreeNode node)
            {
                SetSelectedActionTo(node);
            }
        }

        private void handler_Action_GetFocus(object sender, EventArgs e)
        {
            if (treeActions.SelectedNode is ActionBaseTreeNode node)
            {
                var action = node.Content;
                if (!ReferenceEquals(action, pgProperties.SelectedObject))
                {
                    InvokeActionCallback();
                    SetSelectedActionTo(node);
                }
            }
        }

        private void SetSelectedActionTo(ActionBaseTreeNode node)
        {
            if (node is null)
            {
                treeActions.SelectedNode = null;
                treeConditions.Nodes.Clear();

                gridHotSpots.Enabled = false;
                gridHotSpots.DataSource = null;

                if (pgProperties.SelectedObject is QuesterAction)
                {
                    pgProperties.SelectedObject = null;
                    propertyChangedCallback = null;
                }
                conditionListChangedCallback = null;
            }
            else
            {
                treeActions.SelectedNode = node;

                treeConditions.Nodes.Clear();
                treeConditions.Nodes.AddRange(node.GetConditionTreeNodes());

                pgProperties.SelectedObject = node.Content;

                gridHotSpots.Enabled = node.UseHotSpots;
                gridHotSpots.DataSource = node.HotSpots;

                propertyChangedCallback = node.UpdateView;
                conditionListChangedCallback = node.CopyConditionNodesFrom;
            }
        }

        private void InsertAction(ActionBaseTreeNode selectedNode, ActionBaseTreeNode insertingNode)
        {
            treeActions.BeginUpdate();
            if (selectedNode is null)
                treeActions.Nodes.Add(insertingNode);
            else if (selectedNode.AllowChildren)
            {
                selectedNode.Nodes.Insert(0, insertingNode);
                selectedNode.UpdateView();
            }
            else if (selectedNode.Parent is null)
            {
                treeActions.Nodes.Insert(selectedNode.Index + 1, insertingNode);
            }
            else
            {
                selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, insertingNode);
                if (selectedNode.Parent is ActionPackTreeNode parentActionPack)
                    parentActionPack.UpdateView();
            }
            treeActions.EndUpdate();

            profile.Saved = false;
        }

        private void InvokeActionCallback()
        {
            treeActions.BeginUpdate();
            propertyChangedCallback?.Invoke();
            conditionListChangedCallback?.Invoke(treeConditions.Nodes);
            treeActions.EndUpdate();
        }

        private void ResetSelectedAction()
        {
            conditionListChangedCallback = null;
            propertyChangedCallback = null;

            treeConditions.Nodes.Clear();
            pgProperties.SelectedObject = null;
        }
        #endregion




        #region TreeViewNode manipulation
        private void handler_TreeView_NodeCheckedChanged(object sender, TreeViewEventArgs e)
        {
            switch (e.Node)
            {
                case ActionBaseTreeNode actionNode:
                    actionNode.Disabled = !actionNode.Checked;
                    if (ReferenceEquals(actionNode.Content, pgProperties.SelectedObject))
                        pgProperties.Refresh();
                    break;
                case ConditionBaseTreeNode conditionNode:
                    conditionNode.Locked = conditionNode.Checked;
                    if (ReferenceEquals(conditionNode.Content, pgProperties.SelectedObject))
                        pgProperties.Refresh();
                    break;
            }
            profile.Saved = false;
        }

        private void handler_PropertyChanged(object sender, PropertyValueChangedEventArgs e)
        {
            //TODO: Переименование CustomRegion'a
            if (pgProperties.SelectedObject is CustomRegion customRegion)
            {

            }
            propertyChangedCallback?.Invoke();

            profile.Saved = false;
            UpdateWindowCaption();
        }
        #endregion




        #region Profile manipulation
        private void handler_Profile_New(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!profile.Saved)
            {
                switch (XtraMessageBox.Show("Profile was modified but did not saved!\n" +
                                            "Would you like to save it ?\n" +
                                            "Press 'Cancel' button to continue profile edition.", "Caution!",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        SaveProfile();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            profile.SetProfile(new Profile { Saved = true }, string.Empty);
            UI_fill();

            txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Make new Profile.");
        }

        private void handler_Profile_Load(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!profile.Saved)
            {
                switch (XtraMessageBox.Show("Profile was modified but did not saved!\n" +
                                            "Would you like to save it ?\n" +
                                            "Press 'Cancel' button to continue profile edition.", "Caution!",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        SaveProfile();
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            string path = string.Empty;
            var prof = AstralAccessors.Quester.Core.Load(ref path);
            if (prof != null)
            {
                profile.SetProfile(prof, path);
                txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Profile '{path}' loaded.");
                UI_fill();
            }
        }

        private void handler_Profile_Save(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SaveProfile())
            {
                txtLog.AppendText(string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(profile.FileName)
                        ? string.Empty
                        : " '" + profile.FileName + "'",
                    " saved.",
                    Environment.NewLine));
                UpdateWindowCaption();
            }
        }

        private void handler_Profile_SaveAs(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SaveProfile(true))
            {
                txtLog.AppendText(string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(profile.FileName)
                        ? string.Empty
                        : " '" + profile.FileName + "'",
                    " saved.",
                    Environment.NewLine));
                UpdateWindowCaption();
            }
        }

        private void handler_Profile_Upload(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var saved = profile.Saved;
            if (saved || SaveProfile())
            {
#if true
                var actualProfile = profile.GetProfile().CreateDeepCopy();
                AstralAccessors.Quester.Core.SetProfile(actualProfile, profile.FileName);
#else
                AstralAccessors.Quester.Core.Profile = CopyHelper.CreateDeepCopy(profile.GetProfile());
                var fileName = profile.FileName;
                Astral.API.CurrentSettings.LastQuesterProfile = fileName;
                Astral.Controllers.Settings.Get.LastQuesterProfile = string.IsNullOrEmpty(fileName)
                    ? string.Empty
                    : Path.GetFileName(profile.FileName);
                if (AstralAccessors.Controllers.Roles.IsRunning)
                    AstralAccessors.Controllers.Roles.ToggleRole(true);
                AstralAccessors.Quester.Entrypoint.RefreshQuesterMainPanel(); 
#endif
                txtLog.AppendText(
                    string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(profile.FileName)
                        ? string.Empty
                        : " '" + profile.FileName + "'",
                    saved ? string.Empty : " saved and",
                    " uploaded to the Quester-engine.",
                    Environment.NewLine));

                UpdateWindowCaption();
            }
            else
            {
                XtraMessageBox.Show("Unable to upload profile into Quester-engine because of saving error.", "Upload error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void handler_Profile_UploadAndFocusAction(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var saved = profile.Saved;
            if (saved || SaveProfile())
            {
#if true
                var actualProfile = profile.GetProfile().CreateDeepCopy();
                var actionId = treeActions.SelectedNode is ActionBaseTreeNode selectedNode
                                 ? selectedNode.Content.ActionID
                                 : Guid.Empty;

                AstralAccessors.Quester.Core.SetProfile(actualProfile, profile.FileName, actionId);
#else
                var actualProfile = CopyHelper.CreateDeepCopy(profile.GetProfile());
                AstralAccessors.Quester.Core.Profile = actualProfile;
                var fileName = profile.FileName;
                Astral.Controllers.Settings.Get.LastQuesterProfile = string.IsNullOrEmpty(fileName)
                    ? string.Empty
                    : Path.GetFileName(profile.FileName);
                if (AstralAccessors.Controllers.Roles.IsRunning)
                    AstralAccessors.Controllers.Roles.ToggleRole(true);
                if (treeActions.SelectedNode is ActionBaseTreeNode selectedNode)
                    QuesterHelper.SetStartPoint(actualProfile.MainActionPack, selectedNode.Content.ActionID);
                AstralAccessors.Quester.Entrypoint.RefreshQuesterMainPanel(); 
#endif
                txtLog.AppendText(string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(profile.FileName)
                        ? string.Empty
                        : " '" + profile.FileName + "'",
                    saved ? string.Empty : " saved and",
                    " uploaded to the Quester-engine.",
                    Environment.NewLine));

                UpdateWindowCaption();
            }
            else
            {
                XtraMessageBox.Show("Unable to upload profile into Quester-engine because of saving error.", "Upload error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Сохранение профиля в файл
        /// </summary>
        /// <param name="saveAs"></param>
        /// <returns></returns>
        private bool SaveProfile(bool saveAs = false)
        {
            try
            {
                propertyChangedCallback?.Invoke();
                conditionListChangedCallback?.Invoke(treeConditions.Nodes);

                // Восстановление команд
                profile.Actions = ListReconstruction(treeActions.Nodes);

                // Восстановление не требуется, т.к. списки реализованы через BindingList<T>
                //profile.Vendors = ListReconstruction<NPCInfos>(listVendor.Items);
                //profile.CustomRegions = ListReconstruction<CustomRegion>(listCustomRegions.Items);
                //profile.BlackList = ListReconstruction<string>(listBlackList.Items);

                if (saveAs)
                    profile.SaveAs();
                else profile.Save();

                DialogResult = DialogResult.OK;
                return profile.Saved;
            }
            catch (Exception exc)
            {
                XtraMessageBox.Show(exc.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Преобразование списка узлов дерева <paramref name="nodes"/> в перечисление объектов типа <see cref="QuesterAction"/>
        /// </summary>
        /// <param name="nodes">Коллекция узлов</param>
        /// <returns></returns>
        private static IEnumerable<QuesterAction> ListReconstruction(TreeNodeCollection nodes)
        {
            if (nodes != null)
            {   
                foreach (ActionBaseTreeNode node in nodes)
                {
                    var action = node.ReconstructInternal();
                    if (action != null)
                        yield return action;
                }
            }
        }
        #endregion




        #region HotSpots manipulation
        private void handler_HotSpot_RowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = e.RowHandle.ToString();
            }
        }

        private void handler_HotSpot_DragOver(object sender, DragOverEventArgs e)
        {
            DragOverGridEventArgs args = DragOverGridEventArgs.GetDragOverGridEventArgs(e);
            e.InsertType = args.InsertType;
            e.InsertIndicatorLocation = args.InsertIndicatorLocation;
            e.Action = args.Action;
            Cursor.Current = args.Cursor;
            args.Handled = true;
        }

        private void handler_HotSpot_DragDrop(object sender, DragDropEventArgs e)
        {
            if (e.Target is GridView targetGrid
                && e.Source is GridView sourceGrid)
            {
                if (e.Action == DragDropActions.None || targetGrid != sourceGrid)
                    return;
                if (sourceGrid.GridControl.DataSource is List<Vector3> hotSpotsList)
                {
                    Point hitPoint = targetGrid.GridControl.PointToClient(Cursor.Position);
                    GridHitInfo hitInfo = targetGrid.CalcHitInfo(hitPoint);

                    int[] sourceHandles = e.GetData<int[]>();

                    int targetRowHandle = hitInfo.RowHandle;
                    int targetSpotIndex = targetGrid.GetDataSourceRowIndex(targetRowHandle);

                    var draggedSpots = new List<Vector3>(sourceHandles.Length);
                    foreach (int sourceHandle in sourceHandles)
                    {
                        int oldSpotIndex = sourceGrid.GetDataSourceRowIndex(sourceHandle);
                        var oldSpot = hotSpotsList[oldSpotIndex];
                        draggedSpots.Add(oldSpot);
                    }

                    sourceGrid.BeginUpdate();
                    int newSpotIndex;
                    switch (e.InsertType)
                    {
                        case InsertType.Before:
                            newSpotIndex = targetSpotIndex > sourceHandles[sourceHandles.Length - 1]
                                ? targetSpotIndex - 1
                                : targetSpotIndex;
                            for (int i = draggedSpots.Count - 1; i >= 0; i--)
                            {
                                var spot = draggedSpots[i];
                                hotSpotsList.Remove(spot);
                                hotSpotsList.Insert(newSpotIndex, spot);
                            }
                            profile.Saved = false;
                            UpdateWindowCaption();
                            break;
                        case InsertType.After:
                            newSpotIndex = targetSpotIndex < sourceHandles[0] ? targetSpotIndex + 1 : targetSpotIndex;
                            for (int i = 0; i < draggedSpots.Count; i++)
                            {
                                var spot = draggedSpots[i];
                                hotSpotsList.Remove(spot);
                                hotSpotsList.Insert(newSpotIndex, spot);
                            }
                            profile.Saved = false;
                            UpdateWindowCaption();
                            break;
                        default:
                            newSpotIndex = -1;
                            break;
                    }
                    sourceGrid.RefreshData();
                    sourceGrid.EndUpdate();

                    int insertedIndex = targetGrid.GetRowHandle(newSpotIndex);
                    targetGrid.FocusedRowHandle = insertedIndex;
                    targetGrid.SelectRow(targetGrid.FocusedRowHandle);
                }
            }
        }

        private void handler_HotSpots_ButtonClick(object sender, [NotNull] DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var btn = e.Button;
            switch (btn.Properties.Caption)
            {
                case "Add HotSpot":
                    AddHotSpot();
                    break;
                case "Delete HotSpot":
                    DeleteHotSpot();
                    break;
                case "Edit Coordinates":
                    ChangeHotSpotCoordinateEditMode(btn.IsChecked == true);
                    break;
            }
        }

        private void AddHotSpot()
        {
            if (gridHotSpots.DataSource is List<Vector3> hotSpots)
            {
                var pos = EntityManager.LocalPlayer.Location.Clone();
                Node node = profile.CurrentMesh.ClosestNode(pos.X, pos.Y, pos.Z, out double distance, false);
                gridViewHotSpots.BeginUpdate();
                if (node != null
                    && distance < 10)
                    hotSpots.Add(node.Position);
                else hotSpots.Add(pos);

                profile.Saved = false;
                UpdateWindowCaption();

                gridViewHotSpots.RefreshData();
                gridViewHotSpots.EndUpdate();
            }
        }

        private void DeleteHotSpot()
        {
            var selectedRows = gridViewHotSpots.GetSelectedRows();
            if (gridHotSpots.DataSource is List<Vector3> hotSpots
                && selectedRows.Length > 0)
            {
                gridViewHotSpots.BeginUpdate();
                for (int i = 0; i < selectedRows.Length; i++)
                {
                    int ind = selectedRows[i] - i;
                    hotSpots.RemoveAt(ind);
                }

                profile.Saved = false;
                UpdateWindowCaption();
                //TODO Обновление ActionTreeNode при пустом списке HotSpot's
                gridViewHotSpots.RefreshData();
                gridViewHotSpots.EndUpdate();
            }
        }
        
        private void ChangeHotSpotCoordinateEditMode(bool allowEdit)
        {
            gridViewHotSpots.OptionsBehavior.Editable = allowEdit;
        }
        #endregion




        #region List manipulation
        private void handler_ListBox_ItemSelected(object sender, EventArgs e)
        {
            if (sender is ListBoxControl listBox)
            {
                var obj = listBox.SelectedItem;
                if (obj != null && !ReferenceEquals(obj, pgProperties.SelectedObject))
                {
                    propertyChangedCallback?.Invoke();
                    pgProperties.SelectedObject = obj;
                    propertyChangedCallback = () => listBox.Refresh();
                }
            }
        }

        private void handler_Vendor_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var btn = e.Button;
            switch (btn.Properties.Caption)
            {
                case "Add Vendor":
                    {
                        Entity entity = null;
                        while (TargetSelectForm.GUIRequest("Get NPC", "Target the NPC and press ok.") == DialogResult.OK)
                        {
                            Entity betterEntityToInteract = Interact.GetBetterEntityToInteract();
                            if (betterEntityToInteract.IsValid)
                            {
                                entity = betterEntityToInteract;
                                break;
                            }
                        }

                        if (entity?.IsValid != true)
                        {
                            XtraMessageBox.Show("Select an target.");
                            return;
                        }

                        var player = EntityManager.LocalPlayer;
                        NPCInfos npcInfos = new NPCInfos
                        {
                            DisplayName = entity.Name,
                            Position = entity.Location.Clone(),
                            CostumeName = entity.CostumeRef.CostumeName,
                            MapName = player.MapState.MapName,
                            RegionName = player.RegionInternalName
                        };

                        var vendors = profile.Vendors;
                        if (!vendors.Contains(npcInfos))
                        {
                            vendors.Add(npcInfos);
                            listVendor.SelectedItem = npcInfos;

                            profile.Saved = false;
                            UpdateWindowCaption();
                            return;
                        }

                        XtraMessageBox.Show("This vendor is already in list.");
                        break;
                    }
                case "Delete Vendor":
                    {
                        if (listVendor.SelectedItem is NPCInfos item)
                        {
                            profile.Vendors.Remove(item);
                            profile.Saved = false;
                            UpdateWindowCaption();
                        }
                        break;
                    }
            }
        }

        private void handler_CustomRegions_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var btn = e.Button;
            if (btn.Properties.Caption == "Delete CustomRegion")
            {
                if (listCustomRegions.SelectedItem is CustomRegion item)
                {
                    profile.CustomRegions.Remove(item);
                    profile.Saved = false;
                    UpdateWindowCaption();
                }
            }
        }

        private void handler_BlackList_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            var btn = e.Button;
            switch (btn.Properties.Caption)
            {
                case "Add":
                    {
                        string entPattern = string.Empty;
                        ItemFilterStringType strMatchType = ItemFilterStringType.Simple;
                        EntityNameType nameType = EntityNameType.InternalName;
                        var target = EntityViewer.GUIRequest(ref entPattern, ref strMatchType, ref nameType);

                        if (target?.IsValid != true)
                        {
                            XtraMessageBox.Show("No valid target.");
                            return;
                        }

                        if (target.RelationToPlayer == EntityRelation.Friend)
                        {
                            XtraMessageBox.Show("Target should be an Enemy.");
                            return;
                        }

                        var blackList = profile.BlackList;
                        if (!blackList.Contains(target.InternalName))
                        {
                            blackList.Add(target.InternalName);
                            profile.Saved = false;
                            UpdateWindowCaption();
                            return;
                        }

                        XtraMessageBox.Show("Already in list.");
                        break;
                    }
                case "Delete":
                    {
                        var item = listBlackList.SelectedItem?.ToString();
                        if (!string.IsNullOrEmpty(item))
                        {
                            profile.BlackList.Remove(item);
                            profile.Saved = false;
                            UpdateWindowCaption();
                        }
                        break;
                    }
            }
        }
        #endregion




        #region Mapper manupulation
        private MapperFormExt mapperForm;
        private void handler_OpenMapper(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (mapperForm is null || mapperForm.IsDisposed)
            {
                mapperForm = new MapperFormExt(profile);
                mapperForm.OnDraw += DrawSelectedAction;
            }

            mapperForm.Show();
            mapperForm.Focus();
        }

        private void DrawSelectedAction(MapperGraphics graphics)
        {
            var tag = treeActions.SelectedNode?.Tag;
            switch (tag)
            {
                case ActionPack actionPack when actionPack.SimultaneousActions:
                    bool shouldDrawHotSpots = true;
                    foreach (var action in actionPack.Actions)
                    {
                        if (shouldDrawHotSpots && action.UseHotSpots)
                        {
                            ComplexPatch_Mapper.DrawHotSpots(action.HotSpots, graphics);
                            shouldDrawHotSpots = false;
                        }
                        action.OnMapDraw(graphics);
                    }
                    break;
                case QuesterAction action:
                    if (action.UseHotSpots)
                    {
                        ComplexPatch_Mapper.DrawHotSpots(action.HotSpots, graphics);
                    }
                    action.OnMapDraw(graphics);
                    break;
            }
        }
        #endregion




        #region ActionNode Highlighting
        private readonly LinkedList<TreeNode> highlightedTreeNodes = new LinkedList<TreeNode>();
        private LinkedListNode<TreeNode> selectedHighlightedNode;

        private void handler_Filter_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                FilterActionNodes(txtActionFilter.Text);
        }

        private void handler_Filter_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            switch (e.Button.Kind)
            {
                case ButtonPredefines.Search:
                    FilterActionNodes(txtActionFilter.Text);
                    SelectFirstHighlightedNode();
                    break;
                case ButtonPredefines.SpinUp:
                    SelectPreviousHighlightedNode();
                    break;
                case ButtonPredefines.SpinDown:
                    SelectNextHighlightedNode();
                    break;
                case ButtonPredefines.Clear:
                    ResetFilter();
                    ClearTreeNodeHighlighting(treeActions.Nodes);
                    break;
            }
        }

        private void FilterActionNodes(string filter)
        {
            highlightedTreeNodes.Clear();
            selectedHighlightedNode = null;
            treeActions.BeginUpdate();
            if (string.IsNullOrEmpty(filter))
                ClearTreeNodeHighlighting(treeActions.Nodes);
            else HighlightTreeNodes(treeActions.Nodes, filter);
            treeActions.EndUpdate();
        }

        private void ClearTreeNodeHighlighting(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = Color.Empty;
                if (node.Nodes.Count > 0)
                    ClearTreeNodeHighlighting(node.Nodes);
            }
        }

        private void HighlightTreeNodes(TreeNodeCollection nodes, string filter)
        {
            foreach (TreeNode node in nodes)
            {
                if (IsNodeMatchFilter(node, filter))
                {
                    highlightedTreeNodes.AddLast(node);
                    node.BackColor = Color.Yellow;
                }
                else node.BackColor = Color.Empty;
                if (node.Nodes.Count > 0)
                    HighlightTreeNodes(node.Nodes, filter);
            }
        }

        private void SelectFirstHighlightedNode()
        {
            if (highlightedTreeNodes.Count == 0)
                NotifyNoActionsMatchesFilter();
            selectedHighlightedNode = highlightedTreeNodes.First;
            var node = selectedHighlightedNode?.Value;
            if (node != null)
            {
                treeActions.SelectedNode = node;
                node.EnsureVisible();
            }
        }

        private void SelectLastHighlightedNode()
        {
            if (highlightedTreeNodes.Count == 0)
                NotifyNoActionsMatchesFilter();
            selectedHighlightedNode = highlightedTreeNodes.Last;
            var node = selectedHighlightedNode?.Value;
            if (node != null)
            {
                treeActions.SelectedNode = node;
                node.EnsureVisible();
            }
        }
        
        private void SelectNextHighlightedNode()
        {
            if (highlightedTreeNodes.Count == 0)
                NotifyNoActionsMatchesFilter();

            selectedHighlightedNode = selectedHighlightedNode?.Next;
            var node = selectedHighlightedNode?.Value;
            if (node != null)
            {
                treeActions.SelectedNode = node;
                node.EnsureVisible();
            }
            else SelectFirstHighlightedNode();
            
        }
        
        private void SelectPreviousHighlightedNode()
        {
            if (highlightedTreeNodes.Count == 0)
                NotifyNoActionsMatchesFilter();

            selectedHighlightedNode = selectedHighlightedNode?.Previous;
            var node = selectedHighlightedNode?.Value;
            if (node != null)
            {
                treeActions.SelectedNode = node;
                node.EnsureVisible();
            }
            else SelectLastHighlightedNode();
            
        }
        
        private static bool IsNodeMatchFilter(TreeNode node, string filter)
        {
            if (node.Text.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            var tag = node.Tag;
            if (tag is null)
                return false;
            if (tag.GetType().Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            if (tag is QuesterAction action
                && action.ActionID.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            return false;
        }
        
        private void ResetFilter()
        {
            txtActionFilter.Text = string.Empty;
            highlightedTreeNodes.Clear();
            selectedHighlightedNode = null;
        }
        
        private void NotifyNoActionsMatchesFilter()
        {
            XtraMessageBox.Show($"No one actions matches filter '{txtActionFilter.Text}'", "Filtering info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion




        #region Edit history
        private void handler_Undo(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var action = undoStack.Pop();
                action.Undo();
                if (undoStack.Count > 0)
                {
                    action = undoStack.Peek();
                    btnUndo.Hint = action.UndoLabel; 
                }
                else btnUndo.Hint = string.Empty;
            }
        }

        private delegate void EditActionEvent(IQEdit qEdit);

        private void AfterQuesterActionEdited(IQEdit qEdit)
        {
            profile.Saved = false;
            UpdateWindowCaption();

            undoStack.Push(qEdit);
            btnUndo.Hint = qEdit.UndoLabel;
            redoStack.Clear();
            btnRedo.Hint = string.Empty;
        }

        private void ApplyEditCondition(IQEdit editCondition)
        {
            treeConditions.BeginUpdate();
            editCondition.Apply();
            treeConditions.EndUpdate();
            undoStack.Push(editCondition);
            btnUndo.Hint = editCondition.UndoLabel;
            redoStack.Clear();
            btnRedo.Hint = string.Empty;
        }
        #endregion
    }
}