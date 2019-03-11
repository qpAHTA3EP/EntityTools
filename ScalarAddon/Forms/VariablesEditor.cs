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
                varEditor.clmnType.DataSource = VarTypes;
                varEditor.clmnType.Items.Clear();
                varEditor.clmnType.Items.AddRange(Enum.GetNames(typeof(VarTypes)));
            }

            if (vars != null)
            {
                // Заполнение DataGridView значениями
                foreach (Variable var in vars)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(varEditor.dgvVariables);
                    newRow.Cells[varEditor.clmnName.DisplayIndex].Value = var.Key;
                    newRow.Cells[varEditor.clmnType.DisplayIndex].Value = var.VarType.ToString();
                    newRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                    newRow.Tag = var;
                    varEditor.dgvVariables.Rows.Add(newRow);                    
                }
                
                // отображение редактора переменных
                DialogResult result = varEditor.ShowDialog();
                if(result == DialogResult.OK)
                {
                    if(varEditor.dgvVariables.CurrentRow.Tag is Variable var)
                        MessageBox.Show($"Selected variable is {{{var}}}");
                    else MessageBox.Show($"Incorrect variable was selected");
                }
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
