using AcTp0Tools.Reflection;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Services;
using EntityTools.Tools;
using EntityTools.Tools.Export;
using EntityTools.Tools.Targeting;
using HarmonyLib;
using Microsoft.Win32;
using MyNW.Classes;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using AcTp0Tools;
using API = Astral.Quester.API;
using Task = System.Threading.Tasks.Task;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : /* UserControl //*/ BasePanel
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
        }

        private void handler_Test_1(object sender, EventArgs e)
        {
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

#else
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
#else
            var changeWPDistance = API.Engine.Navigation.GetProperty<double>("ChangeWPDist");

            if (!changeWPDistance.IsValid)
            {
                XtraMessageBox.Show($"Не удалось получить доступ к полю 'Navigation.ChangeWPDist'");
                return;
            }

            var currentChangeWPDistance = changeWPDistance.Value;

            XtraMessageBox.Show($"Текущее значение 'Navigation.ChangeWPDist' равно {currentChangeWPDistance}");
#endif
        }

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
            string uiGenId = string.Empty;
            EntityTools.Core.UserRequest_SelectUIGenId(ref uiGenId);
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
            EntityTools.Core.UserRequest_EditEntityId(ref pattern, ref strMatch, ref nameType);
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
            string auraId = string.Empty;
            EntityTools.Core.UserRequest_SelectAuraId(ref auraId);
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
#if false
                    MessageBox.Show("The specified filename is incorrect.\n" +
                                                $"{expType} will be saved in the file:\n" +
                                                fullFileName, "Caution!", MessageBoxButtons.OK); 
#else
                    XtraMessageBox.Show("The specified filename is incorrect.\n" +
                                        $"{expType} will be saved in the file:\n" +
                                        fullFileName, "Caution!", MessageBoxButtons.OK);
#endif
                }

                var dirName = Path.GetDirectoryName(fullFileName);
                if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                switch (expType)
                {
                    case ExportTypes.Auras:
                        {
                            AurasWrapper auras = new AurasWrapper(EntityManager.LocalPlayer.Character);

                            var serializer = new XmlSerializer(typeof(AurasWrapper));
                            using (var fileStream = new StreamWriter(fullFileName))
                                serializer.Serialize(fileStream, auras);
                            break;
                        }
                    case ExportTypes.Interfaces:
                        {
                            var interfaces = new InterfaceWrapper();

                            var serializer = new XmlSerializer(typeof(InterfaceWrapper));
                            using (var fileStream = new StreamWriter(fullFileName))
                                serializer.Serialize(fileStream, interfaces);
                            break;
                        }
                    case ExportTypes.Missions:
                        {
                            MissionsWrapper missions = new MissionsWrapper(EntityManager.LocalPlayer);

                            var serializer = new XmlSerializer(typeof(MissionsWrapper));
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
                }

#if false
                if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?",
                            $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes) 
#else
                if (XtraMessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?",
                    $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
#endif
                    Process.Start(fullFileName);
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

        private void handler_Validate(object sender, EventArgs e)
        {
            if (EntityTools.Core.CheckCore())
            {

                XtraMessageBox.Show($"EntityToolsCore is VALID!\n\rCore hash: {EntityTools.CoreHash}");
                Logger.WriteLine($"EntityToolsCore is VALID! Core hash: {EntityTools.CoreHash}");
            }
            else
            {
                XtraMessageBox.Show("EntityToolsCore is INVALID!",//\n\rCore hash: {EntityTools.CoreHash}", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Logger.WriteLine($"EntityToolsCore is INVALID!\n\r");
            }
        }

        private void handler_DebugMonitorActivate(object sender, EventArgs e)
        {
            if(ckbDebugMonitor.Checked && BLAttackersList != null)
                backgroundWorker.RunWorkerAsync();
            else backgroundWorker.CancelAsync();
        }
        StaticFieldAccessor<Func<List<string>>> BLAttackersList = typeof(Astral.Logic.NW.Combats).GetStaticField<Func<List<string>>>("BLAttackersList");

        private void ShowMostInjuredAlly(Entity entity)
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

        private void btnSetMachineId_Click(object sender, EventArgs e)
        {

        }

        private void handler_OpenCredentialManager(object sender, EventArgs e)
        {
            var form = new CredentialManager();
            form.Show();
        }

        private void handler_TeamMonitor(object sender, EventArgs e)
        {
            //new ObjectInfoForm().Show(new PlayerTeamMonitor(), 500);
            EntityTools.Core.Monitor(new PlayerTeamMonitor());
        }
    }
}
