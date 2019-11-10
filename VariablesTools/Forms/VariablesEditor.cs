using System;
using System.Linq;
using System.Windows.Forms;
using VariableTools.Expressions;
using DevExpress.XtraEditors;
using VarCollection = System.Collections.Generic.Dictionary<string, double>;

namespace VariableTools.Forms
{
    public partial class VariablesEditor : XtraForm //*/Form
    {
        private static VariablesEditor varEditor;

        public VariablesEditor()
        {
            InitializeComponent();
        }

        public static void Show(bool allowEdit = false)
        {
            if (varEditor == null)
            {
                varEditor = new VariablesEditor();

                varEditor.chbAllowEdit.Visible = allowEdit;

                varEditor.dgvVariables.ReadOnly = !allowEdit;

                if (allowEdit)
                {
                    varEditor.btnSelect.Visible = true;
                    varEditor.btnSelect.Text = "Save";
                    varEditor.btnCancel.Visible = true;
                    varEditor.btnCancel.Text = "Cancel";
                }
                else
                {
                    varEditor.btnSelect.Visible = false;
                    varEditor.btnSelect.Text = string.Empty;
                    varEditor.btnCancel.Visible = true;
                    varEditor.btnCancel.Text = "Close";
                }
            }

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables();

            // отображение редактора переменных
            DialogResult result = varEditor.ShowDialog();
            if (allowEdit && result == DialogResult.OK)
            {
                // Сохраняем значение переменных в коллекцию
                VariablesTools.Variables.Clear();
                foreach(DataGridViewRow row in varEditor.dgvVariables.Rows)
                {
                    if (Parser.TryParse(row.Cells[varEditor.clmnValue.DisplayIndex].Value, out double newValue))
                        newValue = 0;

                    // Реализация через Dictionary<string, double>
                    //if(VariablesTools.Variables.ContainsKey(row.Cells[varEditor.clmnName.DisplayIndex].Value.ToString()))
                    //    VariablesTools.Variables[row.Cells[varEditor.clmnName.DisplayIndex].Value.ToString()] = newValue;
                    //else VariablesTools.Variables.Add(row.Cells[varEditor.clmnName.DisplayIndex].Value.ToString(), newValue);

                    // Реализация через VariableCollection
                    if (!VariablesTools.Variables.TryAdd(row.Cells[varEditor.clmnName.DisplayIndex].Value.ToString(), newValue))
                        VariablesTools.Variables.Add(row.Cells[varEditor.clmnName.DisplayIndex].Value.ToString(), newValue);
                }
            }
        }

        public static string GetVariable(string var_name = "")
        {
            if (varEditor == null)
            {
                varEditor = new VariablesEditor();

            }
            varEditor.chbAllowEdit.Visible = false;
            varEditor.dgvVariables.ReadOnly = true;

            varEditor.btnSelect.Visible = true;
            varEditor.btnSelect.Text = "Select";
            varEditor.btnCancel.Visible = true;
            varEditor.btnCancel.Text = "Cancel";

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables(var_name);

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
            foreach (var v in VariablesTools.Variables.ToList())
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(varEditor.dgvVariables);
                //newRow.Cells[varEditor.clmnName.DisplayIndex].Value = v.Key;
                newRow.Cells[varEditor.clmnName.DisplayIndex].Value = v.Name;

                newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = v.Value;
                int ind = varEditor.dgvVariables.Rows.Add(newRow);

                //if (v.Key == cur_var) 
                if (v.Name == cur_var)
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
            //        VariablesTools.Variables[var_name] = var_val;
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
            //    //if ((var_name = VariablesTools.Variables.Set(,
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
