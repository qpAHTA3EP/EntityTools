using System;
using System.Linq;
using System.Windows.Forms;
using AstralVariables.Expressions;
using VarCollection = System.Collections.Generic.Dictionary<string, double>;

namespace AstralVariables.Forms
{
    public partial class VariablesEditor : Form
    {
        private static VariablesEditor varEditor;

        public VariablesEditor()
        {
            InitializeComponent();
        }

        //public static string GetVariable(VarCollection vars)
        //{
        //    if (varEditor == null)
        //    {
        //        varEditor = new VariablesEditor();

        //        varEditor.chbAllowEdit.Visible = false;

        //        varEditor.dgvVariables.ReadOnly = true;

        //        varEditor.btnSelect.Text = "Select";
        //    }

        //    if (vars != null)
        //    {
        //        // Заполнение DataGridView значениями
        //        varEditor.FillDgvVariables(vars);

        //        // отображение редактора переменных
        //        DialogResult result = varEditor.ShowDialog();
        //        if (result == DialogResult.OK)
        //            return varEditor.dgvVariables.CurrentRow.Cells[varEditor.clmnName.DisplayIndex].Value.ToString();
        //    }
        //    return string.Empty;
        //}
        public static string GetVariable(string var)
        {
            if (varEditor == null)
            {
                varEditor = new VariablesEditor();

                varEditor.chbAllowEdit.Visible = false;

                varEditor.dgvVariables.ReadOnly = true;

                varEditor.btnSelect.Text = "Select";
            }

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables(var);

            // отображение редактора переменных
            DialogResult result = varEditor.ShowDialog();
            if (result == DialogResult.OK)
                return varEditor.dgvVariables.CurrentRow.Cells[varEditor.clmnName.DisplayIndex].Value.ToString();

            return string.Empty;
        }
        /// <summary>
        /// Заполение dgvVariables списком переменных из vars
        /// </summary>
        /// <param name="vars">Список отображаемых переменных</param>
        private void FillDgvVariables(string cur_var = "")
        {
            varEditor.dgvVariables.Rows.Clear();

            int curVarInd = -1;
            foreach (var v in VariablesAddon.Variables.ToList())
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(varEditor.dgvVariables);
                newRow.Cells[varEditor.clmnName.DisplayIndex].Value = v.Key;

                newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = v.Value;
                int ind = varEditor.dgvVariables.Rows.Add(newRow);

                if (v.Key == cur_var) 
                    curVarInd = ind; // сохраняем индекс строки выбранной переменной
            }

            if(curVarInd >= 0)
                dgvVariables.Rows[curVarInd].Cells[0].Selected = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //foreach (DataGridViewRow row in dgvVariables.Rows)
            //{
            //    if (row.IsNewRow)
            //        continue;

            //    string var_name = row.Cells[clmnName.DisplayIndex].Value.ToString();

            //    if (Parser.TryParse(row.Cells[clmnValue.DisplayIndex].Value, out double var_val)
            //        && !Parser.IsForbidden(var_name))
            //    {
            //        VariablesAddon.Variables[var_name] = var_val;
            //    }
            //    else
            //    {
            //        DialogResult dResult = MessageBox.Show(varEditor, $"Variable [{row.Index + 1}]'{var_name}' is incorect.\n" +
            //                                                           "It will be deleted from the collection." +
            //                                                           "Do you want to correct it ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (dResult == DialogResult.Yes)
            //        {
            //            DialogResult = DialogResult.Cancel;
            //            dgvVariables.Rows[row.Index].Selected = true;
            //        }
            //        else DialogResult = DialogResult.OK;
            //    }
            //    //if ((var_name = VariablesAddon.Variables.Set(,
            //    //                                        row.Cells[clmnValue.DisplayIndex].Value)) == null)
            //    //{
            //    //    row.Tag = null;
            //    //    DialogResult dResult = MessageBox.Show(varEditor, $"Variable [{row.Index+1}]'{row.Cells[clmnName.DisplayIndex].Value}' is incorect.\n" +
            //    //                                                       "It will be deleted from the collection." +
            //    //                                                       "Do you want to correct it ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    //    if (dResult == DialogResult.Yes)
            //    //    {
            //    //        DialogResult = DialogResult.Cancel;
            //    //        dgvVariables.Rows[row.Index].Selected = true;
            //    //    }
            //    //    else DialogResult = DialogResult.OK;
            //    //}
            //    //else
            //    //{
            //    //    row.Tag = var_name;
            //    //    DialogResult = DialogResult.OK;
            //    //}
            //}
            DialogResult = DialogResult.OK;
            Close();
        }

        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewRow curRow = dgvVariables.Rows[e.RowIndex];
            if (curRow != null && !curRow.IsNewRow)
            {
                // Проверка валидности значения ячейки в столбце значений 'clmnValue'
                /*if (e.ColumnIndex == clmnValue.DisplayIndex)
                {
                    // пустая строка в качестве значения переменной - допускается
                    if (!Parser.TryParse(curRow.Cells[e.ColumnIndex].Value, out double val))
                        e.Cancel = true;
                }
                // Проверка валидности значения ячейки в столбце имени переменной 'clmnName'
                else */if (e.ColumnIndex == clmnName.DisplayIndex)
                {
                    if(Parser.IsForbidden(curRow.Cells[e.ColumnIndex].Value.ToString()))
                    {
                        MessageBox.Show(varEditor, "Name of the {Variable} is forbidden!", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }

                    // Добавить проверку уникальности имени переменной
                    foreach(DataGridViewRow r in dgvVariables.Rows)
                    {

                        if (!r.IsNewRow && r.Index != e.RowIndex && e.FormattedValue.Equals(r.Cells[e.ColumnIndex].Value))
                        {
                            MessageBox.Show(varEditor, "Name of the {Variable} should be unique!\n" +
                                                       $"Variable '{e.FormattedValue}' already is present in the collection", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }

        private void chbAllowEdit_CheckedChanged(object sender, EventArgs e)
        {
            dgvVariables.ReadOnly = !chbAllowEdit.Checked;
            if (dgvVariables.ReadOnly)
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVariables.AllowUserToAddRows = false;
            }
            else
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvVariables.AllowUserToAddRows = true;
            }
        }

        private void dgvVariables_ReadOnlyChanged(object sender, EventArgs e)
        {
            chbAllowEdit.Checked = !dgvVariables.ReadOnly;
            if (dgvVariables.ReadOnly)
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVariables.AllowUserToAddRows = false;
            }
            else
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvVariables.AllowUserToAddRows = true;
            }
        }

        private void dgvVariables_RowCanSelect(object sender, DataGridViewCellEventArgs e)
        {
            bool valid = false;
            DataGridViewRow row = dgvVariables.Rows[e.RowIndex];

            valid = !(row.IsNewRow
                 && Parser.IsForbidden(row.Cells[clmnName.DisplayIndex].Value.ToString()));
            btnSelect.Enabled = valid;
        }


    }
}
