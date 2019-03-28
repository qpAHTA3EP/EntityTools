using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Astral.Quester.Classes;
using DevExpress.XtraEditors.Controls;
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

            // Настройка окна для отображения списка CustomRegion
            listEditor.Text = "CustomRegionSelect";
            listEditor.listitems.DataSource = null;//= Astral.Quester.API.CurrentProfile.CustomRegions;
            listEditor.listitems.DisplayMember = string.Empty;//"Name";
            listEditor.listitems.ToolTip = "Hold 'Ctrl' or 'Shift' key to select several CustomRegions";

            List<string> regions = value as List<string>;

            // Заполнение списка регионов
            listEditor.listitems.Items.Clear();
            foreach(CustomRegion cr in Astral.Quester.API.CurrentProfile.CustomRegions)
            {
                int ind = listEditor.listitems.Items.Add(cr.Name);
                if(regions!= null && regions.Contains(cr.Name))
                    listEditor.listitems.SetSelected(ind, true);
                else listEditor.listitems.SetSelected(ind, false);
            }

            // Отображение списка CustomRegion пользователю
            DialogResult dialogResult = listEditor.ShowDialog();

            if (dialogResult == DialogResult.OK && listEditor.listitems.SelectedItems.Count > 0)
            {
                // Формирование списка выбранных CustomRegion
                regions.Clear();
                foreach (object item in listEditor.listitems.SelectedItems)
                {
                    // Формирование списка с привязкой данных 
                    //CustomRegion cr = item as CustomRegion;
                    //if (cr != null)
                    //    regions.Add(cr.Name);

                    // Формирование списка без привязкой данных 
                    if (item is string crName)
                    {
                        regions.Add(crName);
                    }
                }
                return regions;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
