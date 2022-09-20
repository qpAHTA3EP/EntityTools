﻿using AcTp0Tools;
using AcTp0Tools.Reflection;
using Astral.Logic.UCC.Classes;
using Astral.Logic.UCC.Forms;
using DevExpress.XtraEditors;
using EntityCore.Tools;
using EntityCore.UCC.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using EntityTools.Tools;
using EntityTools.UCC.Conditions;

namespace EntityCore.Forms
{
    public partial class UccEditor : XtraForm
    {
        //private static readonly XmlSerializer profileSerializer = new XmlSerializerFactory().CreateSerializer(typeof(Profil));
        private static UccEditor uccEditor;
        private Profil uccProfile;
        private string uccProfileFileName;
        private bool profileUnsaved;
        // Функтор, выполняемый при изменении propertyGrid
        private Action propertyCallback;
        // Функтор, выполняемый при изменении списка propertyConditions
        private Action conditionCallback;
        // Скопированная ucc-команда
        private UCCAction uccActionCache;
        // Скопированная ucc-условие
        private UCCCondition uccConditionCache;
        // Скопированный объект TargetPriority
        private TargetPriorityEntry targetPriorityCache;
        // Активный (выбранный) TreeView
        private Control selectedControl;
        // UCC-умение, с которым сопоставлен список ucc-условий
        private UCCAction selectedUccAction;

        private UccEditor()
        {
            InitializeComponent();
            editBasePriority.Properties.Items.AddRange(Enum.GetValues(typeof(Astral.Logic.UCC.Ressources.Enums.TargetPriorityBase)));
            var settingFile = FileTools.UccEditorSettingsFile;
            if (File.Exists(settingFile))
            {
                dockManager.RestoreLayoutFromXml(settingFile);
            }
        }



        /// <summary>
        /// Редактирование ucc-профиля <paramref name="profile"/>
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="param">Дополнительные аргументы:<br/>
        /// - адресная строка файла редактируемого профиля;<br/>
        /// - флаг модального режима</param>
        /// <returns></returns>
        public static bool Edit(Profil profile = null, params object[] param)
        {
            if (uccEditor is null
                || uccEditor.IsDisposed)
                uccEditor = new UccEditor();
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
            return uccEditor.Show(profile, profileName, modal);
        }

        public new void Show()
        {
            Show(null);
        }
        private new void ShowDialog(){}
        private bool Show(Profil profile, string profileFileName = "", bool modal = false)
        {
            UI_reset();

            if (profile != null)
            {
                uccProfile = profile;
                uccProfileFileName = string.IsNullOrEmpty(profileFileName) ? string.Empty : Path.GetFullPath(profileFileName);
                Text = string.IsNullOrEmpty(profileFileName) ? "UCC Editor" : uccProfileFileName;
                profileUnsaved = false;
                UI_fill(uccProfile);
            }
            else
            {
                uccProfile = new Profil();
                uccProfileFileName = string.Empty;
                Text = "UCC Editor";
                profileUnsaved = false;
            }
            txtLog.AppendText(string.Concat("Profile",
                string.IsNullOrEmpty(profileFileName) ? string.Empty : " '" + uccProfileFileName + "'",
                " opened.",
                Environment.NewLine));


#if does_not_work
            documentManager.ContainerControl.ActiveControl = panCombat;
            docCombat.Control.Focus();
            dockManager.DockController.Activate(panCombat);
            tabbedView.ActivateDocument(docCombat.Control);
            selectedControl = treeCombatActions;
            documentManager.DockManager.ActivePanel = panCombat;
            documentGroup.SetSelected(docCombat); 
#endif
            if (modal)
                base.ShowDialog();
            else base.Show();

            return !profileUnsaved;
        }



        #region Drag & Drop
        private void handler_TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
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
                TreeNode draggedNode = (TreeNode)(e.Data.GetData(typeof(UccActionTreeNode))
                                                  ?? e.Data.GetData(typeof(UccActionPackTreeNode))
                                                  ?? e.Data.GetData(typeof(UccConditionPackTreeNode))
                                                  ?? e.Data.GetData(typeof(UccConditionTreeNode))
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
                if (!draggedNode.Equals(targetNode)
                    && !ContainsNode(draggedNode, targetNode))
                {
                    if (targetNode is UccActionPackTreeNode actionPackNode
                        && !altHolded)
                    {
                        // Если удерживается ALT, то dtaggedNode помещается после actionPack'a

                        // Целевой узел является UCCActionPack'ом
                        // Пепремещаем узел во нутрь целевого узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            draggedNode.Remove();
                            actionPackNode.Nodes.Add(draggedNode);
                            ChangeWindowCaption();
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                            ChangeWindowCaption();
                        }

                        // Раскрываем целевой узел дерева, в который перемещен/скопирован перетаскиваемый узел
                        targetNode.Expand();
                    }
                    else if (targetNode is UccConditionPackTreeNode conditionPackNode
                        && !altHolded)
                    {
                        // Если удерживается ALT, то dtaggedNode помещается после actionPack'a

                        // Целевой узел является UCCActionPack'ом
                        // Пепремещаем узел во нутрь целевого узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            draggedNode.Remove();
                            conditionPackNode.Nodes.Add(draggedNode);
                            ChangeWindowCaption();
                        }
                        // Вставляем копию перетаскиваемого узла во внутрь целевого узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' into Children of [{targetNode.Index}]'{targetNode.Text}'");
                            targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                            ChangeWindowCaption();
                        }

                        // Раскрываем целевой узел дерева, в который перемещен/скопирован перетаскиваемый узел
                        targetNode.Expand();
                    }
                    else
                    {
                        // Выбираем коллекцию узлов, в которую нужно добавить перетаскиваемый узел
                        var treeNodeCollection = targetNode.Parent?.Nodes ?? treeView.Nodes;

                        // Если нажата клавиша ALT, вставляем ПЕРЕД targetNode.
                        // В противном случае - после targetNode.
                        //var targetInd = targetNode.Index + (e.KeyState & 32) == 32 ? 0 : 1;// Проверяем нажатие клавиши ALT

                        // Перемещение узла
                        if (e.Effect == DragDropEffects.Move)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' after [{targetNode.Index}]'{targetNode.Text}'");
                            draggedNode.Remove();

                            treeNodeCollection.Insert(targetNode.Index + 1, draggedNode);
                            ChangeWindowCaption();
                        }
                        // Копирование узла
                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' after [{targetNode.Index}]'{targetNode.Text}'");
                            treeNodeCollection.Insert(targetNode.Index + 1, (TreeNode)draggedNode.Clone());
                            ChangeWindowCaption();
                        }
                    }
                }
                else
                {
                    // Перемещаем в конец списка 
                    if (e.Effect == DragDropEffects.Move)
                    {
                        //Logger.WriteLine(Logger.LogType.Debug, $"Move [{draggedNode.Index}]'{draggedNode.Text}' at the of action list");
                        draggedNode.Remove();

                        treeCombatActions.Nodes.Add(draggedNode);
                        ChangeWindowCaption();
                    }
                    // Копирование узла
                    else if (e.Effect == DragDropEffects.Copy)
                    {
                        //Logger.WriteLine(Logger.LogType.Debug, $"Copy [{draggedNode.Index}]'{draggedNode.Text}' at the of action list");
                        treeView.Nodes.Add((TreeNode)draggedNode.Clone());
                        ChangeWindowCaption();
                    }
                } 
            }
        }
        // Determine whether one node is a parent 
        // or ancestor of a second node.
        /// <summary>
        /// Проверяем иерархическое отношене узла <paramref name="parentNode"/> по отношению к <paramref name="childNode"/>
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        /// <returns>True, если <paramref name="childNode"/> является дочерним для <paramref name="parentNode"/>.</returns>
        private bool ContainsNode(TreeNode parentNode, TreeNode childNode)
        {
            // Проверяем наличие родителя у "childNode"
            if (childNode.Parent == null) return false;

            if (childNode.Parent.Equals(parentNode)) return true;

            // Производим рекурсивную проверку иерархического отношения "parentNode" по отношению к родительскому узлу "childNode"
            return ContainsNode(parentNode, childNode.Parent);
        }
        // Set the target drop effect to the effect 
        // specified in the ItemDrag event handler.
        private void handler_TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

                // Select the node at the mouse position.
                var targeTreeNode = treeView.GetNodeAt(targetPoint);
                propertyGrid.SelectedObject = targeTreeNode;
                e.Effect = e.AllowedEffect; 
            }
        }
        // Select the node under the mouse pointer to indicate the 
        // expected drop location.
        private void handler_TreeView_DragOver(object sender, DragEventArgs e)
        {
            if (sender is TreeView treeView)
            {
                // Извлечение перетаскиваемого узла
                TreeNode draggedNode = (TreeNode)(e.Data.GetData(typeof(UccActionTreeNode))
                                                  ?? e.Data.GetData(typeof(UccActionPackTreeNode))
                                                  ?? e.Data.GetData(typeof(UccConditionPackTreeNode))
                                                  ?? e.Data.GetData(typeof(UccConditionTreeNode))
                                                  ?? e.Data.GetData(typeof(TreeNode)));

                if (ReferenceEquals(treeView, draggedNode.TreeView))
                {
                    // Определяем координаты курсора мыши относительно treeView
                    Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

                    var targetNode = treeView.GetNodeAt(targetPoint);

                    // Определяем узел дерева, расположенный под курсором мыши
                    treeView.SelectedNode = targetNode;

#if true
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
#else
                    e.Effect = e.AllowedEffect;  
#endif
                }
                else e.Effect = DragDropEffects.None;
            }
        }
        #endregion



        #region Action's property manipulation
        private void handler_NodeSelected(object sender, TreeViewEventArgs e)
        {
            SetTreeNodeCallback(e.Node);
            selectedControl = sender as TreeView;
            var tag = e.Node.Tag;
            propertyGrid.SelectedObject = tag;
            if (tag is UCCAction uccAction)
                selectedUccAction = uccAction;
        }

        private void SetTreeNodeCallback(TreeNode treeNode)
        {
            switch (treeNode)
            {
                case IUccActionTreeNode uccActionNode:
                    conditionCallback?.Invoke();
                    treeConditions.Nodes.Clear();
                    treeConditions.Nodes.AddRange(uccActionNode.ConditionTreeNodes.ToArray());
                    propertyCallback = uccActionNode.UpdateView;
                    conditionCallback = () =>
                    {
                        TreeNode[] condNodes = new TreeNode[treeConditions.Nodes.Count];
                        treeConditions.Nodes.CopyTo(condNodes, 0);
                        uccActionNode.ConditionTreeNodes = condNodes;
                    };
                    break;
                case IUccTreeNode<UCCCondition> uccConditionNode:
                    propertyCallback = uccConditionNode.UpdateView;
                    break;
            }
        }
        private void handler_NodeCheckedChanged(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Tag)
            {
                case UCCAction uccAction:
                    uccAction.Enabled = e.Node.Checked;
                    if (ReferenceEquals(uccAction, propertyGrid.SelectedObject))
                        propertyGrid.Refresh();
                    break;
                case UCCCondition uccCondition:
                    uccCondition.Locked = e.Node.Checked;
                    if (ReferenceEquals(uccCondition, propertyGrid.SelectedObject))
                        propertyGrid.Refresh();
                    break;
            }
            profileUnsaved = true;
        }
        private void handler_PropertyChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyCallback?.Invoke();
            profileUnsaved = true;
        }
        #endregion



        #region Profile manipucation
        private void handler_LoadProfile(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            openFileDialog.InitialDirectory = Path.Combine(Astral.Controllers.Directories.SettingsPath,"CC");
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var prfl = Astral.Functions.XmlSerializer.Deserialize<Profil>(openFileDialog.FileName, false, 1);
                    if (prfl != null)
                    {
                        UI_reset();
                        UI_fill(prfl);
                        uccProfile = prfl;
                        uccProfileFileName = openFileDialog.FileName;
                        Text = string.IsNullOrEmpty(uccProfileFileName) ? "UCC Editor" : uccProfileFileName;
                        txtLog.AppendText(string.Concat("Profile",
                            string.IsNullOrEmpty(uccProfileFileName) ? String.Empty : " '" + uccProfileFileName + "'",
                            " loaded.",
                            Environment.NewLine));
                        profileUnsaved = false;
                    }
                }
                catch (Exception exc)
                {
                    XtraMessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void handler_ReloadProfile(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (uccProfile != null)
            {
                UI_reset();
                UI_fill(uccProfile);
                txtLog.AppendText(string.Concat("Profile",
                    string.IsNullOrEmpty(uccProfileFileName) ? String.Empty : " '" + uccProfileFileName + "'",
                    " reloaded.",
                    Environment.NewLine));
            }
        }

        private void handler_NewProfile(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uccProfile = new Profil();
            uccProfileFileName = string.Empty;
            Text = "UCC Editor";
            profileUnsaved = false;
            UI_reset();
            txtLog.AppendText("Make blank Profile.\n");
        }

        private void handler_SaveProfile(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*if (string.IsNullOrEmpty(uccProfileFileName))
            {
                saveFileDialog.InitialDirectory = Path.Combine(Astral.Controllers.Directories.SettingsPath, "CC");
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                uccProfileFileName = saveFileDialog.FileName;
            }*/

            if (SaveProfile(uccProfileFileName))
            {
                Text = string.IsNullOrEmpty(uccProfileFileName) ? "UCC Editor" : uccProfileFileName;
                txtLog.AppendText(string.Concat("Profile",
                                                string.IsNullOrEmpty(uccProfileFileName) ? String.Empty : " '" + uccProfileFileName +  "'",
                                                " saved.",
                                                Environment.NewLine));
                profileUnsaved = false;
            }
        }

        private void handler_SaveProfileAs(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            saveFileDialog.InitialDirectory = Path.Combine(Astral.Controllers.Directories.SettingsPath, "CC");
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            if (SaveProfile(saveFileDialog.FileName))
            {
                uccProfileFileName = saveFileDialog.FileName;

                txtLog.AppendText(string.Concat("Profile",
                    string.IsNullOrEmpty(uccProfileFileName) ? String.Empty : " '" + uccProfileFileName + "'",
                    " saved.",
                    Environment.NewLine));
                ChangeWindowCaption();
            }
        }

        /// <summary>
        /// Восстановление списка ucc-команд из дерева TreeNode
        /// </summary>
        /// <param name="nodes">Коллекция узлов</param>
        /// <returns></returns>
        private List<UCCAction> ActionListReconstruction(TreeNodeCollection nodes)
        {
            if (nodes?.Count > 0)
            {
                var actionList = new List<UCCAction>(nodes.Count);
                foreach (TreeNode node in nodes)
                {
                    if (node is IUccActionTreeNode uccNode)
                    {
                        var action = uccNode.ReconstructInternal();
                        if (action != null)
                            actionList.Add(action);
                    }
                }
                return actionList;
            }
            return null;
        }

        /// <summary>
        /// Сохранение профиль в файл <paramref name="fileName"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool SaveProfile(string fileName)
        {
            try
            {
                propertyCallback?.Invoke();
                conditionCallback?.Invoke();

                // Восстановление умений, используемых в бою
                uccProfile.ActionsCombat = ActionListReconstruction(treeCombatActions.Nodes);
                // Восстановление умений, используемых при патрулировании
                uccProfile.ActionsPatrol = ActionListReconstruction(treePatrolActions.Nodes);

                // Восстановление тактики
                {
                    // Использование зелий
                    uccProfile.UsePotions = checkerUsePotion.Checked;
                    uccProfile.SmartPotionUse = checkerSmartPotionUsage.Checked;
                    uccProfile.PotionHealth = Convert.ToInt32(editHealthProcent.Value);

                    uccProfile.EnableTargetPriorities = checkerTacticActivator.Checked;
                    if(editBasePriority.EditValue is Astral.Logic.UCC.Ressources.Enums.TargetPriorityBase targetPriorityBase)
                        uccProfile.TargetPriority = targetPriorityBase;
                    else if(Enum.TryParse(editBasePriority.EditValue.ToString(), out targetPriorityBase))
                        uccProfile.TargetPriority = targetPriorityBase;
                    uccProfile.TargetChangeCD = Convert.ToInt32(editChangeCooldown.Value);
                    uccProfile.TargetPriorityRange = Convert.ToInt32(editBasePriorityRange.Value);

                    IEnumerable<TargetPriorityEntry> GetTargetPriorityEntries()
                    {
                        for(int i = 0; i < listPriorities.ItemCount; i++)
                            if (listPriorities.Items[i] is TargetPriorityEntry target)
                                yield return target;
                    }
                    uccProfile.TargetPriorities = GetTargetPriorityEntries().ToList();
                }

                DialogResult = DialogResult.OK;
                if (string.IsNullOrEmpty(fileName))
                    return true;
#if true
                return Astral.Functions.XmlSerializer.Serialize(fileName, uccProfile, 1);
#else
                using (TextWriter fileStream = new StreamWriter(fileName, false))
                {
                    //Astral_Functions_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types, 2);

                    profileSerializer.Serialize(fileStream, profile); 
                    return true;
                }
#endif 

            }
            catch (Exception exc)
            {
                XtraMessageBox.Show(exc.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void handler_ProfileToEngine(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (AstralAccessors.Controllers.Roles.IsRunning)
            {
                XtraMessageBox.Show("Stop the bot before exporting Ucc-Profile into the UccEngine.");
                return;
            }
            // Пересобираем профиль
            if (profileUnsaved)
                SaveProfile("");

            Astral.Logic.UCC.Core.Get.mProfil = CopyHelper.CreateDeepCopy(uccProfile);
        }

        /// <summary>
        /// Отображение профиля <paramref name="profile"/> в окне редактора.
        /// </summary>
        /// <param name="profile"></param>
        private void UI_fill(Profil profile)
        {
            if(profile is null)
                return;

            // Отображение дерева умений, используемых в бою
            if (profile.ActionsCombat?.Count > 0)
            {
                treeCombatActions.Nodes.Clear();
                treeCombatActions.Nodes.AddRange(profile.ActionsCombat.ToTreeNodes(true));
            }
            // Отображение дерева умений, 
            if (profile.ActionsPatrol?.Count > 0)
            {
                treePatrolActions.Nodes.Clear();
                treePatrolActions.Nodes.AddRange(profile.ActionsPatrol.ToTreeNodes(true));
            }
            // Отображение тактики
            {
                // Использование зелий
                checkerUsePotion.Checked = profile.UsePotions;
                checkerSmartPotionUsage.Checked = profile.SmartPotionUse;
                editHealthProcent.Value = profile.PotionHealth == 0 ? 40 : profile.PotionHealth;

                checkerTacticActivator.Checked = profile.EnableTargetPriorities;
                handler_TacticUsageChanged(checkerTacticActivator);
                editBasePriority.EditValue = profile.TargetPriority;
                editChangeCooldown.Value = profile.TargetChangeCD;
                editBasePriorityRange.Value = profile.TargetPriorityRange;

                listPriorities.Items.Clear();
                if(profile.TargetPriorities?.Count > 0)
                    listPriorities.Items.AddRange(profile.TargetPriorities.ToArray());
            }
        }

        private void UI_reset()
        {
            treeCombatActions.Nodes.Clear();
            treePatrolActions.Nodes.Clear();
            treeConditions.Nodes.Clear();
            listPriorities.Items.Clear();
            propertyGrid.SelectedObject = null;
            selectedControl = null;
            propertyCallback = null;
            conditionCallback = null;
        }
        
        private void ChangeWindowCaption(bool saved = false)
        {
            Text = (saved ? string.Empty : "* ")
                 + (string.IsNullOrEmpty(uccProfileFileName) ? "New ucc-Profile" : uccProfileFileName);
            profileUnsaved = !saved;
        }

        private void handler_Closing(object sender, FormClosingEventArgs e)
        {
            if (profileUnsaved)
            {
                switch (XtraMessageBox.Show("Profile was modified but did not saved!\n" +
                                            "Would you like to save it ?\n" +
                                            "Press 'Cancel' button to continue profile edition.", "Caution!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.Yes:
                        SaveProfile(uccProfileFileName);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            dockManager.SaveLayoutToXml(FileTools.UccEditorSettingsFile);
        }
       #endregion


        
        #region List of Priorities
        private int selectedTargetPriorityInd;
        private Point pointPrioritiesItem;

        private void handler_TacticUsageChanged(object sender, EventArgs e = null)
        {
            groupPriority.Enabled = checkerTacticActivator.Checked;
        }

        private void handler_Priorities_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left
                && ReferenceEquals(sender, listPriorities))
            {
                pointPrioritiesItem = new Point(e.X, e.Y);
                selectedTargetPriorityInd = listPriorities.IndexFromPoint(pointPrioritiesItem);
                if (selectedTargetPriorityInd >= 0)
                    return;
            }
            pointPrioritiesItem = Point.Empty;
            selectedTargetPriorityInd = -1;
        }

        private void handler_Priorities_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (pointPrioritiesItem != Point.Empty
                    && Math.Abs(e.Y - pointPrioritiesItem.Y) > SystemInformation.DragSize.Height)
                    listPriorities.DoDragDrop(sender, DragDropEffects.Move);
        }

        private void handler_Priorities_DragOver(object sender, DragEventArgs e)
        {
            if (ReferenceEquals(sender, listPriorities))
            {
                pointPrioritiesItem = new Point(e.X, e.Y);
                selectedTargetPriorityInd = listPriorities.IndexFromPoint(pointPrioritiesItem);
                if (selectedTargetPriorityInd >= 0)
                    listPriorities.SelectedIndex = selectedTargetPriorityInd;
            }
            e.Effect = DragDropEffects.Move;
        }

        private void handler_Priorities_DragDrop(object sender, DragEventArgs e)
        {
            if (ReferenceEquals(sender, listPriorities)
                && selectedTargetPriorityInd >= 0)
            {
                Point newPoint = new Point(e.X, e.Y);
                newPoint = listPriorities.PointToClient(newPoint);
                int currentInd = listPriorities.IndexFromPoint(newPoint);
                object item = listPriorities.Items[selectedTargetPriorityInd];
                if (currentInd == -1)
                {
                    listPriorities.Items.RemoveAt(selectedTargetPriorityInd);
                    listPriorities.Items.Add(item);
                    ChangeWindowCaption();
                }
                else if (selectedTargetPriorityInd != currentInd)
                {
                    if (selectedTargetPriorityInd > currentInd)
                    {
                        listPriorities.Items.RemoveAt(selectedTargetPriorityInd);
                        listPriorities.Items.Insert(currentInd + 1, item);
                    }
                    else
                    {
                        listPriorities.Items.RemoveAt(selectedTargetPriorityInd);
                        listPriorities.Items.Insert(currentInd, item);
                    }
                    ChangeWindowCaption();
                }
                pointPrioritiesItem = Point.Empty;
                selectedTargetPriorityInd = -1;
            }
        }

        private void handler_SelectedPriorityChanged(object sender, EventArgs e)
        {
            selectedControl = listPriorities;
            conditionCallback = null;
            var obj = listPriorities.SelectedItem;
            if (obj != null)
            {
                propertyGrid.SelectedObject = obj;
                propertyCallback = () => listPriorities.Refresh();
            }
            else propertyCallback = null;
        }

        private void handler_PriorityAdd(object sender, EventArgs e = null)
        {
            // Добавляем объект TargetPriority
            TargetPriorityEntry targetPriorityEntry = ChoosePriority.Show();
            if (targetPriorityEntry != null)
            {
                var selectedPriorityInd = listPriorities.SelectedIndex;
                if (selectedPriorityInd >= 0)
                    listPriorities.Items.Insert(selectedPriorityInd + 1, targetPriorityEntry);
                else listPriorities.Items.Add(targetPriorityEntry);

                ChangeWindowCaption();
            }
        }

        private void handler_PriorityDelete(object sender, EventArgs e = null)
        {
            // Удаляем объект TargetPriority
            var selectedPriorityInd = listPriorities.SelectedIndex;
            if (selectedPriorityInd >= 0)
            {
                listPriorities.Items.RemoveAt(selectedPriorityInd);
                ChangeWindowCaption();
            }
        }

        private void handler_PriorityShortCut(object sender, KeyEventArgs e)
        {
            if (ReferenceEquals(sender, listPriorities))
            {
                if (e.Control)
                {
                    if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                    {
                        if (listPriorities.SelectedItem is TargetPriorityEntry priorityEntry)
                            targetPriorityCache = CopyHelper.CreateDeepCopy(priorityEntry);
                    }
                    else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                    {
                        if (targetPriorityCache != null)
                        {
                            var targetPriorityEntry = CopyHelper.CreateDeepCopy(targetPriorityCache);
                            var selectedPriorityInd = listPriorities.SelectedIndex;
                            if (selectedPriorityInd >= 0)
                                listPriorities.Items.Insert(selectedPriorityInd + 1, targetPriorityEntry);
                            else listPriorities.Items.Add(targetPriorityEntry);

                            ChangeWindowCaption();
                        }
                    }
                    else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                    {
                        handler_PriorityDelete(sender);
                    } 
                    return;
                }
                if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
                {
                    handler_PriorityAdd(sender);
                }
            }
        }
        #endregion



        private void handler_Focused(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, panConditions)
                || ReferenceEquals(sender, treeConditions))
            {
                selectedControl = treeConditions;
                var selectedNode= treeConditions.SelectedNode;
                propertyGrid.SelectedObject = selectedNode?.Tag;
                SetTreeNodeCallback(selectedNode);
            }
            else if (ReferenceEquals(sender, panCombat)
                     || ReferenceEquals(sender, treeCombatActions))
            {
                selectedControl = treeCombatActions;
                var selectedNode = treeCombatActions.SelectedNode;
                var tag = selectedNode?.Tag;
                propertyGrid.SelectedObject = tag;
                selectedUccAction = tag as UCCAction;
                SetTreeNodeCallback(selectedNode);
            }
            else if (ReferenceEquals(sender, panPatrol)
                     || ReferenceEquals(sender, treePatrolActions))
            {
                selectedControl = treePatrolActions;
                var selectedNode = treePatrolActions.SelectedNode;
                var tag = selectedNode?.Tag;
                propertyGrid.SelectedObject = tag;
                selectedUccAction = tag as UCCAction;
                SetTreeNodeCallback(selectedNode);
            }
            else if (ReferenceEquals(sender, panTactic))
            {
                selectedControl = listPriorities;
                propertyGrid.SelectedObject = listPriorities.SelectedItem;
                propertyCallback = () => listPriorities.Refresh();
                treeConditions.Nodes.Clear();
                conditionCallback = null;
            }
            else
            {
                selectedControl = null;
                propertyGrid.SelectedObject = null;
                conditionCallback = null;
                propertyCallback = null;
            }
        }




        #region Conditions
        private void handler_ConditionAdd(object sender, EventArgs e = null)
        {
            // Добавление UCCCondition
            if (ItemSelectForm.GetAnInstance(out UCCCondition newCondition, false))
            {
                if (newCondition.MakeTreeNode() is IUccTreeNode<UCCCondition> newTreeNode)
                {
                    var selectedNode = treeConditions.SelectedNode;
                    if (selectedNode != null)
                    {
                        if (selectedNode is UccActionPackTreeNode actPackNode)
                            // Если выделенный узел является UccActionPackTreeNode
                            // добавляем новую команду в список его узлов
                            actPackNode.Nodes.Add((TreeNode)newTreeNode);
                        else
                        {
                            // добавляем новую команду после выделенного узла
                            if (selectedNode.Parent is null)
                                treeConditions.Nodes.Insert(selectedNode.Index + 1, (TreeNode)newTreeNode);
                            else
                                selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, (TreeNode)newTreeNode);
                        }
                    }
                    // добавляем новую команду в конец списка узлов дерева
                    else treeConditions.Nodes.Add((TreeNode)newTreeNode);

                    propertyGrid.SelectedObject = newCondition;
                    propertyCallback = newTreeNode.UpdateView;

                    ChangeWindowCaption();
                }
            }
        }

        private void handler_ConditionDelete(object sender, EventArgs e = null)
        {
            var conditionNode = treeConditions.SelectedNode;

            if (conditionNode != null)
            {
                if (conditionNode is UccConditionPackTreeNode)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ConditionPack",
                            "Confirmation",
                            MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                if (ReferenceEquals(propertyGrid.SelectedObject, conditionNode.Tag))
                    propertyCallback = null;
                propertyGrid.SelectedObject = null;
                conditionNode.Remove(); 
            }
        }

        private void handler_ConditionCopy(object sender, EventArgs e = null)
        {
            if (treeConditions.SelectedNode?.Tag is UCCCondition uccCondition)
                uccConditionCache = CopyHelper.CreateDeepCopy(uccCondition);
        }

        private void handler_ConditionPaste(object sender, EventArgs e = null)
        {
            if (uccConditionCache != null)
            {
                // Добавляем UCC-команду
                var newCondition = CopyHelper.CreateDeepCopy(uccConditionCache);
                var newNode = newCondition.MakeTreeNode();
                var selectedNode = treeConditions.SelectedNode;
                if (selectedNode != null)
                {
                    if (selectedNode is UccConditionPackTreeNode conditionPackNode)
                        // Если выделенный узел является UccActionPackTreeNode
                        // добавляем новую команду в список его узлов
                        conditionPackNode.Nodes.Add(newNode);
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

                propertyGrid.SelectedObject = newCondition;
                if (newNode is IUccTreeNode<UCCCondition> uccConditionTreeNode)
                    propertyCallback = uccConditionTreeNode.UpdateView;

                ChangeWindowCaption();
            }
        }

        private void handler_ConditionShortCut(object sender, KeyEventArgs e)
        {
            if (ReferenceEquals(sender, treeConditions))
            {
                if (e.Control)
                {
                    if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                    {
                        handler_ConditionCopy(sender);
                    }
                    else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                    {
                        handler_ConditionPaste(sender);
                    }
                    else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                    {
                        handler_ConditionDelete(sender);
                    } 
                    return;
                }

                if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
                {
                    handler_ConditionAdd(sender);
                }
            }
        }

        private void handler_ConditionTest(object sender, EventArgs e)
        {
            if (treeConditions?.SelectedNode?.Tag is UCCCondition uccCnd)
            {
                UCCAction uccAction = selectedUccAction;
                //if (documentGroup.SelectedDocument == docCombat)
                //    uccAction = treeCombatActions.SelectedNode?.Tag as UCCAction;
                //else if (documentGroup.SelectedDocument == docPatrol)
                //    uccAction = treePatrolActions.SelectedNode?.Tag as UCCAction;
                //else
                //{
                //    XtraMessageBox.Show(
                //        "Testing of Tactic is unavailable. Select Combat or Patrol ucc-action list first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                if (uccAction is null)
                {
                    XtraMessageBox.Show(
                        "No ucc-action selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Тестирование выбранного условия
                bool result;
                string msg;
                if (uccCnd is ICustomUCCCondition iCond)
                {
                    result = iCond.IsOK(uccAction);
                    msg = iCond.TestInfos(uccAction);
                }
                else
                {
                    result = uccCnd.IsOK(uccAction);
                    msg = $"{uccCnd.Target} {uccCnd.Tested} : {uccCnd.Value}";
                }

                if (string.IsNullOrEmpty(msg))
                    msg = uccCnd.ToString();

                msg = string.Concat(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] "),
                                    "Testing selected condition:", Environment.NewLine,
                                    "   ", msg, Environment.NewLine,
                                    "Result: ", result, Environment.NewLine);

                txtLog.AppendText(msg);
                XtraMessageBox.Show(msg, "Condition Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else XtraMessageBox.Show(
                "No ucc-condition selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void handler_ConditionTestAll(object sender, EventArgs e)
        {
            var nodes = treeConditions.Nodes;
            var nodeCount = nodes.Count;
            if (nodeCount == 0)
            {
                XtraMessageBox.Show(
                    "Ucc-condition list is empty.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            UCCAction uccAction = selectedUccAction;
            //if (documentGroup.SelectedDocument == docCombat)
            //    uccAction = treeCombatActions.SelectedNode?.Tag as UCCAction;
            //else if (documentGroup.SelectedDocument == docPatrol)
            //    uccAction = treePatrolActions.SelectedNode?.Tag as UCCAction;
            //else
            //{
            //    XtraMessageBox.Show(
            //        "Testing of Tactic is unavailable. Select Combat or Patrol ucc-action list first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (uccAction is null)
            {
                XtraMessageBox.Show(
                    "No ucc-action selected.", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int okLockedNum = 0;    // Счетчик количества истиных залоченных условий
            int okUnlockedNum = 0;  // счетчик количества истиных незалоченных условий
            bool lockedTrue = true; // флаг истинности всех залоченных условий 

            var strBuilder = new StringBuilder();
            strBuilder.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ")).Append("Testing the list of ").Append(nodeCount).AppendLine(" ucc-condition:");
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is UCCCondition uccCnd)
                {
                    bool result;
                    string msg;
                    if (uccCnd is ICustomUCCCondition iCond)
                    {
                        result = iCond.IsOK(uccAction);
                        msg = iCond.TestInfos(uccAction);
                    }
                    else
                    {
                        result = uccCnd.IsOK(uccAction);
                        msg = $"{uccCnd.Target} {uccCnd.Tested} : {uccCnd.Value}";
                    }

                    if (string.IsNullOrEmpty(msg))
                        msg = uccCnd.ToString();

                    if (uccCnd.Locked)
                    {
                        strBuilder.Append("   [L] ");
                        lockedTrue &= result;
                        if (result)
                            okLockedNum++;
                    }
                    else
                    {
                        strBuilder.Append("   [U] ");
                        if (result)
                            okUnlockedNum++;
                    }
                    strBuilder.Append(msg).Append(" => ").AppendLine(result.ToString());
                }
            }

#if true            
            // Поскольку обработка списка условий производится встроенным алготимом Бота
            // с флагом uccAction.OneCondMustGood итоговый результат ложный,
            // если все условия залочены и они истины (т.е. нет ни одного незаложченных истиного условия).
            bool totalResult = lockedTrue
                               // Соответствует правилу дизъюнкции с учетом замечания указанного выше 
                               && (uccAction.OneCondMustGood && okUnlockedNum > 0
                                   // соответствует правилу конъюнкции
                                   || nodeCount == okLockedNum + okUnlockedNum);
#else               
            // Альтернативный корректный результат проверки с флагом uccAction.OneCondMustGood 
            // Вычислялся бы следующим образом
            bool totalResult = lockedTrue 
                                    // Соответствует правилу дизъюнкции
                                && (uccAction.OneCondMustGood && (nodeCount == okLockedNum || okUnlockedNum > 0)
                                    // соответствует правилу конъюнкции
                                    || nodeCount == okLockedNum + okUnlockedNum);
#endif
            strBuilder.Append("Result: ").AppendLine(totalResult.ToString());
            var resutlMsg = strBuilder.ToString();
            txtLog.AppendText(resutlMsg);
            XtraMessageBox.Show(resutlMsg, "Condition Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion



        #region Action lists
        private void handler_ActionAdd(object sender, EventArgs e = null)
        {
            TreeView selectedTreeView;
            if (ReferenceEquals(sender, treeCombatActions))
                selectedTreeView = treeCombatActions;
            else if (ReferenceEquals(sender, treePatrolActions))
                selectedTreeView = treePatrolActions;
            else return;
            
            // Добавление UCCAction
            if (AddUccActionForm.GUIRequest(out UCCAction action))
            {
                if (action.MakeTreeNode() is IUccActionTreeNode newTreeNode)
                {
                    var selectedNode = selectedTreeView.SelectedNode;
                    if (selectedNode != null)
                    {
                        if (selectedNode is UccActionPackTreeNode actPackNode)
                            // Если выделенный узел является UccActionPackTreeNode
                            // добавляем новую команду в список его узлов
                            actPackNode.Nodes.Add((TreeNode)newTreeNode);
                        else
                        {
                            // добавляем новую команду после выделенного узла
                            if (selectedNode.Parent is null)
                                selectedTreeView.Nodes.Insert(selectedNode.Index + 1, (TreeNode)newTreeNode);
                            else
                                selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, (TreeNode)newTreeNode);
                        }
                    }
                    // добавляем новую команду в конец списка узлов дерева
                    else selectedTreeView.Nodes.Add((TreeNode)newTreeNode);

                    propertyGrid.SelectedObject = action;
                    propertyCallback = newTreeNode.UpdateView;

                    ChangeWindowCaption();
                }
            }
        }

        private void handler_ActionDelete(object sender, EventArgs e = null)
        {
            TreeView selectedTreeView;
            if (ReferenceEquals(sender, treeCombatActions))
                selectedTreeView = treeCombatActions;
            else if (ReferenceEquals(sender, treePatrolActions))
                selectedTreeView = treePatrolActions;
            else return;

            var actionNode = selectedTreeView.SelectedNode;

            if (actionNode != null)
            {
                if (actionNode is UccActionPackTreeNode)
                {
                    if (XtraMessageBox.Show("Confirm deleting of the ActionPack",
                            "Confirmation",
                            MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                }
                conditionCallback = null;
                propertyCallback = null;
                treeConditions.Nodes.Clear();
                selectedUccAction = null;
                propertyGrid.SelectedObject = null;
                actionNode.Remove(); 
            }
        }

        private void handler_ActionCopy(object sender, EventArgs e = null)
        {
            if (sender is TreeView selectedTreeView
                && selectedTreeView.SelectedNode?.Tag is UCCAction uccAction)
                // Копируем ucc-команду
                uccActionCache = uccAction.Clone();//CopyHelper.CreateDeepCopy(uccAction);
        }

        private void handler_ActionPaste(object sender, EventArgs e = null)
        {
            TreeView selectedTreeView;
            if (ReferenceEquals(sender, treeCombatActions))
                selectedTreeView = treeCombatActions;
            else if (ReferenceEquals(sender, treePatrolActions))
                selectedTreeView = treePatrolActions;
            else return;

            if (uccActionCache != null)
            {
                // Добавляем UCC-команду
                var newAction = uccActionCache.Clone();//CopyHelper.CreateDeepCopy(uccActionCache);
                var newNode = newAction.MakeTreeNode();
                var selectedNode = selectedTreeView.SelectedNode;
                if (selectedNode != null)
                {
                    if (selectedNode is UccActionPackTreeNode actPackNode)
                        // Если выделенный узел является UccActionPackTreeNode
                        // добавляем новую команду в список его узлов
                        actPackNode.Nodes.Add(newNode);
                    else
                    {
                        // добавляем новую команду после выделенного узла
                        if (selectedNode.Parent is null)
                            selectedTreeView.Nodes.Insert(selectedNode.Index + 1, newNode);
                        else selectedNode.Parent.Nodes.Insert(selectedNode.Index + 1, newNode);
                    }
                }
                // добавляем новую команду в конец списка узлов дерева
                else selectedTreeView.Nodes.Add(newNode);

                propertyGrid.SelectedObject = newAction;
                if (newNode is IUccActionTreeNode uccActionTreeNode)
                    propertyCallback = uccActionTreeNode.UpdateView;

                ChangeWindowCaption();
            }
        }

        private void handler_ActionShortCut(object sender, KeyEventArgs e)
        {
            if (!ReferenceEquals(sender, treeCombatActions)
                && !ReferenceEquals(sender, treePatrolActions))
                return;

            if (e.Control)
            {
                if (e.KeyCode == Keys.C || e.KeyData == Keys.C)
                {
                    handler_ActionCopy(sender);
                }
                else if (e.KeyCode == Keys.V || e.KeyData == Keys.V)
                {
                    handler_ActionPaste(sender);
                }
                else if (e.KeyCode == Keys.Delete || e.KeyData == Keys.Delete)
                {
                    handler_ActionDelete(sender);
                }
            }
            if (e.KeyCode == Keys.Insert || e.KeyData == Keys.Insert)
            {
                handler_ActionAdd(sender);
            }
        }

        private void handler_ActionTest(object sender, EventArgs e)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void handler_AcionTestAll(object sender, EventArgs e)
        {
            XtraMessageBox.Show(
                "Not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}