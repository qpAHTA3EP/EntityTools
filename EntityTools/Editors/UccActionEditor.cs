using Astral.Logic.UCC.Classes;
using EntityCore.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class UccActionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (AddUccActionForm.GUIRequest(out UCCAction action))
                return action;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
