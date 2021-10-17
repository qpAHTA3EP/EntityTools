using AcTp0Tools.Patches;
using AcTp0Tools.Reflection;
using AStar;
using Astral.Controllers;
using Astral.Quester.Classes;
using HarmonyLib;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Astral;
using DevExpress.XtraEditors;
using Action = System.Action;
// ReSharper disable InconsistentNaming

namespace AcTp0Tools
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static class Quester
        {
#if false
            public static class Action
            {
                public static readonly InstancePropertyAccessor<Astral.Quester.Classes.Action, Astral.Quester.Classes.ActionDebug> ActionDebug = null;

                static Action()
                {
                    ActionDebug = typeof(Astral.Quester.Classes.Action).GetInstanceProperty<Astral.Quester.Classes.ActionDebug>("Debug");
                }
            } 
#endif
            public static class FSM
            {
                public static class States
                {
                    public static class Combat
                    {
                        static readonly StaticFieldAccessor<int> ignoreCombatMinHP =
                            typeof(Astral.Quester.FSM.States.Combat).GetStaticField<int>("ignoreCombatMinHP");
                        public static int IgnoreCombatMinHP
                        {
                            get => ignoreCombatMinHP.Value;
                            set => ignoreCombatMinHP.Value = value;
                        }

                        static Action<bool, int, int> setIgnoreCombat;
                        /// <summary>
                        /// Установка параметров, управляющих режимом игнорирования боя IgnoreCombat
                        /// </summary>
                        /// <param name="value"></param>
                        /// <param name="minHP">Минимальное значение HP, при котором бой принудительно активируется</param>
                        /// <param name="time">Продолжительность времени в течение которого игнорируются атаки</param>
                        public static void SetIgnoreCombat(bool value, int minHP = -1, int time = 0)
                        {
                            setIgnoreCombat?.Invoke(value, minHP, time);
                        }

                        static Combat()
                        {
                            Type combatType = typeof(Astral.Quester.FSM.States.Combat);
                            setIgnoreCombat = combatType.GetStaticAction<bool, int, int>(nameof(SetIgnoreCombat));
                        }
                    }
                }
            }

            public static class Core
            {
                #region Meshes
#if StaticPropertyAccessor_Meshes
                /// <summary>
                /// Функтор доступа к графу путей (карте) текущего профиля
                /// </summary>
                private static readonly StaticPropertyAccessor<Graph> _meshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");
                public static Graph Meshes
                {
                    get
                    {
                        if (_meshes.IsValid)
                            return _meshes.Value;
                        return null;
                    }
                }

                /// <summary>
                /// Функтор доступа к коллекции графов путей (карт) текущего профиля
                /// Astral.Quester.Core.MapsMeshes
                /// </summary>
                private static readonly StaticPropertyAccessor<Dictionary<string, Graph>> _mapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes");
                public static Dictionary<string, Graph> MapsMeshes
                {
                    get
                    {
                        if (_mapsMeshes.IsValid)
                            return _mapsMeshes.Value;
                        return null;
                    }
                }
#else
                /// <summary>
                /// Функтор доступа к графу путей (карте) текущего профиля
                /// </summary>
                public static Graph Meshes
                {
#if false           // Оригинальный код
                    get
                    {
	                    Dictionary<string, Graph> mapsMeshes = Core.MapsMeshes;
	                    Graph result;
	                    lock (mapsMeshes)
	                    {
		                    LocalPlayerEntity localPlayer = Class1.LocalPlayer;
		                    if (localPlayer.IsValid)
		                    {
			                    string mapName = localPlayer.MapState.MapName;
			                    if (mapName.Length > 0)
			                    {
				                    if (!Core.MapsMeshes.ContainsKey(mapName))
				                    {
					                    Graph graph = new Graph();
					                    if (Core.CurrentProfileZipMeshFile.Length > 0)
					                    {
						                    Class88.Class89 @class = new Class88.Class89(graph, Class88.Class89.Enum2.const_1, mapName + ".bin", null);
						                    Class88.smethod_3(Core.CurrentProfileZipMeshFile, @class);
						                    if (@class.Success)
						                    {
							                    Logger.WriteLine(mapName + " loaded from profile zip !");
							                    graph = (@class.Object as Graph);
						                    }
					                    }
					                    Core.MapsMeshes.Add(mapName, graph);
				                    }
				                    return Core.MapsMeshes[mapName];
			                    }
		                    }
		                    result = new Graph();
	                    }
	                    return result;
                    }
#endif
                    get
                    {
                        Graph graph = null;
                        PrefixGetMeshes(ref graph);
                        return graph;
                    }
                }
                private static bool PrefixGetMeshes(ref Graph __result)
                {
                    Dictionary<string, Graph> mapsMeshes = _mapsMeshes;
                    lock (mapsMeshes)
                    {
                        var localPlayer = EntityManager.LocalPlayer;
                        if (localPlayer.IsValid)
                        {
                            string mapName = localPlayer.MapState.MapName;
                            if (!string.IsNullOrEmpty(mapName))
                            {
                                if (mapsMeshes.ContainsKey(mapName))
                                {
                                    __result = mapsMeshes[mapName];
                                }
                                else if (LoadMeshFromZipFile(CurrentProfileZipMeshFile, mapName, out Graph mesh))
                                {
                                    mapsMeshes.Add(mapName, mesh);
                                    __result = mesh;
                                }
                                else
                                {
                                    __result = new Graph();
                                    mapsMeshes.Add(mapName, __result);
                                }
                            }
                        }
                    }

                    return false;
                }

                /// <summary>
                /// Загрузка графа путей <param name="meshes"/> для карты <param name="mapName"/>
                /// из файла <param name="zipFileName"/>
                /// </summary>
                /// <returns>True, если граф путей <param name="meshes"/> успешно загружен</returns>
                public static bool LoadMeshFromZipFile(string zipFileName, string mapName, out Graph meshes)
                {
                    meshes = null;
                    if (!File.Exists(zipFileName)) return false;

                    try
                    {
                        using (var zipFile = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
                        {
                            var meshEntry = zipFile.GetEntry(mapName + ".bin");
                            if (meshEntry is null) return false;

                            using (Stream stream = meshEntry.Open())
                            {
                                var binaryFormatter = new BinaryFormatter();
                                if (binaryFormatter.Deserialize(stream) is Graph meshesFromFile)
                                {
                                    meshes = meshesFromFile;
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    return false;
                }

                /// <summary>
                /// Функтор доступа к коллекции графов путей (карт) текущего профиля
                /// Astral.Quester.Core.MapsMeshes
                /// </summary>
                public static Dictionary<string, Graph> MapsMeshes => _mapsMeshes;
                private static Dictionary<string, Graph> _mapsMeshes = new Dictionary<string, Graph>();
                private static bool PrefixGetMapsMeshes(ref Dictionary<string, Graph> __result)
                {
                    __result = _mapsMeshes;
                    return false;
                }
                private static bool PrefixSetMapsMeshes(Dictionary<string, Graph> value)
                {
                    _mapsMeshes = value;
                    return false;
                }
#endif

#if false       // Оригинальный код Астрала
                public static string CurrentProfileZipMeshFile
		        {
			        get
			        {
				        if (Class1.CurrentSettings.LastQuesterProfile.Length > 0 && Core.Profile.Saved)
				        {
					        string text = Class1.CurrentSettings.LastQuesterProfile;
					        if (Core.Profile.UseExternalMeshFile && Core.Profile.ExternalMeshFileName.Length >= 10)
					        {
						        text = new FileInfo(text).DirectoryName + "\\" + Core.Profile.ExternalMeshFileName;
					        }
					        if (File.Exists(text))
					        {
						        return text;
					        }
				        }
				        return string.Empty;
			        }
		        }
#else
                public static string CurrentProfileZipMeshFile
                {
                    get
                    {
                        var lastQuesterProfileFileName = Astral.API.CurrentSettings.LastQuesterProfile;
                        var currentProfile = Astral.Quester.API.CurrentProfile;
                        if (lastQuesterProfileFileName.Length > 0
                            && currentProfile.Saved)
                        {
                            string meshesFile;
                            if (currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= 10)
                                meshesFile = Path.Combine(Path.GetDirectoryName(lastQuesterProfileFileName) ?? string.Empty,
                                                          currentProfile.ExternalMeshFileName);
                            else meshesFile = lastQuesterProfileFileName;
                            if (File.Exists(meshesFile))
                            {
                                return meshesFile;
                            }
                        }
                        return string.Empty;
                    }
                }
#endif
#if false
                /// <summary>
                /// Функтор доступа к списку названий карт в файле текущего профиля
                /// Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName)
                /// </summary>
                public static readonly Func<string, List<string>> AvailableMeshesFromFile = typeof(Astral.Quester.Core).GetStaticFunction<string, List<string>>("AvailableMeshesFromFile");

                /// <summary>
                /// Доступ к методу 
                /// Astral.Quester.Core.LoadAllMeshes();
                /// </summary>
                public static readonly Func<int> LoadAllMeshes = typeof(Astral.Quester.Core).GetStaticFunction<int>("LoadAllMeshes"); 
#else
                /// <summary>
                /// Дозагрузка из файла текущего профиля недостающих карт MapsMeshes
                /// </summary>
                /// <returns></returns>
                public static int LoadAllMeshes()
                {
                    var zipFileName = CurrentProfileZipMeshFile;

                    if (!File.Exists(zipFileName)) return 0;

                    int loadedMaps = 0;
                    try
                    {
                        using (var zipFile = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
                        {
                            Dictionary<string, Graph> mapsMeshes = _mapsMeshes;
                            lock (mapsMeshes)
                            {
                                var binaryFormatter = new BinaryFormatter();
                                foreach (var zipEntry in zipFile.Entries)
                                {
                                    var zipEntryName = zipEntry.FullName;

                                    if (!zipEntryName.EndsWith(".bin"))
                                        continue;

                                    var mapName = zipEntryName.Substring(0, zipEntryName.Length - 4);
                                    if (mapsMeshes.ContainsKey(mapName))
                                        continue;

                                    using (Stream stream = zipEntry.Open())
                                    {
                                        if (binaryFormatter.Deserialize(stream) is Graph meshesFromFile)
                                        {
                                            mapsMeshes.Add(mapName, meshesFromFile);
                                            loadedMaps++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    return loadedMaps;
                }
#endif
                #endregion

                #region Profile

                private static readonly PropertyInfo propProfile;
                private static readonly MethodInfo originalSetProfile;
                private static readonly MethodInfo postfixSetProfile;
                public static Astral.Quester.Classes.Profile Profile
                {
                    get => Astral.Quester.API.CurrentProfile;
                    set
                    {
                        if (value is null)
                            throw new ArgumentNullException(nameof(value));
                        originalSetProfile?.Invoke(null, new object[] { value } );
                    }
                }

                /// <summary>
                /// препатч <seealso cref="Astral.Quester.Core.Load(string, bool)"/>
                /// </summary>
                private static bool PrefixLoad(string Path, bool savePath = true)
                {
                    Load(Path);
                    return false;
                }

                /// <summary>
                /// Загрузка Quester-профиля, заданного <param name="profilePath"/>
                /// Аналог <seealso cref="Astral.Quester.Core.Load(string, bool)"/>
                /// </summary>
                public static void Load(string profilePath)
                {
                    if (string.IsNullOrEmpty(profilePath))
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            InitialDirectory = Directories.ProfilesPath,
                            DefaultExt = "amp.zip",
                            Filter = @"Astral mission profile (*.amp.zip)|*.amp.zip"
                        };

                        if (openFileDialog.ShowDialog() != DialogResult.OK)
                            return;

                        profilePath = openFileDialog.FileName;
                    }

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

                            Profile profile;
                            using (var stream = zipProfileEntry.Open())
                            {
                                Astral_Functions_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types, 2);
                                XmlSerializer serializer = new XmlSerializer(typeof(Profile), types.ToArray());
                                profile = serializer.Deserialize(stream) as Profile;

                                if (profile is null)
                                {
                                    Astral.Logger.Notify($"Unable to load {profilePath}'. Deserialization of 'profile.xml' failed", true);
                                    //Astral.Logger.WriteLine("Unable to load " + profilePath);
                                    return;
                                }
                            }

#if load_mapMeshes
                            var mapName = EntityManager.LocalPlayer.MapState.MapName;
                            if (string.IsNullOrEmpty(mapName))
                                return;
                            var mapMeshesName = mapName + ".bin";
                            Graph meshes = null;

                            ZipArchiveEntry zipMeshEntry = zipFile.GetEntry(mapMeshesName);
                            if (zipMeshEntry != null)
                            {
                                using (var stream = zipMeshEntry.Open())
                                {
                                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                                    if (binaryFormatter.Deserialize(stream) is Graph meshesFromFile)
                                        meshes = meshesFromFile;
                                }
                            }
                            //else Astral.Logger.Notify($"File '{Path.GetFileName(profilePath)}' does not contain '{mapMeshesName}'");  
#endif

                            Profile.ResetCompleted();
                            Profile = profile;
                            Astral.API.CurrentSettings.LastQuesterProfile = profilePath;
                            // TODO Проверить не приводит ли следующая строка к некорректной работе AddIgnoredFoes
                            AstralAccessors.Logic.NW.Combats.BLAttackersList = null;


                            var allProfileMeshes = _mapsMeshes;
                            lock (allProfileMeshes)
                            {
                                allProfileMeshes.Clear();
#if load_mapMeshes
                                if (meshes != null)
                                    allProfileMeshes.Add(mapName, meshes);
#endif
                            } 

                            if (AstralAccessors.Controllers.BotComs.BotServer.Server.IsRunning)
                            {
                                AstralAccessors.Controllers.BotComs.BotServer.SendQuesterProfileInfos();
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

                private static bool PrefixSave(bool saveas = false)
                {
                    Save(saveas);
                    return false;
                }

                /// <summary>
                /// Сохранение quester-профиля
                /// Аналог <seealso cref="Astral.Quester.Core.Save(bool)"/>
                /// </summary>
                public static void Save(bool saveAs = false)
                {
                    string fullProfileName = Astral.API.CurrentSettings.LastQuesterProfile;
                    string profileName;
                    if (string.IsNullOrEmpty(fullProfileName))
                    {
                        profileName = EntityManager.LocalPlayer.MapState.MapName;
                    }
                    else
                    {
                        profileName = Path.GetFileName(fullProfileName);
                        if (profileName.EndsWith("amp.zip"))
                            profileName = profileName.Substring(0, profileName.Length - 8);
                    }

                    
                    string dirName = Path.GetDirectoryName(fullProfileName);
                    if (string.IsNullOrEmpty(dirName))
                        dirName = Directories.ProfilesPath;
                    var currentProfile = Profile;

                    bool useExternalMeshes = currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= ".meshes.bin".Length + 1;

                    string externalMeshFileName = string.Empty;
                    bool needLoadAllMeshes = false;

                    if (!currentProfile.Saved || saveAs)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog
                        {
                            InitialDirectory = dirName,
                            DefaultExt = "amp.zip",
                            Filter = @"Astral mission profile (*.amp.zip)|*.amp.zip",
                            FileName = string.IsNullOrEmpty(profileName)
                                ? EntityManager.LocalPlayer.MapState.MapName
                                : profileName
                        };
                        if (saveFileDialog.ShowDialog() != DialogResult.OK)
                            return;
                        fullProfileName = saveFileDialog.FileName;
                        needLoadAllMeshes = true;
                    }
                    if (useExternalMeshes)
                        externalMeshFileName = Path.Combine(Path.GetDirectoryName(fullProfileName) ?? string.Empty, currentProfile.ExternalMeshFileName);

                    if (needLoadAllMeshes && !useExternalMeshes)
                        AstralAccessors.Quester.Core.LoadAllMeshes();

                    ZipArchive zipFile = null;
                    try
                    {

                        // Открываем архивный файл профиля
                        zipFile = ZipFile.Open(fullProfileName, File.Exists(fullProfileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                        // Сохраняем в архив файл профиля "profile.xml"
                        lock (currentProfile)
                        {
                            if (SaveProfile(zipFile))
                                currentProfile.Saved = true;
                        }

                        // Сохраняем файлы мешей
                        if (useExternalMeshes)
                        {
                            // Если используется внешние меши
                            // в файле профиля нужно удалить все "лишние" файлы
                            if (zipFile.Mode == ZipArchiveMode.Update)
                                foreach (var entry in zipFile.Entries)
                                {
                                    if (!entry.Name.Equals("profile.xml", StringComparison.OrdinalIgnoreCase))
                                        entry.Delete();
                                }
                            // закрываем архив профиля
                            zipFile.Dispose();

                            // открываем архив с внешними мешами
                            zipFile = ZipFile.Open(externalMeshFileName,
                                File.Exists(externalMeshFileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);
                        }

                        // сохраняем меши в архивный файл
                        lock (_mapsMeshes)
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            foreach (var mesh in _mapsMeshes)
                            {
                                // удаляем мусор (скрытые вершины и ребра)
                                mesh.Value.RemoveUnpassable();

                                string meshName = mesh.Key + ".bin";

                                SaveMesh(zipFile, meshName, mesh.Value, binaryFormatter);
                            }
                        }

                        API.CurrentSettings.LastQuesterProfile = fullProfileName;

                        Logger.Notify(string.Concat("Profile '", fullProfileName, "' saved"));
                    }
                    catch (Exception exc)
                    {
                        Logger.WriteLine(Logger.LogType.Debug, exc.ToString());
                        Logger.Notify(string.Concat("Profile '", fullProfileName, "' saved"), true);
                    }
                    finally
                    {
                        zipFile?.Dispose();
                    }
                }

                /// <summary>
                /// Сохранение мешей <paramref name="mesh"/> в архивный файл <paramref name="zipFile"/> под именем <paramref name="meshName"/>
                /// </summary>
                public static bool SaveMesh(ZipArchive zipFile, string meshName, Graph mesh, BinaryFormatter binaryFormatter = null)
                {
                    //TODO: Безопасное сохранение mesh'а, чтобы при возникновении ошибки старое содержимое не удалялось
                    //TODO: Исправить сохранение внешних мешей
                    if (zipFile is null)
                        return false;

                    if (binaryFormatter is null)
                        binaryFormatter = new BinaryFormatter();

                    try
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            binaryFormatter.Serialize(memoryStream, mesh);

                            ZipArchiveEntry zipMeshEntry;
                            if (zipFile.Mode == ZipArchiveMode.Update)
                            {
                                zipMeshEntry = zipFile.GetEntry(meshName);
                                if (zipMeshEntry != null)
                                    zipMeshEntry.Delete();
                            }
                            zipMeshEntry = zipFile.CreateEntry(meshName);

                            using (var zipMeshStream = zipMeshEntry.Open())
                            {
                                memoryStream.WriteTo(zipMeshStream);
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, e.ToString());
                    }

                    return false;
                }

                /// <summary>
                /// Сохранение профиля в архивный файл <paramref name="zipFile"/>
                /// </summary>
                public static bool SaveProfile(ZipArchive zipFile)
                {
                    //TODO: Безопасное сохранение профиля, чтобы при возникновении ошибки старое содержимое не удалялось
                    var currentProfile = Profile;

                    Astral_Functions_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types, 2);
                    XmlSerializer serializer = new XmlSerializer(currentProfile.GetType(), types.ToArray());

                    using (var memStream = new MemoryStream())
                    {
                        try
                        {
                            currentProfile.Saved = true;
                            serializer.Serialize(memStream, currentProfile);

                            ZipArchiveEntry zipProfileEntry;
                            if (zipFile.Mode == ZipArchiveMode.Update)
                            {
                                zipProfileEntry = zipFile.GetEntry("profile.xml");
                                zipProfileEntry?.Delete();
                            }
                            zipProfileEntry = zipFile.CreateEntry("profile.xml");
                            using (var zipProfileStream = zipProfileEntry.Open())
                            {
                                memStream.WriteTo(zipProfileStream);
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            currentProfile.Saved = false;
                            Astral.Logger.WriteLine(Astral.Logger.LogType.Debug, e.ToString());
                        }
                    }
                    return false;
                }
#endregion

#region Events
#if ProfileNewLoadEvents
                /// <summary>
                /// Делегат, вызываемый перед вызовом <seealso cref="Astral.Quester.Core.Load(string Path, bool savePath = true)"/>,
                /// то есть перед загрузкой Quester-профиля
                /// </summary>
                public delegate void BeforeLoadEvent(ref string Path);
                public static event BeforeLoadEvent BeforeLoad;
                private static bool PrefixLoad(ref string Path, bool savePath)
                {
                    BeforeLoad?.Invoke(ref Path);
                    return true;
                }

                /// <summary>
                /// Делегат, вызываемый после вызова <seealso cref="Astral.Quester.Core.Load(string Path, bool savePath = true)"/>,
                /// то есть после загрузки Quester-профиля
                /// </summary>
                public delegate void AfterLoadEvent(string path);
                public static event AfterLoadEvent AfterLoad;
                private static void PostfixLoad(string Path, bool savePath)
                {
                    _mapsMeshes.Clear();
                    AfterLoad?.Invoke(Path);
                }

                /// <summary>
                /// Делегат, вызываемый перед вызовом <seealso cref="Astral.Quester.Core.New()"/>,
                /// то есть перед созданием нового Quester-профиля
                /// </summary>
                public delegate void BeforeNewEvent();
                public static event BeforeNewEvent BeforeNew;
                private static bool PrefixNew()
                {
                    BeforeNew?.Invoke();
                    return true;
                }

                /// <summary>
                /// Делегат, вызываемый после вызова <seealso cref="Astral.Quester.Core.New()"/>,
                /// то есть после создания нового Quester-профиля
                /// </summary>
                public delegate void AfterNewEvent();
                public static event AfterNewEvent AfterNew;
                private static void PostfixNew()
                {
                    _mapsMeshes.Clear();
                    AfterNew?.Invoke();
                }
#endif
                public delegate void ProfileChangedEvent();
                /// <summary>
                /// Событие, происходящее после изменение <seealso cref="Astral.Quester.Core.Profile"/>
                /// </summary>
                public static event ProfileChangedEvent OnProfileChanged;

                private static void PostfixSetProfile(Profile value)
                {
                    _mapsMeshes.Clear();
                    OnProfileChanged?.Invoke();
                }
#endregion

                internal static void ApplyPatches() { }

                static Core()
                {
                    var tCore = typeof(Astral.Quester.Core);
                    var tPatch = typeof(Core);
#if ProfileNewLoadEvents
                    var originalLoad = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(tPatch, nameof(PrefixLoad));
                    var postfixLoad = AccessTools.Method(tPatch, nameof(PostfixLoad));

                    if (originalLoad != null
                        && postfixLoad != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad), new HarmonyMethod(postfixLoad));
                    }

                    var originalNew = AccessTools.Method(tCore, nameof(Astral.Quester.Core.New));
                    var prefixNew = AccessTools.Method(tPatch, nameof(PrefixNew));
                    var postfixNew = AccessTools.Method(tPatch, nameof(PostfixNew));

                    if (originalNew != null
                        && postfixNew != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixNew), new HarmonyMethod(postfixNew));
                    } 
#endif
                    propProfile = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Profile));
                    originalSetProfile = propProfile.GetSetMethod(true);
                    postfixSetProfile = AccessTools.Method(tPatch, nameof(PostfixSetProfile));

                    if (originalSetProfile != null
                        && postfixSetProfile != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalSetProfile, null, new HarmonyMethod(postfixSetProfile));
                    }

                    var propMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Meshes));
                    var originalGetMeshes = propMeshes.GetGetMethod(true);
                    var prefixGetMeshes = AccessTools.Method(tPatch, nameof(Core.PrefixGetMeshes));

                    if (originalGetMeshes != null
                        && prefixGetMeshes != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalGetMeshes, new HarmonyMethod(prefixGetMeshes));
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' succeeded");
                    }
                    else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' failed");

                    var propMapsMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.MapsMeshes));
                    var originalGetMapsMeshes = propMapsMeshes.GetGetMethod(true);
                    var originalSetMapsMeshes = propMapsMeshes.GetSetMethod(true);
                    var prefixGetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixGetMapsMeshes));
                    var prefixSetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixSetMapsMeshes));
                    if (originalGetMapsMeshes != null
                        && prefixGetMapsMeshes != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalGetMapsMeshes, new HarmonyMethod(prefixGetMapsMeshes));
                        Astral.Logger.WriteLine(Logger.LogType.Debug,
                            $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Astral.Logger.WriteLine(Logger.LogType.Debug,
                        $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    if (originalSetMapsMeshes != null
                        && prefixSetMapsMeshes != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalSetMapsMeshes, new HarmonyMethod(prefixSetMapsMeshes));
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    var originalLoad = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(tPatch, nameof(PrefixLoad));
                    if (originalLoad != null &&
                        prefixLoad != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad));
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' succeeded");
                    }
                    else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' failed");

                    var originalSave = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Save));
                    var prefixSave = AccessTools.Method(tPatch, nameof(PrefixSave));
                    if (originalSave != null &&
                        prefixSave != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalSave, new HarmonyMethod(prefixSave));
                        Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' succeeded");
                    }
                    else Astral.Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' failed");

                }
            }

#if false
            public static class Entrypoint
            {
#if false
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction"); 
#endif
                public delegate void LoadRoleEvent(Astral.Quester.Entrypoint entrypoint);

                private static MethodInfo originalOnLoadMethod;
                private static MethodInfo prefixOnLoadMethod;
                private static MethodInfo postfixOnLoadMethod;

                /// <summary>
                /// Событие, вызываемое перед загрузкой квестера
                /// </summary>
                public static event LoadRoleEvent BeforeLoadRole;

                private static bool prefixOnLoad(Astral.Quester.Entrypoint __instance)
                {
                    BeforeLoadRole?.Invoke(__instance);
                    return true;
                }

                /// <summary>
                /// Событие, вызываемое после загрузки квестера
                /// </summary>
                public static event LoadRoleEvent AfterLoadRole;

                private static bool postfixOnLoad(Astral.Quester.Entrypoint __instance)
                {
                    AfterLoadRole?.Invoke(__instance);
                    return true;
                }

                static Entrypoint()
                {
                    var tEntrypoint = typeof(Astral.Quester.Entrypoint);
                    var tPatch = typeof(Entrypoint);

                    originalOnLoadMethod = AccessTools.Method(tEntrypoint, "OnLoad");
                    prefixOnLoadMethod = AccessTools.Method(tEntrypoint, nameof(prefixOnLoad));
                    postfixOnLoadMethod = AccessTools.Method(tEntrypoint, nameof(postfixOnLoad));

                    if (originalOnLoadMethod != null
                        && prefixOnLoadMethod != null
                        && postfixOnLoadMethod != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalOnLoadMethod, new HarmonyMethod(prefixOnLoadMethod), new HarmonyMethod(postfixOnLoadMethod));
                    }
                }
            }  
#endif

            public static class Forms
            {
                public static class Editor
                {
                    /// <summary>
                    /// Функтор доступа к экземпляру Квестер-редактора
                    /// Astral.Quester.Forms.Main.editorForm
                    /// </summary>
                    static readonly StaticFieldAccessor<Astral.Quester.Forms.Editor> editorForm = typeof(Astral.Quester.Forms.Main).GetStaticField<Astral.Quester.Forms.Editor>("editorForm");
                    public static Astral.Quester.Forms.Editor EditorForm => editorForm.Value;

                    // Метод Astral.Quester.Forms.Editor.RefreshRegions() является публичным
                    //public void RefreshRegions() => editorForm.Value?.RefreshRegions();
                }
            }
        }
    }
}