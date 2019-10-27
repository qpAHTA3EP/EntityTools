using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntityTools.Forms;

namespace EntityTools.Editors
{
    public class UiIdEditor : UITypeEditor
    {
        private static UIViewer uiViewer;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (uiViewer == null)
                uiViewer = new UIViewer();

            string newValue = uiViewer.GetUiGenId(value as string);

            if (!string.IsNullOrEmpty(newValue))
                return newValue;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}

