using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AStar;
using EntityTools.Tools.Extensions;

namespace EntityTools.Patches.Mapper.Tools
{
    /// <summary>
    /// Инструмент для удаления вершин
    /// </summary>
    public class EditEdgeTool : IMapperTool
    {
        /// <summary>
        /// Список добавленных/удаленных ребер
        /// Второй пары компонент указывает на произведенную с ним операцию:
        /// True - добавление 
        /// False - удаление 
        /// </summary>
        readonly LinkedList<Tuple<Arc, bool>> modifiedArcs = new LinkedList<Tuple<Arc, bool>>();

        private Node startNode;

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => true;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.EditEdges;

        public bool HandleCustomDraw => startNode != null;
        /// <summary>
        /// Отрисовка выделенной вершины
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (startNode != null)
            {
                double startX = startNode.X;
                double startY = startNode.Y;

                graphics.DrawLine(Pens.Orange, startX, startY, worldMouseX, worldMouseY);
                graphics.FillCircleCentered(Brushes.Red, startX, startY, 14);
                graphics.FillCircleCentered(Brushes.Orange, startX, startY, 8);
            }
        }

        /// <summary>
        /// Специальный курсор мыши
        /// </summary>
        public bool CustomMouseCusor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
        {
            text = string.Empty;
            textAlignment = Alignment.None;
            font = Control.DefaultFont;
            brush = Brushes.White;

            return false;
        }


        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                nodes.Clear();
                startNode = null;
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;
            if (e.Button == MouseButtons.Right
                 && nodes.Count > 0)
            {
                if (startNode is null)
                {
                    startNode = nodes.Last;
                }
                else
                {
                    var endNode = nodes.Last;
                    if (!startNode.Equals(endNode))
                    {
                        int arcsNum = 0;
                        foreach (Arc arc in startNode.OutgoingArcs)
                            if (!arc.Disabled && arc.EndNode.Equals(endNode))
                            {
                                arc.Disabled = true;
                                modifiedArcs.AddFirst(Tuple.Create(arc, false));
                                arcsNum++;
                            }
                        foreach (Arc arc in startNode.IncomingArcs)
                            if (!arc.Disabled && arc.StartNode.Equals(endNode))
                            {
                                arc.Disabled = true;
                                modifiedArcs.AddFirst(Tuple.Create(arc, false));
                                arcsNum++;
                            }

                        if (arcsNum == 0)
                        {
                            // Добавляем ребро, так как между выбранными вершинами нет рёбер
                            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                            // добавляем ребро в прямом направлении
                            {
                                Arc arc = graph.AddArc(startNode, endNode, 1);
                                arc.Disabled = false;
                                modifiedArcs.AddFirst(Tuple.Create(arc, true));
                            }
                            else
                            {
                                // Добавляем ребра в прямом и в обратном направлении
                                Arc arc = graph.AddArc(startNode, endNode, 1);
                                arc.Disabled = false;
                                modifiedArcs.AddFirst(Tuple.Create(arc, true));

                                arc = graph.AddArc(endNode, startNode, 1);
                                arc.Disabled = false;
                                modifiedArcs.AddFirst(Tuple.Create(arc, true));
                            }
                        }

                        if ((Control.ModifierKeys & Keys.Alt) != Keys.Alt)
                            startNode = endNode;
                    }
                    else
                    {
                        startNode = null;
                        nodes.Clear();
                    }

                }
                nodes.Clear();
            }
        }

        /// <summary>
        /// Указывает, что инструмент был применен
        /// </summary>
        public bool Applied => modifiedArcs.Count > 0;

        /// <summary>
        /// Откат изменений, внесенных инструментом
        /// </summary>
        public void Undo()
        {
            if (modifiedArcs.Count > 0)
            {
                startNode = null;
                modifiedArcs.ForEach(tuple => tuple.Item1.Disabled = tuple.Item2);
                modifiedArcs.Clear();
            }
        }
    }
}