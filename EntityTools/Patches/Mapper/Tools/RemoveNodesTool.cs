using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AStar;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент для удаления вершин
    /// </summary>
    public class RemoveNodesTool : IMapperTool
    {
        /// <summary>
        /// Список вершин, помеченных на удаление (Unpassable)
        /// </summary>
        readonly List<Node> deletedNodes = new List<Node>();

        public RemoveNodesTool() { }
        private RemoveNodesTool(IEnumerable<Node> nodes)
        {
            deletedNodes.AddRange(nodes);
            deletedNodes.ForEach(nd => nd.Passable = false);
        }
        private RemoveNodesTool(Node node)
        {
            node.Passable = false;
            deletedNodes.Add(node);
        }

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => true;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.DeleteNodes;

        public bool HandleCustomDraw => true;
        /// <summary>
        /// Отрисовка удаляемых вершин
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (nodes.Count > 0)
            {
                foreach (Node node in nodes)
                    drawSelectedNode(graphics, node);
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


        /// <summary>
        /// Отрисовка выбранной вершины
        /// </summary>
        private static void drawSelectedNode(MapperGraphics graphics, Node node)
        {
            graphics.FillCircleCentered(Brushes.White, node.Position, 14);
            graphics.FillCircleCentered(Brushes.Red, node.Position);
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Delete
                && nodes.Count > 0)
            {

                undo = new RemoveNodesTool(nodes);
                nodes.Clear();
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button == MouseButtons.Right
                && Control.ModifierKeys == Keys.Alt)
            {
                // производим поиск вершины
                double minDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;
                Node node = graph.ClosestNodeOxyProjection(e.X, e.Y, minDistance);

                // удаление выбранной вершины
                if (node != null)
                {
                    node.Passable = false;

                    undo = new RemoveNodesTool(node);
                }
            }
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Applied => deletedNodes.Count > 0;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo()
        {
            if (deletedNodes.Count > 0)
            {
                deletedNodes.ForEach(nd => nd.Passable = true);
                deletedNodes.Clear();
            }
        }
    }
}