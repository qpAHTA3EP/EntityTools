using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using EntityTools.Forms;
using EntityTools.Tools.CustomRegions;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Editors
{
#if DEVELOPER
    internal class CustomRegionCollectionEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is CustomRegionCollection crCollection)
            {

                if (pgAccessor is null)
                    pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

                if (pgAccessor.IsValid)
                {
                    if (pgAccessor.Value?.ParentForm is QuesterEditor parentForm)
                    {
                        crCollection.DesignContext = parentForm.Profile;
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
