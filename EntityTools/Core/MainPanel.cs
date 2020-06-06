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
using EntityTools.Enums;
using EntityTools.Services;
using Astral.Classes.ItemFilter;
using System.Collections.Generic;
using MyNW.Classes;
using DevExpress.XtraEditors;
using EntityTools.Tools.BuySellItems;
using MyNW.Patchables.Enums;
using EntityTools.Forms;
using System.Diagnostics;
using EntityTools.UCC.Conditions;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : /* UserControl //*/ Astral.Forms.BasePanel
    {
        private EntityDef entDif = new EntityDef();

        public EntityToolsMainPanel() : base("Entity Tools")
        {
            InitializeComponent();

            cbbxExportSelector.DataSource = Enum.GetValues(typeof(ExportTypes));

            //ckbSpellStuckMonitor.Checked = EntityTools.PluginSettings.UnstuckSpells.Active;
            ckbSpellStuckMonitor.DataBindings.Add(nameof(ckbSpellStuckMonitor.Checked),
                                                EntityTools.PluginSettings.UnstuckSpells,
                                                nameof(EntityTools.PluginSettings.UnstuckSpells.Active),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

#if DEVELOPER
            // Настройки Mapper'a
            ckbMapperPatch.DataBindings.Add(nameof(ckbMapperPatch.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.Patch),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            seMapperWaipointDistance.DataBindings.Add(nameof(seMapperWaipointDistance.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            seMapperMaxZDif.DataBindings.Add(nameof(seMapperMaxZDif.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.MaxElevationDifference),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            seWaypointEquivalenceDistance.DataBindings.Add(nameof(seWaypointEquivalenceDistance.Value),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbMapperForceLinkingWaypoint.DataBindings.Add(nameof(ckbMapperForceLinkingWaypoint.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            ckbMapperLinearPath.DataBindings.Add(nameof(ckbMapperLinearPath.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.LinearPath),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            // Настройки EntityToolsLogger
            ckbEnableLogger.DataBindings.Add(nameof(ckbEnableLogger.Checked),
                                                EntityTools.PluginSettings.Logger,
                                                nameof(EntityTools.PluginSettings.Logger.Active),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            ckbExtendedActionDebugInfo.DataBindings.Add(nameof(ckbExtendedActionDebugInfo.Checked),
                                                EntityTools.PluginSettings.Logger,
                                                nameof(EntityTools.PluginSettings.Logger.ExtendedActionDebugInfo),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            

            // Настройки EntityCache
            seGlobalCacheTime.DataBindings.Add( nameof(seGlobalCacheTime.Value),
                                                EntityTools.PluginSettings.EntityCache,
                                                nameof(EntityTools.PluginSettings.EntityCache.GlobalCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            seLocalCacheTime.DataBindings.Add(nameof(seLocalCacheTime.Value),
                                                EntityTools.PluginSettings.EntityCache,
                                                nameof(EntityTools.PluginSettings.EntityCache.LocalCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            seCombatCacheTime.DataBindings.Add(nameof(seCombatCacheTime.Value),
                                                EntityTools.PluginSettings.EntityCache,
                                                nameof(EntityTools.PluginSettings.EntityCache.CombatCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

#else
            btnEntities.Visible = false;
            btnAuraViewer.Visible = false;
            btnUiViewer.Visible = false;

            tabOptions.PageVisible = false;
            tabRelogger.PageVisible = false;
            tabMapper.PageVisible = false;
            tabDebug.PageVisible = false;
#endif
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            #region Старые тесты
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

#if Test_EntitySelectForm
            entDif = EntitySelectForm.GetEntity(entDif.NameUntranslated);
            if (entDif != null)
                MessageBox.Show($"Selected Entity:\n" +
                    $"Name: {entDif.Name}\n" +
                    $"InternalName: {entDif.InternalName}\n" +
                    $"NameUntranslated: {entDif.NameUntranslated}");
            else MessageBox.Show("No Entity was selected");

#endif

#if Test_InsertInsignia
            // Ищем все неэкипированные инсигнии (впервый раз)
            List<InventorySlot> freeInsignias = EntityManager.LocalPlayer.BagsItems.FindAll(slot => slot.Item.ItemDef.InternalName.StartsWith("Insignia"));
            // сортировка списка "инсигний";
            freeInsignias.Sort(Actions.InsertInsignia.InsigniaQualityDescendingComparison);

            StringBuilder sb = new StringBuilder();
            foreach (InventorySlot slot in freeInsignias)
            {
                sb./*Append(slot.Item.ItemDef.DisplayName).*/Append('[').Append(slot.Item.ItemDef.InternalName).Append("] number is ").Append(slot.Item.Count).AppendLine();
            }
            MessageBox.Show(sb.ToString());

            InventorySlot insigniaSlot = InsertInsignia(freeInsignias);

            if (insigniaSlot != null)
            {
                sb.Clear();
                sb.Append("InsigniaSlot.IsValid=").Append(insigniaSlot.IsValid).AppendLine();
                sb.Append("InsigniaSlot.Filled=").Append(insigniaSlot.Filled).AppendLine();
                sb.Append("InsigniaSlot.Item.IsValid=").Append(insigniaSlot.Item.IsValid).AppendLine();
                sb.Append("InsigniaSlot.Item.ItemDef.IsValid=").Append(insigniaSlot.Item.ItemDef.IsValid).AppendLine();
                sb.Append("InsigniaSlot.Item.ItemDef.InternalName=").Append(insigniaSlot.Item.ItemDef.InternalName).AppendLine();

                MessageBox.Show(sb.ToString());
            } 
#endif

#if Test_MountBonusPriority
            MountBonusPriorityListForm.GetBonusList();
            InsigniaBonusSelectForm.GetMountBonuses();
#endif

#if Test_ComplexCondition
            ConditionListForm listEditor = new ConditionListForm();
            var condList = listEditor.GetConditionList();
            if (condList != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var cond in condList)
                {
                    sb.AppendLine(cond.ToString());
                }
                MessageBox.Show(sb.ToString());
            }
            else MessageBox.Show("Conditions is empty");
#endif

#if DUMP_TEST
            ReflectionHelper.DumpClassMethod(typeof(Astral.Logic.UCC.Forms.AddClass), "AddClass");

            ReflectionHelper.DumpClassMethod(typeof(TestClass), "TestClass");
#endif

#if Login
            ReflectionHelper.ExecStaticMethodByArgs(typeof(MyNW.Internals.Injection), "\u0001", new object[] { "--", "--" }, out object res);
#endif

#if Mapper
            //var node = Astral.Quester.Core.GetNearesNodetPosition(EntityManager.LocalPlayer.Location, false);
            //var meshes = Astral.Quester.Core.Meshes;
            var meshes = typeof(Astral.Quester.Core).GetStaticPropertyAccessor<AStar.Graph>("Meshes");
            XtraMessageBox.Show(meshes.Value.ToString());


            var GetNearesNodes = typeof(Astral.Quester.Core).GetStaticMethodInvoker<MyNW.Classes.Vector3>("GetNearesNodetPosition", new Type[] { typeof(MyNW.Classes.Vector3), typeof(bool) });
            var pos = GetNearesNodes.Invoke(EntityManager.LocalPlayer.Location, false);
            XtraMessageBox.Show(pos.ToString());
#endif

#if Test_LinkedList

            Dictionary<int, Pair<WatchPair, WatchPair>> listWatches = new Dictionary<int, Pair<WatchPair, WatchPair>>();
            Dictionary<int, Pair<WatchPair, WatchPair>> linkedListWatches = new Dictionary<int, Pair<WatchPair, WatchPair>>();
            Pair<WatchPair, WatchPair> listTotal = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
            Pair<WatchPair, WatchPair> LinkListTotal = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());

            Stopwatch sw = new Stopwatch();
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                double dist = 100 + rnd.Next(700);
                Pair<WatchPair, WatchPair> watch = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
                sw.Restart();
                List<Entity> list = new List<Entity>();
                foreach (Entity ent in EntityManager.GetEntities())
                {
                    list.Add(ent);
                }
                sw.Stop();
                watch.First.Ticks = sw.ElapsedTicks;
                listTotal.First.Ticks += sw.ElapsedTicks;
                watch.First.Millisecond = sw.ElapsedMilliseconds;
                listTotal.First.Millisecond += sw.ElapsedMilliseconds;
                sw.Restart();
                list.RemoveAll((o) => o.Location.Distance3DFromPlayer > dist);
                sw.Stop();
                watch.Second.Ticks = sw.ElapsedTicks;
                listTotal.Second.Ticks += sw.ElapsedTicks;
                watch.Second.Millisecond = sw.ElapsedMilliseconds;
                listTotal.Second.Millisecond += sw.ElapsedMilliseconds;
                listWatches.Add(i, watch);

                watch = new Pair<WatchPair, WatchPair>(new WatchPair(), new WatchPair());
                sw.Restart();
                LinkedList<Entity> linkList = new LinkedList<Entity>();
                foreach (Entity ent in EntityManager.GetEntities())
                {
                    linkList.AddLast(ent);
                }
                sw.Stop();
                watch.First.Ticks = sw.ElapsedTicks;
                LinkListTotal.First.Ticks += sw.ElapsedTicks;
                watch.First.Millisecond = sw.ElapsedMilliseconds;
                LinkListTotal.First.Millisecond += sw.ElapsedMilliseconds;
                sw.Restart();
                //list.RemoveAll((o) => o.Location.Distance3DFromPlayer > 500);
                LinkedList<Entity> newlinkList = new LinkedList<Entity>();
                while (linkList.Count > 0)
                {
                    LinkedListNode<Entity> node = linkList.First;
                    linkList.RemoveFirst();
                    if (node.Value.Location.Distance3DFromPlayer > dist)
                        newlinkList.AddLast(node);
                }

                sw.Stop();
                watch.Second.Ticks = sw.ElapsedTicks;
                LinkListTotal.Second.Ticks += sw.ElapsedTicks;
                watch.Second.Millisecond = sw.ElapsedMilliseconds;
                LinkListTotal.Second.Millisecond += sw.ElapsedMilliseconds;
                linkedListWatches.Add(i, watch);
            }

            EntityToolsLogger.WriteLine("List<Entity> Test:");
            EntityToolsLogger.WriteLine($"\tCreate Total (t):\t {listTotal.First.Ticks.ToString()}");
            EntityToolsLogger.WriteLine($"\tCreate Total (ms):\t {listTotal.First.Millisecond}");
            EntityToolsLogger.WriteLine($"\tRemote Total (t):\t {listTotal.Second.Ticks.ToString()}");
            EntityToolsLogger.WriteLine($"\tRemote Total (ms):\t {listTotal.Second.Millisecond}");
            EntityToolsLogger.WriteLine("--------------------------------------------------------");
            foreach (var w in listWatches)
            {
                Logger.WriteLine($"\t{w.Value.First.Millisecond.ToString("")}\t{w.Value.First.Ticks}\t\t{w.Value.Second.Millisecond}\t{w.Value.Second.Ticks}\t");
            }
            EntityToolsLogger.WriteLine("========================================================");
            EntityToolsLogger.WriteLine(string.Empty);
            EntityToolsLogger.WriteLine("LinkedList<Entity> Test:");
            EntityToolsLogger.WriteLine($"\tCreate Total (t):\t {LinkListTotal.First.Ticks.ToString("N3")}");
            EntityToolsLogger.WriteLine($"\tCreate Total (ms):\t {LinkListTotal.First.Millisecond}");
            EntityToolsLogger.WriteLine($"\tRemote Total (t):\t {LinkListTotal.Second.Ticks.ToString("N3")}");
            EntityToolsLogger.WriteLine($"\tRemote Total (ms):\t {LinkListTotal.Second.Millisecond}");
            EntityToolsLogger.WriteLine("--------------------------------------------------------");
            foreach (var w in linkedListWatches)
            {
                EntityToolsLogger.WriteLine($"\t{w.Value.First.Millisecond}\t{w.Value.First.Ticks}\t\t{w.Value.Second.Millisecond}\t{w.Value.Second.Ticks}\t");
            }
            EntityToolsLogger.WriteLine("========================================================");
#endif

#if AStarDiagnostic
            string filename = AStar.SearchStatistics.SaveLog();
            if (!string.IsNullOrEmpty(filename)
                || MessageBox.Show(this, $"Would you like to open {Path.GetFileName(filename)}?", $"Open {Path.GetFileName(filename)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(filename);

#endif

#if Test_FindAllEntity
            LinkedList<Entity> entities = EntityTools.Core.FindAllEntity(@"^M12_(Batiri_Elite_Raptorrider|Dinosaur_Raptor_Standard_Enervoraptor)",
                                                            ItemFilterStringType.Regex, EntityNameType.InternalName, EntitySetType.Complete,
                                                            true, 0, 30, false, null, null);
            StringBuilder sb = new StringBuilder(1024);
            foreach(Entity ett in entities)
            {
                sb.AppendLine($"{ett.InternalName}: Distance({ett.Location.Distance3DFromPlayer})");
            }
            MessageBox.Show(sb.ToString());
#endif

#if Test_IndexedBags_Categories
            IndexedBags bag = new IndexedBags();
            StringBuilder sb = new StringBuilder();
            foreach(ItemCategory cat in Enum.GetValues(typeof(ItemCategory)))
            {
                var list = bag[cat];
                if(list != null && list.Count > 0)
                {
                    sb.AppendLine($"Catherogy [{cat}] contains:");
                    foreach(var slot in list)
                    {
                        sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).AppendLine("]");
                    }
                }
            }
            //XtraMessageBox.Show(sb.ToString());
            Astral.Logger.WriteLine(sb.ToString());
#endif

#if Test_IndexedBags_Filter
            List<ItemFilterEntryExt> filter = new List<ItemFilterEntryExt>()
            {
                new ItemFilterEntryExt(){ StringType = ItemFilterStringType.Simple, EntryType = BuyEntryType.Category, Identifier = "*Artifact*"},
                new ItemFilterEntryExt(){ StringType = ItemFilterStringType.Simple, EntryType = BuyEntryType.Category, Identifier = "Insignia"}
            };
            IndexedBags bag = new IndexedBags(filter);
            StringBuilder sb = new StringBuilder();
            foreach(var f in filter)
            {
                var list = bag[f];
                if(list != null && list.Count > 0)
                {
                    sb.Append(f.ToString()).AppendLine(" contains:");
                    foreach(var slot in list)
                    {
                        sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append("] {");
                        int catNum = 0;
                        foreach(var cat in slot.Item.ItemDef.Categories)
                        {
                            if (catNum > 0) sb.Append(", ");
                            sb.Append(cat);
                            catNum++;
                        }
                        sb.AppendLine("}");
                    }
                }
            }
            //XtraMessageBox.Show(sb.ToString());
            Astral.Logger.WriteLine(sb.ToString());
#endif

#if Test_ItemListEditor
            List<ItemFilterEntryExt> filter = new List<ItemFilterEntryExt>()
            {
                new ItemFilterEntryExt(){ StringType = ItemFilterStringType.Simple, EntryType = ItemFilterEntryType.Category, Identifier = "*Artifact*"},
                new ItemFilterEntryExt(){ Mode = ItemFilterMode.Exclude, StringType = ItemFilterStringType.Simple, EntryType = ItemFilterEntryType.Category, Identifier = "Insignia"},
                new ItemFilterEntryExt(){ StringType = ItemFilterStringType.Regex, EntryType = ItemFilterEntryType.Identifier, Identifier = "Ring" }
            };

            if (ItemFilterEditorForm.GUIRequiest(ref filter))
            {
                IndexedBags bag = new IndexedBags(filter);
                StringBuilder sb = new StringBuilder();
                foreach (var f in filter)
                {
                    var list = bag[f];
                    if (list != null && list.Count > 0)
                    {
                        sb.Append(f.ToString()).AppendLine(" contains:");
                        foreach (var slot in list)
                        {
                            sb.Append('\t').Append(slot.Item.ItemDef.DisplayName).Append('[').Append(slot.Item.ItemDef.InternalName).Append("] {");
                            int catNum = 0;
                            foreach (var cat in slot.Item.ItemDef.Categories)
                            {
                                if (catNum > 0) sb.Append(", ");
                                sb.Append(cat);
                                catNum++;
                            }
                            sb.AppendLine("}");
                        }
                    }
                }
                XtraMessageBox.Show(sb.ToString());
                Astral.Logger.WriteLine(sb.ToString());
            }
#endif

#if Test_BagsAccess
            Stopwatch stopwatch = new Stopwatch();
            StringBuilder sb = new StringBuilder();
            stopwatch.Start();
            var AllItems = EntityManager.LocalPlayer.AllItems;
            stopwatch.Stop();
            sb.AppendLine("Allitem (direct): ");
            sb.Append("\tcount = ").AppendLine(AllItems.Count.ToString());
            sb.Append("\taccessTime = ").Append(stopwatch.ElapsedMilliseconds).Append(" ms (").Append(stopwatch.ElapsedTicks).AppendLine(")");

            stopwatch.Restart();
            var BagsItems = EntityManager.LocalPlayer.BagsItems;
            stopwatch.Stop();
            sb.AppendLine("BagsItems (direct): ");
            sb.Append("\tcount = ").AppendLine(BagsItems.Count.ToString());
            sb.Append("\taccessTime = ").Append(stopwatch.ElapsedMilliseconds).Append(" ms (").Append(stopwatch.ElapsedTicks).AppendLine(")");

            stopwatch.Restart();
            var CraftingResources = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.CraftingResources).GetItems;
            stopwatch.Stop();
            sb.AppendLine("CraftingResources (direct): ");
            sb.Append("\tcount = ").AppendLine(CraftingResources.Count.ToString());
            sb.Append("\taccessTime = ").Append(stopwatch.ElapsedMilliseconds).Append(" ms (").Append(stopwatch.ElapsedTicks).AppendLine(")");

            XtraMessageBox.Show(sb.ToString());
#endif

#if Test_UCCQuesterCheck_ConditionXml
#if false
            UCCQuesterCheck c = new UCCQuesterCheck();
            c.Condition = new Astral.Quester.Classes.Conditions.PartyStatus() { Type = Astral.Quester.Classes.Conditions.PartyStatus.PartyStatusType.IsInAParty };
            string xml = c.ConditionXml;
            bool test = ((ICustomUCCCondition)c).IsOK(null);
            XtraMessageBox.Show(xml + Environment.NewLine + test);

            Astral.Functions.XmlSerializer.Serialize("UCCQuesterCheck.xml", c, 2);
#else
            UCCQuesterCheck c = Astral.Functions.XmlSerializer.Deserialize<UCCQuesterCheck>("UCCQuesterCheck.xml", true, 2);
            string xml = c.ConditionXml;
            bool test = ((ICustomUCCCondition)c).IsOK(null);
            XtraMessageBox.Show(xml + Environment.NewLine + test);
#endif
#endif
            #endregion

#if UCCQuesterCheck_Serialization
            UCCQuesterCheck c = new UCCQuesterCheck() { Condition = new Astral.Quester.Classes.Conditions.PartyStatus() { Type = Astral.Quester.Classes.Conditions.PartyStatus.PartyStatusType.IsNotInAParty } };
            bool result = c.Condition.IsValid;
            XtraMessageBox.Show(result.ToString());
            Astral.Functions.XmlSerializer.Serialize("UCCQuesterCheck.xml", c, 1);
#endif

#if BagsList_IXmlSerializable
#if Serialization
            BagsList bags = new BagsList(BagsList.FullInventory);
            //XtraMessageBox.Show(bags.ToString());

            XmlSerializer serializer = new XmlSerializer(bags.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, bags);
                XtraMessageBox.Show(writer.ToString());
            }

            Astral.Functions.XmlSerializer.Serialize("BagsList.xml", bags, 0); 
#endif
#if !Deserialization

#endif
            BagsList bags = Astral.Functions.XmlSerializer.Deserialize<BagsList>("BagsList.xml");
            if (bags != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (InvBagIDs id in bags)
                {
                    sb.AppendLine(id.ToString());
                }
                XtraMessageBox.Show(sb.ToString());
            }
            else XtraMessageBox.Show("Bags is null");
#endif
#if true
            if (Tools.Reflection.AstralAccessors.ItemFilter.IsMatch != null)
                XtraMessageBox.Show("AstralAccessors.ItemFilter.IsMatch is OK!");
            else XtraMessageBox.Show("AstralAccessors.ItemFilter.IsMatch is NULL!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }

        private void btnUccEditor_Click(object sender, EventArgs e)
        {
            Astral.Logic.UCC.Forms.Editor uccEditor = null;
            if (UCCEditorExtensions.GetUccEditor(ref uccEditor))
            {
                uccEditor.Show();
            }
        }

#if старый_экспорт
        private void MainPanel_Load(object sender, EventArgs e)
        {
            //bteMissions.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderMissions, FileTools.defaulFileMissions);
            //bteAuras.Properties.NullValuePrompt = Path.Combine(FileTools.defaulExportFolderAuras, FileTools.defaulFileAuras);
        }

        private void bte_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (sender is DevExpress.XtraEditors.ButtonEdit bte)
            {
                string fileName = string.Empty;

                if (string.IsNullOrEmpty(bte.Text) || (bte.Text.IndexOfAny(Path.GetInvalidPathChars()) != -1))
                {

                    if (bte.Name == bteAuras.Name) fldrBroserDlg.SelectedPath = FileTools.defaultExportFolderAuras;
                    if (bte.Name == bteMissions.Name) fldrBroserDlg.SelectedPath = FileTools.defaultExportFolderMissions;
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
                        if (bte.Name == bteAuras.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaultFileAuras);
                        if (bte.Name == bteMissions.Name) bte.Text = Path.Combine(fldrBroserDlg.SelectedPath, FileTools.defaultFileMissions);
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
                fullFileName = Path.Combine(FileTools.defaultExportFolderAuras, FileTools.defaultFileAuras);
                MessageBox.Show("The specified filename is invalid.\n" +
                                "Auras will be saved in the file:\n" +
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
                fullFileName = Path.Combine(FileTools.defaultExportFolderMissions, FileTools.defaultFileMissions);
                MessageBox.Show("The specified filename is invalid.\n" +
                    "Missions will be saved in the file:\n" +
                    fullFileName, "Caution!", MessageBoxButtons.OK);
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            XmlSerializer serialiser = new XmlSerializer(typeof(MissionsWrapper));
            TextWriter FileStream = new StreamWriter(fullFileName);
            serialiser.Serialize(FileStream, missions);
            FileStream.Close();

            if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(fullFileName);
        }

        private void btnInterfaces_Click(object sender, EventArgs e)
        {
            InterfaceWrapper Interfaces = new InterfaceWrapper();

            string fullFileName = FileTools.ReplaceMask(bteInterfaces.Text);

            if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                fullFileName = Path.Combine(FileTools.defaultExportFolderInterfaces, FileTools.defaultFileInterfaces);
                MessageBox.Show("The specified filename is invalid.\n" +
                    "Interafaces will be saved in the file:\n" +
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

        private void btnStates_Click(object sender, EventArgs e)
        {
            string fullFileName = FileTools.ReplaceMask(bteStates.Text);

            if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                fullFileName = Path.Combine(FileTools.defaultExportFolderStates, FileTools.defaultFileStates);
                MessageBox.Show("The specified filename is invalid.\n" +
                    "Missions info will be saved in the file:\n" +
                    fullFileName, "Caution!", MessageBoxButtons.OK);
            }

            if (!Directory.Exists(Path.GetDirectoryName(fullFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

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

            if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(fullFileName);

        } 
#endif

        private void cbSpellStuckMonitor_CheckedChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.UnstuckSpells.Active = ckbSpellStuckMonitor.Checked;
        }

        private void cbEnchantHelperActivator_CheckedChanged(object sender, EventArgs e)
        {
            if(cbEnchantHelperActivator.Checked)
                EnchantHelper.Start();
            else EnchantHelper.Stop();
        }

        private void btnUiViewer_Click(object sender, EventArgs e)
        {
#if DEVELOPER
            string uiGenId = string.Empty;
        EntityTools.Core.GUIRequest_UIGenId(ref uiGenId);
#endif
        }

        private void btnEntities_Click(object sender, EventArgs e)
        {
#if DEVELOPER
            string pattern = string.Empty;
            EntityNameType nameType = EntityNameType.InternalName;
            ItemFilterStringType strMatch = ItemFilterStringType.Simple;
        EntityTools.Core.GUIRequest_EntityId(ref pattern, ref strMatch, ref nameType);
#endif
        }

        private void btnAuraViewer_Click(object sender, EventArgs e)
        {
#if DEVELOPER
            string auraId = string.Empty;
            EntityTools.Core.GUIRequest_AuraId(ref auraId);
#endif
        }

        private void btnGetMachineId_Click(object sender, EventArgs e)
        {
            var machineid = Memory.MMemory.ReadString(Memory.BaseAdress + 0x2640BD0, Encoding.UTF8, 64);
            lblAccount.Text = $"Account:   @{EntityManager.LocalPlayer.AccountLoginUsername}";
            tbMashingId.Text = machineid;
        }

        private void tbExportFileSelector_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (cbbxExportSelector.SelectedItem is ExportTypes expType)
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
            if (cbbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                fileName = Path.Combine(Astral.Controllers.Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                fileName.Replace(Astral.Controllers.Directories.AstralStartupPath, @".\");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            if (cbbxExportSelector.SelectedItem is ExportTypes expType)
            {

                string fileName = Path.Combine(Astral.Controllers.Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                fileName.Replace(Astral.Controllers.Directories.AstralStartupPath, @".\");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (cbbxExportSelector.SelectedItem is ExportTypes expType)
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

        private void btnShowMapper_Click(object sender, EventArgs e)
        {
            Astral.Quester.Forms.MapperForm.Open();
        }

        private void ckbEnableLogger_CheckedChanged(object sender, EventArgs e)
        {
            gbxLogger.Enabled = ckbEnableLogger.Checked;
            if (ckbEnableLogger.Checked)
                ETLogger.Start();
            else ETLogger.Stop();
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            if(File.Exists(ETLogger.LogFilePath))
                System.Diagnostics.Process.Start(ETLogger.LogFilePath);
        }

        private void btnCheckCore_Click(object sender, EventArgs e)
        {
            if (EntityTools.Core.CheckCore())
            {

                XtraMessageBox.Show($"EntityToolsCore is VALID!\n\rCore hash: {EntityTools.CoreHash}");
                Astral.Logger.WriteLine($"EntityToolsCore is VALID! Core hash: {EntityTools.CoreHash}");
            }
            else
            {
                XtraMessageBox.Show($"EntityToolsCore is INVALID!",//\n\rCore hash: {EntityTools.CoreHash}", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Astral.Logger.WriteLine($"EntityToolsCore is INVALID!\n\rCore hash: {EntityTools.CoreHash}");
            }
        }

        private void ckbDebugMonitor_CheckedChanged(object sender, EventArgs e)
        {
#if DEVELOPER
            if (ckbDebugMonitor.Checked)
                Patches.UCC.Patch_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = ShowMonitor;
            else Patches.UCC.Patch_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = null; 
#endif
        }

        private void ShowMonitor(Entity entity)
        {
            string info = string.Empty;
            if(entity != null)
            {
                info = string.Concat("MostInjuredAlly: ", entity.DebugName, Environment.NewLine, 
                    "--------------------------------------", Environment.NewLine,
                    '\t', nameof(entity.IsPlayer), '=', entity.IsPlayer, Environment.NewLine,
                    '\t', nameof(entity.CombatDistance3), '=', entity.CombatDistance3, Environment.NewLine,
                    '\t', nameof(entity.Character.AttribsBasic.HealthPercent), '=', entity.Character.AttribsBasic.HealthPercent);
            }
            tbDebugMonitorInfo.Text = info;
        }
    }
}
