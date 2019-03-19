using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityPlugin.Forms;
using MyNW.Classes;

namespace EntityPlugin.Editors
{
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Entity selectedEntity = UIEntitySelectForm.GetEntity();

            if (selectedEntity != null && selectedEntity.IsValid)
            {
                return selectedEntity.NameUntranslated;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
