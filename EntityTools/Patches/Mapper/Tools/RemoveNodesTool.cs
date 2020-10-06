using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AStar;
using EntityTools.Patches.Mapper.Tools;

namespace EntityTools.Patches.Mapper
{
    public partial class MapperFormExt
    {
        protected class RemoveNodesTool : IMapperTool
        {
            /// <summary>
            /// Список вершин, помеченных на удаление (Unpassable)
            /// </summary>
            readonly List<Node> deletedNodes = new List<Node>();

            MapperFormExt mapper;

            private RemoveNodesTool() { }
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
            public RemoveNodesTool(MapperFormExt form)
            {
                BindTo(form);
            }

            /// <summary>
            /// Режим редактирования
            /// </summary>
            public MapperEditMode EditMode => mapper is null ? MapperEditMode.None : MapperEditMode.DeleteNodes;

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

            /// <summary>
            /// Отрисовка удаляемых вершин
            /// </summary>
            public void CustomDraw(MapperFormExt form, MapperGraphics graphics)
            {
                if (form.selectedNodes.Count > 0)
                {
                    foreach (Node node in form.selectedNodes)
                        drawSelectedNode(form.Graphics, node);
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

            private void handler_KeyUp(MapperFormExt form, MapperGraphics graphics, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Delete
                    && form.selectedNodes.Count > 0)
                {
                    // Помечаем вершины "непроходимыми"
#if false
                    deletedNodes.AddRange(form.selectedNodes);
                    deletedNodes.ForEach(nd => nd.Passable = false);
                    form.selectedNodes.Clear(); 
#else
                    var deleteNodeTool = new RemoveNodesTool(form.selectedNodes);
                    form.selectedNodes.Clear();
                    form.undoStack.Push(deleteNodeTool);
#endif
                }
            }

            private void handler_RightMouseClick(MapperFormExt form, MapperGraphics graphics, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right
                    && ModifierKeys == Keys.Alt)
                {
                    // Получаем игровые координаты, соответствующие координатам клика
                    form.Graphics.GetWorldPosition(e.Location, out double worldX, out double worldY);

                    // производим поиск вершины
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    Node node = graphics.VisibleGraph.ClosestNodeOxyProjection(worldX, worldY, minDistance);

                    // удаление выбранной вершины
                    if (node != null)
                    {
                        node.Passable = false;
#if false
                        deletedNodes.Add(node); 
#else
                        var deleteNodeTool = new RemoveNodesTool(node);
                        form.undoStack.Push(deleteNodeTool);
#endif
                    }
                }
            }

            /// <summary>
            /// Указывает, что инструмент был применен
            /// </summary>
            public bool Applied => deletedNodes.Count > 0;

            public bool Apply()
            {
                if (mapper.selectedNodes.Count > 0)
                {
                    foreach (var node in mapper.selectedNodes)
                        node.Passable = false;
                    deletedNodes.AddRange(mapper.selectedNodes);
                    mapper.selectedNodes.Clear();
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Откат изменений, внесенных инструментом
            /// </summary>
            public void Undo(MapperFormExt mapper)
            {
                if (deletedNodes.Count > 0)
                {
                    deletedNodes.ForEach(nd => nd.Passable = true);
                    deletedNodes.Clear();
                }
            }
        }
    }
}