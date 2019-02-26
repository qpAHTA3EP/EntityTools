using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    public partial class UIEntitySelectForm : Form
    {
        public UIEntitySelectForm()
        {
            InitializeComponent();
        }

        public static void GetEntity()
        {
            UIEntitySelectForm selectForm = new UIEntitySelectForm();

            selectForm.dgwEntities.DataSource = EntityManager.GetEntities();

            selectForm.clmnName.DataPropertyName = "Name";
            selectForm.clmnInternalName.DataPropertyName = "InternalName";
            selectForm.clmnNameUntranslated.DataPropertyName = "NameUntranslated";
                        
            DialogResult dialogResult = selectForm.ShowDialog();
            //if (dialogResult == DialogResult.OK)
            //{
            //    if (selectForm.dgwEntities.SelectedRows. != null)
            //    {
            //        Entity selectedEntity = listEditor.listitems.SelectedItem as Entity;
            //        if (selectedEntity != null && selectedEntity.IsValid)
            //            MessageBox.Show($"Selected Entity is '{selectedEntity.Name}'" + Environment.NewLine +
            //                $"InternalName = [{selectedEntity.InternalName}]" + Environment.NewLine +
            //                $"NameUntranslated = [{selectedEntity.NameUntranslated}]");
            //        else MessageBox.Show("Entity is not valid at the moment!");
            //    }

            //}
        }
    }
}
