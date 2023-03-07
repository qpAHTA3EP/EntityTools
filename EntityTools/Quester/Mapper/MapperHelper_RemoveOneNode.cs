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

namespace EntityTools.Patches.Mapper
{
    public static class MapperHelper_RemoveOneNode
    {
#if AstralMapper
        private static Astral.Forms.UserControls.Mapper mapper; 
#else
        private static MapperExt mapper;
#endif
        private static Node node = null;
        private static Vector3 nodePos = null;

        public static void Initialize(MapperExt m)
        {
            node = null;
            nodePos = null;
            mapper = m;
            mapper.OnClick += handler_MapperClick;
            mapper.CustomDraw += handler_DrawSelectedNode;
            mapper.OnMapperKeyUp += handler_KeyUp;
        }

        private static void handler_DrawSelectedNode(GraphicsNW g)
        {
            if (node != null && mapper != null)
            {
                if(nodePos is null)
                    nodePos = new Vector3((float)node.X, (float)node.Y, (float)node.Z);

                var size = new Size(14, 14);
                g.drawFillEllipse(nodePos, size, Brushes.Red);
                size.Height = 8;
                size.Width = 8;
                g.drawFillEllipse(nodePos, size, Brushes.White);
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
                Node nearestNode = null;
                double minDistance = EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance;
                foreach (Node n in ((Graph)AstralAccessors.Quester.Core.Meshes).Nodes)
                {
                    if (n.Passable)
                    {
                        var dist = MathHelper.Distance2D((float) n.X, (float) n.Y, worldPos.X, worldPos.Y);
                        if (dist <= minDistance)
                        {
                            nearestNode = n;
                            minDistance = dist;
                        }
                    }
                }

                if(node != null && nearestNode != null
                    && Point3D.DistanceBetween(node.Position, nearestNode.Position) <= EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance)
                {
                    node.Passable = false;
                    node = null;
                    nodePos = null;
                }
                else node = nearestNode;
            }
        }

        /// <summary>
        /// Отмена выделения вершины при нажатии клавиши 'Escape'
        /// Удаление при нажатии 'Delete'
        /// </summary>
        /// <param name="e"></param>
        /// <param name="graphicsnw"></param>
        private static void handler_KeyUp(KeyEventArgs e, GraphicsNW graphicsnw)
        {
            if (e.KeyCode == Keys.Delete)
            {
                node.Passable = false;
                node = null;
                nodePos = null;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                node = null;
                nodePos = null;
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
                mapper.CustomDraw -= handler_DrawSelectedNode;
                mapper = null;
            }

            node = null;
            nodePos = null;
        }

    }
}
