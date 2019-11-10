
using VariableTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace VariableTools.Editors
{
    class VariableSelectUiEditor : UITypeEditor
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