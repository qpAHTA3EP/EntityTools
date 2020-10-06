#define LOG

#if PATCH_ASTRAL
using Astral.Logic.NW;
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
using EntityTools.Patches.Mapper.Tools;
using System.Collections.Generic;

namespace EntityTools.Patches.Mapper
{
#if PATCH_ASTRAL
    public partial class MapperFormExt : XtraForm//, IMapperForm //*/Form
    {
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
        #endregion

#if true
        /// <summary>
        /// Кэш вершин графа путей
        /// </summary>
        //protected MapperGraphCache MappingCache;

#endif
        /// <summary>
        /// Инструмент для выделения вершин
        /// </summary>
        private readonly NodeSelectTool selectedNodes;

        private readonly Stack<IMapperTool> undoStack;

        /// <summary>
        /// Кэш-код текущей активной карты
        /// </summary>
        private int currentMapHash = 0;

        protected IMapperTool CurrentTool
        {
            get => currentTool;
            set
            {
                if (currentTool != null)
                {
                    if (currentTool.Applied)
                    {
                        currentTool.Unbind();
                        undoStack.Push(currentTool);
                    }
                }
                currentTool = value;
            }
        }
        private IMapperTool currentTool = null;

        #region Инициализация формы
        private MapperFormExt()
        {
            InitializeComponent();

#if Not_MonoMapper
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
#endif
            MouseWheel += handler_MouseWheel;

            barMainTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible;
            barEditMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;

            btnShowStatBar.Visible = !barStatus.Visible && !barMainTools.Visible && !barEditMeshes.Visible;

            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;

            selectedNodes = new NodeSelectTool(this);

            undoStack = new Stack<IMapperTool>();

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
        private void handler_FormLoad(object sender, EventArgs e)
        {
            Binds.RemoveShiftAction(Keys.M);

            barMainTools.Visible = EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible;
            barEditMeshes.Visible = EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible;
            barStatus.Visible = EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible;
            btnShowStatBar.Visible = !barStatus.Visible && !barMainTools.Visible && !barEditMeshes.Visible;

            Location = EntityTools.PluginSettings.Mapper.MapperForm.Location;
            Size = EntityTools.PluginSettings.Mapper.MapperForm.Size;
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// СОбытие при закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FormClose(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();

            EntityTools.PluginSettings.Mapper.MapperForm.MainToolsBarVisible = barMainTools.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.EditMeshesBarVisible = barEditMeshes.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.StatusBarVisible = barStatus.Visible;
            EntityTools.PluginSettings.Mapper.MapperForm.Location = Location;
            EntityTools.PluginSettings.Mapper.MapperForm.Size = Size;

            InterruptAllModifications();

            Binds.RemoveShiftAction(Keys.M);
        }

        /// <summary>
        /// Фоновый процесс обновления заголовка окна строки состояния
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void work_FormStatusUpdate(object sender, DoWorkEventArgs e)
        {
            string formCaption = string.Empty,
                   posStr = string.Empty;
            //Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);

            var updateFormStatus = new System.Action(() =>
            {
                Text = formCaption;
                lblMousePos.Caption = posStr;
                using (Graphics.ReadLock())
                    MapPicture.Image = Graphics.getImage();
            });

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
                Vector3 pos = player.Location;
                posStr = !player.IsLoading ? $"{pos.X:N1} | {pos.Y:N1} | {pos.Z:N1}" : "Loading";

                int hash = AstralAccessors.Quester.Core.Meshes.Value.GetHashCode();
                if (currentMapHash != hash)
                {
                    var currentMapInfo = player.CurrentZoneMapInfo;
                    formCaption = formCaption = string.Concat(currentMapInfo.DisplayName, '[', currentMapInfo.MapName,']');
                    currentMapHash = hash;

                    // Карта изменилась - сбрасываем состояние инструментов
                    if (currentTool != null)
                    {
                        currentTool.Unbind();
                        currentTool = null;
                    }
                    selectedNodes.Clear();
                    undoStack.Clear();
                }

                DrawMapper();

                if (InvokeRequired)
                    Invoke(updateFormStatus);
                else
                {
                    Text = formCaption;
                    lblMousePos.Caption = posStr;
                    using (Graphics.ReadLock())
                        MapPicture.Image = Graphics.getImage();
                }

#if Non_MonoMapper
                Thread.Sleep(500); 
#else
                Thread.Sleep(EntityTools.PluginSettings.Mapper.MapperForm.RedrawMapperTimeout);
#endif
            }
        }
        #endregion


        #region IMapperForm
        public delegate void CustomMapperDraw(MapperFormExt sender, MapperGraphics graphics);
        public delegate void CustomMouseEvent(MapperFormExt sender, MapperGraphics graphics, MouseEventArgs e);
        public delegate void CustomKeyEvent(MapperFormExt sender, MapperGraphics graphics, KeyEventArgs e);

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

        private void handler_MouseClick(object sender, MouseEventArgs e)
        {
            OnMapperMouseClick?.Invoke(this, Graphics, e);
        }
        private void handler_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMapperMouseDoubleClick?.Invoke(this, Graphics, e);
        }
        private void handler_KeyUp(object sender, KeyEventArgs e)
        {
            OnMapperKeyUp?.Invoke(@this, Graphics, e);
        }

        #region Перемещение изображения
        private Point mouseClickPosition = new Point();
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

                //if (dx > 5 || dy > 5)
                {
                    MapLockOnPlayer = false;
                    Graphics.CenterPosition.X += (float)dx;
                    Graphics.CenterPosition.Y += (float)dy;

                    mouseClickPosition.X = e.X;
                    mouseClickPosition.Y = e.Y;
                }
            }
        }
        private void handler_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClickPosition.X = 0;
            mouseClickPosition.Y = 0;

            OnMapperMouseUp?.Invoke(@this, Graphics, e);
        }
        #endregion
        #endregion

        /// <summary>
        /// Прерывание всех операций по изменению графа путей (мешей)
        /// </summary>
        private void InterruptAllModifications(MapperEditMode mode = MapperEditMode.None)
        {
            if (mode != MapperEditMode.Mapping)
            {
                btnStopMapping.Checked = true;
                //handler_StopMapping();
            }

            if (mode != MapperEditMode.EditEdges)
            {
                btnEditEdges.Checked = false;
                //MapperHelper_EditEdges.Reset();
            }
            if (mode != MapperEditMode.RelocateNodes)
            {
                btnMoveNodes.Checked = false;
                //MapperHelper_MoveNodes.Reset();
            } 

            if (mode != MapperEditMode.DeleteNodes)
            {
                btnRemoveNodes.Checked = false;
                //MapperHelper_RemoveNodes.Reset();
            }

#if false
            if (mode != MapperEditMode.AddCustomRegion
                        && mode != MapperEditMode.EditCustomRegion)
            {
                handler_CancelCRManipulation(@this, null);
                //MapperHelper_CustomRegion.Reset();
            } 
#endif
        }

        #region Drawings
        protected MapperGraphics Graphics = new MapperGraphics(360, 360);

        /// <summary>
        /// Коэффициент масштабирования
        /// </summary>
        public double Zoom
        {
            get
            {
                int zoomPos = Convert.ToInt32(trackZoom.EditValue);
                if (zoomPos < 0)
                    zoomPos = 0;
                else if (zoomPos > 20)
                    zoomPos = 20;
                switch (zoomPos)
                {
                    case 0:
                        return 0.2;
                    case 1:
                        return 0.3;
                    case 2:
                        return 0.4;
                    case 3:
                        return 0.5;
                    case 4:
                        return 0.7;
                    case 5:
                        return 1.0;
                    case 6:
                        return 1.2;
                    case 7:
                        return 1.5;
                    case 8:
                        return 1.8;
                    case 9:
                        return 2.2;
                    case 10:
                        return 2.5;
                    case 11:
                        return 2.7;
                    case 12:
                        return 3.0;
                    case 13:
                        return 3.4;
                    case 14:
                        return 3.7;
                    case 15:
                        return 4.0;
                    case 16:
                        return 4.5;
                    case 17:
                        return 5.0;
                    case 18:
                        return 5.5;
                    case 19:
                        return 6.0;
                    case 20:
                        return 8.0;
                    default:
                        return 1.0;
                }
            }
        }

        /// <summary>
        /// Флаг фиксации персонажа в центре карты
        /// </summary>
        public bool MapLockOnPlayer
        {
            get => btnLockMapOnPlayer.Checked;
            set => btnLockMapOnPlayer.Checked = value;
        }

        /// <summary>
        /// Координаты центра отображаемой карты
        /// </summary>
        public Vector3 CenterOfMap
        {
            get
            {
                using (Graphics.ReadLock())
                {
                    return Graphics.CenterPosition.Clone();
                }
            }
            set
            {
                if (value != null)
                    using (Graphics.WriteLock())
                    {
                        Graphics.CenterPosition = value.Clone();
                    }
            }
        }

        /// <summary>
        /// Координаты курсора мыши, относительно формы <see cref="MapperFormExt"/>
        /// </summary>
        public Point RelativeMousePosition
        {
            get
            {
                return MapPicture.PointToClient(Control.MousePosition);
            }
        }

        /// <summary>
        /// Метод для фоновой отрисовки карты
        /// </summary>
        /// <param name="token"></param>
#if Not_MonoMapper
        private void work_DrawMap(CancellationToken token)
#else
        private void DrawMapper()
#endif
        {
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
#if Not_MonoMapper
            while (!IsDisposed && Visible && !token.IsCancellationRequested) 
#else
            if(!IsDisposed && Visible)
#endif
            {
                try
                {
                    if (MapPicture.Visible && MapPicture.Width > 0 && MapPicture.Height > 0)
                    {
                        using (Graphics.WriteLock())
                        {
                            if (MapLockOnPlayer)
                            {
                                Graphics.CenterPosition = EntityManager.LocalPlayer.Location;
                            }

                            int imgWidth = MapPicture.Width;
                            int imgHeight = MapPicture.Height;
                            Graphics.ImageWidth = imgWidth;
                            Graphics.ImageHeight = imgHeight;
                            Graphics.Zoom = Zoom;
                            Graphics.resetImage();

                            // Вычисляем координаты границ изображения
                            Graphics.GetWorldPosition(0, 0, out double leftBorder, out double topBorder);
                            Graphics.GetWorldPosition(imgWidth, imgHeight, out double rightBorder, out double downBorder);

                            Vector3 location = null;
                            float x, y;

            #region Отрисовка области удаления вершин
#if false
                            int nodeDeleteDiameter =
 Convert.ToInt32(Astral.Controllers.Settings.Get.DeleteNodeRadius * graphicsNW.Zoom * 2);
                            var imagePos = MapPicture.PointToClient(Control.MousePosition);
                            graphicsNW.drawEllipse(imagePos, new Size(nodeDeleteDiameter, nodeDeleteDiameter), Pens.Beige);
#endif
            #endregion

            #region Отрисовка нодов
                            foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                            {
                                location = targetableNode.WorldInteractionNode.Location;
                                x = location.X;
                                y = location.Y;
                                if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                                {
                                    if (targetableNode.Categories.Contains("Loot"))
                                        Graphics.FillCircleCentered(Brushes.Gold, location, 6);
                                    else Graphics.FillSquareCentered(Brushes.YellowGreen, location, 6);
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
                                        Graphics.FillSquareCentered(Brushes.DarkOrange, location, 12);
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
#if true
                            try
                            {
                                if (!AstralAccessors.Controllers.Roles.CurrentRole_OnMapDraw(Graphics))
                                    //lock (AstralAccessors.Quester.Core.Meshes.Value.SyncRoot) <- Блокировка графа есть в DrawMeshes(..)
                                    Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes.DrawMeshes(Graphics,
                                        AstralAccessors.Quester.Core.Meshes);
                            }
                            catch (Exception ex)
                            {
                                ETLogger.WriteLine(LogType.Error, string.Concat(nameof(DrawMapper), ": Перехвачено исключение \n\r", ex), true);
                                Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes.DrawMeshes(Graphics,
                                        AstralAccessors.Quester.Core.Meshes);
                            }
#else
                            DrawMeshes(graphicsNW, AstralAccessors.Quester.Core.Meshes);
#endif
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
                                        Graphics.FillRhombCentered(Brushes.LawnGreen, location, 10, 17);
                                    else if (!entity.IsDead && !entity.Critter.IsLootable)
                                    {
                                        var relationToPlayer = entity.RelationToPlayer;
                                        if (relationToPlayer == MyNW.Patchables.Enums.EntityRelation.Foe)
                                            Graphics.FillRhombCentered(Brushes.OrangeRed, location, 10, 10);
                                        else if (relationToPlayer == MyNW.Patchables.Enums.EntityRelation.Friend)
                                            Graphics.FillRhombCentered(Brushes.Green, location, 5, 8);
                                        else Graphics.FillRhombCentered(Brushes.LightGray, location, 5, 8);
                                    }
                                }
                            }
            #endregion

            #region Отрисовка специальной графики
                            OnMapperDraw?.Invoke(this, Graphics); 
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
                            {
                                Graphics.DrawCircleCentered(Pens.Red, location, 12);
                            } 
            #endregion

            #region Отрисовка персонажа
                            location = EntityManager.LocalPlayer.Location;
                            x = location.X;
                            y = location.Y;
                            if (leftBorder <= x && x <= rightBorder && downBorder <= y && y <= topBorder)
                            {
                                double angle = EntityManager.LocalPlayer.Yaw * 180 / Math.PI;
#if true
                                Bitmap image = MapperGraphicsHelper.RotateImage(Properties.Resources.charArrow, (float)angle);
                                //_graphics.FillCircleCentered(Brushes.DarkGoldenrod, EntityManager.LocalPlayer.Location, 15);
                                Graphics.drawImage(location, image);
#else
                            _graphics.FillCircleCentered(Brushes.DarkGoldenrod, EntityManager.LocalPlayer.Location, 15);
                            _graphics.DrawImageCentered(EntityManager.LocalPlayer.Location, Properties.Resources.charArrow, angle);
#endif
                            }
            #endregion
                        }

#if Not_MonoMapper
                        Astral.Controllers.Forms.InvokeOnMainThread(() =>
                                        {
                                            using (_graphics.ReadLock())
                                                MapPicture.Image = _graphics.getImage();
                                        }); 
#endif
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

#if Not_MonoMapper
        private Task mapDrawingTask = null;
        private CancellationTokenSource mapDrawingTokenSource = null;

        public void StartMapDrawings()
        {
            if (mapDrawingTask?.Status != TaskStatus.Running)
            {
                mapDrawingTokenSource?.Cancel();
                mapDrawingTokenSource = new CancellationTokenSource();
                CancellationToken token = mapDrawingTokenSource.Token;

                mapDrawingTask = Task.Run(() => work_DrawMap(token), token);
            }
        }

        public void StopMapDrawings()
        {
            mapDrawingTokenSource?.Cancel();
        } 
#endif
        #endregion

        #region Mapping
#if false

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
            MappingCache?.StopCache();
        }

        /// <summary>
        /// Прокладывание пути
        /// </summary>
        private void work_Mapping(CancellationToken token)
        {
            try
            {
#if Not_MonoMapper
                if (graphCache == null)
                    graphCache = new MapperGraphCache(mapper);
                else graphCache.StartCache(mapper); 
#else
                throw new NotImplementedException();
#endif

#if PROFILING && DEBUG
                AddNavigationNodeChached.ResetWatch();
#endif
                if (MappingCache.NodesCount != 0)
                {
                    if (LinearPath)
                        lastNodeDetail = MapperHelper_AddNodes.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), MappingCache);
                    else lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), MappingCache);
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
                                        lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), MappingCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), MappingCache, null, false) ?? lastNodeDetail;
                                }
                                else
                                {
                                    // Строим комплексный (многосвязный путь)
                                    if (ForceLinkLastWaypoint)
                                        lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), MappingCache, lastNodeDetail, false) ?? lastNodeDetail;
                                    else lastNodeDetail = MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), MappingCache, null, false) ?? lastNodeDetail;
                                }
                                break;
                            case MappingType.Unidirectional:
                                {
                                    lastNodeDetail = MapperHelper_AddNodes.LinkLast(EntityManager.LocalPlayer.Location.Clone(), MappingCache, lastNodeDetail, true) ?? lastNodeDetail;
                                }
                                break;
                        }
                        MappingCache.LastAddedNode = lastNodeDetail?.Node;
                    }
                    Thread.Sleep(100);
                }
                if (token.IsCancellationRequested)
                {
                    // Инициировано прерывание 
                    // Связываем текущее местоположение с графом
                    if (LinearPath)
                        MapperHelper_AddNodes.LinkNearest1(EntityManager.LocalPlayer.Location.Clone(), MappingCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    else MapperHelper_AddNodes.LinkNearest3Side(EntityManager.LocalPlayer.Location.Clone(), MappingCache, lastNodeDetail, MappingType == MappingType.Unidirectional);
                    lastNodeDetail = null;
                }
            }
            catch (Exception ex)
            {
#if PROFILING && DEBUG
                AddNavigationNodeStatic.LogWatch();
#endif
#if LOG && DEBUG
                ETLogger.WriteLine(LogType.Debug, $"MapperExtWithCache:: Graph Nodes: {MappingCache.FullGraph.NodesCount}");
#endif
                ETLogger.WriteLine(ex.ToString());
            }
        } 
#else
        private void handler_StopMapping(object sender = null, ItemClickEventArgs e = null)
        {
        }
#endif
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
#if false
            editMode = MapperEditMode.EditCustomRegion;
            InterruptAllModifications(MapperEditMode.EditCustomRegion);

            btnLockMapOnPlayer.Checked = false;

            barCustomRegion.FloatLocation = new Point(Location.X + 40, Location.Y + 60);
            barCustomRegion.DockStyle = BarDockStyle.None;
            barCustomRegion.Visible = true;
            barEditCustomRegion.Visible = false;
#if Not_MonoMapper
            MapperHelper_CustomRegion.BeginAdd(mapper, false); 
#endif
#endif
        }

        /// <summary>
        /// Запуск процедуры добавления элиптического региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_AddElipticalCR(object sender, ItemClickEventArgs e)
        {
#if false
            editMode = MapperEditMode.EditCustomRegion;
            InterruptAllModifications(MapperEditMode.EditCustomRegion);

            btnLockMapOnPlayer.Checked = false;

            barCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 40, Location.Y + MapPicture.Location.Y + 60);
            barCustomRegion.DockStyle = BarDockStyle.None;
            barCustomRegion.Visible = true;
            barEditCustomRegion.Visible = false;
#if Not_MonoMapper
            MapperHelper_CustomRegion.BeginAdd(mapper, true); 
#endif
#endif
        }

        /// <summary>
        /// Завершение процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_FinishCRAddition(object sender, ItemClickEventArgs e)
        {
#if false
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
#endif
        }

        /// <summary>
        /// Выбор и редактирование существующего CustomRegion'а
        /// </summary>
        private void handler_EditCR(object sender, ItemClickEventArgs e)
        {

#if false
            if (btnEditCR.Checked)
            {
                if (Astral.Quester.API.CurrentProfile.CustomRegions?.Count > 0)
                {
                    editMode = MapperEditMode.EditCustomRegion;
                    InterruptAllModifications(MapperEditMode.EditCustomRegion);

                    btnLockMapOnPlayer.Checked = false;

                    // Привязываем список CustomRegion'ов
                    if (!ReferenceEquals(listCRSelector.DataSource, Astral.Quester.API.CurrentProfile.CustomRegions))
                    {
                        listCRSelector.DataSource = Astral.Quester.API.CurrentProfile.CustomRegions;
                        listCRSelector.ValueMember = "Name";
                        listCRSelector.DisplayMember = "Name";
                        editCRSelector.EditValue = Astral.Quester.API.CurrentProfile.CustomRegions.First();
                    }

                    btnRenameCR.Checked = false;

                    barEditCustomRegion.FloatLocation = new Point(Location.X + MapPicture.Location.X + 40,
                        Location.Y + MapPicture.Location.Y + 60);
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
#endif
        }

        /// <summary>
        /// Прерывание процедуры добавления региона
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_CancelCRManipulation(object sender, ItemClickEventArgs e)
        {
#if false
            editMode = MapperEditMode.None;
            MapperHelper_CustomRegion.Reset();

            barCustomRegion.Visible = false;
            barEditCustomRegion.Visible = false;
            btnEditCR.Checked = false; 
#endif
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

        private void handler_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = 0;
            if(e.Delta > 0)
                delta = 1;
            else if(e.Delta < 0)
                delta = -1;
            trackZoom.EditValue = Convert.ToInt32(trackZoom.EditValue) + delta;
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
                    if (CurrentProfileNeedSave)
                    {
                        // Открываем архивный файл профиля
                        zipFile = ZipFile.Open(profileName, File.Exists(profileName) ? ZipArchiveMode.Update : ZipArchiveMode.Create);

                        // Сохраняем в архив файл профиля "profile.xml"
                        lock (currentProfile)
                        {
                            Patch_Astral_Quester_Core_Save.SaveProfile(zipFile);
                            currentProfile.Saved = true; 
                        }

                        CurrentProfileNeedSave = false;
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

                    lock (mesh)
                    {
                        // удаляем мусор (скрытые вершины и ребра)
                        mesh.RemoveUnpassable();
                        // сохраняем меш в архивный файл
                        Patch_Astral_Quester_Core_Save.SaveMesh(zipFile, meshName, mesh); 
                    }

                    XtraMessageBox.Show(string.Concat("The profile '", Path.GetFileName(profileName), "' updated:\n\r\t",
                                                        useExternalMeshFile ? externalMeshFileName +'\\' : string.Empty, meshName,
                                                        profileUpdated ? "\n\r\tprofile.xml" : string.Empty));
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handler_ImportCurrentMapMeshesFromGame(object sender, ItemClickEventArgs e)
        {
            //if (((Graph)CoreCurrentMapMeshes)?.NodesCollection.Count == 0
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
#if false
            if (btnEditEdges.Checked)
            {
                editMode = MapperEditMode.EditEdges;
                InterruptAllModifications(MapperEditMode.EditEdges);

                btnLockMapOnPlayer.Checked = false;

#if Not_MonoMapper
                MapperHelper_EditEdges.Initialize(mapper); 
#endif
            }
            else MapperHelper_EditEdges.Reset(); 
#endif
        }

        private void handler_RelocateNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
#if false
            if (btnMoveNodes.Checked)
            {
                editMode = MapperEditMode.RelocateNodes;
                InterruptAllModifications(MapperEditMode.RelocateNodes);

                btnLockMapOnPlayer.Checked = false;

#if Not_MonoMapper
                MapperHelper_MoveNodes.Initialize(mapper); 
#endif
            }
            else MapperHelper_MoveNodes.Reset(); 
#endif
        }

        private void handler_DeleteNodes_ModeChanged(object sender, ItemClickEventArgs e)
        {
            RemoveNodesTool removeTool = CurrentTool as RemoveNodesTool;
            if (btnRemoveNodes.Checked)
            {
                if (removeTool is null)
                    CurrentTool = new RemoveNodesTool(this);
                else removeTool.BindTo(@this);
            }
            else
            {
                if (removeTool != null)
                {
                    removeTool.Unbind();
                    CurrentTool = null;
                }
            }
#if false
            if (btnRemoveNodes.Checked)
            {
                editMode = MapperEditMode.DeleteNodes;
                InterruptAllModifications(MapperEditMode.DeleteNodes);

                MappingCache.StartCache();

                btnLockMapOnPlayer.Checked = false;

#if Not_MonoMapper
                MapperHelper_RemoveNodes.Initialize(mapper); 
#endif
            }
            else MapperHelper_RemoveNodes.Reset(); 
#endif
        }

        private void handler_Undo(object sender, ItemClickEventArgs e)
        {
            if(currentTool != null
                && currentTool.Applied)
            {
                currentTool.Undo(this);
                return;
            }
            if (undoStack.Count > 0)
            {
#if false
                InterruptAllModifications(); 
#endif
                var tool = undoStack.Pop();
                tool.Undo(this);
            }
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

            if (((Graph)AstralAccessors.Quester.Core.Meshes).RemoveUnpassable() > 0)
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
