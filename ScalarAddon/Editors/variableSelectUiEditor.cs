
using AstralVariables.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace AstralVariables.Editors
{
    class variableSelectUiEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string var_name = VariablesEditor.GetVariable(value.ToString());
            if (!string.IsNullOrEmpty(var_name) || !string.IsNullOrWhiteSpace(var_name))
                return var_name;
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}