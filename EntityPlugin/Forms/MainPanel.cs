//#define Test_EntitySelectForm

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using EntityTools.States;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Forms
{
    public partial class MainPanel : /* UserControl //*/ Astral.Forms.BasePanel
    {
        private EntitySelectForm.EntityDif entDif = new EntitySelectForm.EntityDif();

        public MainPanel() : base("Entity Tools")
        {
            InitializeComponent();

            ckbSpellStuckMonitor.Checked = States.SpellStuckMonitor.Activate;
            cbSlideMonitor.Checked = States.SlideMonitor.Activate;
            gbSlideMonitor.Enabled = States.SlideMonitor.Activate;
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

            #region Test_InsertInsignia
            //// Ищем все неэкипированные инсигнии (впервый раз)
            //List<InventorySlot> freeInsignias = EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.Item.ItemDef.InternalName.StartsWith("Insignia"));
            //// сортировка списка "инсигний";
            //freeInsignias.Sort(Actions.InsertInsignia.InsigniaQualityDescendingComparison);

            //StringBuilder sb = new StringBuilder();
            //foreach(InventorySlot slot in freeInsignias)
            //{
            //    sb./*Append(slot.Item.ItemDef.DisplayName).*/Append('[').Append(slot.Item.ItemDef.InternalName).Append("] number is ").Append(slot.Item.Count).AppendLine();
            //}
            //MessageBox.Show(sb.ToString());

            //InventorySlot insigniaSlot = InsertInsignia(freeInsignias);

            //if (insigniaSlot != null)
            //{
            //    sb.Clear();
            //    sb.Append("InsigniaSlot.IsValid=").Append(insigniaSlot.IsValid).AppendLine();
            //    sb.Append("InsigniaSlot.Filled=").Append(insigniaSlot.Filled).AppendLine();
            //    sb.Append("InsigniaSlot.Item.IsValid=").Append(insigniaSlot.Item.IsValid).AppendLine();
            //    sb.Append("InsigniaSlot.Item.ItemDef.IsValid=").Append(insigniaSlot.Item.ItemDef.IsValid).AppendLine();
            //    sb.Append("InsigniaSlot.Item.ItemDef.InternalName=").Append(insigniaSlot.Item.ItemDef.InternalName).AppendLine();

            //    MessageBox.Show(sb.ToString());
            //}
            #endregion

            InsigniaBonusSelectForm.GetMountBonuses();
        }

        private void ckbDebugInfo_CheckedChanged(object sender, EventArgs e)
        {
            EntityTools.DebugInfoEnabled = ckbDebugInfo.Checked;
        }

        private void MainPanel_Load(object sender, EventArgs e)
        {
            ckbDebugInfo.Checked = EntityTools.DebugInfoEnabled;

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

            if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(fullFileName);
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

            if(MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(fullFileName);
        }

        private void btnInterfaces_Click(object sender, EventArgs e)
        {
            InterfaceWrapper Interfaces = new InterfaceWrapper();

            string fullFileName = FileTools.ReplaceMask(bteInterfaces.Text);

            if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                fullFileName = Path.Combine(FileTools.defaultExportFolderInterfaces, FileTools.defaulFileInterfaces);
                MessageBox.Show("The specified filename is invalid.\n" +
                    "Missions info will be saved in the file:\n" +
                    fullFileName, "Caution!", MessageBoxButtons.OK);
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            XmlSerializer serialiser = new XmlSerializer(typeof(InterfaceWrapper));
            TextWriter FileStream = new StreamWriter(fullFileName);
            serialiser.Serialize(FileStream, Interfaces);
            FileStream.Close();

            if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(fullFileName);

        }

        private void cbSpellStuckMonitor_CheckedChanged(object sender, EventArgs e)
        {
            SpellStuckMonitor.Activate = ckbSpellStuckMonitor.Checked;
        }

        private void seSlideFilter_EditValueChanged(object sender, EventArgs e)
        {
            SlideMonitor.Filter = (float)seSlideFilter.Value;
        }

        private void seTimerUnslide_EditValueChanged(object sender, EventArgs e)
        {
            SlideMonitor.CheckTimeNotSlide = (int)seTimerUnslide.Value;
        }

        private void seTimerSlide_EditValueChanged(object sender, EventArgs e)
        {
            SlideMonitor.CheckTimeNotSlide = (int)seTimerUnslide.Value;
        }

        private void gcbSlideMonitor_CustomButtonChecked(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {

        }

        private void cbSlideMonitor_CheckedChanged(object sender, EventArgs e)
        {
            SlideMonitor.Activate = cbSlideMonitor.Checked;
            gbSlideMonitor.Enabled = cbSlideMonitor.Checked;
        }

        private void btnUiViewer_Click(object sender, EventArgs e)
        {
            #region Перенесено в btnInterfaces_Click(...)
            //InterfaceWrapper Interfaces = new InterfaceWrapper();

            //string fullFileName = Path.Combine(FileTools.defaultExportFolderInterfaces, FileTools.defaulFileInterfaces);

            //if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
            //    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            //XmlSerializer serialiser = new XmlSerializer(typeof(InterfaceWrapper));
            //TextWriter FileStream = new StreamWriter(fullFileName);
            //serialiser.Serialize(FileStream, Interfaces);
            //FileStream.Close();

            //if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    System.Diagnostics.Process.Start(fullFileName);
            #endregion
            #region Тест работы с UIGen.ButtonClick()
            // Тест работы с UIGen.ButtonClick()

            //UIGen uiGen = UIManager.GetUIGenByName("Playerstatus_Mounts_Subtab_Activemounts");
            //if (uiGen != null)
            //{
            //    if (uiGen.IsValid && uiGen.IsVisible
            //        && uiGen.Type == MyNW.Patchables.Enums.UIGenType.Button)
            //        // Вызывает экстренное закрытие игрового клиента
            //        uiGen.ButtonClick();
            //    else MessageBox.Show($"Name={uiGen?.Name}\r\n\t IsValid={uiGen?.IsValid}\r\n\t IsVisible={uiGen?.IsVisible}\r\n\t Type={uiGen?.Type}");
            //}
            //else MessageBox.Show("'Playerstatus_Mounts_Subtab_Activemounts' not found");
            #endregion
            #region Тест формы отображения интерфейсов
            UIViewer.GetUiGen();
            #endregion
        }

        private InventorySlot InsertInsignia(List<InventorySlot> freeInsignias = null)
        {
            // freeInsignias - Список всех неэкипированных инсигний (знаков скакунов)            

            // Выбираем всех активных коней
            List<InventorySlot> activeMounts = EntityManager.LocalPlayer.GetInventoryBagById(MyNW.Patchables.Enums.InvBagIDs.MountEquippedActiveSlots).GetItems;
            //EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.BagId == MyNW.Patchables.Enums.InvBagIDs.MountEquippedActiveSlots);

            foreach (InventorySlot mount in activeMounts)
            {
#if DEBUG_INSERTINSIGNIA
                int insertedNum = 0;

                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Check mount '{mount.Item.ItemDef.InternalName}'");
#endif
                // Проверяем наличие свободного места для знака
                foreach (ItemGemSlotDef insgnSlotDef in mount.Item.ItemDef.EffectiveItemGemSlots)
                {
                    // insgnSlotDef - описание слота
                    // insgnSlot - слот знака
                    ItemGemSlot insgnSlot = mount.Item.SpecialProps.GetGemSlotByIndex(insgnSlotDef.Index);
                    if (insgnSlotDef != null && insgnSlotDef.IsValid
                        && (insgnSlot == null || !insgnSlot.IsValid || !insgnSlot.SlottedItem.IsValid))
                    {
                        // обнаружен "пустой слот" знака скакуна
#if DEBUG_INSERTINSIGNIA
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Mount [{mount.Item.ItemDef.InternalName}] has free slot {insgnSlotDef.Index}");
#endif
                        // ищем подходящие инсигнии (знаки скакуна)
                        if (freeInsignias == null || freeInsignias.Count == 0)
                        {
                            // Ищем все неэкипированные инсигнии (впервый раз)
                            freeInsignias = EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.Item.ItemDef.InternalName.StartsWith("Insignia"));
                            // сортировка списка "инсигний";
                            freeInsignias.Sort(Actions.InsertInsignia.InsigniaQualityDescendingComparison);
                            if (freeInsignias == null || freeInsignias.Count == 0)
                            {
                                // в инвентаре отсутствуют инсигнии
#if DEBUG_INSERTINSIGNIA
                                Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: No one insignia found in the Bags");
#endif
                                return null;
                            }
                        }

                        // Ищем первую попавшуюся подходящую инсигнию (знак скакуна)
                        InventorySlot insignia = freeInsignias.Find(insgn => (insgnSlotDef.Type == (uint)Tools.MountInsignias.InsigniaType.Universal || insgn.Item.ItemDef.GemType == insgnSlotDef.Type));

                        // экипируем найденный знак
                        if (insignia != null && insignia.IsValid)
                        {
                            mount.Item.GemThisItem(insignia.Item, insgnSlotDef.Index);
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, $"{GetType().Name}: Insert '{insignia.Item.ItemDef.InternalName}' at the slot {insgnSlotDef.Index} of [{mount.Item.ItemDef.InternalName}]");
#if DEBUG_INSERTINSIGNIA
                            insertedNum++;
#endif
                            return insignia;
                        }
                    }

                }
            }

            return null;
        }

        private void cbEnchantHelperActivator_CheckedChanged(object sender, EventArgs e)
        {
            EnchantHelper.Enabled = cbEnchantHelperActivator.Checked;
        }
    }
}
