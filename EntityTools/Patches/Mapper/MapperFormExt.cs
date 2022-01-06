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
using AcTp0Tools;
#if PATCH_ASTRAL
using Astral.Logic.NW;
using Astral.Quester.Classes;
using MyNW.Classes;
// ReSharper disable UnusedVariable
#endif

namespace EntityTools.Patches.Mapper
{
    //TODO Подружить с ролью Professions
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm //*/Form
    {
        /// <summary>
        /// Флаг удержания персонажа в центре карты
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
        public static Vector3 SpecialObject { get; set; }

        /// <summary>
        /// Координаты курсора мыши, относительно формы <see cref="MapperFormExt"/>
        /// </summary>
        private Point RelativeMousePosition => MapPicture.PointToClient(MousePosition);

        #region Инициализация формы
        internal MapperFormExt()
        {
            InitializeComponent();

            MouseWheel += handler_MouseWheel;

            barMapping.Visible = EntityTools.Config.Mapper.MapperForm.MappingBarVisible;
            barGraphTools.Visible = EntityTools.Config.Mapper.MapperForm.MeshesBarVisible;
            barGraphEditTools.Visible = EntityTools.Config.Mapper.MapperForm.NodeToolsBarVisible;
            barCustomRegions.Visible = EntityTools.Config.Mapper.MapperForm.CustomRegionBarVisible;
            barStatus.Visible = EntityTools.Config.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barGraphTools.Visible && !barGraphEditTools.Visible && !barCustomRegions.Visible;

            Location = EntityTools.Config.Mapper.MapperForm.Location;

            BindingControls();

            _mappingTool = new MappingTool(() => AstralAccessors.Quester.Core.Meshes) {
                Linear = EntityTools.Config.Mapper.LinearPath,
                ForceLink = EntityTools.Config.Mapper.ForceLinkingWaypoint
            };

            _graphics.GraphCache.CacheDistanceZ = EntityTools.Config.Mapper.MapperForm.LayerDepth;
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
#if false
            menuCacheActive.DataBindings.Add(nameof(menuCacheActive.Checked),
                                                    EntityTools.Config.Mapper,
                                                    nameof(EntityTools.Config.Mapper.CacheActive),
                                                    false, DataSourceUpdateMode.OnPropertyChanged); 
#endif

#if false
            // Удаление двойным кликом мыши больше не используется

            /* Astral.API.CurrentSettings.DeleteNodeRadius не реализует INotifyPropertyChanged
             * поэтому привязка нижеуказанным методом невозможна
             * menuDeleteRadius.DataBindings.Add(new Binding(nameof(menuDeleteRadius.EditValue),
                                        EntityTools.Config.Mapper,
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
#endif

            btnMappingForceLink.DataBindings.Add(nameof(btnMappingForceLink.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.ForceLinkingWaypoint),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            btnMappingLinearPath.DataBindings.Add(nameof(btnMappingLinearPath.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.LinearPath),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            ckbChacheEnable.DataBindings.Add(nameof(ckbChacheEnable.Checked),
                                                EntityTools.Config.Mapper,
                                                nameof(EntityTools.Config.Mapper.CacheActive),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            editLayerDepth.DataBindings.Add(nameof(editLayerDepth.Value),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.LayerDepth),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            #region Customization
            colorEditBidirPath.DataBindings.Add(nameof(colorEditBidirPath.EditValue),
                                                    EntityTools.Config.Mapper.MapperForm,
                                                    nameof(EntityTools.Config.Mapper.MapperForm.BidirectionalPathColor),
                                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorEditUnidirPath.DataBindings.Add(nameof(colorEditUnidirPath.EditValue),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.UnidirectionalPathColor),
                                                false, DataSourceUpdateMode.OnPropertyChanged);

            colorBackground.DataBindings.Add(nameof(colorBackground.EditValue),
                                                EntityTools.Config.Mapper.MapperForm,
                                                nameof(EntityTools.Config.Mapper.MapperForm.BackgroundColor),
                                                false, DataSourceUpdateMode.OnPropertyChanged);


            ckbEnemies.DataBindings.Add(nameof(ckbEnemies.Checked),
                                            EntityTools.Config.Mapper.MapperForm,
                                            nameof(EntityTools.Config.Mapper.MapperForm.DrawEnemies),
                                            false, DataSourceUpdateMode.OnPropertyChanged);

            colorEnemies.DataBindings.Add(nameof(colorEnemies.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.EnemyColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbFriends.DataBindings.Add(nameof(ckbFriends.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawFriends),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorFriends.DataBindings.Add(nameof(colorFriends.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.FriendColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbPlayers.DataBindings.Add(nameof(ckbPlayers.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawPlayers),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorPlayers.DataBindings.Add(nameof(colorPlayers.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.PlayerColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbOtherNPC.DataBindings.Add(nameof(ckbOtherNPC.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawOtherNPC),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorOtherNPC.DataBindings.Add(nameof(colorOtherNPC.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.OtherNPCColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbNodes.DataBindings.Add(nameof(ckbNodes.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawNodes),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorNodes.DataBindings.Add(nameof(colorNodes.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.NodeColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            ckbSkillnodes.DataBindings.Add(nameof(ckbSkillnodes.Checked),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.DrawSkillNodes),
                                    false, DataSourceUpdateMode.OnPropertyChanged);

            colorSkillnodes.DataBindings.Add(nameof(colorSkillnodes.EditValue),
                                    EntityTools.Config.Mapper.MapperForm,
                                    nameof(EntityTools.Config.Mapper.MapperForm.SkillNodeColor),
                                    false, DataSourceUpdateMode.OnPropertyChanged); 
            #endregion


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

            EntityTools.Config.Mapper.MapperForm.MappingBarVisible = barMapping.Visible;
            EntityTools.Config.Mapper.MapperForm.MeshesBarVisible = barGraphTools.Visible;
            EntityTools.Config.Mapper.MapperForm.NodeToolsBarVisible = barGraphEditTools.Visible;
            EntityTools.Config.Mapper.MapperForm.CustomRegionBarVisible = barCustomRegions.Visible;
            EntityTools.Config.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;

#if false
            btnShowStatBar.Visible = !barStatus.Visible && !barMapping.Visible && !barMeshes.Visible && !barNodeTools.Visible && !barCustomRegions.Visible; 
#endif

            EntityTools.Config.Mapper.MapperForm.Location = Location;
            if(WindowState == FormWindowState.Normal)
                EntityTools.Config.Mapper.MapperForm.Size = Size;

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
                   statusStr,
                   playerPosStr,
                   mousePosStr,
                   zoomStr = string.Empty;
            Image img = null;



            //TODO Заменить Environment.TickCount на DateTime.Now.Ticks 
#if DrawMapper_Measuring
            Timeout timeout = new Timeout(0);
            const int SPEED_MEASURES_NUM = 10;
            const int MAPPER_MEASURES_NUM = 10;
            Stopwatch sw = new Stopwatch();

            long[] drawMapperMeasures = new long[MAPPER_MEASURES_NUM];

            Tuple<Vector3, int, double>[] speedMeasures = new Tuple<Vector3, int, double>[SPEED_MEASURES_NUM];
            Vector3 lastPlayerPos = null;
            int lastTickCount = Environment.TickCount;
            int movingTime = 0;
            double pathDistance = 0;

            int currentMesure = 0;

            const int time = 5000;
            double frames = 0;
            double fps = 0, cps = 0;

            int cacheVer = 0;
#endif
            try
            {
                while (!IsDisposed
                        && !backgroundWorker.CancellationPending)
                {
                    var player = EntityManager.LocalPlayer;
                    bool isLoading = player.IsLoading || !player.MapState.IsValid;
                    if (isLoading)
                    {
                        _currentMapHash = 0;
                        playerPosStr = "Loading";
                        statusStr = "-";
                        mousePosStr = "-";
                        movingTime = 0;
                        pathDistance = 0;
                        lastPlayerPos = null;
                    }
                    else
                    {
                        var playerPos = player.Location.Clone();
                        Vector3 pos = LockOnPlayer ? playerPos : _graphics.CenterPosition;

                        // Вычисляем скорость перемещения;
                        int currMeasureInd = currentMesure % SPEED_MEASURES_NUM;
                        int prevMeasureInd = currMeasureInd == 0 ? SPEED_MEASURES_NUM - 1 : currMeasureInd - 1;
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
                        for (currMeasureInd = 0; currMeasureInd < SPEED_MEASURES_NUM; currMeasureInd++)
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
#else
                        playerPosStr = string.Concat(pos.X.ToString("N1"), " | ", pos.Y.ToString("N1"), " | ", pos.Z.ToString("N1"), " || ", speed.ToString("N3"), "f/s (", num, ")");
#endif
                        zoomStr = string.Concat(Zoom * 100, '%');

                        int hash = AstralAccessors.Quester.Core.Meshes?.GetHashCode() ?? 0;
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
                        
                        drawMapperMeasures[currentMesure % MAPPER_MEASURES_NUM] = sw.ElapsedMilliseconds;

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
                        statusStr = string.Concat(fps.ToString("N1"), " fps | ", (drawMapperMeasures.Sum() / 10d).ToString("N1"), " ms | ", cps.ToString("N1"), " cps");
                        //statusStr = string.Concat("(1) ", speed.ToString("N2"), "fps (", distance.ToString("N2"), '/', (totalTicks / 1000d).ToString("N2"), ") || (2) ", speed2.ToString("N2"), " fps (", pathDistance.ToString("N2"), '/', (movingTime / 1000d).ToString("N2"), ')');
                        //statusStr = string.Concat(speed1.ToString("N3"), "f/ms (", num, ") | ",speed2.ToString("N3"), "f/s (", totalDistance.ToString("N2"), '/', (totalTicks / 1000d).ToString("N2"), ")");

                        _graphics.GetWorldPosition(RelativeMousePosition, out double mouseX, out double mouseY);

                        mousePosStr = string.Concat(mouseX.ToString("N1"), " | ", mouseY.ToString("N1"));

                    }

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            Text = formCaption;
                            lblMousePos.Caption = mousePosStr;
                            lblPlayerPos.Caption = playerPosStr;
                            lblDrawInfo.Caption = statusStr;
                            lblZoom.Caption = zoomStr;
                            MapPicture.Image = img;
                        }));

                    }
                    else
                    {
                        Text = formCaption;
                        lblMousePos.Caption = statusStr;
                        lblZoom.Caption = zoomStr;
                        MapPicture.Image = img;
                    }

                    Thread.Sleep(EntityTools.Config.Mapper.MapperForm.RedrawMapperTimeout);
                }
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, "MapperFormUpdate: Перехвачено исключение: " + exc);
                if (InvokeRequired)
                    Invoke(new Action(() => Text = exc.Message));
                else
                {
                    Text = exc.Message;
                }
                throw;
            }
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

                    var graph = _graphics.GraphCache;
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

                    var graph = _graphics.GraphCache;
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

                    var graph = _graphics.GraphCache;
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

                    var graph = _graphics.GraphCache;
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
                    MapperEditMode mode = _mappingTool.MappingMode != MappingMode.Stoped 
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
            if (mode != MapperEditMode.Mapping && _mappingTool?.MappingMode == MappingMode.Stoped)
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
            if (mode != MapperEditMode.AddCustomRegion
                && mode != MapperEditMode.EditCustomRegion)
            {
                barEditCustomRegion.Visible = false;
            }
            if(mode != MapperEditMode.DistanceMeasurement)
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

        Timeout mouseWeelTimeout = new Timeout(0);

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
                            _graphics.MoveCenterPosition(0, 0, 50);
                        else _graphics.MoveCenterPosition(0, 0, -50);
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
        private readonly MapperGraphics _graphics = new MapperGraphics(360, 360);

        /// <summary>
        /// Метод для фоновой отрисовки карты
        /// </summary>
        private void DrawMapper()
        {
            Timeout timeout = new Timeout(0);

            if (IsDisposed || !Visible || !MapPicture.Visible || MapPicture.Width <= 0 ||
                MapPicture.Height <= 0) return;

            try
            {
                using (_graphics.WriteLock())
                {
                    int imgWidth = MapPicture.Width;
                    int imgHeight = MapPicture.Height;

                    double leftBorder, topBorder, rightBorder, downBorder;

                    // Вычисляем координаты границ изображения
                    if (LockOnPlayer)
                        _graphics.Reinitialize(EntityManager.LocalPlayer.Location, imgWidth, imgHeight, Zoom, out leftBorder, out topBorder, out rightBorder, out downBorder);
                    else _graphics.Reinitialize(imgWidth, imgHeight, Zoom, out leftBorder, out topBorder, out rightBorder, out downBorder);

                    Vector3 location;
                    float x, y;

                    #region Отрисовка нодов
                    bool drawNode = EntityTools.Config.Mapper.MapperForm.DrawNodes;
                    bool drawSkillNode = EntityTools.Config.Mapper.MapperForm.DrawSkillNodes;
                    if (drawNode || drawSkillNode)
                    {
                        var lootBrush = _graphics.DrawingTools.SkillnodeBrush;
                        var brush = _graphics.DrawingTools.NodeBrush;
                        foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                        {
                            location = targetableNode.WorldInteractionNode.Location;
                            x = location.X;
                            y = location.Y;
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
                        var objectPosition = SpecialObject;
                        if (objectPosition != null && objectPosition.IsValid)
                        {
                            _graphics.FillTriangleCentered(Brushes.Red, objectPosition, 16);
                            _graphics.DrawText("!", objectPosition);
                        }

#endif
                    #endregion

                    #region Отрисовка АОЕ
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
                    #endregion

                    #region Отрисовка графики, связанной с выполняемой ролью 
                    try
                    {
                        if (!AstralAccessors.Controllers.Roles.CurrentRole.OnMapDraw(_graphics))
                            //lock (AstralAccessors.Quester.Core.Meshes.Value.SyncRoot) <- Блокировка графа есть в DrawMeshes(..)
                            ComplexPatch_Mapper.DrawMeshes(_graphics,
                                AstralAccessors.Quester.Core.Meshes);
                    }
                    catch (Exception ex)
                    {
                        ETLogger.WriteLine(LogType.Error, string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                        ComplexPatch_Mapper.DrawMeshes(_graphics,
                            AstralAccessors.Quester.Core.Meshes);
                    }
                    #endregion

                    #region Отрисовка игроков и НПС
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
                            location = entity.Location;
                            x = location.X;
                            y = location.Y;
                            if (entity.ContainerId != playerContainerId
                                && leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                            {
                                if (entity.IsPlayer)
                                {
                                    if (drawPlayers)
                                        _graphics.FillRhombCentered(playerBrush, location, 10, 17);
                                }
                                else if (!entity.IsDead && !entity.Critter.IsLootable)
                                {
                                    var relationToPlayer = entity.RelationToPlayer;
                                    if (relationToPlayer == EntityRelation.Foe)
                                    {
                                        if (drawEnemies)
                                            _graphics.FillRhombCentered(enemyBrush, location, 10, 10);
                                    }
                                    else if (relationToPlayer == EntityRelation.Friend)
                                    {
                                        if(drawFriends)
                                            _graphics.FillRhombCentered(friendBrush, location, 6, 6);
                                    }
                                    else if(drawOtherNpc)
                                        _graphics.FillRhombCentered(otherBrush, location, 6, 6);
                                }
                            }
                        } 
                    }
                    #endregion

                    #region Отрисовка специальной графики и указателя мыши
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
                            customMouseCursor = tool.CustomMouseCusor(mouseX, mouseY, out mcText, out mcAlign, out mcFont, out mcBrush);

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

                        if(customMouseCursor)
                        {
                            _graphics.DrawText(mcText, mouseX, mouseY, mcAlign, mcFont, mcBrush);
                        }
                        else
                        {
                            _graphics.DrawText(string.Concat("(x)", mouseX.ToString("N2")), mouseX, mouseY, Alignment.BottomLeft, mcFont, mcBrush);
                            _graphics.DrawText(string.Concat("(y)", mouseY.ToString("N2")), mouseX, mouseY, Alignment.TopLeft, mcFont, mcBrush);
                        }
                    }
                    catch (Exception ex)
                    {
                        InterruptAllModifications();
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
                        InterruptAllModifications();
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
                        _graphics.DrawCircleCentered(Pens.Red, x, y, 14);
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
            catch (ObjectDisposedException ex)
            {
                Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                throw;
            }
            catch (ThreadAbortException ex)
            {
                Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex);
                throw;
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
                return !string.IsNullOrEmpty(currentProfileName) && Astral.Controllers.Settings.Get.LastQuesterProfile == currentProfileName;
            }
            set
            {
                if (value)
                    currentProfileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
                else currentProfileName = string.Empty;
            }
        }
        string currentProfileName = string.Empty;

        /// <summary>
        /// Запуск процедуры добавления CustomRegion'а
        /// </summary>
        private void handler_StartAddingCustomRegion(object sender, ItemClickEventArgs e)
        {
            AddCustomRegionTool addCRTool = CurrentTool as AddCustomRegionTool;
            if (addCRTool is null)
                CurrentTool = new AddCustomRegionTool(btnCRTypeSelector.Checked);

            btnLockMapOnPlayer.Checked = false;

            barEditCustomRegion.Text = @"Add CustomRegion";
            barEditCustomRegion.DockStyle = BarDockStyle.None;
            barEditCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 20, 
                Location.Y + MapPicture.Location.Y + 20 
                + (barMapping.Visible && barMapping.DockStyle == BarDockStyle.Top ? barMapping.Size.Height : 0)
                + (barGraphTools.Visible && barGraphTools.DockStyle == BarDockStyle.Top ? barGraphTools.Size.Height : 0));
            //barEditCustomRegion.FloatSize = new Size(Math.Max(100, MapPicture.Width - 40), 0);
            
            editCRName.Visibility = BarItemVisibility.Always;
            editCRName.EditWidth = Math.Max(300, Width - 40);
            
            editCRSelector.Visibility = BarItemVisibility.Never;
            editCRSelector.EditWidth = Math.Max(300, Width - 40);

            btnCRTypeSelector.Visibility = BarItemVisibility.Always;
            
            btnCRRename.Visibility = BarItemVisibility.Never;
            btnCRRename.Checked = false;
            
            btnCRAdditionAccept.Visibility = BarItemVisibility.Always;
            btnCREditionAccept.Visibility = BarItemVisibility.Never;
            
            barEditCustomRegion.Visible = true;
        }

        /// <summary>
        /// Выбор и редактирование существующего CustomRegion'а
        /// </summary>
        private void handler_StartEditingCustomRegion(object sender, ItemClickEventArgs e)
        {
            var customRegions = Astral.Quester.API.CurrentProfile.CustomRegions;
            if (customRegions.Count == 0)
            {
                XtraMessageBox.Show("No CustomRegions available to change");
                return;
            }

            EditCustomRegionTool editCRTool = CurrentTool as EditCustomRegionTool;
            if (editCRTool is null)
                CurrentTool = new EditCustomRegionTool();

            btnLockMapOnPlayer.Checked = false;

            barEditCustomRegion.Text = @"Edit CustomRegion";
            barEditCustomRegion.DockStyle = BarDockStyle.None;
            barEditCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 20,
                Location.Y + MapPicture.Location.Y + 20
                + (barMapping.Visible && barMapping.DockStyle == BarDockStyle.Top
                    ? barMapping.Size.Height
                    : 0)
                + (barGraphTools.Visible && barGraphTools.DockStyle == BarDockStyle.Top
                    ? barGraphTools.Size.Height
                    : 0));

            editCRName.Visibility = BarItemVisibility.Never;
            editCRName.EditValue = string.Empty;
            editCRName.EditWidth = Math.Max(300, Width - 40);

            btnCRTypeSelector.Visibility = BarItemVisibility.Always;

            editCRSelector.Visibility = BarItemVisibility.Always;
            editCRSelector.EditWidth = Math.Max(300, Width - 40);

            btnCRRename.Visibility = BarItemVisibility.Always;
            btnCRRename.Checked = false;

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

        /// <summary>
        /// Изменение модифицируемого CustomRegion'a
        /// </summary>
        private void handler_ChangeSelectedCustomRegion(object sender, EventArgs e = null)
        {
            EditCustomRegionTool editCRTool = CurrentTool as EditCustomRegionTool;

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

                        //Обновление списка CustomRegion'ов в Quester-редакторе
                        AstralAccessors.Quester.Forms.Editor.EditorForm?.RefreshRegions();

                        CurrentTool = new EditCustomRegionTool(cr);
                    }
                    else editCRTool.AttachTo(cr);
                }

                var crPos = cr.Position.Clone();
                crPos.Z = CenterOfMap.Z;
                CenterOfMap = crPos;

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

                string crName = editCRName.EditValue.ToString().Trim();
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

                            //Обновление списка CustomRegion'ов в Quester-редакторе
                            AstralAccessors.Quester.Forms.Editor.EditorForm?.RefreshRegions();

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
            if (CurrentTool is EditCustomRegionTool editCRTool)
            {
                var newName = editCRName.EditValue.ToString();
                bool nameChanged = btnCRRename.Checked && !string.IsNullOrEmpty(newName) && !editCRTool.Name.Equals(newName);

                if (editCRTool.Modified || nameChanged)
                {
                    var editedCR = editCRTool.Apply(newName);

                    сurrentProfileNeedSave = true;

                    //Обновление списка CustomRegion'ов в Quester-редакторе
                    AstralAccessors.Quester.Forms.Editor.EditorForm?.RefreshRegions();

                    editCRName.Visibility = BarItemVisibility.Never;
                    editCRName.EditValue = string.Empty;

                    btnCRRename.Checked = false;

                    editCRSelector.Visibility = BarItemVisibility.Always;
                    editCRSelector.Refresh();

                    CurrentTool = new EditCustomRegionTool(editedCR);
                }
            }
        }

        /// <summary>
        /// Прерывание процедуры добавления/редактирования региона
        /// </summary>
        private void handler_CancelCRManipulation(object sender, ItemClickEventArgs e)
        {
            var tool = CurrentTool;
            if (tool is AddCustomRegionTool
                || tool is EditCustomRegionTool)
                CurrentTool = null;
            barEditCustomRegion.Visible = false;
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
            panelSettings.Visible = btnSettings.Checked;
        }

        private void handler_LayerDepth_Changed(object sender, EventArgs e)
        {
            _graphics.GraphCache.CacheDistanceZ = editLayerDepth.Value > 0 ? Convert.ToDouble(editLayerDepth.Value) : double.MaxValue;
        }
                        #endregion

        #region Изменение графа (Meshes)
        /// <summary>
        /// Сохранение в файл текущего Quester-профиля
        /// </summary>
        private void handler_SaveChanges2QuesterProfile(object sender, ItemClickEventArgs e)
        {
            // Штатное сохранение профиля реализовано в
            // Astral.Quester.Core.Save(false)

            //TODO переделать с использованием Core.CurrentProfileZipMeshFile и внешних мешей

            string meshName = EntityManager.LocalPlayer.MapState.MapName + ".bin";
            string profileName = Astral.Controllers.Settings.Get.LastQuesterProfile;
            if (File.Exists(profileName))
            {
                var currentProfile = Astral.Quester.API.CurrentProfile;
                bool useExternalMeshFile = currentProfile.UseExternalMeshFile && currentProfile.ExternalMeshFileName.Length >= 10;
                string externalMeshFileName = useExternalMeshFile ? Path.Combine(Path.GetDirectoryName(profileName), currentProfile.ExternalMeshFileName) : string.Empty;
                Graph mesh = AstralAccessors.Quester.Core.Meshes;

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
                            //Patch_Astral_Quester_Core_Save.SaveProfile(zipFile);
                            AstralAccessors.Quester.Core.SaveProfile(zipFile);
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

                    bool succeeded = false;
                    if (mesh?.NodesCount > 0)
                    {
                        lock (mesh)
                        {
                            // удаляем мусор (скрытые вершины и ребра)
                            mesh.RemoveUnpassable();
                            // сохраняем меш в архивный файл
                            //succeeded = Patch_Astral_Quester_Core_Save.SaveMesh(zipFile, meshName, mesh);
                            succeeded = AstralAccessors.Quester.Core.SaveMesh(zipFile, meshName, mesh);
                        } 
                    }

                    if (succeeded)
                        Logger.Notify(string.Concat("The profile '", Path.GetFileName(profileName), "' updated:\n\r\t",
                                                        useExternalMeshFile ? externalMeshFileName + '\\' : string.Empty, meshName,
                                                        profileUpdated ? "\n\r\tprofile.xml" : string.Empty));
                    else Logger.Notify(string.Concat("Fail to save changes to the profile '", Path.GetFileName(profileName), '\''), true);
                }
                catch (Exception exc)
                {
                    Logger.WriteLine(exc.ToString());
                    Logger.Notify(exc.ToString(), true);
                }
                finally
                {
                    zipFile?.Dispose();
                }
            }
        }

        /// <summary>
        /// Импорт мешей карты (графа путей) из Игры
        /// </summary>
        private void handler_ImportCurrentMapMeshesFromGame(object sender, ItemClickEventArgs e)
        {
            handler_Mapping_Stop();
            ResetToolState();

            Graph graph = AstralAccessors.Quester.Core.Meshes;
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directories.ProfilesPath,
                DefaultExt = "amp.zip",
                Filter = @"Astral mission profile (*.amp.zip)|*.amp.zip|(*.mesh.zip)|*.mesh.zip"
            };
            
            if (openFileDialog.ShowDialog() != DialogResult.OK 
                || !File.Exists(openFileDialog.FileName)) 
                return;

            try
            {
                using (ZipArchive profile = ZipFile.Open(openFileDialog.FileName, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry currentMapEntry = profile.GetEntry(currentMapMeshesName);
                    if(currentMapEntry is null)
                    {
                        XtraMessageBox.Show("Selected file doesn't contain current map meshes!");
                        return;
                    }
                    var mapsMeshes = AstralAccessors.Quester.Core.MapsMeshes;

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
            var graph = AstralAccessors.Quester.Core.Meshes;
            if (graph?.NodesCount > 0 && XtraMessageBox.Show(this, "Are you sure to delete all map nodes ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                handler_Mapping_Stop();
                ResetToolState();

                _currentMapHash = 0;
                graph.Clear();
            }
        }

        /// <summary>
        /// Оценка графа на наличие мусора
        /// </summary>
        private void handler_MeshesInfo(object sender, ItemClickEventArgs e)
        {
            var graph = AstralAccessors.Quester.Core.Meshes;
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
            var graph = AstralAccessors.Quester.Core.Meshes;

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

        private void handler_Mapping_BidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingBidirectional.Checked)
            {
                _mappingTool.MappingMode = MappingMode.Bidirectional;
                InterruptAllModifications(MapperEditMode.Mapping);
            }
        }

        private void handler_Mapping_UnidirectionalPath(object sender, ItemClickEventArgs e)
        {
            if (btnMappingUnidirectional.Checked)
            {
                _mappingTool.MappingMode = MappingMode.Unidirectional;
                InterruptAllModifications(MapperEditMode.Mapping);
            }
        }

        private void handler_Mapping_Stop(object sender = null, ItemClickEventArgs e = null)
        {
            if (btnMappingStop.Checked)
            {
                _mappingTool.MappingMode = MappingMode.Stoped;
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
    }
#endif
}
