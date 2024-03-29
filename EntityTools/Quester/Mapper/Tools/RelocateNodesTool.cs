﻿using System;
using AStar;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using EntityTools.Quester.Mapper;

namespace EntityTools.Quester.Mapper.Tools
{
    /// <summary>
    /// Инструмент перемещения вершин
    /// </summary>
    public class RelocateNodesTool : IMapperTool
    {
        public RelocateNodesTool()
        {
            movedNodes = new List<Node>();
            selectedNodes = new LinkedList<Node>();
        }

        protected RelocateNodesTool(List<Node> moved, double dx, double dy)
        {
            if (moved is null)
                throw new ArgumentNullException(nameof(moved));
            movedNodes = moved;

            dX = dx;
            dY = dy;

            selectedNodes = new LinkedList<Node>();
        }

        /// <summary>
        /// Список перемещенных вершин
        /// </summary>
        readonly List<Node> movedNodes;

        /// <summary>
        /// Смещение координат
        /// </summary>
        readonly private double dX, dY;

        /// <summary>
        /// Набор выделенных вершин
        /// </summary>
        private readonly LinkedList<Node> selectedNodes;
        private int graphHash;

        // координаты начала области выделения вершин
        private double selectAreaStartX;
        private double selectAreaStartY;

        /// <summary>
        /// Использование механизма выделения вершин
        /// </summary>
        public bool AllowNodeSelection => false;

        /// <summary>
        /// Режим редактирования
        /// </summary>
        public MapperEditMode EditMode => MapperEditMode.RelocateNodes;

        public bool HandleCustomDraw => true;
        /// <summary>
        /// Отрисовка перемещаемых вершин
        /// </summary>
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool _, double worldMouseX, double worldMouseY)
        {
            switch (Control.ModifierKeys)
            {
                case Keys.Control:
                    // Отрисовываем только выбранные вершины
                    Draw_SelectedNodes(graphics, selectedNodes);
                    break;
                case Keys.Shift:
                    // Отрисовываем выбранные вершины
                    Draw_SelectedNodes(graphics, selectedNodes);
                    // Отрисовываем прямоугольник выделения
                    if (selectAreaStartX != 0 && selectAreaStartY != 0)
                        graphics.DrawRectangle(Pens.Gainsboro, selectAreaStartX, selectAreaStartY, worldMouseX, worldMouseY);
                    break;
                default:
                    if (selectedNodes?.Count > 0)
                    {
                        Node baseNode = selectedNodes.Last.Value;

                        if (baseNode != null)
                        {
                            // Вычисляем смещение
                            double dx = worldMouseX - baseNode.X;
                            double dy = worldMouseY - baseNode.Y;

                            // Отрисовываем выбранные вершины и их новое местоположение
                            Draw_SelectedNodes_Edges_NewNodesPositions(graphics, selectedNodes, dx, dy);
                        }
                    }
                    break;
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
        /// Отрисовка всех вершин <param name="nodes"/>
        /// </summary>
        private static void Draw_SelectedNodes(MapperGraphics graphics, LinkedList<Node> nodes)
        {
            var red = Brushes.Red;
            var orange = Brushes.Orange;
            foreach (var node in nodes)
            {
                var pos = node.Position;
                graphics.FillCircleCentered(red, pos, 14);
                graphics.FillCircleCentered(orange, pos); 
            }
        }

        /// <summary>
        /// Отрисовка вершин <param name="nodes"/> в исходном и в новом положении,
        /// а также всех их ребер
        /// </summary>
        private static void Draw_SelectedNodes_Edges_NewNodesPositions(MapperGraphics graphics, LinkedList<Node> nodes, double dx, double dy)
        {
            var redBrush = Brushes.Red;
            var orangeBrush = Brushes.Orange;
            var orangePen = Pens.Orange;

            foreach (var node in nodes)
            {
                var pos = node.Position;
                var x1 = pos.X + dx;
                var y1 = pos.Y + dy;

                //отрисовка вершины в новой позиции
                graphics.FillCircleCentered(orangeBrush, x1, y1, 16);
                graphics.FillCircleCentered(redBrush, x1, y1, 10);

                foreach (Arc arc in node.IncomingArcs)
                {
                    if (arc.StartNode.Passable)
                    {
                        var startNodePos = arc.StartNode.Position;
                        if (nodes.Contains(arc.StartNode))
                        {
                            double x2 = startNodePos.X + dx,
                                y2 = startNodePos.Y + dy;
                            graphics.DrawLine(orangePen, x1, y1, x2, y2);
                        }
                        else graphics.DrawLine(orangePen, x1, y1, startNodePos.X, startNodePos.Y);
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
                            graphics.DrawLine(orangePen, x1, y1, x2, y2);
                        }
                        else graphics.DrawLine(orangePen, x1, y1, endNodePos.X, endNodePos.Y);
                    }
                }

                //отрисовка вершины в исходной позиции
                graphics.FillCircleCentered(redBrush, pos, 14);
                graphics.FillCircleCentered(orangeBrush, pos);
            }
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool _, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape)
            {
                // Отключаем выделение регионом
                selectAreaStartX = 0;
                selectAreaStartY = 0;

                // Сбрасываем выделение
                selectedNodes.Clear();
                graphHash = 0;
            }
            else if (e.KeyCode == Keys.Shift)
            {
                selectAreaStartX = 0;
                selectAreaStartY = 0;
            }
            else if (e.KeyCode == Keys.Enter
            && selectedNodes.Count > 0)
            {
                // перемещение группу вершин
                Node baseNode = selectedNodes.Last.Value;

                if (baseNode != null)
                {
                    double dx = worldMouseX - baseNode.X;
                    double dy = worldMouseY - baseNode.Y;

                    MoveNodes(dx, dy, out undo); 
                }
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

            switch (Control.ModifierKeys)
            {
                // Нажата клавиша Shift - выделяем область
                case Keys.Shift when selectAreaStartX == 0 && selectAreaStartY == 0:
                    // Задаем начальную точку области выделения
                    selectAreaStartX = mouseX;
                    selectAreaStartY = mouseY;
                    break;
                case Keys.Shift:
                {
                    MapperHelper.FixRange(selectAreaStartX, mouseX, out double left, out double right);
                    MapperHelper.FixRange(selectAreaStartY, mouseY, out double down, out double top);

                    // Выделяем все вершины, охваченные областью выделения и добавляем в группу 
                    int hash = graph.GetHashCode();
                    
                    if (graphHash != hash)
                        selectedNodes.Clear();

                    foreach (Node nd in graph.NodesCollection)
                    {
                        var x = nd.X;
                        var y = nd.Y;
                        if (nd.Passable
                            && left <= x && x <= right
                            && down <= y && y <= top)
                            selectedNodes.AddLast(nd);
                    }

                    graphHash = hash;

                    // Сбрасываем
                    selectAreaStartX = 0;
                    selectAreaStartY = 0;
                    break;
                }
                case Keys.Control:
                {
                    double minDistance = EntityTools.Config.Mapper.WaypointEquivalenceDistance;
                    graph.ClosestNodeOxyProjection(mouseX, mouseY, out Node node, out int hash, minDistance);

                    if (node == null) return;

                    if (graphHash == hash)
                    {
                        // добавление и удаление вершины из(в) группу "перемещаемых"
                        if (selectedNodes.Contains(node))
                            // Вершина входит в группу "выделенных" и на неё кликнули повторно
                            // то есть она подлежит удалению из группы
                            selectedNodes.Remove(node);
                        else // Добавляем вершину в группу "выделенных"
                            selectedNodes.AddLast(node);
                    }
                    else
                    {
                        // сбрасываем выделение и выделяем одну вершину
                        selectedNodes.Clear();
                        selectedNodes.AddLast(node);
                        graphHash = hash;
                    }

                    break;
                }
                default:
                {
                    if(selectedNodes.Count > 0)
                    {
                        // перемещение группу вершин
                        Node baseNode = selectedNodes.Last?.Value;

                        if (baseNode != null)
                        {
                            double dx = e.X - baseNode.X;
                            double dy = e.Y - baseNode.Y;

                            MoveNodes(dx, dy, out undo); 
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Перемещение вершин, запоминание состояния для отката действия
        /// </summary>
        private void MoveNodes(double dx, double dy, out IMapperTool undo)
        {
            undo = null;
            if (selectedNodes.Count <= 0) return;

            var nodes = new List<Node>(selectedNodes.Count);
            foreach (var node in selectedNodes)
            {
                node.Move(dx, dy, 0);
                nodes.Add(node);
            }
            undo = new RelocateNodesTool(nodes, dx, dy);
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
            if (movedNodes.Count <= 0) return;

            movedNodes.ForEach(nd => nd.Move(-dX, -dY, 0));
            movedNodes.Clear();
        }
    }
}