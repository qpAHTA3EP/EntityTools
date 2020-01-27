using System;
using System.Linq;
using System.Windows.Forms;
using VariableTools.Expressions;
using DevExpress.XtraEditors;
using VarCollection = System.Collections.Generic.Dictionary<string, double>;
using VariableTools.Classes;
using static VariableTools.Classes.VariableCollection;

namespace VariableTools.Forms
{
    public partial class VariablesSelectForm :  XtraForm //*/ Form
    {
        public int NameInd { get => clmnName.DisplayIndex; }
        public int ValueInd { get => clmnValue.DisplayIndex; }
        public int AccScopeInd { get => clmnAccScope.DisplayIndex; }
        public int ProfScopeInd { get => clmnProfScope.DisplayIndex; }

        private static VariablesSelectForm varEditor;

        public VariablesSelectForm()
        {
            InitializeComponent();
            // Вызывает Exeption
            clmnAccScope.ValueType = typeof(AccountScopeType);
            clmnProfScope.ValueType = typeof(ProfileScopeType);
        }

        public static VariableKey GetVariable(VariableKey Key)
        {
            if (varEditor == null)
                varEditor = new VariablesSelectForm();

            // Заполнение DataGridView значениями
            if(Key != null)
                varEditor.FillDgvVariables(Key.Name, Key.AccountScope, Key.ProfileScope);
            else    varEditor.FillDgvVariables();


            // отображение редактора переменных
            DialogResult result = varEditor.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.AccScopeInd].Value, out AccountScopeType accScope)
                    && Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.ProfScopeInd].Value, out ProfileScopeType profScope))
                {
                    if (varEditor.dgvVariables.CurrentRow.Tag is VariableContainer variable)
                        return variable.Key;
                    else
                    {
                        string name = varEditor.dgvVariables.CurrentRow.Cells[varEditor.NameInd].Value.ToString();
                        if (!string.IsNullOrEmpty(name)
                            && Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.AccScopeInd].Value, out AccountScopeType ascope)
                            && Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.ProfScopeInd].Value, out ProfileScopeType pscope))
                            return new VariableKey(name, ascope, pscope);
                    }
                }
            }

            return null;
        }

        public static VariableKey GetVariable(string var_name = "", AccountScopeType accScope = AccountScopeType.Global, ProfileScopeType profScope = ProfileScopeType.Common)
        {
            if (varEditor == null)
                varEditor = new VariablesSelectForm();

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables(var_name, accScope, profScope);

            // отображение редактора переменных
            DialogResult result = varEditor.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (varEditor.dgvVariables.CurrentRow.Tag is VariableContainer variable)
                    return variable.Key;
                else
                {
                    string name = varEditor.dgvVariables.CurrentRow.Cells[varEditor.NameInd].Value.ToString();
                    if (!string.IsNullOrEmpty(name)
                        && Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.AccScopeInd].Value, out AccountScopeType ascope)
                        && Parser.TryParse(varEditor.dgvVariables.CurrentRow.Cells[varEditor.ProfScopeInd].Value, out ProfileScopeType pscope))
                            return new VariableKey(name, ascope, pscope);
                }
            }

            return null;
        }

        /// <summary>
        /// Заполение dgvVariables списком переменных из vars
        /// </summary>
        /// <param name="vars">Список отображаемых переменных</param>
        private void FillDgvVariables(string cur_var = "", AccountScopeType accScope = AccountScopeType.Global, ProfileScopeType profScope = ProfileScopeType.Common)
        {
            varEditor.dgvVariables.Rows.Clear();

            int curVarInd = -1;
            foreach (var v in VariableTools.Variables.ToList())
            {
                if (v.IsScoped)
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(varEditor.dgvVariables);
                    newRow.Cells[NameInd].Value = v.Name;
                    newRow.Cells[AccScopeInd].Value = v.AccountScope;
                    newRow.Cells[ProfScopeInd].Value = v.ProfileScope;
                    newRow.Cells[ValueInd].Value = v.Value;
                    newRow.Tag = v;
                    int ind = varEditor.dgvVariables.Rows.Add(newRow);

                    if (v.Name == cur_var
                        && v.AccountScope == accScope
                        && v.ProfileScope == profScope)
                        curVarInd = ind; // сохраняем индекс строки выбранной переменной
                }
            }

            if(curVarInd >= 0)
                dgvVariables.Rows[curVarInd].Cells[NameInd].Selected = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }
    }
}
