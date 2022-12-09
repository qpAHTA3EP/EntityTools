using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using ACTP0Tools.Reflection;
using Astral.Quester.Forms;
using EntityTools.Forms;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;

namespace EntityTools.Editors
{
    internal class CustomRegionEditor : UITypeEditor
    {
        private PropertyAccessor<PropertyGrid> pgAccessor;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string customRegion = value?.ToString();
            if (pgAccessor is null)
                pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

            if (pgAccessor.IsValid)
            {
                if (pgAccessor.Value?.ParentForm is QuesterEditor questerEditor)
                {
                    var crList = questerEditor.Profile.CustomRegions.Select(cr => cr.Name);
                    if (ItemSelectForm.GetAnItem(() => crList, ref customRegion))
                    {
                        return customRegion;
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
