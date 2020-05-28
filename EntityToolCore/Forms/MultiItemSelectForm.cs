//#define DGV
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EntityCore.Forms
{
    public partial class MultiItemSelectForm : XtraForm
    {
        private static MultiItemSelectForm @this = null;

#if DGV
        internal int SelectColumnInd { get => clmnSelect.DisplayIndex; }
        //internal int ItemsNamesColumnInd { get => clmnItemsNames.DisplayIndex; }

        Action<DataGridView> Fill;
#endif
        Action fillAction;

        public MultiItemSelectForm()
        {
            InitializeComponent();
        }

#if DGV
        internal static bool GUIRequest(string caption, Action<DataGridView> fill, Action<DataGridView> select)
        {
            bool result = false;
            if (fill != null && select != null)
            {
                if (@this == null)
                    @this = new MultiSelectForm();
                @this.Text = caption;
                @this.Fill = fill;

                if (@this.ShowDialog() == DialogResult.OK)
                {
                    select(@this.dgvItems);
                    result = true;
                }
            }

            return result;
        }

#else
        internal static bool GUIRequest<T>(string caption, Func<IEnumerable<T>> source, ref List<T> selecteValues)
        {
            bool result = false;
            if (source != null)
            {
                if (@this == null)
                    @this = new MultiItemSelectForm();
                @this.Text = caption;
                if (selecteValues != null && selecteValues.Count > 0)
                {
                    List<T> values = selecteValues;
                    @this.fillAction = () => {
#if disabled_20200527_1229
                        // Когда selecteValues не пустой, возникает ошибка

                        @this.Items.DataSource = source(); 
                        for (int i = 0; i < @this.ItemList.ItemCount; i++)
                            {
                                if (values.Contains((T)@this.ItemList.Items[i].Value))
                                    @this.ItemList.Items[i].CheckState = CheckState.Checked;
                                else @this.ItemList.Items[i].CheckState = CheckState.Unchecked;
                            }
                        
#else
                        @this.ItemList.Items.Clear();
                        foreach(var item in source())
                        {
                            bool cheched = values.Contains(item);
                            @this.ItemList.Items.Add(item, cheched);
                        }
#endif
                    };
                }
                else
                {
                    @this.fillAction = () => @this.ItemList.DataSource = source();
                }

                if (@this.ShowDialog() == DialogResult.OK
                    && @this.ItemList.SelectedItem is T)
                {
                    var checkList = @this.ItemList.CheckedItems;
                    if (checkList.Count > 0)
                    {
                        if (selecteValues is null)
                            selecteValues = new List<T>();
                        else selecteValues.Clear();

                        foreach (var item in checkList)
                            selecteValues.Add((T)item);
                        result = true;
                    }
                }
            }
            return result;
        }
#endif

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
#if DGV
            Fill?.Invoke(dgvItems);
#else
            fillAction?.Invoke();
#endif
        }

        private void MultiSelectForm_Shown(object sender, EventArgs e)
        {
#if DGV
            Fill?.Invoke(dgvItems);
#else
            fillAction?.Invoke();
#endif
        }
    }
}