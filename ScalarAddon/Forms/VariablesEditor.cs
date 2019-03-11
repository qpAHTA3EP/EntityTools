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
            //
            // clmnType
            //
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
                    if(e.Cancel = string.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        MessageBox.Show(varEditor, "Empty name for variable is not allowed", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Добавить проверку уникальности имени переменной
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
    }
}
