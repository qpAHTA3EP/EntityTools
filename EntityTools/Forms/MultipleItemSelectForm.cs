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

namespace EntityTools.Forms
{
    public partial class MultipleItemSelectForm : XtraForm
    {
        private static MultipleItemSelectForm @this = null;

        Action fillAction;

        public MultipleItemSelectForm()
        {
            InitializeComponent();
        }

        internal static bool GUIRequest<T>(Func<IEnumerable<T>> source, ref List<T> selectedValues, string caption = "")
        {
            bool result = false;

            if (source != null)
            {
                if (@this == null)
                    @this = new MultipleItemSelectForm();
                @this.Text = caption;
                @this.ItemList.DataSource = null;
                @this.fillAction = null;

                if (selectedValues != null && selectedValues.Count > 0)
                {
                    List<T> values = selectedValues;
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
                        
#endif
                        @this.ItemList.Items.Clear();
                        foreach(var item in source())
                        {
                            bool @checked = values.Contains(item);
                            @this.ItemList.Items.Add(item, @checked);
                        }
                    };
                }
                else @this.fillAction = () => @this.ItemList.DataSource = source();

                if (@this.ShowDialog() == DialogResult.OK)
                {
                    var checkList = @this.ItemList.CheckedItems;
                    if (checkList.Count > 0)
                    {
                        if (selectedValues is null)
                            selectedValues = new List<T>(checkList.Count);
                        else selectedValues.Clear();

                        foreach (var item in checkList)
                            selectedValues.Add((T)item);
                    }
                    else selectedValues?.Clear();
                    result = true;
                }
                @this.ItemList.DataSource = null;
                @this.fillAction = null;
            }

            return result;
        }
        internal static bool GUIRequest<T>(Func<IEnumerable<T>> source, ref IList<T> selectedValues, string caption = "")
        {
            bool result = false;

            if (source != null)
            {
                if (@this == null)
                    @this = new MultipleItemSelectForm();
                @this.Text = caption;
                @this.ItemList.DataSource = null;
                @this.fillAction = null;

                if (selectedValues != null && selectedValues.Count > 0)
                {
                    var values = selectedValues;
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
                        
#endif
                        @this.ItemList.Items.Clear();
                        foreach (var item in source())
                        {
                            bool cheched = values.Contains(item);
                            @this.ItemList.Items.Add(item, cheched);
                        }
                    };
                }
                else @this.fillAction = () => @this.ItemList.DataSource = source();

                if (@this.ShowDialog() == DialogResult.OK)
                {
                    var checkList = @this.ItemList.CheckedItems;
                    if (checkList.Count > 0)
                    {
                        if (selectedValues is null)
                            selectedValues = new List<T>(checkList.Count);
                        else selectedValues.Clear();

                        foreach (var item in checkList)
                            selectedValues.Add((T)item);
                    }
                    else selectedValues?.Clear();
                    result = true;
                }
                @this.ItemList.DataSource = null;
                @this.fillAction = null;
            }

            return result;
        }


        private void handler_Select(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void handler_Reload(object sender, EventArgs e)
        {
            fillAction?.Invoke();
        }

        private void handler_UncheckAll(object sender, EventArgs e)
        {
            ItemList.UnCheckAll();
        }

        private void handler_CheckAll(object sender, EventArgs e)
        {
            ItemList.CheckAll();
        }
    }
}