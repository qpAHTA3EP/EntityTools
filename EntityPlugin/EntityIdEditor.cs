using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Forms;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityPlugin.Editors
{
    public class EntityIdEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Astral.Quester.UIEditors.Forms.SelectList listEditor = new Astral.Quester.UIEditors.Forms.SelectList();
            listEditor.MinimumSize = new System.Drawing.Size(1000, 500);

            listEditor.listitems.DataSource = EntityManager.GetEntities();
            listEditor.listitems.DisplayMember = "Name";
            listEditor.listitems.ValueMember = "NameUntranslated";
            

            DialogResult dialogResult = listEditor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (listEditor.listitems.SelectedItem != null)
                {
                    return listEditor.listitems.SelectedValue;
                    //Entity selectedEntity = listEditor.listitems.SelectedItem as Entity;
                    //if (selectedEntity != null && selectedEntity.IsValid)
                    //    return selectedEntity.NameUntranslated;
                }
            }
            return string.Empty;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
