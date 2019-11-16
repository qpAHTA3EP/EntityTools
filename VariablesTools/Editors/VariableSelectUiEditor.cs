
using VariableTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using VariableTools.Actions;
using VariableTools.Classes;

namespace VariableTools.Editors
{
    class VariableSelectUiEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //VariableContainer varContainer = null;
            //if (context.Container is SetVariable setVar)
            //    varContainer = setVar.variableContainer;

            //if (varContainer != null && varContainer.IsValid)
            //{
            //    VariablesEditor.GetVariable(varContainer);
            //    if (varContainer != null)
            //    {
            //        setVar.variableContainer = varContainer;
            //        setVar.
            //    }
            //}
            //else
            {
                string var_name = VariablesEditor.GetVariable(value.ToString());
                if (!string.IsNullOrEmpty(var_name) || !string.IsNullOrWhiteSpace(var_name))
                    return var_name;
                return value;
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}