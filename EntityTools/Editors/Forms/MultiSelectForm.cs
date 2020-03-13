using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EntityTools.Editors.Forms
{
    public partial class MultiSelectForm :XtraForm
    {
        internal int SelectColumnInd { get => clmnSelect.DisplayIndex; }
        //internal int ItemsNamesColumnInd { get => clmnItemsNames.DisplayIndex; }

        internal delegate void FillItems(DataGridView dgv);

        internal FillItems FillGrid;
        internal FillItems GetSelectedItems;

        public MultiSelectForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            GetSelectedItems(dgvItems);
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            FillGrid(dgvItems);
        }

        private void MultiSelectForm_Shown(object sender, EventArgs e)
        {
            FillGrid(dgvItems);
        }
    }
}