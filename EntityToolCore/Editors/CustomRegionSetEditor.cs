using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using EntityCore.Forms;
using EntityTools.Forms;
using EntityTools.Tools.CustomRegions;

namespace EntityCore.Editors
{
#if DEVELOPER
    class CustomRegionSetEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //((PropertyGrid)((System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry)context).OwnerGrid).ParentForm
            if (value is CustomRegionCollection crCollection)
            {
                
                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid
                    && pgAccessor.Value?.ParentForm is QuesterEditor qeForm)
                {
                    crCollection.DebugContext = qeForm.Profile;
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
