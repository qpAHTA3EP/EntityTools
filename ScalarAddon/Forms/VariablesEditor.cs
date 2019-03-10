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
        }

        public static bool Show(VariableCollection vars)
        {
            if (varEditor == null)
                varEditor = new VariablesEditor();

            if (vars != null)
            {
                varEditor.dgvVariables.AutoGenerateColumns = true;
                varEditor.dgvVariables.DataSource = vars.ToList();

                varEditor.clmnName.DataPropertyName = "Key";
                varEditor.clmnType.DataPropertyName = "Type";
                //varEditor.clmnType.Items.Clear();
                //varEditor.clmnType.Items.Add(VariableTypes.Boolean.ToString());
                //varEditor.clmnType.Items.Add(VariableTypes.Integer.ToString());
                //varEditor.clmnType.Items.Add(VariableTypes.DateTime.ToString());
                //varEditor.clmnType.Items.Add(VariableTypes.String.ToString());
                //varEditor.clmnType.Items.Add(VariableTypes.ItemsCount.ToString());

                varEditor.clmnValue.DataPropertyName = "Value";

                DialogResult result = varEditor.ShowDialog();
                if(result == DialogResult.OK)
                {
                    VariableItem var = varEditor.dgvVariables.CurrentRow.DataBoundItem as VariableItem;
                    MessageBox.Show($"Selected variable is {{{var}}}");
                }
            }
            return false;
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
