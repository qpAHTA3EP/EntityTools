using AStar;
using Astral;
using Astral.Controllers;
using Astral.Forms;
using Astral.Logic;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using EntityTools.Editors.Forms;
using EntityTools.Tools;
using EntityTools.Tools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using AstralMapperOriginals;
using QuesterEditorForm = Astral.Quester.Forms.Editor;
using QuesterMainForm = Astral.Quester.Forms.Main;

namespace EntityTools.Patches.Mapper
{
    public enum MappingType
    {
        Bidirectional,
        Unidirectional,
        Stoped
    }

    public partial class MapperFormExt : XtraForm //*/Form
    {
        #region Reflection helpers
        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static readonly StaticPropertyAccessor<AStar.Graph> CoreCurrentMapMeshe = typeof(Astral.Quester.Core).GetStaticProperty<AStar.Graph>("Meshes");

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

            //mapper = new Astral.Forms.UserControls.Mapper(); // Создание экземпляра при объявлении
            mapper.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            mapper.Dock = DockStyle.Fill;
            mapper.CustomDraw = null;
            mapper.Location = new Point(6, 26);
            mapper.Name = "mapper";
            mapper.OnClick = null;
            mapper.Size = new Size(372, 275);
            mapper.TabIndex = 0;
            Controls.Add(mapper);

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
            MappingCanceler.Cancel();
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
                        lastNodeDetail = AddNavigationNodeStatic.LinkNearest(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                    else lastNodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                }
                while (MappingType != MappingType.Stoped
                       && !token.IsCancellationRequested)
                {
                    /* 3. Вариант реализации с проверкой расстояния только до предыдущего узла*/
                    if (lastNodeDetail != null)
                        lastNodeDetail.Rebase(EntityManager.LocalPlayer.Location);

                    if (lastNodeDetail == null || lastNodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        switch(MappingType)
                        {
                            case MappingType.Bidirectional:
                                if (LinearPath)
                                {
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = AddNavigationNodeStatic.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = AddNavigationNodeStatic.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                else
                                {
                                    // Строим комплексный (многосвязный путь)
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                break;
                            case MappingType.Unidirectional:
                                {
                                    lastNodeDetail = AddNavigationNodeStatic.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, true) ?? lastNodeDetail;
                                }
                                break;
                        }
                    }
                    Thread.Sleep(100);
                }
                if (token.IsCancellationRequested)
                {
                    // Инициировано прерывание 
                    // Связываем текущее местоположение с графом
                    if (LinearPath)
                        AddNavigationNodeStatic.LinkNearest(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    else AddNavigationNodeStatic.LinkComplex(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    lastNodeDetail = null;
                    //token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
                Logger.WriteLine(Logger.LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {Meshes.Value.Nodes.Count}");
#endif
                Logger.WriteLine(ex.ToString());
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
            if (((Graph)CoreCurrentMapMeshe)?.Nodes.Count == 0 
                || XtraMessageBox.Show(this, "Are you sure to import game nodes ? All actual nodes must be delete !", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eventStopMapping();
                MapperStopDrawing?.Invoke(mapper);
                string locker = "eventImportNodesFromGame";
                lock(locker)
                {
                    ((Graph)CoreCurrentMapMeshe).Clear();
                    Astral.Logic.NW.GoldenPath.GetCurrentMapGraph(CoreCurrentMapMeshe);
                    graphCache.RegenCache(null);
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
                    if(CoreAvailableMeshesFromFile != null)
                        foreach (string str in CoreAvailableMeshesFromFile(openFileDialog.FileName))
                        //foreach (string str in Astral.Quester.Core.AvailableMeshesFromFile(openFileDialog.FileName))
                        {
                            Class88.Class89 item = new Class88.Class89(new Graph(), Class88.Class89.Enum2.const_1, str + ".bin", null);
                            list.Add(item);
                        }
                    Class88.smethod_4(openFileDialog.FileName, list);
                    foreach (Class88.Class89 class2 in list)
                    {
                        Logger.WriteLine("found : " + class2.FileName + " : " + class2.Success.ToString());
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
                        if (((AStar.Graph)CoreCurrentMapMeshe).Nodes.Count == 0 || Class81.smethod_0("Are you sure, that will delete current map nodes ?", null))
                        {
                            MapperStopDrawing?.Invoke(mapper);
                            string locker = "eventImportNodesFromFile";
                            lock (locker)
                            {
                                CoreMapsMeshes.Value[mapName] = profileMeshes[mapName];
                                graphCache.RegenCache();
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
                    && ((((AStar.Graph)CoreCurrentMapMeshe).Nodes.Count == 0 && CoreMapsMeshes.Value.Count <= 1)
                    || Class81.smethod_0("Are you sure, that will delete all maps nodes ?", null)))
                {
                    MapperStopDrawing?.Invoke(mapper);
                    string locker = "eventImportNodesFromFile";
                    lock (locker)
                    {
                        CoreMapsMeshes.Value = profileMeshes;
                        graphCache.RegenCache();
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directories.ProfilesPath;
            saveFileDialog.DefaultExt = "amp.zip";
            saveFileDialog.Filter = "Astral mesh file (*.mesh.zip)|*.mesh.zip";
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
                Graph g = ((Graph)CoreCurrentMapMeshe);
                lock (g)
                {
                    g.Clear();
                }
                graphCache.RegenCache(null);
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
            /*newRegion = new CustomRegion();
            XtraMessageBox.Show("Right click on map to create the region.\n" +
                "You should start by the left/top of desired area.\n" +
                "Right click again to finish the creation.");

            mapper.OnClick = delegate (MouseEventArgs me, GraphicsNW g)
            {
                if (me.Button == MouseButtons.Right)
                {
                    if (!newRegion.Position.IsValid)
                    {
                        Vector3 worldPos = g.getWorldPos(me.Location);
                        newRegion.Position = new Vector3((float)((int)worldPos.X), (float)((int)worldPos.Y), 0f);
                        return;
                    }
                    Vector3 worldPos2 = g.getWorldPos(me.Location);
                    worldPos2.X = (float)((int)worldPos2.X);
                    worldPos2.Y = (float)((int)worldPos2.Y);
                    newRegion.Width = Convert.ToInt32(worldPos2.X - newRegion.Position.X);
                    newRegion.Height = Convert.ToInt32(worldPos2.Y - newRegion.Position.Y);
                    newRegion.Name = InputBox.GetValue("Set region name");
                    if (newRegion.Name.Length > 0)
                    {
                        Astral.Quester.API.CurrentProfile.CustomRegions.Add(newRegion);
                        CustomRegionHelper.RefreshQuesterEditorForm();
                    }
                    mapper.OnClick = null;
                    mapper.CustomDraw = null;
                }
            };
            mapper.CustomDraw = delegate (GraphicsNW g)
            {
                if (newRegion.Position.IsValid)
                {
                    Vector3 worldPos = g.getWorldPos(mapper.RelativeMousePosition);
                    worldPos.X = (float)((int)worldPos.X);
                    worldPos.Y = (float)((int)worldPos.Y);
                    int num = newRegion.Width;
                    int num2 = newRegion.Height;
                    if (num == 0)
                    {
                        num = Convert.ToInt32(worldPos.X - newRegion.Position.X);
                        num2 = Convert.ToInt32(worldPos.Y - newRegion.Position.Y);
                    }
                    g.drawRectangle(newRegion.Position, new Vector3((float)num, (float)num2, 0f), Pens.Green);
                }
            };*/
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
    }

    /// <summary>
    /// Помощник для добавления CustomRegion
    /// </summary>
    internal static class CustomRegionHelper
    {
        public static CustomRegion Clone(this CustomRegion @this)
        {
            if (@this != null)
                return new CustomRegion()
                {
                    Position = @this.Position.Clone(),
                    Height = @this.Height,
                    Width = @this.Width,
                    Eliptic = @this.Eliptic,
                    Name = @this.Name
                };
            return null;
        }

        /// <summary>
        /// Режим перетаскивания якорей CustomRegion'a
        /// </summary>
        public enum DragMode
        {
            None, // перетаскивание якорей разрешено, но не производится
            TopLeft,
            TopCenter,
            TopRight,
            Left,
            Center,
            Right,
            DownLeft,
            DownCenter,
            DownRight,
            Disabled // перетаскивание якорей запрещено
        }

        private static Vector3 startPoint = null;
        private static Vector3 endPoint = null;
        private static bool isElliptical = false;
        private static Astral.Forms.UserControls.Mapper mapper;
        
        private static CustomRegion customRegion = null;

        internal static CustomRegion GetCustomRegion()
        {
            if (customRegion != null
                && customRegion.Position.IsValid
                && customRegion.Height != 0
                && customRegion.Width != 0)
                return customRegion;
            else
            {
                if (startPoint != null && startPoint.IsValid
                    && endPoint != null && endPoint.IsValid)
                {
                    Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                    Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                    return customRegion = new CustomRegion()
                    {
                        Position = topLeft,
                        Eliptic = isElliptical,
                        Height = (int)(downRight.Y - topLeft.Y),
                        Width = (int)(downRight.X - topLeft.X),
                        Name = string.Empty
                    };
                }
            }
            return null;
        }

        private static DragMode dragMode = DragMode.Disabled;
        private static Vector3 anchorPoint = null;
        private static readonly float anchorSize = 4;

        #region Reflection
        /// <summary>
        /// Функтор доступа к экземпляру Квестер-редактора
        /// Astral.Quester.Forms.Main.editorForm
        /// </summary>
        private static readonly StaticFielsAccessor<QuesterEditorForm> QuesterEditor = typeof(QuesterMainForm).GetStaticField<QuesterEditorForm>("editorForm");

        /// <summary>
        /// Функтор обновления списка CustomRegion'ов в окне Квестер-редактора
        /// </summary>
        private static Func<Object, System.Action> QuesterEditorRefreshRegions = null; 
        #endregion

        /// <summary>
        /// Начала процедуры добавления CustomRegion'a
        /// </summary>
        /// <param name="m"></param>
        /// <param name="elliptical"></param>
        public static void BeginAdd(Astral.Forms.UserControls.Mapper m, bool elliptical = false)
        {
            startPoint = null;
            endPoint = null;
            anchorPoint = null;
            customRegion = null;
            isElliptical = elliptical;
            dragMode = DragMode.Disabled;

            if (mapper != null && !mapper.IsDisposed)
            {
                mapper = m;
                mapper.OnClick += eventMapperClick;
                mapper.CustomDraw += eventMapperDraw;
            }
        }

        internal static bool IsComplete
        {
            get
            {
                return dragMode == DragMode.None
                       && ((customRegion != null
                                && customRegion.Position.IsValid
                                && customRegion.Height != 0
                                && customRegion.Width != 0)
                            || (startPoint != null && endPoint != null
                                && startPoint.IsValid
                                && startPoint != endPoint));
            }
        }

        private static void eventMapperClick(MouseEventArgs me, GraphicsNW g)
        {
            if (me.Button == MouseButtons.Right)
            {
                Vector3 worldPos = g.getWorldPos(me.Location);
                if (startPoint == null)
                {
                    startPoint = new Vector3((float)Math.Round(worldPos.X), (float)Math.Round(worldPos.Y), 0f);
                    return;
                }
                if(endPoint == null)
                {
                    endPoint = new Vector3((float)Math.Round(worldPos.X), (float)Math.Round(worldPos.Y), 0f);
                    dragMode = DragMode.None;
                }

                if (customRegion == null
                    || !customRegion.Position.IsValid)
                {
                    Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                    Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                    customRegion = new CustomRegion()
                    {
                        Position = topLeft,
                        Eliptic = isElliptical,
                        Height = (int)(downRight.Y - topLeft.Y),
                        Width = (int)(downRight.X - topLeft.X),
                    };
                    dragMode = DragMode.None;
                    return;
                }

                if (dragMode != DragMode.Disabled)
                {
                    if(anchorPoint == null)
                        // вычисление якоря
                        SelectAnchor(customRegion, worldPos, out anchorPoint, out dragMode);
                    else
                    {
                        // вычисление изменений/смещений региона
                        CustomRegion cr = TransformCustomRegion(customRegion, anchorPoint, worldPos, dragMode);
                        if(cr != null)
                        {
                            customRegion = cr;
                            dragMode = DragMode.None;
                            anchorPoint = null;
                        }
                    }
                }
            }
        }

        private static bool SelectAnchor(CustomRegion cr, Vector3 pos, out Vector3 anchor, out DragMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                Vector3 topLeft = cr.Position.Clone();
                // TopLeft
                if (CheckAnchorSelection(topLeft, pos))
                {
                    anchor = topLeft;
                    mode = DragMode.TopLeft;
                    return true;
                }

                // TopCenter
                Vector3 topCenter = new Vector3(topLeft.X + (float)cr.Width / 2f, topLeft.Y, 0f);
                if (CheckAnchorSelection(topCenter, pos))
                {
                    anchor = topCenter;
                    mode = DragMode.TopCenter;
                    return true;
                }

                // TopRight
                Vector3 topRight = new Vector3(topLeft.X + (float)cr.Width, topLeft.Y, 0f);
                if (CheckAnchorSelection(topRight, pos))
                {
                    anchor = topRight;
                    mode = DragMode.TopRight;
                    return true;
                }

                // Left
                Vector3 left = new Vector3(topLeft.X, topLeft.Y + (float)cr.Height / 2f, 0f);
                if (CheckAnchorSelection(left, pos))
                {
                    anchor = left;
                    mode = DragMode.Left;
                    return true;
                }

                // Center
                Vector3 center = new Vector3(topLeft.X + (float)cr.Width / 2f, topLeft.Y + (float)cr.Height / 2f, 0f);
                if (CheckAnchorSelection(center, pos))
                {
                    anchor = center;
                    mode = DragMode.Center;
                    return true;
                }

                // Right
                Vector3 right = new Vector3(topLeft.X + (float)cr.Width, topLeft.Y + (float)cr.Height / 2f, 0f);
                if (CheckAnchorSelection(right, pos))
                {
                    anchor = right;
                    mode = DragMode.Right;
                    return true;
                }

                // DownLeft
                Vector3 downLeft = new Vector3(topLeft.X, topLeft.Y + (float)cr.Height, 0f);
                if (CheckAnchorSelection(downLeft, pos))
                {
                    anchor = downLeft;
                    mode = DragMode.DownLeft;
                    return true;
                }

                // DownCenter
                Vector3 downCenter = new Vector3(topLeft.X + (float)cr.Width / 2f, topLeft.Y + (float)cr.Height, 0f);
                if (CheckAnchorSelection(downCenter, pos))
                {
                    anchor = downCenter;
                    mode = DragMode.DownCenter;
                    return true;
                }

                // DownRight
                Vector3 downRight = new Vector3(cr.Position.X + (float)cr.Width,
                                                cr.Position.Y + (float)cr.Height, 0f);
                if (CheckAnchorSelection(downRight, pos))
                {
                    anchor = downRight;
                    mode = DragMode.DownRight;
                    return true;
                }
            }
            anchor = null;
            mode = DragMode.None;
            return false;
        }

        private static bool CheckAnchorSelection(Vector3 anchor, Vector3 pos)
        {
            float dx = anchor.X - pos.X;
            float dy = anchor.Y - pos.Y;
            return Math.Sign(dx) * dx <= anchorSize
                && Math.Sign(dy) * dy <= anchorSize;
        }

        public static CustomRegion TransformCustomRegion(CustomRegion cr, Vector3 anchorPoint, Vector3 worldPos, DragMode mode)
        {
            if (cr != null
                && cr.Position.IsValid)
            {
                int dx = (int)Math.Round(worldPos.X - anchorPoint.X);
                int dy = (int)Math.Round(worldPos.Y - anchorPoint.Y);

                switch (mode)
                {
                    case DragMode.TopLeft:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        cr.Width -= dx;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.TopCenter:
                        cr.Position.Y += dy;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.TopRight:
                        cr.Position.Y += dy;
                        cr.Width += dx;
                        cr.Height -= dy;
                        return cr;
                    case DragMode.Left:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        return cr;
                    case DragMode.Center:
                        cr.Position.X += dx;
                        cr.Position.Y += dy;
                        return cr;
                    case DragMode.Right:
                        cr.Width += dx;
                        return cr;
                    case DragMode.DownLeft:
                        cr.Position.X += dx;
                        cr.Width -= dx;
                        cr.Height += dy;
                        return cr;
                    case DragMode.DownCenter:
                        cr.Height += dy;
                        return cr;
                    case DragMode.DownRight:
                        cr.Width += dx;
                        cr.Height += dy;
                        return cr;
                }
            }
            return null;
        }

        private static void eventMapperDraw(GraphicsNW g)
        {
            Vector3 topLeft, top, topRight, left, center, right, downLeft, down, downRight;
            float width, height;
            
            if (customRegion == null)
            {
                if (startPoint != null)
                {
                    if (endPoint != null)
                    {
                        topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X),
                                              Math.Max(startPoint.Y, endPoint.Y), 0f);
                        downRight = new Vector3(Math.Max(startPoint.X, endPoint.X),
                                                Math.Min(startPoint.Y, endPoint.Y), 0f);

                        height = (float)Math.Round(downRight.Y - topLeft.Y); // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                        width = (float)Math.Round(downRight.X - topLeft.X);  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)

                        customRegion = new CustomRegion()
                        {
                            Position = topLeft.Clone(),
                            Height = (int)height,
                            Width = (int)width,
                            Eliptic = isElliptical
                        };
                    }
                    else
                    {
                        Vector3 worldPos = g.getWorldPos(mapper.RelativeMousePosition);
                        topLeft = new Vector3((float)Math.Round(Math.Min(startPoint.X, worldPos.X)),
                                              (float)Math.Round(Math.Max(startPoint.Y, worldPos.Y)), 0f);
                        downRight = new Vector3((float)Math.Round(Math.Max(startPoint.X, worldPos.X)),
                                                (float)Math.Round(Math.Min(startPoint.Y, worldPos.Y)), 0f);//*/

                        height = (float)Math.Round(downRight.Y - topLeft.Y); // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                        width = (float)Math.Round(downRight.X - topLeft.X);  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)
                    }
                }
                else return;
            }
            else
            {
                CustomRegion cr;
                if (dragMode == DragMode.None)
                    cr = customRegion;
                else
                    cr = TransformCustomRegion(customRegion.Clone(), anchorPoint, g.getWorldPos(mapper.RelativeMousePosition), dragMode);

                if (cr == null)
                    return;

                topLeft = cr.Position;
                downRight = new Vector3(cr.Position.X + cr.Width,
                                        cr.Position.Y + cr.Height, 0);

                height = cr.Height; // Math.Max(startPoint.Y, endPoint.Y) - Math.Min(startPoint.Y, endPoint.Y)
                width = cr.Width;  // Math.Max(startPoint.X, endPoint.X) - Math.Min(startPoint.X, endPoint.X)
            }
            Pen penRect = (isElliptical) ? Pens.DarkOliveGreen : Pens.LimeGreen;
            Pen penCR = Pens.LimeGreen;
            Brush brushRect = (isElliptical) ? Brushes.DarkOliveGreen : Brushes.LimeGreen;
            Brush brushCR = Brushes.LimeGreen;

            // Отрисовака опорного прямоугольника якоря topLeft
            g.drawRectangle(topLeft, new Vector3(width, height, 0), penRect);
            top = new Vector3(topLeft.X + width / 2, topLeft.Y, 0f);
            down = new Vector3(topLeft.X + width / 2, topLeft.Y + height, 0f);//*/
            g.drawLine(top, down, penRect);
            right = new Vector3(topLeft.X + width, topLeft.Y + height / 2, 0f);
            left = new Vector3(topLeft.X, topLeft.Y + height / 2, 0f);
            g.drawLine(left, right, penRect);
            DrawAnchor(g, topLeft, brushRect);
            DrawAnchor(g, downRight, brushRect);
            topRight = new Vector3(topLeft.X+ width, topLeft.Y, 0f);
            DrawAnchor(g, topRight, brushRect);
            downLeft = new Vector3(topLeft.X, topLeft.Y + height, 0f);
            DrawAnchor(g, downLeft, brushRect);
            center = new Vector3(topLeft.X + width / 2, topLeft.Y + height / 2, 0f);
            DrawAnchor(g, center, brushRect);

            // Отрисовка Эллипса
            if(isElliptical)
                g.drawComplexeEllipse(topLeft, new Vector3(width, height, 0), penCR);

            // Отрисовака якоря top
            DrawAnchor(g, top, brushCR);
            // Отрисовака якоря left
            DrawAnchor(g, left, brushCR);
            // Отрисовака якоря right
            DrawAnchor(g, right, brushCR);
            // Отрисовака якоря down
            DrawAnchor(g, down, brushCR);
        }

        public static void DrawAnchor(GraphicsNW g, Vector3 anchor, Brush brush = null)
        {
            if (brush == null)
                brush = Brushes.Green;
            g.drawFillPolygon(new List<Vector3>() { new Vector3(anchor.X - anchorSize, anchor.Y - anchorSize, 0),
                                                    new Vector3(anchor.X + anchorSize, anchor.Y - anchorSize, 0),
                                                    new Vector3(anchor.X + anchorSize, anchor.Y + anchorSize, 0),
                                                    new Vector3(anchor.X - anchorSize, anchor.Y + anchorSize, 0),
                                                  }, brush);
        }

        internal static void RefreshQuesterEditorForm()
        {
            if (QuesterEditor.Value is QuesterEditorForm editor
                && editor?.IsDisposed == false)
            {
                if (QuesterEditorRefreshRegions == null)
                {
                    if ((QuesterEditorRefreshRegions = typeof(QuesterEditorForm).GetAction("RefreshRegions")) != null)
                        QuesterEditorRefreshRegions(editor)();
                }
                else QuesterEditorRefreshRegions(editor)();
            }
        }

        /// <summary>
        /// Завершение процедуры добавления CustomRegion'a
        /// </summary>
        internal static bool Finish(string crName)
        {
            if (dragMode == DragMode.None)
            {
                CustomRegion cr = GetCustomRegion();
                if (cr != null
                    && !string.IsNullOrEmpty(crName))
                {
                    if (customRegion == null
                        || !customRegion.Position.IsValid)
                    {
                        Vector3 topLeft = new Vector3(Math.Min(startPoint.X, endPoint.X), Math.Max(startPoint.Y, endPoint.Y), 0f);
                        Vector3 downRight = new Vector3(Math.Max(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), 0f);
                        customRegion = new CustomRegion()
                        {
                            Position = topLeft,
                            Eliptic = isElliptical,
                            Height = (int)(downRight.Y - topLeft.Y),
                            Width = (int)(downRight.X - topLeft.X),
                            Name = crName
                        };

                    }
                    else customRegion.Name = crName;

                    Astral.Quester.API.CurrentProfile.CustomRegions.Add(customRegion);
                    RefreshQuesterEditorForm();
                    Reset();

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Сброс всех сохраненных значений
        /// </summary>
        internal static void Reset()
        {
            startPoint = null;
            endPoint = null;
            anchorPoint = null;
            customRegion = null;
            dragMode = DragMode.Disabled;

            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.OnClick -= eventMapperClick;
                mapper.CustomDraw -= eventMapperDraw;
                mapper = null;
            }
        }
    }
}
