using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AStar;
using Astral.Logic.Classes.Map;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;
using static EntityTools.Patches.Mapper.MapperGraphicsHelper;

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper_RemoveNodes

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

            if (nodes.Count > 0 && mapper != null)
            {
                foreach (Node node in nodes)
                    graphics.drawSelectedNode(node);
            }

            if (drawSelectRegion)
            {
                //Vector3 topLeft = new Vector3(Math.Min(, currentWorldPos.X), Math.Max(startSelectRegionPos.Y, currentWorldPos.Y), 0f);
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
            graphics.FillCircleCentered(Brushes.White, node.Position, 14);
            graphics.FillCircleCentered(Brushes.Red, node.Position, 8);
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

                            // Очищаем выделенную группу вершин
                            nodes.Clear();
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Отмена выделения вершин
        /// </summary>
        /// <param name="e"></param>
        /// <param name="graphicsnw"></param>
        private static void handler_KeyUp(KeyEventArgs e, GraphicsNW graphicsnw)
        {
            if (e.KeyCode == Keys.Shift)
            {
                startSelectRegionPos.X = 0;
                startSelectRegionPos.Y = 0;
                startSelectRegionPos.Z = 0;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (var node in nodes)
                {
                    node.Passable = false;
                }
                nodes.Clear();
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
