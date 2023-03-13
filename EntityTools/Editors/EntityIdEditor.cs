using Astral.Classes.ItemFilter;
using EntityTools.Forms;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace EntityTools.Editors
{
    internal class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var strPattern = value is null ? string.Empty : value.ToString();
            EntityNameType nameType = EntityNameType.NameUntranslated;
            ItemFilterStringType strFilterType = ItemFilterStringType.Simple;

            var instance = context?.Instance;

            if (instance is IEntityIdentifier entityId)
            {
                nameType = entityId.EntityNameType;
                strFilterType = entityId.EntityIdType;
                if (EntityViewer.GUIRequest(ref strPattern, ref strFilterType, ref nameType) != null)
                {
                    if (nameType != entityId.EntityNameType)
                        entityId.EntityNameType = nameType;
                    if (strFilterType != entityId.EntityIdType)
                        entityId.EntityIdType = strFilterType;
                    return strPattern;
                }
            }
            else if (EntityViewer.GUIRequest(ref strPattern, ref strFilterType, ref nameType) != null)
                return strPattern;

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
