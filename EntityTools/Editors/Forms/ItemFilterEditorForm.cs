using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Quester.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using EntityTools.Enums;
using EntityTools.Tools.BuySellItems;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace EntityTools.Forms
{
    public partial class ItemFilterEditorForm : /* Form //*/XtraForm
    {
        static ItemFilterEditorForm @this;

        /// <summary>
        /// Список категорий предметов
        /// </summary>
        static readonly IEnumerable<ItemCategory> categories = Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>().ToList();

        /// <summary>
        /// Списко-фильтр
        /// </summary>
        BindingList<ItemFilterEntryExt> filter = new BindingList<ItemFilterEntryExt>();

        /// <summary>
        /// Функтор, заполняющий фильтр исходными значениями
        /// </summary>
        Action fillListAction;

        private ItemFilterEditorForm()
        {
            InitializeComponent();

#if false
            RepositoryItemComboBox repositoryItemComboBox = new RepositoryItemComboBox();
            repositoryItemComboBox.QueryPopUp += this.method_0;
            repositoryItemComboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.btnReverse.Visible = false;
            this.colEntryType.ColumnEdit = repositoryItemComboBox; 
#endif
        }

        public static bool GUIRequiest(ref List<ItemFilterEntryExt> list)
        {
            if (@this == null)
                @this = new ItemFilterEditorForm();

            // Копирование исходного списка для возможности отката
            List<ItemFilterEntryExt> originalList = list;
            if (list != null)
                @this.fillListAction = () =>
                {
                    @this.filter.Clear();
                    if (originalList != null)
                        foreach (var f in originalList)
                            @this.filter.Add(Reflection.CopyHelper.CreateDeepCopy(f));
                };
            else @this.fillListAction = null;

            @this.purchaseOptions.DataSource = @this.filter;

            if(@this.ShowDialog() == DialogResult.OK)
            {
                list = @this.filter.ToList();
                return true;
            }
            else return false;
        }

        #region Обработчики
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
#if false
            string text = string.Empty;
            List<string> list = new List<string>();
            foreach (InventorySlot inventorySlot in EntityManager.LocalPlayer.AllItems)
            {
                if (!list.Contains(inventorySlot.Item.ItemDef.DisplayName) && this.Filter.method_0(inventorySlot.Item))
                {
                    list.Add(inventorySlot.Item.ItemDef.DisplayName);
                    text = string.Concat(new string[]
                    {
                        text,
                        inventorySlot.Item.ItemDef.DisplayName,
                        " [",
                        inventorySlot.Item.ItemDef.InternalName,
                        "]",
                        Environment.NewLine
                    });
                }
            }
            XtraMessageBox.Show(text);  
#endif
            IndexedBags bag = new IndexedBags(filter);
            string bagDskr = bag.Description();
            if (string.IsNullOrEmpty(bagDskr))
                bagDskr = "No item matches";
            XtraMessageBox.Show(bagDskr);
#if false
            StringBuilder sb = new StringBuilder();
            foreach (var f in filter)
            {
                var list = bag[f];
                if (list != null && list.Count > 0)
                {
                    sb.Append(f.ToString()).AppendLine(" contains:");
                    foreach (var slot in list)
                    {
                        sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append("] {");
                        int catNum = 0;
                        foreach (var cat in slot.Item.ItemDef.Categories)
                        {
                            if (catNum > 0) sb.Append(", ");
                            sb.Append(cat);
                            catNum++;
                        }
                        sb.AppendLine("}");
                    }
                }
            } 
#endif
        }

        /// <summary>
        /// Очистка всего списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Are you sure to clear the filter list ?", "Delete all entry?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                filter.Clear();
            }
        }

        /// <summary>
        /// Экспорт фильтра в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.SettingsPath;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = nameof(ItemFilterEntryExt) + " filter profile (*.xml)|*.xml";
            saveFileDialog.FileName = nameof(ItemFilterEntryExt) + "_Filters.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
#if false
                Astral.Functions.XmlSerializer.Serialize(saveFileDialog.FileName, filter.ToList()); 
#else
                XmlSerializer serialiser = new XmlSerializer(/*typeof(SettingsContainer)*/filter.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serialiser.Serialize(file, filter);
                }
#endif
            }
        }

        /// <summary>
        /// Импорт фильтра из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directories.SettingsPath;
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = nameof(ItemFilterEntryExt) + " filter profile (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
#if false
                List<ItemFilterEntryExt> newFilter = Astral.Functions.XmlSerializer.Deserialize<List<ItemFilterEntryExt>>(openFileDialog.FileName, false);
#else
                List<ItemFilterEntryExt> newFilter = null;

                XmlSerializer serialiser = new XmlSerializer(typeof(List<ItemFilterEntryExt>));
                using (StreamReader fileStream = new StreamReader(openFileDialog.FileName))
                {
                    if(serialiser.Deserialize(fileStream) is List<ItemFilterEntryExt> list)
                    {
                        newFilter = list;
                    }
                }
#endif
                if (newFilter != null && newFilter.Count > 0)
                {
                    if (filter.Count > 0)
                    {
                        switch (XtraMessageBox.Show("Add to current list ? (Else, clear it before)", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilter)
                                    filter.Add(item);
                                return;
                            case DialogResult.No: // Замена существующего списка
                                filter.Clear();
                                foreach (var item in newFilter)
                                    filter.Add(item);
                                return;
                            default:
                                return;
                        }
                    }
                    else
                    {
                        foreach (var item in newFilter)
                            filter.Add(item);
                    }
                }
                else XtraMessageBox.Show("Empty or file opening error.");
            }
        }

        /// <summary>
        /// Сборос списка фильтра к исходному состоянию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReload_Click(object sender, EventArgs e)
        {
            purchaseOptions.DataSource = null;
            fillListAction?.Invoke();
            purchaseOptions.DataSource = filter;
        }

        /// <summary>
        /// Вызов внешнего редактора для выбора Identifier для элемента фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditItemFilterEntry_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (purchaseOptions.FocusedView is DevExpress.XtraGrid.Views.Grid.GridView gridView)
            {
                if (gridView.GetFocusedRowCellValue(colEntryType) is ItemFilterEntryType fType)
                {
                    if (fType == ItemFilterEntryType.Category)
                    {
                        //Выбор категории
                        ItemCategory cat = ItemCategory.None;
                        if (EntityTools.Core.GUIRequest_Item(() => categories, ref cat))
                        {
                            gridView.SetFocusedValue(cat.ToString());
                        }
                    }
                    else
                    {
                        //Выбор предмета
                        GetAnItem.ListItem rewardItem = null;
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.Store
                            || EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.StoreCollection)
                            rewardItem = GetAnItem.Show(1);
                        else rewardItem = GetAnItem.Show(0);

                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                        {
                            gridView.SetFocusedValue(rewardItem.ItemId);
                        }
                    }
                }
            }
        }

        private void bntSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        private void ItemFilterEditorForm_Shown(object sender, EventArgs e)
        {
            fillListAction?.Invoke();
        }
    }
}
