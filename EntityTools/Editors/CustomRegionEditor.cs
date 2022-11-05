using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Reflection;
using Astral.Quester.Forms;

namespace EntityTools.Editors
{
    class CustomRegionEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;
        private PropertyAccessor<QuesterProfileProxy> profileProxyAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string customRegion = value?.ToString();
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
                    {
                        var crList = profileProxyAccessor.Value.CustomRegions.Select(cr => cr.Name);
                        if (global::EntityTools.EntityTools.Core.UserRequest_SelectItem(() => crList, ref customRegion))
                        {
                            return customRegion;
                        }
                    }
                }
            }
            else
            {
                customRegion = GetAnId.GetCustomRegion();
                if (!string.IsNullOrEmpty(customRegion))
                    return customRegion;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
	}
}
