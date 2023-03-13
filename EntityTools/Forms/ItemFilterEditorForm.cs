using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

using Infrastructure.Classes;
using Infrastructure.Reflection;

using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Quester.Forms;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;

using EntityTools.Forms;

using EntityTools.Enums;
using EntityTools.Tools.Inventory;

using MyNW.Internals;
using MyNW.Patchables.Enums;

namespace EntityTools.Forms
{
    public partial class ItemFilterEditorForm : XtraForm
    {
        static ItemFilterEditorForm @this;
        private static XmlSerializer ItemFilterEntryListSerializer = new XmlSerializer(typeof(List<ItemFilterEntryExt>));

        /// <summary>
        /// Список категорий предметов
        /// </summary>
        static readonly IEnumerable<ItemCategory> categories = Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>().ToList();
        /// <summary>
        /// Список типов предметов
        /// </summary>
        static readonly IEnumerable<ItemType> itemTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList();

        /// <summary>
        /// Список-фильтров
        /// </summary>
        ObservableCollection<ItemFilterEntryExt> filter = new ObservableCollection<ItemFilterEntryExt>();

        /// <summary>
        /// Функтор, заполняющий фильтр исходными значениями
        /// </summary>
        Action fillListAction;

        private ItemFilterEditorForm()
        {
            InitializeComponent();
        }

        public static bool GUIRequiest(ref List<ItemFilterEntryExt> list)
        {
            if (@this is null)
                @this = new ItemFilterEditorForm();

            // Копирование исходного списка для возможности отката
            List<ItemFilterEntryExt> originalList = list;
            if (list != null)
                @this.fillListAction = () =>
                {
                    @this.filter.Clear();
                    if (originalList?.Count > 0)
                    {
                        @this.filter = new ObservableCollection<ItemFilterEntryExt>(originalList.CreateDeepCopy());
                    }
                };
            else @this.fillListAction = null;

            @this.purchaseOptions.DataSource = @this.filter;

            if(@this.ShowDialog() == DialogResult.OK)
            {
                list = @this.filter.ToList();
                return true;
            }

            return false;
        }

        #region Обработчики
        private void btnTest_Click(object sender, EventArgs e)
        {
            IndexedBags bag = new IndexedBags(filter, BagsList.GetFullPlayerInventory());
            string bagDskr = bag.Description();
            if (string.IsNullOrEmpty(bagDskr))
                bagDskr = "No item matches";
            XtraMessageBox.Show(bagDskr);
        }

        /// <summary>
        /// Очистка всего списка
        /// </summary>
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
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directories.SettingsPath,
                DefaultExt = "xml",
                Filter = nameof(ItemFilterEntryExt) + " filter profile (*.xml)|*.xml",
                FileName = nameof(ItemFilterEntryExt) + "_Filters.xml"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(filter.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serializer.Serialize(file, filter);
                }
            }
        }

        /// <summary>
        /// Импорт фильтра из файла
        /// </summary>
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = FileTools.GetOpenDialog(filter: nameof(ItemFilterEntryExt) + " filter profile (*.xml)|*.xml",
                                                                defaultExtension: "xml",
                                                                initialDir: Directories.SettingsPath);
            
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                List<ItemFilterEntryExt> newFilter = null;

                XmlSerializer serializer = ItemFilterEntryListSerializer;
                using (StreamReader fileStream = new StreamReader(openDialog.FileName))
                {
                    if(serializer.Deserialize(fileStream) is List<ItemFilterEntryExt> list)
                    {
                        newFilter = list;
                    }
                }
                if (newFilter != null && newFilter.Count > 0)
                {
                    if (filter.Count > 0)
                    {
                        switch (XtraMessageBox.Show("Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
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

                    foreach (var item in newFilter)
                        filter.Add(item);
                }
                else
                {
                    ItemFilterCore newFilterCore = Astral.Functions.XmlSerializer.Deserialize<ItemFilterCore>(openDialog.FileName);

                    if (newFilterCore != null && newFilterCore.Entries.Count > 0)
                    {
                        switch (XtraMessageBox.Show("There are ItemFilter at the Astral's format in the File.\n\r" +
                            "You can import only ItemId and Category entry from it.\n\r" +
                            "Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilterCore.Entries)
                                    if(item.Type == ItemFilterType.ItemID || item.Type == ItemFilterType.ItemCatergory)
                                        filter.Add(new ItemFilterEntryExt(item));
                                return;
                            case DialogResult.No: // Замена существующего списка
                                filter.Clear();
                                foreach (var item in newFilterCore.Entries)
                                    if (item.Type == ItemFilterType.ItemID || item.Type == ItemFilterType.ItemCatergory)
                                        filter.Add(new ItemFilterEntryExt(item));
                                return;
                            default:
                                return;
                        }
                    }

                    XtraMessageBox.Show("Empty or file opening error.");
                }
            }
        }

        /// <summary>
        /// Сборос списка фильтра к исходному состоянию
        /// </summary>
        private void btnReload_Click(object sender, EventArgs e)
        {
            purchaseOptions.DataSource = null;
            fillListAction?.Invoke();
            purchaseOptions.DataSource = filter;
        }

        /// <summary>
        /// Вызов внешнего редактора для выбора Identifier для элемента фильтра
        /// </summary>
        private void btnEditItemFilterEntry_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (purchaseOptions.FocusedView is GridView focusedView)
            {
                if (focusedView.GetFocusedRowCellValue(colEntryType) is ItemFilterEntryType fType)
                {
                    if (fType == ItemFilterEntryType.Category)
                    {
                        //Выбор категории
                        ItemCategory cat = ItemCategory.None;
                        if (ItemSelectForm.GetAnItem(() => categories, ref cat))
                        {
                            focusedView.SetFocusedValue(cat.ToString());
                        }
                    }
                    else if(fType == ItemFilterEntryType.ItemType)
                    {
                        //Выбор типа предмета
                        ItemType type = ItemType.None;
                        if (ItemSelectForm.GetAnItem(() => itemTypes, ref type))
                        {
                            focusedView.SetFocusedValue(type.ToString());
                        }
                    }
                    else if(fType == ItemFilterEntryType.Identifier)
                    {
                        //Выбор предмета
                        GetAnItem.ListItem rewardItem;
                        var screenType = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType;
                        if (screenType == ScreenType.Store
                            || screenType == ScreenType.StoreCollection)
                            rewardItem = GetAnItem.Show(1);
                        else rewardItem = GetAnItem.Show();

                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                        {
                            focusedView.SetFocusedValue(rewardItem.ItemId);
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

        private void DeleteItemFilter(object sender, EventArgs e)
        {
            if (purchaseOptions.FocusedView is GridView grid)
            {
                grid.DeleteSelectedRows();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            filter.Add(new ItemFilterEntryExt());
        }
    }
}
