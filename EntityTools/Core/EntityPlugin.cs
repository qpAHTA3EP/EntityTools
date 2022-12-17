using ACTP0Tools;
using ACTP0Tools.Classes.Quester;
using Astral.Addons;
using Astral.Forms;
using Astral.Logic.UCC;
using EntityTools.Core;
using EntityTools.Patches;
using EntityTools.Patches.Mapper;
using EntityTools.Properties;
using EntityTools.Services;
using EntityTools.Tools;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("EntityCore")]
#if !ENCRYPTED_CORE
[assembly: SuppressIldasm]
#endif

// ReSharper disable once CheckNamespace
namespace EntityTools
{
    public class EntityTools : Plugin
    {
        private static bool _assemblyResolveDelegateBinded;
        private static readonly string AssemblyResolveName = $"^{Assembly.GetExecutingAssembly().GetName().Name},";

        public override string Name => "Entity Tools";
        public override string Author => "MichaelProg";
        public override Image Icon => Resources.Entity;
        public override BasePanel Settings => _panel ?? (_panel = new EntityToolsMainPanel());
        private BasePanel _panel;

        public static EntityToolsSettings Config { get; set; } = new EntityToolsSettings();

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
            if (!_assemblyResolveDelegateBinded)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                _assemblyResolveDelegateBinded = true;
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
                QuesterHelper.SavePreprocessor();

                if (!Directory.Exists(Path.GetDirectoryName(FileTools.SettingsFile)))
                {
                    var dir = Path.GetDirectoryName(FileTools.SettingsFile);
                    if (string.IsNullOrEmpty(dir))
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

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (Regex.IsMatch(args.Name, AssemblyResolveName))
                return typeof(EntityTools).Assembly;
            return null;
        }
    }
}
