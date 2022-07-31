using AcTp0Tools.Reflection;
using Astral.Addons;
using Astral.Classes.ItemFilter;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic.UCC;
using Astral.Logic.UCC.Classes;
using Astral.Quester.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using EntityTools.Core;
using EntityTools.Core.Interfaces;
using EntityTools.Enums;
using EntityTools.Patches;
using EntityTools.Patches.Mapper;
using EntityTools.Properties;
using EntityTools.Services;
using EntityTools.Tools;
using MyNW.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using AcTp0Tools;
using Action = Astral.Quester.Classes.Action;

[assembly: InternalsVisibleTo("EntityCore")]
#if !ENCRYPTED_CORE
[assembly: SuppressIldasm()]
#endif

// ReSharper disable once CheckNamespace
namespace EntityTools
{
    public class EntityTools : Plugin
    {
        internal static string CoreHash;
        internal static IEntityToolsCore Core { get; private set; } = new EntityCoreProxy();

        private static bool assemblyResolve_Deletage_Binded;
        private static readonly string assemblyResolve_Name = $"^{Assembly.GetExecutingAssembly().GetName().Name},";

        public override string Name => "Entity Tools";
        public override string Author => "MichaelProg";
        public override Image Icon => Resources.EntityIcon;
        public override BasePanel Settings => _panel ?? (_panel = new EntityToolsMainPanel());
        private BasePanel _panel;

        public static EntityToolsSettings Config { get; set; } = new EntityToolsSettings();

        /// <summary>
        /// Управление выбранной ролью (запуск/остановка)
        /// </summary>
        private static readonly Action<bool> ToggleRole = typeof(Roles).GetStaticAction<bool>("ToggleRole", BindingFlags.Public);
        public static void StopBot()
        {
#if DEBUG
            ETLogger.WriteLine(LogType.Debug, Environment.StackTrace); 
#endif
            ToggleRole(false);
        }

        public override void OnBotStart()
        {
            if(Config.UnstuckSpells.Active)
                UnstuckSpells.Start();

#if PROFILING && DEBUG
            InteractEntities.ResetWatch();
            //InteractEntitiesCached.ResetWatch();
            //InteractEntitiesCachedTimeout.ResetWatch();
            EntitySelectionTools.ResetWatch();
            SearchCached.ResetWatch();
            EntityCache.ResetWatch();
            EntityCacheRecord.ResetWatch();
            SearchDirect.ResetWatch();
#endif
        }

        public override void OnBotStop()
        {
#if PROFILING && DEBUG
            InteractEntities.LogWatch();
            //InteractEntitiesCached.LogWatch();
            //InteractEntitiesCachedTimeout.LogWatch();
            EntitySelectionTools.LogWatch();
            SearchCached.LogWatch();
            EntityCache.LogWatch();
            EntityCacheRecord.LogWatch();
            SearchDirect.LogWatch();
#endif
        }

        public override void OnLoad()
        {
            if (!assemblyResolve_Deletage_Binded)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                assemblyResolve_Deletage_Binded = true;
            }
            if (Config.Logger.Active)
                ETLogger.Start();

            // Загрузка ядра из ресурса
#if false
            Assembly.Load(Properties.Resources.Realms);
            //if (!File.Exists(@".\Logs\Assemplies.log"))
            //    File.Create(@".\Logs\Assemplies.log");

            using (StreamWriter file = new StreamWriter(@".\Logs\Assemblies.log", false))
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    file.WriteLine("=================================================================");
                    file.WriteLine(a.FullName);
                    file.WriteLine("-----------------------------------------------------------------");
                    foreach (Type t in a.GetTypes())
                    {
                        file.Write('\t');
                        file.WriteLine(t.FullName);
                    }
                }
            } 
#endif

            LoadSettings();

#if PATCH_ASTRAL
            // Применение патчей
            ETPatcher.Apply();

            // Проверка и перезагрузка UCC-профиля (при необходимости)
            string lastCustomClass = Astral.Controllers.Settings.Get.LastCustomClass;

            if (lastCustomClass == "UCC" 
                && API.CurrentProfile.ActionsCombat.Count == 0
                && File.Exists(Astral.Controllers.Settings.Get.LastUCCProfile))
            {
                // Повторная попытка загрузить последний использовавшийся UCC-профиль
                ETLogger.WriteLine($"{GetType().Name}: Second try to load ucc-profile '{Astral.Controllers.Settings.Get.LastUCCProfile}'", true);
                API.LoadProfile(Astral.Controllers.Settings.Get.LastUCCProfile);
            }
#endif
        }

        public override void OnUnload()
        {
            ComplexPatch_Mapper.CloseMapper();
            SaveSettings();
        }

        /// <summary>
        /// Загрузка настроек из файла
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(FileTools.SettingsFile))
                {
                    XmlSerializer serialiser = new XmlSerializer(Config.GetType());
                    using (StreamReader fileStream = new StreamReader(FileTools.SettingsFile))
                    {
                        if (serialiser.Deserialize(fileStream) is EntityToolsSettings settings)
                        {
                            Config = settings;
                            ETLogger.WriteLine($"{nameof(EntityTools)}: Load settings from {Path.GetFileName(FileTools.SettingsFile)}", true);
                        }
                        else
                        {
                            Config = new EntityToolsSettings();
                            ETLogger.WriteLine($"{nameof(EntityTools)}: Settings file not found. Use default", true);
                        }
                    }
                }
            }
            catch
            {
                Config = new EntityToolsSettings();
                ETLogger.WriteLine(LogType.Error, $"{nameof(EntityTools)}: Error load settings file {Path.GetFileName(FileTools.SettingsFile)}. Use default", true);
            }
        }

        /// <summary>
        /// Сохранение свойств
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                AstralAccessors.Quester.SavePreprocessor();

                if (!Directory.Exists(Path.GetDirectoryName(FileTools.SettingsFile)))
                {
                    var dir = Path.GetDirectoryName(FileTools.SettingsFile);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        ETLogger.WriteLine(LogType.Error, $"{nameof(EntityTools)}: Error to save settings file {Path.GetFileName(FileTools.SettingsFile)}", true);
                        return;
                    }
                    Directory.CreateDirectory(dir);
                }

                XmlSerializer serializer = new XmlSerializer(/*typeof(SettingsContainer)*/Config.GetType());
                using (TextWriter fileStream = new StreamWriter(FileTools.SettingsFile, false))
                {
                    serializer.Serialize(fileStream, Config);
                }
            }
            catch (Exception exp)
            {
                ETLogger.WriteLine(LogType.Error, $"{nameof(EntityTools)}: Error to save settings file {Path.GetFileName(FileTools.SettingsFile)}", true);
                ETLogger.WriteLine(LogType.Error, exp.ToString(), true);
            }
        }

        #region Подмена обработчика кнопки "Combats"
        //// Старый обработчик из Astral.Forms.Panels.Main
        ////private void \u0003(object \u0002, TileItemEventArgs \u0003)
        ////{
        ////	for (;;)
        ////	{
        ////		if (!false)
        ////		{
        ////			if (Roles.CurrentRole.Name == "Professions")
        ////			{
        ////				goto IL_10;
        ////			}
        ////          Forms.ShowPanel(new \u001E.\u0003());
        ////			if (!false)
        ////			{
        ////				return;
        ////			}
        ////			goto IL_10;
        ////		}
        ////		IL_18:
        ////		if (7 == 0)
        ////		{
        ////			continue;
        ////		}
        ////		if (8 != 0)
        ////		{
        ////			break;
        ////		}
        ////		IL_10:
        ////		XtraMessageBox.Show("Not configurable with professions role.");
        ////		goto IL_18;
        ////	}
        ////}

        //// Фрагмент инициализации "кнопки" "Combats"
        ////this.\u0007.AppearanceItem.Normal.BackColor = Color.MediumPurple;
        ////this.\u0007.AppearanceItem.Normal.BackColor2 = Color.SlateBlue;
        ////this.\u0007.AppearanceItem.Normal.BorderColor = Color.MediumPurple;
        ////this.\u0007.AppearanceItem.Normal.Options.UseBackColor = true;
        ////this.\u0007.AppearanceItem.Normal.Options.UseBorderColor = true;
        ////tileItemElement3.Image = \u0001.combat;
        ////tileItemElement3.ImageAlignment = 5;
        ////tileItemElement3.Text = "Combats";
        ////this.\u0007.Elements.Add(tileItemElement3);
        ////this.\u0007.Name = "tileItem3";
        ////this.\u0007.ItemClick += new TileItemClickEventHandler(this.\u0003);

        ///// <summary>
        ///// Главная форма Astral'a
        ///// </summary>
        //private Astral.Forms.Main mainForm = null;
        ///// <summary>
        ///// Главная панель Astral'a (Main)
        ///// </summary>
        //private Astral.Forms.Panels.Main mainPanel = null;

        //public void SubscribeEvent_MainPanel_CombatTile_Click()
        //{
        //    // Поиск главной формы 
        //    do
        //    {
        //        foreach (Form form in Application.OpenForms)
        //        {
        //            if (form is Astral.Forms.Main)
        //            {
        //                mainForm = (Astral.Forms.Main)form;
        //                break;
        //            }
        //        }
        //        Thread.Sleep(500);
        //    }
        //    while (mainForm != null);

        //    // поиск главной панели
        //    if (mainForm != null)
        //    {
        //        do
        //        {
        //            if(mainForm.main)
        //            Thread.Sleep(500);
        //        }
        //        while (mainPanel != null);
        //    }
        //}
        #endregion


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (Regex.IsMatch(args.Name, assemblyResolve_Name))
                return typeof(EntityTools).Assembly;
            return null;
        }

        /// <summary>
        /// Класс-заместитель, инициализирующий ядро
        /// </summary>
        internal class EntityCoreProxy : IEntityToolsCore
        {
            static Func<bool> _internalInitializer = LoadCore;

#if DEVELOPER
            public string EntityDiagnosticInfos(object obj)
            {
                if (_internalInitializer())
                    return Core.EntityDiagnosticInfos(obj);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{obj.GetType().Name}'. Stop bot", true);
                StopBot();
                return string.Empty;
            }
#endif
            public bool Initialize(object obj)
            {
                if (_internalInitializer())
                    return Core.Initialize(obj);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{obj.GetType().Name}'. Stop bot");
                //StopBot();
                return false;
            }

            public bool Initialize(Action action)
            {
                if (_internalInitializer())
                    return Core.Initialize(action);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing in quester action '{action.GetType().Name}'[{action.ActionID}]. Stop bot", true);
                //StopBot();
                return false;
            }

            public bool Initialize(Condition condition)
            {
                if (_internalInitializer())
                    return Core.Initialize(condition);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing in quester condition '{condition.GetType().Name}'[{condition.GetHashCode():X2}]. Stop bot", true);
                //StopBot();
                return false;
            }

            public bool Initialize(UCCAction action)
            {
                if (_internalInitializer())
                    return Core.Initialize(action);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing in ucc action '{action.GetType().Name}'[{action.GetHashCode():X2}]. Stop bot", true);
                //StopBot();
                return false;
            }

            public bool Initialize(UCCCondition condition)
            {
                if (_internalInitializer())
                    return Core.Initialize(condition);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing in ucc condition '{condition.GetType().Name}'[{condition.GetHashCode():X2}]. Stop bot", true);
                //StopBot();
                return false;
            }

            public bool TryGet<T>(ref T obj, params object[] args) where T : class
            {
                if (_internalInitializer())
                    return Core.TryGet(ref obj, args);
                if (Equals(obj, null))
                    ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing unknown object. Stop bot", true);
                else ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{obj.GetType().Name}'[{obj.GetHashCode():X2}]. Stop bot", true);
                return false;
            }

#if false
            public bool TryGet(ref object obj, params object[] args)
            {
                if (InternalInitialize())
                    return Core.TryGet(ref obj, args);
                if (Equals(obj, null))
                    ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing unknown object. Stop bot", true);
                else ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{obj.GetType().Name}'[{obj.GetHashCode():X2}]. Stop bot", true);
                return false;
            }  
#endif
            public object Get(Type type, params object[] args)
            {
                if (_internalInitializer())
                    return Core.Get(type, args);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{type?.Name ?? string.Empty}'. Stop bot", true);
                return null;
            }

            public T Get<T>(params object[] args) where T : class
            {
                if (_internalInitializer())
                    return Core.Get<T>(args);
                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore failed while initializing '{typeof(T).Name}'. Stop bot", true);
                return default;
            }
#if DEVELOPER
            public bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, string displayName = "")
            {
                if (_internalInitializer())
                    return Core.UserRequest_SelectItem(source, ref selectedValue, displayName);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rItem request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Item request denied.", true);

                return false;
            }

            public bool UserRequest_SelectItem<T>(Func<IEnumerable<T>> source, ref T selectedValue, ListControlConvertEventHandler itemFormatter)
            {
                if (_internalInitializer())
                    return Core.UserRequest_SelectItem(source, ref selectedValue, itemFormatter);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rItem request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Item request denied.", true);

                return false;
            }

            public bool UserRequest_SelectItemList<T>(Func<IEnumerable<T>> source, ref IList<T> selectedValues, string caption = "")
            {
                if (_internalInitializer())
                    return Core.UserRequest_SelectItemList(source, ref selectedValues);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rItems request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Items request denied.", true);

                return false;
            }

            public bool UserRequest_EditValue(ref string value, string message = "", string caption = "", FormatInfo formatInfo = null)
            {
                if (_internalInitializer())
                    return Core.UserRequest_EditValue(ref value, message, caption, formatInfo);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rValue edition request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! UIGen request denied.", true);

                return false;
            }

            public bool UserRequest_SelectAuraId(ref string id)
            {
                if (_internalInitializer())
                    return Core.UserRequest_SelectAuraId(ref id);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rAura request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Aura request denied.", true);

                return false;
            }

            public bool UserRequest_SelectUIGenId(ref string id)
            {
                if (_internalInitializer())
                    return Core.UserRequest_SelectUIGenId(ref id);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rUIGen request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! UIGen request denied.", true);

                return false;
            }

            public bool UserRequest_EditEntityId(ref string entPattern, ref ItemFilterStringType strMatchType, ref EntityNameType nameType)
            {
                if (_internalInitializer())
                    return Core.UserRequest_EditEntityId(ref entPattern, ref strMatchType, ref nameType);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rEntityId request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! EntityId request denied.", true);

                return false;
            }

            public bool UserRequest_EditUccConditions(ref System.Collections.ObjectModel.ObservableCollection<UCCCondition> list)
            {
                if (_internalInitializer())
                    return Core.UserRequest_EditUccConditions(ref list);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rUCC conditions request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! UCC conditions request denied.", true);

                return false;
            }

            public bool UserRequest_EditUccConditions(ref System.Collections.ObjectModel.ObservableCollection<UCCCondition> list, ref LogicRule logic, ref bool negation)
            {
                if (_internalInitializer())
                    return Core.UserRequest_EditUccConditions(ref list, ref logic, ref negation);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rUCC conditions request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! UCC conditions request denied.", true);

                return false;
            }
            
            public bool UserRequest_EditCustomRegionList(ref List<string> crList)
            {
                if (_internalInitializer())
                    return Core.UserRequest_EditCustomRegionList(ref crList);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rCustomRegions request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! CustomRegions request denied.", true);

                return false;
            }

            public bool UserRequest_GetNodeLocation(ref Vector3 pos, string caption, string message = "")
            {
                if (_internalInitializer())
                    return Core.UserRequest_GetNodeLocation(ref pos, caption, message);

                XtraMessageBox.Show("EntityToolsCore is invalid!\nNodeLocation request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Node Location request denied.", true);

                return false;
            }

            public bool UserRequest_GetPosition(ref Vector3 pos, string caption, string message = "")
            {
                if (_internalInitializer())
                    return Core.UserRequest_GetPosition(ref pos, caption, message);

                XtraMessageBox.Show("EntityToolsCore is invalid!\nPosition request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Position request denied.", true);

                return false;
            }
            public bool UserRequest_GetEntityToInteract(ref Entity entity)
            {
                if (_internalInitializer())
                    return Core.UserRequest_GetEntityToInteract(ref entity);

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rEntityToInteract request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! EntityToInteract request denied.", true);

                return false;
            }

            public bool UserRequest_GetUccAction(out UCCAction action)
            {
                if (_internalInitializer())
                    return Core.UserRequest_GetUccAction(out action);
                action = null;

                XtraMessageBox.Show("EntityToolsCore is invalid!\n\rUCCAction request denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! UCCAction request denied.", true);

                return false;
            }
            public bool UserRequest_Edit(object obj, params object[] param)
            {
                if (obj is null)
                    return false;

                if (_internalInitializer())
                    return Core.UserRequest_Edit(obj, param);

                XtraMessageBox.Show($"EntityToolsCore is invalid!\n\rThe edition of the object {obj.GetType().Name} denied.", "EntityTools error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                ETLogger.WriteLine(LogType.Error, $"EntityToolsCore is invalid! Forbid an edition of the {obj.GetType().Name}.", true);

                obj = null;
                return false;
            }

            public void Monitor(object monitor)
            {
                if (_internalInitializer())
                    Core.Monitor(monitor);
            }
#endif
#if DEBUG
            public LinkedList<Entity> FindAllEntity(string pattern, ItemFilterStringType matchType = ItemFilterStringType.Simple, EntityNameType nameType = EntityNameType.NameUntranslated, EntitySetType setType = EntitySetType.Complete, bool healthCheck = false, float range = 0, float zRange = 0, bool regionCheck = false, List<CustomRegion> customRegions = null, Predicate<Entity> specialCheck = null)
            {
                if (_internalInitializer())
                    return Core.FindAllEntity(pattern, matchType, nameType, setType, healthCheck, range, zRange, regionCheck, customRegions, specialCheck);

                ToggleRole(false);

                ETLogger.WriteLine(LogType.Error, "EntityToolsCore is invalid! Entities search aborted. Stop bot.", true);

                return null;
            }
#endif
            public bool CheckCore()
            {
                if (_internalInitializer())
                    return Core.CheckCore();
                return false;
            }

#if true
            /// <summary>
            /// Загрузка сборки, содержащей реализацию ядра из альтернативного файлового потока
            /// </summary>
            /// <returns></returns>
            private static bool LoadCore()
            {
                // Попытка загрузки ядра производится только после привязки делегата CurrentDomain_AssemblyResolve
                if (!assemblyResolve_Deletage_Binded)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                    assemblyResolve_Deletage_Binded = true;
                }

                try
                {
                    var @locker = Core;
                    lock (@locker)
                    {
                        if (@locker != Core)
                            return Core.CheckCore();

                        var executingAssembly = Assembly.GetExecutingAssembly();
                        var ETfilename = executingAssembly.GetName().Name + ".dll";
                        var wrongETLocation = Path.Combine(Directories.AstralStartupPath, ETfilename);
                        if (File.Exists(wrongETLocation))
                        {
                            var msg = string.Concat("The file ", ETfilename, " save location is incorrect.\n" +
                                "You should replace it from the folder: ", Directories.AstralStartupPath,
                                "\nto the folder: ", Directories.PluginsPath);

                            ETLogger.WriteLine(LogType.Error, msg, true);
                            return false;
                        }

                        using (FileStream file = FileStreamHelper.OpenWithStream(executingAssembly.Location, "Core",
                            FileMode.Open, FileAccess.Read))
                        {
                            byte[] coreBytes = new byte[file.Length];
                            if (file.Read(coreBytes, 0, (int)file.Length) > 0)
                            {
#if ENCRYPTED_CORE
                                byte[] key = SysInfo.SysInformer.GetMashineID(false).TextToBytes();
                                if (CryptoHelper.DecryptFile_Rijndael(coreBytes, key, out byte[] decryptedCoreBytes))
                                {
#if DEBUG
                                    File.WriteAllText("EntityCore_key", key.ToHexString());
                                    File.WriteAllBytes("EntityCore_encrypt", coreBytes);
                                    File.WriteAllBytes("EntityCore_decrypt", decryptedCoreBytes);
                                    File.WriteAllText("EntityCore_decrypt.md5", CryptoHelper.MD5_HashString(decryptedCoreBytes));
#endif
                                    try
                                    {
                                        Assembly assembly = Assembly.Load(decryptedCoreBytes);
                                        CoreHash = CryptoHelper.MD5_HashString(decryptedCoreBytes);
#else
                                try
                                {
                                    Assembly assembly = Assembly.Load(coreBytes);
                                    CoreHash = CryptoHelper.MD5_HashString(coreBytes);
#endif
                                    Type coreType = typeof(IEntityToolsCore);
                                    foreach (Type type in assembly.GetTypes())
                                    {
#if false
                                                if (type.GetInterface(nameof(IEntityToolsCore)) != null)
#else
                                        if (type.GetInterfaces().Contains(coreType))
#endif
                                        {
                                            if (Activator.CreateInstance(type) is IEntityToolsCore core)
                                            {
                                                Core = core;
                                                return true;
                                            }
                                        }
                                    }
                                }
#if !ENCRYPTED_CORE
                                catch
                                {
                                    // ignored
                                }
#else
                                    catch (Exception e)
                                    {
                                        string msg = "Fail to load decrypted EntityToolCore\r\n" + e.Message;
                                        ETLogger.WriteLine(LogType.Error, msg);
                                        Astral.Logger.WriteLine(msg);
                                    }
                                }
                                else
                                {
                                    string msg = "Fail to decrypt EntityToolCore";
                                    ETLogger.WriteLine(LogType.Error, msg);
                                    Astral.Logger.WriteLine(msg);
                                }
#endif
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine(LogType.Debug, e.ToString(), true);
                }
                finally
                {
                    _internalInitializer = DoNothing;
                }

                return false;
            }
#else
            private static bool LoadCore()
            {
                // Попытка загрузки ядра производится только после привязки делегата CurrentDomain_AssemblyResolve
                if (!assemblyResolve_Deletage_Binded)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                    assemblyResolve_Deletage_Binded = true;
                }

                try
                {
                    var assembly = Assembly.Load(Properties.Resources.Bites);

                    CoreHash = CryptoHelper.MD5_HashString(Properties.Resources.Bites);
                    Type coreType = typeof(IEntityToolsCore);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.GetInterfaces().Contains(coreType))
                        {
                            if (Activator.CreateInstance(type) is IEntityToolsCore core)
                            {
                                Core = core;
                                return Core.CheckCore();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ETLogger.WriteLine(LogType.Debug, e.ToString(), true);
                }
                finally
                {
                    InternalInitialize = DoNothing;
                }

                return false;
            }
#endif

            private static bool DoNothing()
            {
                return false;
            }


            public void Dispose()
            {
                _internalInitializer = null;
            }
        }
    }
}
