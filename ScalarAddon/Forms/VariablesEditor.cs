using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstralVars.Classes;

namespace AstralVars.Forms
{
    public partial class VariablesEditor : Form
    {
        private static VariablesEditor varEditor;
        private VariableCollection varSource;

        public VariablesEditor()
        {
            InitializeComponent();
        }

        public static Variable GetVariable(VariableCollection vars)
        {
            if (varEditor == null)
            {
                varEditor = new VariablesEditor();

                //заполняем список доступных для выбора значений "типов" строками
                varEditor.clmnType.Items.Clear();
                varEditor.clmnType.Items.AddRange(Enum.GetNames(typeof(VarTypes)));

                // Следующая реализация вызывает ошибку при попытке выбрать двугое значение в списке
                //varEditor.clmnType.DataSource = VariablesParcer.varTypes;

                varEditor.dgvVariables.ReadOnly = true;

                varEditor.btnSelect.Text = "Select";
            }

            if (vars != null)
            {
                // Заполнение DataGridView значениями
                varEditor.FillDgvVariables(vars);

                // отображение редактора переменных
                DialogResult result = varEditor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (varEditor.dgvVariables.CurrentRow.Tag is Variable var)
                        MessageBox.Show($"Selected variable is {{{var}}}");
                    else MessageBox.Show($"Incorrect variable was selected");
                }
            }
            return null;
        }

        /// <summary>
        /// Заполение dgvVariables списком переменных из vars
        /// </summary>
        /// <param name="vars">Список отображаемых переменных</param>
        private void FillDgvVariables(VariableCollection vars = null)
        {
            if (vars != null)
                varSource = vars;

            varEditor.dgvVariables.Rows.Clear();
            foreach (Variable var in varSource)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(varEditor.dgvVariables);
                newRow.Cells[varEditor.clmnName.DisplayIndex].Value = var.Key;
                newRow.Cells[varEditor.clmnType.DisplayIndex].Value = var.VarType.ToString();
                switch (var.VarType)
                {
                    case VarTypes.Boolean:
                        //DataGridViewCheckBoxCell cbCell = new DataGridViewCheckBoxCell();
                        DataGridViewComboBoxCell cbCell = new DataGridViewComboBoxCell();
                        cbCell.Items.AddRange(new string[] { "True", "False"});
                        cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                        cbCell.Value = var.Value;
                        newRow.Cells[varEditor.clmnValue.DisplayIndex] = cbCell;
                        break;
                    case VarTypes.Integer:
                        newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                        //int.TryParse(newRow.Cells[varEditor.clmnValue.DisplayIndex].Value.ToString(), out int iRres);
                        break;
                    case VarTypes.DateTime:
                        DataGridViewDateTimeCell dtCell = new DataGridViewDateTimeCell();
                        dtCell.Value = var.Value;
                        newRow.Cells[varEditor.clmnValue.DisplayIndex] = dtCell;
                        //newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                        //DateTime.TryParse(newRow.Cells[varEditor.clmnValue.DisplayIndex].Value.ToString(), out DateTime dtRes);
                        break;
                    case VarTypes.Counter:
                        newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                        //VariablesParcer.GetItemID(newRow.Cells[varEditor.clmnValue.DisplayIndex].Value.ToString(), out string stRes);
                        break;
                }


                newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                newRow.Tag = var;
                varEditor.dgvVariables.Rows.Add(newRow);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!dgvVariables.Rows[e.RowIndex].IsNewRow)
            {
                if (e.ColumnIndex == clmnValue.DisplayIndex)
                {
                    // пустая строка в качестве значения переменной - допускается
                    string val = e.FormattedValue.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        if(Enum.TryParse(dgvVariables.Rows[e.RowIndex].Cells[clmnType.DisplayIndex].Value.ToString(), true, out VarTypes vType))
                        {
                            switch (vType)
                            {
                                case VarTypes.Boolean:
                                    e.Cancel = !bool.TryParse(val, out bool bRes);
                                    break;
                                case VarTypes.Integer:
                                    e.Cancel = !int.TryParse(val, out int iRres);
                                    break;
                                case VarTypes.DateTime:
                                    e.Cancel = !DateTime.TryParse(val, out DateTime dtRes);
                                    break;
                                case VarTypes.Counter:
                                    e.Cancel = !VariablesParcer.GetItemID(val, out string stRes);
                                    break;
                            }

                            if(e.Cancel)
                                MessageBox.Show($"Invalid value '{val}' for variables type of '{vType}'");
                        }
                    }
                }
                else if (e.ColumnIndex == clmnName.DisplayIndex)
                {
                    if(string.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        MessageBox.Show(varEditor, "Empty name for variable is not allowed", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }

                    // Добавить проверку уникальности имени переменной
                    foreach(DataGridViewRow row in dgvVariables.Rows)
                    {
                        if (!row.IsNewRow && row.Index != e.RowIndex && e.FormattedValue.Equals(row.Cells[e.ColumnIndex].Value))
                        {
                            MessageBox.Show(varEditor, "Name of the variable should be unique!\n" +
                                                       $"Variable '{e.FormattedValue}' already is present in the collection", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }
                else if(e.ColumnIndex == clmnType.DisplayIndex)
                {
                    if(Enum.TryParse(dgvVariables.Rows[e.RowIndex].Cells[clmnType.DisplayIndex].Value.ToString(), true, out VarTypes newType))
                    {
                        DataGridViewRow row = dgvVariables.Rows[e.RowIndex];
                        Variable oldVar = row.Tag as Variable;
                        if(oldVar != null && oldVar.VarType != newType)
                        {
                            DialogResult dResult = MessageBox.Show(varEditor, $"You change type of variable '{row.Cells[clmnName.DisplayIndex].Value}' from '{oldVar.VarType}' to '{newType}'!\n" +
                                                       $"The value of the variable would be erased!" +
                                                       $"Confirm the changes", "Errors", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (dResult == DialogResult.Yes)
                            {
                                switch (newType)
                                {
                                    case VarTypes.Boolean:
                                        bool.TryParse(row.Cells[clmnValue.DisplayIndex].Value.ToString(), out bool bRes);
                                        break;
                                    case VarTypes.Integer:
                                        int.TryParse(row.Cells[clmnValue.DisplayIndex].Value.ToString(), out int iRres);
                                        break;
                                    case VarTypes.DateTime:
                                        DateTime.TryParse(row.Cells[clmnValue.DisplayIndex].Value.ToString(), out DateTime dtRes);
                                        break;
                                    case VarTypes.Counter:
                                        VariablesParcer.GetItemID(row.Cells[clmnValue.DisplayIndex].Value.ToString(), out string stRes);
                                        break;
                                    case VarTypes.String:
                                        break;
                                }
                            }
                            else e.Cancel = true;
                        }
                    }
                    else e.Cancel = true;
                }
            }
        }

        private void dgvVariables_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }

        private void chbAllowEdit_CheckedChanged(object sender, EventArgs e)
        {
            dgvVariables.ReadOnly = !chbAllowEdit.Checked;
        }

        private void dgvVariables_ReadOnlyChanged(object sender, EventArgs e)
        {
            chbAllowEdit.Checked = !dgvVariables.ReadOnly;
        }

        private void VariablesEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
