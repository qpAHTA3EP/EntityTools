//#define Test_EntitySelectForm
//#define DUMP_TEST

using AcTp0Tools;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.Classes.FSM;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Patches.UCC;
using EntityTools.Services;
using EntityTools.Tools;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using API = Astral.Quester.API;
using Memory = MyNW.Memory;
using Task = System.Threading.Tasks.Task;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : /* UserControl //*/ BasePanel
    {
        //TODO: Отображать метрики EntityCache и AStar
        public EntityToolsMainPanel() : base("Entity Tools")
        {
            InitializeComponent();

            cbxExportSelector.DataSource = Enum.GetValues(typeof(ExportTypes));

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
#if false
            var tester = new SimplePropertyAccessTester();
            var result = tester.Test();

            XtraMessageBox.Show(result);  
#endif
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1_000_000; i++)
            {
                var g = AstralAccessors.Quester.Core.Meshes;
            }
            sw.Stop();

#if false
            XtraMessageBox.Show(
                    $"1'000'000 reads of the property 'Astral.Quester.Core.Meshes'\n" +
                        $"at the map '{EntityManager.LocalPlayer.MapAndRegion}'\n" +
                        $"using StaticPropertyAccessor<Graph>:\n" +
                        $"{sw.ElapsedMilliseconds,8} ms {sw.ElapsedTicks,10} ticks"); 
#else
            XtraMessageBox.Show(
                $"1'000'000 reads of the property 'Astral.Quester.Core.Meshes'\n" +
                $"at the map '{EntityManager.LocalPlayer.MapAndRegion}'\n" +
                $"using Harmony patches:\n" +
                $"{sw.ElapsedMilliseconds,8} ms {sw.ElapsedTicks,10} ticks");
#endif
        }

        private void handler_Test_2(object sender, EventArgs e)
        {
#if false
            QuesterAssistantAccessors.Classes.Monitoring.Frames.Sleep(1000); 
#else
            var info = AstralAccessors.Logic.NW.Combats.AbortCombatCondition_DebugInfo();
            XtraMessageBox.Show(info);
#endif

        }
        private void handler_Test_3(object sender, EventArgs e)
        {
#if false
            var tester = new StaticMethodPatchTester();
            var result = tester.Test();
            XtraMessageBox.Show(result); 
#endif
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
            EntityTools.Core.GUIRequest_UIGenId(ref uiGenId);
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
            EntityTools.Core.GUIRequest_EntityId(ref pattern, ref strMatch, ref nameType);
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
            EntityTools.Core.GUIRequest_AuraId(ref auraId);
            if (!string.IsNullOrEmpty(auraId))
                Clipboard.SetText(auraId);
#endif
        }

        private void handler_GetMachineId(object sender, EventArgs e)
        {
            var machineid = Memory.MMemory.ReadString(Memory.BaseAdress + 0x2640BD0, Encoding.UTF8, 64);
            lblAccount.Text = $@"Account:   @{EntityManager.LocalPlayer.AccountLoginUsername}";
            tbMashingId.Text = machineid;
        }

        private void handler_ChangeExportingFileName(object sender, ButtonPressedEventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                if (string.IsNullOrEmpty(tbExportFileSelector.Text))
                {
                    fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                    fileName = fileName.Replace(Directories.AstralStartupPath, @".\");
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

        private void handler_ChangeExportingData(object sender, EventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                fileName = fileName.Replace(Directories.AstralStartupPath, @".\");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void handler_ResetExportSettings(object sender, EventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {

                string fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.DefaultExportFileName);
                fileName = fileName.Replace(Directories.AstralStartupPath, @".\");
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
                    MessageBox.Show("The specified filename is incorrect.\n" +
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

                if (MessageBox.Show(this, $"Would you like to open {Path.GetFileName(fullFileName)}?", $"Open {Path.GetFileName(fullFileName)}?", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
#if DEVELOPER
#if true
            if (ckbDebugMonitor.Checked)
                Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = ShowMostInjuredAlly;
            else Patch_Logic_UCC_Classes_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = null; 
#else

#endif
#endif
        }

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
    }
}
