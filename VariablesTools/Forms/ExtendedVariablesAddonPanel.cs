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

        /// <summary>
        /// Флаг, отключающий "Валидацию" на время заполнения dgvVariables исходными данными
        /// </summary>
        public bool loadingVariables = false;

        public ExtendedVariablesToolsPanel() :base ("Variable Tools")
        {
            InitializeComponent();

            clmnAccScope.ValueType = typeof(AccountScopeType);
            clmnAccScope.DataSource = Enum.GetValues(typeof(AccountScopeType));

            clmnProfileScope.ValueType = typeof(ProfileScopeType);
            clmnProfileScope.DataSource = Enum.GetValues(typeof(ProfileScopeType));

            ckbDebug.Checked = VariableTools.DebugMessage;
        }

        private void ExtendedVariablesAddonPanel_Load(object sender, EventArgs e)
        {
            FillDgvVariables();
            loadingVariables = false;
        }

        private void ExtendedVariablesAddonPanel_Leave(object sender, EventArgs e)
        {
            //dgvVariablesToCollection();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //dgvVariablesToCollection();
            VariableTools.SaveToFile();
        }

        private void dntLoad_Click(object sender, EventArgs e)
        {
            if (!loadingVariables ||
                XtraMessageBox.Show(/*Form.ActiveForm, */"Do you really want to load variables from file ?\n" +
                                    "All changes will be lost.",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                VariableTools.LoadFromFile();
                FillDgvVariables();
                loadingVariables = false;
            }
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            DataGridViewRow newRow = new DataGridViewRow();
            VariableContainer newVar = new VariableContainer();
            newRow.CreateCells(dgvVariables);
            newRow.Cells[NameInd].Value = newVar.Name;
            newRow.Cells[ValueInd].Value = newVar.Value;
            newRow.Cells[AccScopeInd].Value = newVar.AccountScope;//.ToString();
            newRow.Cells[ProfScopeInd].Value = newVar.ProfileScope;//.ToString();
            newRow.Cells[QualifierInd].Value = newVar.ScopeQualifier;
            newRow.Cells[SaveInd].Value = newVar.Save;
            newRow.Tag = newVar;
            dgvVariables.Rows.Add(newRow);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvVariables.CurrentRow != null
                && XtraMessageBox.Show(/*Form.ActiveForm, */$"Do you really want to delete variable {{{dgvVariables.CurrentRow.Cells[NameInd].Value.ToString()}}} ?",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                VariableContainer newVar = dgvVariables.CurrentRow.Tag as VariableContainer;
                dgvVariables.Rows.Remove(dgvVariables.CurrentRow);
                VariableTools.Variables.Remove(newVar.Key);
            }
        }

        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!loadingVariables
                && dgvVariables.NewRowIndex != e.RowIndex)
            {
                string name = string.Empty;
                if (e.ColumnIndex == NameInd)
                {
                    // Чтение и корректировка имени переменной
                    // проверка на пустую строку
                    name = e.FormattedValue.ToString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        // проверка корректности имени переменной
                        if (Parser.CorrectForbiddenName(name, out string correctName))
                            // согласие на замену некорректного имени переменной
                            if (XtraMessageBox.Show(/*Form.ActiveForm, */
                                                    $"Задано недопустимое имя переменно '{name}'!\n" +
                                                    $"Хотите его исправить на '{correctName}'?\n" +
                                                    $"The name '{name}' is incorrect! \n" +
                                                    $"Whould you like to change it to '{correctName}'?",
                                                    "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value = correctName;
                                name = correctName;
                            }
                            else
                            {
                                e.Cancel = true;
                                return;
                            }
                    }
                    else
                    {
                        e.Cancel = true;
                        XtraMessageBox.Show(/*Form.ActiveForm, */"Пустое имя переменной не допустимо!\n" +
                                            "Empty variable name is not valid!", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        return;
                    }
                }
                else name = dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value.ToString();

                AccountScopeType accScope = AccountScopeType.Global;
                if (e.ColumnIndex == AccScopeInd)
                {
                    if (!Parser.TryParse(e.FormattedValue, out accScope))
                    {
                        e.Cancel = true;
                        XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Account' некорректно!\n" +
                                            "Account scope is not valid!", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[AccScopeInd].Value, out accScope))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Account' некорректно!\n" +
                                        "Account scope is not valid!", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    return;
                }

                ProfileScopeType profScope = ProfileScopeType.Common;
                if (e.ColumnIndex == ProfScopeInd)
                {
                    if (!Parser.TryParse(e.FormattedValue, out profScope))
                    {
                        e.Cancel = true;
                        XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Profile' некорректно!\n" +
                                            "Profile scope is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out profScope))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Profile' некорректно!\n" + 
                                        "Profile scope is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ValueInd].Value, out double value))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Задано некорректное значение переменной!\n" + 
                                        "Value of the Variable is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[SaveInd].Value, out bool save))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Флаг 'Save' имеет некорректное состояние!\n" +
                                        "Save flag of the Variable is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (VariableTools.Variables.TryGetValue(out VariableContainer variableColl, name, accScope, profScope))
                {
                    // В коллекции существует переменная с теми же идентифицирующими признаками,
                    // которые заданы в DataGridView.CurrentRow
                    // Если найденная переменная не соответствует переменной, связанной с текущей строкой
                    // значит заданная комбинация является недопустимой (т.к. повторяет уже существующую)
                    if (!object.ReferenceEquals(variableColl, dgvVariables.Rows[e.RowIndex].Tag))
                    {
                        e.Cancel = true;
                        Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{Name}: The combination of {{{name}, {accScope}, {profScope}}} parameters that identify the variable should not be repeated.\n");
                        XtraMessageBox.Show(/*Form.ActiveForm, */$"Сочетание параметров {{{name}, {accScope}, {profScope}}} должно быть уникальным для каждой переменной.\n" +
                            $"Вам необходимо изменить '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'.\n" +
                            $"The combination of {{{name}, {accScope}, {profScope}}} parameters that identify the variable should not be repeated.\n" +
                            $"You need to change '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (dgvVariables.Rows[e.RowIndex].Tag is VariableContainer variableTag
                            && variableTag.IsValid)
                {
                    // связанная со строкой переменная находится в коллекции переменных
                    // при этом в коллекции отсутствует переменная с такими же идентифицирующими признаками
                    variableTag.Name = name;
                    variableTag.AccountScope = accScope;
                    variableTag.ProfileScope = profScope;
                    variableTag.Value = value;
                    variableTag.Save = save;
                }
                else
                {
                    // Переменная, связанная со строкой не задана, либо не сохранена в коллекцию
                    variableTag = VariableTools.Variables.Add(value, name, accScope, profScope);
                    if (variableTag != null)
                        dgvVariables.Rows[e.RowIndex].Tag = variableTag;
                    else
                    {
                        Logger.WriteLine(Logger.LogType.Debug, $"FAILED to store variable from DataGridViewRow {e.RowIndex}");
                        XtraMessageBox.Show(/*Form.ActiveForm, */$"Сочетание параметров {{{name}, {accScope}, {profScope}}} должно быть уникальным для каждой переменной.\n" +
                            $"Вам необходимо изменить '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'.\n" +
                            $"The combination of {{{name}, {accScope}, {profScope}}} parameters that identify the variable should not be repeated.\n" +
                            $"You need to change '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'.",
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                    }
                }
            }
        }
        //private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (dgvVariables.NewRowIndex != e.RowIndex
        //        || e.FormattedValue.Equals(dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value))
        //        return;

        //    if (e.ColumnIndex != NameInd
        //        || e.ColumnIndex == AccScopeInd
        //        || e.ColumnIndex == ProfScopeInd)
        //    {
        //        string name = string.Empty;
        //        if (e.ColumnIndex == NameInd)
        //        {
        //            // Чтение и корректировка имени переменной
        //            // проверка на пустую строку
        //            if (!string.IsNullOrEmpty(name))
        //            {
        //                // проверка корректности имени переменной
        //                if (Parser.CorrectForbiddenName(name, out string correctName))
        //                    // согласие на замену некорректного имени переменной
        //                    if (XtraMessageBox.Show($"The name '{name}' is incorrect! \n" +
        //                                            $"Whould you like to change it to '{correctName}'?",
        //                                            "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        //                    {
        //                        dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value = correctName;
        //                        name = correctName;
        //                    }
        //                    else
        //                    {
        //                        e.Cancel = true;
        //                        return;
        //                    }
        //            }
        //            else
        //            {
        //                e.Cancel = true;
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            name = dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value.ToString();
        //        }

        //        AccountScopeType accScope = AccountScopeType.Global;
        //        if (e.ColumnIndex == AccScopeInd)
        //        {
        //            // области видимости Проверка 
        //            if (EntityManager.LocalPlayer.IsValid)
        //            {
        //                if (!Parser.TryParse(e.FormattedValue, out accScope))
        //                {
        //                    e.Cancel = true;
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                if (dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value !=
        //                    e.FormattedValue)
        //                {
        //                    e.Cancel = true;
        //                    XtraMessageBox.Show("Impossible to change AccountScope of the variable while character is not in game!");
        //                    return;
        //                }
        //            }
        //        }
        //        else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[AccScopeInd].Value, out accScope))
        //        {
        //            e.Cancel = true;
        //            return;
        //        }

        //        ProfileScopeType profScope = ProfileScopeType.Common;
        //        if (e.ColumnIndex == ProfScopeInd)
        //        {
        //            if (!Parser.TryParse(e.FormattedValue, out profScope))
        //            {
        //                e.Cancel = true;
        //                return;
        //            }
        //        }
        //        else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out profScope))
        //        {
        //            e.Cancel = true;
        //            return;
        //        }

        //        VariableCollection.VariableKey key = new VariableCollection.VariableKey(name, accScope, profScope);
        //        if (VariableTools.Variables.ContainsKey(key))
        //        {
        //            e.Cancel = true;
        //            XtraMessageBox.Show($"The combination of {{{name}, '{key.Qualifier}'}} parameters that identify the variable should not be repeated.");
        //            return;
        //        }

        //        dgvVariablesChanged = dgvVariablesChanged || (e.Cancel == false
        //                    && dgvVariables.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != e.FormattedValue);
        //    }
        //    e.Cancel = false;
        //}

        private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!loadingVariables
                && dgvVariables.NewRowIndex != e.RowIndex)
            {
                if (e.ColumnIndex == AccScopeInd
                  || e.ColumnIndex == ProfScopeInd)
                {
                    if (Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[AccScopeInd].Value, out AccountScopeType accScope)
                        && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out ProfileScopeType profScope))
                    {
                        dgvVariables.Rows[e.RowIndex].Cells[QualifierInd].Value = VariableTools.GetScopeQualifier(accScope, profScope);
                    }
                    else dgvVariables.Rows[e.RowIndex].Cells[QualifierInd].Value = string.Empty;
                }
            }
        }
        //private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == AccScopeInd
        //        || e.ColumnIndex == ProfScopeInd
        //        || e.ColumnIndex == NameInd)
        //    {
        //        string name = dgvVariables.Rows[e.RowIndex].Cells[NameInd].Value.ToString();
        //        if (!string.IsNullOrEmpty(name)
        //            && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[AccScopeInd].Value, out AccountScopeType accScope)
        //            && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out ProfileScopeType profScope)
        //            && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[SaveInd].Value, out bool save)
        //            && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ValueInd].Value, out double value))
        //        {
        //            // установка квалификатора области видимости
        //            dgvVariables.Rows[e.RowIndex].Cells[QualifierInd].Value = VariableTools.GetScopeQualifier(accScope, profScope);

        //            if (dgvVariables.Rows[e.RowIndex].Tag is VariableContainer variable)
        //            {
        //                variable.Name = name;
        //                variable.AccountScope = accScope;
        //                variable.ProfileScope = profScope;
        //                variable.Value = value;
        //                variable.Save = save;

        //                dgvVariablesChanged = false;
        //                return;
        //            }
        //            else
        //            {
        //                // Внесение изменений в коллекцию
        //                VariableCollection.VariableKey key = new VariableCollection.VariableKey(name, accScope, profScope);
        //                if (VariableTools.Variables.TryGetValue(out variable, key))
        //                {
        //                    variable.Value = value;
        //                    variable.Save = save;

        //                    dgvVariablesChanged = false;
        //                    return;
        //                }
        //                else
        //                {
        //                    VariableTools.Variables.Add(value, name, accScope, profScope);
        //                    dgvVariablesChanged = false;
        //                    return;
        //                }
        //            }
        //        }

        //        Logger.WriteLine(Logger.LogType.Debug, $"FAILED to store variable from DataGridViewRow {e.RowIndex}");
        //    }
        //}

        private void dgvVariablesToCollection()
        {
            VariableTools.Variables.Clear();
            foreach(DataGridViewRow row in dgvVariables.Rows)
            {
                if(!row.IsNewRow)
                {
                    // Чтение и корректировка имени переменной
                    string name = row.Cells[NameInd].Value.ToString();
                    string correctName = string.Empty;
                    bool nameOk = !string.IsNullOrEmpty(name) && !Parser.CorrectForbiddenName(name, out correctName);
                    if (!nameOk && XtraMessageBox.Show(/*Form.ActiveForm, */
                                                       $"The name '{name}' is incorrect! \n" +
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
                        if (Parser.TryParse(row.Cells[ValueInd].Value, out double val)
                            && Parser.TryParse(row.Cells[AccScopeInd].Value, out AccountScopeType accScope)
                            && Parser.TryParse(row.Cells[ProfScopeInd].Value, out ProfileScopeType profScope)
                            && Parser.TryParse(row.Cells[SaveInd].Value, out bool save))
                        {

                            VariableContainer newVar = new VariableContainer(val, name, accScope, profScope)
                            {
                                Save = save
                            };
                            if (!VariableTools.Variables.TryAdd(newVar))
                            {
                                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}}[{accScope}, {profScope}] to the collection from row {row.Index}.");
                                XtraMessageBox.Show(/*Form.ActiveForm, */$"Failed to store variable {{{name}}}({accScope}) to the collection from row {row.Index}.");
                            }
                        }
                        else Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}} to the collection from row {row.Index}.");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}} to the collection from row {row.Index}.");

                }
            }
            loadingVariables = false;
        }

        private void FillDgvVariables()
        {
            loadingVariables = true;
            dgvVariables.Rows.Clear();
            loadingVariables = false;
            using (var v = VariableTools.Variables.GetEnumerator())
                while (v.MoveNext())
                {
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(dgvVariables);
                    newRow.Cells[NameInd].Value = v.Current.Name;
                    newRow.Cells[SaveInd].Value = v.Current.Save;
                    newRow.Cells[ProfScopeInd].Value = v.Current.ProfileScope;//.ToString();
                    newRow.Cells[AccScopeInd].Value = v.Current.AccountScope;//.ToString();
                    newRow.Cells[QualifierInd].Value = v.Current.ScopeQualifier;
                    newRow.Cells[ValueInd].Value = v.Current.Value;
                    newRow.Tag = v.Current;

                    int ind = dgvVariables.Rows.Add(newRow);
                }
            loadingVariables = false;
        }

        private void ckbDebug_CheckedChanged(object sender, EventArgs e)
        {
            VariableTools.DebugMessage = ckbDebug.Checked;
        }

        //private void dgvVariables_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        //{
        //    if(dgvVariables.IsCurrentCellDirty)
        //        dgvVariables.CommitEdit(DataGridViewDataErrorContexts.Commit);
        //}
    }
}
