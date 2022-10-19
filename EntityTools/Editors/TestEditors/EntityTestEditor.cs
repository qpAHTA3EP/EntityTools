using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class EntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string msg = EntityTools.Core.EntityDiagnosticInfos(context?.Instance);
            if (!string.IsNullOrEmpty(msg))
                XtraMessageBox.Show(msg, $"Test of '{value}'"); 
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
