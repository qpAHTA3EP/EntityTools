using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using AStar;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмет перемещения вершин
    /// </summary>
    public class RelocateNodesTool : IMapperTool
    {
        /// <summary>
        /// Список перемещенных вершин
        /// </summary>
        List<Node> movedNodes = new List<Node>();

        /// <summary>
        /// Смещение координат
        /// </summary>
        double dX, dY;

        public RelocateNodesTool() { }
        private RelocateNodesTool(IEnumerable<Node> nodes, double dx, double dy)
        {
            dX = dx;
            dY = dy;
            movedNodes.AddRange(nodes);
            movedNodes.ForEach(nd => 
            {
                var pos = nd.Position;
                nd.Position = new Point3D(pos.X + dx, pos.Y + dy, pos.Z);
            });
        }
        private RelocateNodesTool(Node node)
        {
            node.Passable = false;
            movedNodes.Add(node);
        }

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => true;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.RelocateNodes;

        public bool HandleCustomDraw => true;
        /// <summary>
        /// Отрисовка удаляемых вершин
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (nodes.Count > 0)
            {
                bool needDrawNewPos = !((Control.ModifierKeys & Keys.Shift) == Keys.Shift
                    || (Control.ModifierKeys & Keys.Control) == Keys.Control);

                Node baseNode = nodes.Last;

                // Вычисляем смещение
                double dx = worldMouseX - baseNode.X;
                double dy = worldMouseY - baseNode.Y;

                if (needDrawNewPos)
                {       // Отрисовываем выбранные вершины и их новое местоположение
                    foreach (Node node in nodes)
                    {
                        drawNewEdgePos(graphics, nodes, node, dx, dy);
#if false
                        drawSelectedNode(graphics, node);
                        drawNewNodePos(graphics, node, dx, dy);
                    }
#else
                    }
                    foreach (Node node in nodes)
                    {
                        drawSelectedNode(graphics, node);
                        drawNewNodePos(graphics, node, dx, dy);
                    }
#endif
                }
                else // Отрисовываем только выбранные вершины
                    foreach (Node node in nodes)
                        drawSelectedNode(graphics, node);

            }
        }

        /// <summary>
        /// Отрисовка выбранной вершины в исходном пложени
        /// </summary>
        private static void drawSelectedNode(MapperGraphics graphics, Node node)
        {
            graphics.FillCircleCentered(Brushes.Red, node.Position, 14);
            graphics.FillCircleCentered(Brushes.Orange, node.Position, 8);
        }
        /// <summary>
        /// Отрисовка нового положения вершины
        /// </summary>
        private static void drawNewNodePos(MapperGraphics graphics, Node node, double dx, double dy)
        {
            graphics.FillCircleCentered(Brushes.Orange, node.X + dx, node.Y + dy, 16);
            graphics.FillCircleCentered(Brushes.Red, node.X + dx, node.Y + dy, 10);
        }

        /// <summary>
        /// Отрисовка нового положения ребер
        /// </summary>
        private static void drawNewEdgePos(MapperGraphics graphics, IEnumerable<Node> nodes, Node node, double dx, double dy)
        {
            double x1 = node.X + dx,
                y1 = node.Y + dy;

            foreach (Arc arc in node.IncomingArcs)
            {
                if (arc.StartNode.Passable)
                {
                    var startNodePos = arc.StartNode.Position;
                    if (nodes.Contains(arc.StartNode))
                    {
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
                    if (nodes.Contains(arc.EndNode))
                    {
                        double x2 = endNodePos.X + dx,
                                y2 = endNodePos.Y + dy;
                        graphics.DrawLine(Pens.Orange, x1, y1, x2, y2);
                    }
                    else graphics.DrawLine(Pens.Orange, x1, y1, endNodePos.X, endNodePos.Y);
                }
            }
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Enter
                && (Control.ModifierKeys & Keys.Shift) == 0
                && (Control.ModifierKeys & Keys.Control) == 0
                && nodes.Count > 0)
            {
                // перемещение группу вершин
                Node baseNode = nodes.Last;

                double dx = worldMouseX - baseNode.X;
                double dy = worldMouseY - baseNode.Y;

                MoveNodes(nodes, dx, dy, out undo);
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button == MouseButtons.Right
                && (Control.ModifierKeys & Keys.Shift) == 0
                && (Control.ModifierKeys & Keys.Control) == 0
                && nodes.Count > 0)
            {
                // перемещение группу вершин
                Node baseNode = nodes.Last;

                double dx = e.X - baseNode.X;
                double dy = e.Y - baseNode.Y;

                MoveNodes(nodes, dx, dy, out undo);
            }
        }

        /// <summary>
        /// Перемещение вершин, запоминание состояния для отката действия
        /// </summary>
        public void MoveNodes(NodeSelectTool nodes, double dx, double dy, out IMapperTool undo)
        {
            undo = null;
            if (nodes.Count > 0)
            {
                if (movedNodes.Count > 0)
                {
                    undo = new RelocateNodesTool { movedNodes = this.movedNodes, dX = this.dX, dY = this.dY };
                    movedNodes = new List<Node>(nodes);

                }
                else movedNodes.AddRange(nodes);
                movedNodes.ForEach(nd => nd.Move(dx, dy, 0));
                dX = dx;
                dY = dy;
            }
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Applied => movedNodes.Count > 0;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo()
        {
            if (movedNodes.Count > 0)
            {
                movedNodes.ForEach(nd => nd.Move(-dX, -dY, 0));
                movedNodes.Clear();
            }
        }
    }
}