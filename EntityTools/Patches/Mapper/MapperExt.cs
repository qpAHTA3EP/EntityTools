using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using AStar;
using Astral.Logic.Classes.Map;
using Astral.Logic.NW;
using DevExpress.XtraEditors;
using MyNW.Classes;
using MyNW.Internals;
using Astral;
using EntityTools.Reflection;
using DevExpress.XtraEditors.Filtering;
using static AStar.Tools.RWLocker;

//using Astral.Professions.Classes;

namespace EntityTools.Patches.Mapper
{
    public partial class MapperExt : XtraUserControl
    {
        public MapperExt()
        {
            InitializeComponent();

#if OnZoomChanged
            //MapPicture.MouseWheel += event_ZoomChanged;
            //MouseWheel += event_ZoomChanged;  
#endif
        }

#if OnZoomChanged
        private void event_ZoomChanged(object sender, MouseEventArgs e)
        {
            ZoomPos += e.Delta;
            OnZoomChanged?.Invoke(this, ZoomPos);
        } 
#endif

        /// <summary>
        /// Отображение на карте координат заданного объекта
        /// </summary>
        /// <param name="objectPos"></param>
        /// <param name="centerMapOnObject"></param>
        public void ShowObjectOnMap(Vector3 objectPos, bool centerMapOnObject = false) //method_0
        {
            try
            {
                if (centerMapOnObject)
                {
                    MapLockOnPlayer = false;
                    CenterOfMap = objectPos;
                    objectPosition = objectPos.Clone();
                }
            }
            catch
            {
                objectPosition = null;
            }
        }

        #region Events
        public delegate void MapperMouseEvent(MouseEventArgs e, GraphicsNW graphicsNW);
        public delegate void MapperKeyPressEvent(KeyPressEventArgs e, GraphicsNW graphicsNW);
        public delegate void MapperKeyEvent(KeyEventArgs e, GraphicsNW graphicsNW);

        /// <summary>
        /// Делегат для отрисовки пользовательской графики на карте
        /// </summary>
        public Action<GraphicsNW> CustomDraw { get; set; }

        /// <summary>
        /// Делегат для расширения реакции на событие OnClick
        /// </summary>
        public new Action<MouseEventArgs, GraphicsNW> OnClick { get; set; }

        /// <summary>
        /// Делегат, уведомляющий об отпускании клавиши
        /// </summary>
        public event MapperKeyEvent OnMapperKeyUp;

        /// <summary>
        /// Ltkt
        /// </summary>
        public event MapperKeyEvent OnMapperKeyDown;

        /// <summary>
        /// Делегат уведомляющий об изменении режима центровки карты
        /// </summary>
        public event CheckedChangedEventHandler OnMapLockOnPlayerChanged;

        /// <summary>
        /// Делегат, уведомляющий о нажатой алфавитно-цифровой клавише
        /// </summary>
        public event MapperKeyPressEvent OnMapperKeyPress;

        /// <summary>
        /// Делегат, уведомляющий о нажатии клавиши мыши
        /// </summary>
        public event MapperMouseEvent OnMapperMouseDown;

        /// <summary>
        /// Делегат, уведомляющий о нажатии клавиши мыши
        /// </summary>
        public event MapperMouseEvent OnMapperMouseUp;

#if OnZoomChanged
        /// <summary>
        /// Делегат, уведомляющий об изменении масштаба
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="zoomPos"></param>
        public delegate void ZoomChangedEventHandler(object sender, int zoomPos);
        public event ZoomChangedEventHandler OnZoomChanged; 
#endif
        #endregion

        #region Обработчики событий
        private void handler_Load(object sender, EventArgs e)
        {
            StartMapDrawings();
        }

        private void handler_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                StartMapDrawings();
            else StopMapDrawings();
        }

        private void handler_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (mousePointClickPosition == Point.Empty)
                        mousePointClickPosition = e.Location;
                    Vector3 vector = CenterOfMap.Clone();
                    vector.X -= (float)((e.Location.X - mousePointClickPosition.X) / Zoom);
                    vector.Y -= (float)((mousePointClickPosition.Y - e.Location.Y) / Zoom);
                    MapLockOnPlayer = false;
                    CenterOfMap = vector;
                    mousePointClickPosition = e.Location;
                }
            }
            catch
            {
            }
        }

        private void handler_MouseUp(object sender, MouseEventArgs e)
        {
            mousePointClickPosition = Point.Empty;
            if(OnMapperMouseUp != null)
                using (WriteLock())
                    OnMapperMouseUp(e, graphicsNW);
        }

        private void handler_MouseDown(object sender, MouseEventArgs e)
        {
            if(OnMapperMouseDown!=null)
                using (WriteLock())
                    OnMapperMouseDown(e, graphicsNW);
        }

        private void handler_MouseClick(object sender, MouseEventArgs e)
        {
            if (OnClick != null)
                using (WriteLock())
                    OnClick(e, graphicsNW);
        }

        private void handler_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Vector3 worldPos;
            using (ReadLock())
                worldPos = graphicsNW.getWorldPos(e.Location);

            try
            {
                if (AstralAccessors.Controllers.Roles.CurrentRole_Name == "Quester")
                {
                    lock (((Graph)AstralAccessors.Quester.Core.Meshes).SyncRoot)
                        AstralAccessors.Quester.Core.RemoveNodesFrom2DPosition(worldPos, Astral.Controllers.Settings.Get.DeleteNodeRadius);
                }
            }
            catch (Exception exc)
            {
                ETLogger.WriteLine(LogType.Error, string.Concat(exc.Message, Environment.NewLine, exc.StackTrace), true);
            }
        }

        private void handler_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(OnMapperKeyPress !=null)
                using (WriteLock())
                    OnMapperKeyPress(e, graphicsNW);
        }

        private void handler_KeyUp(object sender, KeyEventArgs e)
        {
            if(OnMapperKeyUp!=null)
                using(WriteLock())
                    OnMapperKeyUp(e, graphicsNW);
        }

        private void handler_KeyDown(object sender, KeyEventArgs e)
        {
            if(OnMapperKeyDown != null)
                using(WriteLock())
                    OnMapperKeyDown(e, graphicsNW);
        }
        #endregion

        #region Drawings
        public GraphicsNW GraphicsNW => graphicsNW;
        private GraphicsNW graphicsNW = new GraphicsNW(360, 360);

        #region ReaderWriterLocker
        /// <summary>
        /// Объект синхронизации доступа к объекту <see cref="graphicsNW"/>
        /// </summary>
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Блокировка объекта <see cref="graphicsNW"/>, допускающая одновременноа чтение использовать в конструкции <seealso cref="using"/>
        /// Использовать в конструкции <code>using(ReadLock())
        /// { do something with <see cref="graphicsNW"/> }</code>
        /// </summary>
        /// <returns></returns>
        public ReadLockToken ReadLock() => new ReadLockToken(@lock);
        /// <summary>
        /// Блокировка объекта <see cref="graphicsNW"/> для записи.
        /// Использовать в конструкции <code>using(WriteLock())
        /// { do something with <see cref="graphicsNW"/> }</code>
        /// </summary>
        /// <returns></returns>
        public WriteLockToken WriteLock() => new WriteLockToken(@lock);
        #endregion

        /// <summary>
        /// Преобразования шага масштобирования в коэффициент масштабирования
        /// </summary>
        /// <param name="zoomPos"></param>
        /// <returns></returns>
        public double ZoomCoefficient(int zoomPos)
        {
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

        /// <summary>
        /// Целочисленный шаг масштабирования 
        /// </summary>
        public int ZoomPos
        {
            get => _zoomPos;
            set
            {
                if (value < 0)
                    _zoomPos = 0;
                else if (value > 20)
                    _zoomPos = 20;
                else _zoomPos = value;
                _zoom = ZoomCoefficient(_zoomPos);
            }
        }
        private int _zoomPos = 5;

        /// <summary>
        /// Коэффициент масштабирования
        /// </summary>
        public double Zoom => _zoom;
        private double _zoom = 1;

        /// <summary>
        /// Флаг фиксации персонажа в центре карты
        /// </summary>
        public bool MapLockOnPlayer
        {
            get => _mapLockOnPlayer;
            set
            {
                _mapLockOnPlayer = value;
                OnMapLockOnPlayerChanged?.Invoke(this, new CheckedChangedEventArgs(value));
            }
        }
        private bool _mapLockOnPlayer = true;

        /// <summary>
        /// Координаты центра отображаемой карты
        /// </summary>
        public Vector3 CenterOfMap
        {
            get
            {
                using (ReadLock())
                {
                    return graphicsNW.CenterPosition.Clone();  
                }
            }
            set
            {
                if(value != null)
                    using (WriteLock())
                    {
                        graphicsNW.CenterPosition = value.Clone();
                    }
            }
        }

        /// <summary>
        /// Метод для фоновой отрисовки карты
        /// </summary>
        /// <param name="token"></param>
        private void work_DrawMap(CancellationToken token) // method_1
        {
            Astral.Classes.Timeout timeout = new Astral.Classes.Timeout(0);
            while (!IsDisposed && Visible && !token.IsCancellationRequested)
            {
                try
                {
                    if (MapPicture.Visible && MapPicture.Width > 0 && MapPicture.Height > 0)
                    {
                        using (WriteLock())
                        {
                            if (MapLockOnPlayer)
                            {
                                graphicsNW.CenterPosition = EntityManager.LocalPlayer.Location;
                            }

                            graphicsNW.ImageWidth = Math.Max(MapPicture.Width, 100);
                            graphicsNW.ImageHeight = Math.Max(MapPicture.Height, 100);
                            graphicsNW.Zoom = Zoom;
                            graphicsNW.resetImage();
                            graphicsNW.drawMap();

                            #region Отрисовка области удаления вершин

#if false
                            int nodeDeleteDiameter =
 Convert.ToInt32(Astral.Controllers.Settings.Get.DeleteNodeRadius * graphicsNW.Zoom * 2);
                            var imagePos = MapPicture.PointToClient(Control.MousePosition);
                            graphicsNW.drawEllipse(imagePos, new Size(nodeDeleteDiameter, nodeDeleteDiameter), Pens.Beige);
#endif

                            #endregion

                            #region Отрисовка персонажей

                            uint playerContainerId = EntityManager.LocalPlayer.ContainerId;
                            foreach (Entity entity in EntityManager.GetEntities())
                            {
                                if (entity.IsPlayer && entity.ContainerId != playerContainerId)
                                {
                                    graphicsNW.drawFillEllipse(entity.Location, new Size(6, 6), Brushes.AliceBlue);
                                }
                            }

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

                            #region Отрисовка нодов

                            foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus
                                .TargetableNodes)
                            {
                                if (targetableNode.Categories.Contains("Loot"))
                                {
                                    graphicsNW.drawFillEllipse(targetableNode.WorldInteractionNode.Location,
                                        new Size(6, 6), Brushes.Gold);
                                }
                            }

                            #endregion

                            // Отрисовка графики, связанной с выполняемой ролью 
#if true
                            //Roles.CurrentRole.OnMapDraw(graphicsNW_0);
                            if (!AstralAccessors.Controllers.Roles.CurrentRole_OnMapDraw(graphicsNW))
                                //lock (AstralAccessors.Quester.Core.Meshes.Value.SyncRoot) <- Блогировка графа есть в DrawMeshes(..)
                                Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes.DrawMeshes(graphicsNW,
                                    AstralAccessors.Quester.Core.Meshes);
#else
                            DrawMeshes(graphicsNW, AstralAccessors.Quester.Core.Meshes);
#endif

                            // Отрисовка костров
                            foreach (MinimapWaypoint minimapWaypoint in EntityManager.LocalPlayer.Player.MissionInfo
                                .Waypoints)
                                if (minimapWaypoint.IsCampFire)
                                    graphicsNW.drawFillEllipse(minimapWaypoint.Position, new Size(6, 6),
                                        Brushes.DarkOrange);


                            // Отрисовка специального объекта, заданного методом showObjectOnMap 
                            if (objectPosition != null && objectPosition.IsValid)
                                graphicsNW.drawFillEllipse(objectPosition, new Size(7, 7), Brushes.Orange);

                            // Отрисовка специальной графики
                            CustomDraw?.Invoke(graphicsNW);

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

                            #region Отрисовка персонажа

                            float angle = EntityManager.LocalPlayer.Yaw * 180f / 3.14159274f;
                            Bitmap image = RotateImage(Properties.Resources.charArrow, angle);
                            graphicsNW.drawImage(EntityManager.LocalPlayer.Location, image, default(Size));

                            #endregion

                            // Отрисовка точки назначения
                            graphicsNW.drawEllipse(Navigator.GetDestination, new Size(10, 10), Pens.Red);
                        }

                        Astral.Controllers.Forms.InvokeOnMainThread(() =>
                        {
                            using (ReadLock())
                                MapPicture.Image = graphicsNW.getImage();
                        });
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex.ToString());
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex.ToString());
                    break;
                }
                catch (ThreadAbortException ex)
                {
                    Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex.ToString());
                    break;
                }
                catch (Exception ex)
                {
                    if (timeout.IsTimedOut)
                    {
                        timeout.ChangeTime(2000);
                        Logger.WriteLine(Logger.LogType.Debug, "Error in map thread :\r\n" + ex.ToString());
                    }
                }
                finally
                {
                    Thread.Sleep(150);
                }
            }
        }

        public Point RelativeMousePosition => MapPicture.PointToClient(Control.MousePosition);

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

        /// <summary>
        /// Вращение изображения
        /// </summary>
        public static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            double w = (double)image.Width;
            double h = (double)image.Height;
            double num3;
            for (num3 = (double)angle * 3.1415926535897931 / 180.0; num3 < 0.0; num3 += 6.2831853071795862)
            {
            }
            double num4;
            double num5;
            double num6;
            double num7;
            if ((num3 >= 0.0 && num3 < 1.5707963267948966) || (num3 >= 3.1415926535897931 && num3 < 4.71238898038469))
            {
                num4 = Math.Abs(Math.Cos(num3)) * w;
                num5 = Math.Abs(Math.Sin(num3)) * w;
                num6 = Math.Abs(Math.Cos(num3)) * h;
                num7 = Math.Abs(Math.Sin(num3)) * h;
            }
            else
            {
                num4 = Math.Abs(Math.Sin(num3)) * h;
                num5 = Math.Abs(Math.Cos(num3)) * h;
                num6 = Math.Abs(Math.Sin(num3)) * w;
                num7 = Math.Abs(Math.Cos(num3)) * w;
            }
            double a = num4 + num7;
            double a2 = num6 + num5;
            int num8 = (int)Math.Ceiling(a);
            int num9 = (int)Math.Ceiling(a2);
            Bitmap bitmap = new Bitmap(num8, num9);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Point[] destPoints;
                if (num3 >= 0.0 && num3 < 1.5707963267948966)
                {
                    destPoints = new Point[]
                    {
                        new Point((int)num7, 0),
                        new Point(num8, (int)num5),
                        new Point(0, (int)num6)
                    };
                }
                else if (num3 >= 1.5707963267948966 && num3 < 3.1415926535897931)
                {
                    destPoints = new Point[]
                    {
                        new Point(num8, (int)num5),
                        new Point((int)num4, num9),
                        new Point((int)num7, 0)
                    };
                }
                else if (num3 >= 3.1415926535897931 && num3 < 4.71238898038469)
                {
                    destPoints = new Point[]
                    {
                        new Point((int)num4, num9),
                        new Point(0, (int)num6),
                        new Point(num8, (int)num5)
                    };
                }
                else
                {
                    destPoints = new Point[]
                    {
                        new Point(0, (int)num6),
                        new Point((int)num7, 0),
                        new Point((int)num4, num9)
                    };
                }
                graphics.DrawImage(image, destPoints);
            }
            return bitmap;
        }
        #endregion
    }
}

