using EntityTools.Forms;
using EntityTools.Tools.ItemFilter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace EntityTools.Editors
{
#if DEVELOPER
    public partial class ItemFilterCoreExtEditor<TFilterEntry> where TFilterEntry : IFilterEntry
    { }
    public partial class ItemFilterCoreExtEditor<TFilterEntry> : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is ItemFilterCoreExt<TFilterEntry> filterCore)
            {
                if (ItemFilterEditorForm<TFilterEntry>.GUIRequiest(ref filterCore))
                    return filterCore;
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
