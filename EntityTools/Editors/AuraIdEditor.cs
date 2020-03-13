using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    public class AuraIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string auraId = AuraSelectForm.GetAuraId();
            if (!string.IsNullOrEmpty(auraId))
                return auraId;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
