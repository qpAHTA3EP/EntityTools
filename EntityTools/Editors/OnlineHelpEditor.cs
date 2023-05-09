using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    public class OnlineHelpEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.PropertyDescriptor?.Attributes[typeof(OnlineHelpAttribute)] is OnlineHelpAttribute attribute
                && !string.IsNullOrWhiteSpace(attribute.Url))
            {
                Process.Start(attribute.Url); ;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }

    public class OnlineHelpAttribute : Attribute
    {
        public OnlineHelpAttribute(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}
