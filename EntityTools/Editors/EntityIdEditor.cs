using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Actions;
using EntityTools.Forms;
using EntityTools.Tools;
using EntityTools.Conditions;
using static EntityTools.Forms.EntitySelectForm;
using EntityTools.UCC;
using EntityTools.Actions.Deprecated;

namespace EntityTools.Editors
{
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string oldVal = string.Empty;
            EntityNameType nameType = EntityNameType.NameUntranslated;

            if (context.Instance != null)
            {
                if (context.Instance is MoveToEntity)
                    nameType = ((MoveToEntity)context.Instance).EntityNameType;
                else if (context.Instance is InteractEntities)
                    nameType = ((InteractEntities)context.Instance).EntityNameType;
                else if (context.Instance is EntityProperty)
                    nameType = ((EntityProperty)context.Instance).EntityNameType;
                else if (context.Instance is EntityProperty)
                    nameType = ((EntityProperty)context.Instance).EntityNameType;
                else if (context.Instance is EntityCountInCustomRegions)
                    nameType = ((EntityCountInCustomRegions)context.Instance).EntityNameType;
                else if (context.Instance is AbortCombatEntity)
                    nameType = ((AbortCombatEntity)context.Instance).EntityNameType;
                //else if (context.Instance is InteractNPCext)
                //    nameType = ((InteractNPCext)context.Instance).NameType;                
            }

            if (value != null)
                oldVal = value.ToString();

            EntityDif selectedEntityDif = EntitySelectForm.GetEntity(oldVal);

            if (selectedEntityDif != null && selectedEntityDif.IsValid)
            {
                switch(nameType)
                {
                    case EntityNameType.InternalName:
                        return selectedEntityDif.InternalName;
                    case EntityNameType.NameUntranslated:
                        return selectedEntityDif.NameUntranslated;
                }                
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
