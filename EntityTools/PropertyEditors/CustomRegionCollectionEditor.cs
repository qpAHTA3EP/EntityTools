using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using EntityTools.Forms;
using EntityTools.Tools.CustomRegions;

namespace EntityTools.PropertyEditors
{
#if DEVELOPER
    class CustomRegionCollectionEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;
        private PropertyAccessor<QuesterProfileProxy> profileProxyAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is CustomRegionCollection crCollection)
            {

                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid)
                {
                    var parentForm = pgAccessor.Value?.ParentForm;
                    if (parentForm != null)
                    {
                        if (profileProxyAccessor is null)
                            profileProxyAccessor = parentForm.GetProperty<QuesterProfileProxy>("Profile");
                        if (profileProxyAccessor.IsValid)
                            crCollection.DebugContext = profileProxyAccessor.Value;
                    }
                }

                if (CustomRegionCollectionEditorForm.RequestUser(ref crCollection))
                {
                    return crCollection;
                }
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
