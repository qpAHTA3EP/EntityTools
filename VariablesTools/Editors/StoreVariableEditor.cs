using VariableTools.Expressions;
using VariableTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using VariableTools.Classes;
using DevExpress.XtraEditors;
using VariableTools.Actions;
using System.Windows.Forms;

namespace VariableTools.Editors
{
    class StoreVariableEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if(context.Instance is SetVariable SetVarCommand)
            {
                if (SetVarCommand.Key != null && SetVarCommand.Key.IsValid)
                {
                    double result = 0;
                    switch (XtraMessageBox.Show("Yes: Вычислить 'Equation' и сохранить результат в переменную.\n" +
                                               "        (Calculate an 'Equation' and save the result to the variable).\n" +
                                               "No: Записать в переменную значение 0 (ноль)\n" +
                                               "      (Save zero value to the variable).",
                                               "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            if (!SetVarCommand.Equation.Calcucate(out result))
                                result = 0;
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            break;
                        default:
                            return value;
                    }

                    if (VariableTools.Variables.TryGetValue(out VariableContainer variable, SetVarCommand.Key))
                        variable.Value = result;
                    else variable = VariableTools.Variables.Add(result, SetVarCommand.Key.Name, SetVarCommand.Key.AccountScope, SetVarCommand.Key.ProfileScope);

                    if (variable != null)
                        XtraMessageBox.Show($"В переменную {variable.ToString()} записано значение '{variable.Value}'\n" +
                            $"The variable {variable.ToString()} stored with the value '{variable.Value}'");
                    else XtraMessageBox.Show($"Сохранение переменной {variable.ToString()} НЕ БЫЛО ВЫПОЛНЕНО !\n" +
                            $"The variable {variable.ToString()} was not stored!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else XtraMessageBox.Show("Имя переменной некорректно!\nThe variable name is invalid!", 
                                         "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
