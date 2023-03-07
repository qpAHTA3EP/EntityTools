using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Infrastructure;
using Infrastructure.Quester;
using Infrastructure.Patches;
using Astral;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.Classes.FSM;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Forms;
using EntityTools.Services;
using EntityTools.Servises.SlideMonitor;
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

            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            lblVersion.Text = $"{assemblyInfo.Name} v.{assemblyInfo.Version}";

            cbxExportSelector.Properties.Items.AddRange(Enum.GetValues(typeof(ExportTypes)));
            cbxExportSelector.SelectedIndex = 0;

            ckbSpellStuckMonitor.DataBindings.Add(nameof(ckbSpellStuckMonitor.Checked),
                EntityTools.Config.UnstuckSpells,
                nameof(EntityTools.Config.UnstuckSpells.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);

            // Настройки Mapper'a
            ckbMapperPatch.DataBindings.Add(nameof(ckbMapperPatch.Checked),
                EntityTools.Config.Mapper,
                nameof(EntityTools.Config.Mapper.Patch),
                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbETLogger.DataBindings.Add(nameof(ckbETLogger.Checked),
                EntityTools.Config.Logger,
                nameof(EntityTools.Config.Logger.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);

            #region QuesterProfilePreprocessing
            ckbEnapleQuesterProfilePreprocessing.DataBindings.Add(nameof(ckbEnapleQuesterProfilePreprocessing.Checked),
                    QuesterHelper.Preprocessor,
                    nameof(QuesterHelper.Preprocessor.Active),
                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbAutoSavePreprocessedProfile.DataBindings.Add(nameof(ckbAutoSavePreprocessedProfile.Checked),
                QuesterHelper.Preprocessor,
                nameof(QuesterHelper.Preprocessor.AutoSave),
                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbAutoSavePreprocessedProfile.DataBindings.Add(nameof(ckbAutoSavePreprocessedProfile.Enabled),
                QuesterHelper.Preprocessor,
                nameof(QuesterHelper.Preprocessor.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);


            gridReplacements.DataSource = QuesterHelper.Preprocessor.Replacement;

            gridReplacements.DataBindings.Add(nameof(gridReplacements.Enabled),
                QuesterHelper.Preprocessor,
                nameof(QuesterHelper.Preprocessor.Active),
                false, DataSourceUpdateMode.OnPropertyChanged);
            #endregion

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
        }

        private void handler_Test_2(object sender, EventArgs e)
        {
        }

        private void handler_Test_3(object sender, EventArgs e)
        {
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
            string uiGenId = UIViewer.GUIRequest(string.Empty);
            if(!string.IsNullOrEmpty(uiGenId))
                Clipboard.SetText(uiGenId);
        }

        private void handler_OpenEntitiesViewer(object sender, EventArgs e)
        {
            string pattern = string.Empty;
            EntityNameType nameType = EntityNameType.InternalName;
            ItemFilterStringType strMatch = ItemFilterStringType.Simple;
            EntityViewer.GUIRequest(ref pattern, ref strMatch, ref nameType);
            if (!string.IsNullOrEmpty(pattern))
                Clipboard.SetText(pattern);
        }

        private void handler_OpenMissionMonitor(object sender, EventArgs e)
        {
            var missMonitor = new MissionMonitorForm();
            missMonitor.Show();
        }

        private void handler_OpenAuraViewer(object sender, EventArgs e)
        {
            string auraId = AuraViewer.GUIRequest();
            if (!string.IsNullOrEmpty(auraId))
                Clipboard.SetText(auraId);
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
            var item = new QuesterHelper.ReplacementItem();
            QuesterHelper.Preprocessor.Replacement.Add(item);
        }

        private void handler_DeleteProcessingItem(object sender, EventArgs e)
        {
            gridViewPreprocessing.DeleteSelectedRows(); 
        }

        private void handler_ClearProcessingItems(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show($"Are you sure to to delete all items ?",
                                    "Clear Items", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                QuesterHelper.Preprocessor.Replacement.Clear();
            }
        }

        private void handler_TestProcessingItem(object sender, EventArgs e)
        {
            var replacement = QuesterHelper.Preprocessor.Replacement;

            if(replacement.Count == 0)
                return;

            OpenFileDialog openDialog = Infrastructure.Classes.FileTools.GetOpenDialog( filter: @"Astral mission profile (*.amp.zip)|*.amp.zip",
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
                var items = QuesterHelper.Preprocessor.Replacement;
                XmlSerializer serializer = new XmlSerializer(items.GetType());
                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    serializer.Serialize(file, items);
                }
            }
        }

        private void handler_ImportPreprocessingProfile(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = Infrastructure.Classes.FileTools.GetOpenDialog(filter: "Replacements (*.xml)|*.xml",
                                                                                   defaultExtension: "xml",
                                                                                   initialDir: Directories.SettingsPath);
                
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                List<QuesterHelper.ReplacementItem> items = null;

                XmlSerializer serialiser = new XmlSerializer(typeof(List<QuesterHelper.ReplacementItem>));
                using (StreamReader fileStream = new StreamReader(openDialog.FileName))
                {
                    if (serialiser.Deserialize(fileStream) is List<QuesterHelper.ReplacementItem> list)
                    {
                        items = list;
                    }
                }
                if (items?.Count > 0)
                {
                    var replacements = QuesterHelper.Preprocessor.Replacement;
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
            QuesterHelper.SavePreprocessor();
        }

        private void handler_EntityCacheMonitor(object sender, EventArgs e)
        {
            CollectionInfoForm.Show(() => SearchCached.EntityCache);
        }

        private void handler_EditUcc(object sender, EventArgs e)
        {
#if true
            UccEditor.Edit();
#else
            UccEditor.Edit(Astral.Logic.UCC.API.CurrentProfile, Astral.API.CurrentSettings.LastUCCProfile);
#endif
        }

        private void handler_EditQuester(object sender, EventArgs e)
        {
#if true
            QuesterEditor.Edit();
#else
            var profile = Astral.Quester.API.CurrentProfile;
            var profileName = Astral.API.CurrentSettings.LastQuesterProfile;

            if (profile.Saved && !string.IsNullOrEmpty(profileName))
                QuesterEditor.Edit(profile, profileName);
            else QuesterEditor.Edit(profile); 
#endif
        }
    }
}
