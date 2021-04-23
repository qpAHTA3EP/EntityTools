using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using AStar;
using Astral.Controllers;
using Astral.Quester;
using AcTp0Tools.Reflection;
using MyNW.Internals;
using AcTp0Tools;

namespace EntityTools.Patches.Quester
{
    internal class Patch_Astral_Quester_Core_Save : Patch
    {
        internal Patch_Astral_Quester_Core_Save()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Quester.Core).GetMethod("Save", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Quester_Core_Save: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(Save), ReflectionHelper.DefaultFlags);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Patches.SaveQuesterProfile;

#if false // void Astral.Quester.Core.Save(bool saveas = false)
    	public static void Astral.Quester.Core.Save(bool saveas = false)
		{
			string text = Settings.Get.LastQuesterProfile;
			bool flag = true;
			if (!Core.Profile.Saved || saveas)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.InitialDirectory = Directories.ProfilesPath;
				saveFileDialog.DefaultExt = "amp.zip";
				saveFileDialog.Filter = "Astral mission profil (*.amp.zip)|*.amp.zip";
				saveFileDialog.FileName = Class1.LocalPlayer.MapState.MapName;
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				flag = false;
				text = saveFileDialog.FileName;
			}
			if (!flag && (!Core.Profile.UseExternalMeshFile || Core.Profile.ExternalMeshFileName.Length < 10))
			{
				Logger.WriteLine("Check all unloaded meshes before save ...");
				Core.LoadAllMeshes();
			}
			Dictionary<string, Graph> mapsMeshes = Core.MapsMeshes;
			lock (mapsMeshes)
			{
				List<Class88.Class89> list = new List<Class88.Class89>();
				if (Core.MapsMeshes == null)
				{
					Core.MapsMeshes = new Dictionary<string, Graph>();
				}
				List<Type> list2 = new List<Type>();
				list2.Add(typeof(Astral.Quester.Classes.Action));
				list2.Add(typeof(Condition));
				list2.Add(typeof(UCCAction));
				list2.Add(typeof(UCCCondition));
				Class88.Class89 item = new Class88.Class89(Core.Profile, Class88.Class89.Enum2.const_0, "profile.xml", list2);
				list.Add(item);
				bool flag3 = flag;
				if (Core.Profile.UseExternalMeshFile && Core.Profile.ExternalMeshFileName.Length >= 10)
				{
					flag3 = false;
				}
				else
				{
					foreach (KeyValuePair<string, Graph> keyValuePair in Core.MapsMeshes)
					{
						Class88.Class89 item2 = new Class88.Class89(keyValuePair.Value, Class88.Class89.Enum2.const_1, keyValuePair.Key + ".bin", null);
						list.Add(item2);
					}
				}
				if (flag3 && Class88.smethod_1(Core.CurrentProfileZipMeshFile, "meshes.bin").Count > 0)
				{
					flag3 = false;
				}
				Core.Profile.Saved = true;
				Class88.smethod_2(text, list, flag3);
				Settings.Get.LastQuesterProfile = text;
				if (Core.Profile.UseExternalMeshFile && Core.Profile.ExternalMeshFileName.Length >= 10)
				{
					list.Clear();
					foreach (KeyValuePair<string, Graph> keyValuePair2 in Core.MapsMeshes)
					{
						Class88.Class89 item3 = new Class88.Class89(keyValuePair2.Value, Class88.Class89.Enum2.const_1, keyValuePair2.Key + ".bin", null);
						list.Add(item3);
					}
					string text2 = new FileInfo(text).DirectoryName + "\\" + Core.Profile.ExternalMeshFileName;
					Logger.WriteLine(text2);
					Class88.smethod_2(text2, list, flag);
				}
			}
		}
#endif
        /// <summary>
        /// Сохранение quester-профиля
        /// </summary>
        public static void Save(bool saveAs = false)
        {
            string fullProfileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
            string profileName = string.IsNullOrEmpty(Astral.Controllers.Settings.Get.LastQuesterProfile) 
                ? EntityManager.LocalPlayer.MapState.MapName
                : Path.GetFileNameWithoutExtension(Astral.Controllers.Settings.Get.LastQuesterProfile);
            var currentProfile  = API.CurrentProfile;
            bool useExternalMeshes = currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= ".meshes.bin".Length + 1;
#if false   // string Quester.Core.CurrentProfileZipMeshFile
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
#endif
            string externalMeshFileName = string.Empty;
            bool needLoadAllMeshes = false;

            if (!currentProfile.Saved || saveAs)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Directories.ProfilesPath;
                saveFileDialog.DefaultExt = "amp.zip";
                saveFileDialog.Filter = @"Astral mission profil (*.amp.zip)|*.amp.zip";
                saveFileDialog.FileName = string.IsNullOrEmpty(fullProfileName) ? EntityManager.LocalPlayer.MapState.MapName : fullProfileName;
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                fullProfileName = saveFileDialog.FileName;
                needLoadAllMeshes = true;
            }
            if(useExternalMeshes)
                externalMeshFileName = Path.Combine(Path.GetDirectoryName(fullProfileName)?? string.Empty, currentProfile.ExternalMeshFileName);

            if (needLoadAllMeshes && !useExternalMeshes)
                AstralAccessors.Quester.Core.LoadAllMeshes();

            var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes;

            ZipArchive zipFile = null;
            try
            {

                // Открываем архивный файл профиля
                zipFile = ZipFile.Open(fullProfileName, File.Exists(fullProfileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                // Сохраняем в архив файл профиля "profile.xml"
                lock (currentProfile)
                {
                    if(SaveProfile(zipFile))
                        currentProfile.Saved = true; 
                }

                // Сохраняем файлы мешей
                if (useExternalMeshes)
                {
                    // Если используется внешние меши
                    // в файле профиля нужно удалить все "лишние" файлы
                    if(zipFile.Mode == ZipArchiveMode.Update)
                        foreach(var entry in zipFile.Entries)
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
                lock (mapsMeshes)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    foreach (var mesh in mapsMeshes)
                    {
                        // удаляем мусор (скрытые вершины и ребра)
                        mesh.Value.RemoveUnpassable();

                        string meshName = mesh.Key + ".bin";

                        SaveMesh(zipFile, meshName, mesh.Value, binaryFormatter);
                    } 
                }

                Astral.Controllers.Settings.Get.LastQuesterProfile = fullProfileName;

                Astral.Logger.Notify(string.Concat("Profile '", fullProfileName, "' saved"));
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, exc.ToString(), true);
                Astral.Logger.Notify(string.Concat("Profile '", fullProfileName, "' saved"), true);
            }
            finally
            {
                zipFile?.Dispose();
            }
        }

        /// <summary>
        /// Сохранение мешей <paramref name="mesh"/> в архивный файл <paramref name="zipFile"/> под именем <paramref name="meshName"/>
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="meshName"></param>
        /// <param name="mesh"></param>
        /// <param name="binaryFormatter"></param>
        public static bool SaveMesh(ZipArchive zipFile, string meshName, Graph mesh, BinaryFormatter binaryFormatter = null)
        {
            //TODO: Безопасное сохранение mesh'а, чтобы при возникновении ошибки старое содержимое не удалялось
            //TODO: Исправиль сохранение внешних мешей
            if (zipFile is null)
                return false;

            if (binaryFormatter is null)
                binaryFormatter = new BinaryFormatter();

#if false
            ZipArchiveEntry zipMeshEntry;
            if (zipFile.Mode == ZipArchiveMode.Update)
            {
                zipMeshEntry = zipFile.GetEntry(meshName);
                if (zipMeshEntry != null)
                    zipMeshEntry.Delete();
            }
            zipMeshEntry = zipFile.CreateEntry(meshName);

            // Сохранение графа путей (мешей)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, mesh);

                using (var zipMeshStream = zipMeshEntry.Open())
                {
                    byte[] meshBytes = memoryStream.ToArray();

                    zipMeshStream.Write(meshBytes, 0, meshBytes.Length);

                    return true;
                }
            } 
#else
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
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }
#endif
            return false;
        }

        /// <summary>
        /// Сохранение профиля в архивный файл <paramref name="zipFile"/>
        /// </summary>
        public static bool SaveProfile(ZipArchive zipFile)
        {
            //TODO: Безопасное сохранение профиля, чтобы при возникновении ошибки старое содержимое не удалялось
            AcTp0Tools.Patches.Astral_Functions_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types, 2);
            XmlSerializer serializer = new XmlSerializer(API.CurrentProfile.GetType(), types.ToArray());
#if false
            ZipArchiveEntry zipProfileEntry;
            if (zipFile.Mode == ZipArchiveMode.Update)
            {
                zipProfileEntry = zipFile.GetEntry("profile.xml");
                zipProfileEntry?.Delete();
            }
            zipProfileEntry = zipFile.CreateEntry("profile.xml");
            using (var zipProfileStream = zipProfileEntry.Open())
            {
                API.CurrentProfile.Saved = true;
                serializer.Serialize(zipProfileStream, API.CurrentProfile);
            return true;
            } 
#else
            var currentProfile = API.CurrentProfile;
            bool saved = currentProfile.Saved;
            using (var memStream = new MemoryStream())
            {
                try
                {
                    currentProfile.Saved = true;
                    serializer.Serialize(memStream, API.CurrentProfile);

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
                    ETLogger.WriteLine(LogType.Error, e.ToString(), true);
                }
            }
#endif
            return false;
        }
    }
}
