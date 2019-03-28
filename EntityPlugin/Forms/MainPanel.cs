#define Test_EntitySelectForm

using System;
using System.Text;
using System.Windows.Forms;
using Astral.Quester.Classes;
using MyNW.Classes;
using MyNW.Internals;
using static DevExpress.XtraEditors.BaseListBoxControl;

namespace EntityPlugin.Forms
{
    public partial class MainPanel : Astral.Forms.BasePanel
    {
        private EntitySelectForm.EntityDif entDif = new EntitySelectForm.EntityDif();

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

        private void btnEntities_Click(object sender, EventArgs e)
        {
#region Test_MultiSelectCustomRegion
#if Test_MultiSelectCustomRegion
            //Entity entity = new Entity(IntPtr.Zero);

            Astral.Quester.UIEditors.Forms.SelectList listEditor = new Astral.Quester.UIEditors.Forms.SelectList();
            //listEditor.MinimumSize = new System.Drawing.Size(1000, 500);

            listEditor.Text = "CustomRegionSelect";
            listEditor.listitems.DataSource = Astral.Quester.API.CurrentProfile.CustomRegions;
            listEditor.listitems.DisplayMember = "Name";
            listEditor.listitems.ToolTip = "Press Ctrl+LMB to select several CustomRegions";

            // Этот вариант вызывает обработчик listBoxControl_MouseMove
            //listEditor.listitems.MouseMove += listBoxControl_MouseMove;

            //ToolTipController toolTipController = new ToolTipController();
            //toolTipController.BeforeShow += listBoxControl_toolTipBeforShow;
            //listEditor.listitems.ToolTipController = toolTipController;

            DialogResult dialogResult = listEditor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                StringBuilder strBldr = new StringBuilder();

                if (listEditor.listitems.SelectedItems.Count > 0)
                {
                    strBldr.AppendLine("Selected CustomRegions are:");

                    foreach (CustomRegion item in listEditor.listitems.SelectedItems)
                    {
                        CustomRegion cr = item as CustomRegion;
                        if (cr != null)
                            strBldr.AppendLine(cr.Name);
                        else strBldr.AppendLine($"Selected object [{item.ToString()}] can not be cast to CustomRegion");
                    }
                }

                if (strBldr.Length == 0)
                    strBldr.AppendLine("No one CustomRegion was selected");

                MessageBox.Show(strBldr.ToString());
            }
#endif
#endregion

#region Test_EntitySelectForm
#if Test_EntitySelectForm
            entDif = EntitySelectForm.GetEntity(entDif.NameUntranslated);

            MessageBox.Show($"Selected Entity:\n" +
                $"Name: {entDif.Name}\n" +
                $"InternalName: {entDif.InternalName}\n" +
                $"NameUntranslated: {entDif.NameUntranslated}");

#endif
#endregion
        }
    }
}
