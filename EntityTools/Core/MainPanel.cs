using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using ACTP0Tools;
using ACTP0Tools.Patches;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using EntityCore.Forms;
using EntityCore.Tools;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Services;
using EntityTools.Tools;
using EntityTools.Tools.Entities;
using EntityTools.Tools.Export;
using EntityTools.Tools.Powers;
using EntityTools.UCC.Editor;
using Microsoft.Win32;
using MyNW.Internals;
using API = Astral.Quester.API;
using QuesterEditor = EntityTools.Quester.Editor.QuesterEditor;
using Task = System.Threading.Tasks.Task;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : BasePanel
    {
        //TODO: Отображать метрики EntityCache и AStar
        public EntityToolsMainPanel() : base("Entity Tools")
        {
            InitializeComponent();

            cbxExportSelector.Properties.Items.AddRange(Enum.GetValues(typeof(ExportTypes)));
            cbxExportSelector.SelectedIndex = 0;

            ckbSpellStuckMonitor.DataBindings.Add(nameof(ckbSpellStuckMonitor.Checked),
                EntityTools.Config.UnstuckSpells,
                nameof(EntityTools.Config.UnstuckSpells.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);

#if DEVELOPER
            // Настройки Mapper'a
            ckbMapperPatch.DataBindings.Add(nameof(ckbMapperPatch.Checked),
                EntityTools.Config.Mapper,
                nameof(EntityTools.Config.Mapper.Patch),
                false, DataSourceUpdateMode.OnPropertyChanged);

#if false
            seMapperWaipointDistance.DataBindings.Add(nameof(seMapperWaipointDistance.Value),
                                        EntityTools.Config.Mapper,
                                        nameof(EntityTools.Config.Mapper.WaypointDistance),
                                        false, DataSourceUpdateMode.OnPropertyChanged);
            seMapperMaxZDif.DataBindings.Add(nameof(seMapperMaxZDif.Value),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.MaxElevationDifference),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            seWaypointEquivalenceDistance.DataBindings.Add(nameof(seWaypointEquivalenceDistance.Value),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.WaypointEquivalenceDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbMapperForceLinkingWaypoint.DataBindings.Add(nameof(ckbMapperForceLinkingWaypoint.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            ckbMapperLinearPath.DataBindings.Add(nameof(ckbMapperLinearPath.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.LinearPath),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
#endif

#if false
            // Настройки EntityToolsLogger
            ckbEnableLogger.DataBindings.Add(nameof(ckbEnableLogger.Checked),
                                                EntityTools.Config.Logger,
                                                nameof(EntityTools.Config.Logger.Active),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            ckbExtendedActionDebugInfo.DataBindings.Add(nameof(ckbExtendedActionDebugInfo.Checked),
                                                EntityTools.Config.Logger,
                                                nameof(EntityTools.Config.Logger.ExtendedActionDebugInfo),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
#else
            ckbETLogger.DataBindings.Add(nameof(ckbETLogger.Checked),
                EntityTools.Config.Logger,
                nameof(EntityTools.Config.Logger.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);
#endif
            #region QuesterProfilePreprocessing
            ckbEnapleQuesterProfilePreprocessing.DataBindings.Add(nameof(ckbEnapleQuesterProfilePreprocessing.Checked),
                    AstralAccessors.Quester.Preprocessor,
                    nameof(AstralAccessors.Quester.Preprocessor.Active),
                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbAutoSavePreprocessedProfile.DataBindings.Add(nameof(ckbAutoSavePreprocessedProfile.Checked),
                AstralAccessors.Quester.Preprocessor,
                nameof(AstralAccessors.Quester.Preprocessor.AutoSave),
                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbAutoSavePreprocessedProfile.DataBindings.Add(nameof(ckbAutoSavePreprocessedProfile.Enabled),
                AstralAccessors.Quester.Preprocessor,
                nameof(AstralAccessors.Quester.Preprocessor.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);


            gridReplacements.DataSource = AstralAccessors.Quester.Preprocessor.Replacement;

            gridReplacements.DataBindings.Add(nameof(gridReplacements.Enabled),
                AstralAccessors.Quester.Preprocessor,
                nameof(AstralAccessors.Quester.Preprocessor.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);
            #endregion


#if false
            // Настройки EntityCache
            editGlobalCacheTime.DataBindings.Add(nameof(editGlobalCacheTime.Value),
                                                EntityTools.Config.EntityCache,
                                                nameof(EntityTools.Config.EntityCache.GlobalCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            editLocalCacheTime.DataBindings.Add(nameof(editLocalCacheTime.Value),
                                                EntityTools.Config.EntityCache,
                                                nameof(EntityTools.Config.EntityCache.LocalCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            editCombatCacheTime.DataBindings.Add(nameof(editCombatCacheTime.Value),
                                                EntityTools.Config.EntityCache,
                                                nameof(EntityTools.Config.EntityCache.CombatCacheTime),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            // Настройки SlideMonitor
            cbxSlideMonitor.DataSource = Enum.GetValues(typeof(SlideMonitorState));

            cbxSlideMonitor.DataBindings.Add(nameof(cbxSlideMonitor.SelectedItem),
                                             EntityTools.Config.SlideMonitor,
                                             nameof(EntityTools.Config.SlideMonitor.State),
                                             false, DataSourceUpdateMode.OnPropertyChanged);

            editSlideTimeout.DataBindings.Add(nameof(editSlideTimeout.Value),
                                                EntityTools.Config.SlideMonitor,
                                                nameof(EntityTools.Config.SlideMonitor.Timeout),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            editSlideFilter.DataBindings.Add(nameof(editSlideFilter.Value),
                                                EntityTools.Config.SlideMonitor,
                                                nameof(EntityTools.Config.SlideMonitor.BoatFilter),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
#endif


#else
            btnEntities.Visible = false;
            btnAuraViewer.Visible = false;
            btnUiViewer.Visible = false;

            tabOptions.PageVisible = false;
            tabRelogger.PageVisible = false;
            tabMapper.PageVisible = false;
            tabDebug.PageVisible = false;
#endif

            pgConfigs.SelectedObject = EntityTools.Config;

#if DEBUG
            tabDebug.PageVisible = true;
            tabDebug.PageEnabled = true;
#else
            tabDebug.PageVisible = false;
            tabDebug.PageEnabled = false;
#endif
        }

        private void handler_Test_1(object sender, EventArgs e)
        {
#if false
            StringBuilder sb = new StringBuilder("Powers:\n");

            for (int i = 0; i < 15; i++)
            {
                var power = Powers.GetPowerBySlot(i);
                if (power != null && power.IsValid)
                {
                    var powDef = power.PowerDef;
                    //sb.Append("\t[").Append(i).Append("]\t").Append(powDef.DisplayName).Append(" (").Append(powDef.InternalName).AppendLine(")");
                    sb.AppendFormat("\t[{0}]\t{1} ({2})\n", i, powDef.DisplayName, powDef.InternalName);
                }
                else
                {
                    //sb.Append("\t[").Append(i).AppendLine("]\tInvalid");
                    sb.AppendFormat("\t[{0}]\tInvalid\n", i);
                }
            }

            sb.AppendLine("------------------");

            var art = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.ArtifactPrimary).GetItems.FirstOrDefault()?.Item;
            if (art != null && art.IsValid)
            {
                var power = art.Powers.FirstOrDefault();
                if (power != null && power.IsValid)
                {
                    var powDef = power.PowerDef;
                    sb.AppendFormat("Artifact\t[{0}]\t{1} ({2})\n", power.TraySlot, powDef.DisplayName, powDef.InternalName);
                }
            }
            var mount = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.MountEquippedActivePower).GetItems.FirstOrDefault()?.Item;
            if (mount != null && mount.IsValid)
            {
                var power = mount.Powers.FirstOrDefault();
                if (power != null && power.IsValid)
                {
                    var powDef = power.PowerDef;
                    sb.AppendFormat("Mount\t[{0}]\t{1} ({2})\n", power.TraySlot, powDef.DisplayName, powDef.InternalName);
                }
            }

            XtraMessageBox.Show(sb.ToString()); 
#else
            var id = tbText.Text;
            var slot = EntityManager.LocalPlayer.AllItems.FirstOrDefault(s =>
                s.Filled && s.Item.ItemDef.InternalName == id);
            if (slot!= null)
                XtraMessageBox.Show($@"ItemLevel: {slot.Item.ItemProgression_GetItemLevel()}");
            else XtraMessageBox.Show($@"No item '{id}' found");
#endif
        }

        private void handler_Test_2(object sender, EventArgs e)
        {
#if false
            Traverse aoeList = Traverse.Create(typeof(AOECheck)).Property("List");
            if (!aoeList.PropertyExists())
            {
                XtraMessageBox.Show("AOE list does not accessible");
                return;
            }

            if (aoeList.GetValue() is IEnumerable<AOECheck.AOE> enumerable)
            {
                var sb = new StringBuilder();
                foreach (var aoe in enumerable)
                {
                    sb.AppendLine(aoe.ID);
                }

                XtraMessageBox.Show(sb.ToString());
            }
            else XtraMessageBox.Show(@"Can't enumerate <AOECheck.AOE> in 'aoeList'");
#elif false
            var type = typeof(AOECheck.AOE);
            var list = Traverse.Create(typeof(AOECheck)).Property("List");
            if (list.GetValue() is List<AOECheck.AOE> aoeList)
            {
                var sb = new StringBuilder();
                foreach (var aoe in aoeList)
                {
                    sb.AppendLine(aoe.ID);
                }

                XtraMessageBox.Show(sb.ToString());
            }
            XtraMessageBox.Show(type.ToString());

#elif false
            var aoeList = Traverse.Create(typeof(AOECheck)).Property("List").GetValue();

            //var iterator = aoeList.GetValue<List<AOECheck.AOE>>().GetEnumerator();
            //try
            //{
            //    while (iterator.MoveNext())
            //    {
            //        var item = iterator.Current;
            //    }
            //}
            //finally
            //{
            //    iterator.Dispose();
            //}

            var aoeList_GetEnumerator = Traverse.Create(aoeList).Method("GetEnumerator");
            var enumeratorObj = aoeList_GetEnumerator.GetValue();
            var enumerator = Traverse.Create(enumeratorObj);
            var enumerator_MoveNext = enumerator.Method("MoveNext");
            var enumerator_Dispose = enumerator.Method("Dispose");

            var aoeType = typeof(AOECheck.AOE);
            var ID = aoeType.GetProperty<string>(nameof(AOECheck.AOE.ID));
            try
            {
                if (enumerator_MoveNext.MethodExists())
                {
                    var enumerator_Current = enumerator.Property("Current");
                    if (enumerator_Current.PropertyExists())
                    {
                        var sb = new StringBuilder();
                        while (enumerator_MoveNext.GetValue<bool>())
                        {
                            var aoe = enumerator_Current.GetValue();
                            sb.AppendLine(ID[aoe]);
                        }

                        XtraMessageBox.Show(sb.ToString());
                    }
                }
            }
            finally
            {
                if (enumerator_Dispose.MethodExists())
                    enumerator_Dispose.GetValue();
            }
#elif false
            var slot = EntityManager.LocalPlayer.GetInventoryBagById(InvBagIDs.Inventory).Slots
                .FirstOrDefault(s => s.Filled && s.Item.ItemDef.InternalName == "T1_Enchantment_Tutorial");
            slot?.Evolve();
#elif true
            var pwr = Powers.GetPowerByInternalName("M19_Instance_Fpower_Summon_Lulu");
            if (pwr != null && pwr.IsValid)
            {
                var trg = Astral.Logic.UCC.Core.CurrentTarget;
                if (trg != null && trg.IsValid)
                {
                    Powers.ExecPower(pwr, trg, true);
                    Thread.Sleep(500);
                    Powers.ExecPower(pwr, trg, false);
                }
            }
            else
            {
                XtraMessageBox.Show("Power does not found");
            }
#endif
        }

        private void handler_Test_3(object sender, EventArgs e)
        {
#if false
            StaticPropertyAccessor<List<AOECheck.AOE>> aoeList =
                    typeof(AOECheck).GetStaticProperty<List<AOECheck.AOE>>("List");
            if (!aoeList.IsValid)
            {
                XtraMessageBox.Show("AOE list does not accessible");
                return;
            }

            var sb = new StringBuilder();
            foreach (var aoe in aoeList.Value)
            {
                sb.AppendLine(aoe.ID);
            }

            XtraMessageBox.Show(sb.ToString());
#elif false
            var aoeType = typeof(AOECheck.AOE);
            var aoeList = Traverse.Create(typeof(AOECheck)).Property("List");
            Type aoeListType = aoeList.GetValue()?.GetType();
            var listType = typeof(List<>); //aoeListType.GetGenericTypeDefinition();
            var aoeListType1 = listType.MakeGenericType(aoeType);

            bool isEqualType = aoeListType == aoeListType1;

            XtraMessageBox.Show($"{aoeType}\n" +
                                $"{aoeListType}\n" +
                                $"{aoeListType1}\n" +
                                $"{listType}\n" +
                                $"{isEqualType}");
#elif false
            var changeWPDistance = API.Engine.Navigation.GetProperty<double>("ChangeWPDist");

            if (!changeWPDistance.IsValid)
            {
                XtraMessageBox.Show($"Не удалось получить доступ к полю 'Navigation.ChangeWPDist'");
                return;
            }

            var currentChangeWPDistance = changeWPDistance.Value;

            XtraMessageBox.Show($"Текущее значение 'Navigation.ChangeWPDist' равно {currentChangeWPDistance}");
#elif false
            var pwr = Powers.GetPowerBySlot(22);
            if (pwr != null && pwr.IsValid)
            {
                var trg = Astral.Logic.UCC.Core.CurrentTarget;
                if (trg != null && trg.IsValid)
                {
                    Powers.ExecPower(pwr, trg, true);
                    Thread.Sleep(500);
                    Powers.ExecPower(pwr, trg, false);
                }
            }
            else
            {
                XtraMessageBox.Show("Power does not slotted");
            }
#elif false
            //var editor = TypeDescriptor.GetEditor(typeof(CustomRegionCollection), typeof(UITypeEditor));
            ////EntityTools.Core.CheckCore();
            ////var editor2 = TypeDescriptor.GetEditor(typeof(CustomRegionCollection), typeof(UITypeEditor));
            //var properties = TypeDescriptor.GetProperties(typeof(IsInCustomRegionSet));
            //var p = properties[nameof(IsInCustomRegionSet.CustomRegions)];
            //var editorAttribute = p.Attributes[typeof(EditorAttribute)];
            //XtraMessageBox.Show($"{editor?.GetType().FullName}\n{editorAttribute.}");

            ViewModelDecorator<IsInCustomRegionSet> CustomRegionCollectionDecorator = new ViewModelDecorator<IsInCustomRegionSet>((
                    typeof(CustomRegionCollection),
                    new[] { new EditorAttribute(typeof(CustomRegionCollectionEditor), typeof(UITypeEditor)) }));

            var cond = new IsInCustomRegionSet();
            var typeDef = CustomRegionCollectionDecorator.Decorate(cond);

            var editor1 = TypeDescriptor.GetEditor(cond.CustomRegions, typeof(UITypeEditor));
            var editor2 = typeDef.GetProperties()[nameof(cond.CustomRegions)].GetEditor(typeof(UITypeEditor));

            XtraMessageBox.Show($"{editor1?.GetType().FullName}\n{editor2?.GetType().FullName}");
#elif false
            //var cond = new IsInCustomRegionSet();
            var condType = typeof(IsInCustomRegionSet);//cond.GetType();
            // prepare our property overriding type descriptor
            PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(condType).GetTypeDescriptor(condType));
            // iterate through properies in the supplied object/type
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(condType)) {
                // for every property that complies to our criteria
                if (pd.PropertyType ==  typeof(CustomRegionCollection) ) {
                    // we first construct the custom PropertyDescriptor with the TypeDescriptor's built-in capabilities
                    PropertyDescriptor pd2 = TypeDescriptor.CreateProperty(
                        condType, // or just _settings, if it's already a type
                        pd,                  // base property descriptor to which we want to add attributes
                        // The PropertyDescriptor which we'll get will just wrap that
                        // base one returning attributes we need.
                        new EditorAttribute( // the attribute in question
                            typeof(CustomRegionCollectionEditor),
                            typeof(System.Drawing.Design.UITypeEditor)
                        )
                        // this method really can take as many attributes as you like, not just one
                    );
                
                    // and then we tell our new PropertyOverridingTypeDescriptor to override that property
                    ctd.OverrideProperty(pd2);
                }
            }
            
            // then we add new descriptor provider that will return our descriptor instead of default
            TypeDescriptor.AddProvider(new TypeDescriptorOverridingProvider(ctd), condType);

            propertyGrid.SelectedObject = new IsInCustomRegionSet(); //cond;
#elif false
            // Декорирование свойств типов для вызова корректного редактора
            var tCustomRegionCollection = typeof(CustomRegionCollection);
            var tCustomRegionCollectionEditor = typeof(CustomRegionCollectionEditor);
            var tUITypeEditor = typeof(UITypeEditor);
            //var editorAttribute = new EditorAttribute(typeof(CustomRegionSetEditor),
            //                                         typeof(UITypeEditor));
            foreach (Type type in ACTP0Serializer.QuesterTypes)
            {
                bool shouldOverrideProperty = false;
                PropertyOverridingTypeDescriptor ctd = new PropertyOverridingTypeDescriptor(TypeDescriptor.GetProvider(type).GetTypeDescriptor(type));
                // iterate through properties in the supplied object/type
                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(type))
                {
                    // for every property that complies to our criteria
                    if (pd.PropertyType == tCustomRegionCollection)
                    {
                        // we first construct the custom PropertyDescriptor with the TypeDescriptor's built-in capabilities
                        var newPD = TypeDescriptor.CreateProperty(
                                tCustomRegionCollection, // or just _settings, if it's already a type
                                pd,                      // base property descriptor to which we want to add attributes
                                                         // The PropertyDescriptor which we'll get will just wrap that
                                                         // base one returning attributes we need.
                                new EditorAttribute(tCustomRegionCollectionEditor,tUITypeEditor));

                        // and then we tell our new PropertyOverridingTypeDescriptor to override that property
                        ctd.OverrideProperty(newPD);
                        shouldOverrideProperty = true;
                    }
                }

                // then we add new descriptor provider that will return our descriptor instead of default
                if (shouldOverrideProperty)
                {
                    var descriptor = new TypeDescriptorOverridingProvider(ctd);
                    descriptorProvider.Add(descriptor);
                    TypeDescriptor.AddProvider(descriptor, type);
                }
            }
#endif
        }
        private static readonly List<TypeDescriptionProvider> descriptorProvider = new List<TypeDescriptionProvider>();

        private void handler_Test_4(object sender, EventArgs e)
        {
            int hash = "str".GetHashCode();
        }

        private void handler_SpellStuckMonitorActivation(object sender, EventArgs e)
        {
            EntityTools.Config.UnstuckSpells.Active = ckbSpellStuckMonitor.Checked;
        }

        private void handler_EnchantHelperActivation(object sender, EventArgs e)
        {
            if(cbEnchantHelperActivator.Checked)
                EnchantHelper.Start();
            else EnchantHelper.Stop();
        }

        private void handler_OpenUiViewer(object sender, EventArgs e)
        {
#if DEVELOPER
            string uiGenId = UIViewer.GUIRequest(string.Empty);
            if(!string.IsNullOrEmpty(uiGenId))
                Clipboard.SetText(uiGenId);
#endif
        }

        private void handler_OpenEntitiesViewer(object sender, EventArgs e)
        {
#if DEVELOPER
            string pattern = string.Empty;
            EntityNameType nameType = EntityNameType.InternalName;
            ItemFilterStringType strMatch = ItemFilterStringType.Simple;
            EntityViewer.GUIRequest(ref pattern, ref strMatch, ref nameType);
            if (!string.IsNullOrEmpty(pattern))
                Clipboard.SetText(pattern);
#endif
        }

        private void handler_OpenMissionMonitor(object sender, EventArgs e)
        {
#if DEVELOPER
#if false
            var missMonitor = new MissionMonitorForm2();
            missMonitor.Show();  
#else
            var missMonitor = new MissionMonitorForm();
            missMonitor.Show();
#endif
#endif
        }

        private void handler_OpenAuraViewer(object sender, EventArgs e)
        {
#if DEVELOPER
            string auraId = AuraViewer.GUIRequest();
            if (!string.IsNullOrEmpty(auraId))
                Clipboard.SetText(auraId);
#endif
        }

        private void handler_ChangeExportingFileName(object sender, ButtonPressedEventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                switch (e.Button.Kind)
                {
                    case ButtonPredefines.Undo:
                    {
                        string fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                        fileName = fileName.Replace(Directories.AstralStartupPath, ".");
                        tbExportFileSelector.Text = fileName;
                        break;
                    }
                    case ButtonPredefines.Ellipsis:
                    {
                        string fileName;
                        if (string.IsNullOrEmpty(tbExportFileSelector.Text))
                        {
                            fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                            fileName = fileName.Replace(Directories.AstralStartupPath, ".");
                        }
                        else fileName = tbExportFileSelector.Text;

                        string directory = Path.GetDirectoryName(fileName);
                        if (!string.IsNullOrEmpty(directory)
                            && !Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        dlgSaveFile.InitialDirectory = directory;

                        dlgSaveFile.FileName = fileName;
                        if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                            tbExportFileSelector.Text = dlgSaveFile.FileName;
                        break;
                    }
                }
            }
        }

        private void handler_ChangeExportingData(object sender, EventArgs e)
        {
            if (sender == cbxExportSelector
                && cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                fileName = fileName.Replace(Directories.AstralStartupPath, ".");
                tbExportFileSelector.Text = fileName;
                cbxExportSelector.SelectedItem = expType;
            }
        }

        private void handler_ResetExportSettings(object sender, EventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {

                string fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                fileName = fileName.Replace(Directories.AstralStartupPath, ".");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void handler_Export(object sender, EventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fullFileName = FileTools.ReplaceMask(tbExportFileSelector.Text);

                if (string.IsNullOrEmpty(fullFileName) || fullFileName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                {
                    fullFileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.ReplaceMask(FileTools.DefaultExportFileName));
                    XtraMessageBox.Show("The specified filename is incorrect.\n" +
                                        $"{expType} will be saved in the file:\n" +
                                        fullFileName, "Caution!", MessageBoxButtons.OK);
                }

                var dirName = Path.GetDirectoryName(fullFileName);
                if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                switch (expType)
                {
                    case ExportTypes.Auras:
                        {
                            var auras = new AurasWrapper(EntityManager.LocalPlayer.Character);

                            var serializer = new XmlSerializer(auras.GetType());
                            using (var fileStream = new StreamWriter(fullFileName))
                                serializer.Serialize(fileStream, auras);
                            break;
                        }
                    case ExportTypes.Interfaces:
                        {
                            var interfaces = new InterfaceWrapper();

                            var serializer = new XmlSerializer(interfaces.GetType());
                            using (var fileStream = new StreamWriter(fullFileName))
                                serializer.Serialize(fileStream, interfaces);
                            break;
                        }
                    case ExportTypes.Missions:
                        {
                            var missions = new MissionsWrapper(EntityManager.LocalPlayer);

                            var serializer = new XmlSerializer(missions.GetType());
                            using (var fileStream = new StreamWriter(fullFileName))
                                serializer.Serialize(fileStream, missions);
                            break;
                        }
                    case ExportTypes.States:
                        {
                            using (var sw = new StreamWriter(fullFileName, false, Encoding.Default))
                            {
                                sw.WriteLine($"Character: {EntityManager.LocalPlayer.InternalName}");
                                sw.WriteLine($"DateTime: {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
                                sw.WriteLine();
                                foreach (State state in API.Engine.States)
                                {
                                    sw.WriteLine($"{state.DisplayName} {state.Priority}");
                                    sw.WriteLine($"\t{state.GetType().FullName}");
                                }
                            }
                            break;
                        }
                    case ExportTypes.Powers:
                    {
                        var powers = new PowersWrapper();

                        var serializer = new XmlSerializer(powers.GetType());
                        using (var fileStream = new StreamWriter(fullFileName))
                            serializer.Serialize(fileStream, powers);
                        break;
                    }
                }

                if (XtraMessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?",
                    $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start(fullFileName);
                }
            }
        }

        private void handler_EnableLogger(object sender, EventArgs e)
        {
            if (ckbETLogger.Checked)
                ETLogger.Start();
            else ETLogger.Stop();
        }

        private void handler_OpenLogFile(object sender, EventArgs e)
        {
            if(File.Exists(ETLogger.LogFilePath))
                Process.Start(ETLogger.LogFilePath);
        }
        
        private void handler_DebugMonitorActivate(object sender, EventArgs e)
        {
#if BLAttackersListMonitor
            if (ckbDebugMonitor.Checked && BLAttackersList != null)
                backgroundWorker.RunWorkerAsync();
            else backgroundWorker.CancelAsync(); 
#elif true
            if (ckbDebugMonitor.Checked)
                backgroundWorker.RunWorkerAsync();
            else backgroundWorker.CancelAsync();
#endif
        }

#if ShowMostInjuredAlly
        private void ShowMostInjuredAlly(Entity entity)
        {
            string info = string.Empty;
            if (entity != null)
            {
                info = string.Concat("MostInjuredAlly: ", entity.DebugName, Environment.NewLine,
                    "--------------------------------------", Environment.NewLine,
                    '\t', nameof(entity.IsPlayer), '=', entity.IsPlayer, Environment.NewLine,
                    '\t', nameof(entity.CombatDistance3), '=', entity.CombatDistance3, Environment.NewLine,
                    '\t', nameof(entity.Character.AttribsBasic.HealthPercent), '=', entity.Character.AttribsBasic.HealthPercent);
            }
            tbDebugMonitorInfo.Text = info;
        } 
#endif

        private void work_PowerSearch(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            IPowerCache powCache = new PowerCache("M19_Instance_Fpower_Summon_Lulu");

            tbDebugMonitorInfo.Text = $"[{DateTime.Now:HH:mm:ss.ffff}] Start searching the power 'M19_Instance_Fpower_Summon_Lulu'\n";
            while (!backgroundWorker.CancellationPending)
            {
                var power = powCache.GetPower();

                string info = string.Concat('[', DateTime.Now.ToString("HH:mm:ss.ffff"), "] Power ",
                    power != null && power.IsValid
                        ? power.PowerDef.FullDisplayName + "[Slot=" + power.GetTraySlot() + ", Id=" + power.PowerId + "]"
                        : "not found", Environment.NewLine);
                tbDebugMonitorInfo.AppendText(info);

                Thread.Sleep(550);
            } 
        }

#if BLAttackersListMonitor
        StaticFieldAccessor<Func<List<string>>> BLAttackersList = typeof(Astral.Logic.NW.Combats).GetStaticField<Func<List<string>>>("BLAttackersList");
        private void work_BlackList(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!BLAttackersList.IsValid
                || BLAttackersList.Value is null)
                return;

            while (!backgroundWorker.CancellationPending)
            {
                var list = BLAttackersList.Value();
                if (list?.Count > 0)
                    tbDebugMonitorInfo.Lines = list.ToArray();
                else tbDebugMonitorInfo.Text = "FoeBlackList is empty";
                Thread.Sleep(500);
            }
        } 
#endif

        private void handler_Up(object sender, EventArgs e)
        {
            if (int.TryParse(tbText.Text, out int time))
                Task.Run(() => {
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                    Thread.Sleep(time);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
                });
            else MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
        }

        private void handler_Down(object sender, EventArgs e)
        {
            if (int.TryParse(tbText.Text, out int time))
                Task.Run(() => {
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
                    Thread.Sleep(time);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
                });
            else MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
        }

        private void handler_Left(object sender, EventArgs e)
        {
            if (int.TryParse(tbText.Text, out int time))
                Task.Run(() => {
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
                    Thread.Sleep(time);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
                });
            else MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
        }

        private void handler_Right(object sender, EventArgs e)
        {
            if (int.TryParse(tbText.Text, out int time))
                Task.Run(() => {
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
                    Thread.Sleep(time);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
                });
            else MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
        }

        private void handler_Jump(object sender, EventArgs e)
        {
            if (int.TryParse(tbText.Text, out int time))
                Task.Run(() => {
                    MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                    Thread.Sleep(time);
                    MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
                });
            else MyNW.Internals.Movements.SetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
        }

        private void handler_Stop(object sender, EventArgs e)
        {
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveForward);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.Jump);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.MoveBackward);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeLeft);
            MyNW.Internals.Movements.UnsetMovementType(MyNW.Internals.Movements.MovementsType.StrafeRight);
        }

        private void handler_Interact(object sender, EventArgs e)
        {
        }

        private void handler_SaveSettings(object sender, EventArgs e)
        {
            EntityTools.SaveSettings();
        }

        private void handler_GetMachineId(object sender, EventArgs e)
        {
#if false
            var machineid = Memory.MMemory.ReadString(Memory.BaseAdress + 0x2640BD0, Encoding.UTF8, 64);
            lblAccount.Text = $@"Account:   @{EntityManager.LocalPlayer.AccountLoginUsername}";
            tbMashineId.Text = machineid; 
#endif
            using (var crypticCoreKey = Registry.CurrentUser.OpenSubKey(@"Software\Cryptic\Core"))
            {
                if (crypticCoreKey != null)
                {
                    var machineId = crypticCoreKey.GetValue("machineId");
                    tbMashineId.Text = machineId.ToString();
                }
            }
            using (var crypticLauncherKey = Registry.CurrentUser.OpenSubKey(@"Software\Cryptic\Cryptic Launcher"))
            {
                if (crypticLauncherKey != null)
                {
                    var userName = crypticLauncherKey.GetValue("UserName");
                    lblAccount.Text = userName.ToString();
                }
            }
        }

        private void handler_SetMachineId(object sender, EventArgs e)
        {

        }

        private void handler_OpenCredentialManager(object sender, EventArgs e)
        {
            
        }

        private void handler_TeamMonitor(object sender, EventArgs e)
        {
            ObjectInfoForm.Show(new PlayerTeamHelper.Monitor());
        }

        private void handler_AddProcessingItem(object sender, EventArgs e)
        {
#if true
            var item = new AstralAccessors.Quester.ReplacementItem();
            AstralAccessors.Quester.Preprocessor.Replacement.Add(item);
#else
            gridViewPreprocessing.AddNewRow();
#endif
        }

        private void handler_DeleteProcessingItem(object sender, EventArgs e)
        {
#if false
            if (gridPreprocessingProfile.FocusedView is GridView gridView)
            {
                if (gridView.FocusedValue is ReplacementItem item)
                {
                    gridView.DeleteSelectedRows();
                }
            } 
#else
            gridViewPreprocessing.DeleteSelectedRows(); 
#endif
        }

        private void handler_ClearProcessingItems(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show($"Are you sure to to delete all items ?",
                                    "Clear Items", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                AstralAccessors.Quester.Preprocessor.Replacement.Clear();
            }
        }

        private void handler_TestProcessingItem(object sender, EventArgs e)
        {
#if false
            if (gridViewPreprocessing.GetFocusedRow() is ReplacementItem item)
            {
                if (!item.IsValid)
                {
                    XtraMessageBox.Show($"Selected processing Item is invalid",
                        "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var testdata = new[]
                {
                    "AdventuringHead",
                    "AdventuringTrousers",
                    "AdventuringRings",
                    "AdventuringHands"
                };

                var sb = new StringBuilder("Processing result: \n\t");
                sb.AppendLine($"({item.Type}){item.Pattern} => {item.Replacement}\n");

                foreach (var s in testdata)
                {
                    sb.AppendLine(item.Replace(s, out string output)
                        ? $"\t{s} => {output}"
                        : $"\t{s}");
                }

                XtraMessageBox.Show(sb.ToString());
            } 
#else
            var replacement = AstralAccessors.Quester.Preprocessor.Replacement;

            if(replacement.Count == 0)
                return;

            OpenFileDialog openDialog = ACTP0Tools.Classes.FileTools.GetOpenDialog( filter: @"Astral mission profile (*.amp.zip)|*.amp.zip",
                                                                                    defaultExtension: "amp.zip",
                                                                                    initialDir: Directories.ProfilesPath );

            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            var profilePath = openDialog.FileName;

            using (var zipFile = ZipFile.Open(profilePath, ZipArchiveMode.Read))
            {
                try
                {
                    ZipArchiveEntry zipProfileEntry = zipFile.GetEntry("profile.xml");
                    if (zipProfileEntry is null)
                    {
                        Astral.Logger.Notify($"File '{Path.GetFileName(profilePath)}' does not contain 'profile.xml'");
                        return;
                    }

                    using (var stream = zipProfileEntry.Open())
                    {
                        ACTP0Serializer.GetExtraTypes(out List<Type> types, 2);

                        int lineNum = -1;
                        int matchNum = 0;
                        var report = new StringBuilder($"Reading profile {profilePath}\n");
                        var modifiedProfile = new StringBuilder();

                        using (var reader = new StreamReader(stream))
                        {
                            string inLine;
                            while ((inLine = reader.ReadLine()) != null)
                            {
                                var line = inLine;
                                lineNum++;
                                int lineChanges = 0;
                                foreach (var item in replacement)
                                {
                                    if (item.Replace(line, out string outStr))
                                    {
                                        matchNum++;
                                        lineChanges++;
                                        line = outStr;
                                    }
                                }

                                if (lineChanges > 0)
                                {
                                    report.AppendFormat("[{0:########}] line '{1}'\n", lineNum, inLine.Trim());
                                    report.Append("\t=> '").Append(line.Trim()).AppendLine("'");
                                }

                                modifiedProfile.AppendLine(inLine);
                            }
                        }
                        if (matchNum > 0)
                        {
                            report.Append(matchNum).AppendLine(" modifications are done");

                            var reportPath = (profilePath.EndsWith(".amp.zip")
                                                   ? profilePath.Substring(0, profilePath.Length - 8)
                                                   : profilePath)
                                + DateTime.Now.ToString(@"_yyyy-MM-dd_HH-mm\.\l\o\g");

                            File.WriteAllText(reportPath, report.ToString());

                            if (XtraMessageBox.Show($"There are {matchNum} modifications in the profile '{Path.GetFileName(profilePath)}'.\n" +
                                                    "Would you like to open detail report", "Preprocessing result", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                if (File.Exists(reportPath))
                                {
                                    Process.Start(reportPath);
                                } 
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                }
                catch (Exception ex)
                {
                    Logger.Notify(ex.ToString(), true);
                    //XtraMessageBox.Show(ex.ToString());
                }
            }
#endif
        }

        private void handler_Help(object sender, EventArgs e)
        {
            XtraMessageBox.Show("Пример:\n" +
                                "\tType := Regex\n" +
                                "\tPattern := \\bAdventuring(?!Hands|Trousers|Rings)(?<name>\\w+)\\b\n" +
                                "\tReplacement := ${name}\n" +
                                "Регулярное выражение \"Pattern\" интерпретируется следующим образом:\n" +
                                "Алфавитно цифровая последовательность должна начинаться словом \"Adventuring\",\n" +
                                "при этом ему должен предшествовать любой символ не относящийся к алфавитно-цифровым.\n" +
                                "Следом не должны встречаться подстроки \"Hands\", \"Trousers\" или \"Rings\" (без пробелов или иных разделителе).\n" +
                                "\n" +
                                "Последовательность должна заканчивается группой из одного и более алфавитно-цифровых символов \"\\w+\",\n" +
                                "которой присваивается имя 'name'. Эта группа будет использоваться при формировании новой строки.\n" +
                                "\n" +
                                "Выражение подстановки \"${name}\" при обработке будет заменено на захваченную именованную группу символов \"name\"\n" +
                                "\n" +
                                "Таким образом, выражение \"AdventuringHead\" будет преобразовано в \"Head\",\n" +
                                "тогда как выражение \"AdventuringTrousers\" останется неизменным, поскольку содержит запрещенную подстроку \"Trousers\"",
                                "Пример", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void handler_ExportPreprocessingProfile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directories.SettingsPath,
                DefaultExt = "xml",
                Filter = "Replacements (*.xml)|*.xml",
                FileName = "Replacements.xml"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var items = AstralAccessors.Quester.Preprocessor.Replacement;
                XmlSerializer serializer = new XmlSerializer(items.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serializer.Serialize(file, items);
                }
            }
        }

        private void handler_ImportPreprocessingProfile(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = ACTP0Tools.Classes.FileTools.GetOpenDialog(filter: "Replacements (*.xml)|*.xml",
                                                                                   defaultExtension: "xml",
                                                                                   initialDir: Directories.SettingsPath);
                
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                List<AstralAccessors.Quester.ReplacementItem> items = null;

                XmlSerializer serialiser = new XmlSerializer(typeof(List<AstralAccessors.Quester.ReplacementItem>));
                using (StreamReader fileStream = new StreamReader(openDialog.FileName))
                {
                    if (serialiser.Deserialize(fileStream) is List<AstralAccessors.Quester.ReplacementItem> list)
                    {
                        items = list;
                    }
                }
                if (items?.Count > 0)
                {
                    var replacements = AstralAccessors.Quester.Preprocessor.Replacement;
                    gridReplacements.BeginUpdate();
                    if (replacements.Count > 0)
                    {
                        switch (XtraMessageBox.Show($"The 'Preprocessing List' has {replacements.Count} items.\n" +
                                                    $"\tPress 'Yes' to add item to the existing list\n" +
                                                    $"\tPress 'No' to replace they.", "Import Preprocessing List", MessageBoxButtons.YesNoCancel))
                        {
                            case DialogResult.Yes: // Добавление к существующему списку
                                foreach (var item in items)
                                    replacements.Add(item);
                                return;
                            case DialogResult.No: // Замена существующего списка
                                replacements.Clear();
                                foreach (var item in items)
                                    if(!replacements.Contains(item))
                                        replacements.Add(item);
                                return;
                            default:
                                return;
                        }
                    }

                    foreach (var item in items)
                        if (!replacements.Contains(item))
                            replacements.Add(item);
                    gridReplacements.EndUpdate();
                }
                else
                {
                    XtraMessageBox.Show("Empty or file opening error.");
                }
            }
        }

        private void handler_SaveQuesterProfilePreprocessor(object sender, EventArgs e)
        {
            AstralAccessors.Quester.SavePreprocessor();
        }

        private void handler_EntityCacheMonitor(object sender, EventArgs e)
        {
            CollectionInfoForm.Show(() => SearchCached.EntityCache);
        }

        private void handler_EditUcc(object sender, EventArgs e)
        {
            UccEditor.Edit(Astral.Logic.UCC.API.CurrentProfile, Astral.API.CurrentSettings.LastUCCProfile);
        }

        private void handler_EditQuester(object sender, EventArgs e)
        {
            var profile = Astral.Quester.API.CurrentProfile;
            var profileName = Astral.API.CurrentSettings.LastQuesterProfile;

            if (profile.Saved && !string.IsNullOrEmpty(profileName))
                QuesterEditor.Edit(profile, profileName);
            else QuesterEditor.Edit(profile);
        }
    }
}
