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
    public partial class MultiSelectForm : XtraForm
    {
        private static MultiSelectForm @this = null;

        internal int SelectColumnInd { get => clmnSelect.DisplayIndex; }
        //internal int ItemsNamesColumnInd { get => clmnItemsNames.DisplayIndex; }

        Action<DataGridView> Fill;

        public MultiSelectForm()
        {
            InitializeComponent();
        }

        internal static bool GUIRequest(string caption, Action<DataGridView> fill, Action<DataGridView> select)
        {
            bool result = false;
            if(fill != null && select != null)
            {
                if (@this == null)
                    @this = new MultiSelectForm();
                @this.Text = caption;
                @this.Fill = fill;

                if(@this.ShowDialog() == DialogResult.OK)
                {
                    select(@this.dgvItems);
                    result = true;
                }
            }

            return result;
        }
        
        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Fill(dgvItems);
        }

        private void MultiSelectForm_Shown(object sender, EventArgs e)
        {
            Fill(dgvItems);
        }
    }
}