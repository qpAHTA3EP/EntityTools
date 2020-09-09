#define LOG

#if PATCH_ASTRAL
using Astral.Logic.Classes.Map;
using MyNW.Classes;
using MappingType = EntityTools.Enums.MappingType; 
#endif

using AStar;
using Astral;
using Astral.Controllers;
using Astral.Quester.Classes;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using EntityTools.Reflection;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using AstralMapperOriginals;
//using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm //*/Form
    {
#if AstralMapper
        private Astral.Forms.UserControls.Mapper mapper = null; 
#else
        MapperExt mapper = null;
#endif

        #region Reflection helpers
#if false
        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static readonly StaticPropertyAccessor<AStar.Graph> CoreCurrentMapMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Graph>("Meshes");

        /// <summary>
        /// Функтор доступа к словарю Astral.Quester.Core.MapsMeshes
        /// </summary>
        private static readonly StaticPropertyAccessor<Dictionary<string, Graph>> CoreMapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes"); 

        /// <summary>
        /// Функтор доступа к списку карт в файле профиля
        /// Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName)
        /// </summary>
        private static readonly Func<string, List<string>> CoreAvailableMeshesFromFile = typeof(Astral.Quester.Core).GetStaticFunction<string, List<string>>("AvailableMeshesFromFile");

        /// <summary>
        /// Доступ к Astral.Quester.Core.LoadAllMeshes();
        /// </summary>
        private static readonly Func<int> CoreLoadAllMeshes = typeof(Astral.Quester.Core).GetStaticFunction<int>("LoadAllMeshes");

        /// <summary>
        /// Функтор доступа к Astral.Logic.NW.GoldenPath.GetCurrentMapGraph(...);
        /// </summary>
        //private static readonly Func<AStar.Graph, bool> GetCurrentMapGraph = typeof(Astral.Logic.NW.GoldenPath).GetStaticFunction<AStar.Graph, bool>("GetCurrentMapGraph");
#endif

#if AstralMapper
        // PictureBox Astral.Forms.UserControls.Mapper.MapPicture;
        //private static readonly StaticFielsAccessor<PictureBox> mapperPicture = typeof(Astral.Forms.UserControls.Mapper).GetField<PictureBox>("MapPicture", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Функтор запускающего фоновый поток отрисовки изображения Mapper'a
        /// </summary>
        private static readonly Func<object, System.Action> MapperStartDrawing = typeof(Astral.Forms.UserControls.Mapper).GetAction("\u0002", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Функтор запускающего фоновый поток отрисовки изображения Mapper'a
        /// </summary>
        private static readonly Func<object, System.Action> MapperStopDrawing = typeof(Astral.Forms.UserControls.Mapper).GetAction("\u0003", BindingFlags.Instance | BindingFlags.Public); 
#endif
        #endregion

        #region Инициализация формы
        private MapperFormExt()
        {
            InitializeComponent();

#if AstralMapper
            mapper = new Astral.Forms.UserControls.Mapper
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right),
                Dock = DockStyle.Fill,
                CustomDraw = null,
                Location = new Point(6, 26),
                Name = "mapper",
                OnClick = null,
                Size = new Size(372, 275),
                TabIndex = 0
            }; 
#else
            mapper = new MapperExt
            {
                Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right),
                Dock = DockStyle.Fill,
                CustomDraw = null,
                Location = new Point(6, 26),
                Name = "mapper",
                OnClick = null,
                Size = new Size(372, 275),
                TabIndex = 0,
                ZoomPos = Convert.ToInt32(statZoom.EditValue),
                MapLockOnPlayer = statLockMapOnPlayer.Checked
            };
            mapper.OnMapLockOnPlayerChanged += handler_SetMapLockOnPlayer;
            //mapper.OnZoomChanged += event_SetZoom;
#endif
            Controls.Add(mapper);

            MouseWheel += handler_MouseWheel;
            BindingControls();
        }

        /// <summary>
        /// Привязка элементов управления к данным
        /// </summary>
        public void BindingControls()
        {
            menuWaypointDistance.DataBindings.Add(nameof(menuWaypointDistance.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            menuWaypointDistance.Edit.EditValueChanged += handler_WaypointDistanceChanged;
            menuWaypointDistance.Edit.Leave += handler_WaypointDistanceChanged;

            menuMaxZDifference.DataBindings.Add(nameof(menuMaxZDifference.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.MaxElevationDifference),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            menuEquivalenceDistance.DataBindings.Add(nameof(menuEquivalenceDistance.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            menuCacheActive.DataBindings.Add(nameof(menuCacheActive.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.CacheActive),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            /* Astral.API.CurrentSettings.DeleteNodeRadius не реализует INotifyPropertyChanged
             * поэтому привязка нижеуказанным методом невозможна
             * menuDeleteRadius.DataBindings.Add(new Binding(nameof(menuDeleteRadius.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(Astral.API.CurrentSettings.DeleteNodeRadius))); //*/

            ((ISupportInitialize)bsrcAstralSettings).BeginInit();
            bsrcAstralSettings.DataSource = Astral.API.CurrentSettings;
            menuDeleteRadius.DataBindings.Add(nameof(menuDeleteRadius.EditValue),
                                              bsrcAstralSettings,
                                              nameof(Astral.API.CurrentSettings.DeleteNodeRadius),
                                              false, DataSourceUpdateMode.OnPropertyChanged);
            ((ISupportInitialize)bsrcAstralSettings).EndInit();
            menuDeleteRadius.Edit.EditValueChanged += handler_DeleteRadiusChanged;
            menuDeleteRadius.Edit.Leave += handler_DeleteRadiusChanged;

            menuForceLinkLast.DataBindings.Add(nameof(menuForceLinkLast.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            /* Сохранение опции между сессиями не требуется
             * Отслеживание флага производится через свойство LinearPath
             * menuLinearPath.DataBindings.Add(nameof(menuLinearPath.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.LinearPath)); */
        }

        /// <summary>
        /// Открытие формы
        /// </summary>
        public static void Open()
        {
            if (EntityTools.PluginSettings.Mapper.Patch)
            {
                if (@this != null && !@this.IsDisposed)
                {
                    @this.Focus();
                    return;
                }
                else
                {
                    @this = new MapperFormExt();
                    @this.Show();
                }
            }
            else Astral.Quester.Forms.MapperForm.Open();
        }

        /// <summary>
        /// Событие при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_LoadMapperForm(object sender, EventArgs e)
        {
            Binds.RemoveShiftAction(Keys.M);
            //mapper.DoubleClick += eventMapperDoubleClick;
            //mapper.CustomDraw += eventMapperDrawCache;
            //ReflectionHelper.ExecMethod(mapper, "\u0002", new object[] { }, out object res, BindingFlags.Instance | BindingFlags.NonPublic);

            // PictureBox Astral.Forms.UserControls.Mapper.MapPicture;
            //if (ReflectionHelper.GetFieldValue(mapper, "MapPicture", out object mapPictureObj, BindingFlags.Instance | BindingFlags.NonPublic)
            //    && ReflectionHelper.SubscribeEvent(mapPictureObj, "MouseDoubleClick", this, "eventMapperDoubleClick", true))
            //    SubscribedMapperMouseDoubleClick = true;

            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// СОбытие при закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ClosingMapperForm(object sender, FormClosingEventArgs e)
        {
            MappingCanceler?.Cancel();
            backgroundWorker?.CancelAsync();
#if AstralMapper
            if (mapper != null)
            {
                MapperStopDrawing?.Invoke(mapper); 

                //mapper.DoubleClick -= eventMapperDoubleClick;
                //mapper.CustomDraw -= eventMapperDrawCache;

                //if (SubscribedMapperMouseDoubleClick
                //    && ReflectionHelper.GetFieldValue(mapper, "MapPicture", out object mapPictureObj, BindingFlags.Instance | BindingFlags.NonPublic)
                //    && ReflectionHelper.UnsubscribeEvent(mapPictureObj, "MouseDoubleClick", this, "eventMapperDoubleClick", true))
                //    SubscribedMapperMouseDoubleClick = false;
            }
#endif
            CustomRegionHelper.Reset();

            lastNodeDetail = null;
            Binds.RemoveShiftAction(Keys.M);
        }

        /// <summary>
        /// Фоновый процесс обновления заголовка окна строки состояния
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void work_UpdateFormStatus(object sender, DoWorkEventArgs e)
        {
            string formCaption = string.Empty,
                posStr = string.Empty;
            var UpdateFormStatus = new System.Action(() =>
            { Text = formCaption; statMousePos.Caption = posStr; });

            while (!IsDisposed
                   && !backgroundWorker.CancellationPending)
            {
                formCaption = string.Concat(EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName, " [", EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName, ']');

                Vector3 pos = EntityManager.LocalPlayer.Location;
                posStr = !EntityManager.LocalPlayer.IsLoading ? $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1}" : "Loading";

                if (InvokeRequired)
                    Invoke(UpdateFormStatus);
                else
                {
                    Text = formCaption;
                    statMousePos.Caption = posStr;
                }

                Thread.Sleep(500);
            }
        }
        #endregion

        #region Mapping
        /// <summary>
        /// Кэш вершин графа путей
        /// </summary>
        private MapperGraphCache graphCache;

        /// <summary>
        /// Task, прокладывающий путь
        /// </summary>
        private Task MappingTask;
        /// <summary>
        /// Токен отмены MappingTask
        /// </summary>
        private CancellationTokenSource MappingCanceler = null;
        /// <summary>
        /// Тип текущего режим прокладывания пути
        /// </summary>
        private MappingType MappingType
        {
            get
            {
                if (mapAndRegion_whereMapping != EntityManager.LocalPlayer.MapAndRegion)
                {
                    handler_StopMapping();
                    return MappingType.Stoped;
                }

                if (menuBidirectional.Checked)
                    return MappingType.Bidirectional;
                else if (menuUnidirectional.Checked)
                    return MappingType.Unidirectional;
                else return MappingType.Stoped;
            }
        }
        /// <summary>
        /// Флаг линейного пут
        /// </summary>
        private bool LinearPath => menuLinearPath.Checked;
        
        /// <summary>
        /// Флаг принудительного связывания с предыдущей точкой пути
        /// </summary>
        private bool ForceLinkLastWaypoint => menuForceLinkLast.Checked;

        /// <summary>
        /// последний добавленный узел
        /// </summary>
        private NodeDetail lastNodeDetail = null;
        /// <summary>
        /// Название карты и региона, на которой активировано прокладывание пути
        /// </summary>
        private string mapAndRegion_whereMapping = null;

        /// <summary>
        /// Запуск потока прокладывания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_StartMapping(object sender, ItemClickEventArgs e)
        {
            if (e.Item is BarCheckItem checkedItem
                && checkedItem.Checked)
            {
                if (MappingTask == null
                    || MappingTask.IsCanceled || MappingTask.IsCompleted || MappingTask.IsFaulted)
                {
                    mapAndRegion_whereMapping = EntityManager.LocalPlayer.MapAndRegion;
                    MappingCanceler = new CancellationTokenSource();
                    MappingTask = Task.Factory.StartNew(() => work_Mapping(MappingCanceler.Token), MappingCanceler.Token);
                    if (MappingTask != null)
                        MappingTask.ContinueWith(new System.Action<Task>((Task t) => handler_StopMapping()));
                }
            }
        }

        /// <summary>
        /// Событие остановки прокладывания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_StopMapping(object sender = null, ItemClickEventArgs e = null)
        {
            MappingCanceler?.Cancel();
            lastNodeDetail = null;
            menuBidirectional.Checked = false;
            menuBidirectional.Reset();
            menuUnidirectional.Checked = false;
            menuUnidirectional.Reset();
            menuCheckStopMapping.Checked = true;
            graphCache?.StopCache();
        }

        /// <summary>
        /// Прокладывание пути
        /// </summary>
        private void work_Mapping(CancellationToken token)
        {
            //NodeDetail nodeDetail = null;
            try
            {
                if (graphCache == null)
                    graphCache = new MapperGraphCache(mapper);
                else graphCache.StartCache(mapper);

#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                if (graphCache.Nodes.Count != 0)
                {
                    if (LinearPath)
                        lastNodeDetail = AddNavigationNodeAdvanced.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                    else lastNodeDetail = AddNavigationNodeAdvanced.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                }
                while (MappingType != MappingType.Stoped
                       && !token.IsCancellationRequested)
                {
                    /* 3. Вариант реализации с проверкой расстояния только до предыдущего узла*/
                    if (lastNodeDetail != null)
                        lastNodeDetail.Rebase(EntityManager.LocalPlayer.Location);

                    if (lastNodeDetail == null || lastNodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        switch (MappingType)
                        {
                            case MappingType.Bidirectional:
                                if (LinearPath)
                                {
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = AddNavigationNodeAdvanced.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = AddNavigationNodeAdvanced.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                else
                                {
                                    // Строим комплексный (многосвязный путь)
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = AddNavigationNodeAdvanced.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = AddNavigationNodeAdvanced.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                break;
                            case MappingType.Unidirectional:
                                {
                                    lastNodeDetail = AddNavigationNodeAdvanced.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, true) ?? lastNodeDetail;
                                }
                                break;
                        }
                        graphCache.LastAddedNode = lastNodeDetail?.Node;
                    }
                    Thread.Sleep(100);
                }
                if (token.IsCancellationRequested)
                {
                    // Инициировано прерывание 
                    // Связываем текущее местоположение с графом
                    if (LinearPath)
                        AddNavigationNodeAdvanced.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    else AddNavigationNodeAdvanced.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    lastNodeDetail = null;
                    //token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
#endif
#if LOG && DEBUG
                ETLogger.WriteLine(LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {graphCache.FullGraph.Nodes.Count}");
#endif
                ETLogger.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region Meshes_Manipulation
        /// <summary>
        /// Сохранение в файл текущего Quester-профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_SaveCurrentMapMeshes2QuesterProfile(object sender, ItemClickEventArgs e)
        {
            // Сохранение профиля реализовано
            // Astral.Quester.Core.Save(false)

            string mapName = EntityManager.LocalPlayer.MapState.MapName;
            string meshName = mapName + ".bin";
            string fileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
            if (Astral.Quester.API.CurrentProfile.UseExternalMeshFile)
            {
                // Профиль использует "внешние" меши
            }
            else
            {
                // Сохранение графа путей (карты) в файл текущего профиля
                if (File.Exists(fileName))
                {
                    try
                    {
                        using (ZipArchive profile = ZipFile.Open(fileName, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry zipMeshEntry = null;

                            // Проверяем наличие меша карты в zip-файле
                            foreach (ZipArchiveEntry entry in profile.Entries)
                            {
                                if (entry.Name.Equals(meshName, StringComparison.OrdinalIgnoreCase))
                                {
                                    zipMeshEntry = entry;
                                    break;
                                }
                            }

                            if (zipMeshEntry is null)
                                // Меш карты не обнаружен. Создаем
                                zipMeshEntry = profile.CreateEntry(meshName);
                            else
                            {
                                // Меш карты найден. Удаляем старый и создаем новый
                                zipMeshEntry.Delete();
                                zipMeshEntry = profile.CreateEntry(meshName);
                            }

                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                BinaryFormatter binaryFormatter = new BinaryFormatter();
                                binaryFormatter.Serialize(memoryStream, AstralAccessors.Quester.Core.Meshes.Value);

                                using (var zipMeshStream = zipMeshEntry.Open())
                                {
                                    byte[] meshBytes = memoryStream.ToArray();

                                    zipMeshStream.Write(meshBytes, 0, meshBytes.Length);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Logger.WriteLine(exc.ToString());
                        XtraMessageBox.Show(exc.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Импорт мешей карты (графа путей) из Игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ImportCurrentMapMeshesFromGame(object sender, ItemClickEventArgs e)
        {
            //if (((Graph)CoreCurrentMapMeshes)?.Nodes.Count == 0
            if (((Graph)AstralAccessors.Quester.Core.Meshes)?.Nodes.Count == 0
                || XtraMessageBox.Show(this, "Are you sure to import game nodes ? All actual nodes must be delete !", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_StopMapping();
#if AstralMapper
                MapperStopDrawing?.Invoke(mapper); 
#endif
                //Graph graph = (Graph)CoreCurrentMapMeshes;
                Graph graph = AstralAccessors.Quester.Core.Meshes;
                lock (graph)
                {
                    graph.Clear();
                    Astral.Logic.NW.GoldenPath.GetCurrentMapGraph(graph);
                    //graphCache?.RegenCache(null);
                }
#if AstralMapper
                MapperStartDrawing?.Invoke(mapper); 
#endif
            }
            //GoldenPath.GetCurrentMapGraph(coreMeshes);
            //GetCurrentMapGraph?.Invoke(coreMeshes);
        }

        /// <summary>
        /// Импорт мешей одной или всех карт из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ImportMapMeshesFromFile(object sender, ItemClickEventArgs e)
        {
            string currentMapName = EntityManager.LocalPlayer.MapState.MapName;
            if (string.IsNullOrEmpty(currentMapName))
            {
                XtraMessageBox.Show("Impossible to get current map name !");
                return;
            }
            string currentMapMeshesName = currentMapName + ".bin";
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directories.ProfilesPath,
                DefaultExt = "amp.zip",
                Filter = "Astral mission profil (*.amp.zip)|*.amp.zip|(*.mesh.zip)|*.mesh.zip"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
            {
                handler_StopMapping();
                try
                {
                    using (ZipArchive profile = ZipFile.Open(openFileDialog.FileName, ZipArchiveMode.Read))
                    {
                        ZipArchiveEntry currentMapEntry = profile.GetEntry(currentMapMeshesName);
                        if(currentMapEntry is null)
                        {
                            XtraMessageBox.Show("This profile doesn't contain current map !");
                            return;
                        }
                        var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes.Value;

                        DialogResult dialogResult = XtraMessageBox.Show("Import current map only ?\n\rElse import all.", "Map import", MessageBoxButtons.YesNoCancel);
                        if (dialogResult == DialogResult.Yes)
                        {
                            // Экспорт одной "текуще" карты
                            using (Stream stream = currentMapEntry.Open())
                            {       
                                BinaryFormatter binaryFormatter = new BinaryFormatter();
                                Graph currentMapMeshes = binaryFormatter.Deserialize(stream) as Graph;
                                if(currentMapMeshes != null)
                                {
                                    lock (mapsMeshes)
                                    {
                                        if (mapsMeshes.ContainsKey(currentMapName))
                                            mapsMeshes[currentMapName] = currentMapMeshes;
                                        else mapsMeshes.Add(currentMapName, currentMapMeshes);
                                    }
                                    Logger.WriteLine($"Import '{currentMapMeshesName}' from {openFileDialog.FileName}");
                                    return;
                                }
                                else
                                {
                                    string msg = $"Failed to import '{currentMapMeshesName}' from {openFileDialog.FileName}";
                                    Logger.WriteLine(msg);
                                    XtraMessageBox.Show(msg);
                                    return;
                                }
                            }

                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            // Экспорт всех карт, содержащихся в заданном профиле
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Import maps meshes from  '").Append(openFileDialog.FileName).AppendLine("':");
                            foreach (ZipArchiveEntry entry in profile.Entries)
                            {
                                if (entry.Name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                                {
                                    string entryMapName = entry.Name.Substring(0, entry.Name.Length - 4);
                                    Graph entryMapMeshes = null;
                                    using (Stream stream = entry.Open())
                                    {
                                        entryMapMeshes = binaryFormatter.Deserialize(stream) as Graph;
                                    }
                                    if (entryMapMeshes != null)
                                    {
                                        lock (mapsMeshes)
                                        {
                                            if (mapsMeshes.ContainsKey(entryMapName))
                                                mapsMeshes[entryMapName] = entryMapMeshes;
                                            else mapsMeshes.Add(entryMapName, entryMapMeshes); 
                                        }
                                        sb.Append("\t'").Append(currentMapMeshesName).AppendLine("' - succeeded");
                                    }
                                    else sb.Append("\t'").Append(currentMapMeshesName).AppendLine("' - failed");
                                }
                            } 
                            Logger.WriteLine(sb.ToString());
                        }

                    }
                }
                catch (Exception exc)
                {
                    Logger.WriteLine(exc.ToString());
                    XtraMessageBox.Show(exc.ToString());
                }
#if false
                // Код перенесен из Astral'a
                Dictionary<string, Graph> profileMeshes = new Dictionary<string, Graph>();
                List<Class88.Class89> list = new List<Class88.Class89>();
                List<Type> baseTypeList = new List<Type>
                {
                    typeof(Astral.Quester.Classes.Action),
                    typeof(Condition)
                };
                Class88.Class89 @class = new Class88.Class89(profileMeshes, Class88.Class89.Enum2.const_1, "meshes.bin", null);
                list.Add(@class);
                Class88.smethod_4(openFileDialog.FileName, list);
                if (@class.Success)
                {
                    profileMeshes = (@class.Object as Dictionary<string, Graph>);
                }
                else
                {
                    list.Clear();
                    profileMeshes = new Dictionary<string, Graph>();
                    //if (CoreAvailableMeshesFromFile != null)
                    //    foreach (string str in CoreAvailableMeshesFromFile(openFileDialog.FileName))
                    //    foreach (string str in Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName))
                    foreach (string str in AstralAccessors.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName))
                    {
                        Class88.Class89 item = new Class88.Class89(new Graph(), Class88.Class89.Enum2.const_1, str + ".bin", null);
                        list.Add(item);
                    }
                    Class88.smethod_4(openFileDialog.FileName, list);
                    foreach (Class88.Class89 class2 in list)
                    {
                        ETLogger.WriteLine("found : " + class2.FileName + " : " + class2.Success.ToString());
                        if (class2.Success)
                        {
                            profileMeshes.Add(class2.FileName.Replace(".bin", ""), class2.Object as Graph);
                        }
                    }
                }
                DialogResult dialogResult = XtraMessageBox.Show("Import nodes for the current map only ? Else import all.", "Nodes import", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {
                    if (profileMeshes.ContainsKey(mapName))
                    {
                        //if (((Graph)CoreCurrentMapMeshes).Nodes.Count == 0 
                        if (((Graph)AstralAccessors.Quester.Core.Meshes).Nodes.Count == 0
                            || Class81.smethod_0("Are you sure, that will delete current map nodes ?", null))
                        {
                            handler_StopMapping();
#if AstralMapper
                            MapperStopDrawing?.Invoke(mapper); 
#endif
                            //var coreMeshes = CoreMapsMeshes.Value;
                            var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes.Value;
                            lock (mapsMeshes)
                            {
                                mapsMeshes[mapName] = profileMeshes[mapName];
                            }
#if AstralMapper
                            MapperStartDrawing?.Invoke(mapper); 
#endif
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("This profile doesn't contain nodes for this map !");
                    }
                }
                if (dialogResult == DialogResult.No
                    //&& ((((Graph)CoreCurrentMapMeshes).Nodes.Count == 0 && CoreMapsMeshes.Value.Count <= 1)
                    && ((((Graph)AstralAccessors.Quester.Core.Meshes).Nodes.Count == 0 && AstralAccessors.Quester.Core.MapsMeshes.Value.Count <= 1)
                    || Class81.smethod_0("Are you sure, that will delete all maps nodes ?", null)))
                {
                    handler_StopMapping();
#if AstralMapper
                    MapperStopDrawing?.Invoke(mapper); 
#endif
                    lock (AstralAccessors.Quester.Core.MapsMeshes)
                    {
                        //CoreMapsMeshes.Value = profileMeshes;
                        AstralAccessors.Quester.Core.MapsMeshes.Value = profileMeshes;
                    }
#if AstralMapper
                    MapperStartDrawing?.Invoke(mapper); 
#endif
                }
#endif
            }
        }

        /// <summary>
        /// Экспорт узлов в файл 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ExportCurrentMapMeshes2File(object sender, ItemClickEventArgs e)
        {
#if false
            // Код перенесен из Astral'a
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directories.ProfilesPath,
                DefaultExt = "amp.zip",
                Filter = "Astral mesh file (*.mesh.zip)|*.mesh.zip"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Astral.Quester.Core.LoadAllMeshes();
                handler_StopMapping();
                //CoreLoadAllMeshes();
                AstralAccessors.Quester.Core.LoadAllMeshes();
                List<Class88.Class89> list = new List<Class88.Class89>();
                //foreach (KeyValuePair<string, Graph> keyValuePair in CoreMapsMeshes.Value)
                foreach (var keyValuePair in AstralAccessors.Quester.Core.MapsMeshes.Value)
                {
                    Class88.Class89 item = new Class88.Class89(keyValuePair.Value, Class88.Class89.Enum2.const_1, keyValuePair.Key + ".bin", null);
                    list.Add(item);
                }
                Class88.SaveMeshes2Files(saveFileDialog.FileName, list, false); 
            }
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Очистка мешей карты (графа)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ClearCurrentMapMeshes(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show(this, "Are you sure to delete all map nodes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_StopMapping();
#if AstralMapper
                MapperStopDrawing?.Invoke(mapper); 
#endif
                //Graph graph = CoreCurrentMapMeshes;
                Graph graph = AstralAccessors.Quester.Core.Meshes;
                lock (graph.Locker)
                {
                    graph.Clear();
                }
#if AstralMapper
                MapperStartDrawing?.Invoke(mapper); 
#endif
            }
        }
    #endregion

        #region CustomRegion_Manipulation
        /// <summary>
        /// Запуск процедуры добавления прямоугольного региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AddRectangularCR(object sender, ItemClickEventArgs e)
        {
            toolbarCustomRegion.FloatLocation = new Point(Location.X + 40, Location.Y + 60);
            toolbarCustomRegion.Visible = true;
            CustomRegionHelper.BeginAdd(mapper, false);
        }

        /// <summary>
        /// Запуск процедуры добавления элиптического региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AddElipticalCR(object sender, ItemClickEventArgs e)
        {
            toolbarCustomRegion.FloatLocation = new Point(Location.X + 40, Location.Y + 60);
            toolbarCustomRegion.Visible = true;
            CustomRegionHelper.BeginAdd(mapper, true);
        }

        /// <summary>
        /// Завершение процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_MenuCRAcceptClick(object sender, ItemClickEventArgs e)
        {
            string crName = menuCRName.EditValue.ToString()?.Trim();
            if (string.IsNullOrEmpty(crName))
            {
                XtraMessageBox.Show("The Name of the CustomRegion is not valid !");
                return;
            }
            else if (CustomRegionHelper.IsComplete)
            {
                if (CustomRegionHelper.Finish(crName))
                    toolbarCustomRegion.Visible = false;
                else XtraMessageBox.Show("The Name of the CustomRegion is not valid !");
            }
            else XtraMessageBox.Show("Finish the edition of the CuatomRegion!");
        }

        /// <summary>
        /// Прерывание процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_MenuCRCancelClick(object sender, ItemClickEventArgs e)
        {
            CustomRegionHelper.Reset();
            toolbarCustomRegion.Visible = false;
        }
        #endregion

        private void handler_DeleteRadiusChanged(object sender, EventArgs e)
        {
            Astral.API.CurrentSettings.DeleteNodeRadius = Convert.ToInt32(menuDeleteRadius.EditValue);
        }
        private void handler_WaypointDistanceChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.Mapper.WaypointDistance = Convert.ToInt32(menuWaypointDistance.EditValue);
        }

        private void handler_ZoomChanged(object sender, EventArgs e)
        {
            mapper.ZoomPos = Convert.ToInt32(statZoom.EditValue);
        }
        private void handler_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = 0;
            if(e.Delta > 0)
                delta = 1;
            else if(e.Delta < 0)
                delta = -1;
            //mapper.ZoomPos += delta;
            statZoom.EditValue = mapper.ZoomPos + delta;
        }
        private void handler_SetZoom(object sender, int zoomPos)
        {
            statZoom.EditValue = zoomPos;
        }

        private void handler_MapLockOnOnPlayerChanged(object sender, ItemClickEventArgs e)
        {
            mapper.MapLockOnPlayer = statLockMapOnPlayer.Checked;
        }
        private void handler_SetMapLockOnPlayer(object sender, DevExpress.XtraEditors.Filtering.CheckedChangedEventArgs e)
        {
            statLockMapOnPlayer.Checked = e.IsChecked;
        }

        private void handler_ShowStatusBar(object sender, EventArgs e)
        {
            statusBar.Visible = true;
            btnShowStatBar.Visible = false;
        }

        private void handler_VisibleChanged(object sender, EventArgs e)
        {
            btnShowStatBar.Visible = !statusBar.Visible && !toolbarMainMapper.Visible;
        }
    }
#endif
}
