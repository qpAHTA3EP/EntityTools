#define LOG
#define DrawMapper_Measuring


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AStar;
using Astral;
using Astral.Controllers;
using Astral.Quester.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using EntityTools.Patches.Mapper.Tools;
using EntityTools.Properties;
using EntityTools.Reflection;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = System.Action;
using GoldenPath = Astral.Logic.NW.GoldenPath;
using MinimapWaypoint = MyNW.Classes.MinimapWaypoint;
using Timeout = Astral.Classes.Timeout;
#if PATCH_ASTRAL
using Astral.Logic.NW;
using Astral.Quester.Classes;
using MyNW.Classes;
#endif

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm//, IMapperForm //*/Form
    {
        #region Инструменты точечного редактирования графа
        /// <summary>
        /// Инструмент для выделения вершин
        /// </summary>
        private readonly NodeSelectTool _selectedNodes = new NodeSelectTool();
        /// <summary>
        /// Список изменений (для отката)
        /// </summary>
        private readonly Stack<IMapperTool> _undoStack = new Stack<IMapperTool>();

        /// <summary>
        /// Хэш-код текущей активной карты
        /// </summary>
        private int _currentMapHash;

        /// <summary>
        /// Активный инструмент изменения графа
        /// </summary>
        private IMapperTool CurrentTool
        {
            get => _currentTool;
            set
            {
                if (_currentTool != null)
                {
                    if (_currentTool.Applied)
                        _undoStack.Push(_currentTool);
                }
                _currentTool = value;
                if (value != null)
                    InterruptAllModifications(value.EditMode);
                else
                {
                    _selectedNodes.Clear();
                    InterruptAllModifications(MapperEditMode.None);
                }
            }
        }
        private IMapperTool _currentTool; 
        #endregion

        /// <summary>
        /// Флаг удержания персонажа в центре карты
        /// 
        /// </summary>
        private bool LockOnPlayer
        {
            get => btnLockMapOnPlayer.Checked;
            set => btnLockMapOnPlayer.Checked = value;
        }

        /// <summary>
        /// Координаты центра отображаемой карты
        /// </summary>
        private Vector3 CenterOfMap
        {
            get
            {
                using (_graphics.ReadLock())
                {
                    return _graphics.CenterPosition.Clone();
                }
            }
            set
            {
                if (value != null && value.IsValid)
                {
                    using (_graphics.WriteLock())
                    {
                        _graphics.CenterPosition = value.Clone();
                    }
                    LockOnPlayer = false;
                }
            }
        }

        /// <summary>
        /// Координаты курсора мыши, относительно формы <see cref="MapperFormExt"/>
        /// </summary>
        private Point RelativeMousePosition => MapPicture.PointToClient(MousePosition);

        private void ResetToolState()
        {
            _currentTool = null;
            using (_selectedNodes.WriteLock())
                _selectedNodes.Clear();
            _undoStack.Clear();
            InterruptAllModifications();
        }

        #region Инициализация формы
        private MapperFormExt()
        {
            InitializeComponent();

            MouseWheel += handler_MouseWheel;

            barMapping.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MappingBarVisible;
            barMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MeshesBarVisible;
            barNodeTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.NodeToolsBarVisible;
            barCustomRegions.Visible = EntityTools.PluginSettings.Mapper.MapperForm.CustomRegionBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barMeshes.Visible && !barNodeTools.Visible && !barCustomRegions.Visible;

            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;

            BindingControls();
            _mappingTool = new MappingTool(() => AstralAccessors.Quester.Core.Meshes.Value)
            {
                Linear = EntityTools.PluginSettings.Mapper.LinearPath,
                ForceLink = EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint
            };// (GetMappingGraph);

        }

        /// <summary>
        /// Привязка элементов управления к данным
        /// </summary>
        private void BindingControls()
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
            bsrcAstralSettings.DataSource = API.CurrentSettings;
            editDeleteRadius.DataBindings.Add(nameof(editDeleteRadius.EditValue),
                                              bsrcAstralSettings,
                                              nameof(API.CurrentSettings.DeleteNodeRadius),
                                              false, DataSourceUpdateMode.OnPropertyChanged);
            ((ISupportInitialize)bsrcAstralSettings).EndInit();
            editDeleteRadius.Edit.EditValueChanged += handler_DeleteRadiusChanged;
            editDeleteRadius.Edit.Leave += handler_DeleteRadiusChanged;

            btnMappingForceLink.DataBindings.Add(nameof(btnMappingForceLink.Checked),
                                                EntityTools.PluginSettings.Mapper,
                                                nameof(EntityTools.PluginSettings.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            editBidirPathColor.DataBindings.Add(nameof(editBidirPathColor.EditValue),
                                                EntityTools.PluginSettings.Mapper.MapperForm,
                                                nameof(EntityTools.PluginSettings.Mapper.MapperForm.BidirectionalPathColor),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            editUnidirPathColor.DataBindings.Add(nameof(editUnidirPathColor.EditValue),
                                                EntityTools.PluginSettings.Mapper.MapperForm,
                                                nameof(EntityTools.PluginSettings.Mapper.MapperForm.UnidirectionalPathColor),
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
                    @this.Focus();
                else
                {
                    @this = new MapperFormExt();
                    @this.Show();
                }
            }
            else MapperForm.Open();
        }

        /// <summary>
        /// Событие при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FormLoad(object sender, EventArgs e)
        {
            Binds.RemoveShiftAction(Keys.M);

            barMapping.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MappingBarVisible;
            barMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MeshesBarVisible;
            barNodeTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.NodeToolsBarVisible;
            barCustomRegions.Visible = EntityTools.PluginSettings.Mapper.MapperForm.CustomRegionBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barMeshes.Visible && !barNodeTools.Visible && !barCustomRegions.Visible;
            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;
            Size = EntityTools.PluginSettings.Mapper.MapperForm.Size;
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Обработчик события закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FormClose(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();

            EntityTools.PluginSettings.Mapper.MapperForm.MappingBarVisible = barMapping.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.MeshesBarVisible = barMeshes.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.NodeToolsBarVisible = barNodeTools.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.CustomRegionBarVisible = barCustomRegions.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barMeshes.Visible && !barNodeTools.Visible && !barCustomRegions.Visible;

            EntityTools.PluginSettings.Mapper.MapperForm.Location = Location;
            EntityTools.PluginSettings.Mapper.MapperForm.Size = Size;

            InterruptAllModifications();

            Binds.RemoveShiftAction(Keys.M);
        }

        /// <summary>
        /// Фоновый процесс обновления окна Mapper'a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void work_MapperFormUpdate(object sender, DoWorkEventArgs e)
        {
            string formCaption = string.Empty,
                   statusStr = string.Empty,
                   zoomStr = string.Empty;
            //Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);

            var updateFormStatus = new Action(() =>
            {
                Text = formCaption;
                lblMousePos.Caption = statusStr;
                lblZoom.Caption = zoomStr;
                using (_graphics.ReadLock())
                    MapPicture.Image = _graphics.getImage();
            });


#if DrawMapper_Measuring
            Stopwatch sw = new Stopwatch();
            long[] drawMapperMeasures = new long[10];
            int currentMesure = 0;

            int time = 5000;
            Timeout timeout = new Timeout(0);
            double frames = 0;
            double fps = 0;
#endif
            try
            {
                while (!IsDisposed
                        && !backgroundWorker.CancellationPending)
                {
#if false
                if (editMode == MapperEditMode.None)
                    formCaption = string.Concat(EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName,
                        " [", EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName, ']');
                else formCaption = string.Concat(editMode, " | ",
                    EntityManager.LocalPlayer.CurrentZoneMapInfo.DisplayName,
                    " [", EntityManager.LocalPlayer.CurrentZoneMapInfo.MapName, ']'); 
#endif

                    var player = EntityManager.LocalPlayer;
#if !DrawMapper_Measuring
                    Vector3 pos = player.Location;
                    statusStr = !player.IsLoading && pos.IsValid ? $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1}" : "Loading"; 
#endif
                    zoomStr = string.Concat(Zoom * 100, '%');

                    int hash = AstralAccessors.Quester.Core.Meshes.Value.GetHashCode();
                    if (_currentMapHash != hash)
                    {
                        var currentMapInfo = player.CurrentZoneMapInfo;
                        formCaption = formCaption = string.Concat(currentMapInfo.DisplayName, '[', currentMapInfo.MapName, ']');
                        _currentMapHash = hash;

                        // Карта изменилась - сбрасываем состояние инструментов
                        ResetToolState();
                    }

#if DrawMapper_Measuring
                    sw.Restart();
                    DrawMapper();
                    sw.Stop();
                    drawMapperMeasures[currentMesure % 10] = sw.ElapsedTicks;
                    currentMesure++;
                    frames++;
                    if (timeout.IsTimedOut)
                    {

                        fps = frames / time * 1000;
                        frames = 0;
                        timeout.ChangeTime(time);
                    }
                    statusStr = string.Concat(fps.ToString("N1"), " fps | ", (drawMapperMeasures.Sum() / 10_0000d).ToString("N2"), " ms");
#endif


                    if (InvokeRequired)
                        Invoke(updateFormStatus);
                    else
                    {
                        Text = formCaption;
                        lblMousePos.Caption = statusStr;
                        lblZoom.Caption = zoomStr;
                        using (_graphics.ReadLock())
                            MapPicture.Image = _graphics.getImage();
                    }

#if Non_MonoMapper
                    Thread.Sleep(150); 
#else
                    Thread.Sleep(EntityTools.PluginSettings.Mapper.MapperForm.RedrawMapperTimeout);
#endif
                }
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, "MapperFormUpdate: Перехвачено исключение: " + exc);
                throw;
            }
        }
        #endregion

#if false
        public delegate void CustomMapperDraw(/*MapperFormExt sender,*/ MapperGraphics graphics);
        public delegate void CustomMouseEvent(/*MapperFormExt sender,*/ MapperGraphics graphics, MouseEventArgs e);
        public delegate void CustomKeyEvent(/*MapperFormExt sender,*/ MapperGraphics graphics, KeyEventArgs e);

        /// <summary>
        /// Событие  отрисовки специальной графики на карте
        /// </summary>
        public event CustomMapperDraw OnMapperDraw;
        /// <summary>
        /// Клик мыши на Mapper'e
        /// </summary>
        public event CustomMouseEvent OnMapperMouseClick;
        /// <summary>
        /// Двойной клик мыши на Mapper'e
        /// </summary>
        public event CustomMouseEvent OnMapperMouseDoubleClick;
        /// <summary>
        /// Отпускание клавиши на Mapper'e
        /// </summary>
        public event CustomKeyEvent OnMapperKeyUp;
        /// <summary>
        /// Отпускание клавиши мыши
        /// </summary>
        public event CustomMouseEvent OnMapperMouseUp; 
#endif

        #region Переадресация событий инструментам
        /// <summary>
        /// Обработчик события MouseClick - Уведомление активного инструмента о нажатии кнопки мыши
        /// </summary>
        private void handler_MouseClick(object sender, MouseEventArgs e)
        {
            var tool = CurrentTool;
            if (tool != null)
            {
                double x = 0, y = 0;
                IMapperTool undo = null;
                if (tool.AllowNodeSelection)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    MapperMouseEventArgs me = new MapperMouseEventArgs(e.Button, e.Clicks, x, y);

                    var graph = _graphics.VisibleGraph;
                    using (graph.WriteLock())
                    {
                        using (_selectedNodes.WriteLock())
                        {
                            _selectedNodes.OnMouseClick(graph, me);
                            if (tool.HandleMouseClick)
                                tool.OnMouseClick(graph, _selectedNodes, me, out undo);
                        }
                    }
                    if (undo != null)
                        _undoStack.Push(undo);
                }
                else if (tool.HandleMouseClick)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    MapperMouseEventArgs me = new MapperMouseEventArgs(e.Button, e.Clicks, x, y);

                    var graph = _graphics.VisibleGraph;
                    using (graph.WriteLock())
                        tool.OnMouseClick(graph, null, me, out undo);
                }
            }
        }

        private void handler_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        /// <summary>
        /// Обработчик события KeyUp - уведомления активного иснтрумента о нажатой клавише
        /// </summary>
        private void handler_KeyUp(object sender, KeyEventArgs e)
        {
            var tool = CurrentTool;
            if (tool != null)
            {
                double x = 0, y = 0;
                IMapperTool undo = null;
                if (tool.AllowNodeSelection)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    var graph = _graphics.VisibleGraph;
                    using (graph.WriteLock())
                    {
                        using (_selectedNodes.WriteLock())
                        {
                            _selectedNodes.OnKeyUp(graph, e);
                            if (tool.HandleMouseClick)
                                tool.OnKeyUp(graph, _selectedNodes, e, x, y, out undo);
                        }
                    }
                    if (undo != null)
                        _undoStack.Push(undo);
                }
                else if (tool.HandleMouseClick)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    var graph = _graphics.VisibleGraph;
                    using (graph.WriteLock())
                        tool.OnKeyUp(graph, _selectedNodes, e, x, y, out undo);
                }
            }
        } 
        #endregion

        #region Перемещение изображения
        private Point mouseClickPosition;

        private void handler_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mouseClickPosition == Point.Empty)
                {
                    mouseClickPosition.X = e.X;
                    mouseClickPosition.Y = e.Y;
                    return;
                }

                double scale = Zoom;
                double dx = (mouseClickPosition.X - e.Location.X) / scale;
                double dy = (e.Location.Y - mouseClickPosition.Y) / scale;

                LockOnPlayer = false;
                using (_graphics.ReadLock())
                    _graphics.MoveCenterPosition(dx, dy);
                mouseClickPosition.X = e.X;
                mouseClickPosition.Y = e.Y;
            }
        }

        private void handler_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClickPosition.X = 0;
            mouseClickPosition.Y = 0;
        }
        #endregion

        /// <summary>
        /// Прерывание всех операций по изменению графа путей (мешей)
        /// </summary>
        private void InterruptAllModifications(MapperEditMode mode = MapperEditMode.None)
        {
            if (mode != MapperEditMode.Mapping)
            {
                btnMappingStop.Checked = true;
            }
            if (mode != MapperEditMode.EditEdges)
            {
                btnEditEdges.Checked = false;
            }
            if (mode != MapperEditMode.RelocateNodes)
            {
                btnMoveNodes.Checked = false;
            } 
            if (mode != MapperEditMode.DeleteNodes)
            {
                btnRemoveNodes.Checked = false;
            }

            if (mode != MapperEditMode.AddCustomRegion)
            {
                barEditCustomRegion.Visible = false;
            }

            if (mode != MapperEditMode.EditCustomRegion)
            {
                btnEditCR.Checked = false;
                barEditCustomRegion.Visible = false;
            }
        }

        #region Управление масштабом
        /// <summary>
        /// Коэффициент масштабирования
        /// </summary>
        public double Zoom { get; private set; } = 1.0;

        private void handler_ZoomIn(object sender, ItemClickEventArgs e = null)
        {

            if (Zoom < 0.4)
                Zoom = Math.Round(Zoom + 0.1, 1);
            else if (Zoom <= 0.8)
                Zoom = Math.Round(Zoom + 0.2, 1);
            else if (Zoom <= 4.5)
                Zoom = Math.Round(Zoom + 0.5, 1);
            else if (Zoom <= 7)
                Zoom = Math.Round(Zoom + 1);
            if (Zoom < 0.1)
                Zoom = 0.1;
            else if (Zoom > 8)
                Zoom = 8;
            //editZoom.EditValue = Zoom;
            lblZoom.Caption = $"{Zoom * 100}%";
        }

        private void handler_ZoomOut(object sender, ItemClickEventArgs e = null)
        {
            if (Zoom <= 0.5)
                Zoom = Math.Round(Zoom - 0.1, 1);
            else if (Zoom <= 1.0)
                Zoom = Math.Round(Zoom - 0.2, 1);
            else if (Zoom <= 5.0)
                Zoom = Math.Round(Zoom - 0.5, 1);
            else if (Zoom <= 8)
                Zoom = Math.Round(Zoom - 1);
            if (Zoom < 0.1)
                Zoom = 0.1;
            else if (Zoom > 8)
                Zoom = 8;
            //editZoom.EditValue = Zoom;
            lblZoom.Caption = $"{Zoom * 100}%";
        }

        private void handler_DoubleClickZoom(object sender, ItemClickEventArgs e)
        {
            Zoom = 2.5;
        }

        Timeout mouseWeelTimeout = new Timeout(0);
        private void handler_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mouseWeelTimeout.IsTimedOut)
            {
                if (e.Delta > 0)
                    handler_ZoomIn(sender);
                else if (e.Delta < 0)
                    handler_ZoomOut(sender);
                mouseWeelTimeout.ChangeTime(100);
            }
        } 
        #endregion

        #region Drawings
        private readonly MapperGraphics _graphics = new MapperGraphics(360, 360);

        /// <summary>
        /// Метод для фоновой отрисовки карты
        /// </summary>
        private void DrawMapper()
        {
            Timeout timeout = new Timeout(0);
#if Not_MonoMapper
            while (!IsDisposed && Visible && !token.IsCancellationRequested) 
#else
            if (!IsDisposed && Visible
                 && MapPicture.Visible && MapPicture.Width > 0 && MapPicture.Height > 0)
#endif
            {
                try
                {
                    using (_graphics.WriteLock())
                    {
                        if (LockOnPlayer)
                            _graphics.CenterPosition = EntityManager.LocalPlayer.Location;

                        int imgWidth = MapPicture.Width;
                        int imgHeight = MapPicture.Height;
                        _graphics.ImageWidth = imgWidth;
                        _graphics.ImageHeight = imgHeight;
                        _graphics.Zoom = Zoom;
                        _graphics.resetImage();

                        // Вычисляем координаты границ изображения
                        _graphics.GetWorldPosition(0, 0, out double leftBorder, out double topBorder);
                        _graphics.GetWorldPosition(imgWidth, imgHeight, out double rightBorder, out double downBorder);

                        Vector3 location = null;
                        float x, y;

                        #region Отрисовка нодов
                        foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                        {
                            location = targetableNode.WorldInteractionNode.Location;
                            x = location.X;
                            y = location.Y;
                            if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                            {
                                if (targetableNode.Categories.Contains("Loot"))
                                    _graphics.FillCircleCentered(Brushes.Gold, x, y, 6);
                                else _graphics.FillSquareCentered(Brushes.YellowGreen, x, y, 6);
                            }
                        }
                        #endregion

                        #region Отрисовка костров
                        foreach (MinimapWaypoint minimapWaypoint in EntityManager.LocalPlayer.Player.MissionInfo.Waypoints)
                        {
                            location = minimapWaypoint.Position;
                            x = location.X;
                            y = location.Y;
                            if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder
                                && minimapWaypoint.IsCampFire)
                            {
                                _graphics.FillSquareCentered(Brushes.DarkOrange, x, y, 12);
                            }
                        }
                        #endregion

                        #region Отрисовка специального объекта, заданного методом showObjectOnMap
#if false
                        if (objectPosition != null && objectPosition.IsValid)
                            graphicsNW.drawFillEllipse(objectPosition, new Size(7, 7), Brushes.Orange);

                        // Отрисовка специальной графики
                        CustomDraw?.Invoke(graphicsNW); 
#endif
                        #endregion

                        #region Отрисовка АОЕ
#if false
                        //foreach (AOECheck.AOE aoe in AOECheck.List)
                        if (AstralAccessors.Controllers.AOECheck.List != null)
                        {
                            foreach (AOECheck.AOE aoe in AstralAccessors.Controllers.AOECheck.GetAOEList<AOECheck.AOE>())
                            {
                                Vector3 vector = new Vector3();
                                if (aoe.Location != null)
                                {
                                    vector = aoe.Location();
                                }
                                if (aoe.Source != null && aoe.Source.IsValid)
                                {
                                    vector = aoe.Source.Location;
                                }
                                if (aoe.Radius >= 1f && aoe.Radius < 50f && vector.IsValid)
                                {
                                    Brush brush = Brushes.Red;
                                    if (aoe.IsCustom)
                                    {
                                        brush = Brushes.Purple;
                                    }
                                    graphicsNW.drawFillEllipse(vector, (double)aoe.Radius, brush);
                                }
                            }
                        }
#endif
                        #endregion

                        #region Отрисовка графики, связанной с выполняемой ролью 
                        try
                        {
                            if (!AstralAccessors.Controllers.Roles.CurrentRole_OnMapDraw(_graphics))
                                //lock (AstralAccessors.Quester.Core.Meshes.Value.SyncRoot) <- Блокировка графа есть в DrawMeshes(..)
                                Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes.DrawMeshes(_graphics,
                                    AstralAccessors.Quester.Core.Meshes);
                        }
                        catch (Exception ex)
                        {
                            ETLogger.WriteLine(LogType.Error, string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                            Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes.DrawMeshes(_graphics,
                                    AstralAccessors.Quester.Core.Meshes);
                        }
                        #endregion

                        #region Отрисовка игроков и НПС
                        uint playerContainerId = EntityManager.LocalPlayer.ContainerId;
                        foreach (Entity entity in EntityManager.GetEntities())
                        {
                            location = entity.Location;
                            x = location.X;
                            y = location.Y;
                            if (entity.ContainerId != playerContainerId
                                && leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                            {
                                if (entity.IsPlayer)
                                    _graphics.FillRhombCentered(Brushes.LawnGreen, location, 10, 17);
                                else if (!entity.IsDead && !entity.Critter.IsLootable)
                                {
                                    var relationToPlayer = entity.RelationToPlayer;
                                    if (relationToPlayer == EntityRelation.Foe)
                                        _graphics.FillRhombCentered(Brushes.OrangeRed, location, 10, 10);
                                    else if (relationToPlayer == EntityRelation.Friend)
                                        _graphics.FillRhombCentered(Brushes.Green, location, 6, 6);
                                    else _graphics.FillRhombCentered(Brushes.LightGray, location, 6, 6);
                                }
                            }
                        }
                        #endregion

                        #region Отрисовка специальной графики
                        // Отрисовка активного инструмента редактирования
                        try
                        {
                            var tool = CurrentTool;
                            if (tool != null
                                && (tool.AllowNodeSelection || tool.HandleCustomDraw))
                            {
                                _graphics.GetWorldPosition(RelativeMousePosition, out double mouseX, out double mouseY);
                                if (tool.AllowNodeSelection)
                                {
                                    using (_selectedNodes.ReadLock())
                                    {
                                        _selectedNodes.OnCustomDraw(_graphics, mouseX, mouseY);

                                        if (tool.HandleCustomDraw)
                                            tool.OnCustomDraw(_graphics, _selectedNodes, mouseX, mouseY);
                                    }
                                }
                                else if (tool.HandleCustomDraw)
                                    tool.OnCustomDraw(_graphics, null, mouseX, mouseY);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (timeout.IsTimedOut)
                            {
                                ETLogger.WriteLine(LogType.Error,
                                    string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                                timeout.ChangeTime(2000);
                            }
                        }

                        // Отрисовка инструмента прокладывания пути
                        try
                        {
                            if (_mappingTool.MappingMode != MappingMode.Stoped)
                                _mappingTool.OnCustomDraw(_graphics);
                        }
                        catch (Exception ex)
                        {
                            if (timeout.IsTimedOut)
                            {
                                ETLogger.WriteLine(LogType.Error,
                                    string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                                timeout.ChangeTime(2000);
                            }
                        }
                        #endregion

                        #region Отрисовка области уклонения от АОЕ
#if false
                        //if (Astral.Logic.NW.Movements.LastValidPoses != null && !Astral.Logic.NW.Movements.lastvlidposto.IsTimedOut)
                        if (AstralAccessors.Logic.NW.Movements.LastValidPoses.IsValid()
                            && AstralAccessors.Logic.NW.Movements.LastValidPoses.Value != null
                            && !AstralAccessors.Logic.NW.Movements.LastValidPosesTimeout.Value.IsTimedOut)
                        {
                            foreach (Astral.Logic.NW.Movements.DodgeLosTestResult dodgeLosTestResult in AstralAccessors.Logic.NW.Movements.LastValidPoses.Value)
                            {
                                if (dodgeLosTestResult.Collided)
                                {
                                    graphicsNW.drawFillEllipse(dodgeLosTestResult.CollidePos, new Size(4, 4), Brushes.Orange);
                                }
                                else
                                {
                                    graphicsNW.drawFillEllipse(dodgeLosTestResult.TestedPos, new Size(4, 4), Brushes.Green);
                                }
                            }
                        }
#endif
                        #endregion

                        #region Отрисовка точки назначения
                        location = Navigator.GetDestination;
                        x = location.X;
                        y = location.Y;
                        if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                            _graphics.DrawCircleCentered(Pens.Red, location, 12);
                        #endregion

                        #region Отрисовка персонажа
                        location = EntityManager.LocalPlayer.Location;
                        x = location.X;
                        y = location.Y;
                        if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                        {
                            double angle = EntityManager.LocalPlayer.Yaw * 180 / Math.PI;
#if true
                            Bitmap image = MapperHelper.RotateImage(Resources.charArrow, (float)angle);
                            //_graphics.FillCircleCentered(Brushes.DarkGoldenrod, EntityManager.LocalPlayer.Location, 15);
                            _graphics.drawImage(location, image);
#else
                        _graphics.FillCircleCentered(Brushes.DarkGoldenrod, EntityManager.LocalPlayer.Location, 15);
                        _graphics.DrawImageCentered(EntityManager.LocalPlayer.Location, Properties.Resources.charArrow, angle);
#endif
                        }
                        #endregion
                    }
                }
#if Not_MonoMapper
                catch (ObjectDisposedException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                    break; 
                }
                catch (InvalidOperationException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                    break; 
                }
                catch (ThreadAbortException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                    break; 
                    throw;
                }
                catch (Exception ex)
                {
                    if (timeout.IsTimedOut)
                    {
                        timeout.ChangeTime(2000);
                        Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                    }
                }
                finally
                {
                    Thread.Sleep(150);
                } 
#else
                catch (Exception ex)
                {
                    if (timeout.IsTimedOut)
                    {
                        ETLogger.WriteLine(LogType.Error,
                            string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                        timeout.ChangeTime(2000);
                    }
                }
#endif
            }
        }
        #endregion

        #region Добавление и изменение CustomRegion'ов
        /// <summary>
        /// Флаг, указывающий, что CustomRegion'ы в профиле были изменены, и его необходимо "сохранить"
        /// </summary>
        bool сurrentProfileNeedSave
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
        /// Запуск процедуры добавления CustomRegion'а
        /// </summary>
        private void handler_AddCustomRegion(object sender, ItemClickEventArgs e)
        {
            AddCustomRegionTool addCRTool = CurrentTool as AddCustomRegionTool;
            if (addCRTool is null)
                CurrentTool = new AddCustomRegionTool(btnCRTypeSelector.Checked);

            btnLockMapOnPlayer.Checked = false;

            barEditCustomRegion.Text = "Add CustomRegion";
            barEditCustomRegion.DockStyle = BarDockStyle.None;
            barEditCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 20, 
                Location.Y + MapPicture.Location.Y + 20 
                + (barMapping.Visible && barMapping.DockStyle == BarDockStyle.Top ? barMapping.Size.Height : 0)
                + (barMeshes.Visible && barMeshes.DockStyle == BarDockStyle.Top ? barMeshes.Size.Height : 0));
            //barEditCustomRegion.FloatSize = new Size(Math.Max(100, MapPicture.Width - 40), 0);
            editCRName.EditWidth = Math.Max(300, Width - 40);
            editCRSelector.EditWidth = Math.Max(300, Width - 40);

            btnCRTypeSelector.Visibility = BarItemVisibility.Always;
            editCRSelector.Visibility = BarItemVisibility.Never;
            editCRName.Visibility = BarItemVisibility.Always;
            btnCRRename.Visibility = BarItemVisibility.Never;
            btnCRAdditionAccept.Visibility = BarItemVisibility.Always;
            btnCREditionAccept.Visibility = BarItemVisibility.Never;

            barEditCustomRegion.Visible = true;
        }

        /// <summary>
        /// Выбор и редактирование существующего CustomRegion'а
        /// </summary>
        private void handler_EditCustomRegion(object sender, ItemClickEventArgs e)
        {
            var customRegions = Astral.Quester.API.CurrentProfile.CustomRegions;
            if (customRegions.Count == 0)
            {
                XtraMessageBox.Show("No CustomRegions available to change");
                btnEditCR.Checked = false;
                return;
            }

            EditCustomRegionTool editCRTool = CurrentTool as EditCustomRegionTool;
            if (btnEditCR.Checked)
            {
                btnLockMapOnPlayer.Checked = false;

                barEditCustomRegion.Text = "Edit CustomRegion";
                barEditCustomRegion.DockStyle = BarDockStyle.None;
                barEditCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 20,
                    Location.Y + MapPicture.Location.Y + 20
                    + (barMapping.Visible && barMapping.DockStyle == BarDockStyle.Top
                        ? barMapping.Size.Height
                        : 0)
                    + (barMeshes.Visible && barMeshes.DockStyle == BarDockStyle.Top
                        ? barMeshes.Size.Height
                        : 0));
                //barEditCustomRegion.FloatSize = new Size(Math.Max(100, MapPicture.Width - 40), 0);
                editCRName.EditWidth = Math.Max(300, Width - 40);
                editCRSelector.EditWidth = Math.Max(300, Width - 40);

                btnCRTypeSelector.Visibility = BarItemVisibility.Always;
                editCRSelector.Visibility = BarItemVisibility.Always;
                editCRName.Visibility = BarItemVisibility.Never;
                btnCRRename.Visibility = BarItemVisibility.Always;
                btnCRAdditionAccept.Visibility = BarItemVisibility.Never;
                btnCREditionAccept.Visibility = BarItemVisibility.Always;

                // Привязываем список CustomRegion'ов
                if (!ReferenceEquals(itemEditCRList.DataSource, customRegions))
                {
                    itemEditCRList.DataSource = customRegions;
                    itemEditCRList.DisplayMember = "Name";
                    editCRSelector.EditValue = customRegions.First();
                }
                else handler_ChangeSelectedCustomRegion(sender);

                barEditCustomRegion.Visible = true;
            }
            else if (editCRTool != null)
                CurrentTool = null;
        }

        /// <summary>
        /// Изменение модифицируемого CustomRegion'a
        /// </summary>
        private void handler_ChangeSelectedCustomRegion(object sender, EventArgs e = null)
        {
            var tool = CurrentTool;
            EditCustomRegionTool editCRTool = tool as EditCustomRegionTool;

            if (editCRSelector.EditValue is CustomRegion cr)
            {
                if (editCRTool is null)
                    CurrentTool = new EditCustomRegionTool(cr);
                else
                {
                    if (editCRTool.Modified
                        && XtraMessageBox.Show($"Changes of the '{editCRTool.Name}' can be lost!\n\r" +
                                               "Press 'Yes' to save changes or 'No' to proceed", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        editCRTool.Apply(Convert.ToString(editCRName.EditValue));
                        сurrentProfileNeedSave = true;
                        CurrentTool = new EditCustomRegionTool(cr);
                    }
                    else editCRTool.AttachTo(cr);
                }
                CenterOfMap = cr.Position;
                btnCRTypeSelector.Checked = cr.Eliptic;
            }
            else if (editCRTool != null)
                CurrentTool = null;
        }

        /// <summary>
        /// Завершение процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AcceptCRAddition(object sender, ItemClickEventArgs e)
        {
            if (CurrentTool is AddCustomRegionTool addCRtool)
            {
                if (!addCRtool.IsCorrect)
                {
                    XtraMessageBox.Show("Coordinates  of the CustomRegion is not valid!");
                    return;
                }

                string crName = editCRName.EditValue.ToString()?.Trim();
                if (string.IsNullOrEmpty(crName))
                    XtraMessageBox.Show("The Name of the CustomRegion is not valid !");
                else 
                {
                    var crList = Astral.Quester.API.CurrentProfile.CustomRegions;
                    if (crList.Count > 0 && (crList.Find(cr => cr.Name == crName) != null)
                        && XtraMessageBox.Show($"CustomRegion '{crName}' is already exist.\n\r" +
                                "Press 'Yes' to proceed or 'No' to change name", $"{crName} is exist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.OK)
                            return;
                    {
                        var cr = addCRtool.GetCustomRegion(crName);
                        if (cr != null)
                        {
                            crList.Add(cr);
                            сurrentProfileNeedSave = true;

                            //TODO: Добавить обновление списка CustomRegion'ов в Quester-редакторе

                            //_undoStack.Push(addCRtool); <- вызывается в CurrentTool.set

                            CurrentTool = null;
                            barEditCustomRegion.Visible = false;
                        }
                        else XtraMessageBox.Show($"Something wrong. '{crName}' was not added");
                    }
                }
            }
        }

        private void handler_AcceptCREdition(object sender, ItemClickEventArgs e)
        {
            if (CurrentTool is EditCustomRegionTool editCRTool && editCRTool.Modified)
            {
                editCRTool.Apply(Convert.ToString(editCRName.EditValue));
                сurrentProfileNeedSave = true;
                editCRName.EditValue = string.Empty;
                editCRName.Visibility = BarItemVisibility.Never;
                editCRSelector.Visibility = BarItemVisibility.Always;
            }
        }

        /// <summary>
        /// Прерывание процедуры добавления/редактирования региона
        /// </summary>
        private void handler_CancelCRManipulation(object sender, ItemClickEventArgs e)
        {
            var tool = CurrentTool;
            if (tool is AddCustomRegionTool addCRtool
                || tool is EditCustomRegionTool editCRtool)
                CurrentTool = null;
            barEditCustomRegion.Visible = false;
            btnEditCR.Checked = false;
        }

        /// <summary>
        /// Переключение типа CustomRegion'a
        /// </summary>
        private void handler_ChangeCustomRegionType(object sender, ItemClickEventArgs e)
        {
            if (btnCRTypeSelector.Checked)
                btnCRTypeSelector.ImageOptions.Image = Resources.miniCREllipce;
            else
                btnCRTypeSelector.ImageOptions.Image = Resources.miniCRRectang;

            var tool = CurrentTool;
            if (tool is AddCustomRegionTool addCRtool)
                addCRtool.IsElliptical = btnCRTypeSelector.Checked;
            else if(tool is EditCustomRegionTool editCRtool)
                editCRtool.IsElliptical = btnCRTypeSelector.Checked;
        }

        private void handler_ChangedRenameCRMode(object sender = null, ItemClickEventArgs e = null)
        {
            if (btnCRRename.Checked)
            {
                if (editCRSelector.EditValue is CustomRegion cr)
                    editCRName.EditValue = cr.Name;
                else editCRName.EditValue = string.Empty;
                editCRSelector.Visibility = BarItemVisibility.Never;
                editCRName.Visibility = BarItemVisibility.Always;
                editCRName.EditWidth = editCRSelector.EditWidth;
            }
            else
            {
                editCRName.EditValue = string.Empty;
                editCRSelector.Visibility = BarItemVisibility.Always;
                editCRName.Visibility = BarItemVisibility.Never;
            }
        }
        #endregion

        private void handler_DeleteRadiusChanged(object sender, EventArgs e)
        {
            API.CurrentSettings.DeleteNodeRadius = Convert.ToInt32(editDeleteRadius.EditValue);
        }
        private void handler_WaypointDistanceChanged(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(editWaypointDistance.EditValue);
            EntityTools.PluginSettings.Mapper.WaypointDistance = value;
            EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance = value / 2;
        }

        private void handler_ShowStatusBar(object sender, EventArgs e)
        {
            barStatus.Visible = true;
            btnShowStatBar.Visible = false;
        }

        private void handler_BarVisibleChanged(object sender, EventArgs e)
        {
            EntityTools.PluginSettings.Mapper.MapperForm.MappingBarVisible = barMapping.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.MeshesBarVisible = barMeshes.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.NodeToolsBarVisible = barNodeTools.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.CustomRegionBarVisible = barCustomRegions.Visible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barMeshes.Visible && !barNodeTools.Visible && !barCustomRegions.Visible;
        }

        #region Meshes_Manipulation
        /// <summary>
        /// Сохранение в файл текущего Quester-профиля
        /// </summary>
        private void handler_SaveChanges2QuesterProfile(object sender, ItemClickEventArgs e)
        {
            // Штатное сохранение профиля реализовано в
            // Astral.Quester.Core.Save(false)

            string meshName = EntityManager.LocalPlayer.MapState.MapName + ".bin";
            string profileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
            var currentProfile = Astral.Quester.API.CurrentProfile;
            bool useExternalMeshFile = currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= 10;
            string externalMeshFileName = useExternalMeshFile ? Path.Combine(Path.GetDirectoryName(profileName), currentProfile.ExternalMeshFileName) : string.Empty;
            Graph mesh = AstralAccessors.Quester.Core.Meshes;

#if true
            if (File.Exists(profileName))
            {
                ZipArchive zipFile = null;
                try
                {
                    bool profileUpdated = false;
                    if (сurrentProfileNeedSave)
                    {
                        // Открываем архивный файл профиля
                        zipFile = ZipFile.Open(profileName, File.Exists(profileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);
                        
                        // Сохраняем в архив файл профиля "profile.xml"
                        lock (currentProfile)
                        {
                            Patch_Astral_Quester_Core_Save.SaveProfile(zipFile);
                            currentProfile.Saved = true; 
                        }

                        сurrentProfileNeedSave = false;
                        profileUpdated = true;
                    }

                    // Сохраняем файлы мешей
                    if (useExternalMeshFile)
                    {
                        // закрываем архив профиля
                        zipFile?.Dispose();
                        
                        // открываем архив с внешними мешами
                        zipFile = ZipFile.Open(externalMeshFileName, 
                            File.Exists(externalMeshFileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);
                    } 
                    else if(zipFile is null)
                        zipFile = ZipFile.Open(profileName, File.Exists(profileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                    bool succeeded;
                    lock (mesh)
                    {
                        // удаляем мусор (скрытые вершины и ребра)
                        mesh.RemoveUnpassable();
                        // сохраняем меш в архивный файл
                        succeeded = Patch_Astral_Quester_Core_Save.SaveMesh(zipFile, meshName, mesh); 
                    }

                    if(succeeded)
                        XtraMessageBox.Show(string.Concat("The profile '", Path.GetFileName(profileName), "' updated:\n\r\t",
                                                        useExternalMeshFile ? externalMeshFileName +'\\' : string.Empty, meshName,
                                                        profileUpdated ? "\n\r\tprofile.xml" : string.Empty));
                    else XtraMessageBox.Show(string.Concat("Fail to save changes to the profile '", Path.GetFileName(profileName), '\''), "Failed !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception exc)
                {
                    Logger.WriteLine(exc.ToString());
                    XtraMessageBox.Show(exc.ToString());
                }
                finally
                {
                    zipFile?.Dispose();
                }
            }
#else
            if (Astral.Quester.API.CurrentProfile.UseExternalMeshFile)
            {
                // Профиль использует "внешние" меши
                XtraMessageBox.Show($"The '{nameof(MapperFormExt)}' doesn't support external Meshes");
            }
            else
            {
                // Сохранение графа путей (карты) в файл текущего профиля
                if (File.Exists(profileName))
                {
                    try
                    {
                        // удаляем мусор (скрытые вершины)
                        AstralAccessors.Quester.Core.Meshes.Value.RemoveUnpassable();

                        bool profileUpdated = false;
                        using (ZipArchive zipFile = ZipFile.Open(fileName, ZipArchiveMode.Update))
                        {
                            ZipArchiveEntry zipMeshEntry = zipFile.GetEntry(meshName);

#if false
                            // Проверяем наличие меша карты в zip-файле
                            foreach (ZipArchiveEntry entry in zipFile.Entries)
                            {
                                if (entry.Name.Equals(meshName, StringComparison.OrdinalIgnoreCase))
                                {
                                    zipMeshEntry = entry;
                                    break;
                                }
                            } 
#endif

                            if (zipMeshEntry != null)
                                // Меш карты найден. Удаляем старый и создаем новый
                                zipMeshEntry.Delete();
                            zipMeshEntry = zipFile.CreateEntry(meshName);

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
                            if (CurrentProfileNeedSave)
                            {
                                ZipArchiveEntry zipProfileEntry = zipFile.GetEntry("profile.xml");

                                if (zipProfileEntry is null)
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
                                    profileUpdated = true;
                                }

                                CurrentProfileNeedSave = false;
                            }
                        }

                        XtraMessageBox.Show(string.Concat("The profile '", Path.GetFileName(fileName), "' updated:\n\r\t",
                                                            meshName,
                                                            profileUpdated ? "profile.xml" : string.Empty));
                    }
                    catch (Exception exc)
                    {
                        Logger.WriteLine(exc.ToString());
                        XtraMessageBox.Show(exc.ToString());
                    }
                }
            } 
#endif
        }

        /// <summary>
        /// Импорт мешей карты (графа путей) из Игры
        /// </summary>
        private void handler_ImportCurrentMapMeshesFromGame(object sender, ItemClickEventArgs e)
        {
            //if (((Graph)CoreCurrentMapMeshes)?.NodesCollection.Count == 0
            if (((Graph)AstralAccessors.Quester.Core.Meshes)?.Nodes.Count == 0
                || XtraMessageBox.Show(this, "Are you sure to import game nodes ? All actual nodes must be delete !", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_Mapping_Stop();
#if AstralMapper
                MapperStopDrawing?.Invoke(mapper); 
#endif
                //Graph graph = (Graph)CoreCurrentMapMeshes;
                Graph graph = AstralAccessors.Quester.Core.Meshes;
                lock (graph)
                {
                    graph.Clear();
                    GoldenPath.GetCurrentMapGraph(graph);
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
                Filter = @"Astral mission profil (*.amp.zip)|*.amp.zip|(*.mesh.zip)|*.mesh.zip"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
            {
                handler_Mapping_Stop();
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
                                    ResetToolState();

                                    lock (mapsMeshes)
                                    {
                                        if (mapsMeshes.ContainsKey(currentMapName))
                                            mapsMeshes[currentMapName] = currentMapMeshes;
                                        else mapsMeshes.Add(currentMapName, currentMapMeshes);
                                    }
                                    Logger.WriteLine($"Import '{currentMapMeshesName}' from {openFileDialog.FileName}");
                                    return;
                                }

                                string msg = $"Failed to import '{currentMapMeshesName}' from {openFileDialog.FileName}";
                                Logger.WriteLine(msg);
                                XtraMessageBox.Show(msg);
                                return;
                            }

                        }
                        if (dialogResult == DialogResult.No)
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
                                    Graph entryMapMeshes;
                                    using (Stream stream = entry.Open())
                                    {
                                        entryMapMeshes = binaryFormatter.Deserialize(stream) as Graph;
                                    }
                                    if (entryMapMeshes != null)
                                    {
                                        ResetToolState();

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
        private void handler_ClearCurrentMapMeshes(object sender, ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show(this, "Are you sure to delete all map nodes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_Mapping_Stop();
                ResetToolState();
#if AstralMapper
                MapperStopDrawing?.Invoke(mapper); 
#endif
                _currentMapHash = 0;
                ((Graph)AstralAccessors.Quester.Core.Meshes).Clear();
#if AstralMapper
                MapperStartDrawing?.Invoke(mapper); 
#endif
            }
        }

        /// <summary>
        /// Оценка графа на наличие мусора
        /// </summary>
        private void handler_MeshesInfo(object sender, ItemClickEventArgs e)
        {
            int correctNodeNum = 0;
            int unpasNodeNum = 0;
            var nodes = ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
            int correctArcNum = 0;
            int disabledArcNum = 0;
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
                    if (arc.Disabled)
                    {
                        disabledArcNum++;
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
                $"\tdisabled:\t{disabledArcNum}\n\r" +
                $"\tunpassable:\t{unpasArcNum}\n\r");
        }

        /// <summary>
        /// Сжатие графа (удаление мусора)
        /// </summary>
        private void handler_MeshesCompression(object sender, ItemClickEventArgs e)
        {
            ResetToolState();

            int correctNodeNum = 0;
            int unpasNodeNum = 0;
            var nodes = ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes;
            int totalNodeNum = nodes.Count;
            int correctArcNum = 0;
            int disabledArcNum = 0;
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
                    if (arc.Disabled)
                    {
                        disabledArcNum++;
                        if (!arc.Passable)
                            unpasArcNum++;
                    }
                    else if (!arc.Passable)
                        unpasArcNum++;
                    else correctArcNum++;
                }
            }

            if (unpasNodeNum > 0 && ((Graph)AstralAccessors.Quester.Core.Meshes).RemoveUnpassable() > 0)
            {
                ResetToolState();

                int correctNodeNumNew = 0;
                int unpasNodeNumNew = 0;
                int correctArcNumNew = 0;
                int disabledArcNumNew = 0;
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
                        if (arc.Disabled)
                        {
                            disabledArcNumNew++;
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
                    $"\tdisabled:\t{disabledArcNum} => {disabledArcNumNew}\n\r" +
                    $"\tunpassable:\t{unpasArcNum} => {unpasArcNumNew}\n\r");
            }
            else XtraMessageBox.Show("Meshes doesn't compressed\n\r" +
                                    $"Total nodes: {nodes.Count}\n\r" +
                                    $"\tcorrect:\t{correctNodeNum}\n\r" +
                                    $"\tunpassable:\t{unpasNodeNum}\n\r" +
                                    $"Total arcs: {totalArcNum}\n\r" +
                                    $"\tcorrect:\t{correctArcNum}\n\r" +
                                    $"\tdisabled:\t{disabledArcNum}\n\r" +
                                    $"\tunpassable:\t{unpasArcNum}\n\r");
        }
        
        #region EditMeshes
        private void handler_EditEdges_ModeChanged(object sender, ItemClickEventArgs e)
        {
            EditEdgeTool editEdgeTool = CurrentTool as EditEdgeTool;
            if (btnEditEdges.Checked)
            {
                if (editEdgeTool is null)
                    CurrentTool = new EditEdgeTool();
                LockOnPlayer = false;
            }
            else if (editEdgeTool != null)
            {
                CurrentTool = null;
            }
        }

        private void handler_RelocateNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
            RelocateNodesTool relocateTool = CurrentTool as RelocateNodesTool;
            if (btnMoveNodes.Checked)
            {
                if (relocateTool is null)
                    CurrentTool = new RelocateNodesTool();
                LockOnPlayer = false;
            }
            else if (relocateTool != null)
                CurrentTool = null; 
        }

        private void handler_DeleteNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
            RemoveNodesTool removeTool = CurrentTool as RemoveNodesTool;
            if (btnRemoveNodes.Checked)
            {
                if (removeTool is null)
                    CurrentTool = new RemoveNodesTool();
                LockOnPlayer = false;
            }
            else if (removeTool != null)
                    CurrentTool = null;
        }
        #endregion
        #endregion

        /// <summary>
        /// Откат изменений
        /// </summary>
        private void handler_Undo(object sender, ItemClickEventArgs e)
        {
            // TODO: добавить перемещение карты к координатам, в которых было совершено изменение
            // TODO: Добавить hint к кнопке отката, указывающий на изменения, которые будут отменены
            IMapperTool tool = CurrentTool;
            if (tool != null
                && tool.Applied)
            {
                tool.Undo();
                CurrentTool = null;
                return;
            }
            CurrentTool = null;

            if (_undoStack.Count > 0)
            {
                tool = _undoStack.Pop();
                if(tool.Applied)
                    tool.Undo();
            }
        }

        #region Mapping
        private readonly MappingTool _mappingTool;

        private IGraph GetMappingGraph()
        {
            if (_graphics.CenterPosition.Distance2DFromPlayer < EntityTools.PluginSettings.Mapper.CacheRadius * 0.75)
                return _graphics.VisibleGraph;
            return AstralAccessors.Quester.Core.Meshes.Value;
        }

        private void handler_Mapping_BidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingBidirectional.Checked)
                _mappingTool.MappingMode = MappingMode.Bidirectional;
        }

        private void handler_Mapping_UnidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingUnidirectional.Checked)
                _mappingTool.MappingMode = MappingMode.Unidirectional;
        }

        private void handler_Mapping_Stop(object sender = null, ItemClickEventArgs e = null)
        {
            if(btnMappingStop.Checked)
                _mappingTool.MappingMode = MappingMode.Stoped;
        }

        private void handler_Mapping_ForceLink(object sender, ItemClickEventArgs e)
        {
            _mappingTool.ForceLink = btnMappingForceLink.Checked;
        }

        private void handler_Mapping_LinearPath(object sender, ItemClickEventArgs e)
        {
            _mappingTool.Linear = btnMappingLinearPath.Checked;
        }
        #endregion
    }
#endif
}
