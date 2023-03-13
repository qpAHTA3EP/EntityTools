using System;
using System.Collections;
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

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper_EditEdges
    {
#if AstralMapper
        private static Astral.Forms.UserControls.Mapper mapper; 
#else
        private static MapperExt mapper;
#endif
        private static Node startNode;
        private static Vector3 startNodePos = null;

        public static void Initialize(MapperExt m)
        {
#if false
            startNode = null;
            startNodePos = null;
            mapper = m;
            mapper.OnClick += handler_MapperClick;
            mapper.CustomDraw += handler_DrawSelectionOnMapper;
            mapper.OnMapperKeyUp += handler_KeyUp; 
#endif
        }

        private static void handler_DrawSelectionOnMapper(GraphicsNW g)
        {
            if (startNode != null && mapper != null)
            {
                Vector3 newPos = g.getWorldPos(mapper.RelativeMousePosition);

                if (startNodePos is null)
                    startNodePos = new Vector3((float) startNode.X, (float) startNode.Y,(float) startNode.Z);

                g.drawLine(startNodePos, newPos, Pens.Orange);

                var size = new Size(14, 14);
                g.drawFillEllipse(startNodePos, size, Brushes.Red);
                size.Height = 8;
                size.Width = 8;
                g.drawFillEllipse(startNodePos, size, Brushes.Orange);

#if false
                size.Height = 16;
                size.Width = 16;
                g.drawFillEllipse(newPos, size, Brushes.Orange);
                size.Height = 10;
                size.Width = 10;
                g.drawFillEllipse(newPos, size, Brushes.Red); 
#endif
            }
        }

        /// <summary>
        /// Клик на Mapper'e используемый для выбора стартовой и конечной вершины ребра
        /// </summary>
        /// <param name="me"></param>
        /// <param name="g"></param>
        private static void handler_MapperClick(MouseEventArgs me, GraphicsNW g)
        {
            if (me.Button == MouseButtons.Right)
            {
                Vector3 worldPos = g.getWorldPos(me.Location);
                if (startNode is null)
                {
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    foreach (Node node in ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                    {
                        if (node.Passable)
                        {
                            var dist = MathHelper.Distance2D((float)node.X, (float)node.Y, worldPos.X, worldPos.Y);
                            if (dist <= minDistance)
                            {
                                startNode = node;
                                minDistance = dist;
                            }
                        }
                    }

                    if(startNode != null)
                        startNodePos= new Vector3((float)startNode.X, (float)startNode.Y, (float)startNode.Z);
                }
                else
                {
                    double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                    Node endNode = null;
                    foreach (Node node in ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                    {
                        if (node.Passable)
                        {
                            var dist = MathHelper.Distance2D((float)node.X, (float)node.Y, worldPos.X, worldPos.Y);
                            if (dist <= minDistance)
                            {
                                endNode = node;
                                minDistance = dist;
                            }
                        }
                    }

                    if (endNode != null)
                    {
                        if (endNode != startNode)
                        {
                            // Проверяем наличие ребер между startNode и endNode
                            ArrayList arcs2Delete = new ArrayList();
                            foreach (Arc arc in startNode.OutgoingArcs)
                                if (arc.EndNode.Equals(endNode))
                                    arcs2Delete.Add(arc);
                            foreach (Arc arc in startNode.IncomingArcs)
                                if (arc.StartNode.Equals(endNode))
                                        arcs2Delete.Add(arc);

                            if (arcs2Delete.Count > 0)
                            {
                                // Удаляем имеющиеся ребра
                                ((Graph)AstralAccessors.Quester.Core.Meshes).RemoveArcs(arcs2Delete);
                            }
                            else
                            {
                                // Добавляем ребро, так как между выбранными вершинами нет рёбер
                                if (Control.ModifierKeys == Keys.Control)
                                    // добавляем ребро в прямом направлении
                                    ((Graph)AstralAccessors.Quester.Core.Meshes).AddArc(startNode, endNode, 1);
                                else // Добавляем ребро в прямо и в обратном направлении
                                     ((Graph)AstralAccessors.Quester.Core.Meshes).Add2Arcs(startNode, endNode, 1);
                            }

                            // Устанавливаем endNode в качестве "выбранной" вершины
                            startNode = endNode;
                            startNodePos = startNodePos = new Vector3((float)startNode.X, (float)startNode.Y, (float)startNode.Z);
                        }
                        else
                        {
                            // endNode совпадает с startNode,
                            // поэтому снимаем выделение
                            startNode = null;
                            startNodePos = null;
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
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                startNode = null;
                startNodePos = null;
            }
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
                mapper = null;
            }

            startNode = null;
            startNodePos = null; 
#endif
        }

    }
}
