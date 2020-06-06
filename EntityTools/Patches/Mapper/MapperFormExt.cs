#define LOG

#if PATCH_ASTRAL
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

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm //*/Form
    {
    #region Reflection helpers
        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static readonly StaticPropertyAccessor<AStar.Graph> CoreCurrentMapMeshes = typeof(Astral.Quester.Core).GetStaticProperty<AStar.Graph>("Meshes");

        /// <summary>
        /// Функтор доступа к словарю Astral.Quester.Core.MapsMeshes
        /// </summary>
        private static readonly StaticPropertyAccessor<Dictionary<string, Graph>> CoreMapsMeshes = typeof(Astral.Quester.Core).GetStaticProperty<Dictionary<string, Graph>>("MapsMeshes");

        /// <summary>
        /// Функтор доступа к списку карт в файле профиля
        /// Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName)
        /// </summary>
        private static readonly Func<string, List<string>> CoreAvailableMeshesFromFile = typeof(Astral.Quester.Core).GetStaticFunction<string, List<string>>("AvailableMeshesFromFile");

        //Astral.Quester.Core.LoadAllMeshes();
        private static readonly Func<int> CoreLoadAllMeshes = typeof(Astral.Quester.Core).GetStaticFunction<int>("LoadAllMeshes");

        // PictureBox Astral.Forms.UserControls.Mapper.MapPicture;
        //private static readonly StaticFielsAccessor<PictureBox> mapperPicture = typeof(Astral.Forms.UserControls.Mapper).GetField<PictureBox>("MapPicture", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Функтор доступа к Astral.Logic.NW.GoldenPath.GetCurrentMapGraph(...);
        /// </summary>
        //private static readonly Func<AStar.Graph, bool> GetCurrentMapGraph = typeof(Astral.Logic.NW.GoldenPath).GetStaticFunction<AStar.Graph, bool>("GetCurrentMapGraph");

        /// <summary>
        /// Функтор запускающего фоновый поток отрисовки изображения Mapper'a
        /// </summary>
        private static readonly Func<object, System.Action> MapperStartDrawing = typeof(Astral.Forms.UserControls.Mapper).GetAction("\u0002", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Функтор запускающего фоновый поток отрисовки изображения Mapper'a
        /// </summary>
        private static readonly Func<object, System.Action> MapperStopDrawing = typeof(Astral.Forms.UserControls.Mapper).GetAction("\u0003", BindingFlags.Instance | BindingFlags.Public);
    #endregion

        /// <summary>
        /// Кэш вершин графа путей
        /// </summary>
        private MapperGraphCache graphCache;

    #region Инициализация формы
        private MapperFormExt()
        {
            InitializeComponent();

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
            Controls.Add(mapper);

            //graphics = new GraphicsNW(Math.Max(mapBox.ClientSize.Width*2, 600), Math.Max(mapBox.ClientSize.Height*2, 600));

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
            menuWaypointDistance.Edit.EditValueChanged += eventWaypointDistanceChanged;
            menuWaypointDistance.Edit.Leave += eventWaypointDistanceChanged;

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
            menuDeleteRadius.Edit.EditValueChanged += eventDeleteRadiusChanged;
            menuDeleteRadius.Edit.Leave += eventDeleteRadiusChanged;

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
        private void eventLoadMapperForm(object sender, EventArgs e)
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
        private void eventClosingMapperForm(object sender, FormClosingEventArgs e)
        {
            MappingCanceler?.Cancel();
            backgroundWorker?.CancelAsync();
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
            CustomRegionHelper.Reset();

            lastNodeDetail = null;
            Binds.RemoveShiftAction(Keys.M);
        }

        /// <summary>
        /// Фоновый процесс обновления заголовка окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkUpdateFormCaption(object sender, DoWorkEventArgs e)
        {
            while (!IsDisposed
                   && !backgroundWorker.CancellationPending)
            {
                string text = EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName + " [" + EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName + "]";
                if (Text != text)
                {
                    if (InvokeRequired)
                        Invoke(new System.Action(() => Text = text));
                    else Text = text;
                }
                Thread.Sleep(1000);
            }
        }
    #endregion

    #region Mapping
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
                if (MapAndRegion != EntityManager.LocalPlayer.MapAndRegion)
                {
                    eventStopMapping();
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
        private bool LinearPath
        {
            get => menuLinearPath.Checked;
        }
        /// <summary>
        /// Флаг принудительного связывания с предыдущей точкой пути
        /// </summary>
        private bool ForceLinkLastWaypoint
        {
            get => menuForceLinkLast.Checked;
        }
        /// <summary>
        /// последний добавленный узел
        /// </summary>
        private NodeDetail lastNodeDetail = null;
        /// <summary>
        /// Название карты и региона, на которой активировано прокладывание пути
        /// </summary>
        private string MapAndRegion = null;

        /// <summary>
        /// Запуск потока прокладывания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventStartMapping(object sender, ItemClickEventArgs e)
        {
            if (e.Item is BarCheckItem checkedItem
                && checkedItem.Checked)
            {
                if (MappingTask == null
                    || MappingTask.IsCanceled || MappingTask.IsCompleted || MappingTask.IsFaulted)
                {
                    MapAndRegion = EntityManager.LocalPlayer.MapAndRegion;
                    MappingCanceler = new CancellationTokenSource();
                    MappingTask = Task.Factory.StartNew(() => MappingWork(MappingCanceler.Token), MappingCanceler.Token);
                    if (MappingTask != null)
                        MappingTask.ContinueWith(new System.Action<Task>((Task t) => eventStopMapping()));
                }
            }
        }

        /// <summary>
        /// Событие остановки прокладывания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventStopMapping(object sender = null, ItemClickEventArgs e = null)
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
        private void MappingWork(CancellationToken token)
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

    #region Mesh_Manipulation
        /// <summary>
        /// Импорт узлов из Игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventImportNodesFromGame(object sender, ItemClickEventArgs e)
        {
            if (((Graph)CoreCurrentMapMeshes)?.Nodes.Count == 0
                || XtraMessageBox.Show(this, "Are you sure to import game nodes ? All actual nodes must be delete !", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eventStopMapping();
                MapperStopDrawing?.Invoke(mapper);
                //string locker = "eventImportNodesFromGame";
                Graph graph = (Graph)CoreCurrentMapMeshes;
                lock (graph)
                {
                    graph.Clear();
                    Astral.Logic.NW.GoldenPath.GetCurrentMapGraph(graph);
                    //graphCache?.RegenCache(null);
                }
                MapperStartDrawing?.Invoke(mapper);
            }
            //GoldenPath.GetCurrentMapGraph(coreMeshes);
            //GetCurrentMapGraph?.Invoke(coreMeshes);
        }

        /// <summary>
        /// Импорт узлов из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventImportNodesFromFile(object sender, ItemClickEventArgs e)
        {
            // Код перенесен из Astral'a
            string mapName = EntityManager.LocalPlayer.MapState.MapName;
            if (string.IsNullOrEmpty(mapName))
            {
                XtraMessageBox.Show("Impossible to get current map name !");
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directories.ProfilesPath,
                DefaultExt = "amp.zip",
                Filter = "Astral mission profil (*.amp.zip)|*.amp.zip|(*.mesh.zip)|*.mesh.zip"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                eventStopMapping();
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
                    if (CoreAvailableMeshesFromFile != null)
                        foreach (string str in CoreAvailableMeshesFromFile(openFileDialog.FileName))
                        //foreach (string str in Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName))
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
                        if (((AStar.Graph)CoreCurrentMapMeshes).Nodes.Count == 0 || Class81.smethod_0("Are you sure, that will delete current map nodes ?", null))
                        {
                            eventStopMapping();
                            MapperStopDrawing?.Invoke(mapper);
                            //string locker = "eventImportNodesFromFile";
                            var coreMeshes = CoreMapsMeshes.Value;
                            lock (coreMeshes)
                            {
                                coreMeshes[mapName] = profileMeshes[mapName];
                            }
                            MapperStartDrawing?.Invoke(mapper);
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("This profile doesn't contain nodes for this map !");
                    }
                }
                if (dialogResult == DialogResult.No
                    && ((((AStar.Graph)CoreCurrentMapMeshes).Nodes.Count == 0 && CoreMapsMeshes.Value.Count <= 1)
                    || Class81.smethod_0("Are you sure, that will delete all maps nodes ?", null)))
                {
                    eventStopMapping();
                    MapperStopDrawing?.Invoke(mapper);
                    object locker = new object();//"eventImportNodesFromFile";
                    lock (locker)
                    {
                        CoreMapsMeshes.Value = profileMeshes;
                    }
                    MapperStartDrawing?.Invoke(mapper);
                }
            }
        }

        /// <summary>
        /// Экспорт узлов в файл 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventExportNodes2Mesh(object sender, ItemClickEventArgs e)
        {
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
                eventStopMapping();
                CoreLoadAllMeshes();
                List<Class88.Class89> list = new List<Class88.Class89>();
                foreach (KeyValuePair<string, Graph> keyValuePair in CoreMapsMeshes.Value)
                {
                    Class88.Class89 item = new Class88.Class89(keyValuePair.Value, Class88.Class89.Enum2.const_1, keyValuePair.Key + ".bin", null);
                    list.Add(item);
                }
                Class88.SaveMeshes2Files(saveFileDialog.FileName, list, false);
            }
        }

        /// <summary>
        /// Очистка графа мешей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventClearNodes(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show(this, "Are you sure to delete all map nodes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eventStopMapping();
                MapperStopDrawing?.Invoke(mapper);
                Graph graph = CoreCurrentMapMeshes;
                lock (graph)
                {
                    graph.Clear();
                }
                MapperStartDrawing?.Invoke(mapper);
            }
        }
    #endregion

    #region CustomRegion_Manipulation
        /// <summary>
        /// Запуск процедуры добавления прямоугольного региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eventAddRectangularCR(object sender, ItemClickEventArgs e)
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
        private void eventAddElipticalCR(object sender, ItemClickEventArgs e)
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
        private void eventMenuCRAcceptClick(object sender, ItemClickEventArgs e)
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
        private void eventMenuCRCancelClick(object sender, ItemClickEventArgs e)
        {
            CustomRegionHelper.Reset();
            toolbarCustomRegion.Visible = false;
        }
    #endregion

        private void eventDeleteRadiusChanged(object sender, EventArgs e)
        {
            Astral.API.CurrentSettings.DeleteNodeRadius = Convert.ToInt32(menuDeleteRadius.EditValue);
        }
        private void eventWaypointDistanceChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.Mapper.WaypointDistance = Convert.ToInt32(menuWaypointDistance.EditValue);
        }
    } 
#endif
}
