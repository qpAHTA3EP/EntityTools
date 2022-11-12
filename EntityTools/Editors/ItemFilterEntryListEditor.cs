using EntityTools.Forms;
using EntityTools.Tools.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    internal class ItemFilterEntryListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is List<ItemFilterEntryExt> collection
                && ItemFilterEditorForm.GUIRequiest(ref collection))
                return new List<ItemFilterEntryExt>(collection);

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
