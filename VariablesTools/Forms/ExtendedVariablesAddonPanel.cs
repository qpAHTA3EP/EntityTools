using System;
using System.Windows.Forms;
using VariableTools.Classes;

namespace VariableTools.Forms
{
    public partial class ExtendedVariablesAddonPanel :  UserControl // */ Astral.Forms.BasePanel
    {
        public ExtendedVariablesAddonPanel() :base ("Variables Tools")
        {
            InitializeComponent();
        }

        private void ExtendedVariablesAddonPanel_Load(object sender, EventArgs e)
        {
            FillDgvVariables();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            VariablesTools.SaveToFile();
        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }

        private void FillDgvVariables()
        {
            dgvVariables.Rows.Clear();
            
            //using (var v = VariablesTools.Variables.GetEnumerator())
            //    while (v.MoveNext())
            //    {
            //        DataGridViewRow newRow = new DataGridViewRow();
            //        newRow.CreateCells(dgvVariables);
            //        newRow.Cells[clmnName.DisplayIndex].Value = v.Current.Name;
            //        newRow.Cells[clmnSave.DisplayIndex].Value = v.Current.Save;
            //        newRow.Cells[clmnScope.DisplayIndex].Value = v.Current.Scope;
            //        newRow.Cells[clmnValue.DisplayIndex].Value = v.Current.Value;
            //        newRow.Tag = v.Current;
                    
            //        int ind = dgvVariables.Rows.Add(newRow);
            //    }

            dgvVariables.AutoGenerateColumns = false;

            dgvVariables.DataSource = VariablesTools.Variables;
            clmnName.DataPropertyName = nameof(VariableContainer.Name);
            clmnSave.DataPropertyName = nameof(VariableContainer.Save);
            clmnSave.DataPropertyName = nameof(VariableContainer.Scope);
            clmnValue.DataPropertyName = nameof(VariableContainer.Value);
        }
    }
}
