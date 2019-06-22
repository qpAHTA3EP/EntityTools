#define Test_EntitySelectForm

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using EntityPlugin.Tools;
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
            if (entDif != null)
                MessageBox.Show($"Selected Entity:\n" +
                    $"Name: {entDif.Name}\n" +
                    $"InternalName: {entDif.InternalName}\n" +
                    $"NameUntranslated: {entDif.NameUntranslated}");
            else MessageBox.Show("No Entity was selected");

#endif
#endregion
        }

        private void ckbDebugInfo_CheckedChanged(object sender, EventArgs e)
        {
            EntityPlugin.DebugInfoEnabled = ckbDebugInfo.Checked;
        }

        private void MainPanel_Load(object sender, EventArgs e)
        {
            ckbDebugInfo.Checked = EntityPlugin.DebugInfoEnabled;
        }

        private void btnAuras_Click(object sender, EventArgs e)
        {
            AurasWrapper auras = new AurasWrapper(EntityManager.LocalPlayer?.Character);

            XmlSerializer serialiser = new XmlSerializer(typeof(AurasWrapper));
            TextWriter FileStream = new StreamWriter(@".\Auras.xml");
            serialiser.Serialize(FileStream, auras);
            FileStream.Close();
        }

        private void bntMissions_Click(object sender, EventArgs e)
        {
            MissionsWrapper missions = new MissionsWrapper(EntityManager.LocalPlayer);

            XmlSerializer serialiser = new XmlSerializer(typeof(MissionsWrapper));
            TextWriter FileStream = new StreamWriter(@".\Missions.xml");
            serialiser.Serialize(FileStream, missions);
            FileStream.Close();
        }
    }
}
