using AstralVariables.Expressions;
using AstralVariables.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace AstralVariables.Editors
{
    class EquationUiEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            NumberExpression numExp = value as NumberExpression;

            NumberExpression newExp = EquationEditor.GetExpression(numExp);
            if (newExp != null)
                return newExp;
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
