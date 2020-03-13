//#define Test_EntitySelectForm
//#define DUMP_TEST

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using EntityTools.Tools;
using MyNW.Internals;
using MyNW;
using EntityTools.UCC.Extensions;
using DevExpress.XtraEditors;
using EntityTools.Enums;
using EntityTools.Servises;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : /* UserControl //*/ Astral.Forms.BasePanel
    {
        private EntityDef entDif = new EntityDef();

        public EntityToolsMainPanel() : base("Entity Tools")
        {
            InitializeComponent();

            cbbExportSelector.DataSource = Enum.GetValues(typeof(ExportTypes));

            //ckbSpellStuckMonitor.Checked = EntityTools.PluginSettings.UnstuckSpells.Active;
            ckbSpellStuckMonitor.DataBindings.Add(nameof(ckbSpellStuckMonitor.Checked),
                                                  EntityTools.PluginSettings.UnstuckSpells,
                                                  nameof(EntityTools.PluginSettings.UnstuckSpells.Active));

            // Настройки Mapper'a
            ckbMapperPatch.DataBindings.Add(nameof(ckbMapperPatch.Checked),
                                               EntityTools.PluginSettings.Mapper,
                                               nameof(EntityTools.PluginSettings.Mapper.Patch),
                                               false, DataSourceUpdateMode.OnPropertyChanged);

            seMapperWaipointDistance.DataBindings.Add(nameof(seMapperWaipointDistance.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointDistance));
            seMapperMaxZDif.DataBindings.Add(nameof(seMapperMaxZDif.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.MaxElevationDifference));
            seWaypointEquivalenceDistance.DataBindings.Add(nameof(seWaypointEquivalenceDistance.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance));

            ckbMapperForceLinkingWaypoint.DataBindings.Add(nameof(ckbMapperForceLinkingWaypoint.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint));
            ckbMapperLinearPath.DataBindings.Add(nameof(ckbMapperLinearPath.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.LinearPath));
        }

        private void btnTest_Click(object sender, EventArgs e)
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

            #region Test_MountBonusPriority
            //MountBonusPriorityListForm.GetBonusList();
            //InsigniaBonusSelectForm.GetMountBonuses();
            #endregion

            #region Test_ComplexCondition
            //ConditionListForm listEditor = new ConditionListForm();
            //var condList = listEditor.GetConditionList();
            //if (condList != null)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (var cond in condList)
            //    {
            //        sb.AppendLine(cond.ToString());
            //    }
            //    MessageBox.Show(sb.ToString());
            //}
            //else MessageBox.Show("Conditions is empty");


            #endregion

            #region DUMP_TEST
#if DUMP_TEST
            ReflectionHelper.DumpClassMethod(typeof(Astral.Logic.UCC.Forms.AddClass), "AddClass");

            ReflectionHelper.DumpClassMethod(typeof(TestClass), "TestClass");
#endif
            #endregion

            #region Login
            //ReflectionHelper.ExecStaticMethodByArgs(typeof(MyNW.Internals.Injection), "\u0001", new object[] { "--", "--" }, out object res);
            #endregion

            #region Mapper
            ////var node = Astral.Quester.Core.GetNearesNodetPosition(EntityManager.LocalPlayer.Location, false);
            ////var meshes = Astral.Quester.Core.Meshes;
            //var meshes = typeof(Astral.Quester.Core).GetStaticPropertyAccessor<AStar.Graph>("Meshes");
            //XtraMessageBox.Show(meshes.Value.ToString());


            //var GetNearesNodes = typeof(Astral.Quester.Core).GetStaticMethodInvoker<MyNW.Classes.Vector3>("GetNearesNodetPosition", new Type[] { typeof(MyNW.Classes.Vector3), typeof(bool)});
            //var pos = GetNearesNodes.Invoke(EntityManager.LocalPlayer.Location, false);
            //XtraMessageBox.Show(pos.ToString());
            #endregion

            #region Test_LinkedList

            //Dictionary<int, Pair<WatchPair, WatchPair>> listWatches = new Dictionary<int, Pair<WatchPair, WatchPair>>();
            //Dictionary<int, Pair<WatchPair, WatchPair>> linkedListWatches = new Dictionary<int, Pair<WatchPair, WatchPair>>();
            //Pair<WatchPair, WatchPair> listTotal = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
            //Pair<WatchPair, WatchPair> LinkListTotal = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());

            //Stopwatch sw = new Stopwatch();
            //Random rnd = new Random();
            //for (int i = 0; i< 1000;  i++)
            //{
            //    double dist = 100 + rnd.Next(700);
            //    Pair<WatchPair, WatchPair> watch = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
            //    sw.Restart();
            //    List<Entity> list = new List<Entity>();
            //    foreach(Entity ent in EntityManager.GetEntities())
            //    {
            //        list.Add(ent);
            //    }
            //    sw.Stop();
            //    watch.First.Ticks = sw.ElapsedTicks;
            //    listTotal.First.Ticks += sw.ElapsedTicks;
            //    watch.First.Millisecond = sw.ElapsedMilliseconds;
            //    listTotal.First.Millisecond += sw.ElapsedMilliseconds;
            //    sw.Restart();
            //    list.RemoveAll((o) => o.Location.Distance3DFromPlayer > dist);
            //    sw.Stop();
            //    watch.Second.Ticks = sw.ElapsedTicks;
            //    listTotal.Second.Ticks += sw.ElapsedTicks;
            //    watch.Second.Millisecond = sw.ElapsedMilliseconds;
            //    listTotal.Second.Millisecond += sw.ElapsedMilliseconds;
            //    listWatches.Add(i, watch);

            //    watch = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
            //    sw.Restart();
            //    LinkedList<Entity> linkList = new LinkedList<Entity>();
            //    foreach (Entity ent in EntityManager.GetEntities())
            //    {
            //        linkList.AddLast(ent);
            //    }
            //    sw.Stop();
            //    watch.First.Ticks = sw.ElapsedTicks;
            //    LinkListTotal.First.Ticks += sw.ElapsedTicks;
            //    watch.First.Millisecond = sw.ElapsedMilliseconds;
            //    LinkListTotal.First.Millisecond += sw.ElapsedMilliseconds;
            //    sw.Restart();
            //    //list.RemoveAll((o) => o.Location.Distance3DFromPlayer > 500);
            //    LinkedList<Entity> newlinkList = new LinkedList<Entity>();
            //    while(linkList.Count > 0)
            //    {
            //        LinkedListNode<Entity> node = linkList.First;
            //        linkList.RemoveFirst();
            //        if (node.Value.Location.Distance3DFromPlayer > dist)
            //            newlinkList.AddLast(node);
            //    }

            //    sw.Stop();
            //    watch.Second.Ticks = sw.ElapsedTicks;
            //    LinkListTotal.Second.Ticks += sw.ElapsedTicks;
            //    watch.Second.Millisecond = sw.ElapsedMilliseconds;
            //    LinkListTotal.Second.Millisecond += sw.ElapsedMilliseconds;
            //    linkedListWatches.Add(i, watch);
            //}

            //Logger.WriteLine("List<Entity> Test:");
            //Logger.WriteLine($"\tCreate Total (t):\t {listTotal.First.Ticks.ToString()}");
            //Logger.WriteLine($"\tCreate Total (ms):\t {listTotal.First.Millisecond}");
            //Logger.WriteLine($"\tRemote Total (t):\t {listTotal.Second.Ticks.ToString()}");
            //Logger.WriteLine($"\tRemote Total (ms):\t {listTotal.Second.Millisecond}");
            //Logger.WriteLine("--------------------------------------------------------");
            //foreach (var w in listWatches)
            //{
            //    Logger.WriteLine($"\t{w.Value.First.Millisecond.ToString("")}\t{w.Value.First.Ticks}\t\t{w.Value.Second.Millisecond}\t{w.Value.Second.Ticks}\t");
            //}
            //Logger.WriteLine("========================================================");
            //Logger.WriteLine(string.Empty);
            //Logger.WriteLine("LinkedList<Entity> Test:");
            //Logger.WriteLine($"\tCreate Total (t):\t {LinkListTotal.First.Ticks.ToString("N3")}");
            //Logger.WriteLine($"\tCreate Total (ms):\t {LinkListTotal.First.Millisecond}");
            //Logger.WriteLine($"\tRemote Total (t):\t {LinkListTotal.Second.Ticks.ToString("N3")}");
            //Logger.WriteLine($"\tRemote Total (ms):\t {LinkListTotal.Second.Millisecond}");
            //Logger.WriteLine("--------------------------------------------------------");
            //foreach (var w in linkedListWatches)
            //{
            //    Logger.WriteLine($"\t{w.Value.First.Millisecond}\t{w.Value.First.Ticks}\t\t{w.Value.Second.Millisecond}\t{w.Value.Second.Ticks}\t");
            //}
            //Logger.WriteLine("========================================================");
            #endregion

            #region AStarDiagnostic
            string filename = AStar.SearchStatistics.SaveLog();
            if (!string.IsNullOrEmpty(filename)
                || MessageBox.Show(this, $"Would you like to open {Path.GetFileName(filename)}?", $"Open {Path.GetFileName(filename)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(filename);
            #endregion
        }

        private void btnUccEditor_Click(object sender, EventArgs e)
        {
            Astral.Logic.UCC.Forms.Editor uccEditor = null;
            if (UCCEditorExtensions.GetUccEditor(ref uccEditor))
            {
                uccEditor.Show();
            }
        }


        #region старый_экспорт
        //private void MainPanel_Load(object sender, EventArgs e)
        //{
        //    //bteMissions.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderMissions, FileTools.defaulFileMissions);
        //    //bteAuras.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderAuras, FileTools.defaulFileAuras);
        //}

        //private void bte_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    if (sender is DevExpress.XtraEditors.ButtonEdit bte)
        //    {
        //        string fileName = string.Empty;

        //        if (string.IsNullOrEmpty(bte.Text) || (bte.Text.IndexOfAny(Path.GetInvalidPathChars()) != -1))
        //        {

        //            if (bte.Name == bteAuras.Name) fldrBroserDlg.SelectedPath = FileTools.defaultExportFolderAuras;
        //            if (bte.Name == bteMissions.Name) fldrBroserDlg.SelectedPath = FileTools.defaultExportFolderMissions;
        //        }
        //        else
        //        {
        //            fldrBroserDlg.SelectedPath = Path.GetDirectoryName(bte.Text);
        //            fileName = Path.GetFileName(bte.Text);
        //        }

        //        if (fldrBroserDlg.ShowDialog() == DialogResult.OK)
        //        {
        //            if (string.IsNullOrEmpty(fileName))
        //            {
        //                if (bte.Name == bteAuras.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaultFileAuras);
        //                if (bte.Name == bteMissions.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaultFileMissions);
        //            }
        //            else bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, fileName);
        //        }
        //    }
        //}

        //private void btnAuras_Click(object sender, EventArgs e)
        //{
        //    AurasWrapper auras = new AurasWrapper(EntityManager.LocalPlayer?.Character);

        //    string fullFileName = FileTools.ReplaceMask(bteAuras.Text);

        //    if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        //    {
        //        fullFileName = Path.Combine(FileTools.defaultExportFolderAuras, FileTools.defaultFileAuras);
        //        MessageBox.Show("The specified filename is invalid.\n" +
        //                        "Auras will be saved in the file:\n" +
        //                        fullFileName, "Caution!", MessageBoxButtons.OK);
        //    }

        //    if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));


        //    XmlSerializer serialiser = new XmlSerializer(typeof(AurasWrapper));
        //    TextWriter FileStream = new StreamWriter(fullFileName);
        //    serialiser.Serialize(FileStream, auras);
        //    FileStream.Close();

        //    if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        System.Diagnostics.Process.Start(fullFileName);
        //}

        //private void btnMissions_Click(object sender, EventArgs e)
        //{
        //    MissionsWrapper missions = new MissionsWrapper(EntityManager.LocalPlayer);

        //    string fullFileName = FileTools.ReplaceMask(bteMissions.Text);

        //    if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        //    {
        //        fullFileName = Path.Combine(FileTools.defaultExportFolderMissions, FileTools.defaultFileMissions);
        //        MessageBox.Show("The specified filename is invalid.\n" +
        //            "Missions will be saved in the file:\n" +
        //            fullFileName, "Caution!", MessageBoxButtons.OK);
        //    }

        //    if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

        //    XmlSerializer serialiser = new XmlSerializer(typeof(MissionsWrapper));
        //    TextWriter FileStream = new StreamWriter(fullFileName);
        //    serialiser.Serialize(FileStream, missions);
        //    FileStream.Close();

        //    if(MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        System.Diagnostics.Process.Start(fullFileName);
        //}

        //private void btnInterfaces_Click(object sender, EventArgs e)
        //{
        //    InterfaceWrapper Interfaces = new InterfaceWrapper();

        //    string fullFileName = FileTools.ReplaceMask(bteInterfaces.Text);

        //    if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        //    {
        //        fullFileName = Path.Combine(FileTools.defaultExportFolderInterfaces, FileTools.defaultFileInterfaces);
        //        MessageBox.Show("The specified filename is invalid.\n" +
        //            "Interafaces will be saved in the file:\n" +
        //            fullFileName, "Caution!", MessageBoxButtons.OK);
        //    }

        //    if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

        //    XmlSerializer serialiser = new XmlSerializer(typeof(InterfaceWrapper));
        //    TextWriter FileStream = new StreamWriter(fullFileName);
        //    serialiser.Serialize(FileStream, Interfaces);
        //    FileStream.Close();

        //    if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        System.Diagnostics.Process.Start(fullFileName);

        //}

        //private void btnStates_Click(object sender, EventArgs e)
        //{
        //    string fullFileName = FileTools.ReplaceMask(bteStates.Text);

        //    if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        //    {
        //        fullFileName = Path.Combine(FileTools.defaultExportFolderStates, FileTools.defaultFileStates);
        //        MessageBox.Show("The specified filename is invalid.\n" +
        //            "Missions info will be saved in the file:\n" +
        //            fullFileName, "Caution!", MessageBoxButtons.OK);
        //    }

        //    if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

        //    using (StreamWriter sw = new StreamWriter(fullFileName, false, Encoding.Default))
        //    {
        //        sw.WriteLine($"Character: {EntityManager.LocalPlayer.InternalName}");
        //        sw.WriteLine($"DateTime: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
        //        sw.WriteLine();
        //        foreach (Astral.Logic.Classes.FSM.State state in Astral.Quester.API.Engine.States)
        //        {
        //            sw.WriteLine($"{state.DisplayName} {state.Priority}");
        //            sw.WriteLine($"\t{state.GetType().FullName}");
        //        }
        //    }

        //    if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //        System.Diagnostics.Process.Start(fullFileName);

        //}
        #endregion

        private void cbSpellStuckMonitor_CheckedChanged(object sender, EventArgs e)
        {
            //SpellStuckMonitor.Activate = false;// ckbSpellStuckMonitor.Checked;
            //UnstuckSpellTask.Active = ckbSpellStuckMonitor.Checked;
            EntityTools.PluginSettings.UnstuckSpells.Active = ckbSpellStuckMonitor.Checked;
        }

        private void cbEnchantHelperActivator_CheckedChanged(object sender, EventArgs e)
        {
            EnchantHelper.Enabled = cbEnchantHelperActivator.Checked;
        }

        private void btnUiViewer_Click(object sender, EventArgs e)
        {
            UIViewer.ShowFreeTool();
        }

        private void btnEntities_Click(object sender, EventArgs e)
        {
            EntitySelectForm.ShowFreeTool();
        }

        private void btnAuraViewer_Click(object sender, EventArgs e)
        {
            Editors.Forms.AuraSelectForm.ShowFreeTool();
        }

        private void btnGetMachineId_Click(object sender, EventArgs e)
        {
            var machineid = Memory.MMemory.ReadString(Memory.BaseAdress + 0x2640BD0, Encoding.UTF8, 64);
            lblAccount.Text = $"Account:   @{EntityManager.LocalPlayer.AccountLoginUsername}";
            tbMashingId.Text = machineid;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if(cbbExportSelector.SelectedItem is ExportTypes expType)
            {
                string fullFileName = FileTools.ReplaceMask(tbExportFileSelector.Text);

                if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                {
                    fullFileName = Path.Combine(Astral.Controllers.Directories.LogsPath, expType.ToString(), FileTools.ReplaceMask(FileTools.defaultExportFileName));
                    MessageBox.Show("The specified filename is incorrect.\n" +
                                    $"{expType} will be saved in the file:\n" +
                                    fullFileName, "Caution!", MessageBoxButtons.OK);
                }

                if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

                switch (expType)
                {
                    case ExportTypes.Auras:
                        {
                            AurasWrapper auras = new AurasWrapper(EntityManager.LocalPlayer?.Character);

                            XmlSerializer serialiser = new XmlSerializer(typeof(AurasWrapper));
                            TextWriter FileStream = new StreamWriter(fullFileName);
                            serialiser.Serialize(FileStream, auras);
                            FileStream.Close();
                            break;
                        }
                    case ExportTypes.Interfaces:
                        {
                            InterfaceWrapper Interfaces = new InterfaceWrapper();

                            XmlSerializer serialiser = new XmlSerializer(typeof(InterfaceWrapper));
                            TextWriter FileStream = new StreamWriter(fullFileName);
                            serialiser.Serialize(FileStream, Interfaces);
                            FileStream.Close();

                            break;
                        }
                    case ExportTypes.Missions:
                        {
                            MissionsWrapper missions = new MissionsWrapper(EntityManager.LocalPlayer);

                            XmlSerializer serialiser = new XmlSerializer(typeof(MissionsWrapper));
                            TextWriter FileStream = new StreamWriter(fullFileName);
                            serialiser.Serialize(FileStream, missions);
                            FileStream.Close();
                            break;
                        }                        
                    case ExportTypes.States:
                        {
                            using (StreamWriter sw = new StreamWriter(fullFileName, false, Encoding.Default))
                            {
                                sw.WriteLine($"Character: {EntityManager.LocalPlayer.InternalName}");
                                sw.WriteLine($"DateTime: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                                sw.WriteLine();
                                foreach (Astral.Logic.Classes.FSM.State state in Astral.Quester.API.Engine.States)
                                {
                                    sw.WriteLine($"{state.DisplayName} {state.Priority}");
                                    sw.WriteLine($"\t{state.GetType().FullName}");
                                }
                            }
                            break;
                        }
                }

                if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(fullFileName);
            }
        }

        private void tbExportFileSelector_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cbbExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                if (string.IsNullOrEmpty(tbExportFileSelector.Text))
                {
                    fileName = Path.Combine(Astral.Controllers.Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                    fileName.Replace(Astral.Controllers.Directories.AstralStartupPath, @".\");
                }
                else fileName = tbExportFileSelector.Text;

                string directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(directory);
                dlgSaveFile.InitialDirectory = directory;

                dlgSaveFile.FileName = fileName;
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                    tbExportFileSelector.Text = dlgSaveFile.FileName;
            }
        }

        private void cbbExportSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                fileName = Path.Combine(Astral.Controllers.Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                fileName.Replace(Astral.Controllers.Directories.AstralStartupPath, @".\");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void btnShowMapper_Click(object sender, EventArgs e)
        {
            Astral.Quester.Forms.MapperForm.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MapperFormExt.Open();
            XtraMessageBox.Show(EntityTools.PluginSettings.Mapper.WaypointDistance.ToString());
        }
    }
}
