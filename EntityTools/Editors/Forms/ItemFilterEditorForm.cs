using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Quester.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using EntityTools.Enums;
using EntityTools.Tools.ItemFilter;
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
    public partial class ItemFilterEditorForm<TFilterEntry> : /* Form //*/XtraForm
    {
        static ItemFilterEditorForm<TFilterEntry> @this;

        /// <summary>
        /// Список категорий предметов
        /// </summary>
        static readonly IEnumerable<ItemCategory> categories = Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>();//.ToList();
        /// <summary>
        /// Список типов предметов
        /// </summary>
        static readonly IEnumerable<ItemType> itemTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>();//.ToList();
        /// <summary>
        /// Список флагов
        /// </summary>
        static readonly IEnumerable<ItemFlagsExt> itemFlags = Enum.GetValues(typeof(ItemFlagsExt)).Cast<ItemFlagsExt>();//new List<ItemFlags> { ItemFlags.Algo, ItemFlags.Bound, ItemFlags.BoundToAccount, ItemFlags.Full, ItemFlags.ProtectedItem, ItemFlags.SlottedOnAssignment, ItemFlags.TrainingFromItem };
        /// <summary>
        /// Списко качества предметов
        /// </summary>
        static readonly IEnumerable<ItemQuality> itemQualities = Enum.GetValues(typeof(ItemQuality)).Cast<ItemQuality>();


        /// <summary>
        /// Список-фильтров
        /// </summary>
        ItemFilterCoreExt<TFilterEntry> filterCore = new ItemFilterCoreExt<TFilterEntry>();

        /// <summary>
        /// Функтор, заполняющий фильтр исходными значениями
        /// </summary>
        Action fillListAction;

        private ItemFilterEditorForm()
        {
            InitializeComponent();
        }

        public static bool GUIRequiest(ref ItemFilterCoreExt<TFilterEntry> filters)
        {
            if (@this == null)
                @this = new ItemFilterEditorForm<TFilterEntry>();

            // Копирование filters в @this.filterCore не нужно, 
            // т.к. при отображении формы будет вызван event_FormShown, 
            // заполняющий фильтр вызовом @this.fillListAction
            if (@this.filterCore is null)
                @this.filterCore = new ItemFilterCoreExt<TFilterEntry>();
            else @this.filterCore.Filters.Clear();

            if (filters is null || filters.Filters == null || filters.Filters.Count == 0)
            {
                @this.fillListAction = null;
                @this.btnReload.Enabled = false;
            }
            else
            {
                // Локальная ссылка на редактируемый фильтр, необходимая в анонимной функции
                ItemFilterCoreExt<TFilterEntry> originalFilter = new ItemFilterCoreExt<TFilterEntry>(filters);

                @this.fillListAction = () =>
                  {
                      @this.filterCore.Filters.Clear();
                      foreach (var f in originalFilter)
                          @this.filterCore.Filters.Add((TFilterEntry)f.Clone());
                  };
                @this.btnReload.Enabled = true;
            }

            Type FilterEntryType = typeof(TFilterEntry);
            if (FilterEntryType == typeof(BuyFilterEntry))
            {
                @this.colGroup.Visible = true;
                @this.colGroup.ToolTip = "Index of the Group. If purchase failed then groups with the higher index will not processed";
                @this.colMode.Visible = false;
                @this.colCount.Visible = true;
                @this.colCheckPlayerLevel.Visible = true;
                @this.colCheckEquipmentLevel.Visible = true;
                @this.colKeepNumber.Visible = true;
                @this.colWear.Visible = true;
            }
            else if (FilterEntryType == typeof(CommonFilterEntry))
            {
                @this.colGroup.Visible = true;
                @this.colGroup.ToolTip = "Index of the Group. All filter in the Group checks at conjunction rule";
                @this.colMode.Visible = false;
                @this.colCount.Visible = false;
                @this.colCheckPlayerLevel.Visible = false;
                @this.colCheckEquipmentLevel.Visible = false;
                @this.colKeepNumber.Visible = false;
                @this.colWear.Visible = false;
            }

            @this.purchaseOptions.DataSource = @this.filterCore.Filters;

            if (@this.ShowDialog() == DialogResult.OK)
            {
                filters = new ItemFilterCoreExt<TFilterEntry>(@this.filterCore);
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
        private void event_TestFilters(object sender, EventArgs e)
        {
            IndexedBags<TFilterEntry> bag = new IndexedBags<TFilterEntry>(filterCore, BagsList.GetFullPlayerInventory());
            string bagDskr = bag.Description();
            if (string.IsNullOrEmpty(bagDskr))
                bagDskr = "No item matches";
            ETLogger.WriteLine(bagDskr, true);
            XtraMessageBox.Show(bagDskr); 
        }

        /// <summary>
        /// Очистка всего списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_ClearFilters(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Are you sure to clear the filter list ?", "Delete all entry?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                filterCore.Filters.Clear();
            }
        }

        /// <summary>
        /// Экспорт фильтра в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_ExportFilters(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.SettingsPath;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = typeof(TFilterEntry).Name + " filter profile (*.xml)|*.xml";
            saveFileDialog.FileName = typeof(TFilterEntry).Name + "_Filters.xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
#if false
                Astral.Functions.XmlSerializer.Serialize(saveFileDialog.FileName, filter.ToList()); 
#else
                XmlSerializer serialiser = new XmlSerializer(/*typeof(SettingsContainer)*/filterCore.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serialiser.Serialize(file, filterCore);
                }
#endif
            }
        }

        /// <summary>
        /// Импорт фильтра из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_ImportFilters(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directories.SettingsPath;
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = typeof(TFilterEntry).Name + " filter profile (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
#if false
                List<ItemFilterEntryExt> newFilter = Astral.Functions.XmlSerializer.Deserialize<List<ItemFilterEntryExt>>(openFileDialog.FileName, false);
#else
                ItemFilterCoreExt<TFilterEntry> newFilterCore = new ItemFilterCoreExt<TFilterEntry>();

                try
                {
                    using (StreamReader fileStream = new StreamReader(openFileDialog.FileName))
                    {
#if false
                        XmlSerializer serialiser = new XmlSerializer(typeof(ItemFilterCoreExt<TFilterEntry>));
                        if (serialiser.Deserialize(fileStream) is ItemFilterCoreExt<TFilterEntry> exportedFilters)
                        {
                            newFilterCore = exportedFilters;
                        }
#else
                        //TODO: Устранить ошибку в ItemFilterCoreExt.ReadInnerXml(...) не reader.Name вовзращает "" вместо имени элемента

                        XmlReaderSettings readerSettings = new XmlReaderSettings();
                        readerSettings.IgnoreWhitespace = true;

                        using (XmlReader reader = XmlReader.Create(fileStream, readerSettings))
                        {
                            newFilterCore.ReadXml(reader);
                        }
#endif
                    }
#endif
                }
                catch (Exception err)
                {
                    ETLogger.WriteLine(LogType.Error, err.Message, true);
                    newFilterCore = null;
                }

                if (newFilterCore != null && newFilterCore.IsValid)
                {
                    if (filterCore.Filters.Count > 0)
                    {
                        switch (XtraMessageBox.Show("Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilterCore)
                                    filterCore.Filters.Add(item);
                                return;
                            case DialogResult.No: // Замена существующего списка
                                filterCore.Filters.Clear();
                                foreach (var item in newFilterCore)
                                    filterCore.Filters.Add(item);
                                return;
                            default:
                                return;
                        }
                    }
                    else
                    {
                        foreach (var item in newFilterCore)
                            filterCore.Filters.Add(item);
                    }
                }
                else
                {
#if true
                    XtraMessageBox.Show("Empty or file opening error.");
#else
                    ItemFilterCore newFilterCore = Astral.Functions.XmlSerializer.Deserialize<ItemFilterCore>(openFileDialog.FileName);

                    if (newFilterCore != null && newFilterCore.Entries.Count > 0)
                    {
                        switch (XtraMessageBox.Show("There are ItemFilter at the Astral's format in the File.\n\r" +
                            // "You can import only ItemId and Category entry from it.\n\r" +
                            "Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilterCore.Entries)
                                    if (item.Type != ItemFilterType.Loot)
                                    {
                                        filterCore.Filters.Add(new TFilterEntry(item));
                                    }
                                return;
                            case DialogResult.No: // Замена существующего списка
                                filterCore.Filters.Clear();
                                foreach (var item in newFilterCore.Entries)
                                    if (item.Type != ItemFilterType.Loot)
                                    {
                                        filterCore.Filters.Add(new TFilterEntry(item));
                                    }
                                return;
                            default:
                                return;
                        }
                    }
                    else XtraMessageBox.Show("Empty or file opening error.");
#endif
                } 
            }
        }

        /// <summary>
        /// Сборос списка фильтра к исходному состоянию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_ReloadFilters(object sender, EventArgs e)
        {
            purchaseOptions.DataSource = null;
            fillListAction?.Invoke();
            purchaseOptions.DataSource = filterCore.Filters;
        }

        /// <summary>
        /// Вызов внешнего редактора для выбора Identifier для элемента фильтра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_EditItemFilterEntry(object sender, ButtonPressedEventArgs e)
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
                    else if(fType == ItemFilterEntryType.Type)
                    {
                        //Выбор типа предмета
                        ItemType type = ItemType.None;
                        if (EntityTools.Core.GUIRequest_Item(() => itemTypes, ref type))
                            gridView.SetFocusedValue(type.ToString());
                    }
                    else if(fType == ItemFilterEntryType.Identifier)
                    {
                        //Выбор предмета
                        GetAnItem.ListItem rewardItem = null;
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.Store
                            || EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.StoreCollection)
                            rewardItem = GetAnItem.Show(1);
                        else rewardItem = GetAnItem.Show(0);

                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.ItemId))
                            gridView.SetFocusedValue(rewardItem.ItemId);
                    }
                    else if(fType == ItemFilterEntryType.Flag)
                    {
                        //Выбор флага
                        ItemFlagsExt flag = ItemFlagsExt.Unidentified;
                        if (EntityTools.Core.GUIRequest_Item(() => itemFlags, ref flag))
                            gridView.SetFocusedValue(flag.ToString());
                    }
                    else if(fType == ItemFilterEntryType.Quality)
                    {
                        //Выбор качества
                        ItemQuality quality = ItemQuality.White;
                        if (EntityTools.Core.GUIRequest_Item(() => itemQualities, ref quality))
                            gridView.SetFocusedValue(quality.ToString());
                    }
                    else if(fType == ItemFilterEntryType.Name)
                    {
                        //Выбор предмета
                        GetAnItem.ListItem rewardItem = null;
                        if (EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.Store
                            || EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.ScreenType == ScreenType.StoreCollection)
                            rewardItem = GetAnItem.Show(1);
                        else rewardItem = GetAnItem.Show(0);

                        if (rewardItem != null && !string.IsNullOrEmpty(rewardItem.DisplayName))
                            gridView.SetFocusedValue(rewardItem.DisplayName);
                    }
                }
            }
        }

        private void event_SaveAndClose(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void event_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        private void event_FormShown(object sender, EventArgs e)
        {
            fillListAction?.Invoke();
        }

        private void event_AddFilterEntry(object sender, EventArgs e)
        {
            //TODO: Переключение фокуса на добавленную строку
#if AddNewItem_to_DataSource
            TFilterEntry f = filterCore.Filters.AddNew();
#else
            if (purchaseOptions.FocusedView is DevExpress.XtraGrid.Views.Grid.GridView gridView)
            {
                gridView.AddNewRow();
            } 
#endif
        }

        private void event_DeleteFilterEntry(object sender, EventArgs e)
        {
            if (purchaseOptions.FocusedView is DevExpress.XtraGrid.Views.Grid.GridView gridView)
            {
                var rowObj = gridView.GetFocusedRow();
                if(rowObj is TFilterEntry filterEntry)
                    filterCore.Filters.Remove(filterEntry);
            }
        }
        #endregion
    }
}
