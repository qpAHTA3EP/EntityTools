using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntityTools.Forms
{
    public partial class ItemListEditorForm<T> : XtraForm //Form
    {
        static ItemListEditorForm<T> @this;
        Func<T> GetNewItem = null;

        private ItemListEditorForm()
        {
            InitializeComponent();
        }

        public static bool GUIRequest(ref IList<T> items, Func<T> getNewItem, string caption = "")
        {
            if (@this is null || @this.IsDisposed)
                @this = new ItemListEditorForm<T>();
            if (items is null)
                items = new List<T>();

            @this.Text = caption;
            @this.itemList.Items.Clear();
            if (items.Count > 0)
            {
                foreach (var item in items)
                    @this.itemList.Items.Add(item);
            }
            @this.GetNewItem = getNewItem;
            @this.btnAdd.Enabled = getNewItem !=null;

            @this.ShowDialog();

            if (@this.itemList.ItemCount > 0)
            {
                items.Clear();
                foreach(T item in @this.itemList.Items)
                    items.Add(item);
                return true;
            }
            return false;
        }

        private void handler_Close(object sender, EventArgs e)
        {
            Close();
        }

        private void handler_AddItem(object sender, EventArgs e)
        {
            if (GetNewItem != null)
            {
                T newItem = GetNewItem();
                if (newItem != null)
                {
                    int ind = itemList.Items.Add(newItem);
                    itemList.SelectedIndex = ind;
                } 
            }
        }

        private void handler_RemoveItem(object sender, EventArgs e)
        {
            int selectedIndex = itemList.SelectedIndex;
            if (selectedIndex > -1 && XtraMessageBox.Show(LookAndFeel, this, "Are you sure to delete selected item ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                itemList.Items.RemoveAt(selectedIndex);
        }

        private void handler_UpItem(object sender, EventArgs e)
        {
            int selectedIndex = itemList.SelectedIndex;
            if (selectedIndex > 0)
            {
                var item = itemList.SelectedValue;
                itemList.Items.RemoveAt(selectedIndex);
                selectedIndex--;
                itemList.Items.Insert(selectedIndex, item);
                itemList.SelectedIndex = selectedIndex;
            }
        }

        private void handler_DownItem(object sender, EventArgs e)
        {
            int selectedIndex = itemList.SelectedIndex;
            if (selectedIndex < itemList.ItemCount - 1)
            {
                var item = itemList.SelectedValue;
                itemList.Items.RemoveAt(selectedIndex);
                selectedIndex++;
                itemList.Items.Insert(selectedIndex, item);
                itemList.SelectedIndex = selectedIndex;
            }
        }
    }
}
