using EntityPlugin.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityPlugin.Editors
{
    class ParagonListEditor : UITypeEditor
    {
        internal static MultiSelectForm listEditor = null;

        internal List<string> regions = null;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //if (listEditor == null)
            //{
            //    listEditor = new MultiSelectForm
            //    {
            //        Text = "Select CustomRegions",
            //        FillGrid = FillRegion2Grid,
            //        GetSelectedItems = GetSelectedRegions
            //    };
            //}

            //regions = value as List<string>;

            //// Отображение списка CustomRegion пользователю
            //DialogResult dialogResult = listEditor.ShowDialog();

            //if (dialogResult == DialogResult.OK)
            //{
            //    //формирование нового списка выбранных регионов производится делегатом GetSelectedRegions
            //    return regions;
            //}
            //return value;
            return null;
        }

        /// <summary>
        /// Делегат, заполняющий DataGridView списком итемов
        /// </summary>
        /// <param name="dgv"></param>
        internal void FillParagon2Grid(DataGridView dgv)
        {
            //int indSelect = dgv.Columns.Contains("clmnSelect") ? dgv.Columns["clmnSelect"].DisplayIndex : -1;
            //int indItems = dgv.Columns.Contains("clmnItems") ? dgv.Columns["clmnItems"].DisplayIndex : -1;
            //if (indSelect == -1 || indItems == -1)
            //    return;

            //dgv.Rows.Clear();
            //foreach (CustomRegion cr in Astral.Quester.API.CurrentProfile.CustomRegions)
            //{
            //    DataGridViewRow row = new DataGridViewRow();
            //    row.CreateCells(listEditor.dgvItems);
            //    row.Cells[indItems].Value = cr.Name;
            //    row.Cells[indSelect].Value = (regions != null && regions.Contains(cr.Name));
            //    listEditor.dgvItems.Rows.Add(row);
            //}
        }

        /// <summary>
        /// Делегат, формирующий список выбранных итемов из DataGridView
        /// </summary>
        /// <param name="dgv"></param>
        internal void GetSelectedParagons(DataGridView dgv)
        {
            //int indSelect = dgv.Columns.Contains("clmnSelect") ? dgv.Columns["clmnSelect"].DisplayIndex : -1;
            //int indItems = dgv.Columns.Contains("clmnItems") ? dgv.Columns["clmnItems"].DisplayIndex : -1;
            //if (indSelect == -1 || indItems == -1)
            //    return;

            //regions.Clear();
            //foreach (DataGridViewRow row in listEditor.dgvItems.Rows)
            //{
            //    if (row.Cells[indSelect].Value.Equals(true))
            //    {
            //        regions.Add(row.Cells[indItems].Value.ToString());
            //    }
            //}
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}