using System;
using System.Linq;
using System.Windows.Forms;
using VariableTools.Expressions;
using DevExpress.XtraEditors;
using VarCollection = System.Collections.Generic.Dictionary<string, double>;
using VariableTools.Classes;

namespace VariableTools.Forms
{
    public partial class VariablesEditor :  XtraForm //*/ Form
    {
        public int NameInd { get => clmnName.DisplayIndex; }
        public int ValueInd { get => clmnValue.DisplayIndex; }
        public int AccScopeInd { get => clmnAccScope.DisplayIndex; }
        public int ProfScopeInd { get => clmnProfScope.DisplayIndex; }

        private static VariablesEditor varEditor;

        public VariablesEditor()
        {
            InitializeComponent();
        }

        public static string GetVariable(VariableContainer varCol)
        {
            if (varEditor == null)
                varEditor = new VariablesEditor();

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables();

            // отображение редактора переменных
            DialogResult result = varEditor.ShowDialog();
            if (result == DialogResult.OK)
                return varEditor.dgvVariables.CurrentRow.Cells[varEditor.clmnName.DisplayIndex].Value.ToString();

            return string.Empty;
        }

        public static string GetVariable(string var_name = "", string scopeQualifier = "", AccountScopeType scope = AccountScopeType.Global)
        {
            if (varEditor == null)
                varEditor = new VariablesEditor();

            // Заполнение DataGridView значениями
            varEditor.FillDgvVariables(var_name, scopeQualifier, scope);

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
        private void FillDgvVariables(string cur_var = "", string scopeQualifier = "", AccountScopeType scope = AccountScopeType.Global)
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
                    newRow.Cells[ValueInd].Value = v.Value;
                    int ind = varEditor.dgvVariables.Rows.Add(newRow);

                    if (v.Name == cur_var)
                        curVarInd = ind; // сохраняем индекс строки выбранной переменной
                }
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
            DialogResult = DialogResult.OK;
            Close();
        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }
    }
}
