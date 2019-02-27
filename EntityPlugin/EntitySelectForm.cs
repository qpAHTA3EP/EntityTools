using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Windows.Forms;

namespace EntityPlugin.Forms
{
    public partial class UIEntitySelectForm : Form
    {
        private static UIEntitySelectForm selectForm;

        public UIEntitySelectForm()
        {
            InitializeComponent();
        }

        public static Entity GetEntity()
        {
            if(selectForm == null)
                selectForm = new UIEntitySelectForm();

            selectForm.dgvEntities.AutoGenerateColumns = false;
            selectForm.dgvEntities.DataSource = EntityManager.GetEntities();

            selectForm.clmnName.DataPropertyName = "Name";
            selectForm.clmnInternalName.DataPropertyName = "InternalName";
            selectForm.clmnNameUntranslated.DataPropertyName = "NameUntranslated";
                       
            DialogResult dialogResult = selectForm.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Entity selectedEntity = selectForm.dgvEntities.CurrentRow.DataBoundItem as Entity;
                if (selectedEntity != null)
                    return selectedEntity;
            }
            return new Entity(IntPtr.Zero);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
