
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
            //if (value is string str)
            //{
            //    AccountScopeType accScope = AccountScopeType.Global;
            //    ProfileScopeType profScope = ProfileScopeType.Common;
            //    if (context.Container is SetVariable setVariable)
            //    {
            //        accScope = setVariable.AccountScope;
            //        profScope = setVariable.ProfileScope;
            //    }
            //    else setVariable = null;


            //    VariableCollection.VariableKey key = VariablesSelectForm.GetVariable(value.ToString(), accScope, profScope);
            //    if (key != null)
            //    {
            //        if (setVariable != null)
            //        {
            //            setVariable.AccountScope = key.AccountScope;
            //            setVariable.ProfileScope = key.ProfileScope;
            //        }
            //        return key.Name;
            //    }
            //    return value;
            //}
            if(value is VariableCollection.VariableKey key)
            {
                VariableCollection.VariableKey newKey = VariablesSelectForm.GetVariable(key);
                if (newKey != null)
                {
                    return newKey;
                }
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}