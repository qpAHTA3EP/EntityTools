using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using AStar;
using Astral;
using Astral.Quester.Classes;
using Infrastructure.Classes;
using Infrastructure.Patches;
using Infrastructure.Reflection;
using MyNW.Internals;
using QuesterAction = Astral.Quester.Classes.Action;

namespace Infrastructure.Quester
{
    public static partial class QuesterHelper
    {
        private static readonly PropertyAccessor<ActionPlayer> GetActionsPlayer = typeof(ActionPack).GetProperty<ActionPlayer>("AP");

        /// <summary>
        /// Поиск в <paramref name="actionPack"/> команды с идентификатором <paramref name="searchedActionId"/>
        /// и установка её в качестве текущей исполняемой команды.
        /// </summary>
        public static void SetStartPoint(this ActionPack actionPack, Guid searchedActionId)
        {
            if (actionPack is null)
                return;
            actionPack.Reset();
            _setStartPointInternal(actionPack, searchedActionId);
        }
        private static QuesterAction _setStartPointInternal(ActionPack actionPack, Guid searchedActionId)
        {
            /*
                private Action SetStartPoint(ActionPack pack, Action startAction)
                {
                    pack.AP.CurrentActions.Clear();
	                foreach (Action action in pack.Actions)
	                {
		                if (action == startAction)
		                {
			                pack.AP.CurrentActions.Add(action);
			                return action;
		                }
		                if (action is ActionPack)
		                {
			                ActionPack pack2 = action as ActionPack;
			                Action action2 = this.SetStartPoint(pack2, startAction);
			                if (action2 != null)
			                {
				                pack.AP.CurrentActions.Add(action);
				                return action2;
			                }
		                }
		                action.SetCompleted(true, "");
	                }
	                pack.SetCompleted(true, "");
	                return null;
                }
             */

            if (actionPack is null)
                return null;
            var actionPlayer = GetActionsPlayer[actionPack];
            if (actionPlayer is null)
                return null;
            var playingActionList = actionPlayer.CurrentActions;
            playingActionList.Clear();
            foreach (var action in actionPack.Actions)
            {
                if (action.ActionID == searchedActionId)
                {
                    playingActionList.Add(action);
                    return action;
                }

                if (action is ActionPack innerActionPack)
                {
                    var foundedAction = _setStartPointInternal(innerActionPack, searchedActionId);
                    if (foundedAction != null)
                    {
                        playingActionList.Add(action);
                        return foundedAction;
                    }
                }
                action.SetCompleted(true, "");
            }
            actionPack.SetCompleted(true, "");
            return null;
        }




        public static void ResetActionPlayer(this ActionPack actionPack)
        {
            var actionPlayer = GetActionsPlayer[actionPack];
            if (actionPlayer is null)
                return;
            var playingActionList = actionPlayer.CurrentActions;
            playingActionList.Clear();
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
        /// Дозагрузка недостающих карт <paramref name="mapsMeshes"/> из файла <paramref name="meshesFileName"/>.
        /// </summary>
        /// <returns>Количество загруженных карт</returns>
        public static int LoadAllMeshes(IDictionary<string, Graph> mapsMeshes, string meshesFileName)
        {
            var zipFileName = meshesFileName;
            int result = 0;
            if (string.IsNullOrEmpty(meshesFileName) 
                || !File.Exists(zipFileName)) 
                return result;

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




        /// <summary>
        /// Сохранение quester-профиля.<br/>
        /// Аналог <seealso cref="Astral.Quester.Core.Save(bool)"/>.
        /// </summary>
        /// <param name="profile">Профиль, сохраняемый в файл.</param>
        /// <param name="profileMeshes">Набор путевых графов профиля.</param>
        /// <param name="currentProfileName">Имя файла, содержащего профиль.</param>
        /// <param name="newProfileName">Имя файла, в который должен быть сохранен профиль.<br/>
        /// Если имя файла не задано, то открывается диалог сохранения файла для того, чтобы пользователь .</param>
        public static bool Save(Profile profile, IDictionary<string, Graph> profileMeshes, string currentProfileName, ref string newProfileName)
        {
            if (profile is null)
                return false;

            string targetProfileName = _getProfileTargetFileName(currentProfileName, newProfileName);
            if (string.IsNullOrEmpty(targetProfileName))
                return false;

            
            string targetMeshesFileName = _getMeshesTargetFileName(profile, currentProfileName, targetProfileName);
            if (string.IsNullOrEmpty(targetMeshesFileName))
            {
                Logger.Notify($"Error of calculating filename for meshes.", true);
                return false;
            }

            if (!_saveMeshes(profileMeshes, targetMeshesFileName))
            {
                return false;
            }

            string meshesCurrentFileName = _getMeshesCurrentFileName(profile, currentProfileName);
            if (!string.IsNullOrEmpty(meshesCurrentFileName)
                && currentProfileName != targetProfileName
                && meshesCurrentFileName != targetMeshesFileName)
            {
                _copyMeshes(meshesCurrentFileName,
                            targetMeshesFileName,
                            name => !profileMeshes.ContainsKey(name));
            }

            if (!_saveProfile(profile, targetProfileName, targetMeshesFileName))
            {
                profile.Saved = false;
                return false;
            }

            Logger.Notify($"Profile '{targetProfileName}' saved");

            if (profile.Saved)
            {
                newProfileName = targetProfileName;
                return true;
            }

#if disabled_2023_03_13
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

                externalMeshesFullFilePath = Path.GetFullPath(Path.Combine(currentDir, profile.ExternalMeshFileName));

                // изменяем путь к файлу внешних мешей, поскольку новое имя файла профиля отличается от текущего
                if (currentProfileName != targetProfileName)
                {
                    var newRelativeExternalMeshesFilePath =
                        targetProfileName.GetRelativePathTo(externalMeshesFullFilePath);
                    if (!string.IsNullOrEmpty(newRelativeExternalMeshesFilePath))
                        profile.ExternalMeshFileName = newRelativeExternalMeshesFilePath;
                }
            }
            else if (needLoadAllInternalMeshes)
                LoadAllMeshes(profileMeshes, externalMeshesFullFilePath);

            ZipArchive zipFile = null;
            try
            {
                // Открываем архивный файл профиля
                zipFile = ZipFile.Open(targetProfileName, File.Exists(targetProfileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                // Сохраняем в архив файл профиля "profile.xml"
                if (!_saveProfile(profile, zipFile))
                {
                    return false;
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
                            _saveAllMeshes(externalMeshesZipFile, profileMeshes, binaryFormatter);

                            // Копируем файлы мешей из файла профиля во внешний архив
                            _copyIntoExternMeshes(profileMeshes, zipFile, externalMeshesZipFile);
                        }
                    }
                    else
                    {
                        // сохраняем путевые графы (меши) в архив профиля
                        _saveAllMeshes(zipFile, profileMeshes, binaryFormatter);
                    }
                }

                _removeProfileBackups(zipFile);

                Logger.Notify($"Profile '{targetProfileName}' saved");

                if (profile.Saved)
                {
                    newProfileName = targetProfileName;
                    return true;
                }
            }
            catch (Exception exc)
            {
                profile.Saved = false;
                Logger.Notify($"Catch an exception while saving profile '{newProfileName}':\n{exc}", true);
            }
            finally
            {
                zipFile?.Dispose();
            }
#endif

            return false; 
        }




        private static string _getProfileTargetFileName(string currentProfileName, string newProfileName)
        {
            if (!string.IsNullOrEmpty(newProfileName))
                return newProfileName;

            string profileNameWithoutExtension;
            if (string.IsNullOrEmpty(currentProfileName)
                || !File.Exists(currentProfileName))
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

            var saveDialog = FileTools.GetSaveDialog(
                                                        fileName: profileNameWithoutExtension,
                                                        filter: @"Astral mission profile (*.amp.zip)|*.amp.zip",
                                                        defaultExtension: "amp.zip"
                                                    );

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return string.Empty;
            return saveDialog.FileName;
        }

        private static string _getMeshesCurrentFileName(Profile profile, string currentProfileName)
        {
            string currentProfileZipMeshFile;
            if (!string.IsNullOrEmpty(currentProfileName))
            {
                if (profile.UseExternalMeshFile && !string.IsNullOrEmpty(profile.ExternalMeshFileName))
                    currentProfileZipMeshFile = Path.Combine(Path.GetDirectoryName(currentProfileName) ?? string.Empty,
                        profile.ExternalMeshFileName);
                else currentProfileZipMeshFile = currentProfileName;
                if (!File.Exists(currentProfileZipMeshFile))
                    currentProfileZipMeshFile = string.Empty;
            }
            else currentProfileZipMeshFile = string.Empty;

            return currentProfileZipMeshFile;
        }

        private static string _getMeshesTargetFileName(Profile profile, string currentProfileName, string targetProfileName)
        {
            bool useExternalMeshes = profile.UseExternalMeshFile
                                  && profile.ExternalMeshFileName.Length >= ".mesh.bin".Length + 1;

            string meshesTargetFilePath = string.Empty;
            if (useExternalMeshes)
            {
                string currentDir = string.Empty;
                if (!string.IsNullOrEmpty(currentProfileName))
                    currentDir = Path.GetDirectoryName(currentProfileName);
                if (string.IsNullOrEmpty(currentDir))
                    currentDir = Astral.Controllers.Directories.ProfilesPath;

                meshesTargetFilePath = Path.GetFullPath(Path.Combine(currentDir, profile.ExternalMeshFileName));

#if false
                // изменяем путь к файлу внешних мешей, поскольку новое имя файла профиля отличается от текущего
                if (currentProfileName != targetProfileName)
                {
                    var newRelativeExternalMeshesFilePath =
                        targetProfileName.GetRelativePathTo(meshesTargetFilePath);
                    if (!string.IsNullOrEmpty(newRelativeExternalMeshesFilePath))
                        profile.ExternalMeshFileName = newRelativeExternalMeshesFilePath;
                } 
#endif
            }
            else meshesTargetFilePath = targetProfileName;

            return meshesTargetFilePath;
        }

        private static bool _saveMeshes(IDictionary<string, Graph> profileMeshes, string targetMeshesFileName)
        {
            var count = profileMeshes?.Count ?? 0;
            if (count == 0)
                return true;

            try
            {
                lock (profileMeshes)
                {
                    var binaryFormatter = new BinaryFormatter();
                    using (var meshesZipFile = ZipFile.Open(targetMeshesFileName,
                                File.Exists(targetMeshesFileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                    {
                        int savedNum = 0;
                        foreach (var mesh in profileMeshes)
                        {
                            // удаляем мусор (скрытые вершины и ребра)
                            mesh.Value.RemoveUnpassable();

                            string meshName = mesh.Key + ".bin";

                            if (!_saveMesh(meshesZipFile, meshName, mesh.Value, binaryFormatter))
                            {
                                ETLogger.WriteLine(LogType.Error, $"Error saving meshes '{meshName}' into file '{targetMeshesFileName}'.", true);
                                return false;
                            }
                            savedNum++;
                        }
                        if (savedNum == count)
                            return true;

                        ETLogger.WriteLine(LogType.Error, $"Only {savedNum} meshes are saved into file '{targetMeshesFileName}' than is less then expected {count}.", true);
                    }
                }
            }
            catch (Exception e)
            {
                ETLogger.WriteLine(LogType.Error, $"Catch an exception while saving meshes:\n{e}.", true);

                throw;
            }
            return false;
        }

#if disabled_2023_03_13
        /// <summary>
        /// Копируем файлы мешей из файла профиля во внешний архив
        /// </summary>
        /// <param name="profileMeshes"></param>
        /// <param name="zipFile"></param>
        /// <param name="externalMeshesZipFile"></param>
        private static void _copyIntoExternMeshes(IDictionary<string, Graph> profileMeshes, ZipArchive zipFile, ZipArchive externalMeshesZipFile)
        {
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
#endif

        private static bool _copyMeshes(string fromFileName, string targetFileName, Predicate<string> shouldCopy, bool shouldDeleteFromSource = false)
        {
            if (!File.Exists(fromFileName))
                return true;

            using (var fromZipFile = ZipFile.Open(fromFileName,
                shouldDeleteFromSource ? ZipArchiveMode.Update : ZipArchiveMode.Read))
            {
                using (var targetZipFile = ZipFile.Open(targetFileName,
                                    File.Exists(targetFileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                {
                    // 1/ Нельзя обращаться к zipFile.Entries в режиме ZipArchiveMode.Create
                    // 2/ Нельзя использовать итератор (foreach) для удаления мешей из исходного архива
                    for (int i = 0; i < fromZipFile.Entries.Count;)
                    {
                        var entry = fromZipFile.Entries[i];

                        var entryName = entry.Name;
                        if (!entryName.Equals("profile.xml", StringComparison.OrdinalIgnoreCase)
                            && entryName.EndsWith(".bin"))
                        {
                            var meshName = entryName.Substring(0, entryName.Length - ".bin".Length);
                            if (shouldCopy(meshName))
                            {
                                // Меш карты, соответствующей entry, ОТСУТСТВУЕТ в currentProfileMeshes 
                                // и не был сохранен во внешний архивный файл
                                ZipArchiveEntry externalMeshEntry = null;
                                if (targetZipFile.Mode == ZipArchiveMode.Update)
                                    externalMeshEntry = targetZipFile.GetEntry(entryName);
                                if (externalMeshEntry is null)
                                    externalMeshEntry = targetZipFile.CreateEntry(entryName);

                                // копирование меша из архива профиля во внешний архив 
                                using (var internalMeshStream = entry.Open())
                                {
                                    using (var externalMeshStream = externalMeshEntry.Open())
                                    {
                                        internalMeshStream.CopyTo(externalMeshStream);
                                    }
                                }
                            }

                            // Удаление скопированного (дублирующегося) меша из архива профиля
                            if (shouldDeleteFromSource)
                            {
                                entry.Delete();
                                continue;
                            }
                        }
                        i++;
                    }
                } 
            }

            return false;
        }

#if disabled_2023_03_13
        private static void _saveAllMeshes(ZipArchive zipArchive, IDictionary<string, Graph> meshes, BinaryFormatter binFormatter)
        {
            foreach (var mesh in meshes)
            {
                // удаляем мусор (скрытые вершины и ребра)
                mesh.Value.RemoveUnpassable();

                string meshName = mesh.Key + ".bin";

                _saveMesh(zipArchive, meshName, mesh.Value, binFormatter);
            }
        } 
#endif

        /// <summary>
        /// Сохранение мешей <paramref name="mesh"/> в архивный файл <paramref name="zipFile"/> под именем <paramref name="meshName"/>
        /// </summary>
        private static bool _saveMesh(ZipArchive zipFile, string meshName, Graph mesh, BinaryFormatter binaryFormatter = null)
        {
            if (zipFile is null)
                return false;

            if (zipFile.Mode != ZipArchiveMode.Update
                && zipFile.Mode != ZipArchiveMode.Create)
                return false;

            //TODO: Безопасное сохранение mesh'а, чтобы при возникновении ошибки старое содержимое не удалялось
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
        private static bool _saveProfile(Profile profile, ZipArchive zipFile)
        {
            if (zipFile.Mode != ZipArchiveMode.Update
                && zipFile.Mode != ZipArchiveMode.Create)
                return false;
            
            lock (profile)
            {
                using (var memStream = new MemoryStream())
                {
                    try
                    {
                        profile.Saved = true;
                        ACTP0Serializer.QuesterProfileSerializer.Serialize(memStream, profile);

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
                            {
                                _makeProfileBackup(zipFile, zipProfileStream);
                                zipProfileStream.Seek(0, SeekOrigin.Begin);
                                zipProfileStream.SetLength(memStream.Length);
                            }
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
            }
            return false;
        }

        private static bool _saveProfile(Profile profile, string targetProfileName, string targetMeshesFileName)
        {
            if (profile is null
                || string.IsNullOrEmpty(targetProfileName))
                return false;


            using (var zipFile = ZipFile.Open(targetProfileName, 
                File.Exists(targetProfileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
            {
                lock (profile)
                {
                    using (var memStream = new MemoryStream())
                    {
                        var backupExternalMeshFileName = profile.ExternalMeshFileName;

                        try
                        {
                            profile.Saved = true;

                            // изменяем путь к файлу внешних мешей, поскольку новое имя файла профиля отличается от текущего
                            if (profile.UseExternalMeshFile)
                            {
                                var newRelativeExternalMeshesFilePath =
                                    targetProfileName.GetRelativePathTo(targetMeshesFileName);
                                if (!string.IsNullOrEmpty(newRelativeExternalMeshesFilePath))
                                    profile.ExternalMeshFileName = newRelativeExternalMeshesFilePath;
                            }

                            ACTP0Serializer.QuesterProfileSerializer.Serialize(memStream, profile);

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
                                {
                                    _makeProfileBackup(zipFile, zipProfileStream);
                                    zipProfileStream.Seek(0, SeekOrigin.Begin);
                                    zipProfileStream.SetLength(memStream.Length);
                                }
                                memStream.WriteTo(zipProfileStream);

                            }

                            _removeProfileBackups(zipFile);

                            return true;
                        }
                        catch (Exception e)
                        {
                            profile.Saved = false;
                            profile.ExternalMeshFileName = backupExternalMeshFileName;
                            Logger.WriteLine(Logger.LogType.Debug, e.ToString());
                        }
                    }
                } 
            }
            return false;
        }

        private static void _makeProfileBackup(ZipArchive zipFile, Stream zipProfileStream)
        {
            var zipProfileBackup = zipFile.CreateEntry($"profile_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml");
            using (var zipProfileBackupStream = zipProfileBackup.Open())
            {
                zipProfileStream.CopyTo(zipProfileBackupStream);
            }
        }

        private static void _removeProfileBackups(ZipArchive zipFile)
        {
            if (zipFile is null
                || zipFile.Mode != ZipArchiveMode.Update)
                return;

            var trunkateDate = DateTime.Now.AddDays(-7);
            var oldBackups = new List<ZipArchiveEntry>();
            foreach (var entry in zipFile.Entries)
            {
                var entryName = entry.Name;
                if (entryName.StartsWith("profile_"))
                {
                    var backupDateStr = entryName.Substring(8, entryName.Length - entryName.IndexOf(".xml"));
                    if (DateTime.TryParse(backupDateStr, out DateTime backupDateTime))
                        oldBackups.Add(entry);
                }

            }
            if (oldBackups.Count > 0)
            {
                foreach (var entry in oldBackups)
                    entry.Delete();
            }
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
                            if (Preprocessor.Active)
                            {
                                using (var profileStream = Preprocessor.Replace(stream, out mods))
                                {
                                    profile = ACTP0Serializer.QuesterProfileSerializer.Deserialize(profileStream) as Profile;
                                    if (mods > 0)
                                        Logger.Notify(
                                            $"There are {mods} modifications in the profile '{Path.GetFileName(profilePath)}'");
                                }
                            }
                            else profile = ACTP0Serializer.QuesterProfileSerializer.Deserialize(stream) as Profile;

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
                            QuesterHelper._saveProfile(profile, zipFile);
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


    }
}
