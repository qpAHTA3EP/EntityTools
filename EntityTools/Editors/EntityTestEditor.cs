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
            EntityTools.Core.EntityDiagnosticInfos(context.Instance);
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
