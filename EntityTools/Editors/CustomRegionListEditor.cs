using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Extensions;

namespace EntityTools.Editors
{
#if DEVELOPER
    class CustomRegionListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            List<string> list = value as List<string>;
            if (EntityTools.Core.UserRequest_EditCustomRegionList(ref list))
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
