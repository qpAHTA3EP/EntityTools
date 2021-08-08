using AcTp0Tools;
using AStar;
using Astral.Logic.Classes.FSM;
using DevExpress.XtraEditors;
using MyNW.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

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

                    MapperHelper.GetLabelAlignment(startX, startY, worldMouseX, worldMouseY, out startAlignment, out textAlignment);
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
                    MapperHelper.GetLabelAlignment(startX, startY, x, y, out Alignment startAlignment, out Alignment endAlignment);

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
                        ComplexPatch_Mapper.DrawRoad(road.Waypoints, graphics, Pens.RoyalBlue, Brushes.RoyalBlue);

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
                    var obj = MapperHelper.SelectObjectByPosition(e.X, e.Y, out double x, out double y, out double z,
                        EntityTools.Config.Mapper.WaypointEquivalenceDistance, graph);
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
                            MapperHelper.GetLabelAlignment(startX, startY, endX, endY, out startAlignment, out endAlignment);

                            double pathLen = 0;
                            DialogResult userAnswer = XtraMessageBox.Show("Generate road with Pathfinding?\n\r" +
                                "\tYes: Road generated by the Pathfinding\n\r" +
                                "\tNo: Road generated on the current meshes", "", MessageBoxButtons.YesNoCancel);
                            if (userAnswer != DialogResult.Cancel)
                            {
                                bool pathfinding = userAnswer == DialogResult.Yes;
                                var fullGraph = AstralAccessors.Quester.Core.Meshes;
                                road = ComplexPatch_Navigation.GenerateRoad(fullGraph, new Vector3((float)startX, (float)startY, (float)startZ), new Vector3((float)endX, (float)endY, (float)endZ), pathfinding);
                                if (road?.Waypoints.Count > 0)
                                    pathLen = ComplexPatch_Navigation.TotalDistance(road.Waypoints);
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
                    if (startX.Equals(0) && startY.Equals(0) && startZ.Equals(0))
                    {
                        startX = e.X;
                        startY = e.Y;
                        startZ = 0;
                        startLable = string.Concat("(x)", startX.ToString("N1"), "; (y)", startY.ToString("N1"));
                    }
                    else if (endX.Equals(0) && endY.Equals(0) && endZ.Equals(0))
                    {
                        endX = e.X;
                        endY = e.Y;
                        endZ = startZ;
                        MapperHelper.GetLabelAlignment(startX, startY, endX, endY, out startAlignment, out endAlignment);

                        double dist = Point3D.DistanceBetween(startX, startY, startZ, endX, endY, endZ);
                        endLable = string.Concat("(x)", endX.ToString("N1"), "; (y)", endY.ToString("N1"), Environment.NewLine,
                                "Distance (2D): ", dist.ToString("N1"));
                        road = null;
                    }
                } 
            }
        }

        public bool Applied => false;
        public void Undo() { }
    }
}
