using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Logic.UCC.Classes;
using EntityTools.UCC.Extensions;

namespace EntityTools.Editors
{
    public class UccActionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (UCCEditorExtensions.GetUccAction(out UCCAction uccAction))
                return uccAction;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
