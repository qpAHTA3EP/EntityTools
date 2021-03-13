using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Forms;
using EntityTools.Tools.CustomRegions;

namespace EntityTools.Editors
{
#if DEVELOPER
    class CustomRegionCollectionEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var crCollection = value as CustomRegionCollection;
            if (CustomRegionCollectionEditorForm.GUIRequiest(ref crCollection))
            {
                return crCollection;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
