using System;
using System.ComponentModel;
using System.Drawing.Design;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.Quester.Actions;
using EntityTools.Quester.Conditions;
using AcTp0Tools.Reflection;
using EntityTools.Core.Interfaces;
using EntityTools.UCC.Actions;
using EntityTools.UCC.Conditions;

namespace EntityTools.Editors
{
#if DEVELOPER
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var strPattern = value is null ? string.Empty : value.ToString();
            EntityNameType nameType = EntityNameType.NameUntranslated;
            ItemFilterStringType strFilterType = ItemFilterStringType.Simple;

            var instance = context?.Instance;

#if true
            if (instance is IEntityIdentifier ettId)
            {
                nameType = ettId.EntityNameType;
                strFilterType = ettId.EntityIdType;
                if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                {
                    if (nameType != ettId.EntityNameType)
                        ettId.EntityNameType = nameType;
                    if (strFilterType != ettId.EntityIdType)
                        ettId.EntityIdType = strFilterType;
                    return strPattern;
                }
            }
            else if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                return strPattern;

#else
            if (instance is null)
                return value;

            string strPattern = value is null ? string.Empty : value.ToString();

            switch (instance)
            {
                case IEntityIdentifier ettId:
                    nameType = ettId.EntityNameType;
                    strFilterType = ettId.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != ettId.EntityNameType)
                            ettId.EntityNameType = nameType;
                        if (strFilterType != ettId.EntityIdType)
                            ettId.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case MoveToEntity moveToEntity:
                    nameType = moveToEntity.EntityNameType;
                    strFilterType = moveToEntity.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != moveToEntity.EntityNameType)
                            moveToEntity.EntityNameType = nameType;
                        if (strFilterType != moveToEntity.EntityIdType)
                            moveToEntity.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case InteractEntities interactEntities:
                    nameType = interactEntities.EntityNameType;
                    strFilterType = interactEntities.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != interactEntities.EntityNameType)
                            interactEntities.EntityNameType = nameType;
                        if (strFilterType != interactEntities.EntityIdType)
                            interactEntities.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case EntityProperty entityProperty:
                    nameType = entityProperty.EntityNameType;
                    strFilterType = entityProperty.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != entityProperty.EntityNameType)
                            entityProperty.EntityNameType = nameType;
                        if (strFilterType != entityProperty.EntityIdType)
                            entityProperty.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case EntityCount entityCountInCustomRegions:
                    nameType = entityCountInCustomRegions.EntityNameType;
                    strFilterType = entityCountInCustomRegions.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != entityCountInCustomRegions.EntityNameType)
                            entityCountInCustomRegions.EntityNameType = nameType;
                        if (strFilterType != entityCountInCustomRegions.EntityIdType)
                            entityCountInCustomRegions.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case DodgeFromEntity dodgeFromEntity:
                    nameType = dodgeFromEntity.EntityNameType;
                    strFilterType = dodgeFromEntity.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != dodgeFromEntity.EntityNameType)
                            dodgeFromEntity.EntityNameType = nameType;
                        if (strFilterType != dodgeFromEntity.EntityIdType)
                            dodgeFromEntity.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                case UCCEntityCheck uccConditionEntityCheck:
                    nameType = uccConditionEntityCheck.EntityNameType;
                    strFilterType = uccConditionEntityCheck.EntityIdType;
                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        if (nameType != uccConditionEntityCheck.EntityNameType)
                            uccConditionEntityCheck.EntityNameType = nameType;
                        if (strFilterType != uccConditionEntityCheck.EntityIdType)
                            uccConditionEntityCheck.EntityIdType = strFilterType;
                        return strPattern;
                    }
                    break;
                default:
                    {
                        if (ReflectionHelper.GetPropertyValue(instance, "EntityNameType", out object entityNameTypeObj)
                            && entityNameTypeObj is EntityNameType entityNameType)
                        {
                            nameType = entityNameType;
                        }
                        if (ReflectionHelper.GetPropertyValue(instance, "EntityIdType", out object itemFilterStringTypeObj)
                            && itemFilterStringTypeObj is ItemFilterStringType itemFilterStringType)
                        {
                            strFilterType = itemFilterStringType;
                        }

                        if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                        {

                            return strPattern;
                        }
                        break;
                    }
            } 
#endif

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
#endif
}
