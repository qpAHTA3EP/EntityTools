//#define DrawMapper_Measuring

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AStar;
using Astral;
using Astral.Controllers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using EntityTools.Patches.Mapper.Tools;
using EntityTools.Properties;
using MyNW.Internals;
using MyNW.Patchables.Enums;
using Action = System.Action;
using GoldenPath = Astral.Logic.NW.GoldenPath;
using MinimapWaypoint = MyNW.Classes.MinimapWaypoint;
using Timeout = Astral.Classes.Timeout;
using ACTP0Tools;
#if PATCH_ASTRAL
using ACTP0Tools.Classes;
using ACTP0Tools.Classes.Quester;
using Astral.Logic.NW;
using Astral.Quester.Classes;
using EntityTools.Forms;
using EntityTools.Quester.Editor.Classes;
using MyNW.Classes;

#endif

namespace EntityTools.Patches.Mapper
{
    //TODO Подружить с ролью Professions
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm
    {
        private MapperSettingsForm settingsForm;

        /// <summary>
        /// Флаг удержания персонажа в центре карты
        /// </summary>
        internal bool LockOnPlayer
        {
            get => btnLockMapOnPlayer.Checked;
            set => btnLockMapOnPlayer.Checked = value;
        }

        /// <summary>
        /// Координаты центра отображаемой карты
        /// </summary>
        internal Vector3 CenterOfMap
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
                if (value == null || !value.IsValid) return;

                using (_graphics.WriteLock())
                {
                    _graphics.CenterPosition = value.Clone();
                }
                LockOnPlayer = false;
            }
        }

        /// <summary>
        /// Специальный объект
        /// </summary>
        public Vector3 SpecialObject { get; set; }

        /// <summary>
        /// Координаты курсора мыши, относительно формы <see cref="MapperFormExt"/>
        /// </summary>
        protected Point RelativeMousePosition => MapPicture.PointToClient(MousePosition);

        // BUG После загрузки нового профиля в Quester'е, продолжает отображаться старый профиль
        public BaseQuesterProfileProxy Profile
        {
            get => _profile;
        }
        private readonly BaseQuesterProfileProxy _profile;

        #region Инициализация формы
        IGraph GetGraph() => _profile.CurrentMesh;

        internal MapperFormExt()
        {
            InitializeComponent();

            _profile = AstralAccessors.Quester.Core.CurrentProfile;

            MouseWheel += handler_MouseWheel;

            var mapperConfig = EntityTools.Config.Mapper;
            var mapperFormConfig = mapperConfig.MapperForm;
            barMapping.Visible = mapperFormConfig.MappingBarVisible;
            barGraphTools.Visible = mapperFormConfig.MeshesBarVisible;
            barGraphEditTools.Visible = mapperFormConfig.NodeToolsBarVisible;
            barCustomRegions.Visible = mapperFormConfig.CustomRegionBarVisible;
            barStatus.Visible = mapperFormConfig.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible 
                                  && !barMapping.Visible 
                                  && !barGraphTools.Visible 
                                  && !barGraphEditTools.Visible 
                                  && !barCustomRegions.Visible;

            Location = mapperFormConfig.Location;

            BindingControls();

            _mappingTool = new MappingTool(GetGraph) {
                Linear = mapperConfig.LinearPath,
                ForceLink = mapperConfig.ForceLinkingWaypoint
            };

            _graphics = new MapperGraphics(360, 360);

            _graphCache = new MapperGraphCache(GetGraph, mapperConfig.CacheActive) {
                CacheDistanceZ = mapperFormConfig.LayerDepth
            };

#if DrawMapper_Measuring
            lblPlayerPos.Visibility = BarItemVisibility.Always;
            lblDrawInfo.Visibility = BarItemVisibility.Always;
            lblMousePos.Visibility = BarItemVisibility.Always;
#else
            lblDrawInfo.Visibility = BarItemVisibility.Never;
            lblMousePos.Visibility = BarItemVisibility.Never;
#endif
            lblPlayerPos.Visibility = BarItemVisibility.Always;
        }

        internal MapperFormExt(BaseQuesterProfileProxy profile)
        {
            if (profile is null)
                throw new ArgumentException(nameof(profile));

            InitializeComponent();

            _profile = profile;

            MouseWheel += handler_MouseWheel;

            var mapperConfig = EntityTools.Config.Mapper;
            var mapperFormConfig = mapperConfig.MapperForm;
            barMapping.Visible = mapperFormConfig.MappingBarVisible;
            barGraphTools.Visible = mapperFormConfig.MeshesBarVisible;
            barGraphEditTools.Visible = mapperFormConfig.NodeToolsBarVisible;
            barCustomRegions.Visible = mapperFormConfig.CustomRegionBarVisible;
            barStatus.Visible = mapperFormConfig.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible
                                  && !barMapping.Visible
                                  && !barGraphTools.Visible
                                  && !barGraphEditTools.Visible
                                  && !barCustomRegions.Visible;
            Location = mapperFormConfig.Location;

            BindingControls();


            _mappingTool = new MappingTool(GetGraph) {
                Linear = mapperConfig.LinearPath,
                ForceLink = mapperConfig.ForceLinkingWaypoint
            };

            _graphics = new MapperGraphics(360, 360);

            _graphCache = new MapperGraphCache(GetGraph, mapperConfig.CacheActive) {
                CacheDistanceZ = mapperFormConfig.LayerDepth
            };

#if DrawMapper_Measuring
            lblPlayerPos.Visibility = BarItemVisibility.Always;
            lblDrawInfo.Visibility = BarItemVisibility.Always;
            lblMousePos.Visibility = BarItemVisibility.Always;
#else
            lblDrawInfo.Visibility = BarItemVisibility.Never;
            lblMousePos.Visibility = BarItemVisibility.Never;
#endif
            lblPlayerPos.Visibility = BarItemVisibility.Always;
        }

        /// <summary>
        /// Привязка элементов управления к данным
        /// </summary>
        private void BindingControls()
        {
            editWaypointDistance.DataBindings.Add(nameof(editWaypointDistance.EditValue),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.WaypointDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
            editWaypointDistance.Edit.EditValueChanged += handler_WaypointDistanceChanged;
            editWaypointDistance.Edit.Leave += handler_WaypointDistanceChanged;

            editMaxZDifference.DataBindings.Add(nameof(editMaxZDifference.EditValue),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.MaxElevationDifference),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            editEquivalenceDistance.DataBindings.Add(nameof(editEquivalenceDistance.EditValue),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.WaypointEquivalenceDistance),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            btnMappingForceLink.DataBindings.Add(nameof(btnMappingForceLink.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            btnMappingLinearPath.DataBindings.Add(nameof(btnMappingLinearPath.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.LinearPath),
                                                false, DataSourceUpdateMode.OnPropertyChanged);
#if false
            // Настройки панели инструментов 
            // Привязка к элементам управления вызывает ошибку времени выполнения

            Location.DataBindings.Add(nameof(Location),
                            EntityTools.Config.Mapper.MapperForm,
                            nameof(EntityTools.Config.Mapper.MapperForm.Location),
                            false, DataSourceUpdateMode.OnPropertyChanged);

            barMapping.DataBindings.Add(nameof(barMapping.Visible),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.MappingBarVisible),
                                    false, DataSourceUpdateMode.OnPropertyChanged);
            barStatus.DataBindings.Add(nameof(barStatus.Visible),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.StatusBarVisible),
                                    false, DataSourceUpdateMode.OnPropertyChanged);
#endif
        }

        /// <summary>
        /// Событие при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FormLoad(object sender, EventArgs e)
        {
            Binds.RemoveShiftAction(Keys.M);

            barMapping.Visible = EntityTools.Config.Mapper.MapperForm.MappingBarVisible;
            barGraphTools.Visible = EntityTools.Config.Mapper.MapperForm.MeshesBarVisible;
            barGraphEditTools.Visible = EntityTools.Config.Mapper.MapperForm.NodeToolsBarVisible;
            barCustomRegions.Visible = EntityTools.Config.Mapper.MapperForm.CustomRegionBarVisible;
            barStatus.Visible = EntityTools.Config.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barGraphTools.Visible && !barGraphEditTools.Visible && !barCustomRegions.Visible;
            Location = EntityTools.Config.Mapper.MapperForm.Location;
            Size = EntityTools.Config.Mapper.MapperForm.Size;
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

            var mapperFormConfig = EntityTools.Config.Mapper.MapperForm;
            mapperFormConfig.MappingBarVisible = barMapping.Visible;
            mapperFormConfig.MeshesBarVisible = barGraphTools.Visible;
            mapperFormConfig.NodeToolsBarVisible = barGraphEditTools.Visible;
            mapperFormConfig.CustomRegionBarVisible = barCustomRegions.Visible;
            mapperFormConfig.StatusBarVisible = barStatus.Visible;

            mapperFormConfig.Location = Location;
            if(WindowState == FormWindowState.Normal)
                mapperFormConfig.Size = Size;

            InterruptAllModifications();

            OnDraw = null;

            Binds.RemoveShiftAction(Keys.M);
        }

        /// <summary>
        /// Фоновый процесс обновления окна Mapper'a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void work_MapperFormUpdate(object sender, DoWorkEventArgs e)
        {
#if DrawMapper_Measuring
            string formCaption = string.Empty;
            string zoomStr = string.Empty;
            string playerPosStr = string.Empty;
            string statusStr = string.Empty;
            Image img = null;

            //TODO Заменить Environment.TickCount на DateTime.Now.Ticks 
            Timeout timeout = new Timeout(0);
            const int speedMeasuresNum = 10;
            const int mapperMeasuresNum = 10;
            Stopwatch sw = new Stopwatch();

            long[] drawMapperMeasures = new long[mapperMeasuresNum];

            Tuple<Vector3, int, double>[] speedMeasures = new Tuple<Vector3, int, double>[speedMeasuresNum];
            Vector3 lastPlayerPos = null;
            int lastTickCount = Environment.TickCount;
            int movingTime = 0;
            double pathDistance = 0;

            int currentMesure = 0;

            const int time = 5000;
            double frames = 0;
            double fps = 0, cps = 0;

            int cacheVer = 0;
            try
            {
                while (!IsDisposed
                        && !backgroundWorker.CancellationPending)
                {
                    var player = EntityManager.LocalPlayer;
                    bool isLoading = player.IsLoading || !player.MapState.IsValid;
                    string mousePosStr; 
                    if (isLoading)
                    {
                        _currentMapHash = 0;
                        mousePosStr = "-";
                        playerPosStr = "Loading";
                        statusStr = "-"; 
                        movingTime = 0;
                        pathDistance = 0;
                        lastPlayerPos = null; 
                    }
                    else
                    {
                        var playerPos = player.Location.Clone();
                        Vector3 pos = LockOnPlayer ? playerPos : _graphics.CenterPosition;

                        // Вычисляем скорость перемещения;
                        int currMeasureInd = currentMesure % speedMeasuresNum;
                        int prevMeasureInd = currMeasureInd == 0 ? speedMeasuresNum - 1 : currMeasureInd - 1;
                        var prevMeasure = speedMeasures[prevMeasureInd];
                        if (prevMeasure != null)
                        {
                            int tickCount = Environment.TickCount;
                            int ticks = tickCount - prevMeasure.Item2;
                            double distance = prevMeasure.Item1.Distance3DFromPlayer;
                            speedMeasures[currMeasureInd] = Tuple.Create(playerPos, tickCount, ticks > 0 ? distance / ticks * 1000d : 0d);
                        }
                        else
                        {
                            speedMeasures[currMeasureInd] = Tuple.Create(playerPos, Environment.TickCount, 0d);
                        }
                        double speed = 0;
                        int num = 0;
                        for (currMeasureInd = 0; currMeasureInd < speedMeasuresNum; currMeasureInd++)
                        {
                            var measure = speedMeasures[currMeasureInd];
                            if (measure != null && measure.Item3 > 0)
                            {
                                speed += measure.Item3;
                                num++;
                            }
                        }
                        if (num > 0)
                            speed /= num;
#if false               // Вывод в лог скорости и статистики перемещения
                        if (timeout.IsTimedOut)
                        {
                            ETLogger.WriteLine(LogType.Debug, $"{speed1:N3} ({num}) | {speed2:N3} ({totalDistance:N2}/{totalTicks/1000d:N2})");
                            foreach(var slot in speedMeasures)
                            {
                                if (slot != null)
                                {
                                    var vec = slot.Item1;
                                    ETLogger.WriteLine(LogType.Debug,
                                        string.Concat(vec is null ? "\tNULL\t" : $"{{ {vec.X:N1}, {vec.Y:N1}, {vec.Z:N1} }};\t", slot.Item2.ToString("N0"), ";\t",
                                        slot.Item3.ToString("N0"), ";\t", slot.Item4.ToString("N2"), ";\t", slot.Item5.ToString("N3")));
                                }
                                else ETLogger.WriteLine(LogType.Debug, "\tNULL");
                            }
                        } 
#endif

#if false               // Вычисляем скорость перемещения 2м способом
                        distance = lastPlayerPos?.Distance3DFromPlayer ?? 0;
                        int tickCount = ticks - lastTickCount;
                        if (distance > 0)
                        {
                            movingTime += tickCount;
                            pathDistance += distance;
                        }
                        lastPlayerPos = playerPos;
                        lastTickCount = ticks;
                        double speed2 = movingTime > 0 ? pathDistance / movingTime * 1000d : 0d; 
#endif
#if false
                        playerPosStr = $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1} || ";  
#elif true
                        playerPosStr = string.Concat(pos.X.ToString("N1"), " | ", pos.Y.ToString("N1"), " | ", pos.Z.ToString("N1"), " || ", speed.ToString("N3"), "f/s (", num, ")");
#endif

                        int hash = _profile.CurrentMesh?.GetHashCode() ?? 0;
                        if (_currentMapHash != hash)
                        {
                            var currentMapInfo = player.CurrentZoneMapInfo;
                            formCaption = string.Concat(currentMapInfo.DisplayName, '[', currentMapInfo.MapName, ']');
                            _currentMapHash = hash;

                            // Карта изменилась - сбрасываем состояние инструментов
                            ResetToolState();
                        }

                        sw.Restart();
                        DrawMapper();

                        sw.Stop(); 
                        using (_graphics.ReadLock())
                            img = _graphics.getImage();
                        drawMapperMeasures[currentMesure % mapperMeasuresNum] = sw.ElapsedMilliseconds;

                        currentMesure++;
                        frames++;
                        if (timeout.IsTimedOut)
                        {
                            int curCacheVer = _graphics.GraphCache.Version;

                            cps = (curCacheVer - cacheVer) / (double)time * 1000d;
                            fps = frames / time * 1000d;
                            frames = 0;
                            timeout.ChangeTime(time);

                            cacheVer = curCacheVer;
                        }
                        statusStr = $"{fps:N1} fps | {drawMapperMeasures.Sum() / 10d:N1} ms | {cps:N1} cps"; 

                        _graphics.GetWorldPosition(RelativeMousePosition, out double mouseX, out double mouseY);
                        mousePosStr = string.Concat(mouseX.ToString("N1"), " | ", mouseY.ToString("N1"));

                        zoomStr = string.Concat(Zoom * 100, '%');
                    }

                    if (InvokeRequired)
                    {
                        var captionText = formCaption;
                        var mousePosText = mousePosStr; 
                        var zoomText = zoomStr;
                        var mapImg = img;
                        Invoke(new Action(() =>
                        {
                            Text = captionText;
                            lblMousePos.Caption = mousePosText;
                            lblPlayerPos.Caption = playerPosStr;
                            lblDrawInfo.Caption = statusStr;  
                            lblZoom.Caption = zoomText;
                            MapPicture.Image = mapImg;
                        }));
                    }
                    else
                    {
                        Text = formCaption;
                        lblMousePos.Caption = mousePosStr; 
                        lblZoom.Caption = zoomStr;
                        MapPicture.Image = img;
                    }

                    Thread.Sleep(EntityTools.Config.Mapper.MapperForm.RedrawMapperTimeout);
                }
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, "MapperFormUpdate catch an exception: " + exc);
                if (InvokeRequired)
                    Invoke(new Action(() => Text = exc.Message));
                else
                {
                    Text = exc.Message;
                }
                throw;
            } 
#else
            try
            {
                string captionMapOnlyPattern;
                string captionMapAndRegionPattern;
                if (Profile is ProfileProxy)
                {
                    captionMapOnlyPattern = "<{0}> : {1}";
                    captionMapAndRegionPattern = "<{0}> : {1}[{2}]";
                }
                else
                {
                    captionMapOnlyPattern = "{0} : {1}";
                    captionMapAndRegionPattern = "{0} : {1}[{2}]";
                }
                while (!IsDisposed
                        && !backgroundWorker.CancellationPending)
                {
                    Image image = null;
                    var player = EntityManager.LocalPlayer;
                    string formCaption;
                    string playerPosStr;
                    string zoomStr = $"{Zoom * 100}%";
                    if (player.IsLoading || !player.MapState.IsValid)
                    {
                        formCaption = string.Empty;
                        playerPosStr = string.Empty;
                    }
                    else
                    {
                        image = DrawMapper();
                        var regionName = player.RegionInternalName;
                        var profileName = Profile.ProfilePath;
                        if (!string.IsNullOrEmpty(profileName))
                        {
                            profileName = Path.GetFileName(profileName);
                            formCaption = string.IsNullOrEmpty(regionName)
                                ? string.Format(captionMapOnlyPattern, profileName, player.MapState.MapName)
                                : string.Format(captionMapAndRegionPattern, profileName, player.MapState.MapName, regionName);
                        }
                        else
                        {
                            formCaption = string.IsNullOrEmpty(regionName)
                                ? player.MapState.MapName
                                : $"{player.MapState.MapName}[{regionName}]";
                        }

                        var pos = LockOnPlayer ? player.Location : _graphics.CenterPosition;
                        playerPosStr = $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1}";
                    }

                    void UpdateFormText()
                    {
                        Text = formCaption;
                        lblZoom.Caption = zoomStr;
                        lblPlayerPos.Caption = playerPosStr;
                        MapPicture.Image = image;
                    }

                    if (InvokeRequired)
                    {
                        Invoke(new Action(UpdateFormText));
                    }
                    else UpdateFormText();

                    Thread.Sleep(EntityTools.Config.Mapper.MapperForm.RedrawMapperTimeout);
                }
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, "MapperFormUpdate catch an exception: " + exc);
                var text = exc.Message;
                if (InvokeRequired)
                    Invoke(new Action(() => Text = text));
                else Text = text;
                throw;
            }
#endif
        }
        #endregion



        #region Переадресация событий инструментам
        /// <summary>
        /// Обработчик события MouseClick - Уведомление активного инструмента о нажатии кнопки мыши
        /// </summary>
        private void handler_MouseClick(object sender, MouseEventArgs e)
        {
            var tool = CurrentTool;
            if (tool != null)
            {
                double x, y;
                IMapperTool undo = null;
                if (tool.AllowNodeSelection)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    MapperMouseEventArgs me = new MapperMouseEventArgs(e.Button, e.Clicks, x, y);

                    var graph = _graphCache;
                    using (graph.ReadLock())
                    {
                        using (_selectedNodes.WriteLock())
                        {
                            _selectedNodes.OnMouseClick(graph, me);
                            if (tool.HandleMouseClick)
                                tool.OnMouseClick(graph, _selectedNodes, me, out undo);
                        }
                    }
                }
                else if (tool.HandleMouseClick)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    MapperMouseEventArgs me = new MapperMouseEventArgs(e.Button, e.Clicks, x, y);

                    var graph = _graphCache;
                    using (graph.WriteLock())
                        tool.OnMouseClick(graph, null, me, out undo);
                }
                if (undo != null)
                    _undoStack.Push(undo);
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
                double x, y;
                IMapperTool undo = null;
                if (tool.AllowNodeSelection)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    var graph = _graphCache;
                    using (graph.ReadLock())
                    {
                        using (_selectedNodes.WriteLock())
                        {
                            _selectedNodes.OnKeyUp(graph, e);
                            if (tool.HandleMouseClick)
                                tool.OnKeyUp(graph, _selectedNodes, e, x, y, out undo);
                        }
                    }
                }
                else if (tool.HandleMouseClick)
                {
                    using (_graphics.ReadLock())
                        _graphics.GetWorldPosition(RelativeMousePosition, out x, out y);

                    var graph = _graphCache;
                    using (graph.WriteLock())
                        tool.OnKeyUp(graph, _selectedNodes, e, x, y, out undo);
                }
                if (undo != null)
                    _undoStack.Push(undo);
            }
        } 
        #endregion



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
        //private int _currentMapHash;
        
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
                    if(ReferenceEquals(_currentTool, value))
                        return;
                    
                    if (_currentTool.Applied)
                        _undoStack.Push(_currentTool);
                }
                _currentTool = value;
                if (value != null)
                    InterruptAllModifications(value.EditMode);
                else
                {
                    _selectedNodes.Clear();
                    MapperEditMode mode = _mappingTool.MappingMode != MappingMode.Stopped 
                        ? MapperEditMode.Mapping 
                        : MapperEditMode.None;
                    InterruptAllModifications(mode); 
                }
            }
        }
        private IMapperTool _currentTool;

        private void ResetToolState()
        {
            _currentTool = null;
            using (_selectedNodes.WriteLock())
                _selectedNodes.Clear();
            _undoStack.Clear();
            InterruptAllModifications();
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
                    _graphics.MoveCenterPosition(dx, dy, 0);
                mouseClickPosition.X = e.X;
                mouseClickPosition.Y = e.Y;
            }
        }

        private void handler_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClickPosition.X = 0;
            mouseClickPosition.Y = 0;
        }

        private void handler_LockOnSpecialObject(object sender, ItemClickEventArgs e)
        {
            if (SpecialObject?.IsValid == true)
                CenterOfMap = SpecialObject;
        }
        #endregion



        /// <summary>
        /// Прерывание всех операций по изменению графа путей (мешей)
        /// </summary>
        private void InterruptAllModifications(MapperEditMode mode = MapperEditMode.None)
        {
            if (mode != MapperEditMode.Mapping && _mappingTool?.MappingMode == MappingMode.Stopped)
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
            if (mode != MapperEditMode.AddNode)
            {
                btnAddNodes.Checked = false;
            }
            if (mode != MapperEditMode.DistanceMeasurement)
            {
                btnDistanceMeasurement.Checked = false;
            }
            if (mode != MapperEditMode.Information)
            {
                btnObjectInfo.Checked = false;
            }
        }



        #region Управление масштабом

        /// <summary>
        /// Коэффициент масштабирования
        /// </summary>
        public double Zoom
        {
            get => _zoom;
            private set
            {
                _zoom = value;
                _graphCache.CacheDistanceX = _graphCache.CacheDistanceX / 1.8d / value;
                _graphCache.CacheDistanceY = _graphCache.CacheDistanceY / 1.8d / value;
            }
        }
        private double _zoom = 1.0;

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
            lblZoom.Caption = string.Concat(Zoom * 100d, '%');
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
            lblZoom.Caption = string.Concat(Zoom * 100d, '%');
        }

        private void handler_DoubleClickZoom(object sender, ItemClickEventArgs e)
        {
            Zoom = 2.5;
        }

        private readonly Timeout mouseWeelTimeout = new Timeout(0);

        private void handler_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mouseWeelTimeout.IsTimedOut)
            {
                if (LockOnPlayer)
                {
                    if (e.Delta > 0)
                        handler_ZoomIn(sender);
                    else if (e.Delta < 0)
                        handler_ZoomOut(sender);
                }
                else
                {
                    if (ModifierKeys == Keys.Alt)
                    {
                        if (e.Delta > 0)
                        {
                            _graphics.MoveCenterPosition(0, 0, 50);
                            _graphCache.MoveCenterPosition(0, 0, -50);
                        }
                        else
                        {
                            _graphics.MoveCenterPosition(0, 0, -50);
                            _graphCache.MoveCenterPosition(0,0, -50);
                        }
                    }
                    else
                    {
                        if (e.Delta > 0)
                            handler_ZoomIn(sender);
                        else if (e.Delta < 0)
                            handler_ZoomOut(sender);
                    }
                }

                mouseWeelTimeout.ChangeTime(100);
            }
        }
        #endregion



        #region Drawings
        private readonly MapperGraphics _graphics;
        private readonly MapperGraphCache _graphCache;

        public delegate void CustomDrawEvent(MapperGraphics graphics);

        /// <summary>
        /// Событие, позволяющее отобразить собственные 
        /// </summary>
        public event CustomDrawEvent OnDraw;


        public double CacheDistanceZ
        {
            get => _graphCache.CacheDistanceZ;
            set => _graphCache.CacheDistanceZ = value;
        }

        /// <summary>
        /// Метод для фоновой отрисовки карты
        /// </summary>
        private Image DrawMapper()
        {
            if (!IsDisposed && Visible && MapPicture.Visible && MapPicture.Width > 0 && MapPicture.Height > 0)
            {
                var errorCounter = 0;
                var errorNotifyTimeout = new Timeout(0);
                try
                {
                    using (_graphics.WriteLock())
                    {
                        int imgWidth = MapPicture.Width;
                        int imgHeight = MapPicture.Height;

                        (double leftBorder,
                            double topBorder,
                            double rightBorder,
                            double downBorder) = AdjustImageAndCacheArea(imgWidth, imgHeight);

                        DrawMapMeshes();
                        DrawCustomRegions(topBorder, downBorder, leftBorder, rightBorder);
                        DrawTargetableNodes(leftBorder, rightBorder, downBorder, topBorder);
                        DrawCampfires(leftBorder, rightBorder, downBorder, topBorder);
                        DrawSpecialObject(leftBorder, rightBorder, downBorder, topBorder);
                        //DrawRoleGraphics();
                        //DrawAOE(leftBorder, rightBorder, downBorder, topBorder);
                        //DrawDodgeArea(leftBorder, rightBorder, downBorder, topBorder);
                        OnDraw?.Invoke(_graphics);
                        DrawEnemies(leftBorder, rightBorder, downBorder, topBorder);
                        DrawMapperTools(topBorder, downBorder, leftBorder, rightBorder);
                        DrawDestination(leftBorder, rightBorder, downBorder, topBorder);
                        DrawPlayer(leftBorder, rightBorder, downBorder, topBorder);

                        return _graphics.getImage();
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\n" + ex);
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\n" + ex);
                    throw;
                }
                catch (ThreadAbortException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\n" + ex);
                    throw;
                }
                catch (Exception ex)
                {
                    errorCounter++;
                    if (errorNotifyTimeout.IsTimedOut)
                    {
                        ETLogger.WriteLine(LogType.Error,
                            $"{nameof(DrawMapper)} catch {errorCounter} exceptions :\n{ex}", true);
                        errorCounter = 0;
                        errorNotifyTimeout.ChangeTime(10_000_000);
                    }
                }
                
            }

            return null;
        }

        private (double leftBorder, double topBorder, double rightBorder, double downBorder)
            AdjustImageAndCacheArea(int imgWidth, int imgHeight)
        {
            double leftBorder;
            double topBorder;
            double rightBorder;
            double downBorder;
            // Вычисляем координаты границ изображения
            if (LockOnPlayer)
            {
                var location = EntityManager.LocalPlayer.Location;
                _graphics.Reinitialize(location, imgWidth, imgHeight, Zoom,
                    out leftBorder, out topBorder, out rightBorder, out downBorder);

                var depthZ = _graphCache.CacheDistanceZ;
                _graphCache.SetCacheArea(leftBorder, topBorder, location.Z + depthZ, rightBorder, downBorder, location.Z - depthZ);
            }
            else

            {
                _graphics.Reinitialize(imgWidth, imgHeight, Zoom, out leftBorder, out topBorder,
                    out rightBorder, out downBorder);
            
                _graphCache.SetCacheArea(leftBorder, topBorder, rightBorder, downBorder);
            }
            return (leftBorder, topBorder, rightBorder, downBorder);
        }

        private void DrawSpecialObject(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
#if false
                        var objectPosition = SpecialObject;
                        if (objectPosition != null && objectPosition.IsValid)
                        {
                            _graphics.FillTriangleCentered(Brushes.Red, objectPosition, 16);
                            _graphics.DrawText("!", objectPosition);
                        }
#endif
        }

#if false
        private void DrawRoleGraphics()
        {
            try
            {
                if (!AstralAccessors.Controllers.Roles.CurrentRole.OnMapDraw(_graphics))
                    ComplexPatch_Mapper.DrawMeshes(
                        _graphics,
                        AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh
                    );
            }
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Error, string.Concat(nameof(DrawRoleGraphics), " catch an exception:\n\r", ex), true);
                ComplexPatch_Mapper.DrawMeshes(
                    _graphics,
                    AstralAccessors.Quester.Core.CurrentProfile.CurrentMesh
                );
            }
        } 
#endif
        private void DrawMapMeshes()
        {
            ComplexPatch_Mapper.DrawMeshes(_graphics, _graphCache);
        }
        
        private void DrawMapperTools(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
            // Отрисовка активного инструмента редактирования
            try
            {
                _graphics.GetWorldPosition(RelativeMousePosition, out double mouseX, out double mouseY);
                var tool = CurrentTool;
                bool customMouseCursor = false;
                string mcText = string.Empty;
                Alignment mcAlign = Alignment.None;
                Font mcFont = DefaultFont;
                Brush mcBrush = Brushes.Gray;

                if (tool != null)
                {
                    customMouseCursor =
                        tool.CustomMouseCursor(mouseX, mouseY, out mcText, out mcAlign, out mcFont, out mcBrush);

                    if (tool.AllowNodeSelection || tool.HandleCustomDraw)
                    {
                        //_graphics.GetWorldPosition(RelativeMousePosition, out double mouseX, out double mouseY);
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

                _graphics.DrawLine(Pens.Gray, mouseX, topBorder, mouseX, downBorder);
                _graphics.DrawLine(Pens.Gray, leftBorder, mouseY, rightBorder, mouseY);

                if (customMouseCursor)
                {
                    _graphics.DrawText(mcText, mouseX, mouseY, mcAlign, mcFont, mcBrush);
                }
                else
                {
                    _graphics.DrawText(string.Concat("(x)", mouseX.ToString("N2")), mouseX, mouseY, Alignment.BottomLeft,
                        mcFont, mcBrush);
                    _graphics.DrawText(string.Concat("(y)", mouseY.ToString("N2")), mouseX, mouseY, Alignment.TopLeft, mcFont,
                        mcBrush);
                }
            }
            catch (Exception ex)
            {
                InterruptAllModifications();
                ETLogger.WriteLine(LogType.Error,
                    string.Concat(nameof(DrawMapperTools), " catch an exception: \n\r", ex), true);
            }

            // Отрисовка инструмента прокладывания пути
            try
            {
                if (_mappingTool.MappingMode != MappingMode.Stopped)
                    _mappingTool.OnCustomDraw(_graphics);
            }
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Error,
                    string.Concat(nameof(DrawMapper), " catch an exception:\n\r", ex), true);
            }
        }

        private void DrawAOE(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
#if false
                        //foreach (AOECheck.AOE aoe in AOECheck.List)
                        if (AstralAccessors.Controllers.AOECheck.List != null)
                        {
                            foreach (AOECheck.AOE aoe in AstralAccessors.Controllers.AOECheck.GetAOEList<AOECheck.AOE>())
                            {
                                Vector3 vector = Vector3.Empty;
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
        }
        
        private void DrawDodgeArea(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
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
        }

        private void DrawCustomRegions(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
            foreach (var cr in _profile.CustomRegions)
            {
#if true
                _graphics.DrawCustomRegion(cr);
#else
                var pos = cr.Position;
                double width = cr.Width;
                double height = -cr.Height;
                double x1 = pos.X;
                double y1 = pos.Y;
                //double x2 = x1 + width;
                //double y2 = y1 - height;

                //bool isVisible = cr.Eliptic
                //    ? 
                //    : leftBorder <= x1 || x1 <= rightBorder || leftBorder <= x2 || x2 <= rightBorder
                //      || topBorder >= y1 || y1 >= downBorder || topBorder >= y2 || y2 >= downBorder;
#if true
                Pen pen = Pens.Blue;
#else
                Pen pen = cr.IsIn
                            ? Pens.Red
                            : Pens.White; 
#endif

                //_graphics.DrawCustomRegionEditable(x1, y1, x2, y2, cr.Eliptic);
                //_graphics.DrawText(cr.Name, x1 + width / 2d, y1, Alignment.BottomCenter, SystemFonts.DefaultFont, Brushes.White);
                if (cr.Eliptic)
                {
                    _graphics.DrawEllipse(pen, x1, y1, width, height, true);
                    _graphics.DrawText(cr.Name, x1 + width / 2d, y1, Alignment.BottomCenter, SystemFonts.DefaultFont, Brushes.Blue);
                }
                else
                {
                    _graphics.DrawRectangle(pen, x1, y1, width, height, true);
                    _graphics.DrawText(cr.Name, x1, y1, Alignment.BottomLeft, SystemFonts.DefaultFont, Brushes.Blue);
                }
#endif
            }
        }

        private void DrawHotSpots(double topBorder, double downBorder, double leftBorder, double rightBorder)
        {
#if false
            if (Roles.IsRunning)
	        {
		        Astral.Quester.Classes.Action getFirstCurrentAction = Core.Profile.GetFirstCurrentAction;
		        if (getFirstCurrentAction.UseHotSpots)
		        {
			        Navmesh.DrawHotSpots(getFirstCurrentAction.HotSpots, graph);
		        }
		        using (List<Astral.Quester.Classes.Action>.Enumerator enumerator2 = Core.Profile.MainActionPack.AP.CurrentActions.GetEnumerator())
		        {
			        while (enumerator2.MoveNext())
			        {
				        Astral.Quester.Classes.Action action = enumerator2.Current;
				        action.OnMapDraw(graph);
			        }
			        goto IL_194;
		        }
	        }
	        if (Main.editorForm != null && !Main.editorForm.IsDisposed)
	        {
		        if (Entrypoint.actionToDraw != null)
		        {
			        if (Entrypoint.actionToDraw.UseHotSpots)
			        {
				        Navmesh.DrawHotSpots(Entrypoint.actionToDraw.HotSpots, graph);
			        }
			        Entrypoint.actionToDraw.OnMapDraw(graph);
		        }
	        }
	        else if (Entrypoint.actionToDraw2 != null && Entrypoint.lastMain != null && !Entrypoint.lastMain.IsDisposed)
	        {
		        if (Entrypoint.actionToDraw2.UseHotSpots)
		        {
			        Navmesh.DrawHotSpots(Entrypoint.actionToDraw2.HotSpots, graph);
		        }
		        Entrypoint.actionToDraw2.OnMapDraw(graph);
	        }
#endif
        }

        private void DrawTargetableNodes(double leftBorder, double rightBorder, double downBorder, double topBorder)
        {
            bool drawNode = EntityTools.Config.Mapper.MapperForm.DrawNodes;
            bool drawSkillNode = EntityTools.Config.Mapper.MapperForm.DrawSkillNodes;
            if (drawNode || drawSkillNode)
            {
                var lootBrush = _graphics.DrawingTools.SkillnodeBrush;
                var brush = _graphics.DrawingTools.NodeBrush;
                foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                {
                    var location = targetableNode.WorldInteractionNode.Location;
                    var x = location.X;
                    var y = location.Y;
                    if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                    {
                        if (targetableNode.Categories.Contains("Loot"))
                        {
                            if (drawNode)
                                _graphics.FillCircleCentered(lootBrush, x, y, 6);
                        }
                        else if (drawSkillNode)
                            _graphics.FillSquareCentered(brush, x, y, 6);
                    }
                }
            }
        }

        private void DrawCampfires(double leftBorder, double rightBorder, double downBorder, double topBorder)
        {
            foreach (MinimapWaypoint minimapWaypoint in EntityManager.LocalPlayer.Player.MissionInfo.Waypoints)
            {
                var location = minimapWaypoint.Position;
                var x = location.X;
                var y = location.Y;
                if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder
                    && minimapWaypoint.IsCampFire)
                {
                    _graphics.FillSquareCentered(Brushes.DarkOrange, x, y, 12);
                }
            }
        }

        private void DrawDestination(double leftBorder, double rightBorder, double downBorder, double topBorder)
        {
            var location = Navigator.GetDestination;
            var x = location.X;
            var y = location.Y;
            if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                _graphics.DrawCircleCentered(Pens.Red, x, y, 14);
        }

        private void DrawPlayer(double leftBorder, double rightBorder, double downBorder, double topBorder)
        {
            var location = EntityManager.LocalPlayer.Location;
            var x = location.X;
            var y = location.Y;
            if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
            {
                double angle = EntityManager.LocalPlayer.Yaw * 180 / Math.PI;
                Bitmap image = MapperHelper.RotateImage(Resources.charArrowColor, (float) angle);
                _graphics.drawImage(location, image);
            }
        }

        private void DrawEnemies(double leftBorder, double rightBorder, double downBorder, double topBorder)
        {
            bool drawEnemies = EntityTools.Config.Mapper.MapperForm.DrawEnemies;
            bool drawFriends = EntityTools.Config.Mapper.MapperForm.DrawFriends;
            bool drawPlayers = EntityTools.Config.Mapper.MapperForm.DrawPlayers;
            bool drawOtherNpc = EntityTools.Config.Mapper.MapperForm.DrawOtherNPC;
            if (drawEnemies || drawFriends || drawPlayers || drawOtherNpc)
            {
                var enemyBrush = _graphics.DrawingTools.EnemyBrush;
                var friendBrush = _graphics.DrawingTools.FriendBrush;
                var playerBrush = _graphics.DrawingTools.PlayerBrush;
                var otherBrush = _graphics.DrawingTools.OtherNPCBrush;
                uint playerContainerId = EntityManager.LocalPlayer.ContainerId;
                foreach (Entity entity in EntityManager.GetEntities())
                {
                    var location = entity.Location;
                    var x = location.X;
                    var y = location.Y;
                    if (entity.ContainerId != playerContainerId
                        && leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                    {
                        if (entity.IsPlayer)
                        {
                            if (drawPlayers)
                                _graphics.FillRhombCentered<Vector3>(playerBrush, location, 10, 17);
                        }
                        else if (!entity.IsDead && !entity.Critter.IsLootable)
                        {
                            var relationToPlayer = entity.RelationToPlayer;
                            if (relationToPlayer == EntityRelation.Foe)
                            {
                                if (drawEnemies)
                                    _graphics.FillRhombCentered<Vector3>(enemyBrush, location, 10, 10);
                            }
                            else if (relationToPlayer == EntityRelation.Friend)
                            {
                                if (drawFriends)
                                    _graphics.FillRhombCentered<Vector3>(friendBrush, location, 6, 6);
                            }
                            else if (drawOtherNpc)
                                _graphics.FillRhombCentered<Vector3>(otherBrush, location, 6, 6);
                        }
                    }
                }
            }
        }
        #endregion

        #region Добавление и изменение CustomRegion'ов
#if false
        /// <summary>
        /// Флаг, указывающий, что CustomRegion'ы в профиле были изменены, и его необходимо "сохранить"
        /// </summary>
        bool CurrentProfileNeedSave
        {
            get => !string.IsNullOrEmpty(currentProfileName) && _profile.FileName == currentProfileName;
            set
            {
                if (value)
                    currentProfileName = _profile.FileName;
                else currentProfileName = string.Empty;
            }
        }
        string currentProfileName = string.Empty; 
#endif

        /// <summary>
        /// Запуск процедуры добавления CustomRegion'а
        /// </summary>
        private void handler_StartAddingCustomRegion(object sender, ItemClickEventArgs e)
        {
            if (CurrentTool is AddCustomRegionTool addTool)
            {
                addTool.Close();
                CurrentTool = null;
            }
            else
            {
                btnLockMapOnPlayer.Checked = false;
                CurrentTool = new AddCustomRegionTool(this, OnCustomRegionAdded);
            }
        }

        /// <summary>
        /// Callback-метод, реализующий реакцию на добавление <paramref name="customRegion"/>
        /// </summary>
        /// <param name="customRegion"></param>
        /// <param name="undo"></param>
        private void OnCustomRegionAdded(CustomRegion customRegion, IMapperTool undo)
        {
            if(customRegion != null)
                _profile.Saved = false;
            CurrentTool = null;
        }

        /// <summary>
        /// Выбор и редактирование существующего CustomRegion'а
        /// </summary>
        private void handler_StartEditingCustomRegion(object sender, ItemClickEventArgs e)
        {
            if (CurrentTool is EditCustomRegionTool editTool)
            {
                editTool.Close();
                CurrentTool = null;
            }
            else
            {
                var crList = _profile.CustomRegions;
                if (crList.Count > 0)
                {
                    btnLockMapOnPlayer.Checked = false;
                    CurrentTool = new EditCustomRegionTool(null, this, OnCustomRegionEdited); 
                }
            }
        }

        /// <summary>
        /// Callback-метод, реализующий реакцию на изменение <paramref name="customRegion"/>
        /// </summary>
        /// <param name="customRegion"></param>
        /// <param name="undo"></param>
        private void OnCustomRegionEdited(CustomRegion customRegion, IMapperTool undo)
        {
            if (customRegion != null)
                _profile.Saved = false;
            if (customRegion != null
                && undo != null)
            {
                _undoStack.Push(undo);
            }
            else CurrentTool = null;
        }
        #endregion

        #region Изменение настроек
        private void handler_WaypointDistanceChanged(object sender, EventArgs e)
        {
            int value = Convert.ToInt32(editWaypointDistance.EditValue);
            EntityTools.Config.Mapper.WaypointDistance = value;
            EntityTools.Config.Mapper.WaypointEquivalenceDistance = value / 2;
        }

        private void handler_ShowStatusBar(object sender, EventArgs e)
        {
            barStatus.Visible = true;
            btnShowStatBar.Visible = false;
        }

        private void handler_BarVisibleChanged(object sender, EventArgs e)
        {
            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barGraphTools.Visible && !barGraphEditTools.Visible && !barCustomRegions.Visible;
        }

        private void handler_ShowSettingsTab(object sender, ItemClickEventArgs e)
        {
            if (settingsForm is null || settingsForm.IsDisposed)
            {
                settingsForm = new MapperSettingsForm();
            }
            if(!settingsForm.Visible)
                settingsForm.Show(this);
        }
        #endregion

        #region Изменение графа (Meshes)
        /// <summary>
        /// Сохранение в файл текущего Quester-профиля
        /// </summary>
        private void handler_SaveChanges2QuesterProfile(object sender, ItemClickEventArgs e)
        {
            _profile.Save();
            if (!_profile.Saved)
                Logger.Notify($"Fail to save profile '{Path.GetFileName(_profile.ProfilePath)}'", true);
        }

        /// <summary>
        /// Импорт мешей карты (графа путей) из Игры
        /// </summary>
        private void handler_ImportCurrentMapMeshesFromGame(object sender, ItemClickEventArgs e)
        {
            handler_Mapping_Stop();
            ResetToolState();

            Graph graph = _profile.CurrentMesh;
            DialogResult dialogResult  = DialogResult.Yes;
            if (graph.NodesCount > 0)
                dialogResult = XtraMessageBox.Show(this,
                    "Are you sure to import game nodes?\n" +
                    "Yes: All nodes of the current map would be deleted!\n" +
                    "No: All game nodes will be added to the current map without deleting existing nodes.", "",
                    MessageBoxButtons.YesNoCancel);

            switch (dialogResult)
            {
                case DialogResult.Yes:
                    using (graph.WriteLock())
                    {
                        graph.Clear();
                        GoldenPath.GetCurrentMapGraph(graph);
                    }
                    break;
                case DialogResult.No:
                    using (graph.WriteLock())
                    {
                        GoldenPath.GetCurrentMapGraph(graph);
                    }
                    break;
            }
            
        }

        /// <summary>
        /// Импорт мешей одной или всех карт из файла
        /// </summary>
        private void handler_ImportMapMeshesFromFile(object sender, ItemClickEventArgs e)
        {
            handler_Mapping_Stop();
            ResetToolState();

            string currentMapName = EntityManager.LocalPlayer.MapState.MapName;
            if (string.IsNullOrEmpty(currentMapName))
            {
                XtraMessageBox.Show("Impossible to get current map name !");
                return;
            }
            
            string currentMapMeshesName = currentMapName + ".bin";

            var openDialog = FileTools.GetOpenDialog(filter: @"Astral mission profile (*.amp.zip)|*.amp.zip|(*.mesh.zip)|*.mesh.zip",
                                                     defaultExtension: "amp.zip",
                                                     initialDir: Directories.ProfilesPath);

            if (openDialog.ShowDialog() != DialogResult.OK 
                || !File.Exists(openDialog.FileName)) 
                return;

            try
            {
                using (ZipArchive profile = ZipFile.Open(openDialog.FileName, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry currentMapEntry = profile.GetEntry(currentMapMeshesName);
                    if(currentMapEntry is null)
                    {
                        XtraMessageBox.Show("Selected file doesn't contain current map meshes!");
                        return;
                    }
                    var mapsMeshes = _profile.MapsMeshes;

                    DialogResult dialogResult = XtraMessageBox.Show("Import current map only ?\n\rElse import all.", "Map import", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // Экспорт одной "текущей" карты
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
                                Logger.WriteLine($"Import '{currentMapMeshesName}' from {openDialog.FileName}");
                                return;
                            }

                            string msg = $"Failed to import '{currentMapMeshesName}' from {openDialog.FileName}";
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
                        sb.Append("Import maps meshes from  '").Append(openDialog.FileName).AppendLine("':");
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
                                    lock (mapsMeshes)
                                    {
                                        if (mapsMeshes.ContainsKey(entryMapName))
                                            mapsMeshes[entryMapName] = entryMapMeshes;
                                        else mapsMeshes.Add(entryMapName, entryMapMeshes); 
                                    }
                                    sb.Append("\t'").Append(entryMapName).AppendLine("' - succeeded");
                                }
                                else sb.Append("\t'").Append(entryMapName).AppendLine("' - failed");
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
            var graph = _profile.CurrentMesh;
            if (graph?.NodesCount > 0 && XtraMessageBox.Show(this, "Are you sure to delete all map nodes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_Mapping_Stop();
                ResetToolState();

                //_currentMapHash = 0;
                graph.Clear();
            }
        }

        /// <summary>
        /// Оценка графа на наличие мусора
        /// </summary>
        private void handler_MeshesInfo(object sender, ItemClickEventArgs e)
        {
            var graph = _profile.CurrentMesh;
            if (graph?.NodesCount > 0)
            {
                int correctNodeNum = 0;
                int unpasNodeNum = 0;
                var nodes = graph.Nodes;
                int correctArcNum = 0;
                int disabledArcNum = 0;
                int unpasArcNum = 0;
                int totalArcCount = 0;

                foreach (Node nd in nodes)
                {
                    if (!nd.Passable)
                        unpasNodeNum++;
                    else correctNodeNum++;
                    foreach (Arc arc in nd.OutgoingArcs)
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
        }

        /// <summary>
        /// Сжатие графа (удаление мусора)
        /// </summary>
        private void handler_MeshesCompression(object sender, ItemClickEventArgs e)
        {
            handler_Mapping_Stop();
            ResetToolState();
            var graph = _profile.CurrentMesh;

            if (graph?.NodesCount > 0)
            {
                int correctNodeNum = 0;
                int unpasNodeNum = 0;
                int totalNodeNum = graph.NodesCount;
                int correctArcNum = 0;
                int disabledArcNum = 0;
                int unpasArcNum = 0;
                int totalArcNum = 0;
                foreach (Node nd in graph.Nodes)
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

                if (unpasNodeNum > 0 && graph.RemoveUnpassable() > 0)
                {
                    ResetToolState();

                    int correctNodeNumNew = 0;
                    int unpasNodeNumNew = 0;
                    int correctArcNumNew = 0;
                    int disabledArcNumNew = 0;
                    int unpasArcNumNew = 0;
                    int totalArcNumNew = 0;
                    foreach (Node nd in graph.Nodes)
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


                    XtraMessageBox.Show($"Total nodes: {totalNodeNum} => {graph.NodesCount}\n\r" +
                        $"\tcorrect:\t{correctNodeNum} => {correctNodeNumNew}\n\r" +
                        $"\tunpassable:\t{unpasNodeNum} => {unpasNodeNumNew}\n\r" +
                        $"Total arcs: {totalArcNum} => {totalArcNumNew}\n\r" +
                        $"\tcorrect:\t{correctArcNum} => {correctArcNumNew}\n\r" +
                        $"\tdisabled:\t{disabledArcNum} => {disabledArcNumNew}\n\r" +
                        $"\tunpassable:\t{unpasArcNum} => {unpasArcNumNew}\n\r");
                }
                else XtraMessageBox.Show("Meshes doesn't compressed\n\r" +
                                        $"Total nodes: {graph.NodesCount}\n\r" +
                                        $"\tcorrect:\t{correctNodeNum}\n\r" +
                                        $"\tunpassable:\t{unpasNodeNum}\n\r" +
                                        $"Total arcs: {totalArcNum}\n\r" +
                                        $"\tcorrect:\t{correctArcNum}\n\r" +
                                        $"\tdisabled:\t{disabledArcNum}\n\r" +
                                        $"\tunpassable:\t{unpasArcNum}\n\r"); 
            }
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
            {
                CurrentTool = null;
            } 
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
            {
                CurrentTool = null;
            }
        }

        private void handler_AddNode_ModeChanged(object sender, ItemClickEventArgs e)
        {
            AddNodeTool addTool = CurrentTool as AddNodeTool;
            if (btnAddNodes.Checked)
            {
                if (addTool is null)
                    CurrentTool = new AddNodeTool();
                LockOnPlayer = false;
            }
            else if (addTool != null)
            {
                CurrentTool = null;
            }
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

        private void handler_Mapping_BidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingBidirectional.Checked)
            {
                _profile.Saved = false;
                _mappingTool.MappingMode = MappingMode.Bidirectional;
                InterruptAllModifications(MapperEditMode.Mapping);
            }
        }

        private void handler_Mapping_UnidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingUnidirectional.Checked)
            {
                _profile.Saved = false;
                _mappingTool.MappingMode = MappingMode.Unidirectional;
                InterruptAllModifications(MapperEditMode.Mapping);
            }
        }

        private void handler_Mapping_Stop(object sender = null, ItemClickEventArgs e = null)
        {
            if (btnMappingStop.Checked)
            {
                _mappingTool.MappingMode = MappingMode.Stopped;
                InterruptAllModifications();
            }
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

        private void handler_DistanceMeasurement_ModeChanged(object sender, ItemClickEventArgs e)
        {
            DistanceMeasurementTool measurementTool = CurrentTool as DistanceMeasurementTool;
            if (btnDistanceMeasurement.Checked)
            {
                if (measurementTool is null)
                    CurrentTool = new DistanceMeasurementTool();
                LockOnPlayer = false;
            }
            else if (measurementTool != null)
                CurrentTool = null;
        }

        private void handler_ObjectInfo(object sender, ItemClickEventArgs e)
        {
            ObjectInfoTool infoTool = CurrentTool as ObjectInfoTool;
            if (btnObjectInfo.Checked)
            {
                if (infoTool is null)
                    CurrentTool = new ObjectInfoTool();
                LockOnPlayer = false;
            }
            else if (infoTool != null)
                CurrentTool = null;
        }

        private void handler_Change_PanelVisibility(object sender, ItemClickEventArgs e)
        {
            if (!barMapping.Visible
                || !barGraphTools.Visible
                || !barGraphEditTools.Visible
                || !barCustomRegions.Visible)
            {
                barMapping.Visible = true;
                barGraphTools.Visible = true;
                barGraphEditTools.Visible = true;
                barCustomRegions.Visible = true;
            }
            else
            {
                barMapping.Visible = false;
                barGraphTools.Visible = false;
                barGraphEditTools.Visible = false;
                barCustomRegions.Visible = false;
            }
        }

        private void handler_HelpRequested(object sender, HelpEventArgs _)
        {
            Process.Start(@"https://qpahta3ep.github.io/EntityToolsDocs/Patches/Mapper/Mapper-RU.html");
        }

        private void handler_OpenDebugTool(object sender, ItemClickEventArgs e)
        {
            ObjectInfoForm.Show(CurrentTool, 0, this);
        }
    }
#endif
}
