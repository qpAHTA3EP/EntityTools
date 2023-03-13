using EntityTools.Forms;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    public class UiIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string newValue = UIViewer.GUIRequest(value as string);

            return string.IsNullOrEmpty(newValue) 
                 ? value
                 : newValue;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}

