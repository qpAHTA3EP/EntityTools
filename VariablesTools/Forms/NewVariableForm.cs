using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VariableTools.Classes;
using VariableTools.Expressions;

namespace VariableTools.Forms
{
    public partial class NewVariableForm : XtraForm //*/Form
    {
        private static NewVariableForm @this;
        private NewVariableForm()
        {
            InitializeComponent();

            cbAccountScope.DataSource = Enum.GetValues(typeof(AccountScopeType));
            cbProfileScope.DataSource = Enum.GetValues(typeof(ProfileScopeType));
        }

        private VariableContainer variable;
        static public DialogResult GetVariable(out VariableContainer variable)
        {
            if (@this is null || @this.IsDisposed)
                @this = new NewVariableForm();
            @this.variable = null;

            @this.ShowDialog();
            variable = @this.variable;
            return @this.DialogResult;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbAccountScope.SelectedValue is AccountScopeType accScope
                && cbProfileScope.SelectedValue is ProfileScopeType profScope)
            {
                if (!string.IsNullOrEmpty(tbName.Text))
                {
                    // проверка корректности имени переменной
                    if (Parser.CorrectForbiddenName(tbName.Text, out string correctName))
                        // согласие на замену некорректного имени переменной
                        if (XtraMessageBox.Show(/*Form.ActiveForm, */
                                                $"Задано недопустимое имя переменно '{tbName.Text}'!\n" +
                                                $"Хотите его исправить на '{correctName}'?\n" +
                                                $"The name '{tbName.Text}' is incorrect! \n" +
                                                $"Whould you like to change it to '{correctName}'?",
                                                "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        {
                            tbName.Text = correctName;
                        }
                        else return;
                    // Проверка уникальности переменной
                    var varKey = new VariableCollection.VariableKey(tbName.Text, accScope, profScope);
                    if (VariableTools.Variables.ContainsKey(varKey))
                    {
                        XtraMessageBox.Show($"Переменная {{{varKey.ToString()}}} уже существует.\n\r" +
                                            $"Укажите другое имя переменной или область видимости\n\r" +
                                            $"Variable {{{varKey.ToString()}}} exists." +
                                            $"Change the Name or the Scope", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (!double.TryParse(tbValue.Text, out double value))
                        {
                            XtraMessageBox.Show($"Некорректное значение '{tbValue.Text}'.\n\r" +
                                                $"Incorrect value '{tbValue.Text}'.",
                                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        variable = VariableTools.Variables.Add(value, tbName.Text, accScope, profScope, ckbSave.Checked);

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                {
                    XtraMessageBox.Show(/*Form.ActiveForm, */"Пустое имя переменной не допустимо!\n" +
                                        "Empty variable name is not valid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            else
            {
                XtraMessageBox.Show("Неверное значение 'AccoutScope' или 'ProfielSkope'!\r\n" +
                                    "Incorrect value of the 'AccoutScope' или 'ProfielSkope'!", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tbValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                if (!char.IsDigit(e.KeyChar))
                {
                    if (((e.KeyChar == '.') || (e.KeyChar == ',')) && (tbValue.Text.IndexOf(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]) == -1))
                    {
                        //разделитель еще не стоит
                        e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    }
                    else e.Handled = true;
                }
            }
        }

        private void NewVariableForm_Load(object sender, EventArgs e)
        {
            tbName.Text = string.Empty;
            cbAccountScope.SelectedItem = AccountScopeType.Global;
            cbProfileScope.SelectedItem = ProfileScopeType.Common;
            tbValue.Text = "0";
            tbName.Focus();
        }
    }
}
