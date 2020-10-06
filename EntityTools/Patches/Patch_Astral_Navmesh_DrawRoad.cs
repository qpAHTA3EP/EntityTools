using System;
using System.Collections.Generic;
using System.Reflection;
using Astral.Logic.Classes.Map;
using System.Drawing;
using MyNW.Classes;
using EntityTools.Reflection;

namespace EntityTools.Patches.Mapper
{
    internal class Patch_Astral_Navmesh_DrawRoad : Patch
    {
        internal Patch_Astral_Navmesh_DrawRoad()
        {
            MethodInfo mi = typeof(Astral.Logic.Navmesh).GetMethod("DrawRoad", ReflectionHelper.DefaultFlags);
            if (mi != null)
            {
                methodToReplace = mi;
            }
            else throw new Exception("Patch_Astral_Navmesh_DrawRoad: fail to initialize 'methodToReplace'");

            methodToInject = GetType().GetMethod(nameof(DrawRoad), ReflectionHelper.DefaultFlags);
        }


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
        public static void DrawRoad(List<Vector3> road, GraphicsNW graphicsNW)
        {
            if (graphicsNW is MapperGraphics mapGraphics)
            {
                mapGraphics.GetWorldPosition(0, 0, out double topLeftX, out double topLeftY);
                mapGraphics.GetWorldPosition(mapGraphics.ImageWidth, mapGraphics.ImageHeight, out double downRightX, out double downRightY);

                Brush brush = Brushes.Blue;
                Pen pen = new Pen(Color.Blue, 2);
                
                using (var wp = road.GetEnumerator())
                {
                    if(wp.MoveNext())
                    {
                        Vector3 startPos = wp.Current;
                        bool startVisible = false,
                            endVisible = false;
                        // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                        if (topLeftX <= startPos.X && startPos.X <= downRightX
                            && downRightY <= startPos.Y && startPos.Y <= topLeftY)
                        {
                            mapGraphics.FillCircleCentered(brush, startPos, 6);
                            startVisible = true;
                        }
                        while (wp.MoveNext())
                        {
                            Vector3 endPos = wp.Current;
                            // Отсеиваем и не отрисовываем точки пути, расположенные за пределами видимого изображения
                            if (topLeftX <= endPos.X && endPos.X <= downRightX
                                && downRightY <= endPos.Y && endPos.Y <= topLeftY)
                            {
                                mapGraphics.FillCircleCentered(brush, endPos, 6);
                                endVisible = true;
                            }
                            else endVisible = false;
                            if(startVisible || endVisible)
                                mapGraphics.DrawLine(pen, startPos.X, startPos.Y, endPos.X, endPos.Y);
                            startPos = endPos;
                            startVisible = endVisible;
                        }
                    }
                }   
            }
            else
            {
                Brush blue = Brushes.Blue;
                Vector3 startPos = new Vector3();
                foreach (Vector3 endPos in road)
                {
                    graphicsNW.drawFillEllipse(endPos, new Size(4, 4), blue);
                    if (startPos.IsValid)
                        graphicsNW.drawLine(startPos, endPos, Pens.Blue);
                    startPos = endPos;
                }
            }
        }
    }
}
