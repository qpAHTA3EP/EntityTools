using System;
using System.Collections.Generic;
using System.Reflection;
using AStar;
using EntityTools.Reflection;
using System.Windows.Forms;
using MyNW.Internals;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DevExpress.XtraEditors;
using System.Xml.Serialization;
using System.IO.Compression;
using DevExpress.LookAndFeel;

namespace EntityTools.Patches
{
    internal class Patch_Astral_Quester_Core_Save : Patch
    {
        internal Patch_Astral_Quester_Core_Save()
        {
            MethodInfo mi = typeof(Astral.Quester.Core).GetMethod("Save", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Quester_Core_Save: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(Save), ReflectionHelper.DefaultFlags);
        }


#if false
    Astral.Quester.Core
    	public static void Save(bool saveas = false)
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
            string profileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
            var currentProfile  = Astral.Quester.API.CurrentProfile;
            bool useExternalMeshes = currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= 10;
            string externalMeshFileName = string.Empty;
            bool needLoadAllMeshes = false;

            if (!Astral.Quester.API.CurrentProfile.Saved || saveAs)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Astral.Controllers.Directories.ProfilesPath;
                saveFileDialog.DefaultExt = "amp.zip";
                saveFileDialog.Filter = "Astral mission profil (*.amp.zip)|*.amp.zip";
                saveFileDialog.FileName = string.IsNullOrEmpty(profileName) ? EntityManager.LocalPlayer.MapState.MapName : profileName;
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                profileName = saveFileDialog.FileName;
                needLoadAllMeshes = true;
            }
            if(useExternalMeshes)
                externalMeshFileName = Path.Combine(Path.GetDirectoryName(profileName), currentProfile.ExternalMeshFileName);

            if (needLoadAllMeshes && !useExternalMeshes)
                AstralAccessors.Quester.Core.LoadAllMeshes();

            var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes.Value;

            ZipArchive zipFile = null;
            try
            {

                // Открываем архивный файл профиля
                zipFile = ZipFile.Open(profileName, File.Exists(profileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                // Сохраняем в архив файл профиля "profile.xml"
                lock (currentProfile)
                {
                    SaveProfile(zipFile);
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

                Astral.Controllers.Settings.Get.LastQuesterProfile = profileName;

                XtraMessageBox.Show(string.Concat("Profile '", profileName, "' saved"));
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, exc.ToString(), true);
                XtraMessageBox.Show(exc.ToString());
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
            if (zipFile is null)
                return false;

            if (binaryFormatter is null)
                binaryFormatter = new BinaryFormatter();

            ZipArchiveEntry zipMeshEntry = null;
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
        }

        /// <summary>
        /// Сохранение профиля в архивный файл <paramref name="zipFile"/>
        /// </summary>
        public static void SaveProfile(ZipArchive zipFile)
        {
            ZipArchiveEntry zipProfileEntry = null;
            if (zipFile.Mode == ZipArchiveMode.Update)
            {
                zipProfileEntry = zipFile.GetEntry("profile.xml");
                if(zipProfileEntry != null)
                    zipProfileEntry.Delete();
            } 
            zipProfileEntry = zipFile.CreateEntry("profile.xml");

            Patch_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types, 2);
            XmlSerializer serializer = new XmlSerializer(Astral.Quester.API.CurrentProfile.GetType(), types.ToArray());
            using (var zipProfileStream = zipProfileEntry.Open())
            {
                serializer.Serialize(zipProfileStream, Astral.Quester.API.CurrentProfile);
                Astral.Quester.API.CurrentProfile.Saved = true;
            }
        }

        
    }
}
