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
using EntityTools.UCC.Conditions;
using EntityTools.Enums;

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
                if (context.Instance is MoveToEntity moveToEntity) 
                    nameType = moveToEntity.EntityNameType;
                else if (context.Instance is InteractEntities interactEntities)
                    nameType = interactEntities.EntityNameType;
                else if (context.Instance is EntityProperty entityProperty)
                    nameType = entityProperty.EntityNameType;
                else if (context.Instance is EntityCount entityCountInCustomRegions)
                    nameType = entityCountInCustomRegions.EntityNameType;
                //else if (context.Instance is AbortCombatEntity abortCombatEntity)
                //    nameType = abortCombatEntity.EntityNameType;
                else if (context.Instance is DodgeFromEntity dodgeFromEntity)
                    nameType = dodgeFromEntity.EntityNameType;
                else if (context.Instance is UCCEntityCheck uccConditionEntityCheck) 
                    nameType = uccConditionEntityCheck.EntityNameType;
                else if(ReflectionHelper.GetPropertyValue(context.Instance, "EntityNameType", out object entityNameTypeObj)
                        && entityNameTypeObj is EntityNameType entityNameType)
                           nameType = entityNameType;
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
