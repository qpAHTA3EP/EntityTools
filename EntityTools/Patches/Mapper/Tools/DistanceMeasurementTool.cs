﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AStar;
using Astral.Logic.Classes.FSM;
using DevExpress.XtraEditors;
using EntityTools.Patches.Navmesh;
using EntityTools.Reflection;
using MyNW.Classes;
using MyNW.Internals;

namespace EntityTools.Patches.Mapper.Tools
{
    public class DistanceMeasurementTool : IMapperTool
    {
        // координаты начальной точки
        private double startX;
        private double startY;
        private double startZ;
        private string startLable;
        Alignment startAlignment;

        // координаты конечной точки
        private double endX;
        private double endY;
        private double endZ;
        private string endLable;
        Alignment endAlignment;

        private Road road;

        public MapperEditMode EditMode => MapperEditMode.DistanceMeasurement;

        public bool AllowNodeSelection => false;

        public bool CustomMouseCusor(double worldMouseX, double worldMouseY, out string text, out Alignment textAlignment, out Font font, out Brush brush)
        {
            bool handleMouseCursor = false;
            text = string.Empty;
            textAlignment = Alignment.None;
            font = Control.DefaultFont;
            brush = Brushes.RoyalBlue;

            if (!(startX == 0 && startY == 0 && startZ == 0))
            {
                if (endX == 0 && endY == 0 && endZ == 0)
                {
                    double dist = Point3D.DistanceBetween(startX, startY, startZ, worldMouseX, worldMouseY, startZ);
                    text = string.Concat("(x)", worldMouseX.ToString("N1"), "; (y)", worldMouseY.ToString("N1"), Environment.NewLine,
                            "Distance (2D): ", dist.ToString("N1"));

                    MapperHelper.GetLableAlingment(startX, startY, worldMouseX, worldMouseY, out startAlignment, out textAlignment);
                    handleMouseCursor = true;
                }
            }

            return handleMouseCursor;
        }

        public bool HandleCustomDraw => true;
        public void OnCustomDraw(MapperGraphics graphics, NodeSelectTool nodes, double worldMouseX, double worldMouseY)
        {
            if (!(startX == 0 && startY == 0 && startZ == 0))
            {
#if false
                double x, y, z;
                string endLbl;

                if (endX == 0 && endY == 0 && endZ == 0)
                {
                    x = worldMouseX;
                    y = worldMouseY;
                    z = startZ;
                    double dist = Point3D.DistanceBetween(startX, startY, startZ, x, y, z);
                    endLbl = string.Concat(//"(x)", worldMouseX.ToString("N1"), "; (y)", worldMouseY.ToString("N1"), Environment.NewLine,
                            "Distance (2D): ", dist.ToString("N1"));
                }
                else
                {
                    x = endX;
                    y = endY;
                    z = endZ;
                    endLbl = endLable;

                    if (road?.Waypoints.Count > 0)
                        Patch_Astral_Logic_Navmesh_DrawRoad.DrawRoad(road.Waypoints, graphics, Pens.RoyalBlue, Brushes.RoyalBlue);
                }
                if (startAlignment == Alignment.None || endAlignment == Alignment.None)
                    MapperHelper.GetLableAlingment(startX, startY, x, y, out Alignment startAlignment, out Alignment endAlignment);

                graphics.FillCircleCentered(Brushes.RoyalBlue, startX, startY, 8);
                graphics.DrawText(startLable, startX, startY, startAlignment, Control.DefaultFont, Brushes.RoyalBlue);

                graphics.DrawLine(Pens.RoyalBlue, startX, startY, x, y);

                graphics.FillCircleCentered(Brushes.RoyalBlue, x, y, 8);
                graphics.DrawText(endLbl, x, y, endAlignment, Control.DefaultFont, Brushes.RoyalBlue);
#else
                double x, y;
                if (endX == 0 && endY == 0 && endZ == 0)
                {
                    x = worldMouseX;
                    y = worldMouseY;
                }
                else
                {
                    x = endX;
                    y = endY;

                    if (road?.Waypoints.Count > 0)
                        Patch_Astral_Logic_Navmesh_DrawRoad.DrawRoad(road.Waypoints, graphics, Pens.RoyalBlue, Brushes.RoyalBlue);

                    graphics.FillCircleCentered(Brushes.RoyalBlue, endX, endY, 8);
                    graphics.DrawText(endLable, endX, endY, endAlignment, Control.DefaultFont, Brushes.RoyalBlue);
                }

                graphics.DrawLine(Pens.AliceBlue, startX, startY, x, y);

                graphics.FillCircleCentered(Brushes.RoyalBlue, startX, startY, 8);
                graphics.DrawText(startLable, startX, startY, startAlignment, Control.DefaultFont, Brushes.RoyalBlue);
#endif
            } 
        }

        public bool HandleKeyUp => true;
        public void OnKeyUp(IGraph graph, NodeSelectTool nodes, KeyEventArgs e, double worldMouseX, double worldMouseY, out IMapperTool undo)
        {
            undo = null;
            if (e.KeyCode == Keys.Escape)
            {
                if (endX == 0 && endY == 0 && endZ == 0)
                {
                    startX = 0;
                    startY = 0;
                    startZ = 0;
                    startLable = string.Empty;
                    startAlignment = Alignment.None;
                }
                else
                {
                    endX = 0;
                    endY = 0;
                    endZ = 0;
                    road = null;
                    endLable = string.Empty;
                    endAlignment = Alignment.None;
                }
            }
        }

        public bool HandleMouseClick => true;
        public void OnMouseClick(IGraph graph, NodeSelectTool nodes, MapperMouseEventArgs e, out IMapperTool undo)
        {
            undo = null;

            if (e.Button == MouseButtons.Right)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    var obj = SelectObjectByPosition(e.X, e.Y, out double x, out double y, out double z,
                        EntityTools.PluginSettings.Mapper.WaypointEquivalenceDistance, graph);
                    if (obj != null)
                    {
                        if (startX == 0 && startY == 0 && startZ == 0)
                        {
                            startX = x;
                            startY = y;
                            startZ = z;
                            startLable = string.Concat("(x)", startX.ToString("N1"), "; (y)", startY.ToString("N1"), "; (z)", startZ.ToString("N1"));
                        }
                        else if (endX == 0 && endY == 0 && endZ == 0)
                        {
                            endX = x;
                            endY = y;
                            endZ = z;
                            double dist2D = Point3D.DistanceBetween(startX, startY, startZ, endX, endY, endZ);
                            MapperHelper.GetLableAlingment(startX, startY, endX, endY, out startAlignment, out endAlignment);

                            double pathLen = 0;
                            DialogResult userAnswer = XtraMessageBox.Show("Generate road with Pathfinding?\n\r" +
                                "\tYes: Road generated by the Pathfinding\n\r" +
                                "\tNo: Road generated on the current meshes", "", MessageBoxButtons.YesNoCancel);
                            if (userAnswer != DialogResult.Cancel)
                            {
                                bool pathfinding = userAnswer == DialogResult.Yes;
                                var fullGraph = AstralAccessors.Quester.Core.Meshes.Value;
                                road = Patch_Astral_Logic_Navmesh_GenerateRoad.GenerateRoad(fullGraph, new Vector3((float)startX, (float)startY, (float)startZ), new Vector3((float)endX, (float)endY, (float)endZ), pathfinding);
                                if (road?.Waypoints.Count > 0)
                                    pathLen = Patch_Astral_Logic_Navmesh_TotalDistance.TotalDistance(road.Waypoints);
                            }
                            if (pathLen > 0)
                                endLable = string.Concat("(x)", endX.ToString("N1"), "; (y)", endY.ToString("N1"), "; (z)", endZ.ToString("N1"), Environment.NewLine,
                                                         "Path length: ", pathLen.ToString("N1"), Environment.NewLine,
                                                         "Distance (3D): ", dist2D.ToString("N1"));
                            else endLable = string.Concat("(x)", endX.ToString("N1"), "; (y)", endY.ToString("N1"), "; (z)", endZ.ToString("N1"), Environment.NewLine,
                                                          "Distance (3D): ", dist2D.ToString("N1"));
                            ETLogger.WriteLine(LogType.Log, string.Concat(nameof(DistanceMeasurementTool), ": from ", startLable, " to ", endLable));
                        }
                    }
                }
                else
                {
                    if (startX == 0 && startY == 0 && startZ == 0)
                    {
                        startX = e.X;
                        startY = e.Y;
                        startZ = 0;
                        startLable = string.Concat("(x)", startX.ToString("N1"), "; (y)", startY.ToString("N1"));
                    }
                    else if (endX == 0 && endY == 0 && endZ == 0)
                    {
                        endX = e.X;
                        endY = e.Y;
                        endZ = startZ;
                        MapperHelper.GetLableAlingment(startX, startY, endX, endY, out startAlignment, out endAlignment);

                        double dist = Point3D.DistanceBetween(startX, startY, startZ, endX, endY, endZ);
                        endLable = string.Concat("(x)", endX.ToString("N1"), "; (y)", endY.ToString("N1"), Environment.NewLine,
                                "Distance (2D): ", dist.ToString("N1"));
                        road = null;
                    }
                } 
            }
        }

        /// <summary>
        /// Поиск объекта, проекция которого на Oxy расположена не далее <paramref name="distance"/> 
        /// от точки, заданной координатами <paramref name="worldX"/>, <paramref name="worldY"/>
        /// </summary>
        private object SelectObjectByPosition(double worldX, double worldY, out double x, out double y, out double z, double distance = -1, IGraph graph = null)
        {
            if(distance <= 0)
                distance = double.MaxValue;

            if (graph is null)
                graph = AstralAccessors.Quester.Core.Meshes.Value;

            Node node = graph.ClosestNodeOxyProjection(worldX, worldY, distance);
            if(node != null)
            {
                var pos = node.Position;
                x = pos.X;
                y = pos.Y;
                z = pos.Z;
                return node;
            }

            var entities = EntityManager.GetEntities();
            var ent = entities.ClosestNodeOxyProjection(worldX, worldY, (e) => e.Location, out x, out y, out z, distance);
            if(ent != null)
            {
                return ent;
            }

            var targetableNodes = EntityManager.LocalPlayer.Player.InteractStatus.TargetableNodes;
            var targetNode = targetableNodes.ClosestNodeOxyProjection(worldX, worldY, (nd) => nd.WorldInteractionNode.Location, out x, out y, out z, distance);
            if (targetNode != null)
                return targetNode;

            x = 0; y = 0; z = 0;
            return null;
        }

        public bool Applied => false;
        public void Undo() { }
    }
}
