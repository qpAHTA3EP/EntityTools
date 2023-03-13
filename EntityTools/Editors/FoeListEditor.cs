using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Forms;

namespace EntityTools.Editors
{
    internal class FoeListEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is List<string> originalItems)
            {
                IList<string> items = new List<string>(originalItems);

                string id = string.Empty;
                ItemFilterStringType strType = ItemFilterStringType.Simple;
                var nameType = EntityNameType.InternalName;

                string GetEntityId()
                {
                    return EntityViewer.GUIRequest(ref id, ref strType, ref nameType) != null 
                        ? id 
                        : null;
                }

                if (ItemListEditorForm<string>.GUIRequest(ref items, GetEntityId, "Foe list"))
                    return new List<string>(items);
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}

