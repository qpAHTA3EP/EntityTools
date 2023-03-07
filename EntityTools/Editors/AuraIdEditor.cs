using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Forms;

namespace EntityTools.Editors
{
    internal class AuraIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string newId = AuraViewer.GUIRequest();
            if (!string.IsNullOrEmpty(newId))
                return newId;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
