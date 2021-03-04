using Astral.Controllers;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Tools.CustomRegions;
using EntityTools.Tools.Extensions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EntityTools.Forms
{
    public partial class CustomRegionCollectionEditorForm : /* Form //*/XtraForm
    {
        static CustomRegionCollectionEditorForm @this;

        /// <summary>
        /// Список регионов
        /// </summary>
        BindingList<CustomRegionEntry> customRegions = new BindingList<CustomRegionEntry>();

        /// <summary>
        /// Функтор, заполняющий фильтр исходными значениями
        /// </summary>
        Action fillGrid;

        private CustomRegionCollectionEditorForm()
        {
            InitializeComponent();
        }

        public static bool GUIRequiest(ref CustomRegionCollection crCollection)
        {
            if (@this == null)
                @this = new CustomRegionCollectionEditorForm();

            // Копирование исходноq коллекции для возможности отказа от внесения изменений
            var originalCrCollection = crCollection;
            if (crCollection?.Count > 0)
                @this.fillGrid = () =>
                {
                    @this.customRegions.Clear();
                    foreach (var cr in Astral.Quester.API.CurrentProfile.CustomRegions)
                    {
                        if (originalCrCollection.TryGetValue(cr.Name, out CustomRegionEntry crEntry))
                            @this.customRegions.Add(crEntry.Clone());
                        else @this.customRegions.Add(new CustomRegionEntry(cr.Name, InclusionType.Ignore));
                    }
                };
            else @this.fillGrid = () =>
            {
                @this.customRegions.Clear();
                foreach (var cr in Astral.Quester.API.CurrentProfile.CustomRegions)
                    @this.customRegions.Add(new CustomRegionEntry(cr.Name, InclusionType.Ignore));
            };

            @this.gridCustomRegions.DataSource = @this.customRegions;

            if (@this.ShowDialog() == DialogResult.OK)
            {
                crCollection = new CustomRegionCollection(@this.customRegions.Where(crEntry => crEntry.Inclusion != InclusionType.Ignore));
                return crCollection.Count > 0;
            }
            return false;
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
#if disabled_20200513_1311
            var slots = EntityManager.LocalPlayer.Player.InteractInfo.ContactDialog.StoreItems;
            if (slots.Count > 0)
                if (XtraMessageBox.Show("Would you like to search the matches in the Store (Yes) or in the player bags (No)?\n\r" +
                                       "Искать соответствующие предметы у торговца (Yes) или в сумке персонажа (No)?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    slots =  
#endif
            IndexedBags bag = new IndexedBags(customRegions, BagsList.GetFullPlayerInventory());
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

#endif
        }

        /// <summary>
        /// Очистка всего списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_Reset(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Are you sure to reset CustomRegion selection?", "Reset selection?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                customRegions.ForEach(crEntry => crEntry.Inclusion = InclusionType.Ignore);
            }
        }

        /// <summary>
        /// Экспорт коллекции в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.SettingsPath;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = nameof(CustomRegionCollection) + " (*.xml)|*.xml";
            saveFileDialog.FileName = nameof(CustomRegionCollection) + ".xml";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
#if false
                Astral.Functions.XmlSerializer.Serialize(saveFileDialog.FileName, filter.ToList()); 
#else
                XmlSerializer serialiser = new XmlSerializer(customRegions.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serialiser.Serialize(file, customRegions);
                }
#endif
            }
        }

        /// <summary>
        /// Импорт коллекции из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
#if false
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
                    if (serialiser.Deserialize(fileStream) is List<ItemFilterEntryExt> list)
                    {
                        newFilter = list;
                    }
                }
#endif
                if (newFilter != null && newFilter.Count > 0)
                {
                    if (customRegions.Count > 0)
                    {
                        switch (XtraMessageBox.Show("Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilter)
                                    customRegions.Add(item);
                                return;
                            case DialogResult.No: // Замена существующего списка
                                customRegions.Clear();
                                foreach (var item in newFilter)
                                    customRegions.Add(item);
                                return;
                            default:
                                return;
                        }
                    }

                    foreach (var item in newFilter)
                        customRegions.Add(item);
                }
                else
                {
                    ItemFilterCore newFilterCore = Astral.Functions.XmlSerializer.Deserialize<ItemFilterCore>(openFileDialog.FileName);

                    if (newFilterCore != null && newFilterCore.Entries.Count > 0)
                    {
                        switch (XtraMessageBox.Show("There are ItemFilter at the Astral's format in the File.\n\r" +
                            "You can import only ItemId and Category entry from it.\n\r" +
                            "Add an entries to the filter (Yes) or replace current filter (No) ?", "Import filters", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in newFilterCore.Entries)
                                    if (item.Type == ItemFilterType.ItemID || item.Type == ItemFilterType.ItemCatergory)
                                        customRegions.Add(new ItemFilterEntryExt(item));
                                return;
                            case DialogResult.No: // Замена существующего списка
                                customRegions.Clear();
                                foreach (var item in newFilterCore.Entries)
                                    if (item.Type == ItemFilterType.ItemID || item.Type == ItemFilterType.ItemCatergory)
                                        customRegions.Add(new ItemFilterEntryExt(item));
                                return;
                            default:
                                return;
                        }
                    }

                    XtraMessageBox.Show("Empty or file opening error.");
                }
            } 
#endif
        }

        /// <summary>
        /// Сборос списка фильтра к исходному состоянию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReload_Click(object sender, EventArgs e)
        {
            gridCustomRegions.DataSource = null;
            fillGrid?.Invoke();
            gridCustomRegions.DataSource = customRegions;
        }

        private void handler_Select(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void handler_Cancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        private void handler_FormShown(object sender, EventArgs e)
        {
            fillGrid?.Invoke();
        }
    }
}
