﻿#define LOG

#if PATCH_ASTRAL
using Astral.Quester.Classes;
using MyNW.Classes;
using MappingType = EntityTools.Enums.MappingType;
#endif

using AStar;
using Astral;
using Astral.Controllers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using EntityTools.Reflection;
using MyNW.Internals;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DevExpress.Utils.MVVM;

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
                ZoomPos = Convert.ToInt32(trackZoom.EditValue),
                MapLockOnPlayer = btnLockMapOnPlayer.Checked
            };
            mapper.OnMapLockOnPlayerChanged += handler_SetMapLockOnPlayer;
            //mapper.OnZoomChanged += event_SetZoom;
#endif
            Controls.Add(mapper);
            MouseWheel += handler_MouseWheel;

            barMainTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible;
            barEditMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMainTools.Visible && !barEditMeshes.Visible;
            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;
            
            BindingControls();
        }

        /// <summary>
        /// Привязка элементов управления к данным
        /// </summary>
        public void BindingControls()
        {
            editWaypointDistance.DataBindings.Add(nameof(editWaypointDistance.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            editWaypointDistance.Edit.EditValueChanged += handler_WaypointDistanceChanged;
            editWaypointDistance.Edit.Leave += handler_WaypointDistanceChanged;

            editMaxZDifference.DataBindings.Add(nameof(editMaxZDifference.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.MaxElevationDifference),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            editEquivalenceDistance.DataBindings.Add(nameof(editEquivalenceDistance.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
#if false
            menuCacheActive.DataBindings.Add(nameof(menuCacheActive.Checked),
                                                    EntityTools.PluginSettings.Mapper,
                                                    nameof(EntityTools.PluginSettings.Mapper.CacheActive),
                                                    false, DataSourceUpdateMode.OnPropertyChanged); 
#endif

            /* Astral.API.CurrentSettings.DeleteNodeRadius не реализует INotifyPropertyChanged
             * поэтому привязка нижеуказанным методом невозможна
             * menuDeleteRadius.DataBindings.Add(new Binding(nameof(menuDeleteRadius.EditValue),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(Astral.API.CurrentSettings.DeleteNodeRadius))); //*/

            ((ISupportInitialize)bsrcAstralSettings).BeginInit();
            bsrcAstralSettings.DataSource = Astral.API.CurrentSettings;
            editDeleteRadius.DataBindings.Add(nameof(editDeleteRadius.EditValue),
                                              bsrcAstralSettings,
                                              nameof(Astral.API.CurrentSettings.DeleteNodeRadius),
                                              false, DataSourceUpdateMode.OnPropertyChanged);
            ((ISupportInitialize)bsrcAstralSettings).EndInit();
            editDeleteRadius.Edit.EditValueChanged += handler_DeleteRadiusChanged;
            editDeleteRadius.Edit.Leave += handler_DeleteRadiusChanged;

            btnForceLinkLast.DataBindings.Add(nameof(btnForceLinkLast.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            /* Сохранение опции между сессиями не требуется
             * Отслеживание флага производится через свойство LinearPath
             * menuLinearPath.DataBindings.Add(nameof(menuLinearPath.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.LinearPath)); */

#if false
            // Настройки панели инструментов 
            // Привязка к элементам управления вызывает ошибку времени выполнения

            Location.DataBindings.Add(nameof(Location),
                            EntityTools.PluginSettings.Mapper.MapperForm.Location,
                            nameof(EntityTools.PluginSettings.Mapper.MapperForm.Location),
                            false, DataSourceUpdateMode.OnPropertyChanged);

            toolbarMainMapper.DataBindings.Add(nameof(toolbarMainMapper.Visible),
                                    EntityTools.PluginSettings.Mapper.MapperForm.MainToolBarVisible,
                                    nameof(EntityTools.PluginSettings.Mapper.MapperForm.MainToolBarVisible),
                                    false, DataSourceUpdateMode.OnPropertyChanged);
            statusBar.DataBindings.Add(nameof(statusBar.Visible),
                                    EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible,
                                    nameof(EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible),
                                    false, DataSourceUpdateMode.OnPropertyChanged);
#endif
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

            barMainTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible;
            barEditMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;
            btnShowStatBar.Visible = !barStatus.Visible && !barMainTools.Visible && !barEditMeshes.Visible;

            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;

            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// СОбытие при закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ClosingMapperForm(object sender, FormClosingEventArgs e)
        {
            EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible = barMainTools.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible = barEditMeshes.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.Location = Location;

            InterruptAllModifications();

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
            { Text = formCaption; lblMousePos.Caption = posStr; });

            while (!IsDisposed
                   && !backgroundWorker.CancellationPending)
            {
                if(editMode == MapperEditMode.None)
                formCaption = string.Concat(EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName, 
                    " [", EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName, ']');
                else formCaption = string.Concat(editMode, " | ",
                    EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName,
                    " [", EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName, ']');

                Vector3 pos = EntityManager.LocalPlayer.Location;
                posStr = !EntityManager.LocalPlayer.IsLoading ? $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1}" : "Loading";

                if (InvokeRequired)
                    Invoke(UpdateFormStatus);
                else
                {
                    Text = formCaption;
                    lblMousePos.Caption = posStr;
                }

                Thread.Sleep(500);
            }
        }
        #endregion

        /// <summary>
        /// Режим редактирования графа
        /// </summary>
        enum MapperEditMode
        {
            None,
            /// <summary>
            /// Прогладывание путей
            /// </summary>
            Mapping,
            /// <summary>
            /// Ручное перемещение вершин
            /// </summary>
            RelocateNodes,
            /// <summary>
            /// Удаление вершин
            /// </summary>
            DeleteNodes,
            /// <summary>
            /// Ручное изменение ребер
            /// </summary>
            EditEdges,
            /// <summary>
            /// Добавление CustomRegion'a
            /// </summary>
            AddCustomRegion,
            /// <summary>
            /// Редактирование CustomRegion'ов
            /// </summary>
            EditCustomRegion,
        }
        MapperEditMode editMode = MapperEditMode.None;

        /// <summary>
        /// Прерывание всех операций по изменению графа путей (мешей)
        /// </summary>
        private void InterruptAllModifications(MapperEditMode mode = MapperEditMode.None)
        {
            if (mode != MapperEditMode.Mapping)
                handler_StopMapping();

            if (mode != MapperEditMode.EditEdges)
            {
                btnEditEdges.Checked = false;
                MapperHelper_EditEdges.Reset();
            }

            if (mode != MapperEditMode.RelocateNodes)
            {
                btnMoveNodes.Checked = false;
                MapperHelper_MoveNodes.Reset();
            }
            if (mode != MapperEditMode.DeleteNodes)
            {
                btnRemoveNodes.Checked = false;
                MapperHelper_RemoveNodes.Reset();
            }

            if (mode != MapperEditMode.AddCustomRegion
                && mode != MapperEditMode.EditCustomRegion)
            {
                handler_CancelCRManipulation(@this, null);
                MapperHelper_CustomRegion.Reset();
            }
        }

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

                if (btnBidirectional.Checked)
                    return MappingType.Bidirectional;
                else if (btnUnidirectional.Checked)
                    return MappingType.Unidirectional;
                else return MappingType.Stoped;
            }
        }
        /// <summary>
        /// Флаг линейного пут
        /// </summary>
        private bool LinearPath => btnLinearPath.Checked;
        
        /// <summary>
        /// Флаг принудительного связывания с предыдущей точкой пути
        /// </summary>
        private bool ForceLinkLastWaypoint => btnForceLinkLast.Checked;

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
                if (MappingTask != null && !MappingTask.IsCanceled && !MappingTask.IsCompleted &&
                    !MappingTask.IsFaulted) return;

                editMode = MapperEditMode.Mapping;
                InterruptAllModifications(MapperEditMode.Mapping);

                mapAndRegion_whereMapping = EntityManager.LocalPlayer.MapAndRegion;
                MappingCanceler = new CancellationTokenSource();
                MappingTask = Task.Factory.StartNew(() => work_Mapping(MappingCanceler.Token), MappingCanceler.Token);
                MappingTask?.ContinueWith(t => handler_StopMapping());
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
            btnBidirectional.Checked = false;
            btnUnidirectional.Checked = false;
            btnStopMapping.Checked = true;
            graphCache?.StopCache();
        }

        /// <summary>
        /// Прокладывание пути
        /// </summary>
        private void work_Mapping(CancellationToken token)
        {
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
                        lastNodeDetail = MapperHelper_AddNodes.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                    else lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache);
                }
                while (MappingType != MappingType.Stoped
                       && !token.IsCancellationRequested)
                {
                    /* 3. Вариант реализации с проверкой расстояния только до предыдущего узла*/
                    lastNodeDetail?.Rebase(EntityManager.LocalPlayer.Location);

                    if (lastNodeDetail == null || lastNodeDetail.Distance > EntityTools.PluginSettings.Mapper.WaypointDistance)
                    {
                        switch (MappingType)
                        {
                            case MappingType.Bidirectional:
                                if (LinearPath)
                                {
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                else
                                {
                                    // Строим комплексный (многосвязный путь)
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, null, false) ?? lastNodeDetail;
                                }
                                break;
                            case MappingType.Unidirectional:
                                {
                                    lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, true) ?? lastNodeDetail;
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
                        MapperHelper_AddNodes.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    else MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), graphCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    lastNodeDetail = null;
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

        #region CustomRegion_Manipulation
        /// <summary>
        /// Флаг, указывающий, что CustomRegion'ы в профиле были изменены, и его необходимо "сохранить"
        /// </summary>
        bool CurrentProfileNeedSave
        {
            get
            {
                return !string.IsNullOrEmpty(editedQuesterProfileName) && Astral.Controllers.Settings.Get.LastQuesterProfile == editedQuesterProfileName;
            }
            set
            {
                if (value)
                    editedQuesterProfileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
                else editedQuesterProfileName = string.Empty;
            }
        }
        string editedQuesterProfileName = string.Empty;


        /// <summary>
        /// Запуск процедуры добавления прямоугольного региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AddRectangularCR(object sender, ItemClickEventArgs e)
        {
            editMode = MapperEditMode.EditCustomRegion;
            InterruptAllModifications(MapperEditMode.EditCustomRegion);

            btnLockMapOnPlayer.Checked = false;

            barCustomRegion.FloatLocation = new Point(Location.X + 40, Location.Y + 60);
            barCustomRegion.DockStyle = BarDockStyle.None;
            barCustomRegion.Visible = true;
            barEditCustomRegion.Visible = false;
            MapperHelper_CustomRegion.BeginAdd(mapper, false);
        }

        /// <summary>
        /// Запуск процедуры добавления элиптического региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AddElipticalCR(object sender, ItemClickEventArgs e)
        {
            editMode = MapperEditMode.EditCustomRegion;
            InterruptAllModifications(MapperEditMode.EditCustomRegion);

            btnLockMapOnPlayer.Checked = false;

            barCustomRegion.FloatLocation = new Point(Location.X + mapper.Location.X + 40, Location.Y + mapper.Location.Y + 60);
            barCustomRegion.DockStyle = BarDockStyle.None;
            barCustomRegion.Visible = true;
            barEditCustomRegion.Visible = false;
            MapperHelper_CustomRegion.BeginAdd(mapper, true);
        }

        /// <summary>
        /// Завершение процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FinishCRAddition(object sender, ItemClickEventArgs e)
        {
            string crName = editCRName.EditValue.ToString()?.Trim();
            if (string.IsNullOrEmpty(crName))
                XtraMessageBox.Show("The Name of the CustomRegion is not valid !");
            else if (MapperHelper_CustomRegion.IsComplete)
            {
                if (MapperHelper_CustomRegion.Finish(crName))
                {
                    barCustomRegion.Visible = false;
                    CurrentProfileNeedSave = true;
                    editMode = MapperEditMode.None;
                }
                else XtraMessageBox.Show("The Name of the CustomRegion is not valid !");
            }
            else XtraMessageBox.Show("Finish the edition of the CustomRegion!");
        }

        /// <summary>
        /// Выбор и редактирование существующего CustomRegion'а
        /// </summary>
        private void handler_EditCR(object sender, ItemClickEventArgs e)
        {

            if (btnEditCR.Checked)
            {
                if (Astral.Quester.API.CurrentProfile.CustomRegions?.Count > 0)
                {
                    editMode = MapperEditMode.EditCustomRegion;
                    InterruptAllModifications(MapperEditMode.EditCustomRegion);

                    btnLockMapOnPlayer.Checked = false;

                    // Привязываем список CustomRegion'ов
                    if(!ReferenceEquals(listCRSelector.DataSource, Astral.Quester.API.CurrentProfile.CustomRegions))
                    {
                        listCRSelector.DataSource = Astral.Quester.API.CurrentProfile.CustomRegions;
                        listCRSelector.ValueMember = "Name";
                        listCRSelector.DisplayMember = "Name";
                        editCRSelector.EditValue = Astral.Quester.API.CurrentProfile.CustomRegions.First();
                    }

                    btnRenameCR.Checked = false;

                    barEditCustomRegion.FloatLocation = new Point(Location.X + mapper.Location.X + 40,
                        Location.Y + mapper.Location.Y + 60);
                    barEditCustomRegion.DockStyle = BarDockStyle.None;
                    btnRenameCR.Checked = false;

                    handler_ChangedRenameCRMode();
                    barEditCustomRegion.Visible = true;
                    barCustomRegion.Visible = false;
                    return;
                }
                btnEditCR.Checked = false;
            }
            barEditCustomRegion.Visible = false;
        }

        /// <summary>
        /// Прерывание процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_CancelCRManipulation(object sender, ItemClickEventArgs e)
        {
            editMode = MapperEditMode.None;
            MapperHelper_CustomRegion.Reset();

            barCustomRegion.Visible = false;
            barEditCustomRegion.Visible = false;
            btnEditCR.Checked = false;
        }

        private void handler_ChangeCustomRegionType(object sender, ItemClickEventArgs e)
        {
            if (btnCRTypeSelector.Checked)
            {
                btnCRTypeSelector.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCREllipce;
            }
            else
            {
                btnCRTypeSelector.ImageOptions.Image = global::EntityTools.Properties.Resources.miniCRRectang;
            }
        }

        private void handler_ChangeEditedCR(object sender, EventArgs e)
        {
            if (editCRSelector.EditValue is CustomRegion cr)
            {
                editCRName.EditValue = cr.Name;
            }
            else
            {
                editCRName.EditValue = string.Empty;
            }
        }

        private void handler_ChangedRenameCRMode(object sender = null, ItemClickEventArgs e = null)
        {
            if (btnRenameCR.Checked)
            {
                editCRSelector.Visibility = BarItemVisibility.Never;
                editCRName.Visibility = BarItemVisibility.Always;
            }
            else
            {
                editCRSelector.Visibility = BarItemVisibility.Always;
                editCRName.Visibility = BarItemVisibility.Never;
            }
        }
        #endregion

        private void handler_DeleteRadiusChanged(object sender, EventArgs e)
        {
            Astral.API.CurrentSettings.DeleteNodeRadius = Convert.ToInt32(editDeleteRadius.EditValue);
        }
        private void handler_WaypointDistanceChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.Mapper.WaypointDistance = Convert.ToInt32(editWaypointDistance.EditValue);
        }

        private void handler_ZoomChanged(object sender, EventArgs e)
        {
            mapper.ZoomPos = Convert.ToInt32(trackZoom.EditValue);
        }
        private void handler_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = 0;
            if(e.Delta > 0)
                delta = 1;
            else if(e.Delta < 0)
                delta = -1;
            //mapper.ZoomPos += delta;
            trackZoom.EditValue = mapper.ZoomPos + delta;
        }
        private void handler_SetZoom(object sender, int zoomPos)
        {
            trackZoom.EditValue = zoomPos;
        }

        private void handler_MapLockOnOnPlayerChanged(object sender, ItemClickEventArgs e)
        {
            mapper.MapLockOnPlayer = btnLockMapOnPlayer.Checked;
        }

        private void handler_SetMapLockOnPlayer(object sender, DevExpress.XtraEditors.Filtering.CheckedChangedEventArgs e)
        {
            btnLockMapOnPlayer.Checked = e.IsChecked;
        }

        private void handler_ShowStatusBar(object sender, EventArgs e)
        {
            barStatus.Visible = true;
            btnShowStatBar.Visible = false;
        }

        private void handler_BarVisibleChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible = barMainTools.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible = barEditMeshes.Visible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMainTools.Visible && !barEditMeshes.Visible;
        }

        #region Meshes_Manipulation
        /// <summary>
        /// Сохранение в файл текущего Quester-профиля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_SaveChanges2QuesterProfile(object sender, ItemClickEventArgs e)
        {
            // Сохранение профиля реализовано в
            // Astral.Quester.Core.Save(false)

            // TODO: Выполнять "сжатие" графа (удалять невидимые вершины и ребра)
            // TODO: сохранять файл профиля, поскольку регионы хранятся в профиле.

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
                        using (ZipArchive zipFile = ZipFile.Open(fileName, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry zipMeshEntry = null;

                            // Проверяем наличие меша карты в zip-файле
                            foreach (ZipArchiveEntry entry in zipFile.Entries)
                            {
                                if (entry.Name.Equals(meshName, StringComparison.OrdinalIgnoreCase))
                                {
                                    zipMeshEntry = entry;
                                    break;
                                }
                            }

                            if (zipMeshEntry is null)
                                // Меш карты не обнаружен. Создаем
                                zipMeshEntry = zipFile.CreateEntry(meshName);
                            else
                            {
                                // Меш карты найден. Удаляем старый и создаем новый
                                zipMeshEntry.Delete();
                                zipMeshEntry = zipFile.CreateEntry(meshName);
                            }

                            // Сохранение графа путей (мешей) активной карты
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

                            // Сохранение регионов
                            if(CurrentProfileNeedSave)
                            {
                                ZipArchiveEntry zipProfileEntry = zipFile.GetEntry("profile.xml");

                                if(zipProfileEntry is null)
                                    zipProfileEntry = zipFile.CreateEntry("profile.xml");
                                else
                                {
                                    zipProfileEntry.Delete();
                                    zipProfileEntry = zipFile.CreateEntry("profile.xml");
                                }

                                XmlSerializer serializer = new XmlSerializer(Astral.Quester.API.CurrentProfile.GetType(), Patch_XmlSerializer_GetExtraTypes.QuesterTypes.ToArray());
                                using (var zipProfileStream = zipProfileEntry.Open())
                                {
                                    serializer.Serialize(zipProfileStream, Astral.Quester.API.CurrentProfile);
                                    Astral.Quester.API.CurrentProfile.Saved = true;
                                }

                                CurrentProfileNeedSave = false;
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
                ((Graph)AstralAccessors.Quester.Core.Meshes).Clear();
#if AstralMapper
                MapperStartDrawing?.Invoke(mapper); 
#endif
            }
        }
        #endregion

        #region EditMeshes
        private void handler_EditEdges_ModeChanged(object sender, ItemClickEventArgs e)
        {
            if (btnEditEdges.Checked)
            {
                editMode = MapperEditMode.EditEdges;
                InterruptAllModifications(MapperEditMode.EditEdges);

                btnLockMapOnPlayer.Checked = false;

                MapperHelper_EditEdges.Initialize(mapper);
            }
            else MapperHelper_EditEdges.Reset();
        }

        private void handler_RelocateNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
            if (btnMoveNodes.Checked)
            {
                editMode = MapperEditMode.RelocateNodes;
                InterruptAllModifications(MapperEditMode.RelocateNodes);

                btnLockMapOnPlayer.Checked = false;

                MapperHelper_MoveNodes.Initialize(mapper);
            }
            else MapperHelper_MoveNodes.Reset();
        }

        private void handler_DeleteNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
            if (btnRemoveNodes.Checked)
            {
                editMode = MapperEditMode.DeleteNodes;
                InterruptAllModifications(MapperEditMode.DeleteNodes);

                btnLockMapOnPlayer.Checked = false;

                MapperHelper_RemoveNodes.Initialize(mapper);
            }
            else MapperHelper_RemoveNodes.Reset();
        }

        private void handler_Undo(object sender, ItemClickEventArgs e)
        {
            InterruptAllModifications();
        }
        #endregion

        private void handler_MeshesInfo(object sender, ItemClickEventArgs e)
        {
            int correctNodeNum = 0;
            int unpasNodeNum = 0;
            var nodes = ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
            int correctArcNum = 0;
            int invalidArcNum = 0;
            int unpasArcNum = 0;
            int totalArcCount = 0;

            foreach (Node nd in nodes)
            {
                if (!nd.Passable)
                    unpasNodeNum++;
                else correctNodeNum++;
                foreach(Arc arc in nd.OutgoingArcs)
                {
                    totalArcCount++;
                    if (arc.Invalid)
                    {
                        invalidArcNum++;
                        if (!arc.Passable)
                            unpasArcNum++;
                    }
                    else if (!arc.Passable)
                        unpasArcNum++;
                    else correctArcNum++;
                }
            }

            XtraMessageBox.Show($"Total nodes: {nodes.Count}\n\r" +
                $"\tcorrect:\t{correctNodeNum}\n\r" +
                $"\tunpassable:\t{unpasNodeNum}\n\r" +
                $"Total arcs: {totalArcCount}\n\r" +
                $"\tcorrect:\t{correctArcNum}\n\r" +
                $"\tinvalid:\t{invalidArcNum}\n\r" +
                $"\tunpassable:\t{unpasArcNum}\n\r");
        }

        private void handler_MeshesCompression(object sender, ItemClickEventArgs e)
        {
            int correctNodeNum = 0;
            int unpasNodeNum = 0;
            var nodes = ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
            int totalNodeNum = nodes.Count;
            int correctArcNum = 0;
            int invalidArcNum = 0;
            int unpasArcNum = 0;
            int totalArcNum = 0;
            foreach (Node nd in nodes)
            {
                if (!nd.Passable)
                    unpasNodeNum++;
                else correctNodeNum++;

                foreach (Arc arc in nd.OutgoingArcs)
                {
                    totalArcNum++;
                    if (arc.Invalid)
                    {
                        invalidArcNum++;
                        if (!arc.Passable)
                            unpasArcNum++;
                    }
                    else if (!arc.Passable)
                        unpasArcNum++;
                    else correctArcNum++;
                }
            }

            if (((Graph)AstralAccessors.Quester.Core.Meshes).Compression() > 0)
            {
                int correctNodeNumNew = 0;
                int unpasNodeNumNew = 0;
                int correctArcNumNew = 0;
                int invalidArcNumNew = 0;
                int unpasArcNumNew = 0;
                int totalArcNumNew = 0;
                nodes = ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
                foreach (Node nd in nodes)
                {
                    if (!nd.Passable)
                        unpasNodeNumNew++;
                    else correctNodeNumNew++;
                    foreach (Arc arc in nd.OutgoingArcs)
                    {
                        totalArcNumNew++;
                        if (arc.Invalid)
                        {
                            invalidArcNumNew++;
                            if (!arc.Passable)
                                unpasArcNumNew++;
                        }
                        else if (!arc.Passable)
                            unpasArcNumNew++;
                        else correctArcNumNew++;
                    }
                }


                XtraMessageBox.Show($"Total nodes: {totalNodeNum} => {nodes.Count}\n\r" +
                    $"\tcorrect:\t{correctNodeNum} => {correctNodeNumNew}\n\r" +
                    $"\tunpassable:\t{unpasNodeNum} => {unpasNodeNumNew}\n\r" +
                    $"Total arcs: {totalArcNum} => {totalArcNumNew}\n\r" +
                    $"\tcorrect:\t{correctArcNum} => {correctArcNumNew}\n\r" +
                    $"\tinvalid:\t{invalidArcNum} => {invalidArcNumNew}\n\r" +
                    $"\tunpassable:\t{unpasArcNum} => {unpasArcNumNew}\n\r");
            }
            else XtraMessageBox.Show($"Meshes doesn't compressed\n\r" +
                                    $"Total nodes: {nodes.Count}\n\r" +
                                    $"\tcorrect:\t{correctNodeNum}\n\r" +
                                    $"\tunpassable:\t{unpasNodeNum}\n\r" +
                                    $"Total arcs: {totalArcNum}\n\r" +
                                    $"\tcorrect:\t{correctArcNum}\n\r" +
                                    $"\tinvalid:\t{invalidArcNum}\n\r" +
                                    $"\tunpassable:\t{unpasArcNum}\n\r");
        }
    }
#endif
}
