using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstralVars.Classes;
using MyNW.Classes;
using MyNW.Internals;

namespace AstralVars.Forms
{
    public partial class VariablesEditor : Form
    {
        //class Pair
        //{
        //    public string Name { get; set; }
        //    public string DisplayName { get; set; }
        //}
        /// <summary>
        /// Кэш Списока предметов с идентификаторами (InternalName), находящимися в сумке персонажа
        /// Key -> InternalName
        /// Value -> отображаемая в списке строка вида "InternalName [DisplayName]"
        /// </summary>
        //private List<Pair> itemsDictionary = new List<Pair>();

        private static VariablesEditor varEditor;
        private VarCollection varSource;

        public VariablesEditor()
        {
            InitializeComponent();
        }

        public static Variable GetVariable(VarCollection vars)
        {
            if (varEditor == null)
            {
                varEditor = new VariablesEditor();

                //заполняем список доступных для выбора значений "типов" строками
                varEditor.clmnType.Items.Clear();
                varEditor.clmnType.Items.AddRange(Enum.GetNames(typeof(VarTypes)));

                // Следующая реализация вызывает ошибку при попытке выбрать двугое значение в списке
                //varEditor.clmnType.DataSource = VarParcer.varTypes;

                varEditor.dgvVariables.ReadOnly = true;

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
                    if(varEditor.dgvVariables.CurrentRow.Tag is Variable var)
                        return var;
                }
            }
            return null;
        }

        /// <summary>
        /// Заполение dgvVariables списком переменных из vars
        /// </summary>
        /// <param name="vars">Список отображаемых переменных</param>
        private void FillDgvVariables(VarCollection vars = null)
        {
            if (vars != null)
                varSource = vars;

            varEditor.dgvVariables.Rows.Clear();
            //itemsDictionary.Clear();

#if AstralLoaded
            //List<InventorySlot> items = (EntityManager.LocalPlayer.IsValid) ? EntityManager.LocalPlayer.AllItems : new List<InventorySlot>();
            
            //foreach (InventorySlot item in items)
            //{
            //    string dispName = $"{item.Item.ItemDef.InternalName} [{item.Item.ItemDef.DisplayName}]";
            //    itemsDictionary.Add(item.Item.ItemDef.InternalName, dispName);
            //    //cbCell.Items.Add(dispName);
            //} 
#else
            //itemsDictionary.Add(new Pair() { Name = "item_1", DisplayName = "item_1 [11]" });
            //itemsDictionary.Add(new Pair() { Name = "item_2", DisplayName = "item_2 [22]" });
            //itemsDictionary.Add(new Pair() { Name = "item_3", DisplayName = "item_3 [33]" });
            //itemsDictionary.Add(new Pair() { Name = "item_4", DisplayName = "item_4 [44]" });
            //itemsDictionary.Add(new Pair() { Name = "Artifactfood", DisplayName = "Artifactfood [Artifactfood_55]" });
            //itemsDictionary.Add(new Pair() { Name = "Gemfood", DisplayName = "Gemfood [Gemfood_55]" });
#endif

            foreach (Variable var in varSource)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(varEditor.dgvVariables);
                newRow.Cells[varEditor.clmnName.DisplayIndex].Value = var.Key;
                newRow.Cells[varEditor.clmnType.DisplayIndex].Value = var.VarType.ToString();
                switch (var.VarType)
                {
                    case VarTypes.Boolean:
                        {
                            DataGridViewComboBoxCell cbCell = newRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewComboBoxCell;
                            bool newCellFlag = false;
                            if (newCellFlag = cbCell == null)
                                cbCell = new DataGridViewComboBoxCell();

                            cbCell.Items.Clear();
                            cbCell.Items.AddRange(new string[] { System.Boolean.TrueString, System.Boolean.FalseString });
                            cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                            cbCell.FlatStyle = FlatStyle.Flat;
                            cbCell.Value = var.Value.ToString();

                            if (newCellFlag)
                                newRow.Cells[varEditor.clmnValue.DisplayIndex] = cbCell;
                            break;
                        }
                    case VarTypes.Number:
                        {
                            DataGridViewTextBoxCell tbCell = newRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewTextBoxCell;
                            bool newCellFlag = false;

                            if (newCellFlag = tbCell == null)
                                tbCell = new DataGridViewTextBoxCell();
                            tbCell.Value = var.Value;

                            if (newCellFlag)
                                newRow.Cells[varEditor.clmnValue.DisplayIndex] = tbCell;
                            break;
                        }
                    case VarTypes.DateTime:
                        {
                            DataGridViewDateTimeCell dtCell = newRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewDateTimeCell;
                            bool newCellFlag = false;

                            if (newCellFlag = dtCell == null)
                                dtCell = new DataGridViewDateTimeCell();

                            dtCell.Value = var.Value;

                            if (newCellFlag)
                                newRow.Cells[varEditor.clmnValue.DisplayIndex] = dtCell;
                            break;
                        }
                    case VarTypes.String:
                        {
                            DataGridViewTextBoxCell tbCell = newRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewTextBoxCell;
                            bool newCellFlag = false;

                            if (newCellFlag = tbCell == null)
                                tbCell = new DataGridViewTextBoxCell();
                            tbCell.Value = var.Value;

                            if (newCellFlag)
                                newRow.Cells[varEditor.clmnValue.DisplayIndex] = tbCell;
                            break;
                        }
                    //case VarTypes.Counter:
                    //    {
                    //        DataGridViewComboBoxCell cbCell = newRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewComboBoxCell;
                    //        bool newCellFlag = false;

                    //        if (newCellFlag = cbCell == null)
                    //            cbCell = new DataGridViewComboBoxCell();

                    //        cbCell.Items.Clear();

                    //        cbCell.DataSource = itemsDictionary;
                    //        cbCell.DisplayMember = "DisplayName";
                    //        cbCell.ValueMember = "Name";

                    //        cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                    //        cbCell.FlatStyle = FlatStyle.Flat;
                    //        cbCell.Value = var.Value;

                    //        if (newCellFlag)
                    //            newRow.Cells[varEditor.clmnValue.DisplayIndex] = cbCell;
                    //        break;
                    //    }
                }

                newRow.Tag = var;
                varEditor.dgvVariables.Rows.Add(newRow);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvVariables.Rows)
            {
                if (row.IsNewRow)
                    continue;
                
                Variable var = null;
                if ((var = VariablesAddon.Variables.Set(row.Cells[clmnName.DisplayIndex].Value,
                                                        row.Cells[clmnType.DisplayIndex].Value,
                                                        row.Cells[clmnValue.DisplayIndex].Value)) == null)
                {
                    row.Tag = null;
                    DialogResult dResult = MessageBox.Show(varEditor, $"Variable [{row.Index+1}]'{row.Cells[clmnName.DisplayIndex].Value}' is incorect.\n" +
                                                                       "It will be deleted from the collection." +
                                                                       "Do you want to correct it ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dResult == DialogResult.Yes)
                    {
                        DialogResult = DialogResult.Cancel;
                        dgvVariables.Rows[row.Index].Selected = true;
                    }
                    else DialogResult = DialogResult.OK;
                }
                else
                {
                    row.Tag = var;
                    DialogResult = DialogResult.OK;
                }
            }
            if (DialogResult == DialogResult.OK)
                Close();
        }

        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewRow curRow = dgvVariables.Rows[e.RowIndex];
            if (curRow != null && !curRow.IsNewRow)
            {
                // Проверка валидности значения ячейки в столбце значений 'clmnValue'
                if (e.ColumnIndex == clmnValue.DisplayIndex)
                {
                    // пустая строка в качестве значения переменной - допускается
                    string val = e.FormattedValue.ToString().Trim();
                    if (!string.IsNullOrEmpty(val))
                    {
                        if( curRow.Cells[clmnType.DisplayIndex].Value != null &&
                            VarParcer.GetType(curRow.Cells[clmnType.DisplayIndex].Value, out VarTypes newVarType))
                        {
                            switch (newVarType)
                            {
                                case VarTypes.Boolean:
                                    e.Cancel = !VarParcer.TryParse(val, out bool bRes);
                                    break;
                                case VarTypes.Number:
                                    e.Cancel = !VarParcer.TryParse(val, out double nRres);
                                    break;
                                case VarTypes.DateTime:
                                    e.Cancel = !VarParcer.TryParse(val, out DateTime dtRes);
                                    break;
                            }

                            if(e.Cancel)
                                MessageBox.Show($"Invalid value '{val}' for variables type of '{newVarType}'");
                        }
                    }
                }
                // Проверка валидности значения ячейки в столбце имени переменной 'clmnName'
                else if (e.ColumnIndex == clmnName.DisplayIndex)
                {
                    if(string.IsNullOrEmpty(e.FormattedValue.ToString()))
                    {
                        MessageBox.Show(varEditor, "Empty name for variable is not allowed", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }

                    if(!VarParcer.CheckVarName(e.FormattedValue.ToString()))
                    {
                        MessageBox.Show(varEditor, $"Name for variable '{e.FormattedValue}' is not allowed because contains forbiddent parts:\n"
                                                    + VarParcer.ForbiddenNameParts,
                                                    "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }

                    // Добавить проверку уникальности имени переменной
                    foreach(DataGridViewRow r in dgvVariables.Rows)
                    {
                        if (!r.IsNewRow && r.Index != e.RowIndex && e.FormattedValue.Equals(r.Cells[e.ColumnIndex].Value))
                        {
                            MessageBox.Show(varEditor, "Name of the variable should be unique!\n" +
                                                       $"Variable '{e.FormattedValue}' already is present in the collection", "Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }
                // Проверка валидности значения ячейки в столбце типа переменной 'clmnType'
                else if (e.ColumnIndex == clmnType.DisplayIndex)
                {
                    // Преобразование нового значения ячейки к VarType
                    if (VarParcer.GetType(e.FormattedValue.ToString(), out VarTypes newType))
                    {
                        bool needCheckValue = true;
                        VarTypes oldType = newType;
                        

                        // Преобразование старого значения ячейки к VarType
                        if (curRow.Cells[e.ColumnIndex].Value == null)
                        {
                            oldType = newType;
                            needCheckValue = true;
                        }
                        else if(VarParcer.GetType(curRow.Cells[e.ColumnIndex].Value, out oldType))
                        {
                            if (oldType != newType)
                            {
                                DialogResult dResult = MessageBox.Show(varEditor, $"You change type of variable '{curRow.Cells[clmnName.DisplayIndex].Value}' from '{oldType}' to '{newType}'!\n" +
                                                       $"The value of the variable will be reset, if it cannot be converted to '{newType}'!\n" +
                                                       $"Do you confirm the changes?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dResult != DialogResult.Yes)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                                else
                                {
                                    needCheckValue = true;
                                }
                            }
                        }
                        // Не удалось получись старый тип переменной
                        else
                        {
                            oldType = newType;
                            needCheckValue = true;
                        }

                        
                        if(needCheckValue) 
                        {
                            switch (newType)
                            {
                                case VarTypes.Boolean:
                                    {
                                        bool bRes = BoolVar.Default;
                                        if(curRow.Cells[clmnValue.DisplayIndex].Value != null)
                                            VarParcer.TryParse(curRow.Cells[clmnValue.DisplayIndex].Value.ToString(), out bRes);

                                        bool newCellFlag = !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewComboBoxCell);

                                        DataGridViewComboBoxCell cbCell;
                                        if (newCellFlag)
                                            cbCell = new DataGridViewComboBoxCell();
                                        else
                                        {
                                            cbCell = curRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewComboBoxCell;
                                            cbCell.DataSource = null;
                                            cbCell.Items.Clear();
                                        }
                                        cbCell.Items.AddRange(new string[] { System.Boolean.TrueString, System.Boolean.FalseString });
                                        cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                                        cbCell.FlatStyle = FlatStyle.Flat;
                                        cbCell.Value = bRes.ToString();

                                        if (newCellFlag)
                                            curRow.Cells[varEditor.clmnValue.DisplayIndex] = cbCell;
                                        break;
                                    }
                                case VarTypes.Number:

                                    {
                                        double nRres = NumVar.Default;
                                        if (curRow.Cells[clmnValue.DisplayIndex].Value != null)
                                            VarParcer.TryParse(curRow.Cells[clmnValue.DisplayIndex].Value.ToString(), out nRres);

                                        bool newCellFlag = !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewTextBoxCell && !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewDateTimeCell));

                                        DataGridViewTextBoxCell tbCell;
                                        if (newCellFlag)
                                            tbCell = new DataGridViewTextBoxCell();
                                        else tbCell = curRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewTextBoxCell;
                                        tbCell.Value = nRres;

                                        if (newCellFlag)
                                            curRow.Cells[varEditor.clmnValue.DisplayIndex] = tbCell;
                                        break;
                                    }
                                case VarTypes.DateTime:
                                    {
                                        DateTime dtRes = DateTimeVar.Default;
                                        if (curRow.Cells[clmnValue.DisplayIndex].Value != null)
                                            VarParcer.TryParse(curRow.Cells[clmnValue.DisplayIndex].Value.ToString(), out dtRes);

                                        bool newCellFlag = !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewDateTimeCell);

                                        DataGridViewDateTimeCell dtCell;
                                        if (newCellFlag)
                                            dtCell = new DataGridViewDateTimeCell();
                                        else dtCell = curRow.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewDateTimeCell;

                                        dtCell.Value = dtRes;

                                        if (newCellFlag)
                                            curRow.Cells[varEditor.clmnValue.DisplayIndex] = dtCell;

                                        break;
                                    }
                                //case VarTypes.Counter:
                                //    {
                                //        VarParcer.TryParse(row.Cells[clmnValue.DisplayIndex].Value.ToString(), out string cntRes);

                                //        bool newCellFlag = !(row.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewComboBoxCell);

                                //        DataGridViewComboBoxCell cbCell;
                                //        if (newCellFlag)
                                //            cbCell = new DataGridViewComboBoxCell();
                                //        else cbCell = row.Cells[varEditor.clmnValue.DisplayIndex] as DataGridViewComboBoxCell; ;

                                //        if (cbCell.DataSource == null)
                                //            cbCell.Items.Clear();

                                //        cbCell.DataSource = itemsDictionary;
                                //        cbCell.DisplayMember = "Value";

                                //        cbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                                //        cbCell.FlatStyle = FlatStyle.Flat;
                                //        cbCell.Value = cntRes;

                                //        if (newCellFlag)
                                //            row.Cells[varEditor.clmnValue.DisplayIndex] = cbCell;

                                //        break;
                                //    }
                                case VarTypes.String:
                                    {
                                        bool newCellFlag = !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewTextBoxCell && !(curRow.Cells[varEditor.clmnValue.DisplayIndex] is DataGridViewDateTimeCell));
                                        if (newCellFlag)
                                        {
                                            DataGridViewTextBoxCell tbCell = new DataGridViewTextBoxCell();
                                            if (curRow.Cells[clmnValue.DisplayIndex].Value == null)
                                                tbCell.Value = StrVar.Default;
                                            else tbCell.Value = curRow.Cells[varEditor.clmnValue.DisplayIndex].Value;
                                            
                                            curRow.Cells[varEditor.clmnValue.DisplayIndex] = tbCell;
                                        }
                                        break;
                                    }
                            }
                        }                        
                    }
                    else e.Cancel = true;
                }
            }
        }

        private void dntReload_Click(object sender, EventArgs e)
        {
            FillDgvVariables();
        }

        private void chbAllowEdit_CheckedChanged(object sender, EventArgs e)
        {
            dgvVariables.ReadOnly = !chbAllowEdit.Checked;
            if (dgvVariables.ReadOnly)
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVariables.AllowUserToAddRows = false;
            }
            else
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvVariables.AllowUserToAddRows = true;
            }
        }

        private void dgvVariables_ReadOnlyChanged(object sender, EventArgs e)
        {
            chbAllowEdit.Checked = !dgvVariables.ReadOnly;
            if (dgvVariables.ReadOnly)
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVariables.AllowUserToAddRows = false;
            }
            else
            {
                dgvVariables.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvVariables.AllowUserToAddRows = true;
            }
        }

        private void dgvVariables_RowCanSelect(object sender, DataGridViewCellEventArgs e)
        {
            bool valid = false;
            DataGridViewRow row = dgvVariables.Rows[e.RowIndex];

            valid = !(row.IsNewRow
                      && VarParcer.CheckVarName(row.Cells[clmnName.DisplayIndex].Value)
                      && VariablesAddon.Variables.Set(row.Cells[clmnName.DisplayIndex].Value,
                                                   row.Cells[clmnType.DisplayIndex].Value,
                                                   row.Cells[clmnValue.DisplayIndex].Value) == null);
            btnSelect.Enabled = valid;
        }
    }
}
