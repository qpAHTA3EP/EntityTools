using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Classes;
using EntityPlugin.Forms;
using MyNW.Classes;


namespace EntityPlugin.Editors
{
    class MultiCustomRegionSelectEditor : UITypeEditor
    {
        internal static Astral.Quester.UIEditors.Forms.SelectList listEditor = new Astral.Quester.UIEditors.Forms.SelectList();

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (listEditor == null)
                listEditor = new Astral.Quester.UIEditors.Forms.SelectList();

            listEditor.Text = "CustomRegionSelect";
            listEditor.listitems.DataSource = Astral.Quester.API.CurrentProfile.CustomRegions;
            listEditor.listitems.DisplayMember = "Name";
            listEditor.listitems.ToolTip = "Hold 'Ctrl' or 'Shift' key to select several CustomRegions";            

            DialogResult dialogResult = listEditor.ShowDialog();
            List<string> regions = new List<string>();

            if (dialogResult == DialogResult.OK && listEditor.listitems.SelectedItems.Count > 0)
            {
                foreach(object item in listEditor.listitems.SelectedItems)
                {
                    CustomRegion cr = item as CustomRegion;
                    if (cr != null)
                        regions.Add(cr.Name);
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
