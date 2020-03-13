using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Conditions;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;
using EntityTools.Enums;
using EntityTools.Reflection;
using EntityTools.Tools;
using Astral.Classes.ItemFilter;

namespace EntityTools.Editors
{
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string oldVal = string.Empty;
            EntityNameType nameType = EntityNameType.NameUntranslated;
            ItemFilterStringType strFilterType = ItemFilterStringType.Simple;

            if (context.Instance != null)
            {
                if (context.Instance is MoveToEntity moveToEntity)
                {
                    nameType = moveToEntity.EntityNameType;
                    strFilterType = moveToEntity.EntityIdType;
                }
                else if (context.Instance is InteractEntities interactEntities)
                {
                    nameType = interactEntities.EntityNameType;
                    strFilterType = interactEntities.EntityIdType;
                }
                else if (context.Instance is EntityProperty entityProperty)
                {
                    nameType = entityProperty.EntityNameType;
                    strFilterType = entityProperty.EntityIdType;
                }
                else if (context.Instance is EntityCount entityCountInCustomRegions)
                {
                    nameType = entityCountInCustomRegions.EntityNameType;
                    strFilterType = entityCountInCustomRegions.EntityIdType;
                }
                //else if (context.Instance is AbortCombatEntity abortCombatEntity)
                //    nameType = abortCombatEntity.EntityNameType;
                else if (context.Instance is DodgeFromEntity dodgeFromEntity)
                {
                    nameType = dodgeFromEntity.EntityNameType;
                    strFilterType = dodgeFromEntity.EntityIdType;
                }
                else if (context.Instance is UCCEntityCheck uccConditionEntityCheck)
                {
                    nameType = uccConditionEntityCheck.EntityNameType;
                    strFilterType = uccConditionEntityCheck.EntityIdType;
                }
                else 
                if (ReflectionHelper.GetPropertyValue(context.Instance, "EntityNameType", out object entityNameTypeObj)
                    && entityNameTypeObj is EntityNameType entityNameType)
                {
                    nameType = entityNameType;
                }
                if (ReflectionHelper.GetPropertyValue(context.Instance, "EntityIdType", out object itemFilterStringTypeObj)
                    && itemFilterStringTypeObj is ItemFilterStringType itemFilterStringType)
                {
                    strFilterType = itemFilterStringType;
                }
            }

            if (value != null)
                oldVal = value.ToString();

            EntityDef selectedEntityDef = EntityTools.Core.GUIRequest_EntityId(oldVal, strFilterType, nameType);

            if (selectedEntityDef != null && selectedEntityDef.IsValid)
            {
                switch(nameType)
                {
                    case EntityNameType.InternalName:
                        return selectedEntityDef.InternalName;
                    case EntityNameType.NameUntranslated:
                        return selectedEntityDef.NameUntranslated;
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
