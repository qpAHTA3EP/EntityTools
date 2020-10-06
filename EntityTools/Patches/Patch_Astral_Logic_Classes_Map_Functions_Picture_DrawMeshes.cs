using System;
using System.Drawing;
using System.Reflection;
using AStar;
using Astral.Logic.Classes.Map;
using MyNW.Classes;
using EntityTools.Reflection;

namespace EntityTools.Patches.Mapper
{
    /// <summary>
    /// Патч метода void Astral.Logic.Classes.Map.Functions.Picture.DrawMeshes(GraphicsNW graph, Graph meshes)
    /// </summary>

    internal class Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes : Patch
    {
        internal Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes()
        {
            MethodInfo mi = typeof(Astral.Logic.Classes.Map.Functions.Picture).GetMethod("DrawMeshes", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Logic_Classes_Map_Functions_Picture_DrawMeshes: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(DrawMeshes), ReflectionHelper.DefaultFlags);
        }


        /// <summary>
        /// Отрисовка графа путей <paramref name="meshes"/> на <paramref name="graphicsNW"/>
        /// </summary>
        /// <param name="graphicsNW"></param>
        /// <param name="meshes"></param>
        public static void DrawMeshes(GraphicsNW graphicsNW, Graph meshes)
        {
            if (graphicsNW is MapperGraphics mapGraphics)
            {
                lock (meshes.SyncRoot)
                {
                    mapGraphics.GetWorldPosition(0, 0, out double left, out double top);
                    mapGraphics.GetWorldPosition(mapGraphics.ImageWidth, mapGraphics.ImageHeight, out double right, out double down);

                    foreach (Node node in meshes.Nodes)
                    {
                        if (node.Passable)
                        {
                            // Отсеиваем и не отрисовываем вершины и ребра, расположенные за пределами видимого изображения
                            if (left <= node.X && node.X <= right
                                && down <= node.Y && node.Y <= top)
                            {
                                bool uniPath = node.IncomingArcsCount <= 1 && node.OutgoingArcsCount <= 1;
                                Brush brush = (uniPath) ? Brushes.SkyBlue : Brushes.Red;
                                Pen pen = (uniPath) ? Pens.SkyBlue : Pens.Red;

                                foreach (Arc arc in node.OutgoingArcs)
                                {
                                    if (arc.EndNode.Passable)
                                        mapGraphics.DrawLine(pen, node.X, node.Y, arc.EndNode.X, arc.EndNode.Y);
                                }
                                mapGraphics.FillCircleCentered(brush, node.Position, 5);
                            }
                        }
                    }
                }
            }
            else
            {
                lock (meshes.SyncRoot)
                {
                    double num = graphicsNW.getWorldPos(new Point(graphicsNW.ImageWidth, graphicsNW.ImageHeight)).Distance2D(graphicsNW.CenterPosition);
#if false
                    foreach (Arc arc in meshes.Arcs)
                    {
                        if (arc.StartNode.Passable && arc.EndNode.Passable)
                        {
                            Vector3 startPos = new Vector3((float)arc.StartNode.X, (float)arc.StartNode.Y, (float)arc.StartNode.Z);
                            Vector3 endPos = new Vector3((float)arc.EndNode.X, (float)arc.EndNode.Y, (float)arc.EndNode.Z);
                            if (startPos.Distance2D(graphicsNW.CenterPosition) < num)
                            {
                                graphicsNW.drawLine(startPos, endPos, Pens.Red);
                            }
                        }
                    } 
#endif

                    var nodeSize = new Size(5, 5);
                    foreach (Node node in meshes.Nodes)
                    {
                        if (node.Passable)
                        {
                            Brush brush = Brushes.Red;
                            Vector3 nodePos = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
                            if (nodePos.Distance2D(graphicsNW.CenterPosition) < num)
                            {
                                int inCount = node.IncomingArcsCount;
                                int outCount = node.OutgoingArcsCount;
                                if (inCount == 1 && outCount == 1)
                                    brush = Brushes.SkyBlue;

                                foreach (Arc arc in node.OutgoingArcs)
                                {
                                    if (arc.EndNode.Passable)
                                    {
                                        Vector3 endPos = new Vector3((float)arc.EndNode.X, (float)arc.EndNode.Y, (float)arc.EndNode.Z);
                                        graphicsNW.drawLine(nodePos, endPos, Pens.Red);
                                    }
                                }
#if true
                                foreach (Arc arc in node.IncomingArcs)
                                {
                                    if (arc.StartNode.Passable)
                                    {
                                        Vector3 startPos = new Vector3((float)arc.StartNode.X, (float)arc.StartNode.Y, (float)arc.StartNode.Z);
                                        graphicsNW.drawLine(startPos, nodePos, Pens.Red);
                                    }
                                }
#endif
                                graphicsNW.drawFillEllipse(nodePos, nodeSize, brush);
                            }
                        }
                    }
                } 
            }
        }
    }
}
