using ACTP0Tools;
using ACTP0Tools.Reflection;
using AStar;
using Astral.Classes.ItemFilter;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using Astral.Quester.Forms;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EntityCore.Quester.Classes;
using EntityCore.Tools;
using EntityCore.UCC.Classes;
using EntityTools.Enums;
using EntityTools.Quester.Conditions;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using Astral.Quester.Classes.Conditions;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using EntityTools.Annotations;
using EntityTools.Patches.Mapper;
using QuesterAction = Astral.Quester.Classes.Action;
using QuesterCondition = Astral.Quester.Classes.Condition;

namespace EntityCore.Forms
{
    public partial class QuesterEditor : XtraForm
    {
        /// <summary>
        /// Редактируемый профиль
        /// </summary>
        public QuesterProfileProxy Profile => _profile;

        private readonly ProfileProxy _profile;

        // Функтор, выполняемый при изменении pgProperties
        private System.Action propertyCallback;

        // Функтор, выполняемый при изменении списка treeConditions
        private System.Action conditionCallback;

        // Функтор, выполняемый при изменении списка treeConditions
        private System.Action hotSpotCallback;

        // Скопированная команда
        private QuesterAction actionCache;

        // Скопированная условие
        private static QuesterCondition conditionCache;

        // Команда, с которым сопоставлен список условий
        private static QuesterAction selectedAction;

        // Выбранный элемент ГИП
        private Control selectedControl;

        //TODO: Исправить валидацию и настройка команд и условий, привязаных к данным профиля: PushProfileToStackAndLoad (путь к профилю), IsInCustomRegion, IsInCustomRegionSet

        #region Попытка подмены PropertyDescriptor'a для установки редактора свойств типа CustomRegionCollection
#if false
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

        public QuesterEditor(Profile profile, string fileName)
        {
            InitializeComponent();

            _profile = profile is null 
                ? new ProfileProxy()
                : new ProfileProxy(profile, fileName);

            var settingFile = FileTools.QuesterEditorSettingsFile;
            if (File.Exists(settingFile))
            {
                dockManager.RestoreLayoutFromXml(settingFile);
            }

            bool isChecked = panHotSpots.CustomHeaderButtons["Edit Coordinates"].IsChecked == true;
            gridViewHotSpots.OptionsBehavior.Editable = isChecked;
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
                    }
                }
            }

            var editor = new QuesterEditor(profile, profileName);
            if (modal)
                return editor.ShowDialog() == DialogResult.OK;
            editor.Show();
            return true;
        }

        /// <summary>
        /// Отображение профиля <paramref name="profile"/> в окне редактора.
        /// </summary>
        /// <param name="profile"></param>
        private void UI_fill(ProfileProxy profile)
        {
            if (profile is null)
            {
                UI_reset();
                return;
            }

            // Отображение набора команд
            treeActions.Nodes.Clear();
            if (profile.Actions.Any())
                treeActions.Nodes.AddRange(profile.Actions.ToTreeNodes(true));

            treeConditions.Nodes.Clear();

            listBlackList.DataSource = profile.BlackList;
            listCustomRegions.DataSource = profile.CustomRegions;
            listVendor.DataSource = profile.Vendors;

            gridHotSpots.DataSource = null;
            
            pgSettings.SelectedObject = profile;
            pgProperties.SelectedObject = null;

            selectedControl = null;
            selectedAction = null;
            propertyCallback = null;
            conditionCallback = null;
            hotSpotCallback = null;

            ChangeWindowCaption();
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

            selectedControl = null;
            selectedAction = null;
            propertyCallback = null;
            conditionCallback = null;

            ChangeWindowCaption();
            ResetFilter();
        }

        private void ChangeWindowCaption()
        {
            if (_profile is null
                || string.IsNullOrEmpty(_profile.FileName))
                Text = "* New profile";
            else Text = (_profile.Saved ? string.Empty : "* ")
                       + _profile.FileName;
        }

        private void handler_Form_Load(object sender, EventArgs e)
        {
            // Подмена метаданных для IsInCustomRegion
            // https://stackoverflow.com/questions/46099675/can-you-how-to-specify-editor-at-runtime-for-propertygrid-for-pcl
            var provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                typeof(IsInCustomRegion),
                typeof(IsInCustomRegionMetadataType));
            TypeDescriptor.AddProvider(provider, typeof(IsInCustomRegion));

            var tPushProfileToStackAndLoad = ACTP0Serializer.PushProfileToStackAndLoad;
            if (tPushProfileToStackAndLoad != null)
            {
                provider = new AssociatedMetadataTypeTypeDescriptionProvider(
                    tPushProfileToStackAndLoad,
                    typeof(PushProfileToStackAndLoadMetadataType));
                TypeDescriptor.AddProvider(provider, tPushProfileToStackAndLoad);
            }


            UI_fill(_profile);
        }

        private void handler_Form_Closing(object sender, FormClosingEventArgs e)
        {
            if (!_profile.Saved)
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
            conditionCallback?.Invoke();
            propertyCallback?.Invoke();

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
                            _profile.Saved = false;
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Insert(0, (TreeNode) draggedNode.Clone());
                            _profile.Saved = false;
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
                            _profile.Saved = false;
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Insert(0, (TreeNode) draggedNode.Clone());
                            _profile.Saved = false;
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
                            _profile.Saved = false;
                        }
                        // Копирование узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            treeNodeCollection.Insert(targetNode.Index + 1, (TreeNode)draggedNode.Clone());
                            _profile.Saved = false;
                        }
                        if (targetNode.Parent is ActionPackTreeNode targetParentTreeNode)
                            targetParentTreeNode.UpdateView();
                    }
                    ChangeWindowCaption();
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
                        _profile.Saved = false;
                        ChangeWindowCaption();
                    }
                    // Копирование узла
                    else if (e.Effect == DragDropEffects.Copy)
                    {
                        treeView.Nodes.Add((TreeNode) draggedNode.Clone());
                        _profile.Saved = false;
                        ChangeWindowCaption();
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
                    handler_Condition_Add(sender);
                    break;
                case "Delete Condition":
                    handler_Condition_Delete(sender);
                    break;
                case "Delete all Conditions":
                    if (XtraMessageBox.Show("Confirm the deletion of all Conditions from the list", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        treeConditions.Nodes.Clear();
                    }

                    break;
                case "Copy Condition":
                    handler_Condition_Copy(sender);
                    break;
                case "Paste Condition":
                    handler_Condition_Paste(sender);
                    break;
                case "Test Condition":
                    handler_Condition_Test(sender);
                    break;
                case "Test all Conditions":
                    handler_Condition_TestAll(sender);
                    break;
            }
        }

        private void handler_Condition_Add(object sender, EventArgs e = null)
        {
            // Добавление 
            //if (ItemSelectForm.GetAnInstance(out QuesterCondition newCondition, false))
            if (AddAction.Show(typeof(Condition)) is Condition newCondition)
            {
                propertyCallback?.Invoke();

                var newTreeNode = newCondition.MakeTreeNode();
                var selectedNode = treeConditions.SelectedNode;
                if (selectedNode != null)
                {
                    if (selectedNode is ConditionPackTreeNode conditionPackTreeNode)
                    {
                        // Если выделенный узел является ActionPackTreeNode
                        // добавляем новую команду в список его узлов
                        conditionPackTreeNode.Nodes.Insert(0, newTreeNode);
                    }
                    else
                    {
                        // добавляем новую команду после выделенного узла
                        if (selectedNode.Parent is null)
                            treeConditions.Nodes.Insert(selectedNode.Index + 1, newTreeNode);
                        else selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, newTreeNode);
                    }
                }
                // добавляем новую команду в конец списка узлов дерева
                else treeConditions.Nodes.Add(newTreeNode);
#if false
                if (newCondition is IsInCustomRegionSet crSet)
                {
#if false
                TypeDescriptor.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));
#elif false
                EntityTools.EntityTools.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));

                var properties = TypeDescriptor.GetProperties(crSet);
#elif false
                pgProperties.SelectedObject = Engine.CustomRegionCollectionDecorator.Decorate(crSet);
#else
                    crSet.CustomRegions.DebugContext = profile;
#endif
                } 
#endif
                _profile.Saved = false;
                pgProperties.SelectedObject = newCondition;
                if (newTreeNode is ITreeNode<QuesterCondition> conditionPackNode)
                    propertyCallback = conditionPackNode.UpdateView;
                else propertyCallback = null;

                ChangeWindowCaption();
            }
        }

        private void handler_Condition_Delete(object sender, EventArgs e = null)
        {
            var conditionNode = treeConditions.SelectedNode;

            if (conditionNode != null)
            {
                if (conditionNode is ConditionPackTreeNode)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ConditionPack",
                        "Confirmation",
                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }

                if (ReferenceEquals(pgProperties.SelectedObject, conditionNode.Tag))
                    propertyCallback = null;
                pgProperties.SelectedObject = null;
                conditionNode.Remove();
                _profile.Saved = false;
                ChangeWindowCaption();
            }
        }

        private void handler_Condition_Copy(object sender, EventArgs e = null)
        {
            if (treeConditions.SelectedNode?.Tag is QuesterCondition condition)
            {
                propertyCallback?.Invoke();

                conditionCache = CopyHelper.CreateDeepCopy(condition);
            }
        }

        private void handler_Condition_Paste(object sender, EventArgs e = null)
        {
            if (conditionCache != null)
            {
                propertyCallback?.Invoke();

                // Добавляем команду
                var newCondition = CopyHelper.CreateDeepCopy(conditionCache);
                var newNode = newCondition.MakeTreeNode();
                var selectedNode = treeConditions.SelectedNode;
                if (selectedNode != null)
                {
                    if (selectedNode is ConditionPackTreeNode conditionPackNode)
                        // Если выделенный узел является ActionPackTreeNode
                        // добавляем новую команду в список его узлов
                        conditionPackNode.Nodes.Insert(0, newNode);
                    else
                    {
                        // добавляем новую команду после выделенного узла
                        if (selectedNode.Parent is null)
                            treeConditions.Nodes.Insert(selectedNode.Index + 1, newNode);
                        else selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, newNode);
                    }
                }
                // добавляем новую команду в конец списка узлов дерева
                else treeConditions.Nodes.Add(newNode);

#if false
                if (newCondition is IsInCustomRegionSet crSet)
                {

#if false
                TypeDescriptor.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));
#elif false
                EntityTools.EntityTools.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));

                var properties = TypeDescriptor.GetProperties(crSet);
#elif false
                pgProperties.SelectedObject = Engine.CustomRegionCollectionDecorator.Decorate(crSet);
#else
                    crSet.CustomRegions.DebugContext = profile;
#endif
                } 
#endif
                pgProperties.SelectedObject = newCondition;
                if (newNode is ITreeNode<QuesterCondition> conditionTreeNode)
                    propertyCallback = conditionTreeNode.UpdateView;

                _profile.Saved = false;
                ChangeWindowCaption();
            }
        }

        private void handler_Condition_ShortCut(object sender, KeyEventArgs e)
        {
            //if (ReferenceEquals(sender, treeConditions))
            {
                if (e.Control)
                {
                    if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                    {
                        handler_Condition_Copy(sender);
                    }
                    else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                    {
                        handler_Condition_Paste(sender);
                    }
                    else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                    {
                        handler_Condition_Delete(sender);
                    }

                    return;
                }

                if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
                {
                    handler_Condition_Add(sender);
                }
            }
        }

        private void handler_Condition_Test(object sender, EventArgs e = null)
        {
            conditionCallback?.Invoke();
            propertyCallback?.Invoke();
            var selectedNode = treeConditions.SelectedNode;
            if (selectedNode is ConditionPackTreeNode conditionPackNode)
                conditionPackNode.ReconstructInternal();
            if (selectedNode?.Tag is QuesterCondition condition)
            {
                // Тестирование выбранного условия
                bool result = condition.IsValid;
                string msg = $"{condition.TestInfos}\nResult: {result}";
                XtraMessageBox.Show(msg, "Condition Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLog.AppendText($"\n{msg}");
                return;
            }

            XtraMessageBox.Show(
                "No condition selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtLog.AppendText("\nNo condition selected.");
        }

        private void handler_Condition_TestAll(object sender, EventArgs e = null)
        {
            conditionCallback?.Invoke();
            propertyCallback?.Invoke();

            if (treeConditions.Nodes?.Count > 0)
            {
                var stringBuilder = new StringBuilder();
                foreach (TreeNode node in treeConditions.Nodes)
                {
                    if (node is ConditionPackTreeNode conditionPackNode)
                        conditionPackNode.ReconstructInternal();
                    if (node.Tag is QuesterCondition condition)
                    {
                        // Тестирование выбранного условия
                        bool result = condition.IsValid;
                        stringBuilder.Append('\t')
                                     .Append(condition)
                                     .Append(" | Result: ")
                                     .AppendLine(result.ToString());
                    }
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

        private void handler_Condition_GetFocus(object sender, EventArgs e)
        {
            var node = treeConditions.SelectedNode;
            if (node != null)
            {
                var tag = node.Tag;
                if (tag != null && !ReferenceEquals(tag, pgProperties.SelectedObject))
                {
                    ChangeTreeNodeCallback(node);
#if false
                if (tag is IsInCustomRegionSet crSet)
                {

#if false
                TypeDescriptor.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));
#elif false
                EntityTools.EntityTools.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));

                var properties = TypeDescriptor.GetProperties(crSet);
#elif false
                pgProperties.SelectedObject = Engine.CustomRegionCollectionDecorator.Decorate(crSet);
                return; 
#else
                    crSet.CustomRegions.DebugContext = profile;
#endif
                } 
#endif
                    pgProperties.SelectedObject = tag;
                }
                //node.BackColor = treeConditions.BackColor; 
            }
        }
        #endregion




        #region Action lists manipulation
        private void handler_Action_ButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            switch (e.Button.Properties.Caption)
            {
                case "Add Action":
                    handler_Action_Add(sender);
                    break;
                case "Delete Action":
                    handler_Action_Delete(sender);
                    break;
                case "Copy Action":
                    handler_Action_Copy(sender);
                    break;
                case "Paste Action":
                    handler_Action_Paste(sender);
                    break;
                case "Test Action":
                    handler_Action_Test(sender);
                    break;
                case "Test all Actions":
                    handler_Action_TestAll(sender);
                    break;
                case "Edit XML":
                    handler_Action_EditXML(sender);
                    break;
            }
        }

        private void handler_Action_Add(object sender, EventArgs e = null)
        {
            if (Astral.Quester.Forms.AddAction.Show(typeof(QuesterAction)) is QuesterAction newAction)
            {
                newAction.GatherInfos();
                var newNode = newAction.MakeTreeNode();

                propertyCallback?.Invoke();
                conditionCallback?.Invoke();

                var selectedNode = treeActions.SelectedNode;
                if (selectedNode is null)
                    treeActions.Nodes.Add(newNode);
                else if (selectedNode is ActionPackTreeNode actionPackNode)
                {
                    actionPackNode.Nodes.Insert(0, newNode);
                    actionPackNode.UpdateView();
                }
                else if (selectedNode.Parent is null)
                {
                    treeActions.Nodes.Insert(selectedNode.Index + 1, newNode);
                }
                else
                {
                    selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, newNode);
                    if(selectedNode.Parent is ActionPackTreeNode parentActionPack)
                        parentActionPack.UpdateView();
                }

                _profile.Saved = false;

                treeActions.SelectedNode = newNode;
                selectedAction = newAction;

                ChangeWindowCaption();
            }
        }

        private void handler_Action_Delete(object sender, EventArgs e = null)
        {
            var actionNode = treeActions.SelectedNode;

            if (actionNode != null)
            {
                var parentNode = actionNode.Parent as ActionPackTreeNode;
                if (actionNode is ActionPackTreeNode)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ActionPack",
                        "Confirmation",
                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }

                conditionCallback = null;
                propertyCallback = null;
                hotSpotCallback = null;

                treeConditions.Nodes.Clear();
                selectedAction = null;
                pgProperties.SelectedObject = null;
                
                actionNode.Remove();
                parentNode?.UpdateView();

                _profile.Saved = false;

                ChangeWindowCaption();
            }
        }

        private void handler_Action_Copy(object sender, EventArgs e = null)
        {
            if (treeActions.SelectedNode?.Tag is QuesterAction action)
            {
                propertyCallback?.Invoke();
                conditionCallback?.Invoke();

                actionCache = CopyHelper.CreateDeepCopy(action);
            }
        }

        private void handler_Action_Paste(object sender, EventArgs e = null)
        {
            if (actionCache != null)
            {
                propertyCallback?.Invoke();
                conditionCallback?.Invoke();

                // Добавляем команду
                var newAction = CopyHelper.CreateDeepCopy(actionCache);
                newAction.ActionID = Guid.NewGuid();
                var newNode = newAction.MakeTreeNode();
                var selectedNode = treeActions.SelectedNode;
                if (selectedNode != null)
                {
                    if (selectedNode is UccActionPackTreeNode actPackNode)
                    {
                        // Если выделенный узел является UccActionPackTreeNode
                        // добавляем новую команду в список его узлов
                        actPackNode.Nodes.Add(newNode);
                        actPackNode.UpdateView();
                    }
                    else
                    {
                        // добавляем новую команду после выделенного узла
                        if (selectedNode.Parent is null)
                            treeActions.Nodes.Insert(selectedNode.Index + 1, newNode);
                        else
                        {
                            selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, newNode);
                            (selectedNode.Parent as ActionPackTreeNode)?.UpdateView();
                        }
                    }
                }
                // добавляем новую команду в конец списка узлов дерева
                else treeActions.Nodes.Add(newNode);

                pgProperties.SelectedObject = newAction;
                if (newNode is IActionTreeNode actionTreeNode)
                    propertyCallback = actionTreeNode.UpdateView;

                ChangeWindowCaption();
            }
        }

        private void handler_Action_ShortCut(object sender, KeyEventArgs e)
        {
            //if (!ReferenceEquals(((Control) sender).Parent, panActions)
            //    && !ReferenceEquals(((Control) sender).Parent, treeActions))
            //    return;

            if (e.Control)
            {
                if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                {
                    handler_Action_Copy(sender);
                }
                else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                {
                    handler_Action_Paste(sender);
                }
                else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                {
                    handler_Action_Delete(sender);
                }
            }

            if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
            {
                handler_Action_Add(sender);
            }
        }

        private void handler_Action_Test(object sender, EventArgs e = null)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void handler_Action_TestAll(object sender, EventArgs e = null)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void handler_Action_EditXML(object sender, EventArgs e = null)
        {
            propertyCallback?.Invoke();
            conditionCallback?.Invoke();

            var selectedNode = treeActions.SelectedNode;
            if (selectedNode?.Tag is QuesterAction action
                && XMLEdit.Show(action) is QuesterAction modifiedAction)
            {
                var newActionNode = modifiedAction.MakeTreeNode();
                int selectedInd = selectedNode.Index;
                var parentNode = selectedNode.Parent;
                treeActions.BeginUpdate();
                selectedNode.Remove();
                if (parentNode is null)
                {
                    treeActions.Nodes.Insert(selectedInd, newActionNode);
                }
                else
                {
                    parentNode.Nodes.Insert(selectedInd, newActionNode);
                    (parentNode as IActionTreeNode)?.UpdateView();
                }
                treeActions.EndUpdate();
                treeActions.SelectedNode = newActionNode;
            }
        }

        private void handler_Action_GetFocus(object sender, EventArgs e)
        {
            var node = treeActions.SelectedNode;
            if (node != null)
            {
                var tag = node.Tag;
                if (tag != null && !ReferenceEquals(tag, pgProperties.SelectedObject))
                {
                    ChangeTreeNodeCallback(node);
                    pgProperties.SelectedObject = tag;
                    if (tag is QuesterAction action)
                    {
                        selectedAction = action;
                        gridHotSpots.DataSource = action.HotSpots;
                    }
                }
                //node.BackColor = highlightedTreeNodes.Contains(node)
                //    ? Color.Yellow
                //    : Color.Empty; 
            }
        }
        #endregion




        #region TreeViewNode manipulation
        private void handler_TreeView_NodeSelected(object sender, TreeViewEventArgs e)
        {
            ChangeTreeNodeCallback(e.Node);
            var tag = e.Node.Tag;
            if (tag is QuesterAction action)
            {
                selectedAction = action;
                if (action.UseHotSpots)
                {
                    gridHotSpots.DataSource = action.HotSpots;
                    panHotSpots.Enabled = true;
                }
                else
                {
                    gridHotSpots.DataSource = null;
                    panHotSpots.Enabled = false;
                }
            }
            else if (tag is IsInCustomRegionSet crSet)
            {
#if false
                TypeDescriptor.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));
#elif false
                EntityTools.EntityTools.AddAttributes(crSet.CustomRegions,
                    new EditorAttribute(typeof(CustomRegionSetEditor), typeof(UITypeEditor)));

                var properties = TypeDescriptor.GetProperties(crSet);
#elif false
                pgProperties.SelectedObject = Engine.CustomRegionCollectionDecorator.Decorate(crSet);
                return; 
#else
                crSet.CustomRegions.DebugContext = _profile;
#endif
            }
            pgProperties.SelectedObject = tag;
        }

        void handler_TreeView_Leave(object sender, EventArgs e)
        {
            if (sender is TreeView treeView)
            {
                var selectedNode = treeView.SelectedNode;
                if (selectedNode != null)
                    selectedNode.BackColor = SystemColors.Highlight;
            }
        }

        private void ChangeTreeNodeCallback(TreeNode treeNode)
        {
            if (treeNode is null)
                return;

            switch (treeNode)
            {
                case IActionTreeNode actionNode:
                    conditionCallback?.Invoke();
                    hotSpotCallback?.Invoke();
                    treeConditions.Nodes.Clear();
                    treeConditions.Nodes.AddRange(actionNode.ConditionTreeNodes);
                    propertyCallback = actionNode.UpdateView;
                    if (treeNode.Parent is IActionTreeNode parentPackNode)
                        propertyCallback += parentPackNode.UpdateView;
                    conditionCallback = () => {
                        TreeNode[] condNodes = new TreeNode[treeConditions.Nodes.Count];
                        treeConditions.Nodes.CopyTo(condNodes, 0);
                        actionNode.ConditionTreeNodes = condNodes;
                    };
                    if (treeNode.Tag is QuesterAction action
                        && action.UseHotSpots)
                        hotSpotCallback = actionNode.UpdateView;
                    else hotSpotCallback = null;
                    break;
                case ITreeNode<QuesterCondition> conditionNode:
                    propertyCallback = conditionNode.UpdateView;
                    break;
            }
        }

        private void handler_TreeView_NodeCheckedChanged(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Tag)
            {
                case QuesterAction action:
                    action.Disabled = !e.Node.Checked;
                    if (ReferenceEquals(action, pgProperties.SelectedObject))
                        pgProperties.Refresh();
                    break;
                case QuesterCondition condition:
                    condition.Locked = e.Node.Checked;
                    if (ReferenceEquals(condition, pgProperties.SelectedObject))
                        pgProperties.Refresh();
                    break;
            }

            _profile.Saved = false;
        }

        private void handler_PropertyChanged(object sender, PropertyValueChangedEventArgs e)
        {
            //TODO: Переименование CustomRegion'a
            if (pgProperties.SelectedObject is CustomRegion customRegion)
            {

            }
            propertyCallback?.Invoke();
            _profile.Saved = false;
            ChangeWindowCaption();
        }
        #endregion




        #region Profile manipulation
        private void handler_Profile_New(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_profile.Saved)
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

            _profile.SetProfile(new Profile { Saved = true }, string.Empty);
            UI_fill(_profile);

            txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Make new Profile.");
        }

        private void handler_Profile_Load(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!_profile.Saved)
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
                _profile.SetProfile(prof, path);
                txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Profile '{path}' loaded.");
                UI_fill(_profile);
            }
        }

        private void handler_Profile_Save(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SaveProfile())
            {
                txtLog.AppendText(string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(_profile.FileName)
                        ? string.Empty
                        : " '" + _profile.FileName + "'",
                    " saved.",
                    Environment.NewLine));
                ChangeWindowCaption();
            }
        }

        private void handler_Profile_SaveAs(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SaveProfile(true))
            {
                txtLog.AppendText(string.Concat("[", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "] Profile",
                    string.IsNullOrEmpty(_profile.FileName)
                        ? string.Empty
                        : " '" + _profile.FileName + "'",
                    " saved.",
                    Environment.NewLine));
                ChangeWindowCaption();
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
                propertyCallback?.Invoke();
                conditionCallback?.Invoke();

                // Восстановление команд
                _profile.Actions = ListReconstruction(treeActions.Nodes);

                // Восстановление не требуется, т.к. списки реализованы через BindingList<T>
                //profile.Vendors = ListReconstruction<NPCInfos>(listVendor.Items);
                //profile.CustomRegions = ListReconstruction<CustomRegion>(listCustomRegions.Items);
                //profile.BlackList = ListReconstruction<string>(listBlackList.Items);

                if (saveAs)
                    _profile.SaveAs();
                else _profile.Save();

                DialogResult = DialogResult.OK;
                return _profile.Saved;
            }
            catch (Exception exc)
            {
                XtraMessageBox.Show(exc.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Преобразование списка узлов дерева TreeViewNode в перечисление объектов типа <typeparamref name="QuesterAction"/>
        /// </summary>
        /// <param name="nodes">Коллекция узлов</param>
        /// <returns></returns>
        private IEnumerable<QuesterAction> ListReconstruction(TreeNodeCollection nodes)
        {
            if (nodes?.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    if (node is IActionTreeNode actionNode)
                    {
                        var action = actionNode.ReconstructInternal();
                        if (action != null)
                            yield return action;
                    }
                }
            }
        }

        /// <summary>
        /// Преобразование списка в перечисление объектов типа <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        private IEnumerable<T> ListReconstruction<T>(ListBoxItemCollection items)
        {
            foreach (var item in items)
                if (item is T t)
                    yield return t;
        }
        #endregion




        #region HotSpots manipulation
        private void handler_HotSpot_RowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

#if false
        GridHitInfo downHitInfo = null;

        private void handler_Grid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left
                && sender is GridControl grid
                && grid.MainView is GridView view)
            {
                downHitInfo = null;

                GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
                if (Control.ModifierKeys != Keys.None)
                    return;
                if (hitInfo.InRow && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                    downHitInfo = hitInfo;
            }
        }

        private void handler_Grid_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (downHitInfo != null
                && e.Button == MouseButtons.Left
                && sender is GridControl grid)
            {
                //Size dragSize = SystemInformation.DragSize;
                //Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                //    downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                //if (!dragRect.Contains(new Point(e.X, e.Y)))
                if (Math.Abs(downHitInfo.HitPoint.Y - e.Y) > SystemInformation.DragSize.Height / 2)
                {
                    grid.DoDragDrop(downHitInfo, DragDropEffects.All);
                    downHitInfo = null;
                }
            }
        }

        private void handler_Grid_DragOver(object sender, DragEventArgs e)
        {
            if (sender is GridControl grid
                && grid.MainView is GridView view
                && e.Data.GetDataPresent(typeof(GridHitInfo))
                && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo eventHitInfo)
            {
                GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                if (hitInfo.InRow
                    && hitInfo.RowHandle != eventHitInfo.RowHandle
                    && hitInfo.RowHandle != GridControl.NewItemRowHandle)
                    e.Effect = DragDropEffects.Move;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void handler_Grid_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (sender is GridControl grid
                && grid.MainView is GridView view
                && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo srcHitInfo)
            {
                GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                int sourceRowHandle = srcHitInfo.RowHandle;
                int targetRowHandle = hitInfo.RowHandle;

                if (sourceRowHandle == targetRowHandle || sourceRowHandle == targetRowHandle + 1)
                    return;

                if (grid.DataSource is List<Vector3> hotSpots)
                {
                    view.BeginUpdate();
                    //DataRow targetRow = view.GetDataRow(targetRowHandle);
                    //DataRow row2 = view.GetDataRow(targetRowHandle + 1);
                    //DataRow dragRow = view.GetDataRow(sourceRowHandle);

                    //decimal val1 = (decimal)targetRow[OrderFieldName];
                    //if (row2 == null)
                    //    dragRow[OrderFieldName] = val1 + 1;
                    //else
                    //{
                    //    decimal val2 = (decimal)row2[OrderFieldName];
                    //    dragRow[OrderFieldName] = (val1 + val2) / 2;
                    //} 
                    var spot = hotSpots[sourceRowHandle];
                    if (sourceRowHandle > targetRowHandle)
                    {
                        hotSpots.RemoveAt(targetRowHandle);
                        hotSpots.Insert(sourceRowHandle, spot);
                    }
                    else
                    {
                        hotSpots.RemoveAt(targetRowHandle);
                        hotSpots.Insert(sourceRowHandle + 1, spot);

                    }
                    view.RefreshData();
                    view.EndUpdate();
                }
            }
        }

#else
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
                            _profile.Saved = false;
                            ChangeWindowCaption();
                            break;
                        case InsertType.After:
                            newSpotIndex = targetSpotIndex < sourceHandles[0] ? targetSpotIndex + 1 : targetSpotIndex;
                            for (int i = 0; i < draggedSpots.Count; i++)
                            {
                                var spot = draggedSpots[i];
                                hotSpotsList.Remove(spot);
                                hotSpotsList.Insert(newSpotIndex, spot);
                            }
                            _profile.Saved = false;
                            ChangeWindowCaption();
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
#endif
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
                Node node = _profile.CurrentMesh.ClosestNode(pos.X, pos.Y, pos.Z, out double distance, false);
                gridViewHotSpots.BeginUpdate();
                if (node != null
                    && distance < 10)
                    hotSpots.Add(node.Position);
                else hotSpots.Add(pos);

                _profile.Saved = false;
                ChangeWindowCaption();

                gridViewHotSpots.RefreshData();
                gridViewHotSpots.EndUpdate();

                hotSpotCallback?.Invoke();
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

                _profile.Saved = false;
                ChangeWindowCaption();

                gridViewHotSpots.RefreshData();
                gridViewHotSpots.EndUpdate();

                hotSpotCallback?.Invoke();
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
                selectedControl = listBox;
                var obj = listBox.SelectedItem;
                if (obj != null && !ReferenceEquals(obj, pgProperties.SelectedObject))
                {
                    propertyCallback?.Invoke();
                    pgProperties.SelectedObject = obj;
                    propertyCallback = () => listBox.Refresh();
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

                        var vendors = _profile.Vendors;
                        if (!vendors.Contains(npcInfos))
                        {
                            vendors.Add(npcInfos);
                            listVendor.SelectedItem = npcInfos;

                            _profile.Saved = false;
                            ChangeWindowCaption();
                            return;
                        }

                        XtraMessageBox.Show("This vendor is already in list.");
                        break;
                    }
                case "Delete Vendor":
                    {
                        if (listVendor.SelectedItem is NPCInfos item)
                        {
                            _profile.Vendors.Remove(item);
                            _profile.Saved = false;
                            ChangeWindowCaption();
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
                    _profile.CustomRegions.Remove(item);
                    _profile.Saved = false;
                    ChangeWindowCaption();
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

                        var blackList = _profile.BlackList;
                        if (!blackList.Contains(target.InternalName))
                        {
                            blackList.Add(target.InternalName);
                            _profile.Saved = false;
                            ChangeWindowCaption();
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
                            _profile.BlackList.Remove(item);
                            _profile.Saved = false;
                            ChangeWindowCaption();
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
                mapperForm = new MapperFormExt(_profile);
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
        private bool IsNodeMatchFilter(TreeNode node, string filter)
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
    }
}