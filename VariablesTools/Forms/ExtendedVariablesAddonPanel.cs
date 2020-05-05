using Astral;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Windows.Forms;
using VariableTools.Classes;
using VariableTools.Expressions;
using static VariableTools.Classes.VariableCollection;

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

        public ExtendedVariablesToolsPanel() : base ("Variable Tools")
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
            if (XtraMessageBox.Show("Вы действительно хотите загрузить переменные из файла ?\n\r" +
                                    "Все имзменения будут потеряны! \n\r" +
                                    "Do you really want to load variables from file ?\n\r" +
                                    "All changes will be lost!",
                        "Warring", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                VariableTools.LoadFromFile();
                FillDgvVariables();
            }
        }

        private void bntAdd_Click(object sender, EventArgs e)
        {
            if (NewVariableForm.GetVariable(out VariableContainer variable) == DialogResult.OK)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(dgvVariables);
                newRow.Cells[NameInd].Value = variable.Name;
                newRow.Cells[ValueInd].Value = variable.Value;
                newRow.Cells[AccScopeInd].Value = variable.AccountScope;//.ToString();
                newRow.Cells[ProfScopeInd].Value = variable.ProfileScope;//.ToString();
                newRow.Cells[QualifierInd].Value = variable.ScopeQualifier;
                newRow.Cells[SaveInd].Value = variable.Save;
                newRow.Tag = variable;
                dgvVariables.Rows.Add(newRow);
            }
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
            if (!loadingVariables && Visible
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
                                                    $"Задано недопустимое имя переменной '{name}'!\n\r" +
                                                    $"Хотите его исправить на '{correctName}'?\n\r\n\r" +
                                                    $"The name '{name}' is incorrect! \n\r" +
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
                        XtraMessageBox.Show(/*Form.ActiveForm, */"Пустое имя переменной не допустимо!\n\r\n\r" +
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
#if true
                else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[AccScopeInd].Value, out accScope))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Account' некорректно!\n" +
                                        "Account scope is not valid!", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    return;
                } 
#endif

                ProfileScopeType profScope = ProfileScopeType.Common;
                if (e.ColumnIndex == ProfScopeInd)
                {
                    if (!Parser.TryParse(e.FormattedValue, out profScope))
                    {
                        e.Cancel = true;
                        XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Profile' некорректно!\n\r\n\r" +
                                            "Profile scope is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
#if true
                else if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ProfScopeInd].Value, out profScope))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Поле 'Profile' некорректно!\n\r\n\r" +
                                        "Profile scope is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } 
#endif

                if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ValueInd].Value, out double value))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Задано некорректное значение переменной!\n\r\n\r" + 
                                        "Value of the Variable is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[SaveInd].Value, out bool save))
                {
                    e.Cancel = true;
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Флаг 'Save' имеет некорректное состояние!\n\r\n\r" +
                                        "Save flag of the Variable is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (VariableTools.Variables.TryGetValue(out VariableContainer variableStored, name, accScope, profScope))
                {
                    // В коллекции существует переменная с теми же идентифицирующими признаками,
                    // которые заданы в DataGridView.CurrentRow
                    // Если найденная переменная не соответствует переменной, ассоциированной с текущей строкой
                    // значит заданная комбинация является недопустимой (т.к. повторяет уже существующую)
                    if (dgvVariables.Rows[e.RowIndex].Tag != null && !ReferenceEquals(variableStored, dgvVariables.Rows[e.RowIndex].Tag))
                    {
                        e.Cancel = true;
                        Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{Name}: The combination of {{{name}, {accScope}, {profScope}}} parameters that identify the variable should not be repeated.\n\r");
                        XtraMessageBox.Show(/*Form.ActiveForm, */$"Сочетание параметров {{{name}, {accScope}, {profScope}}} должно быть уникальным для каждой переменной.\n\r\n\r" +
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
                        e.Cancel = true;
                        Logger.WriteLine(Logger.LogType.Debug, $"FAILED to store variable from DataGridViewRow {e.RowIndex}");
                        XtraMessageBox.Show(/*Form.ActiveForm, */$"Сочетание параметров {{{name}, {accScope}, {profScope}}} должно быть уникальным для каждой переменной.\n\r" +
                                            $"Вам необходимо изменить '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'.\n\r\n\r" +
                                            $"The combination of {{{name}, {accScope}, {profScope}}} parameters that identify the variable should not be repeated.\n\r" +
                                            $"You need to change '{dgvVariables.Columns[e.ColumnIndex].HeaderText}'.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

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
                else if (dgvVariables.Rows[e.RowIndex].Tag is VariableContainer variableTag)
                {
                    if (e.ColumnIndex == ValueInd && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ValueInd].Value, out double val))
                        variableTag.Value = val;
                    else if (e.ColumnIndex == SaveInd && Parser.TryParse(dgvVariables.Rows[e.RowIndex].Cells[ValueInd].Value, out bool save))
                        variableTag.Save = save;
                }
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
                    string correctName = string.Empty;
                    bool nameOk = !string.IsNullOrEmpty(name) && !Parser.CorrectForbiddenName(name, out correctName);
                    if (!nameOk && XtraMessageBox.Show(/*Form.ActiveForm, */
                                                       $"Недопустимое имя переменной '{name}'! \n\r" +
                                                       $"Заменить его на корректное '{correctName}'?\n\r" +
                                                       $"Переменная не будет сохранена, если выберите 'NO'.\n\r\n\r" +
                                                       $"The name '{name}' is incorrect! \n\r" +
                                                       $"Whould you like to change it to '{correctName}'?\n\r" +
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
#if true
                            if(VariableTools.Variables.ContainsKey(name, accScope, profScope))
                            {
                                //В коллекции имеется переменная с такими же идентифицирующими признаками,
                                //как и в обрабатываемой строке таблицы
                                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}}[{accScope}, {profScope}] to the collection from row {row.Index}.");
                                XtraMessageBox.Show(/*Form.ActiveForm, */$"Не удалось сохранить переменную {name}[{accScope}, {profScope}] из строки {row.Index}.\n\r\n\r" +
                                    $"Failed to store variable {name}[{accScope}, {profScope}] to the collection from row {row.Index}.");
                            }
                            else row.Tag = VariableTools.Variables.Add(val, name, accScope, profScope, save);

#if disabled_at_20200505_1448
                            VariableContainer newVar = VariableTools.Variables.Add(val, name, accScope, profScope, save);
                            if (newVar != null)
                            {
                                newVar.Save = save;
                                Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}}[{accScope}, {profScope}] to the collection from row {row.Index}.");
                                XtraMessageBox.Show(/*Form.ActiveForm, */$"Failed to store variable {{{name}}}({accScope}) to the collection from row {row.Index}.");
                            } 
#endif
#else
                            if (row.Tag is VariableContainer variableTag)
                            {
                                if (variableTag.Name != name
                                    || variableTag.AccountScope != accScope
                                    || variableTag.ProfileScope != profScope)
                                {
                                    // Идентифицируеющие признаки переменной, ассоциированной со строкой изменились
                                    // проверяем наличие в коллекции переменной с новыми признаками
                                    var newKey = new VariableKey(name, accScope, profScope);
                                    if(VariableTools.Variables.ContainsKey(newKey))
                                    {
                                        // Коллекция не содержит переменной с такими же идентифицирующими признаками.
                                        // Меняет идентифицирующие признаки для текущей переменной
                                        VariableTools.Variables.ChangeItemKey(variableTag, newKey);
                                        variableTag.Value = val;
                                        variableTag.Save = save;
                                    }
                                    else
                                    {
                                        // Коллекция содержит другую переменную с такими же идентифицирующими признаками, 
                                        // которые заданы текущей строкой таблицы
                                        Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{name}}}[{accScope}, {profScope}] to the collection from row {row.Index}.");
                                        XtraMessageBox.Show($"Переменная {newKey.ToString()} уже существует.\n\r" +
                                            $"The Variable {{{newKey.ToString()}}} exists. Change the Name of Scope in row {row.Index}.");
                                        row.Cells[clmnName.DisplayIndex].Selected = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    // Идентифицирующие признаки у переменной не изменились
                                    variableTag.Value = val;
                                    variableTag.Save = save;
                                }
                            }
                            else
                            {
                                // строка таблицы не ассоциирована ни с одной переменной
                                // проверяем наличие в коллекции переменной с такими же идентифицирующими признаками
                                var variable = VariableTools.Variables[name, accScope, profScope];
                                if (variable is null)
                                    // в коллекции отсутствует переменная с такими же идентифицирующими признаками
                                    VariableTools.Variables.Add(val, name, accScope, profScope);
                                else
                                {
                                    // в коллекции существует другая переменная с такими же идентифицирющими признаками
                                    // которые заданы текущей строкой таблицы
                                    Logger.WriteLine(Logger.LogType.Debug, $"{nameof(VariableTools)}::{GetType().Name}: Failed to store variable {{{variable.ToString()}}} to the collection from row {row.Index}.");
                                    XtraMessageBox.Show($"Переменная {variable.ToString()} уже существует.\n\r" +
                                        $"The Variable {{{variable.ToString()}}} exists. Change the Name of Scope in row {row.Index}.");
                                    row.Cells[clmnName.DisplayIndex].Selected = true;
                                    return;
                                }
                            }
#endif
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

                    dgvVariables.Rows.Add(newRow);
                }
            loadingVariables = false;
        }

        private void ckbDebug_CheckedChanged(object sender, EventArgs e)
        {
            VariableTools.DebugMessage = ckbDebug.Checked;
        }

        private void dgvVariables_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVariables.CurrentRow != null && dgvVariables.CurrentRow.Tag != null
                && dgvVariables.CurrentRow.Tag is VariableContainer variable)
            {
                tbLastAssign.Text = variable.LastOperation;
            }
            else tbLastAssign.Text = string.Empty;
        }

        //private void dgvVariables_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        //{
        //    if(dgvVariables.IsCurrentCellDirty)
        //        dgvVariables.CommitEdit(DataGridViewDataErrorContexts.Commit);
        //}
    }
}
