using AcTp0Tools;
using AcTp0Tools.Reflection;
using AStar;
using Astral.Controllers;
using Astral.Quester;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using Astral.Quester.Classes;
using DevExpress.XtraEditors;

namespace EntityTools.Patches.Quester
{
    internal class Patch_Astral_Quester_Core_Save : Patch
    {
        internal Patch_Astral_Quester_Core_Save()
        {
            if (NeedInjection)
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

        public sealed override bool NeedInjection => EntityTools.Config.Patches.SaveQuesterProfile;

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
            if (useExternalMeshes)
                externalMeshFileName = Path.Combine(Path.GetDirectoryName(fullProfileName) ?? string.Empty, currentProfile.ExternalMeshFileName);

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
                ETLogger.WriteLine(LogType.Error, e.ToString(), true);
            }

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
            return false;
        }

#if false // Astral.Quester.Core.Load(string Path, bool savePath = true)
public static void Load(string Path, bool savePath = true)
{
	if (Path.Length == 0)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.InitialDirectory = Directories.ProfilesPath;
		openFileDialog.DefaultExt = "amp.zip";
		openFileDialog.Filter = "Astral mission profil (*.amp.zip)|*.amp.zip";
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		Path = openFileDialog.FileName;
	}
	Dictionary<string, Graph> mapsMeshes = Core.MapsMeshes;
	lock (mapsMeshes)
	{
		List<Class90.Class91> list = new List<Class90.Class91>();
		List<Type> list2 = new List<Type>();
		list2.Add(typeof(Astral.Quester.Classes.Action));
		list2.Add(typeof(Condition));
		list2.Add(typeof(UCCAction));
		list2.Add(typeof(UCCCondition));
		Class90.Class91 @class = new Class90.Class91(Core.Profile, Class90.Class91.Enum2.const_0, "profile.xml", list2);
		list.Add(@class);
		Class90.Class91 class2 = new Class90.Class91(Core.MapsMeshes, Class90.Class91.Enum2.const_1, "meshes.bin", null);
		list.Add(class2);
		Class90.smethod_4(Path, list);
		if (@class.Success)
		{
			Core.Profile.ResetCompleted();
			Core.Profile = (@class.Object as Profile);
			if (class2.Success)
			{
				Core.MapsMeshes = (class2.Object as Dictionary<string, Graph>);
			}
			else
			{
				Core.MapsMeshes = new Dictionary<string, Graph>();
			}
			Settings.Get.LastQuesterProfile = Path;
		}
		else
		{
			Logger.WriteLine("Unable to load " + Path);
		}
		if (Core.MapsMeshes == null)
		{
			Core.MapsMeshes = new Dictionary<string, Graph>();
		}
		Combats.BLAttackersList = (() => Core.Profile.BlackList);
		if (BotServer.Server.IsRunning)
		{
			BotServer.SendQuesterProfileInfos();
		}
	}
}
public static void Class90.smethod_4(string string_0, List<Class90.Class91> list_0)
{
	ZipFile zipFile = null;
	try
	{
		zipFile = new ZipFile(File.OpenRead(string_0));
		zipFile.UseZip64 = UseZip64.Off;
		foreach (Class90.Class91 @class in list_0)
		{
			ZipEntry entry = zipFile.GetEntry(@class.FileName);
			if (entry == null)
			{
				@class.Success = false;
			}
			else
			{
				Stream inputStream = zipFile.GetInputStream(entry);
				if (@class.Mode == Class90.Class91.Enum2.const_0)
				{
					List<Type> list = new List<Type>();
					foreach (Type right in @class.ExtraBaseTypes)
					{
						List<Type> list2 = Assembly.GetExecutingAssembly().GetTypes().ToList<Type>();
						list2.AddRange(Plugins.GetTypes());
						foreach (Type type in list2)
						{
							if (type.BaseType == right)
							{
								list.Add(type);
							}
						}
					}
					XmlSerializer xmlSerializer = new XmlSerializer(@class.Object.GetType(), list.ToArray());
					@class.Object = xmlSerializer.Deserialize(inputStream);
					@class.Success = true;
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					@class.Object = binaryFormatter.Deserialize(inputStream);
					@class.Success = true;
				}
				inputStream.Close();
			}
		}
	}
	catch (ThreadAbortException)
	{
	}
	catch (Exception ex)
	{
		Logger.WriteLine(ex.ToString());
		XtraMessageBox.Show(ex.ToString());
	}
	finally
	{
		if (zipFile != null)
		{
			zipFile.IsStreamOwner = true;
			zipFile.Close();
		}
	}
}
#endif

        private static OpenFileDialog openFileDialog;

        public static void Load(string Path, bool savePath = true)
        {
            if (string.IsNullOrEmpty(Path))
            {
                if (openFileDialog is null)
                    openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = Directories.ProfilesPath;
                openFileDialog.DefaultExt = "amp.zip";
                openFileDialog.Filter = @"Astral mission profil (*.amp.zip)|*.amp.zip";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Path = openFileDialog.FileName;
            }

            var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes;

            ZipArchive zipFile = null;
            using (zipFile = ZipFile.Open(Path, ZipArchiveMode.Read))
            {
                ZipArchiveEntry zipProfileEntry = zipFile.GetEntry("profile.xml");
                if (zipProfileEntry is null)
                {
                    XtraMessageBox.Show($"File '{System.IO.Path.GetFileName(Path)}' does not contain 'profile.xml'");
                    return;
                }

                Profile profile = null;
                using (var stream = zipProfileEntry.Open())
                {
                    AcTp0Tools.Patches.Astral_Functions_XmlSerializer_GetExtraTypes.GetExtraTypes(out List<Type> types,
                        2);
                    XmlSerializer serializer = new XmlSerializer(typeof(Profile), types.ToArray());
                    profile = serializer.Deserialize(stream) as Profile;

                    if (profile is null)
                    {
                        XtraMessageBox.Show($"Deserialization of 'profile.xml' failed");
                        return;
                    }
                }

                var mapName = EntityManager.LocalPlayer.MapState.MapName;
                if (string.IsNullOrEmpty(mapName))
                    return;
                var mapMeshesName = mapName + ".bin";
                ZipArchiveEntry zipMeshEntry = zipFile.GetEntry(mapMeshesName);
                if (zipMeshEntry is null)
                {
                    XtraMessageBox.Show(
                        $"File '{System.IO.Path.GetFileName(Path)}' does not contain '{mapMeshesName}'");
                    return;
                }

                Graph meshes = null;
                using (var stream = zipMeshEntry.Open())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    if (binaryFormatter.Deserialize(stream) is Graph meshesFromFile)
                    {
                        meshes = meshesFromFile;
                    }
                }

                if (profile != null)
                {
                    Astral.Quester.Core.Profile.ResetCompleted();
                    AstralAccessors.Quester.Core.Profile = profile;
                    var profileMehes = AstralAccessors.Quester.Core.MapsMeshes;
                    lock (profileMehes)
                    {
                        profileMehes.Clear();
                        if (meshes != null)
                            profileMehes.Add(mapName, meshes);
                    }

                    Astral.API.CurrentSettings.LastQuesterProfile = Path;
                }
                else
                {
                    Astral.Logger.WriteLine("Unable to load " + Path);
                }

                AstralAccessors.Logic.NW.Combats.BLAttackersList = null;

                if (Astral.Controllers.BotComs.BotServer.Server.IsRunning)
                {
                    Astral.Controllers.BotComs.BotServer.SendQuesterProfileInfos();
                }
            }
        }
    }
}
