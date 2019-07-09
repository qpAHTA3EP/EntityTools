//#define Test_EntitySelectForm

using System;
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

            bteMissions.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderMissions, FileTools.defaulFileMissions);
            bteAuras.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderAuras, FileTools.defaulFileAuras);
        }

        private void bte_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DevExpress.XtraEditors.ButtonEdit bte = sender as DevExpress.XtraEditors.ButtonEdit;
            if (bte != null)
            {
                string fileName = string.Empty;

                if (string.IsNullOrEmpty(bte.Text) || (bte.Text.IndexOfAny(Path.GetInvalidPathChars()) != -1))
                {

                    if (bte.Name == bteAuras.Name) fldrBroserDlg.SelectedPath = FileTools.defaulExportFolderAuras;
                    if (bte.Name == bteMissions.Name) fldrBroserDlg.SelectedPath = FileTools.defaulExportFolderMissions;
                }
                else
                {
                    fldrBroserDlg.SelectedPath = Path.GetDirectoryName(bte.Text);
                    fileName = Path.GetFileName(bte.Text);
                }

                if (fldrBroserDlg.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        if (bte.Name == bteAuras.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaulFileAuras);
                        if (bte.Name == bteMissions.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaulFileMissions);
                    }
                    else bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, fileName);
                }
            }
        }

        private void btnAuras_Click(object sender, EventArgs e)
        {
            AurasWrapper auras = new AurasWrapper(EntityManager.LocalPlayer?.Character);

            string fullFileName = FileTools.ReplaceMask(bteAuras.Text);

            if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                fullFileName = Path.Combine(FileTools.defaulExportFolderAuras, FileTools.defaulFileAuras);
                MessageBox.Show("The specified filename is invalid.\n" +
                                "Auras info will be saved in the file:\n" +
                                fullFileName, "Caution!", MessageBoxButtons.OK);
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));


            XmlSerializer serialiser = new XmlSerializer(typeof(AurasWrapper));
            TextWriter FileStream = new StreamWriter(fullFileName);
            serialiser.Serialize(FileStream, auras);
            FileStream.Close();
        }

        private void btnMissions_Click(object sender, EventArgs e)
        {
            MissionsWrapper missions = new MissionsWrapper(EntityManager.LocalPlayer);

            string fullFileName = FileTools.ReplaceMask(bteMissions.Text);

            if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                fullFileName = Path.Combine(FileTools.defaulExportFolderMissions, FileTools.defaulFileMissions);
                MessageBox.Show("The specified filename is invalid.\n" +
                    "Missions info will be saved in the file:\n" +
                    fullFileName, "Caution!", MessageBoxButtons.OK);
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            XmlSerializer serialiser = new XmlSerializer(typeof(MissionsWrapper));
            TextWriter FileStream = new StreamWriter(fullFileName);
            serialiser.Serialize(FileStream, missions);
            FileStream.Close();
        }
    }
}
