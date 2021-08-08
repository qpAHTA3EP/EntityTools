using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class TargetEntityTestEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            EntityTools.Core.EntityDiagnosticInfos(context.Instance, true);
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
