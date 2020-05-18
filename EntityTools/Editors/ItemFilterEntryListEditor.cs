using EntityTools.Forms;
using EntityTools.Tools.BuySellItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class ItemFilterEntryListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is List<ItemFilterEntryExt> list)
            {
                if (ItemFilterEditorForm.GUIRequiest(ref list))
                    return new List<ItemFilterEntryExt>(list);
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
