using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using AStar;

using DevExpress.Utils;
using DevExpress.XtraEditors;

using EntityTools.Forms;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Quester.Mapper.Tools
{
    /// <summary>
    /// Инструмент добавления новой вершины
    /// </summary>
    public class AddNodeTool : IMapperTool
    {
        public AddNodeTool() { }

        protected AddNodeTool(Node nd)
        {
            if (nd is null)
                throw new ArgumentNullException(nameof(nd));
            newNode = nd;
        }

        /// <summary>
        /// Новая добавляемая вершина
        /// </summary>
        private Node newNode;
        private readonly LinkedList<Node> selectedNodes = new LinkedList<Node>();

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => false;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.AddNode;

        public bool HandleCustomDraw => true;
        /// <summary>
        /// Отрисовка новой вершин
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool _, double worldMouseX, double worldMouseY)
        {
            var aquaPen = Pens.Aqua;
            if (newNode != null)
            {
                var blueBrush = Brushes.DodgerBlue;
                var aquaBrush = Brushes.Aqua;

                var pos = newNode.Position;
                var x = pos.X;
                var y = pos.Y;
                
                foreach (var nd in selectedNodes)
                {
                    pos = nd.Position;
                    graphics.DrawLine(aquaPen, x, y, pos.X, pos.Y);
                }
                //отрисовка новой вершины
                graphics.FillCircleCentered(aquaBrush, x, y, 16);
                graphics.FillCircleCentered(blueBrush, x, y, 10);

                graphics.DrawCircleCentered(aquaPen, x, y, EntityTools.Config.Mapper.WaypointDistance);
            }
            else
            {
                graphics.DrawCircleCentered(aquaPen, worldMouseX, worldMouseY, EntityTools.Config.Mapper.WaypointDistance);
            }
        }

        /// <summary>
        /// Специальный курсор мыши
        /// </summary>
        public bool CustomMouseCursor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
        {
            text = string.Empty;
            textAlignment = Alignment.None;
            font = Control.DefaultFont;
            brush = Brushes.White;

            return false;
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool _, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape)
            {
                if (selectedNodes.Count > 0)
                {
                    selectedNodes.Clear();
                }
                else newNode = null;
            }
            else if (e.KeyCode == Keys.Enter
            && newNode != null)
            {
                double z = 0;
                // Перед добавлением новой вершины необходимо определить для неё координату Z.
                if (selectedNodes?.Count > 0)
                {
                    // Вычисляем среднее значение Z для новой вершины
                    foreach (var nd in selectedNodes)
                    {
                        z += nd.Position.Z;
                        newNode.ConnectTo(nd,1, out var arc);
                        nd.ConnectTo(newNode, 1, out arc);
                    }
                    z /= selectedNodes.Count;
                }
                else
                {
                    var player = EntityManager.LocalPlayer;
                    // Производим поиск ближайшей путевой вершины для определения Z
                    var goldGraph = Astral.Logic.NW.GoldenPath.GetCurrentMapGraphFromCache();
                    z = player.Location.Z;
                    //Node closestNd = goldGraph.ClosestNode(pos.X, pos.Y, z, out double dist, false);
                    double x = newNode.X,
                           y = newNode.Y,
                           dist2 = Math.Pow(EntityTools.Config.Mapper.WaypointDistance, 2),
                           sumZ = 0;
                    int cnt = 0;
#if true
                    foreach (var nd in goldGraph.NodesCollection)
                    {
                        var d2 = MapperHelper.SquareDistance2D(x, y, nd.X, nd.Y);
                        if (d2 <= dist2)
                        {
                            cnt++;
                            sumZ += nd.Z;
                        }
                    }
                    if (cnt > 0)
                    {
                        z = sumZ / cnt;
                        goto addingNewNode;
                    }
#else
                    var closestNodes = goldGraph.NodesCollection.Where(nd => MapperHelper.SquareDistance2D(x, y, nd.X, nd.Y) <= dist2);
                    if (closestNodes.Any())
                    {
                        z = closestNodes.Average(nd => nd.Z);
                        goto addingNewNode;
                    } 
#endif

                    // Производим поиск ближайшего интерактивного нода для определения Z
                    var newNdPos = (Vector3)newNode.Position;
                    double minDist = double.MaxValue;
                    Vector3 clsPos = null;
                    sumZ = 0;
                    cnt = 0;
                    foreach (var nd in player.Player.InteractStatus.TargetableNodes)
                    {
                        var loc = nd.WorldInteractionNode.Location;
                        var d2 = MapperHelper.SquareDistance2D(newNdPos, loc);
                        if (d2 < minDist)
                        {
                            clsPos = loc;
                            minDist = d2;
                        }
                        if (d2 < dist2)
                        {
                            cnt++;
                            sumZ += loc.Z;
                        }
                    }
                    if (cnt > 0)
                    {
                        z = sumZ / cnt;
                        goto addingNewNode;
                    }
                    if (clsPos != null && minDist <= dist2)
                    {
                        z = clsPos.Z;
                        goto addingNewNode;
                    }

                    // Производим поиск ближайшего Entity для определения Z
#if true
                    minDist = double.MinValue;
                    clsPos = null;
                    sumZ = 0;
                    cnt = 0;
                    foreach (var ett in EntityManager.GetEntities())
                    {
                        var loc = ett.Location;
                        var d2 = MapperHelper.SquareDistance2D(newNdPos, loc);
                        if (d2 < minDist)
                        {
                            clsPos = loc;
                            minDist = d2;
                        }
                        if (d2 < dist2)
                        {
                            cnt++;
                            sumZ += loc.Z;
                        }
                    }
                    if (cnt > 0)
                    {
                        z = sumZ / cnt;
                        goto addingNewNode;
                    }
                    if (clsPos != null && minDist <= dist2)
                    {
                        z = clsPos.Z;
                        goto addingNewNode;
                    }
#else
                    // Использование агрегатной функции и анонимного типа для поиска ближайшего Entity
                    var tar = EntityManager.GetEntities()
                        .Aggregate(new { ett = (Entity)null, minD = double.MaxValue }, (agr, et) =>
                         {
                             var d = MapperHelper.SquareDistance2D(newNdPos, et.Location);
                             if (d < agr.minD)
                             {
                                 return new { ett = et, minD = d };
                             }
                             return agr;
                         }); 
#endif

                    // Ориентиров для определения Z не найдено, поэтому предлагаем ввести вручную
                    string zStr = z.ToString();
                    if (!InputBox.EditValue(ref zStr,
                                            "Enter Z-coordinate for the new Node.\n" +
                                            $"Hint: No Waypoint, InteractNode or Entity found to determine Z-coordinate.\n" +
                                            $"The Player's Z-coordinate is used as default value.",
                                            "Z-coordinate", 
                                            new FormatInfo {FormatString = "N5", FormatType = FormatType.Numeric})
                        || !double.TryParse(zStr, out z))
                    {
                        XtraMessageBox.Show("You should enter the value of Z-coordinate for the new Node.",
                            "Warring", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // Добавляем новую вершину в граф
                addingNewNode:
                newNode.ChangeXYZ(newNode.X, newNode.Y, z);
                graph.AddNode(newNode);
                undo = new AddNodeTool(newNode);
                newNode = null;
                selectedNodes?.Clear();
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool _, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button != MouseButtons.Right) return;

            // Получаем игровые координаты, соответствующие координатам клика
            double mouseX = e.X;
            double mouseY = e.Y;

            if (newNode is null)
            {
                newNode = new Node(mouseX, mouseY, EntityManager.LocalPlayer.Z);
            }
            else
            {
                var nd = graph.ClosestNode(e.X, e.Y, EntityManager.LocalPlayer.Z, out var dist, false);
                if (nd != null && dist < EntityTools.Config.Mapper.WaypointDistance)
                {
                    selectedNodes.AddLast(nd);
                }
            }
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Applied => newNode != null && newNode.Passable;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo()
        {
            newNode.Passable = false;
        }
    }
}