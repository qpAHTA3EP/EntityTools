using Astral;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Windows.Forms;
using VariableTools.Classes;
using VariableTools.Expressions;

namespace VariableTools.Forms
{
    public partial class ExtendedVariablesToolsPanel : /* UserControl // */ Astral.Forms.BasePanel
    {
        public int NameInd { get => clmnName.DisplayIndex; }
        public int ValueInd { get => clmnValue.DisplayIndex; }
        public int SaveInd { get => clmnSave.DisplayIndex; }
        public int ProfScopeInd { get => clmnProfileScope.DisplayIndex; }
        public int AccScopeInd { get => clmnAccScope.DisplayIndex; }
        public int QualifierInd { get => clmnQualifier.DisplayIndex; }

        public bool dgvVariablesChanged = false;

        public ExtendedVariablesToolsPanel() :base ("Variables Tools")
        {
            InitializeComponent();
            clmnAccScope.Items.Add(AccountScopeType.Character.ToString());
            clmnAccScope.Items.Add(AccountScopeType.Account.ToString());
            clmnAccScope.Items.Add(AccountScopeType.Global.ToString());
        }

        private void ExtendedVariablesAddonPanel_Load(object sender, EventArgs e)
        {
            FillDgvVariables();
            dgvVariablesChanged = false;
        }

        private void ExtendedVariablesAddonPanel_Leave(object sender, EventArgs e)
        {
            dgvVariablesToCollection();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvVariablesToCollection();
            VariableTools.SaveToFile();
        }

        private void dntLoad_Click(object sender, EventArgs e)
        {
            if (!dgvVariablesChanged ||
                XtraMessageBox.Show("Do you really want to load variables from file ?\n" +
                                    "All changes will be lost.",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                VariableTools.LoadFromFile();
                FillDgvVariables();
                dgvVariablesChanged = false;
            }
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.CreateCells(dgvVariables);
            newRow.Cells[NameInd].Value = string.Empty;
            newRow.Cells[ValueInd].Value = 0;
            newRow.Cells[AccScopeInd].Value = AccountScopeType.Global.ToString();
            newRow.Cells[ProfScopeInd].Value = false;
            newRow.Cells[QualifierInd].Value = string.Empty;
            dgvVariables.Rows.Add(newRow);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVariables.CurrentRow != null
                && XtraMessageBox.Show($"Do you really want to delete variable {{{dgvVariables.CurrentRow.Cells[NameInd].Value.ToString()}}} ?",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                dgvVariables.Rows.Remove(dgvVariables.CurrentRow);
            }
        }

        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == NameInd)
            {
                // Чтение и корректировка имени переменной
                string name = e.FormattedValue?.ToString();
                
                // проверка на пустую строку
                if(!string.IsNullOrEmpty(name))
                {
                    // проверка корректности имени переменной
                    if (Parser.CorrectForbiddenName(name, out string correctName))
                        // согласие на замену некорректного имени переменной
                        if (XtraMessageBox.Show($"The name '{name}' is incorrect! \n" +
                                               $"Whould you like to change it to '{correctName}'?",
                            "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value = correctName;
                            e.Cancel = false;
                        }
                        else e.Cancel = true;
                    else e.Cancel = false;
                }
                else e.Cancel = true;
            }
            else if (e.ColumnIndex == AccScopeInd)
            {
                // области видимости Проверка 
                if (EntityManager.LocalPlayer.IsValid)
                {
                    if (!(e.FormattedValue is AccountScopeType scope))
                        if (!Enum.TryParse(e.FormattedValue.ToString(), out scope))
                            scope = AccountScopeType.Global;
                    e.Cancel = false;                    
                }
                else
                {
                    if (dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value !=
                        e.FormattedValue)
                    {
                        e.Cancel = true;
                        XtraMessageBox.Show("Impossible change scope of the variable while character is not in game!");
                    }
                }
            } 
            else e.Cancel = false;

            dgvVariablesChanged = (e.Cancel == false
                        && dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != e.FormattedValue);
                
        }

        private void dgvVariables_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if ( ( e.ColumnIndex == AccScopeInd
                    || e.ColumnIndex == ProfScopeInd)
                && dgvVariablesChanged)
            {
                if (!(dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is AccountScopeType scope))
                    if (!Enum.TryParse(dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out scope))
                        scope = AccountScopeType.Global;
                if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out bool profScope))
                    profScope = false;

                // установка квалификатора области видимости
                dgvVariables.Rows[e.RowIndex].Cells[QualifierInd].Value = VariableTools.GetScopeQualifier(scope, profScope);
            }
        }


        private void dgvVariablesToCollection()
        {
            VariableTools.Variables.Clear();
            foreach(DataGridViewRow row in dgvVariables.Rows)
            {
                if(!row.IsNewRow)
                {
                    // Чтение и корректировка имени переменной
                    string name = row.Cells[NameInd].Value.ToString();
                    bool nameOk = !Parser.CorrectForbiddenName(name, out string correctName);
                    if (!nameOk && XtraMessageBox.Show($"The name '{name}' is incorrect! \n" +
                                                       $"Whould you like to change it to '{correctName}'?\n" +
                                                       $"If you select 'NO' the variable will not be stored.",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        name = correctName;
                        row.Cells[NameInd].Value = correctName;
                        nameOk = true;
                    }

                    if (nameOk)
                    {
                        if (!Parser.TryParse(row.Cells[ValueInd].Value, out double val))
                            val = 0;

                        if (!(row.Cells[AccScopeInd].Value is AccountScopeType scope))
                            if (!Enum.TryParse(row.Cells[AccScopeInd].Value.ToString(), out scope))
                                scope = AccountScopeType.Global;

                        if (!Parser.TryParse(row.Cells[SaveInd].Value, out bool save))
                            save = false;

                        VariableContainer v = new VariableContainer(name, val, scope);
                        v.Save = save;

                        if (!VariableTools.Variables.TryAdd(v))
                        {
                            Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}}({scope}) to the collection.");
                            XtraMessageBox.Show($"Failed to store variable {{{name}}}({scope}) to the collection.");
                        }
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}} to the collection.");

                }
            }
            dgvVariablesChanged = false;
        }

        private void FillDgvVariables()
        {
            dgvVariables.Rows.Clear();
            dgvVariablesChanged = false;
            using (var v = VariableTools.Variables.GetEnumerator())
                while (v.MoveNext())
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dgvVariables);
                    newRow.Cells[NameInd].Value = v.Current.Name;
                    newRow.Cells[SaveInd].Value = v.Current.Save;
                    newRow.Cells[ProfScopeInd].Value = v.Current.ProfileScope;
                    newRow.Cells[AccScopeInd].Value = v.Current.AccountScope.ToString();
                    newRow.Cells[QualifierInd].Value = v.Current.ScopeQualifier;
                    newRow.Cells[ValueInd].Value = v.Current.Value;
                    newRow.Tag = v.Current;

                    int ind = dgvVariables.Rows.Add(newRow);
                }
        }

    }
}
