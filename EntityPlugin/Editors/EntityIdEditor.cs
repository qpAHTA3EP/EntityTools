using System;
using System.ComponentModel;
using System.Drawing.Design;
using EntityTools.Forms;
using MyNW.Classes;
using static EntityTools.Forms.EntitySelectForm;

namespace EntityTools.Editors
{
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string oldVal = string.Empty;

            if (value != null)
                oldVal = value.ToString();

            EntityDif selectedEntityDif = EntitySelectForm.GetEntity(oldVal);

            if (selectedEntityDif != null)
            {
                return selectedEntityDif.NameUntranslated;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
