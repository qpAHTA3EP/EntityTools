using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AStar;
using Astral.Logic.Classes.Map;
using Astral.Quester.Classes;
using EntityTools.Enums;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using static EntityTools.Patches.Mapper.MapperGraphicsHelper;

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper_MoveNodes

    {
#if AstralMapper
        private static Astral.Forms.UserControls.Mapper mapper; 
#else
        private static MapperExt mapper;
#endif
        /// <summary>
        /// Группа перемещаемых вершин
        /// </summary>
        private static List<Node> nodes = new List<Node>();
        private static Vector3 startSelectRegionPos = Vector3.Empty;

        public static void Initialize(MapperExt m)
        {
#if false
            nodes.Clear();
            mapper = m;
            mapper.OnClick += handler_MapperClick;
            mapper.CustomDraw += handler_DrawSelectionOnMapper;
            mapper.OnMapperKeyUp += handler_KeyUp; 
#endif
        }

        private static void handler_DrawSelectionOnMapper(MapperGraphics graphics)
        {
#if false
            //Vector3 currentWorldPos = graphics.getWorldPos(mapper.RelativeMousePosition);
            graphics.GetMouseCursorWorldPosition(out double x, out double y);

            bool drawSelectRegion = startSelectRegionPos.IsValid && (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            bool drawNewPoses = !drawSelectRegion && (Control.ModifierKeys & Keys.Control) != Keys.Control && (Control.ModifierKeys & Keys.Shift) != Keys.Shift;

            if (nodes.Count > 0 && mapper != null)
            {
                Node baseNode = nodes.Last();

                double dx = x - baseNode.X;
                double dy = y - baseNode.Y;

                if (drawNewPoses)
                    foreach (Node node in nodes)
                        graphics.drawNewEdgePos(node, dx, dy);

                foreach (Node node in nodes)
                    graphics.drawSelectedNode(node);

                if (drawNewPoses)
                    foreach (Node node in nodes)
                        graphics.drawNewNodePos(node, dx, dy);
            }

            if (drawSelectRegion)
            {
                //Vector3 topLeft = new Vector3(Math.Min(startSelectRegionPos.X, currentWorldPos.X), Math.Max(startSelectRegionPos.Y, currentWorldPos.Y), 0f);
                //Vector3 downRight = new Vector3(Math.Max(startSelectRegionPos.X, currentWorldPos.X), Math.Min(startSelectRegionPos.Y, currentWorldPos.Y), 0f);

                //Vector3 selectRegionSize = new Vector3(downRight.X - topLeft.X, downRight.Y - topLeft.Y, 0f);

                graphics.DrawRectangle(Pens.PowderBlue, startSelectRegionPos.X, startSelectRegionPos.Y, x, y);
            } 
#endif
        }

        /// <summary>
        /// Отрисовка выбранной вершины
        /// </summary>
        /// <param name="node"></param>
        /// <param name="graphics"></param>
        private static void drawSelectedNode(this MapperGraphics graphics, Node node)
        {
            graphics.FillCircleCentered(Brushes.Red, node.Position, 14);
            graphics.FillCircleCentered(Brushes.Orange, node.Position, 8);
        }
        /// <summary>
        /// Нового положения вершины
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="graphics"></param>
        private static void drawNewNodePos(this MapperGraphics graphics, Node node, double dx, double dy)
        {
            graphics.FillCircleCentered(Brushes.Orange, node.X + dx, node.Y + dy, 16);
            graphics.FillCircleCentered(Brushes.Red, node.X + dx, node.Y + dy, 10);
        }

        private static void drawNewEdgePos(this MapperGraphics graphics, Node node, double dx, double dy)
        {
            //Vector3 newPos = new Vector3((float)node.X + dx, (float)node.Y + dy, 0f);
            double x1 = node.X + dx,
                y1 = node.Y + dy;

            foreach (Arc arc in node.IncomingArcs)
            {
                if (arc.StartNode.Passable)
                {
                    var startNodePos = arc.StartNode.Position;
                    if (nodes.IndexOf(arc.StartNode) >= 0)
                    {
                        //Vector3 startNodeNewPos = new Vector3((float)arc.StartNode.X + dx, (float)arc.StartNode.Y + dy, 0f);
                        double x2 = startNodePos.X + dx,
                               y2 = startNodePos.Y + dy;
                        graphics.DrawLine(Pens.Orange, x1, y1, x2, y2);
                    }
                    else graphics.DrawLine(Pens.Orange, x1, y1, startNodePos.X, startNodePos.Y);
                }
            }
            foreach (Arc arc in node.OutgoingArcs)
            {
                if (arc.EndNode.Passable)
                {
                    var endNodePos = arc.EndNode.Position;
                    if (nodes.IndexOf(arc.EndNode) >= 0)
                    {
                        //Vector3 endNodeNewPos = new Vector3((float)arc.EndNode.X + dx, (float)arc.EndNode.Y + dy, 0f);
                        double x2 = endNodePos.X + dx,
                               y2 = endNodePos.Y + dy;
                        graphics.DrawLine(Pens.Orange, x1, y1, x2, y2);
                    }
                    else graphics.DrawLine(Pens.Orange, x1, y1, endNodePos.X, endNodePos.Y);
                }
            }
        }

        /// <summary>
        /// Клик на Mapper'e используемый для выбора вершины и её позиционирования
        /// </summary>
        /// <param name="me"></param>
        /// <param name="graphics"></param>
        private static void handler_MapperClick(MouseEventArgs me, MapperGraphics graphics)
        {
            if (me.Button == MouseButtons.Right)
            {
                //Vector3 worldPos = graphics.getWorldPos(me.Location);
                graphics.GetWorldPosition(me.X, me.Y, out double worldPosX, out double worldPosY);

                if (Control.ModifierKeys == Keys.Shift)
                {
                    if (startSelectRegionPos.IsValid)
                    {
                        // Нажата клавиша Shift
                        // начинаем выделение регионом
                        //Vector3 endSelectRegionPos = graphics.getWorldPos(mapper.RelativeMousePosition);

                        //Vector3 downLeft = new Vector3(Math.Min(startSelectRegionPos.X, endSelectRegionPos.X), Math.Min(startSelectRegionPos.Y, endSelectRegionPos.Y), 0f);
                        //Vector3 topRight = new Vector3(Math.Max(startSelectRegionPos.X, endSelectRegionPos.X), Math.Max(startSelectRegionPos.Y, endSelectRegionPos.Y), 0f);

                        MapperGraphicsHelper.FixRange(worldPosX, startSelectRegionPos.X, out double left, out double right);
                        MapperGraphicsHelper.FixRange(worldPosX, startSelectRegionPos.Y, out double down, out double top);

                        // Выделяем все вершины, охваченные областью выделения и добавляем в группу 
                        //foreach (Node nd in AstralAccessors.Quester.Core.Meshes.Value.Nodes)
                        foreach (Node nd in graphics.VisibleGraph.NodesCollection)
                        {
                            if (nd.Passable
                                && left <= nd.X && nd.X <= right
                                && down <= nd.Y && nd.Y <= top)
                                    nodes.AddUnique(nd);
                        }

                        // Сбрасываем
                        startSelectRegionPos.X = 0;
                        startSelectRegionPos.Y = 0;
                        startSelectRegionPos.Z = 0;
                    }
                    else startSelectRegionPos = graphics.getWorldPos(mapper.RelativeMousePosition);

                }
                else
                {
#if false
                    Node node = null;
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    //foreach (Node nd in ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                    foreach (Node nd in graphics.Graph.NodesCollection)
                    {
                        if (nd.Passable)
                        {
                            var dist = MathHelper.Distance2D((float)nd.X, (float)nd.Y, worldPosX, worldPosY);
                            if (dist <= minDistance)
                            {
                                node = nd;
                                minDistance = dist;
                            }
                        }
                    } 
#else
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    Node node = graphics.VisibleGraph.ClosestNode(worldPosX, worldPosY, 0, out double distance);
                    if (distance > minDistance)
                        node = null;
#endif

                    if (Control.ModifierKeys == Keys.Control)
                    {
                        // добавление и удаление вершины из(в) группу "перемещаемых"
                        if (node != null)
                        {
                            int ind = nodes.IndexOf(node);
                            if (ind >= 0)
                                // Вершина входит в группу "выделенных" и на неё кликнули повторно
                                // то есть она подлежит удалению из группы
                                nodes.Remove(node);
                            else // Добавляем вершину в группу "выделенных"
                                nodes.Add(node);
                        }
                    }
                    else
                    {
                        if (node != null)
                        {
                            // сбрасываем выделение и выделяем одну вершину
                            nodes.Clear();
                            nodes.Add(node);
                        }
                        else if (nodes.Count > 0)
                        {
                            // перемещение группы вершин
                            Node baseNode = nodes.Last();

                            double dx = worldPosX - baseNode.X;
                            double dy = worldPosY - baseNode.Y;

                            foreach (Node nd in nodes)
                                nd.Position = new Point3D(nd.X + dx, nd.Y + dy, nd.Z);
                            nodes.Clear();
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Отмена выделения вершины при нажатии клавиши 'Escape'
        /// </summary>
        /// <param name="e"></param>
        /// <param name="graphics"></param>
        private static void handler_KeyUp(KeyEventArgs e, MapperGraphics graphics)
        {
#if false
            if (e.KeyCode == Keys.Enter)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    MouseEventArgs me = new MouseEventArgs(MouseButtons.Right, 1, Cursor.Position.X, Cursor.Position.Y, 0);
                    handler_MapperClick(me, graphics);
                }
                else
                {
                    // Отключаем выделение регионом
                    startSelectRegionPos.X = 0;
                    startSelectRegionPos.Y = 0;
                    startSelectRegionPos.Z = 0;

                    // перемещение группы вершин
                    Node baseNode = nodes.LastOrDefault();
                    if (baseNode != null)
                    {
                        //Vector3 worldPos = mapper.GraphicsNW.getWorldPos(mapper.RelativeMousePosition);
                        graphics.GetMouseCursorWorldPosition(out double worldPosX, out double worldPosY);

                        double dx = worldPosX - baseNode.X;
                        double dy = worldPosY - baseNode.Y;

                        foreach (Node nd in nodes)
                            nd.Position = new Point3D(nd.X + dx, nd.Y + dy, nd.Z);
                        nodes.Clear();
                    }
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {

                // Отключаем выделение регионом
                startSelectRegionPos.X = 0;
                startSelectRegionPos.Y = 0;
                startSelectRegionPos.Z = 0;

                // Сбрасываем выделение
                nodes.Clear();
            }
            else if (e.KeyCode == Keys.Shift)
            {
                startSelectRegionPos.X = 0;
                startSelectRegionPos.Y = 0;
                startSelectRegionPos.Z = 0;
            } 
#endif
        }

        /// <summary>
        /// Сброс всех сохраненных значений и привязок
        /// </summary>
        internal static void Reset()
        {
#if false
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.OnClick -= handler_MapperClick;
                mapper.CustomDraw -= handler_DrawSelectionOnMapper;
                mapper.OnMapperKeyUp -= handler_KeyUp;
                mapper = null;
            }

            nodes.Clear(); 
#endif
        }

    }
}
