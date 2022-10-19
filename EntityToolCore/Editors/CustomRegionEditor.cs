using ACTP0Tools.Reflection;
using Astral.Quester.Forms;
using EntityCore.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace EntityCore.Editors
{
    class CustomRegionEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string customRegion = value?.ToString();

            if (pgAccessor is null)
                pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

            if (pgAccessor.IsValid
                && pgAccessor.Value?.ParentForm is QuesterEditor qeForm)
            {
                if (ItemSelectForm.GetAnItem(() => qeForm.Profile.CustomRegions.Select(cr => cr.Name), ref customRegion))
                {
                    return customRegion;
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
