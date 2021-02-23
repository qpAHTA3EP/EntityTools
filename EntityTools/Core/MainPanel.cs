//#define Test_EntitySelectForm
//#define DUMP_TEST

using Astral;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.Classes.FSM;
using Astral.Quester.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using EntityTools.Enums;
using EntityTools.Patches.UCC;
using EntityTools.Reflection;
using EntityTools.Services;
using EntityTools.Tools;
using MyNW;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using API = Astral.Quester.API;
using Task = System.Threading.Tasks.Task;
using System.Threading;

namespace EntityTools.Core
{
    public partial class EntityToolsMainPanel : /* UserControl //*/ BasePanel
    {
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
#if MissionGiver
            var giver = new MissionGiverNPC { Id = "Test1", Position = new Vector3(1, 1, 1), MapName = "AAAA" };
            var giverContainer = new TestGiver { Giver = giver };
            XmlSerializer serialiser = new XmlSerializer(giverContainer.GetType());//, new[]{ typeof(MissionGiverNPC), typeof(MissionGiverRemote) });
            using (TextWriter FileStream = new StreamWriter("TestGiver1.xml", false))
            {
                serialiser.Serialize(FileStream, giverContainer);
            }
            giverContainer.Giver = new MissionGiverRemote { Id = "Test2" };
            using (TextWriter FileStream = new StreamWriter("TestGiver2.xml", false))
            {
                serialiser.Serialize(FileStream, giverContainer);
            }
            TestGiver2 giverContainer2 = new TestGiver2 { Giver = new NPCInfos { CostumeName = "Test3", Position = new Vector3(2, 2, 2), MapName = "BBBB" } };
            serialiser = new XmlSerializer(giverContainer2.GetType());
            using (TextWriter FileStream = new StreamWriter("TestGiver3.xml", false))
            {
                serialiser.Serialize(FileStream, giverContainer2);
            } 
#endif
#if false
            var pos = new Vector3(833, 517, 32);

            bool result = await NavigationHelper.ApproachAsync(pos); 
#endif
        }
        private void handler_Test_2(object sender, EventArgs e)
        {
        }
        private void handler_Test_3(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbText.Text))
                return; 

            if(!int.TryParse(tbText.Text, out int iPtr))
                return;

            IntPtr ptr = new IntPtr(iPtr);

            StringBuilder sb = new StringBuilder($"Read pointer {iPtr}");
            sb.AppendLine();
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    IntPtr ptr_i = Memory.MMemory.Read<IntPtr>(ptr + i);
                    string str = Memory.MMemory.ReadString(ptr_i);
                    sb.Append(i).Append(":\t[").Append(ptr_i).Append("] ").AppendLine(str);
                    for (int j = 0; j < 100; j++)
                    {
                        IntPtr ptr_j = Memory.MMemory.Read<IntPtr>(ptr_i + j);
                        string str_j = Memory.MMemory.ReadString(ptr_j);
                        sb.Append(i).Append('+').Append(j).Append(":\t[").Append(ptr_j).Append("] ").AppendLine(str_j);
                    }
                }
                catch
                {
                    sb.Append(i).AppendLine(": ERROR");
                }
            }
            ETLogger.WriteLine(LogType.Debug, sb.ToString(), true);

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
            lblAccount.Text = $"Account:   @{EntityManager.LocalPlayer.AccountLoginUsername}";
            tbMashingId.Text = machineid;
        }

        private void handler_ChangeExportingFileName(object sender, ButtonPressedEventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {
                string fileName;
                if (string.IsNullOrEmpty(tbExportFileSelector.Text))
                {
                    fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                    fileName.Replace(Directories.AstralStartupPath, @".\");
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
                fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                fileName.Replace(Directories.AstralStartupPath, @".\");
                tbExportFileSelector.Text = fileName;
            }
        }

        private void handler_ResetExportSettings(object sender, EventArgs e)
        {
            if (cbxExportSelector.SelectedItem is ExportTypes expType)
            {

                string fileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.defaultExportFileName);
                fileName.Replace(Directories.AstralStartupPath, @".\");
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
                    fullFileName = Path.Combine(Directories.LogsPath, expType.ToString(), FileTools.ReplaceMask(FileTools.defaultExportFileName));
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

        private void handler_ShowMapper(object sender, EventArgs e)
        {
            MapperForm.Open();
        }

        private void handler_EnableLogger(object sender, EventArgs e)
        {
            if (ckbETLogger.Checked)
                ETLogger.Start();
            else ETLogger.Stop();
        }

        private void handler_OpenEntityViewer(object sender, EventArgs e)
        {
            if(File.Exists(ETLogger.LogFilePath))
                Process.Start(ETLogger.LogFilePath);
        }

        private void handler_OpenLogFile(object sender, EventArgs e)
        {
            if(File.Exists(ETLogger.LogFilePath))
                Process.Start(ETLogger.LogFilePath);
        }

        private void handler_CheckCore(object sender, EventArgs e)
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
                Patch_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = ShowMostInjuredAlly;
            else Patch_ActionsPlayer_CheckAlly.MostInjuredAllyChanged = null; 
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

        private void handler_MapperTest(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (AOECheck.AOE aoe in AstralAccessors.Controllers.AOECheck.GetAOEList())
            {
                sb.AppendLine($"{aoe.Location}\t\t{aoe.ID}");
            }
            XtraMessageBox.Show(sb.ToString());
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
    }
}
