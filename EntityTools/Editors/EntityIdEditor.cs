using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Conditions;
using EntityTools.Reflection;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string strPattern = string.Empty;
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
                {
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
            }

            if (value != null)
                strPattern = value.ToString();

            if (EntityTools.Core.GUIRequest_EntityId(ref strPattern, ref strFilterType, ref nameType))
            {
                // сохраняем новые значени опций команды
                if (context.Instance is MoveToEntity moveToEntity)
                {
                    if(nameType != moveToEntity.EntityNameType)
                        moveToEntity.EntityNameType = nameType;
                    if(strFilterType != moveToEntity.EntityIdType)
                        moveToEntity.EntityIdType = strFilterType;
                }
                else if (context.Instance is InteractEntities interactEntities)
                {
                    if(nameType != interactEntities.EntityNameType)
                        interactEntities.EntityNameType = nameType;
                    if (strFilterType != interactEntities.EntityIdType)
                        interactEntities.EntityIdType = strFilterType;
                }
                else if (context.Instance is EntityProperty entityProperty)
                {
                    if(nameType != entityProperty.EntityNameType)
                        entityProperty.EntityNameType = nameType;
                    if(strFilterType != entityProperty.EntityIdType)
                        entityProperty.EntityIdType = strFilterType;
                }
                else if (context.Instance is EntityCount entityCountInCustomRegions)
                {
                    if(nameType != entityCountInCustomRegions.EntityNameType)
                        entityCountInCustomRegions.EntityNameType = nameType;
                    if(strFilterType != entityCountInCustomRegions.EntityIdType)
                        entityCountInCustomRegions.EntityIdType = strFilterType;
                }
                else if (context.Instance is DodgeFromEntity dodgeFromEntity)
                {
                    if(nameType != dodgeFromEntity.EntityNameType)
                        dodgeFromEntity.EntityNameType = nameType;
                    if(strFilterType != dodgeFromEntity.EntityIdType)
                        dodgeFromEntity.EntityIdType = strFilterType;
                }
                else if (context.Instance is UCCEntityCheck uccConditionEntityCheck)
                {
                    if(nameType != uccConditionEntityCheck.EntityNameType)
                        uccConditionEntityCheck.EntityNameType = nameType;
                    if(strFilterType != uccConditionEntityCheck.EntityIdType)
                        uccConditionEntityCheck.EntityIdType = strFilterType;
                }

                return strPattern;
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
