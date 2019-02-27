using System;
using System.Windows.Forms;
using MyNW.Classes;

namespace EntityPlugin.Forms
{
    public partial class MainPanel : Astral.Forms.BasePanel
    {
        public MainPanel() : base("EntityPlugin")
        {
            InitializeComponent();
        }

        //public void listBoxControl_MouseMove(object sender, MouseEventArgs e)
        //{
        //    DevExpress.XtraEditors.ListBoxControl listitems = sender as DevExpress.XtraEditors.ListBoxControl;
        //    if (listitems != null)
        //    {
        //        int index = listitems.IndexFromPoint(e.Location);

        //        if (index > listitems.Items.Count)
        //        {
        //            listitems.ToolTip = string.Empty;
        //            return;
        //        }

        //        //Список Items оказывается пустым, т.к. данные берутся из DataSource
        //        Entity selectedEntity = listitems.Items[index] as Entity;

        //        string tip = String.Empty;

        //        if (selectedEntity!=null && selectedEntity.IsValid)
        //            tip = $"Name = [{selectedEntity.Name}]" + Environment.NewLine +
        //                    $"InternalName = [{selectedEntity.InternalName}]" + Environment.NewLine +
        //                    $"NameUntranslated = [{selectedEntity.NameUntranslated}]";

        //        listitems.ToolTip = tip;
        //    }
        //}
        //public void listBoxControl_toolTipBeforShow(object sender, ToolTipControllerShowEventArgs e)
        //{
        //    Entity selectedEntity = e.SelectedObject as Entity;
        //    string tip = string.Empty;

        //    if (selectedEntity != null && selectedEntity.IsValid)
        //        tip = $"{selectedEntity.InternalName} [{selectedEntity.NameUntranslated}]";

        //    e.ToolTip = tip;
        //}

        private void btnTest_Click(object sender, EventArgs e)
        {
            Entity selectedEntity = UIEntitySelectForm.GetEntity(); 

            if (selectedEntity != null && selectedEntity.IsValid)
                MessageBox.Show($"Selected Entity is '{selectedEntity.Name}'" + Environment.NewLine +
                        $"InternalName = [{selectedEntity.InternalName}]" + Environment.NewLine +
                        $"NameUntranslated = [{selectedEntity.NameUntranslated}]");
            else MessageBox.Show("Entity is not valid at the moment!");

            //Entity entity = new Entity(IntPtr.Zero);

            //Astral.Quester.UIEditors.Forms.SelectList listEditor = new Astral.Quester.UIEditors.Forms.SelectList();
            //listEditor.MinimumSize = new System.Drawing.Size(1000, 500);

            //listEditor.listitems.DataSource = EntityManager.GetEntities();
            //listEditor.listitems.DisplayMember = "Name";
            //listEditor.listitems.ValueMember = "NameUntranslated";
            /////Этот вариант вызывает обработчик listBoxControl_MouseMove
            ////listEditor.listitems.MouseMove += listBoxControl_MouseMove; 

            ////ToolTipController toolTipController = new ToolTipController();
            ////toolTipController.BeforeShow += listBoxControl_toolTipBeforShow;
            ////listEditor.listitems.ToolTipController = toolTipController;

            //DialogResult dialogResult = listEditor.ShowDialog();
            //if (dialogResult == DialogResult.OK)
            //{
            //    if (listEditor.listitems.SelectedItem != null)
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
