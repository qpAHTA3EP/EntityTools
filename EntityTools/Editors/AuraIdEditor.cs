using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class AuraIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string auraId = value.ToString();
            if (EntityTools.Core.GUIRequest_AuraId(ref auraId) &&
                !string.IsNullOrEmpty(auraId))
                return auraId;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
