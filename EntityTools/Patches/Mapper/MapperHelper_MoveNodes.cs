﻿using System;
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
using static EntityTools.Patches.Mapper.GraphicsNWExtensions;

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
            nodes.Clear();
            mapper = m;
            mapper.OnClick += handler_MapperClick;
            mapper.CustomDraw += handler_DrawSelectionOnMapper;
            mapper.OnMapperKeyUp += handler_KeyUp;
        }

        private static void handler_DrawSelectionOnMapper(GraphicsNW g)
        {
            Vector3 currentWorldPos = g.getWorldPos(mapper.RelativeMousePosition);

            bool drawSelectRegion = startSelectRegionPos.IsValid && (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            bool drawNewPoses = !drawSelectRegion && (Control.ModifierKeys & Keys.Control) != Keys.Control && (Control.ModifierKeys & Keys.Shift) != Keys.Shift;

            if (nodes.Count > 0 && mapper != null)
            {
                Node baseNode = nodes.Last();

                float dx = currentWorldPos.X - (float)baseNode.X;
                float dy = currentWorldPos.Y - (float)baseNode.Y;

                if (drawNewPoses)
                    foreach (Node node in nodes)
                        g.drawNewEdgePos(node, dx, dy); 

                foreach (Node node in nodes)
                    g.drawSelectedNode(node);

                if (drawNewPoses)
                    foreach (Node node in nodes)
                        g.drawNewNodePos(node, dx, dy);
            }

            if (drawSelectRegion)
            {
                Vector3 topLeft = new Vector3(Math.Min(startSelectRegionPos.X, currentWorldPos.X), Math.Max(startSelectRegionPos.Y, currentWorldPos.Y), 0f);
                Vector3 downRight = new Vector3(Math.Max(startSelectRegionPos.X, currentWorldPos.X), Math.Min(startSelectRegionPos.Y, currentWorldPos.Y), 0f);

                Vector3 selectRegionSize = new Vector3(downRight.X - topLeft.X, downRight.Y - topLeft.Y, 0f);

                g.drawRectangle(topLeft, selectRegionSize, Pens.PowderBlue);
            }
        }

        /// <summary>
        /// Отрисовка выбранной вершины
        /// </summary>
        /// <param name="node"></param>
        /// <param name="graphicsNW"></param>
        private static void drawSelectedNode(this GraphicsNW graphicsNW, Node node)
        {
            graphicsNW.drawFillEllipse(node.Position.AsVector3(), size14, Brushes.Red);
            graphicsNW.drawFillEllipse(node.Position.AsVector3(), size8, Brushes.Orange);
        }
        /// <summary>
        /// Нового положения вершины
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="graphicsNW"></param>
        private static void drawNewNodePos(this GraphicsNW graphicsNW, Node node, float dx, float dy)
        {
            Vector3 newPos = new Vector3((float)node.X + dx, (float)node.Y + dy, 0f);
            graphicsNW.drawFillEllipse(newPos, size16, Brushes.Orange);
            graphicsNW.drawFillEllipse(newPos, size10, Brushes.Red);
        }

        private static void drawNewEdgePos(this GraphicsNW graphicsNW, Node node, float dx, float dy)
        {
            Vector3 newPos = new Vector3((float)node.X + dx, (float)node.Y + dy, 0f);
            foreach (Arc arc in node.IncomingArcs)
            {
                if (arc.StartNode.Passable)
                {
                    if (nodes.IndexOf(arc.StartNode) >= 0)
                    {
                        Vector3 startNodeNewPos = new Vector3((float)arc.StartNode.X + dx, (float)arc.StartNode.Y + dy, 0f);
                        graphicsNW.drawLine(newPos, startNodeNewPos, Pens.Orange);
                    }
                    else graphicsNW.drawLine(newPos, arc.StartNode.Position.AsVector3(), Pens.Orange);
                }
            }
            foreach (Arc arc in node.OutgoingArcs)
            {
                if (arc.EndNode.Passable)
                {
                    if (nodes.IndexOf(arc.EndNode) >= 0)
                    {
                        Vector3 endNodeNewPos = new Vector3((float)arc.EndNode.X + dx, (float)arc.EndNode.Y + dy, 0f);
                        graphicsNW.drawLine(newPos, endNodeNewPos, Pens.Orange);
                    }
                    else graphicsNW.drawLine(newPos, arc.EndNode.Position.AsVector3(), Pens.Orange);
                }
            }
        }

        /// <summary>
        /// Клик на Mapper'e используемый для выбора вершины и её позиционирования
        /// </summary>
        /// <param name="me"></param>
        /// <param name="g"></param>
        private static void handler_MapperClick(MouseEventArgs me, GraphicsNW g)
        {
            if (me.Button == MouseButtons.Right)
            {
                Vector3 worldPos = g.getWorldPos(me.Location);

                if (Control.ModifierKeys == Keys.Shift)
                {
                    if (startSelectRegionPos.IsValid)
                    {
                        // Нажата клавиша Shift
                        // начинаем выделение регионом
                        Vector3 endSelectRegionPos = g.getWorldPos(mapper.RelativeMousePosition);

                        Vector3 downLeft = new Vector3(Math.Min(startSelectRegionPos.X, endSelectRegionPos.X), Math.Min(startSelectRegionPos.Y, endSelectRegionPos.Y), 0f);
                        Vector3 topRight = new Vector3(Math.Max(startSelectRegionPos.X, endSelectRegionPos.X), Math.Max(startSelectRegionPos.Y, endSelectRegionPos.Y), 0f);

                        // Выделяем все вершины, охваченные областью выделения и добавляем в группу 
                        foreach (Node nd in AstralAccessors.Quester.Core.Meshes.Value.Nodes)
                        {
                            if (nd.Passable
                                && downLeft.X <= nd.X && nd.X <= topRight.X
                                && downLeft.Y <= nd.Y && nd.Y <= topRight.Y)
                                    nodes.AddUnique(nd);
                        }

                        // Сбрасываем
                        startSelectRegionPos.X = 0;
                        startSelectRegionPos.Y = 0;
                        startSelectRegionPos.Z = 0;
                    }
                    else startSelectRegionPos = g.getWorldPos(mapper.RelativeMousePosition);

                }
                else
                {
                    Node node = null;
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    foreach (Node nd in ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                    {
                        if (nd.Passable)
                        {
                            var dist = MathHelper.Distance2D((float)nd.X, (float)nd.Y, worldPos.X, worldPos.Y);
                            if (dist <= minDistance)
                            {
                                node = nd;
                                minDistance = dist;
                            }
                        }
                    }

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

                            double dx = worldPos.X - baseNode.X;
                            double dy = worldPos.Y - baseNode.Y;

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
        /// <param name="graphicsnw"></param>
        private static void handler_KeyUp(KeyEventArgs e, GraphicsNW graphicsnw)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    var mousePos = mapper.RelativeMousePosition;
                    MouseEventArgs me = new MouseEventArgs(MouseButtons.Right, 1, mousePos.X, mousePos.Y, 0);
                    handler_MapperClick(me, mapper.GraphicsNW);
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
                        Vector3 worldPos = mapper.GraphicsNW.getWorldPos(mapper.RelativeMousePosition);

                        double dx = worldPos.X - baseNode.X;
                        double dy = worldPos.Y - baseNode.Y;

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
        }

        /// <summary>
        /// Сброс всех сохраненных значений и привязок
        /// </summary>
        internal static void Reset()
        {
            if (mapper != null && !mapper.IsDisposed)
            {
                mapper.OnClick -= handler_MapperClick;
                mapper.CustomDraw -= handler_DrawSelectionOnMapper;
                mapper.OnMapperKeyUp -= handler_KeyUp;
                mapper = null;
            }

            nodes.Clear();
        }

    }
}
