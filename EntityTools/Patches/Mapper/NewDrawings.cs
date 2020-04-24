using AStar;
using Astral;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EntityTools.Patches.Mapper
{
#if DEVELOPER
    public partial class NewDrawings : Form
    {
        /// <summary>
        /// Функтор доступа к графу 
        /// </summary>
        private static readonly StaticPropertyAccessor<AStar.Graph> CoreCurrentMapMeshes = typeof(Astral.Quester.Core).GetStaticProperty<AStar.Graph>("Meshes");

        public NewDrawings()
        {
            InitializeComponent();

            graphicsNW = new GraphicsNW(Math.Max(600, mapBox.ClientSize.Width), Math.Max(600, mapBox.ClientSize.Height));
        }

        private void eventRefreshMapBox(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            drawMap();
        }

    #region Drawings
        private GraphicsNW graphicsNW = null;

        public double Zoom
        {
            get
            {
                return 1d;
                //switch (menuZoom.EditValue)
                //{
                //    case 0:
                //        return 0.2;
                //    case 1:
                //        return 0.3;
                //    case 2:
                //        return 0.4;
                //    case 3:
                //        return 0.5;
                //    case 4:
                //        return 0.7;
                //    case 5:
                //        return 1.0;
                //    case 6:
                //        return 1.2;
                //    case 7:
                //        return 1.5;
                //    case 8:
                //        return 1.8;
                //    case 9:
                //        return 2.2;
                //    case 10:
                //        return 2.5;
                //    case 11:
                //        return 2.7;
                //    case 12:
                //        return 3.0;
                //    case 13:
                //        return 3.4;
                //    case 14:
                //        return 3.7;
                //    case 15:
                //        return 4.0;
                //    case 16:
                //        return 4.5;
                //    case 17:
                //        return 5.0;
                //    case 18:
                //        return 5.5;
                //    case 19:
                //        return 6.0;
                //    case 20:
                //        return 8.0;
                //    default:
                //        return 1.0;
                //}
            }
        }

        private void drawMap()
        {
            try
            {
                GraphicsNW obj = this.graphicsNW;
                lock (obj)
                {
                    if (menuPlayerCenter.Checked)
                    {
                        graphicsNW.CenterPosition = EntityManager.LocalPlayer.Location;
                    }
                    this.graphicsNW.ImageWidth = Math.Max(mapBox.Width, 600);
                    this.graphicsNW.ImageHeight = Math.Max(mapBox.Height, 600);
                    this.graphicsNW.Zoom = Zoom;
                    this.graphicsNW.resetImage();
                    this.graphicsNW.drawMap();
                    int deleteNodeArea = Convert.ToInt32(Astral.API.CurrentSettings.DeleteNodeRadius * Zoom * 2.0);
                    Point imagePos = mapBox.PointToClient(Control.MousePosition);
                    this.graphicsNW.drawEllipse(imagePos, new Size(deleteNodeArea, deleteNodeArea), Pens.Beige);
                    uint containerId = EntityManager.LocalPlayer.ContainerId;

                    // Отрисовка персонажей
                    /*foreach (Entity entity in EntityManager.GetEntities())
                    {
                        if (entity.IsPlayer && entity.ContainerId != containerId)
                        {
                            this.graphicsNW.drawFillEllipse(entity.Location, new Size(6, 6), Brushes.AliceBlue);
                        }
                    }*/

                    // Отрисовка АОЕ
                    /*foreach (AOECheck.AOE aoe in AOECheck.List)
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
                            this.graphicsNW.drawFillEllipse(vector, (double)aoe.Radius, brush);
                        }
                    }*/

                    // Отрисовка Нодов
                    /*foreach (TargetableNode targetableNode in EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes)
                    {
                        if (targetableNode.Categories.Contains("Loot"))
                        {
                            this.graphicsNW.drawFillEllipse(targetableNode.WorldInteractionNode.Location, new Size(6, 6), Brushes.Gold);
                        }
                    }*/

                    //Roles.CurrentRole.OnMapDraw(this.graphicsNW);

                    // Отрисовка графа путей
                    DrawMeshes(graphicsNW, CoreCurrentMapMeshes);

                    // Отрисовка костров
                    /*foreach (MinimapWaypoint minimapWaypoint in Class1.LocalPlayer.Player.MissionInfo.Waypoints.OrderBy(new Func<MinimapWaypoint, double>(Mapper.<> c.<> 9.method_0)))
                    {
                        if (minimapWaypoint.IsCampFire)
                        {
                            this.graphicsNW.drawFillEllipse(minimapWaypoint.Position, new Size(6, 6), Brushes.DarkOrange);
                        }
                    }*/
                    /*if (this.vector3_0 != null)
                    {
                        this.graphicsNW.drawFillEllipse(this.vector3_0, new Size(7, 7), Brushes.Orange);
                    }*/

                    // Отисовка делегатом
                    /*if (this.CustomDraw != null)
                    {
                        this.CustomDraw(this.graphicsNW);
                    }*/

                    // Отрисовка уклонений
                    /*if (Astral.Logic.NW.Movements.LastValidPoses != null && !Astral.Logic.NW.Movements.lastvlidposto.IsTimedOut)
                    {
                        foreach (Astral.Logic.NW.Movements.DodgeLosTestResult dodgeLosTestResult in Astral.Logic.NW.Movements.LastValidPoses)
                        {
                            if (dodgeLosTestResult.Collided)
                            {
                                this.graphicsNW.drawFillEllipse(dodgeLosTestResult.CollidePos, new Size(4, 4), Brushes.Orange);
                            }
                            else
                            {
                                this.graphicsNW.drawFillEllipse(dodgeLosTestResult.TestedPos, new Size(4, 4), Brushes.Green);
                            }
                        }
                    }*/

                    // Отрисовка стрелки персонажа
                    // Вычисление угла направления персонажа
                    float angle = EntityManager.LocalPlayer.Yaw * 180f / 3.14159274f;
                    Bitmap image = RotateImage(Properties.Resources.charArrow, angle);
                    this.graphicsNW.drawImage(EntityManager.LocalPlayer.Location, image, default(Size));

                    // Отрисовка конечной точки пути
                    //this.graphicsNW.drawEllipse(Navigator.GetDestination, new Size(10, 10), Pens.Red);
                }
                mapBox.BackgroundImage = graphicsNW.getImage();

            }
            /*catch (ObjectDisposedException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }*/
            catch (Exception ex)
            {
                ETLogger.WriteLine(LogType.Debug, "Error in map thread :\r\n" + ex.ToString());
            }
            /*finally
            {
                Thread.Sleep(150);
            }*/
        }

        public static void DrawMeshes(GraphicsNW graph, Graph meshes)
        {
            object obj = meshes.Lock;
            lock (obj)
            {
                double num = graph.getWorldPos(new Point(graph.ImageWidth, graph.ImageHeight)).Distance2D(graph.CenterPosition);
                foreach (Arc arc in meshes.Arcs)
                {
                    if (arc.StartNode.Passable && arc.EndNode.Passable)
                    {
                        Vector3 startPos = new Vector3((float)arc.StartNode.X, (float)arc.StartNode.Y, (float)arc.StartNode.Z);
                        Vector3 endPos = new Vector3((float)arc.EndNode.X, (float)arc.EndNode.Y, (float)arc.EndNode.Z);
                        if (startPos.Distance2D(graph.CenterPosition) < num)
                        {
                            graph.drawLine(startPos, endPos, Pens.Red);
                        }
                    }
                }
                foreach (Node node in meshes.Nodes)
                {
                    if (node.Passable)
                    {
                        Brush brush = Brushes.Red;
                        int count = node.IncomingArcs.Count;
                        int count2 = node.OutgoingArcs.Count;
                        if (count == 1 && count2 == 1)
                        {
                            brush = Brushes.SkyBlue;
                        }
                        Vector3 nodePos = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
                        if (nodePos.Distance2D(graph.CenterPosition) < num)
                        {
                            graph.drawFillEllipse(nodePos, new Size(5, 5), brush);
                        }
                    }
                }
            }
        }

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
#endif
}
