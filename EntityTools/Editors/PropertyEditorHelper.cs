using EntityTools.Quester.Editor;
using Infrastructure;
using Infrastructure.Quester;
using Infrastructure.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace EntityTools.Editors
{
    internal static class PropertyEditorHelper
    {
        private static PropertyAccessor<PropertyGrid> pgAccessor;
        internal static BaseQuesterProfileProxy GetQuesterProfile(this ITypeDescriptorContext context)
        {
            if (pgAccessor is null)
                pgAccessor = context.GetProperty<PropertyGrid>("OwnerGrid");

            if (pgAccessor.IsValid)
            {
                if (pgAccessor.Value?.ParentForm is QuesterEditor questerEditor)
                {
                    return questerEditor.Profile;
                }
            }

            return AstralAccessors.Quester.Core.CurrentProfile;
        }
    }
}
