using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using ACTP0Tools.Patches;
using ACTP0Tools.Reflection;
using AStar;
using Astral;
using Astral.Quester.Classes;
using HarmonyLib;
using MyNW.Internals;
using QuesterAction = Astral.Quester.Classes.Action;

namespace ACTP0Tools.Classes.Quester
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
            SetStartPointInternal(actionPack, searchedActionId);
        }
        private static QuesterAction SetStartPointInternal(ActionPack actionPack, Guid searchedActionId)
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
                    var foundedAction = SetStartPointInternal(innerActionPack, searchedActionId);
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
            if (string.IsNullOrEmpty(meshesFileName) || !File.Exists(zipFileName)) return result;

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

                externalMeshesFullFilePath = Path.GetFullPath(Path.Combine(currentDir, profile.ExternalMeshFileName));

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
                LoadAllMeshes(profileMeshes, externalMeshesFullFilePath);

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


        private static string RequestUserForNewProfileFileName(string currentProfileName)
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
        private static void SaveAllMeshes(ZipArchive zipArchive, IDictionary<string, Graph> meshes, BinaryFormatter binFormatter)
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
        internal static bool SaveProfile(Profile profile, ZipArchive zipFile)
        {
            if (zipFile.Mode != ZipArchiveMode.Update
                && zipFile.Mode != ZipArchiveMode.Create)
                return false;

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
                            MakeProfileBackup(zipFile, zipProfileStream);
                            zipProfileStream.Seek(0, SeekOrigin.Begin);
                            zipProfileStream.SetLength(memStream.Length);
                        }
                        memStream.WriteTo(zipProfileStream);

                        RemoveProfileBackups(zipFile);
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

        private static void MakeProfileBackup(ZipArchive zipFile, Stream zipProfileStream)
        {
            var zipProfileBackup = zipFile.CreateEntry($"profile_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml");
            using (var zipProfileBackupStream = zipProfileBackup.Open())
            {
                zipProfileStream.CopyTo(zipProfileBackupStream);

            }
        }

        private static void RemoveProfileBackups(ZipArchive zipFile)
        {
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
                            QuesterHelper.SaveProfile(profile, zipFile);
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
