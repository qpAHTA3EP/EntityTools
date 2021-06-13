using AcTp0Tools;
using Astral.Classes.ItemFilter;
using EntityTools.Enums;
using EntityTools.UCC.Actions.TargetSelectors;
using System;
using System.ComponentModel;
using System.Drawing.Design;
namespace EntityTools.Editors
{
#if DEVELOPER
    internal class UccTargetSelectorEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Type selectorType = null; 
            if (EntityTools.Core.UserRequest_SelectItem(() => AstralAccessors.Controllers.Plugins.UccTargetSelectors, ref selectorType, "Name") &&
                selectorType != null)
            {
                if (selectorType == typeof(EntityTarget))
                {
                    var strPattern = value is null ? string.Empty : value.ToString();
                    EntityNameType nameType = EntityNameType.NameUntranslated;
                    ItemFilterStringType strFilterType = ItemFilterStringType.Simple;

                    if (EntityTools.Core.UserRequest_EditEntityId(ref strPattern, ref strFilterType, ref nameType))
                    {
                        return new EntityTarget
                        {
                            _entityId = strPattern,
                            _entityIdType = strFilterType,
                            _entityNameType = nameType
                        };
                    }
                }
                else
                {
                    try
                    {
                        var selector = Activator.CreateInstance(selectorType);
                        return selector;
                    }
                    catch (Exception e)
                    {
                        ETLogger.WriteLine(LogType.Error, e.ToString());
                        throw;
                    } 
                }
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
