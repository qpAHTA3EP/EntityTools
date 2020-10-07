using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AStar;
using EntityTools.Patches.Mapper.Tools;

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
#if false
        public RemoveNodesTool(MapperFormExt form)
        {
            BindTo(form);
        } 
#endif

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.DeleteNodes;

#if false
        /// <summary>
        /// Привязка (активация) инструмента к окну 
        /// </summary>
        /// <param name="form"></param>
        public void BindTo(MapperFormExt form)
        {
            if (!ReferenceEquals(mapper, form))
            {
                Unbind();
                mapper = form;
                if (mapper != null)
                {
                    mapper.OnMapperMouseClick += handler_RightMouseClick;
                    mapper.OnMapperKeyUp += handler_KeyUp;
                    mapper.OnMapperDraw += CustomDraw;
                }
            }
        }

        /// <summary>
        /// Отвязка (деактивация) инструмента
        /// </summary>
        public void Unbind()
        {
            if (mapper != null)
            {
                mapper.btnRemoveNodes.Checked = false;
                mapper.OnMapperMouseClick -= handler_RightMouseClick;
                mapper.OnMapperKeyUp -= handler_KeyUp;
                mapper.OnMapperDraw -= CustomDraw;
                mapper = null;
            }
        } 

        public void Dispose()
        {
            Unbind();
            deletedNodes.Clear();
        }
#endif

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
        /// Отрисовка выбранной вершины
        /// </summary>
        private static void drawSelectedNode(MapperGraphics graphics, Node node)
        {
            graphics.FillCircleCentered(Brushes.White, node.Position, 14);
            graphics.FillCircleCentered(Brushes.Red, node.Position, 8);
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
                double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
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

#if false
        public bool Apply()
        {
            if (mapper._selectedNodes.Count > 0)
            {
                deletedNodes.AddRange(mapper._selectedNodes);
                mapper._selectedNodes.Clear();
                foreach (var node in deletedNodes)
                    node.Passable = false;
                return true;
            }

            return false;
        } 
#endif

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