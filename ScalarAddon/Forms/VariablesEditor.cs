using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ValiablesAstralExtention.Classes;

namespace ValiablesAstralExtention.Forms
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
                varEditor.clmnType.Items.Clear();
                varEditor.clmnType.Items.AddRange(Enum.GetNames(typeof(VarTypes)));
            }

            if (vars != null)
            {
                //
                // Заполнение DataGridView значениями
                //
                foreach (Variable var in vars)
                {
                    DataGridViewRow dgvRow = new DataGridViewRow();
                    dgvRow.Cells[varEditor.clmnName.DisplayIndex].Value = var.Key;
                    dgvRow.Cells[varEditor.clmnType.DisplayIndex].Value = var.VarType;
                    dgvRow.Cells[varEditor.clmnValue.DisplayIndex].Value = var.Value;
                    dgvRow.Tag = var;
                }
                //
                // отображение редактора переменных
                //
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
