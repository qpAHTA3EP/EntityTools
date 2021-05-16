using AcTp0Tools.Patches;
using AcTp0Tools.Reflection;
using AStar;
using HarmonyLib;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

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
                                    return false;
                                }
                                if (LoadMeshFromZipFile(CurrentProfileZipMeshFile, mapName, out Graph mesh))
                                {
                                    mapsMeshes.Add(mapName, mesh);
                                    __result = mesh;
                                    return true;
                                }
                            }
                        }
                    }
                    __result = new Graph();

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

                private static void PostfixSetProfile(Astral.Quester.Classes.Profile value)
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

                    var propProfile = AccessTools.Property(tCore, nameof(Astral.Quester.Core.Profile));
                    var originalSetProfile = propProfile.GetSetMethod(true);
                    var postfixSetProfile = AccessTools.Method(tPatch, nameof(PostfixSetProfile));

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
                    }

                    var propMapsMeshes = AccessTools.Property(tCore, nameof(Astral.Quester.Core.MapsMeshes));
                    var originalGetMapsMeshes = propMeshes.GetGetMethod(true);
                    var originalSetMapsMeshes = propMeshes.GetSetMethod(true);
                    var prefixGetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixGetMapsMeshes));
                    var prefixSetMapsMeshes = AccessTools.Method(tPatch, nameof(PrefixSetMapsMeshes));

                    if (originalGetMapsMeshes != null
                        && prefixGetMapsMeshes != null
                        && originalSetMapsMeshes != null
                        && prefixSetMapsMeshes != null)
                    {
                        AcTp0Patcher.Harmony.Patch(originalGetMapsMeshes, new HarmonyMethod(prefixGetMapsMeshes));
                        AcTp0Patcher.Harmony.Patch(originalSetMapsMeshes, new HarmonyMethod(prefixSetMapsMeshes));
                    }
                }
            }
#if false
            public static class Entrypoint
            {
                public static readonly Func<object, Action> OnLoad = typeof(Astral.Quester.Entrypoint).GetAction("OnLoad");
                public static readonly Func<object, Action> OnUnload = typeof(Astral.Quester.Entrypoint).GetAction("OnUnload");
                public static readonly Func<object, Action<GraphicsNW>> OnMapDraw = typeof(Astral.Quester.Entrypoint).GetAction<GraphicsNW>("OnMapDraw");
                public static readonly Func<object, Action<bool>> Start = typeof(Astral.Quester.Entrypoint).GetAction<bool>("Start");
                public static readonly Func<object, Action> Stop = typeof(Astral.Quester.Entrypoint).GetAction("Stop");
                public static readonly Func<object, Action> TooMuchStuckReaction = typeof(Astral.Quester.Entrypoint).GetAction("TooMuchStuckReaction");
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

#if false // Метод Astral.Quester.Forms.Editor.RefreshRegions() является публичным
                    /// <summary>
                    /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
                    /// </summary>
                    private static Func<Astral.Quester.Forms.Editor, System.Action> QuesterEditor_RefreshRegions = null;
                    public static void RefreshRegions()
                    {
                        //TODO: Разобраться почему не видит метод Disposed
#if false
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor && !editor.IsDisposed) 
#else
                        if (editorForm.Value is Astral.Quester.Forms.Editor editor)
#endif
                        {
                            if (QuesterEditor_RefreshRegions == null)
                            {
                                if ((QuesterEditor_RefreshRegions = typeof(Astral.Quester.Forms.Editor).GetAction("RefreshRegions")) != null)
                                    QuesterEditor_RefreshRegions(editor)();
                            }
                            else QuesterEditor_RefreshRegions(editor)();
                        }
                    } 
#endif
                }
            }
        }
    }
}