using ACTP0Tools.Classes.Quester;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using AStar;
using Astral;
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
using ACTP0Tools.Classes;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment
// ReSharper disable RedundantNameQualifier

namespace ACTP0Tools
{
    /// <summary>
    /// Доступ к закрытым членам и методам Astral'a
    /// </summary>
    public static partial class AstralAccessors
    {
        /// <summary>
        /// Доступ к закрытым членам Astral.Quester
        /// </summary>
        public static partial class Quester
        {
            public static class Core
            {
                #region Meshes
                /// <summary>
                /// Функтор доступа к графу путей (карте) текущего профиля
                /// </summary>
                public static Graph Meshes
                {
                    get
                    {
                        Graph graph = null;
                        PrefixGetMeshes(ref graph);
                        return graph;
                    }
                }
                private static bool PrefixGetMeshes(ref Graph __result)
                {
                    var mapsMeshes = _mapsMeshes;
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
                    if (!File.Exists(zipFileName)) 
                        return false;

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

                // TODO Использовать RWLocker для синхронизации доступа к _mapsMeshes
                //private static readonly AStar.Tools.RWLocker mapsMeshesLocker = new RWLocker();

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

                /// <summary>
                /// Дозагрузка недостающих карт <paramref name="mapsMeshes"/> из файла <paramref name="meshesFileName"/>.
                /// </summary>
                /// <returns>Количество загруженных карт</returns>
                public static int LoadAllMeshes(ref Dictionary<string, Graph> mapsMeshes, string meshesFileName)
                {
                    var zipFileName = meshesFileName;
                    int result = 0;
                    if (string.IsNullOrEmpty(meshesFileName) || !File.Exists(zipFileName)) return result;

                    if (mapsMeshes is null)
                        mapsMeshes = new Dictionary<string, Graph>();

                    try
                    {
                        using (var zipFile = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
                        {
                            lock (mapsMeshes)
                            {
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
                                        var binaryFormatter = new BinaryFormatter();
                                        if (binaryFormatter.Deserialize(stream) is Graph meshesFromFile)
                                        {
                                            mapsMeshes.Add(mapName, meshesFromFile);
                                            result++;
                                        }
                                    }
                                } 
                            }

                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                } 

                private static bool PrefixLoadAllMeshes(out int __result)
                {
                    __result = LoadAllMeshes(ref _mapsMeshes, CurrentProfileZipMeshFile);
                    return false;
                }
                #endregion

                #region Profile
                private static readonly MethodInfo originalSetProfile;

                //private static readonly SaveFileDialog saveFileDialog = new SaveFileDialog
                //{
                //    InitialDirectory = Astral.Controllers.Directories.ProfilesPath,
                //    DefaultExt = "amp.zip",
                //    Filter = @"Astral mission profile (*.amp.zip)|*.amp.zip",
                //};
                //private static readonly OpenFileDialog openFileDialog = new OpenFileDialog
                //{
                //    InitialDirectory = Directories.ProfilesPath,
                //    DefaultExt = "amp.zip",
                //    Filter = @"Astral mission profile (*.amp.zip)|*.amp.zip"
                //};

                private static XmlSerializer profileXmlSerializer
                {
                    get
                    {
                        if (_profileXmlSerializer is null)
                        {
                            ACTP0Serializer.GetExtraTypes(out List<Type> types, 2);
                            _profileXmlSerializer = new XmlSerializer(typeof(Profile), types.ToArray());
                        }
                        return _profileXmlSerializer;
                    }
                }
                private static XmlSerializer _profileXmlSerializer;

                /// <summary>
                /// Ссылка на активный quester-профиль, выполняемый в роли Quester
                /// </summary>
                public static Profile Profile
                {
                    get => Astral.Quester.API.CurrentProfile;
                    set
                    {
                        if (value is null)
                            throw new ArgumentNullException(nameof(value));
                        originalSetProfile?.Invoke(null, new object[] { value });
                        ActiveProfileProxy.SetProfile(value, null);
                    }
                }

                /// <summary>
                /// Прокси-объект, опосредующий доступ к активному quester-профилю, выполняемому в роли Quester
                /// </summary>
                public static ActiveProfileProxy ActiveProfileProxy => ActiveProfileProxy.Get();


                public static string GetFullPathOfExternalMapsMeshes(string profilePath, string meshesFile)
                {
                    profilePath = Path.GetFullPath(profilePath);
                    string profileDir = Path.GetDirectoryName(profilePath);
                    if (string.IsNullOrEmpty(profileDir))
                        profileDir = Astral.Controllers.Directories.ProfilesPath;
                    
                    meshesFile = Path.GetFullPath(Path.Combine(profileDir, meshesFile));
                    return meshesFile;
                }

                /// <summary>
                /// Имя файла загруженного (выполняемого) Quester-профиля (без пути)
                /// </summary>
                public static string CurrentProfileName
                {
                    get => _currentProfileName.Value;
                    set => _currentProfileName.Value = value;
                }
                private static readonly StaticPropertyAccessor<string> _currentProfileName;

                /// <summary>
                /// Объеявление делегата, вызываемого в случае ошибки загрузки quester-профиля
                /// </summary>
                /// <param name="profile">Поток исходного файла профиля</param>
                /// <param name="modifiedProfile">Поток модифицированного (исправленного) файла профиля.</param>
                /// <param name="save">Флаг необходимости замены оригинального файла на модифицированный</param>
                /// <returns>Флаг успешности преобразования</returns>
                public delegate bool QuesterProfileLoadErrorEvent(Stream profile, out Stream modifiedProfile, out bool save);

                /// <summary>
                /// препатч <seealso cref="Astral.Quester.Core.Load(string, bool)"/>
                /// </summary>
                internal static bool PrefixLoad(string Path, bool savePath = true)
                {
                    var path = Path;
                    var profile = Load(ref path);

                    if (profile != null)
                    {
                        Profile?.ResetCompleted();
                        Profile = profile;

                        API.CurrentSettings.LastQuesterProfile = path;
                        // TODO Проверить не приводит ли следующая строка к некорректной работе AddIgnoredFoes
                        Logic.NW.Combats.BLAttackersList = null;


                        var allProfileMeshes = _mapsMeshes;
                        lock (allProfileMeshes)
                        {
                            allProfileMeshes.Clear();
                        }

                        if (Controllers.BotComs.BotServer.Server.IsRunning)
                        {
                            Controllers.BotComs.BotServer.SendQuesterProfileInfos();
                        } 
                    }

                    return false;
                }

                /// <summary>
                /// Загрузка Quester-профиля, заданного <param name="profilePath"/> в 
                /// Аналог <seealso cref="Astral.Quester.Core.Load(string, bool)"/>
                /// </summary>
                public static Profile Load(ref string profilePath)
                {
                    if (string.IsNullOrEmpty(profilePath))
                    {
                        var openDialog = FileTools.GetOpenDialog(initialDir: Astral.Controllers.Directories.ProfilesPath,
                                                                 filter: @"Astral mission profile (*.amp.zip)|*.amp.zip", defaultExtension: "amp.zip");
                        if (openDialog.ShowDialog() != DialogResult.OK)
                            return null;

                        profilePath = openDialog.FileName;
                    }

                    int readTries = 2;
                    while (readTries > 0)
                    {
                        try
                        {
                            int mods = 0;
                            Profile profile;
                            using (var zipFile = ZipFile.Open(profilePath, ZipArchiveMode.Read))
                            {
                                ZipArchiveEntry zipProfileEntry = zipFile.GetEntry("profile.xml");
                                if (zipProfileEntry is null)
                                {
                                    Logger.Notify(
                                        $"File '{Path.GetFileName(profilePath)}' does not contain 'profile.xml'", true);
                                    return null;
                                }

                                using (var stream = zipProfileEntry.Open())
                                {
                                    //ACTP0Serializer.GetExtraTypes(out List<Type> types, 2);
                                    //XmlSerializer serializer = new XmlSerializer(typeof(Profile), types.ToArray());
                                    if (Preprocessor.Active)
                                    {
                                        using (var profileStream = Preprocessor.Replace(stream, out mods))
                                        {
                                            profile = profileXmlSerializer.Deserialize(profileStream) as Profile;
                                            if (mods > 0)
                                                Logger.Notify(
                                                    $"There are {mods} modifications in the profile '{Path.GetFileName(profilePath)}'");
                                        }
                                    }
                                    else profile = profileXmlSerializer.Deserialize(stream) as Profile;

                                    if (profile is null)
                                    {
                                        Logger.Notify(
                                            $"Unable to load {profilePath}'. Deserialization of 'profile.xml' failed",
                                            true);
                                        return null;
                                    }
                                }
                            }

                            if (Preprocessor.Active && Preprocessor.AutoSave && mods > 0)
                            {
                                // Сохраняем преобразованный файл профиля
                                using (var zipFile = ZipFile.Open(profilePath, ZipArchiveMode.Update))
                                {
                                    SaveProfile(profile, zipFile);
                                }
                            }
                            return profile;
                        }
                        catch (ThreadAbortException ex)
                        {
                            Logger.Notify(ex.ToString(), true);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Logger.Notify(ex.ToString(), true);
                        }
                        Thread.Sleep(500);
                        readTries--;
                    }

                    return null;
                }

                private static bool PrefixSave(bool saveas = false)
                {
                    string currentProfileName = Astral.API.CurrentSettings.LastQuesterProfile;
                    string profileName = saveas ? string.Empty : currentProfileName;
                    Save(Profile, _mapsMeshes, currentProfileName, ref profileName);
                    return false;
                }

                public static void Save(bool saveas = false)
                {
                    string currentProfileName = Astral.API.CurrentSettings.LastQuesterProfile;
                    string profileName = saveas ? string.Empty : currentProfileName;
                    Save(Profile, _mapsMeshes, currentProfileName, ref profileName);
                }

                /// <summary>
                /// Сохранение quester-профиля.<br/>
                /// Аналог <seealso cref="Astral.Quester.Core.Save(bool)"/>.
                /// </summary>
                /// <param name="profile">Профиль, сохраняемый в файл.</param>
                /// <param name="profileMeshes">Набор путевых графов профиля.</param>
                /// <param name="currentProfileName">Имя файла, содержащего профиль.</param>
                /// <param name="newProfileName">Имя файла, в который должен быть сохранен профиль.<br/>
                /// Если имя файла не задано, то открывается диалог сохранения файла для того, чтобы пользователь .</param>
                public static bool Save(Profile profile, Dictionary<string, Graph> profileMeshes, string currentProfileName, ref string newProfileName)
                {
                    if (profile is null)
                        return false;

                    bool needLoadAllInternalMeshes = false;
                    if (string.IsNullOrEmpty(currentProfileName) || string.IsNullOrEmpty(newProfileName))
                    {
                        newProfileName = RequestUserForNewProfileFileName(currentProfileName);
                        if (string.IsNullOrEmpty(newProfileName)) 
                            return false;
                        needLoadAllInternalMeshes = currentProfileName != newProfileName;
                    }

                    bool useExternalMeshes = profile.UseExternalMeshFile
                                          && profile.ExternalMeshFileName.Length >= ".mesh.bin".Length + 1;

                    string externalMeshesFullFilePath = string.Empty;
                    if (useExternalMeshes)
                    {
                        string currentDir = string.Empty;    
                        if (!string.IsNullOrEmpty(currentProfileName))
                            currentDir = Path.GetDirectoryName(currentProfileName);
                        if (string.IsNullOrEmpty(currentDir))
                            currentDir = Astral.Controllers.Directories.ProfilesPath;

                        externalMeshesFullFilePath = Path.GetFullPath(Path.Combine(currentDir,  profile.ExternalMeshFileName));

                        // изменяем путь к файлу внешних мешей, поскольку новое имя файла профиля отличается от текущего
                        if (currentProfileName != newProfileName)
                        {
                            var newRelativeExternalMeshesFilePath =
                                newProfileName.GetRelativePathTo(externalMeshesFullFilePath);
                            if (!string.IsNullOrEmpty(newRelativeExternalMeshesFilePath))
                                profile.ExternalMeshFileName = newRelativeExternalMeshesFilePath;
                        }
                    }
                    else if (needLoadAllInternalMeshes)
                        LoadAllMeshes(ref profileMeshes, externalMeshesFullFilePath);

                    ZipArchive zipFile = null;
                    try
                    {
                        // Открываем архивный файл профиля
                        zipFile = ZipFile.Open(newProfileName, File.Exists(newProfileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                        // Сохраняем в архив файл профиля "profile.xml"
                        lock (profile)
                        {
                            profile.Saved = true;
                            if (!SaveProfile(profile, zipFile))
                            {
                                profile.Saved = false;
                                return false;
                            }
                        }

                        var binaryFormatter = new BinaryFormatter();
                        // Сохраняем файлы путевых графов (меши)
                        lock (profileMeshes)
                        {
                            if (useExternalMeshes)
                            {
                                // открываем архив с внешними путевыми графами
                                using (var externalMeshesZipFile = ZipFile.Open(externalMeshesFullFilePath,
                                    File.Exists(externalMeshesFullFilePath) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                                {
                                    // сохраняем все загруженные меши во внешний архивный файл
                                    SaveAllMeshes(externalMeshesZipFile, profileMeshes, binaryFormatter);

                                    // Копируем файлы мешей из файла профиля во внешний архив
                                    if (zipFile.Mode != ZipArchiveMode.Create)
                                    {
                                        // 1/ Нельзя обращаться к zipFile.Entries в режиме ZipArchiveMode.Create
                                        // 2/ Нельзя использовать итератор (foreach) для удаления мешей из исходного архива
                                        for (int i = 0; i < zipFile.Entries.Count;)
                                        {
                                            var entry = zipFile.Entries[i];

                                            // Если используется внешние меши, в файле профиля нужно удалить все "лишние" файлы
                                            var entryName = entry.Name;
                                            if (!entryName.Equals("profile.xml", StringComparison.OrdinalIgnoreCase)
                                                && entryName.EndsWith(".bin"))
                                            {
                                                var meshName = entryName.Substring(0, entryName.Length - ".bin".Length);
                                                if (!profileMeshes.ContainsKey(meshName))
                                                {
                                                    // Меш карты, соответствующей entry, ОТСУТСТВУЕТ в currentProfileMeshes 
                                                    // и не был сохранен во внешний архивный файл
                                                    ZipArchiveEntry externalMeshEntry = null;
                                                    if (externalMeshesZipFile.Mode == ZipArchiveMode.Update)
                                                        externalMeshEntry = externalMeshesZipFile.GetEntry(entryName);
                                                    if (externalMeshEntry is null)
                                                        externalMeshEntry = externalMeshesZipFile.CreateEntry(entryName);

                                                    // копирование меша из архива профиля во внешний архив 
                                                    using (var internalMeshStream = entry.Open())
                                                    using (var externalMeshStream = externalMeshEntry.Open())
                                                    {
                                                        internalMeshStream.CopyTo(externalMeshStream);
                                                    }
                                                }

                                                // Удаление скопированного (дублирующегося) меша из архива профиля
                                                if (zipFile.Mode == ZipArchiveMode.Update)
                                                {
                                                    entry.Delete();
                                                    continue;
                                                }
                                            }
                                            i++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // сохраняем путевые графы (меши) в архив профиля
                                SaveAllMeshes(zipFile, profileMeshes, binaryFormatter);
                            }
                        }

                        Logger.Notify($"Profile '{newProfileName}' saved");

                        return profile.Saved;
                    }
                    catch (Exception exc)
                    {
                        Logger.Notify($"Catch an exception while saving profile '{newProfileName}':\n{exc}", true);
                    }
                    finally
                    {
                        zipFile?.Dispose();
                    }

                    return false;
                }

                private static string  RequestUserForNewProfileFileName(string currentProfileName)
                {
                    string profileNameWithoutExtension = currentProfileName;
                    if (string.IsNullOrEmpty(profileNameWithoutExtension))
                    {
                        profileNameWithoutExtension = EntityManager.LocalPlayer.MapState.MapName;
                    }
                    else
                    {
                        profileNameWithoutExtension = Path.GetFileName(currentProfileName);
                        if (profileNameWithoutExtension.EndsWith("amp.zip"))
                            profileNameWithoutExtension =
                                profileNameWithoutExtension.Substring(0, profileNameWithoutExtension.Length - 8);
                    }

                    var saveDialog = FileTools.GetSaveDialog(fileName: profileNameWithoutExtension,
                        filter: @"Astral mission profile (*.amp.zip)|*.amp.zip",
                        defaultExtension: "amp.zip");

                    if (saveDialog.ShowDialog() != DialogResult.OK)
                        return string.Empty;
                    return saveDialog.FileName;
                }
                private static void SaveAllMeshes(ZipArchive zipArchive, Dictionary<string, Graph> meshes, BinaryFormatter binFormatter)
                {
                    foreach (var mesh in meshes)
                    {
                        // удаляем мусор (скрытые вершины и ребра)
                        mesh.Value.RemoveUnpassable();

                        string meshName = mesh.Key + ".bin";

                        SaveMesh(zipArchive, meshName, mesh.Value, binFormatter);
                    }
                }

                /// <summary>
                /// Сохранение мешей <paramref name="mesh"/> в архивный файл <paramref name="zipFile"/> под именем <paramref name="meshName"/>
                /// </summary>
                private static bool SaveMesh(ZipArchive zipFile, string meshName, Graph mesh, BinaryFormatter binaryFormatter = null)
                {
                    if (zipFile is null)
                        return false;

                    if (zipFile.Mode != ZipArchiveMode.Update
                        && zipFile.Mode != ZipArchiveMode.Create)
                        return false;

                    //TODO: Безопасное сохранение mesh'а, чтобы при возникновении ошибки старое содержимое не удалялось
                    //TODO: Исправить сохранение внешних мешей

                    if (binaryFormatter is null)
                        binaryFormatter = new BinaryFormatter();

                    try
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            binaryFormatter.Serialize(memoryStream, mesh);

                            ZipArchiveEntry zipMeshEntry = null;

                            bool upgrading = true;
                            if (zipFile.Mode == ZipArchiveMode.Update)
                                zipMeshEntry = zipFile.GetEntry(meshName);
                            if (zipMeshEntry == null)
                            {
                                zipMeshEntry = zipFile.CreateEntry(meshName);
                                upgrading = false;
                            }

                            using (var zipMeshStream = zipMeshEntry.Open())
                            {
                                if (upgrading)
                                    zipMeshStream.SetLength(memoryStream.Length);
                                memoryStream.WriteTo(zipMeshStream);
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(Logger.LogType.Debug, e.ToString());
                    }

                    return false;
                }

                /// <summary>
                /// Сохранение профиля в архивный файл <paramref name="zipFile"/>
                /// </summary>
                private static bool SaveProfile(Profile profile, ZipArchive zipFile)
                {
                    if (zipFile.Mode != ZipArchiveMode.Update
                        && zipFile.Mode != ZipArchiveMode.Create)
                        return false;

                    //TODO: Безопасное сохранение профиля, чтобы при возникновении ошибки старое содержимое не удалялось
                    //ACTP0Serializer.GetExtraTypes(out List<Type> types, 2);
                    //XmlSerializer serializer = new XmlSerializer(profile.GetType(), types.ToArray());

                    using (var memStream = new MemoryStream())
                    {
                        try
                        {
                            profile.Saved = true;
                            profileXmlSerializer.Serialize(memStream, profile);

                            ZipArchiveEntry zipProfileEntry = null;

                            bool upgrading = true;
                            if (zipFile.Mode == ZipArchiveMode.Update)
                                zipProfileEntry = zipFile.GetEntry("profile.xml");
                            if (zipProfileEntry is null)
                            {
                                zipProfileEntry = zipFile.CreateEntry("profile.xml");
                                upgrading = false;
                            }

                            using (var zipProfileStream = zipProfileEntry.Open())
                            {
                                if (upgrading)
                                    zipProfileStream.SetLength(memStream.Length);
                                memStream.WriteTo(zipProfileStream);
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            profile.Saved = false;
                            Logger.WriteLine(Logger.LogType.Debug, e.ToString());
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
                    ActiveProfileProxy.SetProfile(value, null);
                }
                #endregion

                internal static void ApplyPatches() { }

                static Core()
                {
                    // TODO Пропатчить Core.LoadAllMeshes

                    var tCore = typeof(Astral.Quester.Core);
                    var tPatch = typeof(Core);
#if ProfileNewLoadEvents
                    var originalLoad = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(tPatch, nameof(PrefixLoad));
                    var postfixLoad = AccessTools.Method(tPatch, nameof(PostfixLoad));

                    if (originalLoad != null
                        && postfixLoad != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad), new HarmonyMethod(postfixLoad));
                    }

                    var originalNew = AccessTools.Method(tCore, nameof(Astral.Quester.Core.New));
                    var prefixNew = AccessTools.Method(tPatch, nameof(PrefixNew));
                    var postfixNew = AccessTools.Method(tPatch, nameof(PostfixNew));

                    if (originalNew != null
                        && postfixNew != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixNew), new HarmonyMethod(postfixNew));
                    } 
#endif
                    var propProfile = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Profile));
                    originalSetProfile = propProfile.GetSetMethod(true);
                    var postfixSetProfile = AccessTools.Method(tPatch, nameof(PostfixSetProfile));

                    if (originalSetProfile != null
                        && postfixSetProfile != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalSetProfile, null, new HarmonyMethod(postfixSetProfile));
                    }

                    var propMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Meshes));
                    var originalGetMeshes = propMeshes.GetGetMethod(true);
                    var prefixGetMeshes = AccessTools.Method(tPatch, nameof(Core.PrefixGetMeshes));

                    if (originalGetMeshes != null
                        && prefixGetMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalGetMeshes, new HarmonyMethod(prefixGetMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch the getter of the property 'Astral.Quester.Core.Meshes' failed");

                    var propMapsMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.MapsMeshes));
                    var originalGetMapsMeshes = propMapsMeshes.GetGetMethod(true);
                    var originalSetMapsMeshes = propMapsMeshes.GetSetMethod(true);
                    var prefixGetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixGetMapsMeshes));
                    var prefixSetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixSetMapsMeshes));
                    if (originalGetMapsMeshes != null
                        && prefixGetMapsMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalGetMapsMeshes, new HarmonyMethod(prefixGetMapsMeshes));
                        Logger.WriteLine(Logger.LogType.Debug,
                            $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug,
                        $"Patch the getter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    if (originalSetMapsMeshes != null
                        && prefixSetMapsMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalSetMapsMeshes, new HarmonyMethod(prefixSetMapsMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch the setter of the property 'Astral.Quester.Core.MapsMeshes' failed");

                    var originalLoad = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Load));
                    var prefixLoad = AccessTools.Method(tPatch, nameof(PrefixLoad));
                    if (originalLoad != null &&
                        prefixLoad != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoad, new HarmonyMethod(prefixLoad));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Load' failed");

                    var originalSave = AccessTools.Method(tCore, nameof(Astral.Quester.Core.Save));
                    var prefixSave = AccessTools.Method(tPatch, nameof(PrefixSave));
                    if (originalSave != null &&
                        prefixSave != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalSave, new HarmonyMethod(prefixSave));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.Save' failed");

                    var originalLoadAllMeshes = AccessTools.Method(tCore, "LoadAllMeshes");
                    var prefixLoadAllMeshes = AccessTools.Method(tPatch, nameof(PrefixLoadAllMeshes));
                    if (originalLoadAllMeshes != null &&
                        prefixLoadAllMeshes != null)
                    {
                        ACTP0Patcher.Harmony.Patch(originalLoadAllMeshes, new HarmonyMethod(prefixLoadAllMeshes));
                        Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.LoadAllMeshes' succeeded");
                    }
                    else Logger.WriteLine(Logger.LogType.Debug, $"Patch of 'Astral.Quester.Core.LoadAllMeshes' failed");

                    _currentProfileName = tCore.GetStaticProperty<string>(nameof(Astral.Quester.Core.CurrentProfileName));
                }
            }
        }
    }
}