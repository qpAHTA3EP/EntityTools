using EntityTools.Forms;
using EntityTools.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    class ParagonSelectEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            SelectedParagons paragons = PlayerParagonSelectForm.GetParagons(value as SelectedParagons);

            if (paragons != null)
                return paragons;
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}