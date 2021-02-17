using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Astral.Logic.Classes.Map;
using EntityTools.Patches.Mapper;
using EntityTools.Reflection;
using MyNW.Classes;

namespace EntityTools.Patches.Navmesh
{
    internal class Patch_Astral_Logic_Navmesh_DrawRoad : Patch
    {

        private static readonly Brush defaultBrush = Brushes.Blue;
        private static readonly Pen defaultPen = new Pen(Color.Blue, 2);

        internal Patch_Astral_Logic_Navmesh_DrawRoad()
        {
            if (NeedInjecttion)
            {
                MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("DrawRoad", ReflectionHelper.DefaultFlags);
                if (mi != null)
                {
                    methodToReplace = mi;
                }
                else throw new Exception("Patch_Astral_Logic_Navmesh_DrawRoad: fail to initialize 'methodToReplace'");

                methodToInject = GetType().GetMethod(nameof(DrawRoad), ReflectionHelper.DefaultFlags, null, new Type[] { typeof(List<Vector3>), typeof(GraphicsNW) }, null);
            }
        }

        public sealed override bool NeedInjecttion => EntityTools.Config.Mapper.Patch;

#if false
    Astral.Logic.Navmesh
    	public static void DrawRoad(List<Vector3> waypoints, GraphicsNW graph)
		{
			Vector3 vector = new Vector3();
			foreach (Vector3 vector2 in waypoints)
			{
				Brush blue = Brushes.Blue;
				graph.drawFillEllipse(vector2, new Size(4, 4), blue);
				if (vector.IsValid)
				{
					graph.drawLine(vector, vector2, Pens.Blue);
				}
				vector = vector2;
			}
		}
#endif
        /// <summary>
        /// Отрисовка пути <paramref name="waypoints"/> на <paramref name="graphicsNW"/>
        /// </summary>
        public static void DrawRoad(List<Vector3> waypoints, GraphicsNW graphicsNW)
        {
            if (graphicsNW is MapperGraphics mapGraphics)
            {
                DrawRoad(waypoints, mapGraphics, defaultPen, defaultBrush);
            }
            else
            {
                Brush blue = Brushes.Blue;
                Vector3 startPos = new Vector3();
                foreach (Vector3 endPos in waypoints)
                {
                    graphicsNW.drawFillEllipse(endPos, MapperHelper.Size_4x4/*new Size(4, 4)*/, blue);
                    if (startPos.IsValid)
                        graphicsNW.drawLine(startPos, endPos, Pens.Blue);
                    startPos = endPos;
                }
            }
        }

        public static void DrawRoad(List<Vector3> waypoints, MapperGraphics mapGraphics, Pen pen, Brush brush)
        {
            mapGraphics.GetWorldPosition(0, 0, out double topLeftX, out double topLeftY);
            mapGraphics.GetWorldPosition(mapGraphics.ImageWidth, mapGraphics.ImageHeight, out double downRightX, out double downRightY);

            using (var wp = waypoints.GetEnumerator())
            {
                if (wp.MoveNext())
                {
#if Labeling_Waypoints
                        int i = 0; 
#endif
                    Vector3 startPos = wp.Current;
                    bool startVisible = false,
                        endVisible = false;
                    double x = startPos.X,
                           y = startPos.Y;
                    // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                    if (topLeftX <= x && x <= downRightX
                        && downRightY <= y && y <= topLeftY)
                    {
                        mapGraphics.FillCircleCentered(brush, x, y, 6);
#if Labeling_Waypoints
                            mapGraphics.DrawText(i.ToString(), x, y, Alignment.TopLeft, SystemFonts.DefaultFont, Brushes.White); 
#endif

                        startVisible = true;
                    }
#if Labeling_Waypoints
                        i++; 
#endif
                    while (wp.MoveNext())
                    {
                        Vector3 endPos = wp.Current;
                        x = endPos.X;
                        y = endPos.Y;
                        // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                        if (topLeftX <= x && x <= downRightX
                            && downRightY <= y && y <= topLeftY)
                        {
                            mapGraphics.FillCircleCentered(brush, x, y, 6);
#if Labeling_Waypoints
                                mapGraphics.DrawText(i.ToString(), x, y, Alignment.TopLeft, SystemFonts.DefaultFont, Brushes.White); 
#endif
                            endVisible = true;
                        }
                        else endVisible = false;
#if Labeling_Waypoints
                            i++; 
#endif
                        if (startVisible || endVisible)
                            mapGraphics.DrawLine(pen, startPos.X, startPos.Y, endPos.X, endPos.Y);
                        startPos = endPos;
                        startVisible = endVisible;
                    }
                }
            }
        }
    }
}
