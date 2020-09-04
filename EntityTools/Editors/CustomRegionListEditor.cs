using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Classes;
using DevExpress.XtraEditors.Controls;
using EntityTools.Extensions;
using MyNW.Classes;


namespace EntityTools.Editors
{
#if DEVELOPER
    class CustomRegionListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            List<string> list = value as List<string>;
            if (EntityTools.Core.GUIRequest_CustomRegions(ref list))
            {
                return list.Clone();
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
