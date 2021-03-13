using Astral.Classes.ItemFilter;
using EntityTools.Editors.Forms;
using EntityTools.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class FoeListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is List<string> originalItems)
            {
                IList<string> items = new List<string>(originalItems);

                string id = string.Empty;
                ItemFilterStringType strType = ItemFilterStringType.Simple;
                var nameType = EntityNameType.InternalName;
                Func<string> getNewItem = () => {
                    if (EntityTools.Core.GUIRequest_EntityId(ref id, ref strType, ref nameType))
                        return id;
                    return null;
                };
                if (ItemListEditorForm<string>.GUIRequest(ref items, getNewItem, "Foe list"))
                    return new List<string>(items);
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

